using System;
using System.Windows;
using System.Windows.Controls;
using MKinect;
using MKinect.UI;

namespace MKinectUIExtensions
{
    public static class Helper
    {
        public static void ShowKinectPreview(Kinect kinect, UIElement uie)
        {
            ShowKinectPreview(kinect, uie, true);
        }

        public static void ShowKinectPreview(Kinect kinect, UIElement uie, bool annotated)
        {
            uie.Dispatcher.BeginInvoke(new Action(() =>
            {
                Preview previewWindow = new Preview(kinect);
                previewWindow.Show();
                previewWindow.Annotated = annotated;
                previewWindow.UpdatePreviewEvery(300);
            }));
        }

        public static void DisplayKinectStatus(Kinect kinect, TextBlock textBlock)
        {
            kinect.StatusUpdate += (u, us) =>
            {
                textBlock.Dispatcher.BeginInvoke(new Action(() =>
                {
                    textBlock.Text = string.Format("user {0}: {1}", u, us.ToString());
                }));
            };
        }

        public static void CloseKinect(Kinect kinect)
        {
            if (kinect != null)
            {
                kinect.StopAndClose();
                kinect.Dispose();
            }
        }

        internal static Action If(this Action first, bool condition)
        {
            return () => { if (condition) first(); };
        }

        internal static Action If(this Action first, Func<bool> condition)
        {
            return () => { if (condition()) first(); };
        }

        internal static Action If(this Action first, Action<Action<bool>> condition)
        {
            return () => condition((b) => { if (b) first(); });
        }

        internal static Action<Action<bool>> Not(this Action<Action<bool>> condition)
        {
            return (a) => condition((b) => a(!b));
        }
    }
}
