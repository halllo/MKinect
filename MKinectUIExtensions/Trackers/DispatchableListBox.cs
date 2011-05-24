using System;
using System.Windows.Controls;
using System.Windows.Media;

namespace MKinectUIExtensions.Trackers
{
    public class DispatchableListBox : ListBox
    {
        public DispatchableListBox()
            : base()
        {

        }

        protected void AddTextBoxToListBox(string action, Color background, Color foreground)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                this.AddListBoxEntry(
                    this.GetTextBlock(DateTime.Now.Ticks + ": " + action, background, foreground)
                    )
                )
            );
        }

        private void AddListBoxEntry(TextBlock tb)
        {
            this.Items.Add(tb);
            this.ScrollIntoView(tb);
        }

        private TextBlock GetTextBlock(string text, Color background, Color foreground)
        {
            return new TextBlock()
            {
                Text = text,
                Background = GetBrushFromColor(background),
                Foreground = GetBrushFromColor(foreground)
            };
        }

        private SolidColorBrush GetBrushFromColor(Color color)
        {
            return new SolidColorBrush(color);
        }
    }
}
