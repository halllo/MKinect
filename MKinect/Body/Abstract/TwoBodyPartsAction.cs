
namespace MKinect.Body.Abstract
{
    public abstract class TwoBodyPartsAction : BodyPartAction
    {
        private SkeletonSelector _bodyParts;

        public MVector3D FirstPartPosition { get; private set; }
        public MVector3D SecondPartPosition { get; private set; }

        public TwoBodyPartsAction(Kinect kinect, SkeletonSelector bodyParts)
            : base(kinect)
        {
            this._bodyParts = bodyParts;
        }

        protected override void SkeletonUpdate(uint user, Skeleton s)
        {
            this.FirstPartPosition = this._bodyParts.GetFirstBodyPart(s);
            this.SecondPartPosition = this._bodyParts.GetSecondBodyPart(s);
            this.BodyPartsUpdate(this.FirstPartPosition, this.SecondPartPosition);
        }

        protected abstract void BodyPartsUpdate(MVector3D first, MVector3D second);
    }
}
