using System.Windows.Media;
using MKinect.Body.Actions;

namespace MKinectUIExtensions.Trackers.HighlightCanvas
{
    public class TrackingEllipseCanvas : ViewportedHighlightCanvas
    {
        public TrackingEllipseCanvas()
            : base()
        { }

        protected override HighlightCanvasHighlight NewHighlight()
        {
            return new TrackingEllipse();
        }

        public void ChangeColor(MoveableBodyPart bodyPart, Brush color)
        {
            var highlight = base.GetHighlightOf(bodyPart) as TrackingEllipse;
            highlight.ChangeColor(color);
        }

        public void ForceActivision(MoveableBodyPart bodyPart)
        {
            base.ForceActivision(base.GetHighlightOf(bodyPart));
        }
    }
}
