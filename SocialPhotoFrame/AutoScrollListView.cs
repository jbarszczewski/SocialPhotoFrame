using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SocialPhotoFrame
{
    public class AutoScrollListView : ListView
    {
        private int currentIndex;
        private DispatcherTimer timer;

        public AutoScrollListView()
        {
            this.SizeChanged += (s, e) => this.ScrollIntoView(this.Items[this.currentIndex]);
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(2);
            this.timer.Tick += Timer_Tick;
            this.timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            this.ScrollIntoView(this.Items[this.currentIndex]);
            this.currentIndex++;
            if (this.currentIndex >= this.Items.Count)
                this.currentIndex = 0;
        }
    }
}
