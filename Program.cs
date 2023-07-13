using ImageOrganizer.Helpers;
using ImageOrganizer.Models;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net;
using System.Text;

namespace ImageOrganizer
{
    internal class Program
    {
        #region GLOBAL VARS
        static InputModel inputModel = new InputModel();
        static int downloadedCount = 0;
        static List<string> downladedFilePaths = new List<string>();
        static bool readFromFile = false;

        const string BASE_IMAGE_SITE_URL = "https://picsum.photos/200/300?random=";
        const string NUM_OF_IMAGES_MESSAGE = "Enter the number of images to download";
        const string NUM_OF_PAR_DOWN_LiM_MESSAGE = "Enter the maximum parallel download limit";
        const string SAVE_PATH_MESSAGE = "Enter the save path (default: ./outputs)";
        const string DOWNLOADING_MESSAGE = "Downloading {0} images ({1} parallel downloads at most)";
        const string PROGRESS_MESSAGE = "Progress: {0}/{1}";
        const string INPUT_SOURCE_SELECTION_MESSAGE = "Do you want to read configurations from file(Y/N)?";
        const string SYSTEM_OUTPUT_SEPERATOR = "$$$";

        const int delayBetweenParallels = 1500;


        #endregion

        static async Task Main(string[] args)
        {
            InitConsoleSettings();

            try
            {
                ConfigInputSelections();

                if (inputModel.Count > 0)
                {
                    //start downloading images
                    await DownloadImages();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

        }

        private static void ConfigInputSelections()
        {
            ReadInputSelection();

            if (readFromFile)
            {
                ReadConfigsFromInputFile();
            }
            else
            {
                Console.Clear();
                //gets total image count to be downladed
                GetTotalCount();
                //get parallel process count for downlading
                GetParallelCount();
                //get output folder
                GetDestinationFolder();
            }
        }

        private static void InitConsoleSettings()
        {
            //subscribe to Console Cancel event
            Console.CancelKeyPress += Console_CancelKeyPress;
        }

        private static void ReadConfigsFromInputFile()
        {
            string inputFilePath = @$"{Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName}/Data/Input.json";
            using (StreamReader r = new StreamReader(inputFilePath))
            {
                string json = r.ReadToEnd();
                InputModel model = JsonConvert.DeserializeObject<InputModel>(json);
                if (model != null)
                    inputModel = model;
            }
        }

        private static void ReadInputSelection()
        {
            string inputSourceSelection;
            do
            {
                Console.Clear();
                Console.WriteLine(INPUT_SOURCE_SELECTION_MESSAGE);
                inputSourceSelection = Console.ReadLine();
                if (!string.IsNullOrEmpty(inputSourceSelection) && (inputSourceSelection.ToLower() == "y" || inputSourceSelection.ToLower() == "n"))
                {
                    readFromFile = inputSourceSelection.ToLower() == "y";
                }
                else
                    inputSourceSelection = null;

            } while (inputSourceSelection == null);
        }

        private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs args)
        {
            foreach (var path in downladedFilePaths)
            {
                FileHelper.DeleteFilesIfExists(path);
            }

            Console.WriteLine("\n" + SYSTEM_OUTPUT_SEPERATOR);
            Console.WriteLine("Canceled");
        }

        private static async Task DownloadImages()
        {
            Console.Clear();
            Console.WriteLine(SYSTEM_OUTPUT_SEPERATOR);
            Console.WriteLine(string.Format(DOWNLOADING_MESSAGE, inputModel.Count, inputModel.Parallelism));
            Console.WriteLine("");
            try
            {
                int i = 0;
                while (i < inputModel.Count)
                {

                    var tasks = new List<Task>();

                    for (int j = 1; j <= inputModel.Parallelism; j++)
                    {
                        int index = ++i;

                        if (index > inputModel.Count)
                            continue;


                        tasks.Add(DownLoadFileAsync(BASE_IMAGE_SITE_URL, inputModel.SavePath, index));
                    }

                    await Task.WhenAll(tasks.ToArray());
                    await Task.Delay(delayBetweenParallels);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : ", ex.Message);
            }
            Console.WriteLine(SYSTEM_OUTPUT_SEPERATOR);

        }

        public static async Task DownLoadFileAsync(string address, string destinationFolder, int index)
        {
            try
            {
                string url = FileHelper.GenerateFileSourceURL(address, index);
                string finalDest = FileHelper.GenerateFileDestination(destinationFolder, index); ;

                using (WebClient client = new WebClient())
                {
                    Uri uri = new Uri(url);

                    //client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback4);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted());

                    client.DownloadFileAsync(uri, finalDest);

                    downladedFilePaths.Add(finalDest);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error : ", ex.Message);
            }

        }

        public static AsyncCompletedEventHandler DownloadFileCompleted()
        {
            Action<object, AsyncCompletedEventArgs> action = (sender, e) =>
            {
                downloadedCount++;
                string message = string.Format(PROGRESS_MESSAGE, downloadedCount, inputModel.Count);
                Console.WriteLine(message);
            };
            return new AsyncCompletedEventHandler(action);
        }

        private static void GetTotalCount()
        {
            while (inputModel.Count == 0)
            {
                Console.WriteLine(GenerateInputText(NUM_OF_IMAGES_MESSAGE));
                string countInput = Console.ReadLine();
                int convertedCount;
                int.TryParse(countInput, out convertedCount);
                inputModel.Count = convertedCount;
            }
        }

        private static void GetParallelCount()
        {
            Console.WriteLine(GenerateInputText(NUM_OF_PAR_DOWN_LiM_MESSAGE));
            string paralelInput = Console.ReadLine();
            int convertedParCount;
            int.TryParse(paralelInput, out convertedParCount);
            inputModel.Parallelism = convertedParCount;
        }

        private static void GetDestinationFolder()
        {

            Console.WriteLine(GenerateInputText(SAVE_PATH_MESSAGE));
            string? destInput = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(destInput))
                inputModel.SavePath = destInput;
        }

        private static string GenerateInputText(string message)
        {
            return @$"> {message}:";
        }







    }
}
