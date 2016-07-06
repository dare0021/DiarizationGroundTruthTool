using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
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
        System.Timers.Timer countdownTimer = new System.Timers.Timer(1000);
        /// <summary>
        /// Always enabled.
        /// </summary>
        System.Timers.Timer updateTimer = new System.Timers.Timer(1000 / 60);
        Stopwatch stopwatch = new Stopwatch();
        MediaPlayer mediaPlayer = new MediaPlayer();

        List<DialogEntry> dialogEntries = new List<DialogEntry>();
        Dictionary<int, DialogEntry> ongoingDialogs = new Dictionary<int, DialogEntry>();
        List<Key> pressedKeys = new List<Key>();
        List<char> activePersons = new List<char>();

        String exportDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        String importDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        int countdown;

        public MainWindow()
        {
            InitializeComponent();

            // debug info prints in system context language by default
            CultureInfo useng = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = useng;
            Thread.CurrentThread.CurrentUICulture = useng;

            updateButtons();
            btnExport.IsEnabled = false;
            btnResume.IsEnabled = false;
            countdownTimer.Elapsed += new System.Timers.ElapsedEventHandler(countdownEvent);
            updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(update);

            this.SizeChanged += new SizeChangedEventHandler(resizeComponents);
            this.Closing += new System.ComponentModel.CancelEventHandler(closing);
            this.KeyDown += new KeyEventHandler(keyDownWrapper);
            this.KeyUp += new KeyEventHandler(keyUpWrapper);
            
            displayText("Records the time when you press a number button and when you release the button.\n" + 
                "3 second prep time before starting.\n" + 
                "Different keyboard support different number of simultaneous input");
            updateTimer.Start();
        }

        private void runAfterInitialDraw(object sender, EventArgs e)
        {
        }

        private void resizeComponents(object sender, System.Windows.SizeChangedEventArgs e)
        {
            txtDisp.ScrollToEnd();
        }
        
        public void Dispose(bool disposing)
        {
            if (updateTimer.Enabled)
            {
                updateTimer.Stop();
                updateTimer.Dispose();
            }
            if (countdownTimer.Enabled)
            {
                countdownTimer.Stop();
                countdownTimer.Dispose();
            }
        }

        private void closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
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
            if (stopwatch.IsRunning && key <= '9' && key >= '0' && !activePersons.Contains(key))
            {
                activePersons.Add(key);
                activePersons.Sort();
                displayText("\n" + key + " started talking at \t" + getElapsedTime().ToString(@"hh\:mm\:ss"));
                var newEntry = new DialogEntry(key - '0', getElapsedTime());
                dialogEntries.Add(newEntry);
                ongoingDialogs.Add(newEntry.id, newEntry);
            }
        }

        private void keyUp(char key)
        {
            if (stopwatch.IsRunning && key <= '9' && key >= '0' && activePersons.Contains(key))
            {
                activePersons.Remove(key);
                activePersons.Sort();
                displayText("\n" + key + " stopped talking at \t" + getElapsedTime().ToString(@"hh\:mm\:ss"));
                int toRemove = -1;
                foreach (var kvp in ongoingDialogs)
                {
                    if (kvp.Key == key - '0')
                    {
                        kvp.Value.endTime = getElapsedTime();
                        toRemove = kvp.Key;
                        break;
                    }
                }
                if (toRemove >= 0)
                    ongoingDialogs.Remove(toRemove);
            }
        }

        private void displayText(String txt)
        {
            txtDisp.AppendText(txt);
            txtDisp.ScrollToEnd();
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            start();
        }

        /// <summary>
        /// updates the buttons' IsEnabled property according to whether the stopwatch is running
        /// </summary>
        private void updateButtons()
        {
            if (countdownTimer.Enabled)
            {
                btnRun.IsEnabled = false;
                btnStopAndExport.IsEnabled = false;
                btnStop.IsEnabled = true;
                btnExport.IsEnabled = false;
                btnResume.IsEnabled = false;
                btnOpen.IsEnabled = false;
            }
            else
            {
                var running = stopwatch.IsRunning;
                btnRun.IsEnabled = !running;
                btnStopAndExport.IsEnabled = running;
                btnStop.IsEnabled = running;
                btnExport.IsEnabled = !running;
                btnResume.IsEnabled = !running;
                btnOpen.IsEnabled = !running;
            }
        }

        private void start()
        {
            countdown = 3;
            countdownTimer.Start();
            activePersons.Clear();
            dialogEntries.Clear();
            ongoingDialogs.Clear();
            updateButtons();
        }

        private void update(object source, System.Timers.ElapsedEventArgs e) 
        {
            // timer runs on its own thread means the thread context defaults back to the system context
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-Us");

            DateTime now = e.SignalTime;
            try
            {
                if (countdownTimer.Enabled)
                {
                    Dispatcher.Invoke(new Action(() => {
                        txtTime.Text = "Starting in: " + countdown;
                    }), System.Windows.Threading.DispatcherPriority.Render);
                }
                else
                {
                    Dispatcher.Invoke(new Action(() => {
                        var elapsedTime = getElapsedTime().ToString();
                        txtTime.Text = elapsedTime.Substring(0, elapsedTime.Length - 3);
                    }), System.Windows.Threading.DispatcherPriority.Render);
                }
                Dispatcher.Invoke(new Action(() => {
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
                System.Console.WriteLine(e2.StackTrace);
            }
        }

        private void countdownEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            // countdown over. Start main task.
            if (countdown <= 1)
            {
                countdownTimer.Stop();
                stopwatch.Restart();
                try
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        mediaPlayer.Stop();
                        mediaPlayer.Play();
                        updateButtons();
                        txtDisp.Text = "Started at " + DateTime.Now.ToString("HH:mm:ss tt");
                    }), System.Windows.Threading.DispatcherPriority.Render);
                }
                catch (TaskCanceledException e2)
                {
                    System.Console.WriteLine(e2.StackTrace);
                }
            }
            countdown--;
        }

        private TimeSpan getElapsedTime()
        {
            return stopwatch.Elapsed;
        }
        
        private void stop()
        {
            mediaPlayer.Pause();
            countdownTimer.Stop();
            stopwatch.Stop();
            updateButtons();
        }

        private void btnStopAndExport_Click(object sender, RoutedEventArgs e)
        {
            stop();
            export();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            stop();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            export();
        }

        private void btnResume_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            stopwatch.Start();
            updateButtons();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Close();

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".flac"; // Default file extension
            dlg.Filter = "Free Lossless Audio Codec|*.flac|WAVeform audio file format|*.wav"; // Filter files by extension
            dlg.CheckPathExists = true;
            dlg.AddExtension = true;
            dlg.Multiselect = false; 
            dlg.ValidateNames = true;
            dlg.Title = "Open | Cancel to mute audio";
            dlg.InitialDirectory = importDir;

            // Show save file dialog box
            bool? result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filePath = dlg.FileName;
                importDir = filePath.Substring(0, filePath.LastIndexOf('\\'));
                if (System.IO.Path.GetExtension(filePath).ToLowerInvariant() == ".flac")
                {
                    FlacToWavWrapper.run(filePath, filePath + ".wav");
                    filePath += ".wav";
                }
                mediaPlayer.Open(new Uri(filePath));
            }
        }

        private void export()
        {
            String exportText = "";
            foreach (var dialog in dialogEntries)
            {
                exportText += dialog;
            }

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Untitled"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents|*.txt"; // Filter files by extension
            dlg.OverwritePrompt = true;
            dlg.CheckPathExists = true;
            dlg.AddExtension = true;
            dlg.CreatePrompt = false;
            dlg.ValidateNames = true;
            dlg.Title = "Export";
            dlg.InitialDirectory = exportDir;

            // Show save file dialog box
            bool? result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string savePath = dlg.FileName;
                exportDir = savePath.Substring(0, savePath.LastIndexOf('\\'));
                File.WriteAllText(savePath, exportText);
            }
        }
    }
}
