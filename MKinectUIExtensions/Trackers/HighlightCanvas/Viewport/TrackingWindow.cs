using System.Windows;
using MKinect.Body.Actions;

namespace MKinectUIExtensions.Trackers.HighlightCanvas.Viewport
{
    public class TrackingWindow
    {
        private MoveableBodyPart _center;
        private MoveableBodyPart _circumferencePoint;
        private ViewportPointConverter _pointConverter;

        public double ResizeFactor { get; set; }

        public double X { get; protected set; }
        public double Y { get; protected set; }
        public double Width { get; protected set; }
        public double Height { get; protected set; }

        protected TrackingWindow(MoveableBodyPart center, MoveableBodyPart circumferencePoint)
        {
            this.SetPointInformation(center, circumferencePoint);
            this.ResizeFactor = 1;
            this.SetupMoveEvent();
        }

        private void SetPointInformation(MoveableBodyPart center, MoveableBodyPart circumferencePoint)
        {
            this._pointConverter = null;
            this._center = center;
            this._circumferencePoint = circumferencePoint;
        }

        public static TrackingWindow WithRadiusBetween(MoveableBodyPart center, MoveableBodyPart circumferencePoint)
        {
            return new TrackingWindow(center, circumferencePoint);
        }

        public virtual TrackingWindow ResizedBy(double factor)
        {
            this.ResizeFactor = factor;
            return this;
        }

        public ViewportPointConverter Within(double width, double height)
        {
            return this._pointConverter = new ViewportPointConverter(width, height);
        }

        public ViewportPointConverter Within(INotifySizeChange sizeChanged)
        {
            this._pointConverter = new ViewportPointConverter(sizeChanged.CurrentWidth, sizeChanged.CurrentHeight);
            sizeChanged.NewSizeAvailable += (s) => this._pointConverter.UpdateViewport(s.Width, s.Height);
            return this._pointConverter;
        }

        public ViewportPointConverter Within(Size size)
        {
            return this.Within(size.Width, size.Height);
        }

        #region internals
        private void SetupMoveEvent()
        {
            this._center.IsMoving += this.CenterMoved;
        }

        private void CenterMoved()
        {
            this.AdjustRectangle();
        }

        private void AdjustRectangle()
        {
            this.TryToTransformRectangle();
            if (this._pointConverter != null)
                this._pointConverter.UpdateWindow(this.X, this.Y, this.Width, this.Height);
        }

        protected virtual void TryToTransformRectangle()
        {
            if (this._center.CurrentPosition != null && this._circumferencePoint.CurrentPosition != null)
                TransformRectangle();
        }

        private void TransformRectangle()
        {
            var radius = this._center.Distance2DTo(this._circumferencePoint) * this.ResizeFactor;
            this.SetValuesBasedOnRadius(
                this._center.CurrentProjectedPosition.X,
                this._center.CurrentProjectedPosition.Y,
                radius);
        }

        private void SetValuesBasedOnRadius(double x, double y, double radius)
        {
            this.X = x - radius;
            this.Y = y - radius;
            this.Width = radius * 2;
            this.Height = radius * 2;
        }
        #endregion
    }
}
