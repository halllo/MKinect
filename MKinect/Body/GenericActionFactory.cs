using System;
using MKinect.Body.Abstract;

namespace MKinect.Body
{
    public static class GenericActionFactory
    {
        public static T Create<T>(Kinect kinect, Func<Skeleton, MVector3D> bodyPart) where T : BodyPartAction
        {
            return CreateWithParams(typeof(T),
                kinect, new GenericSkeletonSelector(bodyPart)) as T;
        }

        public static T Create<T>(Kinect kinect, Func<Skeleton, MVector3D> firstBodyPart, Func<Skeleton, MVector3D> secondBodyPart) where T : TwoBodyPartsAction
        {
            return CreateWithParams(typeof(T), 
                kinect, new GenericSkeletonSelector(firstBodyPart, secondBodyPart)) as T;
        }

        private static object CreateWithParams(Type t, params object[] args)
        {
            return Activator.CreateInstance(t, args);
        }
    }
}
