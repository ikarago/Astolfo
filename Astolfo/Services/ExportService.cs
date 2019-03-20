using Astolfo.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace Astolfo.Services
{
    public static class ExportService
    {
        // Public Methods
        public static async Task<bool> ExportOnForegroundTask(VoiceTextModel model, StorageFolder folder, SpeechSynthesisStream synthStream)
        {
            bool success = false;
            string fileExtention = ".wav";

            // TODO Check in what file format stuff needs to be exported

            // Filenames ---> Key - Voice Name - (x)

            // As WAV
            if (fileExtention == ".wma" || fileExtention == ".mp3" || fileExtention == ".m4a")
            {

            }
            else if (fileExtention == ".wav")
            {
                try
                {
                    using (var reader = new DataReader(synthStream))
                    {
                        // Get the StorageFile to put it in
                        StorageFile file = await folder.CreateFileAsync((model.Key + "-" + model.Voice.DisplayName + fileExtention), CreationCollisionOption.GenerateUniqueName);

                        // Create the buffer
                        await reader.LoadAsync((uint)synthStream.Size);
                        IBuffer buffer = reader.ReadBuffer((uint)synthStream.Size);

                        // Write it to the file
                        await FileIO.WriteBufferAsync(file, buffer);
                    }
                    success = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Couldn't export to wav");
                    Debug.WriteLine(ex);
                }
            }


            return success;
        }


        private static async Task<bool> SaveAndEncodeFile(StorageFile fileTarget, SpeechSynthesisStream synthStream, VoiceInformation voice)
        {
            bool success = false;






            return success;
        }









            public static async void ExportAllOnForegroundTask(ObservableCollection<VoiceTextModel> list)
        {

        }

        // TODO Do the export task in the background
        // TODO Set the export to a single folder



    }
}
