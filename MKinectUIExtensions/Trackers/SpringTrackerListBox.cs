using System.Windows.Media;
using MKinect.Body.Actions;

namespace MKinectUIExtensions.Trackers
{
    public class SpringTrackerListBox : DispatchableListBox
    {
        private SpringBodyParts _bodyParts;
        private Color closeColor = Colors.DarkOrange;
        private Color distantColor = Colors.DarkGray;
        private Color foreground = Colors.White;

        public SpringTrackerListBox()
            : base()
        {

        }

        public void StartTracking(SpringBodyParts bodyParts)
        {
            this._bodyParts = bodyParts;
            this._bodyParts.Close += Close;
            this._bodyParts.Distant += Distant;
        }

        private void Close()
        {
            this.AddTextBoxToListBox("close", closeColor, foreground);
        }

        private void Distant()
        {
            this.AddTextBoxToListBox("distant", distantColor, foreground);
        }
    }
}
