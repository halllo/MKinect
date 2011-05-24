using System;
using System.Windows.Controls;
using System.Windows.Media;
using MKinect.Body.Actions;

namespace MKinectUIExtensions.Trackers
{
    public class SizeableBorder : Border
    {
        private SpringBodyParts _bodyParts;
        
        public SizeableBorder()
            : base()
        {
            this.Background = Brushes.Blue;
            this.Width = this.Height = 50;
        }

        public void StartTracking(SpringBodyParts bodyParts)
        {
            this._bodyParts = bodyParts;
            this._bodyParts.Distance += Distance;
        }

        private void Distance(double distance)
        {
            this.Dispatch(() => this.TryToManipulate(distance));
        }

        private void TryToManipulate(double distance)
        {
            this.Height = this.Width = distance;
        }

        private void Dispatch(Action dispatch)
        {
            this.Dispatcher.BeginInvoke(dispatch);
        }
    }
}
