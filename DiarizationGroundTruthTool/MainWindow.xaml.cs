using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiarizationGroundTruthTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Timers.Timer updateTimer = new System.Timers.Timer(1000 / 30);
        List<Key> pressedKeys = new List<Key>();
        List<char> activePersons = new List<char>();
        List<char> progressDingbats = new List<char>(){ '-', '\\', '|', '/' };
        List<char>.Enumerator progressDingbatIter;

        DateTime lastUpdate = DateTime.MinValue;
        DateTime updateTimerStartedAt;

        public MainWindow()
        {
            InitializeComponent();

            CultureInfo useng = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = useng;
            Thread.CurrentThread.CurrentUICulture = useng;
            //GetEnumerator() returns head, which is before first, and contains garbage
            progressDingbatIter = progressDingbats.GetEnumerator();
            progressDingbatIter.MoveNext();

            updateButtons();
            btnExport.IsEnabled = false;
            btnResume.IsEnabled = false;
            updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(update);

            this.SizeChanged += new SizeChangedEventHandler(resizeComponents);
            this.Closing += new System.ComponentModel.CancelEventHandler(closing);
            this.KeyDown += new KeyEventHandler(keyDownWrapper);
            this.KeyUp += new KeyEventHandler(keyUpWrapper);
            
            txtTime.Text = (new DateTime(0)).ToString("HH:mm:ss");
            txtDisp.Text = "Records the time when you press a number button and when you release the button.\n" + 
                "3 second prep time before starting.\n" + 
                "Different keyboard support different number of simultaneous input";
        }

        private void runAfterInitialDraw(object sender, EventArgs e)
        {
        }

        private void resizeComponents(object sender, System.Windows.SizeChangedEventArgs e)
        {
        }

        private void closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            updateTimer.Enabled = false;
            updateTimer.Dispose();
        }

        private void keyDownWrapper(object sender, KeyEventArgs e)
        {
            if (!pressedKeys.Contains(e.Key))
                pressedKeys.Add(e.Key);
            switch (e.Key)
            {
                case Key.D0:
                case Key.NumPad0:
                    keyDown('0');
                    break;
                case Key.D1:
                case Key.NumPad1:
                    keyDown('1');
                    break;
                case Key.D2:
                case Key.NumPad2:
                    keyDown('2');
                    break;
                case Key.D3:
                case Key.NumPad3:
                    keyDown('3');
                    break;
                case Key.D4:
                case Key.NumPad4:
                    keyDown('4');
                    break;
                case Key.D5:
                case Key.NumPad5:
                    keyDown('5');
                    break;
                case Key.D6:
                case Key.NumPad6:
                    keyDown('6');
                    break;
                case Key.D7:
                case Key.NumPad7:
                    keyDown('7');
                    break;
                case Key.D8:
                case Key.NumPad8:
                    keyDown('8');
                    break;
                case Key.D9:
                case Key.NumPad9:
                    keyDown('9');
                    break;
            }
        }

        private void keyUpWrapper(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.Key);
            switch (e.Key)
            {
                case Key.D0:
                case Key.NumPad0:
                    keyUp('0');
                    break;
                case Key.D1:
                case Key.NumPad1:
                    keyUp('1');
                    break;
                case Key.D2:
                case Key.NumPad2:
                    keyUp('2');
                    break;
                case Key.D3:
                case Key.NumPad3:
                    keyUp('3');
                    break;
                case Key.D4:
                case Key.NumPad4:
                    keyUp('4');
                    break;
                case Key.D5:
                case Key.NumPad5:
                    keyUp('5');
                    break;
                case Key.D6:
                case Key.NumPad6:
                    keyUp('6');
                    break;
                case Key.D7:
                case Key.NumPad7:
                    keyUp('7');
                    break;
                case Key.D8:
                case Key.NumPad8:
                    keyUp('8');
                    break;
                case Key.D9:
                case Key.NumPad9:
                    keyUp('9');
                    break;
            }
        }

        private void keyDown(char key)
        {
            if (key <= '9' && key >= '0' && !activePersons.Contains(key))
            {
                activePersons.Add(key);
                activePersons.Sort();
            }
        }

        private void keyUp(char key)
        {
            if (key <= '9' && key >= '0' && activePersons.Contains(key))
            {
                activePersons.Remove(key);
                activePersons.Sort();
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            updateTimerInit();
            updateTimer.Start();
            updateButtons();
        }

        /// <summary>
        /// updates the buttons' IsEnabled property according to whether the updateTimer is running
        /// </summary>
        private void updateButtons()
        {
            var running = updateTimer.Enabled;
            btnRun.IsEnabled = !running;
            btnStopAndExport.IsEnabled = running;
            btnStop.IsEnabled = running;
            btnExport.IsEnabled = !running;
            btnResume.IsEnabled = !running;
        }

        private void updateTimerInit()
        {
            var now = updateTimerStartedAt = DateTime.Now;
            txtDisp.Text = "Started at " + now.ToString("HH:mm:ss tt");
        }

        private void update(object source, System.Timers.ElapsedEventArgs e) 
        {
            // timer runs on its own thread means the thread context defaults back to the system context
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");

            DateTime now = e.SignalTime;
            TimeSpan dt = now - lastUpdate;
            if((source == null) && (e == null))
            {
                updateTimerStartedAt = now;
            }
            try
            {
                if (!progressDingbatIter.MoveNext())
                {
                    progressDingbatIter = progressDingbats.GetEnumerator();
                    progressDingbatIter.MoveNext();
                }
                // prevent variable capture due to Diapatcher lambda function
                var progressDingbat = progressDingbatIter.Current;
                Dispatcher.Invoke(new Action(() => {
                    txtTime.Text = (now - updateTimerStartedAt).ToString(@"hh\:mm\:ss");
                    txtProgress.Text = "" + progressDingbat;
                    String txt = "";
                    foreach (var k in activePersons)
                    {
                        txt += k + " ";
                    }
                    txtPressed.Text = txt;
                }), System.Windows.Threading.DispatcherPriority.Render);
            }
            catch (TaskCanceledException e2)
            {
                ;
            }
            
            lastUpdate = now;
        }
        
        private void stop()
        {
            updateTimer.Stop();
            updateButtons();
        }

        private void btnStopAndExport_Click(object sender, RoutedEventArgs e)
        {
            stop();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            stop();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(!updateTimer.Enabled); 
        }

        private void btnResume_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
