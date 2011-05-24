using System;

namespace MKinect.Body.Actions
{
    public static class MoveableBodyPartExtensions
    {
        public static RelativeMove MovingRelativly(this MoveableBodyPart moveable)
        {
            return new RelativeMove(moveable);
        }
    }

    public class RelativeMove
    {
        private MVector3D _centerPosition;
        private MoveableBodyPart _moveable;

        internal RelativeMove(MoveableBodyPart moveable)
        {
            this._moveable = moveable;
            this._centerPosition = null;
        }

        public RelativeMove RelativeTo(MVector3D position)
        {
            this._centerPosition = position;
            return this;
        }

        public RelativeMove On2DMove(Action<double, double> move)
        {
            this._moveable.MoveLeft += () => this.NotifyPositionRelativeToCenter(move);
            this._moveable.MoveRight += () => this.NotifyPositionRelativeToCenter(move);
            this._moveable.MoveUp += () => this.NotifyPositionRelativeToCenter(move);
            this._moveable.MoveDown += () => this.NotifyPositionRelativeToCenter(move);
            return this;
        }

        private void NotifyPositionRelativeToCenter(Action<double, double> move)
        {
            if (this._centerPosition != null)
            {
                var r2c = this._moveable.CurrentPosition - this._centerPosition;
                move(r2c.X, r2c.Y);
            }
        }
    }
}
