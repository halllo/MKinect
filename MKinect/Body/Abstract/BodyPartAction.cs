using System;

namespace MKinect.Body.Abstract
{
    public abstract class BodyPartAction
    {
        private Kinect _kinect;
        
        public BodyPartAction(Kinect kinect)
        {
            this._kinect = kinect;
            if (this._kinect != null)
                this._kinect.SkeletonUpdate += new Action<uint, Skeleton>(this.SkeletonUpdate);
        }

        protected abstract void SkeletonUpdate(uint user, Skeleton s);

        protected bool Condition(bool check, Action body)
        {
            if (check) body();
            return check;
        }
    }
}
