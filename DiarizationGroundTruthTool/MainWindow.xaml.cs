using System;
using System.Collections.Generic;
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
        DateTime lastUpdate = DateTime.MinValue;
        DateTime updateTimerStartedAt;

        public MainWindow()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-us");
            updateTimer.Enabled = false;
            updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(update);

            this.SizeChanged += new SizeChangedEventHandler(resizeComponents);
            this.Closing += new System.ComponentModel.CancelEventHandler(closing);

            reset();
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

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            updateTimerInit();
            updateTimer.Start();
        }

        private void updateTimerInit()
        {
            updateTimerStartedAt = DateTime.Now;
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
                Dispatcher.Invoke(new Action(() => { txtTime.Text = (now - updateTimerStartedAt).ToString(@"hh\:mm\:ss"); }), System.Windows.Threading.DispatcherPriority.Render);
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
        }

        private void reset()
        {
            txtTime.Text = (new DateTime(0)).ToString("HH:mm:ss");
            txtDisp.Text = "Records the time when you press a number button and when you release the button.\n3 second prep time before starting.";
        }

        private void btnStopAndReset_Click(object sender, RoutedEventArgs e)
        {
            stop();
            reset();
        }

        private void btnStopAndExport_Click(object sender, RoutedEventArgs e)
        {
            stop();
        }
    }
}
