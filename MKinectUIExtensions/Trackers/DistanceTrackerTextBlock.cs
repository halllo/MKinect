using System;
using System.Windows.Controls;
using MKinect.Body;
using MKinect.Body.Actions;

namespace MKinectUIExtensions.Trackers
{
    public class DistanceTrackerTextBlock : TextBlock
    {
        private SpringBodyParts _bodyParts;

        public DistanceTrackerTextBlock()
            : base()
        {

        }

        public void StartTracking(SpringBodyParts bodyParts)
        {
            this._bodyParts = bodyParts;
            this._bodyParts.Distance += (s) => this.RefreshInfo();
        }

        private void RefreshInfo()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.Text = MVector3D.Distance(
                    this._bodyParts.FirstPartPosition,
                    this._bodyParts.SecondPartPosition,
                    (v) => v.Z).ToString();
            }));
        }
    }
}
