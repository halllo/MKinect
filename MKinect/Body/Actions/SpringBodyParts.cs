using System;
using MKinect.Body.Abstract;

namespace MKinect.Body.Actions
{
    public class SpringBodyParts : TwoBodyPartsAction
    {
        public SpringSettings Settings { get; protected set; }
        
        public event Action Close;
        public event Action Distant;
        public event Action<double> Distance;

        public SpringBodyParts(Kinect kinect, SkeletonSelector bodyParts)
            : base(kinect, bodyParts)
        {
            this.Settings = new SpringSettings();
            this.SetupEvents();
        }

        private void SetupEvents()
        {
            this.Close += () => { };
            this.Distant += () => { };
            this.Distance += (s) => { };
        }

        protected override void BodyPartsUpdate(MVector3D first, MVector3D second)
        {
            this.HandleDistance(MVector3D.Distance(first, second));
        }

        private void HandleDistance(double distance)
        {
            if (distance >= this.Settings.DistantThreshold) this.Distant();
            if (distance <= this.Settings.CloseThreshold) this.Close();
            this.Distance(distance);
        }
    }
}
