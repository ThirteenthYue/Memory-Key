
using System.Timers;

namespace Memory_Key
{
    public partial class TimePage : ContentPage
    {
        private System.Timers.Timer timer;

        public TimePage()
        {
            InitializeComponent();
            // initialize time of page
            timer = new System.Timers.Timer(1000); 
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Start();
        }

        // Timing event processing
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                timeLabel.Text = DateTime.Now.ToString("T");
            });
        }
    }

}