using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

using TaskManager.App.UWP.Helpers;
using TaskManager.App.UWP.Services;

using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace TaskManager.App.UWP.ViewModels
{
    // TODO: Add other settings as necessary. For help see https://github.com/microsoft/TemplateStudio/blob/main/docs/UWP/pages/settings.md
    public class SettingsViewModel : ObservableObject
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { SetProperty(ref _elementTheme, value); }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { SetProperty(ref _versionDescription, value); }
        }
                
        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            ElementTheme = param;
                            await ThemeSelectorService.SetThemeAsync(param);
                        });
                }

                return _switchThemeCommand;
            }
        }
        private ICommand _switchThemeCommand;

        public ICommand ChangeDefaultTaskTimeInvokedCommand => _changeDefaultTaskTimeInvokedCommand ?? (_changeDefaultTaskTimeInvokedCommand = new RelayCommand<TimeSpan>(OnChangeDefaultTaskTimeInvoked));
        private ICommand _changeDefaultTaskTimeInvokedCommand;

        public TimeSpan DefaultTaskTime
        {
            get => _defaultTaskTime;
            set => SetProperty(ref _defaultTaskTime, value);
        }
        private TimeSpan _defaultTaskTime;

        public SettingsViewModel()
        {
        }

        public async Task InitializeAsync()
        {
            VersionDescription = GetVersionDescription();
            DefaultTaskTime = await GetDefaultTaskTimeAsync();
            await Task.CompletedTask;
        }

        private string GetVersionDescription()
        {
            var appName = "TafeTaskManager".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private async Task<TimeSpan> GetDefaultTaskTimeAsync()
        {
            var ts = TimeSpan.Zero;

            await DefaultTaskTimeService.InitializeAsync();
            ts = DefaultTaskTimeService.DefaultTaskTime;

            return ts;
        }

        private async void OnChangeDefaultTaskTimeInvoked(TimeSpan defaultTime)
        {
            await DefaultTaskTimeService.SetDefaultTaskTimeAsync(DefaultTaskTime);
        }
    }
}
