using CSCore.CoreAudioAPI;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AudioTrackingApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MMDeviceEnumerator _deviceEnumerator;
        private MMDevice _device;
        private AudioMeterInformation _audioMeterInformation;
        private DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();

            
            _deviceEnumerator = new MMDeviceEnumerator();
            _device = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            _audioMeterInformation = AudioMeterInformation.FromDevice(_device);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartMonitoring();
        }

        public void StartMonitoring()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += CheckAudioVolume;
            dispatcherTimer.Interval = new System.TimeSpan(0, 0, 0, 0, 100); // 100ms
            dispatcherTimer.Start();
        }

        private void CheckAudioVolume(object sender, System.EventArgs e)
        {
            // Read the current peak value
            float peakValue = _audioMeterInformation.PeakValue;

            // Check if the peak value is greater than 10%
            if (peakValue > 0.1)
            {
                // Since we're updating the UI, we need to make sure we're on the UI thread
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Sound notice!");
                });
                dispatcherTimer.Stop();
            }
        }
    }
}