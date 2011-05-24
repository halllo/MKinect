using System;
using System.Windows;

namespace MKinectUIExtensions.Trackers.HighlightCanvas.Viewport
{
    public class ViewportPointConverter
    {
        private double vp_width;
        private double vp_height;
        private double w_width;
        private double w_height;
        private double w_x;
        private double w_y;

        public Remembering Shelve { get; private set; }

        public event Action<double, double> RelativeCoordinates;
        
        internal ViewportPointConverter(double width, double height)
        {
            this.Shelve = new Remembering();
            this.RelativeCoordinates += (x, y) => { };
            this.vp_width = width;
            this.vp_height = height;
        }

        internal void UpdateViewport(double width, double height)
        {
            this.vp_width = width;
            this.vp_height = height;
        }

        internal void UpdateWindow(double x, double y, double width, double height)
        {
            this.w_x = x;
            this.w_y = y;
            this.w_width = width;
            this.w_height = height;
        }

        public void AbsoluteCoordinates(Point point)
        {
            this.AbsoluteCoordinates(point.X, point.Y);
        }

        public void AbsoluteCoordinates(double x, double y)
        {
            this.AbsoluteCoordinates(x, y, (vpx, vpy) => 
                this.NotifyAboutRelativeCoordinates(vpx, vpy));
        }

        public void AbsoluteCoordinates(double x, double y, Action<double, double> result)
        {
            double vpx = (x - w_x) * vp_width / w_width;
            double vpy = (y - w_y) * vp_height / w_height;
            result(vpx, vpy);
        }

        private void NotifyAboutRelativeCoordinates(double rx, double ry)
        {
            this.RelativeCoordinates(rx, ry);
        }
    }
}
