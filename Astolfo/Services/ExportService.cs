﻿using Astolfo.Core.Models;
using Astolfo.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media.MediaProperties;
using Windows.Media.SpeechSynthesis;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Astolfo.Services
{
    public static class ExportService
    {
        // Public Methods
        public static async Task<bool> ExportOnForegroundTask(VoiceTextModel model, StorageFolder folder, SpeechSynthesisStream synthStream, string fileExtention)
        {
            bool success = false;
            //string fileExtention = ".wav";

            // TODO Check in what file format stuff needs to be exported

            // Filenames ---> Key - Voice Name - (x)

            // As WAV
            if (fileExtention == ".wma" || fileExtention == ".mp3" || fileExtention == ".m4a")
            {
                success = await SaveAndEncodeFile(model, folder, synthStream, fileExtention);
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


        private static async Task<bool> SaveAndEncodeFile(VoiceTextModel model, StorageFolder folder, SpeechSynthesisStream synthStream, string fileExtention)
        {
            bool success = false;

            // Initialise some stuff
            MediaEncodingProfile _profile;
            MediaTranscoder _transcoder = new MediaTranscoder();
            CoreDispatcher _dispatcher = Window.Current.Dispatcher;
            CancellationTokenSource _cts = new CancellationTokenSource();

            Debug.WriteLine(fileExtention + " selected");


            // Set encoding profiles
            _profile = null;
            AudioEncodingQuality audioEncodingProfile = AudioEncodingQuality.High;
            if (fileExtention == ".wma")
            {
                _profile = MediaEncodingProfile.CreateWma(audioEncodingProfile);
            }
            else if (fileExtention == ".mp3")
            {
                _profile = MediaEncodingProfile.CreateMp3(audioEncodingProfile);
            }
            else if (fileExtention == ".m4a")
            {
                _profile = MediaEncodingProfile.CreateM4a(audioEncodingProfile);
            }
            else
            {
                Debug.WriteLine("Can't select a media encoding profile");
                return success;
            }


            // Write temporary Wav to Temp-storage
            ApplicationData appData = ApplicationData.Current;
            StorageFile source = await appData.TemporaryFolder.CreateFileAsync("temp.wav", CreationCollisionOption.GenerateUniqueName);
            try
            {
                using (var reader = new DataReader(synthStream))
                {
                    await reader.LoadAsync((uint)synthStream.Size);
                    IBuffer buffer = reader.ReadBuffer((uint)synthStream.Size);
                    await FileIO.WriteBufferAsync(source, buffer);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Couldn't prepare wav for transcoding");
                Debug.WriteLine(ex);
            }


            // Create a file in the folder
            // Get the StorageFile to put it in
            StorageFile file = await folder.CreateFileAsync((model.Key + "-" + model.Voice.DisplayName + fileExtention), CreationCollisionOption.GenerateUniqueName);


            // Prepare transcoding files
            var preparedTranscoderResult = await _transcoder.PrepareFileTranscodeAsync(source, file, _profile);
            if (preparedTranscoderResult.CanTranscode)
            {
                // Set task for transcoding    
                await preparedTranscoderResult.TranscodeAsync().AsTask(_cts.Token);

                // Set Music-properties
                MusicProperties fileProperties = await file.Properties.GetMusicPropertiesAsync();
                fileProperties.Title = file.DisplayName;
                fileProperties.Artist = ("Astolfo " + ResourceExtensions.GetLocalized("VoicedBy") + " " + model.Voice.DisplayName);
                await fileProperties.SavePropertiesAsync();

                // #TODO: Add the newly created file to the systems MRU?
                // Add the file to app MRU and possibly system MRU
                //RecentStorageItemVisibility visibility = SystemMRUCheckBox.IsChecked.Value ? RecentStorageItemVisibility.AppAndSystem : RecentStorageItemVisibility.AppOnly;
                //rootPage.mruToken = StorageApplicationPermissions.MostRecentlyUsedList.Add(file, file.Name, visibility);

                //RecentStorageItemVisibility visibility = RecentStorageItemVisibility.AppOnly;
                //StorageApplicationPermissions.FutureAccessList.Add(fileTarget, fileTarget.DisplayName);


                // Report completed
                success = true;
                Debug.WriteLine(file.DisplayName + file.FileType + " export completed");
            }
            else
            {
                Debug.WriteLine(preparedTranscoderResult.FailureReason);
            }

            // TODO Clear temp folder


            return success;
        }


        // TODO Do the export task in the background
        // TODO Set the export to a single folder



    }
}
