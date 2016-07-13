using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SocialPhotoFrame
{
    public class AutoScrollListView : ListView
    {
        /// <summary>
        ///     The items source property.
        /// </summary>
        public static readonly DependencyProperty ItemDisplayIntervalProperty =
            DependencyProperty.Register("ItemDisplayInterval", typeof(int), typeof(AutoScrollListView), new PropertyMetadata(60, ItemDisplayIntervalChanged));

        private static void ItemDisplayIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as AutoScrollListView;
            if (owner != null)
            {
                owner.timer.Interval = TimeSpan.FromSeconds(owner.ItemDisplayInterval);
                owner.timer.Start();
            }
        }

        private int currentIndex;
        private DispatcherTimer timer;

        public AutoScrollListView()
        {
            this.SizeChanged += (s, e) => this.ScrollIntoView(this.Items[this.currentIndex]);
            this.timer = new DispatcherTimer();
            this.timer.Tick += Timer_Tick;
            this.timer.Interval = TimeSpan.FromSeconds(this.ItemDisplayInterval);
            this.timer.Start();
        }

        public int ItemDisplayInterval
        {
            get { return (int)this.GetValue(ItemDisplayIntervalProperty); }
            set { this.SetValue(ItemDisplayIntervalProperty, value); }
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
