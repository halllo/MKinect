using System;
using MKinect.Body.Abstract;

namespace MKinect.Body.Actions
{
    public class MoveableBodyPart : BodyPartAction
    {
        public string Hint { get; set; }
        public MVector3D CurrentPosition { get; private set; }
        public MVector2D CurrentProjectedPosition { get; set; }
        public MovementSettings Settings { get; protected set; }

        private SkeletonSelector _bodyPart;
        private bool _lastWasMove;

        public event Action MoveLeft;
        public event Action MoveRight;
        public event Action MoveUp;
        public event Action MoveDown;
        public event Action Push;
        public event Action Pull;
        public event Action NoSignificantMovement;
        public event Action IsMoving;

        public MoveableBodyPart(Kinect kinect, SkeletonSelector bodyPart)
            : base(kinect)
        {
            this._bodyPart = bodyPart;
            this.SetupInfo();
            this.SetupEvents();
        }

        private void SetupInfo()
        {
            this.CurrentProjectedPosition = MVector2D.Projection(0, 0);
            this.Settings = new MovementSettings();
        }

        public double Distance2DTo(MoveableBodyPart otherBodyPart)
        {
            return MVector3D.Distance2D(this.CurrentPosition, otherBodyPart.CurrentPosition);
        }

        private void SetupEvents()
        {
            this.MoveDown += () => { };
            this.MoveUp += () => { };
            this.MoveLeft += () => { };
            this.MoveRight += () => { };
            this.Push += () => { };
            this.Pull += () => { };
            this.NoSignificantMovement += () => { };
            this.IsMoving += () => { };
        }

        protected override void SkeletonUpdate(uint user, Skeleton s)
        {
            MVector3D delta = this.GetMovingDelta(s);
            this.HandleDelta(delta);
        }

        private MVector3D GetMovingDelta(Skeleton s)
        {
            MVector3D newPosition = this._bodyPart.GetFirstBodyPart(s);
            if (this.CurrentPosition == null) this.CurrentPosition = newPosition;
            MVector3D delta = newPosition - this.CurrentPosition;
            this.CurrentPosition = newPosition;
            return delta;
        }

        private void HandleDelta(MVector3D delta)
        {
            this.DetermineIfAndHandleMove(delta);
            this.IsMoving();
        }

        private void DetermineIfAndHandleMove(MVector3D delta)
        {
            bool anyMove = this.HandleMovements(delta);
            if (this._lastWasMove && !anyMove) this.NoSignificantMovement();
            this._lastWasMove = anyMove;
        }

        private bool HandleMovements(MVector3D delta)
        {
            bool anyMove = false;
            anyMove |= Condition(delta.X >= this.Settings.XMoveThreshold, this.MoveRight);
            anyMove |= Condition(delta.X <= -this.Settings.XMoveThreshold, this.MoveLeft);
            anyMove |= Condition(delta.Y >= this.Settings.YMoveThreshold, this.MoveDown);
            anyMove |= Condition(delta.Y <= -this.Settings.YMoveThreshold, this.MoveUp);
            anyMove |= Condition(delta.Z >= this.Settings.ZMoveThreshold, this.Pull);
            anyMove |= Condition(delta.Z <= -this.Settings.ZMoveThreshold, this.Push);
            return anyMove;
        }
    }
}
