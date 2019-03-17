﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Astolfo.Core.Models;
using Astolfo.Helpers;
using Windows.Media.SpeechSynthesis;

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
                            // TODO
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
                            // TODO
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


    }
}