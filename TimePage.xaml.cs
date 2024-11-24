
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
            timer = new System.Timers.Timer(1000); // 设置计时器每秒触发一次
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Start();
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                timeLabel.Text = DateTime.Now.ToString("T"); // 使用 "T" 格式化字符串显示当前时间
            });
        }
    }

}