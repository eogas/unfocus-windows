using System;
using System.Configuration;

namespace Unfocus
{
    public static class Config
    {
        public static TimeSpan ReminderInterval { get; set; }
        public static TimeSpan ActivityTimeout { get; set; }
        public static TimeSpan ActivityCheckFrequency { get; set; }

        public static void ReadConfig()
        {
            long reminderMS = long.Parse(ConfigurationManager.AppSettings["ReminderInterval"]);
            ReminderInterval = TimeSpan.FromMilliseconds(reminderMS);

            long timeoutMS = long.Parse(ConfigurationManager.AppSettings["ActivityTimeout"]);
            ActivityTimeout = TimeSpan.FromMilliseconds(timeoutMS);

            long checkFreqMS = long.Parse(ConfigurationManager.AppSettings["ActivityCheckFrequency"]);
            ActivityCheckFrequency = TimeSpan.FromMilliseconds(checkFreqMS);
        }

        public static void SaveConfig()
        {
            // TODO
        }
    }
}
