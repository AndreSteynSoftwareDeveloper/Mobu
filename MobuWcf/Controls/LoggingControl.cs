using System;
using System.IO;
using System.Threading;
using System.Web.Configuration;

namespace MobuWcf.Controls
{
    /// <summary>
    /// Utility to easily log errors and messages for this service to a daily file on the server.
    /// </summary>
    public static class LoggingControl
    {
        /// <summary>
        /// Wait handle to ensure a file is not being accessed by more than one process at the same time.
        /// </summary>
        static EventWaitHandle waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "{ECB16D76-87E0-48C9-A10C-F30D3FBF1545}");

        /// <summary>
        /// Folder location on the server where log files are stored.
        /// This is defaulted if no path has been setup in config.
        /// </summary>
        private static string LogFolderLocation
        {
            get
            {
                try
                {
                    var loc = WebConfigurationManager.AppSettings["MobuLogPath"];

                    if (loc != null && !String.IsNullOrEmpty(loc))
                    {
                        return loc;
                    }
                    else
                    {
                        return "C:\\Mobu\\Logs";
                    }

                }
                catch
                {
                    return "C:\\Mobu\\Logs";
                }
            }
        }

        /// <summary>
        /// Add a log message to a daily file.
        /// </summary>
        /// <param name="message">The message to be added to the log.</param>
        public static void Log(String message)
        {
            try
            {
                waitHandle.WaitOne();
                if (!Directory.Exists(LogFolderLocation))
                {
                    Directory.CreateDirectory(LogFolderLocation);
                }

                var logFileName = Path.Combine(LogFolderLocation, DateTime.Today.ToShortDateString() + ".txt");

                using (StreamWriter sw = new StreamWriter(logFileName, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " " + message);
                }
            }
            catch
            {
                //This is just for debugging and should never happen.
            }
            finally
            {
                waitHandle.Set();
            }
        }

        /// <summary>
        /// Add a log message and exception message to a daily file.
        /// </summary>
        /// <param name="message">The message to be added to the log.</param>
        /// <param name="e">Exception to be logged.</param>
        public static void Log(String message, Exception e)
        {
            try
            {
                waitHandle.WaitOne();
                if (!Directory.Exists(LogFolderLocation))
                {
                    Directory.CreateDirectory(LogFolderLocation);
                }

                var logFileName = Path.Combine(LogFolderLocation, DateTime.Today.ToString("yyyyMMdd")) + ".txt";

                using (StreamWriter sw = new StreamWriter(logFileName, true))
                {
                    sw.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " " + message + " " + e.Message + " ");
                }
            }
            catch
            {
                //This is just for debugging and should never happen.
            }
            finally
            {
                waitHandle.Set();
            }
        }
    }
}
