using MobuWcf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Configuration;

namespace MobuWcf.Controls
{
    /// <summary>
    /// Control used to save, delete and update hosted APK files.
    /// </summary>
    public static class FileControl
    {
        /// <summary>
        /// Wait handle to ensure a file is not being accessed by more than one process at the same time.
        /// </summary>
        static EventWaitHandle waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "{56A06607-88B1-4F2E-9671-CF48F60B2E47}");

        /// <summary>
        /// Folder location on the server where APK files are stored.
        /// This is defaulted if no path has been setup in config.
        /// </summary>
        private static string PackageFolderLocation
        {
            get
            {
                try
                {
                    var loc = WebConfigurationManager.AppSettings["MobuPackagePath"];

                    if (loc != null && !String.IsNullOrEmpty(loc))
                    {
                        return loc;
                    }
                    else
                    {
                        return "C:\\Mobu\\Packages";
                    }

                }
                catch
                {
                    return "C:\\Mobu\\Packages";
                }
            }
        }

        /// <summary>
        /// Returns a list of hosted apks.
        /// APK details are parsed out of the filename.
        /// </summary>
        public static List<PackageDetails> GetPackageList()
        {
            var packages = new List<PackageDetails>();

            try
            {
                if (!Directory.Exists(PackageFolderLocation))
                {
                    Directory.CreateDirectory(PackageFolderLocation);
                }
                else
                {
                    var files = Directory.GetFiles(PackageFolderLocation).ToList();

                    foreach(var file in files)
                    {
                        packages.Add(new PackageDetails(file));
                    }
                }
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while retrieving package list. " + PackageFolderLocation, e);
            }

            return packages;
        }

        /// <summary>
        /// Returns a specific APK file.
        /// </summary>
        /// <param name="path">FIle path of the requested APK.</param>
        /// <returns></returns>
        public static byte[] GetPackage(String path)
        {
            try
            {
                waitHandle.WaitOne();
                if (File.Exists(path))
                {
                    return File.ReadAllBytes(path);
                }
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while retrieving package", e);
            }
            finally
            {
                waitHandle.Set();
            }

            return null;
        }

        /// <summary>
        /// Saves an aPK to file on the server.
        /// </summary>
        /// <param name="packageKey">Filename of the APK containing appname, version and whether it is a critical update or not.</param>
        /// <param name="file">Byte array containing APK data</param>
        /// <returns></returns>
        public static String SavePackage(String packageKey, byte[] file)
        {
            try
            {
                waitHandle.WaitOne();

                if (!Directory.Exists(PackageFolderLocation))
                {
                    Directory.CreateDirectory(PackageFolderLocation);
                }

                var fileName = Path.Combine(PackageFolderLocation, packageKey);

                File.WriteAllBytes(fileName, file);

                return fileName;
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while saving package " + packageKey + ".", e);
            }
            finally
            {
                waitHandle.Set();
            }

            return String.Empty;
        }

        /// <summary>
        /// Deletes an APK file from the server/service.
        /// </summary>
        /// <param name="packageDetails">Details of the APK to be deleted.</param>
        public static bool DeletePackage(PackageDetails packageDetails)
        {
            try
            {
                waitHandle.WaitOne();

                var packageKey = packageDetails.GetPackageKey();
                var path = GetPackagePath(packageKey);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                else
                {
                    LoggingControl.Log("Error while deleting package " + packageDetails.Name + " " + packageDetails.Version + ". Could not find file.");
                    return false;
                }

                return !File.Exists(path);
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while deleting package " + packageDetails.Name + " " + packageDetails.Version + ".", e);
                return false;
            }
            finally
            {
                waitHandle.Set();
            }
        }

        /// <summary>
        /// Updates the filename of an APK to indicate whether it is a critical update or not.
        /// Apps updated through mobu canspecify whether they always get the latest version of an APK or only newer APKs marked as critical.
        /// This makes it possible allow testers to manually get new versions without automatically pushing it to all users.
        /// </summary>
        /// <param name="packageDetails">Details of the package that should be toggled.</param>
        /// <returns></returns>
        public static bool ToggleCriticalState(PackageDetails packageDetails)
        {
            try
            {
                waitHandle.WaitOne();

                var packageKey = packageDetails.GetPackageKey();
                var oldPath = GetPackagePath(packageKey);
                var newPath = oldPath.Remove(oldPath.Length - 1);

                if (File.Exists(oldPath))
                {
                    if (oldPath[oldPath.Length - 1] == '1')
                    {
                        newPath += '0';
                    }
                    else
                    {
                        newPath += '1';
                    }

                    File.Move(oldPath, newPath);
                }
                else
                {
                    LoggingControl.Log("Error while toggling critical state for " + packageDetails.Name + " " + packageDetails.Version + ". Could not find file."); ;
                    return false;
                }

                return File.Exists(newPath) && !File.Exists(oldPath);
            }
            catch (Exception e)
            {
                LoggingControl.Log("Error while toggling critical state for " + packageDetails.Name + " " + packageDetails.Version + ".", e);
                return false;
            }
            finally
            {
                waitHandle.Set();
            }
        }

        /// <summary>
        /// Returns the full file path of a specific APK.
        /// </summary>
        /// <param name="packageKey">The filename key of the package in question. This contains the name, version and critical state of the APK.</param>
        public static String GetPackagePath(String packageKey)
        {
            var folder = PackageFolderLocation;

            return Path.Combine(folder, packageKey);
        }
    }
}

