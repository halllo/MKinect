using System.Collections.Generic;
using System.Windows;
using MKinect.Body.Actions;

namespace MKinectUIExtensions.Trackers.HighlightCanvas
{
    public class HighlightCanvasItemContext
    {
        HighlightCanvas _canvas;
        List<UIElement> _elements;

        public HighlightCanvasItemContext(HighlightCanvas canvas)
        {
            _canvas = canvas;
            _elements = new List<UIElement>();
        }

        internal void SetOn(UIElement element)
        {
            _elements.Add(element);
        }

        public HighlightCanvasItemContextHandlers When(MoveableBodyPart bodyPart)
        {
            return new HighlightCanvasItemContextHandlers(bodyPart, _canvas, _elements);
        }

        private static void SetupHandler(UIElement element, HighlightCanvasItemContext context)
        {
            context.SetOn(element);
        }

        #region attached properties

        public static HighlightCanvasItemContext GetHandler(DependencyObject obj)
        {
            return (HighlightCanvasItemContext)obj.GetValue(HandlerProperty);
        }

        public static void SetHandler(DependencyObject obj, HighlightCanvasItemContext value)
        {
            obj.SetValue(HandlerProperty, value);
        }

        public static readonly DependencyProperty HandlerProperty =
            DependencyProperty.RegisterAttached("Handler", typeof(HighlightCanvasItemContext), typeof(HighlightCanvasItemContext),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(HandlerChanged)));

        private static void HandlerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var ic = e.NewValue as HighlightCanvasItemContext;
            if (ic != null) SetupHandler(sender as UIElement, ic);
        }

        #endregion
    }
}
