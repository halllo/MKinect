using System;
using System.Windows;

namespace MKinectUIExtensions.Trackers.HighlightCanvas.Viewport
{
    public interface INotifySizeChange
    {
        event Action<Size> NewSizeAvailable;

        double CurrentWidth { get; }
        double CurrentHeight { get; }
    }
}
