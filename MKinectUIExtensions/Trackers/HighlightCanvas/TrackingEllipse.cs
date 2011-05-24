using System;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MKinectUIExtensions.Trackers.HighlightCanvas
{
    public class TrackingEllipse : HighlightCanvasHighlight
    {
        private Ellipse _Highlight;

        public TrackingEllipse()
            : base()
        { }

        protected override System.Windows.UIElement CreateHighlight()
        {
            _Highlight = new Ellipse();
            _Highlight.Fill = Brushes.Black;
            _Highlight.Width = 20;
            _Highlight.Height = 20;
            return _Highlight;
        }

        public void ChangeColor(Brush brush)
        {
            _Highlight.Fill = brush;
        }
    }
}
