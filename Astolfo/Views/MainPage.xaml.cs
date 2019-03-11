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
    }
}
