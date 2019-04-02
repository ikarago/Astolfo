using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Astolfo.Core.Models;
using Astolfo.Core.Services;
using Astolfo.Helpers;
using Astolfo.Services;
using Astolfo.Views.Dialogs;
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

        private ObservableCollection<VoiceTextModel> _failedExportData;
        public ObservableCollection<VoiceTextModel> FailedExportData
        {
            get { return _failedExportData; }
            set { Set(ref _failedExportData, value); }
        }

        // Export completion numbers
        private int _completionCurrentlyExporting;
        public int CompletionCurrentlyExporting
        {
            get { return _completionCurrentlyExporting; }
            set { Set(ref _completionCurrentlyExporting, value); }
        }

        private int _completionTotal;
        public int CompletionTotal
        {
            get { return _completionTotal; }
            set { Set(ref _completionTotal, value); }
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

        private Visibility _uxStartUi;
        public Visibility UxStartUi
        {
            get { return _uxStartUi; }
            set { Set(ref _uxStartUi, value); }
        }

        private Visibility _uxExportUi;
        public Visibility UxExportUi
        {
            get { return _uxExportUi; }
            set { Set(ref _uxExportUi, value); }
        }

        private Visibility _uxShowExportComplete;
        public Visibility UxShowExportComplete
        {
            get { return _uxShowExportComplete; }
            set { Set(ref _uxShowExportComplete, value); }
        }

        private Visibility _uxShowFailureList;
        public Visibility UxShowFailureList
        {
            get { return _uxShowFailureList; }
            set { Set(ref _uxShowFailureList, value); }
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
            _failedExportData = new ObservableCollection<VoiceTextModel>();
            _fileExtentions = new ObservableCollection<string>();

            // Set the progressbar to 0
            _completionCurrentlyExporting = 0;
            _completionTotal = 0;
            _completionValue = 0;

            // Get all the voices
            GetVoices();
            GetSupportedFileExtentions();

            // Set UX stuff
            _completionValue = 0;
            _selectedFileExtention = ".wav";    // This is not how you should do it, but sometimes I like dirty hacks :)

            _uxLoadingCsv = Visibility.Collapsed;
            _uxExportUi = Visibility.Collapsed;
            _uxShowFailureList = Visibility.Collapsed;

            _uxStartUi = Visibility.Visible;
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
                        ShowSettingsDialog();
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
                            ShowAboutDialog();
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
            try
            {
                StorageFile file = await picker.PickSingleFileAsync();
                // #DIRTY Copy file to the temp folder to avoid access denied stuff
                ApplicationData appData = ApplicationData.Current;
                StorageFile tempfile = await appData.TemporaryFolder.CreateFileAsync("temp.xlsx", CreationCollisionOption.ReplaceExisting);

                await file.CopyAndReplaceAsync(tempfile);

                // Get data from the .xlsx-file
                // TODO Await this
                Data = ImportService.ImportFromXlsx(tempfile);

                // Hide the start UI, show Export UI
                UxStartUi = Visibility.Collapsed;
                UxExportUi = Visibility.Visible;
            }
            catch
            {
                Debug.WriteLine("MainViewModel - ImportFromXlsx() - No item selected in picker.");
            }

            // Hide Load-screen
            UxLoadingCsv = Visibility.Collapsed;
        }

        private async void ExportAll()
        {
            // Check if there is actual data to export
            if (Data.Count == 0)
            {
                Debug.WriteLine("MainViewModel - ExportAll() - No data to actually export. You idiot!");
                return;
            }

            // Set the CompletionValue to 0
            CompletionValue = 0;
            // Create an empty ObservableCollection to keep track of the failed items
            ObservableCollection<VoiceTextModel> failedData = new ObservableCollection<VoiceTextModel>();
            // Hide the Failed items list (if it's open)
            UxShowFailureList = Visibility.Collapsed;

            // Let the user pick a folder
            var picker = new FolderPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add("*");
            try
            {
                StorageFolder folder = await picker.PickSingleFolderAsync();

                // Save the folder to the the FAL
                StorageApplicationPermissions.FutureAccessList.Add(folder);

                // Create the Speechsynth
                var synth = new SpeechSynthesizer();

                // Set completion stuff
                CompletionCurrentlyExporting = 0;
                CompletionTotal = Data.Count;
                // Get the value of completion percentage
                double completetionAddValue = (double)100 / (double)Data.Count;


                foreach (VoiceTextModel model in Data)
                {
                    if (model.Text != null)
                    {
                        // Create the audiostream
                        // #TODO TEMP, Set the voice (Make this properly in the Import n stuff)
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
                        CompletionCurrentlyExporting++;
                        CompletionValue += completetionAddValue;
                    }
                    else
                    {
                        Debug.WriteLine("Could not export " + model.Key + " - TTS Text property is null");
                        model.SuccessfulExport = false;
                        // Add to the failed list
                        failedData.Add(model);

                        // Increase the completion rate
                        CompletionCurrentlyExporting++;
                        CompletionValue += completetionAddValue;
                        Data = Data;
                    }
                }

                // Display list of failed items
                FailedExportData = failedData;
                if (FailedExportData.Count != 0 && FailedExportData != null)
                {
                    UxShowFailureList = Visibility.Visible;
                }
            }
            catch
            {
                Debug.WriteLine("MainViewModel - ExportAll() - No folder selected in picker.");
                return;
            }
        }

        private async void ShowAboutDialog()
        {
            var dialog = new AboutDialog();
            await dialog.ShowAsync();
        }

        private async void ShowSettingsDialog()
        {
            var dialog = new SettingsDialog();
            await dialog.ShowAsync();
        }
    }
}
