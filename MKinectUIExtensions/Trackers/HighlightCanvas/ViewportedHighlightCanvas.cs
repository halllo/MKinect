using System.Windows;
using System.Windows.Controls;
using MKinect.Body;
using MKinect.Body.Actions;
using MKinectUIExtensions.Trackers.HighlightCanvas.Viewport;

namespace MKinectUIExtensions.Trackers.HighlightCanvas
{
    public abstract class ViewportedHighlightCanvas : HighlightCanvas
    {
        public ViewportedHighlightCanvas()
            : base()
        {
            this._Viewport = null;
        }

        private ViewportPointConverter _Viewport;
        public ViewportPointConverter Viewport
        {
            set
            {
                _Viewport = value;
                _Viewport.RelativeCoordinates += this.MoveRelativeRemembered;
            }
        }

        private void MoveRelativeRemembered(double x, double y)
        {
            var moveable = this._Viewport.Shelve.Remember<HighlightCanvasHighlight>();
            this.MoveRelative(moveable, x, y);
        }

        private void MoveRelative(HighlightCanvasHighlight moveable, double x, double y)
        {
            this.SetXY(moveable.AsUi, x, y);
            moveable.MovedTo(x, y);
        }

        public Point GetPositionOf(MoveableBodyPart bodyPart)
        {
            var projPos = bodyPart.CurrentProjectedPosition;
            return new Point(projPos.X, projPos.Y);
        }

        protected override void MoveHighlightOnCanvas(HighlightCanvasHighlight hl, double x, double y)
        {
            this.SetProjectedPositionBeforeViewport(hl, x, y);
            this.Move(hl, x, y);
        }

        private void SetProjectedPositionBeforeViewport(HighlightCanvasHighlight hl, double x, double y)
        {
            hl.BodyPart.CurrentProjectedPosition = MVector2D.Projection(x, y);
        }

        private void Move(HighlightCanvasHighlight hl, double x, double y)
        {
            if (this._Viewport != null)
                this.RememberAndMoveAbsolute(hl, x, y);
            else
                this.MoveRelative(hl, x, y);
        }

        private void RememberAndMoveAbsolute(HighlightCanvasHighlight hl, double x, double y)
        {
            this._Viewport.Shelve.Remember<HighlightCanvasHighlight>(hl);
            this._Viewport.AbsoluteCoordinates(x, y);
        }

        private void SetXY(UIElement ui, double x, double y)
        {
            if (!double.IsPositiveInfinity(x)) ui.SetValue(Canvas.LeftProperty, x);
            if (!double.IsPositiveInfinity(y)) ui.SetValue(Canvas.TopProperty, y);
        }
    }
}
