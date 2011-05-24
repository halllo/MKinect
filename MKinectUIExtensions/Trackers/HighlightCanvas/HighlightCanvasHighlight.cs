using System;
using System.Windows;
using MKinect.Body.Actions;

namespace MKinectUIExtensions.Trackers.HighlightCanvas
{
    public abstract class HighlightCanvasHighlight
    {
        private UIElement _highlight;
        private RelativeMove _relMove;

        public UIElement AsUi { get { return this._highlight; } }
        public MoveableBodyPart BodyPart { get; private set; }

        public event Action<double, double> OnMove;

        public HighlightCanvasHighlight()
        {
            this._highlight = this.CreateHighlight();
            this.OnMove += (x, y) => { };
        }

        protected abstract UIElement CreateHighlight();

        public TrackingMove StartTracking(MoveableBodyPart bodyPart)
        {
            Action<Func<Point, Point>> onMove = (c) => new Point(0, 0);
            var onMoveSetter = new TrackingMove((o) => onMove = o);
            this.BodyPart = bodyPart;
            this.SetupBodyPartMove(() => onMove);
            return onMoveSetter;
        }

        private void SetupBodyPartMove(Func<Action<Func<Point, Point>>> onMove)
        {
            this.BodyPart.Settings.XMoveThreshold = 0;
            this.BodyPart.Settings.YMoveThreshold = 0;
            this._relMove = this.BodyPart.MovingRelativly();
            this._relMove.On2DMove((x, y) => onMove()((p) => new Point(p.X + x, p.Y + y)));
        }

        public RelativeActivision WhenActivated(Action<HighlightCanvasHighlight> activated)
        {
            Func<HighlightCanvasHighlight> relativeTo = () => this;
            var relativeToSetter = new RelativeActivision((r) => relativeTo = r);
            this.BodyPart.Push += () => this.ActivateAndHandleAndRelative(activated, () => relativeTo);
            return relativeToSetter;
        }

        public void ForceActivision(Action<HighlightCanvasHighlight> activated, Func<HighlightCanvasHighlight> relativeTo)
        {
            this.BodyPart.IsMoving += () => this.ActivateAndHandleAndRelative(activated, () => relativeTo);
        }

        private void ActivateAndHandleAndRelative(Action<HighlightCanvasHighlight> activated, Func<Func<HighlightCanvasHighlight>> relativeTo)
        {
            this._relMove.RelativeTo(relativeTo()().BodyPart.CurrentPosition);
            activated(this);
        }

        public void WhenDeactivated(Action<HighlightCanvasHighlight> deactivated)
        {
            this.BodyPart.Pull += () =>
            {
                deactivated(this);
            };
        }

        internal void MovedTo(double x, double y)
        {
            this.OnMove(x, y);
        }

        public class TrackingMove
        {
            private Action<Action<Func<Point, Point>>> _onMove;

            public TrackingMove(Action<Action<Func<Point, Point>>> onMove)
            {
                this._onMove = onMove;
            }

            public void WhenMoved(Action<Func<Point, Point>> onMove)
            {
                this._onMove(onMove);
            }
        }

        public class RelativeActivision
        {
            private Action<Func<HighlightCanvasHighlight>> _relativeTo;

            public RelativeActivision(Action<Func<HighlightCanvasHighlight>> relativeTo)
            {
                this._relativeTo = relativeTo;
            }

            public void RelativeTo(Func<HighlightCanvasHighlight> relativeTo)
            {
                this._relativeTo(relativeTo);
            }
        }
    }
}
