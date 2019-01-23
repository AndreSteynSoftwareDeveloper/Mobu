using System.ComponentModel;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System;
using MobuUpload.MobuWs;
using MobuUpload.Controls;
using MobuUpload.Models;
using System.ServiceModel;

namespace MobuUpload
{
    /// <summary>
    /// Control class used to keep track of which APKs are currently hosted on the web service and also remove or update those APKs from/on the backend UI and the server.
    /// </summary>
    public class MonitorControl
    {
        #region Public Properties
        /// <summary>
        /// Background worker wich constantly checks connectivity to the Mobu web service.
        /// </summary>
        public WebMonitor WebMonitor;

        /// <summary>
        /// Background worker wich constantly checks which APKS are currently hosted by the web service.
        /// </summary>
        public PackageMonitor PackageMonitor;

        /// <summary>
        /// Control object used to interact with application settings.
        /// </summary>
        public SettingsControl SettingsControl;

        /// <summary>
        /// Object containing the current app settings
        /// </summary>
        public Settings Settings;

        /// <summary>
        /// Boolean indicating wether the web service is reachable using the current settings.
        /// </summary>
        public bool WebServiceStatus;

        /// <summary>
        /// List of details of APKs currently hosted by the web service.
        /// </summary>
        public List<PackageDetails> PackageList;

        /// <summary>
        /// The interval in milleseconds between checks by the monitor workers.
        /// </summary>
        public int PollRate = 5000;

        /// <summary>
        /// The interval in milleseconds between checks by the monitor workers.
        /// </summary>
        public int Timeout = 5000;

        /// <summary>
        /// Reference to the mobu web service
        /// </summary>
        public MobuWsClient WebService;
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor for the monitor control.
        /// </summary>
        /// <param name="settings">Current app settings</param>
        public MonitorControl(Settings settings)
        {
            Settings = settings;
            PackageList = new List<PackageDetails>();                                                                                                                     

            WebService = new MobuWsClient();
            WebService.Endpoint.Address = new EndpointAddress(Settings.WebServiceUrl);

            if (settings.WebServiceUrl.StartsWith("https"))
            {
                var binding = new BasicHttpsBinding();
                binding.Security.Mode = BasicHttpsSecurityMode.Transport;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
                WebService.Endpoint.Binding = binding;
            }
            else
            {
                var binding = new BasicHttpBinding();
                WebService.Endpoint.Binding = binding;
            }

            var timeout = new TimeSpan(0,0,0,0,Settings.WebServiceTimeout);

            WebService.Endpoint.Binding.OpenTimeout = timeout;
            WebService.Endpoint.Binding.CloseTimeout = timeout;
            WebService.Endpoint.Binding.SendTimeout = timeout;
            WebService.Endpoint.Binding.ReceiveTimeout = timeout;


            WebMonitor = new WebMonitor(this);
            PackageMonitor = new PackageMonitor(this, WebService);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns true if web service is reachable.
        /// </summary>
        public bool TestWebService()
        {
            try
            {
                return WebService.TestWebService();
            }
            catch 
            {
                return false;
            }
        }

        /// <summary>
        /// Calls the web service to toggle the critical state of an APK.
        /// </summary>
        /// <param name="packageDetails">Package details of the APK being updated</param>
        public bool ToggleCritical(PackageDetails packageDetails)
        {
            return WebService.TogglePackageCriticalState(packageDetails);
        }

        /// <summary>
        /// Calls the web service to remove an APK from the service.
        /// </summary>
        /// <param name="packageDetails">Package details of the APK being updated</param>
        public void RemovePackage(PackageDetails packageDetails)
        {
            if (WebService.RemovePackage(packageDetails))
            {
                PackageList.RemoveAll(x => x.Name == packageDetails.Name && x.Version == packageDetails.Version);
            }
        }

        /// <summary>
        /// Returns a copy of the package list o be used to populate the UI.
        /// </summary>
        public List<PackageDetails> CopyPackageList()
        {
            var packageList = new List<PackageDetails>();

            foreach (var i in PackageList)
            {
                packageList.Add(new PackageDetails
                {
                    Name = i.Name,
                    Version = i.Version,
                    Critical = i.Critical
                });
            }

            return packageList;
        }
        #endregion
    }

    #region Monitor Workers

    /// <summary>
    /// Background worker constantly checking if web service is reachable using current settings.
    /// </summary>
    public class WebMonitor : BackgroundWorker
    {
        /// <summary>
        /// Reference to parent monitor control class.
        /// </summary>
        private MonitorControl monitorControl;

        /// <summary>
        /// Default constructor for web monitor.
        /// </summary>
        /// <param name="ctrMonitor">Reference to parent monitor control class.</param>
        public WebMonitor(MonitorControl ctrMonitor)
        {
            this.monitorControl = ctrMonitor;
            WorkerSupportsCancellation = true;
            RunWorkerAsync();
        }

        /// <summary>
        /// Work event checking connectivity to the web service.
        /// </summary>
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            while (!CancellationPending)
            {
                monitorControl.WebServiceStatus = monitorControl.TestWebService();
                Thread.Sleep(monitorControl.PollRate);
            }

            Dispose();
        }
    }

    /// <summary>
    /// Background worker constantly updating a local list of hosted packages to be displayed by tu UI.
    /// </summary>
    public class PackageMonitor : BackgroundWorker
    {
        /// <summary>
        /// Reference to parent monitor control class.
        /// </summary>
        private MonitorControl monitorControl;

        /// <summary>
        /// Default constructor for package monitor.
        /// </summary>
        /// <param name="ctrMonitor">Reference to parent monitor control class.</param>
        public PackageMonitor(MonitorControl ctrMonitor, MobuWsClient webService)
        {
            this.monitorControl = ctrMonitor;
            WorkerSupportsCancellation = true;
            RunWorkerAsync();
        }

        /// <summary>
        /// Work event updating the package list.
        /// </summary>
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            while (!CancellationPending)
            {
                try
                {
                    var packages = monitorControl.WebService.GetPackageList();

                    packages = packages.OrderByDescending(x => x.Name).ThenBy(x => x.Version).ToList();

                    monitorControl.PackageList = packages.ToList();
                }
                catch
                {
                    monitorControl.PackageList = new List<PackageDetails>();
                }

                Thread.Sleep(monitorControl.PollRate);
            }

            Dispose();
        }
    }

    #endregion
}


