using System.Windows;
using System.Windows.Controls;

namespace MKinectUIExtensions.Trackers
{
    public static class Pointer
    {
        public static bool GetTracks(DependencyObject obj)
        {
            return (bool)obj.GetValue(TracksProperty);
        }

        public static void SetTracks(DependencyObject obj, bool value)
        {
            obj.SetValue(TracksProperty, value);
        }

        public static DependencyProperty TracksProperty =
            DependencyProperty.RegisterAttached("Tracks", typeof(bool), typeof(Pointer), new UIPropertyMetadata(false, Changed));

        private static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uie = d as UIElement;
            if ((bool)e.NewValue && uie != null) StartTracking(uie);
        }

        private static void StartTracking(UIElement uie)
        {
            uie.SetValue(Canvas.LeftProperty, 20.0);
            uie.SetValue(Canvas.TopProperty, 30.0);
        }
    }
}
