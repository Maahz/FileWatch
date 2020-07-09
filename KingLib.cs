using System;
using System.IO;
using System.Security.Cryptography;



namespace FileWatch
{
    class KingLib
    {
        /// <summary>
        /// Better Copy Function with checksum
        /// </summary>
        /// <param name="fromPath">What to copy</param>
        /// <param name="toPath">Where to copy</param>
        /// <returns>True if checksum completes</returns>
        public static bool CopyFile(string fromPath, string toPath)
        {
            System.Threading.Thread.Sleep(1);
            try
            {
                if (!File.Exists(toPath))
                    File.Copy(fromPath, toPath);

                if (CompareChecksum(fromPath, toPath))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }


        }

        /// <summary>
        /// Add timecoded string to your log file.
        /// </summary>
        /// <param name="logEntry">String you want to add</param>
        /// <param name="logName">Path and/or file name to log file.</param>
        public static void Logger(string logEntry, string logName = "eventLog.txt")
        {
            StreamWriter file = File.AppendText(logName);
            string logString = DateTime.Now + " : " + logEntry;
            file.Write(logString + "\n");

            file.Close();

        }

        #region "Checksum"
        /// <summary>
        /// Compare checksum of files
        /// </summary>
        /// <param name="firstPath">First file</param>
        /// <param name="secondPath">Second file</param>
        /// <param name="reporting">If you want to get checksum result feedback true or false</param>
        /// <returns>If checksum matches returns bool true</returns>
        private static bool CompareChecksum(string firstPath, string secondPath, bool reporting = false)
        {
            string firstChecksum = GetChecksum(firstPath);
            string secondChecksum = GetChecksum(secondPath);
            if (reporting)
            {
                Console.WriteLine("First Checksum: " + firstChecksum);
                Console.WriteLine("Second Checksum: " + secondChecksum);
            }
            if (firstChecksum != secondChecksum)
                throw new ChecksumMismatch();

            return true;

        }

        private static string GetChecksum(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }

        }
        #endregion
        public static void FileWatch(string path, string filter = "*")
        {
            using (FileSystemWatcher allSeeingEye = new FileSystemWatcher())
            {
                allSeeingEye.Path = path;
                allSeeingEye.NotifyFilter = NotifyFilters.Attributes |
                                            NotifyFilters.CreationTime |
                                            NotifyFilters.FileName |
                                            NotifyFilters.LastAccess |
                                            NotifyFilters.LastWrite |
                                            NotifyFilters.Size |
                                            NotifyFilters.Security;
                allSeeingEye.Filter = filter;
                allSeeingEye.Created += OnCreated;
                allSeeingEye.EnableRaisingEvents = true;
                while (Console.Read() != 'q') ;
            }
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Event detected");
        }
    }

    #region "ErrorMessages"
    [Serializable]
    class ChecksumMismatch : Exception
    {
        public ChecksumMismatch()
        {

        }
    }
    #endregion
}
