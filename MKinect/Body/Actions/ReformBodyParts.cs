using System;
using MKinect.Body.Abstract;

namespace MKinect.Body.Actions
{
    public class ReformBodyParts : TwoBodyPartsAction
    {
        public event Action<double> RotateX;
        public event Action<double> RotateY;
        public event Action<double> RotateZ;
        public event Action<double, double, double> Rotate;

        public ReformBodyParts(Kinect kinect, SkeletonSelector bodyParts)
            : base(kinect, bodyParts)
        {
            this.SetupEvents();
        }

        private void SetupEvents()
        {
            this.RotateX += (x) => { };
            this.RotateY += (y) => { };
            this.RotateZ += (z) => { };
            this.Rotate += (x, y, z) => { };
        }

        protected override void BodyPartsUpdate(MVector3D first, MVector3D second)
        {
            var imaginedLine = second - first;
            this.NotifyRotation(
                this.GetAngleOnX(imaginedLine.Y, imaginedLine.Z),
                this.GetAngleOnY(imaginedLine.X, imaginedLine.Z),
                this.GetAngleOnZ(imaginedLine.X, imaginedLine.Y));
        }

        private double GetAngleOnX(double y, double z)
        {
            var rangle = Math.Atan2(y, z);
            return -this.RadianToDegree(rangle);
        }

        private double GetAngleOnY(double x, double z)
        {
            var rangle = Math.Atan2(z, x);
            return -this.RadianToDegree(rangle);
        }

        private double GetAngleOnZ(double x, double y)
        {
            var rangle = Math.Atan2(y, x);
            return -this.RadianToDegree(rangle);
        }

        private void NotifyRotation(double x, double y, double z)
        {
            this.RotateX(x);
            this.RotateY(y);
            this.RotateZ(z);
            this.Rotate(x, y, z);
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
    }
}
