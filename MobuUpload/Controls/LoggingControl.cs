using System;
using System.IO;
using System.Threading;

namespace MobuUpload.Controls
{
    /// <summary>
    /// Utility to easily log errors and messages
    /// </summary>
    public static class LoggingControl
    {
        /// <summary>
        /// Wait handle to ensure a file is not being accessed by more than one process at the same time.
        /// </summary>
        static EventWaitHandle waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "{C965107C-A758-4093-972E-6B736C513D13}");

        /// <summary>
        /// Folder location where log files are stored.
        /// </summary>
        private static string LogFolderLocation
        {
            get
            {
                var defaultLogFolderLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                return defaultLogFolderLocation;
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
