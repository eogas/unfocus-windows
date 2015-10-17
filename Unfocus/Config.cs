using System;
using System.Configuration;

namespace Unfocus
{
    public static class Config
    {
        public static TimeSpan ReminderInterval { get; set; }
        public static TimeSpan ActivityTimeout { get; set; }
        public static TimeSpan ActivityCheckFrequency { get; set; }

        private static Configuration config =
            ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

        public static void ReadConfig()
        {
            var appSettings = config.AppSettings.Settings;

            long reminderMS = long.Parse(appSettings["ReminderInterval"].Value);
            ReminderInterval = TimeSpan.FromMilliseconds(reminderMS);

            long timeoutMS = long.Parse(appSettings["ActivityTimeout"].Value);
            ActivityTimeout = TimeSpan.FromMilliseconds(timeoutMS);

            long checkFreqMS = long.Parse(appSettings["ActivityCheckFrequency"].Value);
            ActivityCheckFrequency = TimeSpan.FromMilliseconds(checkFreqMS);
        }

        // NOTE This doesn't work while debugging
        public static void SaveConfig()
        {
            var appSettings = config.AppSettings.Settings;

            appSettings["ReminderInterval"].Value = ReminderInterval.TotalMilliseconds.ToString();
            appSettings["ActivityTimeout"].Value = ActivityTimeout.TotalMilliseconds.ToString();
            appSettings["ActivityCheckFrequency"].Value = ActivityCheckFrequency.TotalMilliseconds.ToString();

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
