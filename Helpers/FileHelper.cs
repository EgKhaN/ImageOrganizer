using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ImageOrganizer.Helpers
{
    /// <summary>
    /// I would've filled this more,for now put just for Seperation Of Concerns
    /// </summary>
    public static class FileHelper
    {
        public static void DeleteFilesIfExists(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : ", ex.Message);
            }
        }

        public static string GenerateFileDestination(string dest, int index)
        {
            var root = Path.GetFullPath(dest);
            return @$"{root}/{index}.png";
        }

        public static string GenerateFileSourceURL(string address, int index)
        {
            return address + index;
        }

        [Obsolete]
        public static void DownloadProgressCallback4(object sender, DownloadProgressChangedEventArgs e)
        {
            // Displays the operation identifier, and the transfer progress.
            Console.WriteLine("{0}    downloaded {1} of {2} bytes. {3} % complete...",
                (string)e.UserState,
                e.BytesReceived,
                e.TotalBytesToReceive,
                e.ProgressPercentage);
        }
    }
}
