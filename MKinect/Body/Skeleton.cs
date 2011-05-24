using System.Collections.Generic;

namespace MKinect.Body
{
    public class Skeleton
    {
        public MVector3D Head { get; private set; }
        public MVector3D Neck { get; private set; }
        public MVector3D LeftShoulder { get; private set; }
        public MVector3D LeftElbow { get; private set; }
        public MVector3D LeftHand { get; private set; }
        public MVector3D RightShoulder { get; private set; }
        public MVector3D RightElbow { get; private set; }
        public MVector3D RightHand { get; private set; }
        public MVector3D LeftHip { get; private set; }
        public MVector3D Torso { get; private set; }
        public MVector3D LeftKnee { get; private set; }
        public MVector3D LeftFoot { get; private set; }
        public MVector3D RightHip { get; private set; }
        public MVector3D RightKnee { get; private set; }
        public MVector3D RightFoot { get; private set; }

        private Skeleton()
        {
        }

        internal Skeleton(Dictionary<xn.SkeletonJoint, xn.SkeletonJointPosition> dictionary)
            : this()
        {
            this.AssignJoints(dictionary);
        }

        private void AssignJoints(Dictionary<xn.SkeletonJoint, xn.SkeletonJointPosition> dictionary)
        {
            this.Head = this.GetVectorFromJoint(dictionary, xn.SkeletonJoint.Head);
            this.Neck = this.GetVectorFromJoint(dictionary, xn.SkeletonJoint.Neck);
            this.Torso = this.GetVectorFromJoint(dictionary, xn.SkeletonJoint.Torso);
            this.LeftShoulder = this.GetVectorFromJoint(dictionary, xn.SkeletonJoint.LeftShoulder);
            this.LeftElbow = this.GetVectorFromJoint(dictionary, xn.SkeletonJoint.LeftElbow);
            this.LeftHand = this.GetVectorFromJoint(dictionary, xn.SkeletonJoint.LeftHand);
            this.RightShoulder = this.GetVectorFromJoint(dictionary, xn.SkeletonJoint.RightShoulder);
            this.RightElbow = this.GetVectorFromJoint(dictionary, xn.SkeletonJoint.RightElbow);
            this.RightHand = this.GetVectorFromJoint(dictionary, xn.SkeletonJoint.RightHand);
            this.LeftHip = this.GetVectorFromJoint(dictionary, xn.SkeletonJoint.LeftHip);
            this.LeftKnee = this.GetVectorFromJoint(dictionary, xn.SkeletonJoint.LeftKnee);
            this.LeftFoot = this.GetVectorFromJoint(dictionary, xn.SkeletonJoint.LeftFoot);
            this.RightHip = this.GetVectorFromJoint(dictionary, xn.SkeletonJoint.RightHip);
            this.RightKnee = this.GetVectorFromJoint(dictionary, xn.SkeletonJoint.RightKnee);
            this.RightFoot = this.GetVectorFromJoint(dictionary, xn.SkeletonJoint.RightFoot);
        }

        private MVector3D GetVectorFromJoint(Dictionary<xn.SkeletonJoint, xn.SkeletonJointPosition> joints, xn.SkeletonJoint joint)
        {
            return this.GetVector(joints[joint].position);
        }

        private MVector3D GetVector(xn.Point3D p)
        {
            return new MVector3D(p.X, p.Y, p.Z);
        }
    }
}
