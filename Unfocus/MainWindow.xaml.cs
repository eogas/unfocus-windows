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
            tbIcon.Visibility = Visibility.Hidden;
            tbIcon.LeftClickCommand = new ShowWindowCommand(this);
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

        private void StopActivityMonitor()
        {
            activityTimer.Elapsed -= ActivityTimer_Elapsed;
            activityTimer.Stop();
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

                tbIcon.ShowCustomBalloon(new ReminderBalloon(), PopupAnimation.Slide, 5000);
            });
        }

        private void ActivityTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var timeSinceLastInput = WinBindings.TimeSinceLastInput();
            var timedOut = timeSinceLastInput > Config.ActivityTimeout;

            Debug.WriteLine(string.Format("Time since last input: {0} ms", timeSinceLastInput.TotalMilliseconds));

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

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartAlerts();
            StartActivityMonitor();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            StopAlerts();
            StopActivityMonitor();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            // Collapse to tray
            this.Visibility = Visibility.Hidden;
            tbIcon.Visibility = Visibility.Visible;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            tbIcon.Visibility = Visibility.Collapsed;
            tbIcon.Dispose();

            Environment.Exit(0);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            // Collapse to tray
            this.Visibility = Visibility.Hidden;
            tbIcon.Visibility = Visibility.Visible;
        }
    }
}
