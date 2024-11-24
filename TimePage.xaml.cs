
using System.Timers;

/*using Microsoft.Maui.Controls;
using System;*/

namespace Memory_Key
{
    public partial class TimePage : ContentPage
    {
        private System.Timers.Timer timer;

        public TimePage()
        {
            InitializeComponent();
            timer = new System.Timers.Timer(1000); // ���ü�ʱ��ÿ�봥��һ��
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Start();
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                timeLabel.Text = DateTime.Now.ToString("T"); // ʹ�� "T" ��ʽ���ַ�����ʾ��ǰʱ��
            });
        }
    }

}