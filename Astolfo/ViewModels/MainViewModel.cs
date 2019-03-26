using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Astolfo.Core.Models;
using Astolfo.Core.Services;
using Astolfo.Helpers;
using Astolfo.Services;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;

namespace Astolfo.ViewModels
{
    public class MainViewModel : Observable
    {
        // Properties
        private ObservableCollection<VoiceTextModel> _data;
        public ObservableCollection<VoiceTextModel> Data
        {
            get { return _data; }
            set { Set(ref _data, value); }
        }

        private double _completionValue;
        public double CompletionValue
        {
            get { return _completionValue; }
            set { Set(ref _completionValue, value); }
        }


        // List of Voices
        private ObservableCollection<VoiceModel> _voices;
        public ObservableCollection<VoiceModel> Voices
        {
            get { return _voices; }
            set { Set(ref _voices, value); }
        }

        private VoiceModel _selectedVoice;
        public VoiceModel SelectedVoice
        {
            get { return _selectedVoice; }
            set
            {
                Set(ref _selectedVoice, value);
                // TODO Set all the voices for all the items
                //_speechSynthesizer.Voice = _selectedVoice.Voice;
            }
        }

        // List of file extentions
        private ObservableCollection<string> _fileExtentions;
        public ObservableCollection<string> FileExtentions
        {
            get { return _fileExtentions; }
            set { Set(ref _fileExtentions, value); }
        }
        private string _selectedFileExtention;
        public string SelectedFileExtention
        {
            get { return _selectedFileExtention; }
            set { Set(ref _selectedFileExtention, value); }
        }

        // UX Triggers
        private Visibility _uxLoadingCsv;
        public Visibility UxLoadingCsv
        {
            get { return _uxLoadingCsv; }
            set { Set(ref _uxLoadingCsv, value); }
        }


        // Constructor
        public MainViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            // Prepare the ObservableCollections
            _voices = new ObservableCollection<VoiceModel>();
            _data = new ObservableCollection<VoiceTextModel>();
            _fileExtentions = new ObservableCollection<string>();

            // Set the progressbar to 0
            _completionValue = 0;

            // Get all the voices
            GetVoices();
            GetSupportedFileExtentions();

            // Set UX stuff
            _completionValue = 0;
            _selectedFileExtention = ".wav";    // This is not how you should do it, but sometimes I like dirty hacks :)

            _uxLoadingCsv = Visibility.Collapsed;
        }


        // Commands
        private ICommand _importCommand;
        public ICommand ImportCommand
        {
            get
            {
                if (_importCommand == null)
                {
                    _importCommand = new RelayCommand(
                        () =>
                        {
                            ImportFromXlsx();
                        });
                }
                return _importCommand;
            }
        }

        private ICommand _exportCommand;
        public ICommand ExportCommand
        {
            get
            {
                if (_exportCommand == null)
                {
                    _exportCommand = new RelayCommand(
                        () =>
                        {
                            ExportAll();
                        });
                }
                return _exportCommand;
            }
        }

        private ICommand _cancelExportCommand;
        public ICommand CancelExportCommand
        {
            get
            {
                if (_cancelExportCommand == null)
                {
                    _cancelExportCommand = new RelayCommand(
                        () =>
                        {
                            // TODO
                        });
                }
                return _cancelExportCommand;
            }
        }

        private ICommand _pauseExportCommand;
        public ICommand PauseExportCommand
        {
            get
            {
                if (_pauseExportCommand == null)
                {
                    _pauseExportCommand = new RelayCommand(
                        () =>
                        {
                            // TODO
                        });
                }
                return _pauseExportCommand;
            }
        }


        private ICommand _settingsCommand;
        public ICommand SettingsCommand
        {
            get
            {
                if (_settingsCommand == null)
                {
                    _settingsCommand = new RelayCommand(
                    () =>
                    {
                        // TODO
                    });
                }
                return _settingsCommand;
            }
        }

        private ICommand _aboutCommand;
        public ICommand AboutCommand
        {
            get
            {
                if (_aboutCommand == null)
                {
                    _aboutCommand = new RelayCommand(
                        () =>
                        {
                            // TODO
                        });
                }
                return _aboutCommand;
            }
        }


        // Methods


        private void GetVoices()
        {
            var voices = SpeechSynthesizer.AllVoices;
            var defaultVoice = SpeechSynthesizer.DefaultVoice;

            // Put the VoiceInformation into an VoiceModel
            foreach (VoiceInformation voice in voices)
            {
                var voiceModel = new VoiceModel(voice);
                Voices.Add(voiceModel);

                // Check for the default voice of the system and if true set it as the currently selected voice
                if (voiceModel.VoiceId == defaultVoice.Id)
                {
                    SelectedVoice = voiceModel;
                }
            }
        }

        private void GetSupportedFileExtentions()
        {
            var device = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
            bool isDesktop = (device.ContainsKey("DeviceFamily") && device["DeviceFamily"] == "Desktop");
            bool isXbox = (device.ContainsKey("DeviceFamily") && device["DeviceFamily"] == "Xbox");

            // WAV is always supported, so this will be available for all platforms
            FileExtentions.Add(".wav");

            // Check whether the using is using Windows 10 Mobile, if true, only add .wav-export, because the codecs on Mobile are fucking retarded
            if (isDesktop == true)
            {
                FileExtentions.Add(".mp3");
                FileExtentions.Add(".wma");
            }
            else if (isXbox == true)
            {
                FileExtentions.Add(".wma");
            }
        }



        private async void ImportFromXlsx()
        {
            // Show Load screen
            UxLoadingCsv = Visibility.Visible;

            // Get the file picker to select the .csv-file
            FileOpenPicker picker = new FileOpenPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.ViewMode = PickerViewMode.List;

            picker.FileTypeFilter.Add(".xlsx");

            // Get the file
            StorageFile file = await picker.PickSingleFileAsync();
            // #DIRTY Copy file to the temp folder to avoid access denied stuff
            ApplicationData appData = ApplicationData.Current;
            StorageFile tempfile = await appData.TemporaryFolder.CreateFileAsync("temp.xlsx", CreationCollisionOption.ReplaceExisting);

            await file.CopyAndReplaceAsync(tempfile);

            // Get data from the .xlsx-file
            // TODO Await this
            Data = ImportService.ImportFromXlsx(tempfile);

            // Hide Load-screen
            UxLoadingCsv = Visibility.Collapsed;
        }

        private async void ExportAll()
        {
            // Set the CompletionValue to 0
            CompletionValue = 0;

            // Let the user pick a folder
            var picker = new FolderPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add("*");
            StorageFolder folder = await picker.PickSingleFolderAsync();
            // Save the folder to the the FAL
            StorageApplicationPermissions.FutureAccessList.Add(folder);

            // Create the Speechsynth
            var synth = new SpeechSynthesizer();

            // Get the value of completion percentage
            double completetionAddValue = (double)100/ (double)Data.Count;


            foreach (VoiceTextModel model in Data)
            {
                // Create the audiostream
                // TODO TEMP, Set the voice (Make this properly in the Import n stuff)
                if (model.Voice == null)
                {
                    model.Voice = SelectedVoice.Voice;
                }
                // Set the voice of the synth
                if (model.Voice != null)
                {
                    synth.Voice = model.Voice;
                }
                SpeechSynthesisStream synthStream = await synth.SynthesizeTextToStreamAsync(model.Text);

                // Export the file
                model.SuccessfulExport = await ExportService.ExportOnForegroundTask(model, folder, synthStream, SelectedFileExtention);

                // Add a small percentage to the bar for the climb to the top
                CompletionValue += completetionAddValue;
            }
        }
    }
}
