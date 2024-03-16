using NAudio.CoreAudioApi;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace AudioTrackingApplication
{
    public partial class MainWindow : Window
    {
        private readonly MMDeviceEnumerator _deviceEnumerator;
        private readonly MMDevice _device;
        private readonly AudioMeterInformation _audioMeterInformation;
        private DispatcherTimer _dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
            _deviceEnumerator = new MMDeviceEnumerator();
            _device = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            _audioMeterInformation = _device.AudioMeterInformation;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string appName = appNameTextBox.Text;
            if (!string.IsNullOrWhiteSpace(appName))
            {
                RunApplication(appName);
                StartMonitoring(appName);
            }
            else
            {
                MessageBox.Show("Please enter the app name or path.");
            }
        }

        private void RunApplication(string appPath)
        {
            try
            {
                // Starts the specified application and returns the process object associated with it.
                Process.Start(appPath);
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                MessageBox.Show($"Could not start the application: {ex.Message}");
            }
        }

        public void StartMonitoring(string appName)
        {
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += (s, args) => CheckAudioVolume(appName);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100); // 100ms
            _dispatcherTimer.Start();
        }

        private void CheckAudioVolume(string appName)
        {
            var processes = Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(appName));
            bool processFound = false;

            foreach (var process in processes)
            {
                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    processFound = true;
                    float peakValue = _audioMeterInformation.MasterPeakValue;
                    // Check if the peak value is greater than 10%
                    if (peakValue > 0.1)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Volume of {appName} is greater than 10%!");
                        });
                        _dispatcherTimer.Stop();
                    }
                    break;
                }
            }

            if (!processFound)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"{appName} is not running or does not have a window.");
                });
                _dispatcherTimer.Stop();
            }
        }
    }
}