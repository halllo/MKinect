using System;
using System.Windows;
using System.Windows.Controls;
using MKinectUIExtensions.Trackers.HighlightCanvas.Viewport;

namespace MKinectUIExtensions.Trackers.HighlightCanvas
{
    public class CenterAwareCanvas : Canvas, INotifySizeChange
    {
        public Point CurrentCenter { get; private set; }
        public Size CurrentSize { get; private set; }

        public CenterAwareCanvas()
            : base()
        {
            this.NewSizeAvailable += (s) => { };
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            this.HandleNewSize(sizeInfo.NewSize);
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void HandleNewSize(Size newSize)
        {
            this.CurrentSize = newSize;
            this.CurrentCenter = new Point(CurrentSize.Width / 2, CurrentSize.Height / 2);
            this.NewSizeAvailable(CurrentSize);
        }

        public void Dispatch(Action todispatch)
        {
            this.Dispatcher.BeginInvoke(todispatch);
        }

        protected void DispatchConditional(Action todispatch, Action<Action<bool>> condition)
        {
            this.Dispatch(todispatch.If(condition));
        }

        protected void DispatchConditionalNot(Action todispatch, Action<Action<bool>> condition)
        {
            this.Dispatch(todispatch.If(condition.Not()));
        }

        public event Action<Size> NewSizeAvailable;


        public double CurrentWidth
        {
            get { return this.CurrentSize.Width; }
        }

        public double CurrentHeight
        {
            get { return this.CurrentSize.Height; }
        }
    }
}
