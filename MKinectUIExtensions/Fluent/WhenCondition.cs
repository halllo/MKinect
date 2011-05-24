using MKinect.Body.Actions;
using MKinectUIExtensions.Trackers.HighlightCanvas;

namespace MKinectUIExtensions.Fluent
{
    public class WhenCondition
    {
        private MoveableBodyPart _bodyPart;

        internal WhenCondition(MoveableBodyPart bodyPart)
        {
            _bodyPart = bodyPart;
        }

        public WhenAction Enters(HighlightCanvasItemContext element)
        {
            HighlightCanvasItemContextHandlers handler = element.When(_bodyPart);
            return new WhenAction((a) => handler.Selected += a);
        }

        public WhenAction Leaves(HighlightCanvasItemContext element)
        {
            HighlightCanvasItemContextHandlers handler = element.When(_bodyPart);
            return new WhenAction((a) => handler.Unselected += a);
        }
    }
}
