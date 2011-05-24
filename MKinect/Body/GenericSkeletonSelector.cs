using System;
using MKinect.Body.Abstract;

namespace MKinect.Body
{
    public class GenericSkeletonSelector : SkeletonSelector
    {
        private Func<Skeleton, MVector3D> _firstBodyPartSelector;
        private Func<Skeleton, MVector3D> _secondBodyPartSelector;

        public GenericSkeletonSelector(Func<Skeleton, MVector3D> firstBodyPart)
            : this(firstBodyPart, (s) => null)
        { }

        public GenericSkeletonSelector(Func<Skeleton, MVector3D> firstBodyPart, Func<Skeleton, MVector3D> secondBodyPart)
        {
            this._firstBodyPartSelector = firstBodyPart;
            this._secondBodyPartSelector = secondBodyPart;
        }

        public override MVector3D GetFirstBodyPart(Skeleton s)
        {
            return this._firstBodyPartSelector(s);
        }

        public override MVector3D GetSecondBodyPart(Skeleton s)
        {
            return this._secondBodyPartSelector(s);
        }
    }
}
