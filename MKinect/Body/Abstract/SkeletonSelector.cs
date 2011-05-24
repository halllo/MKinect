
namespace MKinect.Body.Abstract
{
    public abstract class SkeletonSelector
    {
        public abstract MVector3D GetFirstBodyPart(Skeleton s);
        public abstract MVector3D GetSecondBodyPart(Skeleton s);
    }
}
