
namespace MKinectUIExtensions.Trackers.HighlightCanvas.Viewport
{
    public class Remembering
    {
        private object t;

        public Remembering()
        {
            this.t = null;
        }

        public void Remember<T>(T t)
        {
            this.t = t;
        }

        public T Remember<T>()
        {
            return (T)this.t;
        }
    }
}
