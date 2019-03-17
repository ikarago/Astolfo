using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Astolfo.Core.Models;
using Astolfo.Helpers;

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


        // Constructor
        public MainViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {

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



    }
}
