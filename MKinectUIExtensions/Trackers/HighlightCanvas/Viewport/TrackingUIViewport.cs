using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using MKinect.Body.Actions;

namespace MKinectUIExtensions.Trackers.HighlightCanvas.Viewport
{
    public class TrackingUIViewport : TrackingWindow
    {
        private Rectangle _displayVp;
        private ViewportedHighlightCanvas _canvas;

        protected TrackingUIViewport(MoveableBodyPart center, MoveableBodyPart circumferencePoint)
            : base(center, circumferencePoint)
        { }

        private Rectangle CreateRectangle()
        {
            return new Rectangle { StrokeThickness = 2, Stroke = Brushes.Black };
        }

        public static new TrackingUIViewport WithRadiusBetween(MoveableBodyPart center, MoveableBodyPart circumferencePoint)
        {
            return new TrackingUIViewport(center, circumferencePoint);
        }

        public TrackingUIViewport OnCanvas(ViewportedHighlightCanvas canvas)
        {
            this._displayVp = this.CreateRectangle();
            this._canvas = canvas;
            this.ReaddRectangle();
            return this;
        }

        public TrackingUIViewport Colorize(SolidColorBrush color)
        {
            this._displayVp.Stroke = color;
            return this;
        }

        protected override void TryToTransformRectangle()
        {
            base.TryToTransformRectangle();
            this._canvas.Dispatch(() => this.ReformWithoutViewport());
        }

        private void ReformWithoutViewport()
        {
            this._displayVp.SetValue(Canvas.LeftProperty, base.X);
            this._displayVp.SetValue(Canvas.TopProperty, base.Y);
            this._displayVp.Width = base.Width;
            this._displayVp.Height = base.Height;
        }

        private void ReaddRectangle()
        {
            if (this._canvas.Children.Contains(this._displayVp)) this._canvas.Children.Remove(this._displayVp);
            if (!this._canvas.Children.Contains(this._displayVp)) this._canvas.Children.Add(this._displayVp);
        }
    }
}
