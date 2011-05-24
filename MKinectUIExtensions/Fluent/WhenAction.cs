using System;
using System.Windows;
using MKinect.Body.Actions;

namespace MKinectUIExtensions.Fluent
{
    public class WhenAction
    {
        private Action<Action<UIElement, MoveableBodyPart>> _eventAction;

        internal WhenAction(Action<Action<UIElement, MoveableBodyPart>> eventAction)
        {
            _eventAction = eventAction;
        }

        public void Do(Action todo)
        {
            _eventAction((u, b) => todo());
        }

        public void Do(Action<UIElement, MoveableBodyPart> todo)
        {
            _eventAction(todo);
        }
    }
}
