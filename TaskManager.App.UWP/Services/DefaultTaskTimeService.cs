using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.App.UWP.Helpers;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace TaskManager.App.UWP.Services
{
    public static class DefaultTaskTimeService
    {
        private const string SettingsKey = "DefaultTaskTimeSetting";

        public static TimeSpan DefaultTaskTime { get; set; } = TimeSpan.FromHours(12);

        public static async Task InitializeAsync()
        {
            DefaultTaskTime = await LoadDefaultTaskTimeFromSettingsAsync();
        }

        public static async Task SetDefaultTaskTimeAsync(TimeSpan defaultTime)
        {
            DefaultTaskTime = defaultTime;

            await SaveDefaultTaskTimeInSettingsAsync(defaultTime);
        }

        private static async Task<TimeSpan> LoadDefaultTaskTimeFromSettingsAsync()
        {
            string defaultTime = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);

            if (!string.IsNullOrEmpty(defaultTime))
            {
                double.TryParse(defaultTime, out var millis);
                var defaultTaskTime = TimeSpan.FromMilliseconds(millis);

                return defaultTaskTime;
            }

            return TimeSpan.Zero;
        }

        private static async Task SaveDefaultTaskTimeInSettingsAsync(TimeSpan defaultTime)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, defaultTime.TotalMilliseconds.ToString());
        }
    }
}
