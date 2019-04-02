using System;

using Astolfo.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Astolfo.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Voice")
            {
                e.Cancel = true;
            }
            if (e.PropertyName == "Done")
            {
                e.Cancel = true;
            }
            if (e.PropertyName == "SuccessfulExport")
            {
                e.Cancel = true;
            }
        }
    }
}
