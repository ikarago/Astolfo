using System;
using System.Threading.Tasks;

using Astolfo.Helpers;

using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Astolfo.Services
{
    public static class ThemeSelectorService
    {
        private const string SettingsKey = "AppBackgroundRequestedTheme";

        public static ElementTheme Theme { get; set; } = ElementTheme.Default;

        public static async Task InitializeAsync()
        {
            Theme = await LoadThemeFromSettingsAsync();
        }

        public static async Task SetThemeAsync(ElementTheme theme)
        {
            Theme = theme;

            await SetRequestedThemeAsync();
            await SaveThemeInSettingsAsync(Theme);
        }

        public static async Task SetRequestedThemeAsync()
        {
            foreach (var view in CoreApplication.Views)
            {
                await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (Window.Current.Content is FrameworkElement frameworkElement)
                    {
                        frameworkElement.RequestedTheme = Theme;
                        SetTitlebarButtonColour();
                    }
                });
            }
        }

        private static void SetTitlebarButtonColour()
        {
            // Make the buttons transparent
            ApplicationViewTitleBar titlebar = ApplicationView.GetForCurrentView().TitleBar;
            titlebar.ButtonBackgroundColor = Colors.Transparent;
            titlebar.ButtonInactiveBackgroundColor = Colors.Transparent;

            // Set the foreground colour depending on the current theme
            // #TODO: Simplify this
            if (Theme == ElementTheme.Dark)
            {
                titlebar.ButtonForegroundColor = Colors.White;
            }
            else if (Theme == ElementTheme.Light)
            {
                titlebar.ButtonForegroundColor = Colors.Black;
            }
            else if (Theme == ElementTheme.Default)
            {
                if (App.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    titlebar.ButtonForegroundColor = Colors.White;
                }
                else if (App.Current.RequestedTheme == ApplicationTheme.Light)
                {
                    titlebar.ButtonForegroundColor = Colors.Black;
                }
            }
        }

        private static async Task<ElementTheme> LoadThemeFromSettingsAsync()
        {
            ElementTheme cacheTheme = ElementTheme.Default;
            string themeName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);

            if (!string.IsNullOrEmpty(themeName))
            {
                Enum.TryParse(themeName, out cacheTheme);
            }

            return cacheTheme;
        }

        private static async Task SaveThemeInSettingsAsync(ElementTheme theme)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, theme.ToString());
        }
    }
}
