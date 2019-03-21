using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Astolfo.Core.Models;
using Astolfo.Helpers;
using Astolfo.Services;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

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

            // Get all the voices
            GetVoices();
        }



        // Commands
        private ICommand _importCsvCommand;
        public ICommand ImportCsvCommand
        {
            get
            {
                if (_importCsvCommand == null)
                {
                    _importCsvCommand = new RelayCommand(
                        () =>
                        {
                            ImportFromCsv();
                        });
                }
                return _importCsvCommand;
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


        private void ImportFromCsv()
        {
            // TODO change this to use the actual method instead of the sample data
            Data = ImportCsvService.UseSampleData();
        }


        private async void ExportAll()
        {
            // Let the user pick a folder
            var picker = new FolderPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add("*");
            StorageFolder folder = await picker.PickSingleFolderAsync();
            // Save the folder to the the FAL
            StorageApplicationPermissions.FutureAccessList.Add(folder);

            // Create the Speechsynth
            var synth = new SpeechSynthesizer();


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
                model.SuccessfulExport = await ExportService.ExportOnForegroundTask(model, folder, synthStream, ".mp3");
            }
        }
    }
}
