using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Unfocus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TaskbarIcon tbIcon;

        Timer alertTimer;
        Timer activityTimer;

        bool running = false;

        public MainWindow()
        {
            InitializeComponent();

            Config.ReadConfig();

            InitTaskbarIcon();

            StartAlerts();

            StartActivityMonitor();
        }

        private void InitTaskbarIcon()
        {
            tbIcon = new TaskbarIcon();
            tbIcon.Icon = Properties.Resources.Eyeball;
            tbIcon.Visibility = Visibility.Visible;
        }

        // probs break this out into a class (ActivityMonitor)
        private void StartActivityMonitor()
        {
            activityTimer = new Timer()
            {
                Interval = Config.ActivityCheckFrequency.TotalMilliseconds,
                AutoReset = true,
                Enabled = true
            };

            activityTimer.Elapsed += ActivityTimer_Elapsed;
            activityTimer.Start();
        }

        private void StartAlerts()
        {
            alertTimer = new Timer()
            {
                Interval = Config.ReminderInterval.TotalMilliseconds,
                AutoReset = true,
                Enabled = true
            };

            alertTimer.Elapsed += AlertTimer_Elapsed;
            alertTimer.Start();

            running = true;
        }

        private void StopAlerts()
        {
            alertTimer.Elapsed -= AlertTimer_Elapsed;
            alertTimer.Stop();

            running = false;
        }

        private void AlertTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Debug.WriteLine("Displaying alert.");

                tbIcon.ShowCustomBalloon(new ReminderBalloon(), PopupAnimation.Slide, 4000);
            });
        }

        private void ActivityTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var timeSinceLastInput = WinBindings.TimeSinceLastInput();
            var timedOut = timeSinceLastInput > Config.ActivityTimeout;

            Debug.WriteLine(string.Format("Time since last input: {0}", timeSinceLastInput.TotalMilliseconds));

            if (timedOut && running)
            {
                Debug.WriteLine("Timed out due to inactivity.");

                StopAlerts();
            }
            else if (!timedOut && !running)
            {
                Debug.WriteLine("Detected user activity, restarting.");

                StartAlerts();
            }
        }
    }
}
