using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using MKinect.Body.Actions;

namespace MKinectUIExtensions.Trackers.HighlightCanvas
{
    public class HighlightCanvasItemContextHandlers
    {
        HighlightCanvas _canvas;
        MoveableBodyPart _bodyPart;
        IEnumerable<UIElement> _elements;
        Dictionary<UIElement, bool> _selectionStates;

        public event Action<UIElement, MoveableBodyPart> Selected;
        public event Action<UIElement, MoveableBodyPart> Unselected;

        internal HighlightCanvasItemContextHandlers(MoveableBodyPart bodyPart, HighlightCanvas canvas, IEnumerable<UIElement> elements)
        {
            _bodyPart = bodyPart;
            _canvas = canvas;
            _elements = elements;
            _selectionStates = _elements.ToDictionary((e) => e, (e) => false);

            Selected += (e, m) => { };
            Unselected += (e, m) => { };
            SetupMoveEvents(bodyPart);
        }

        public HighlightCanvasItemContextHandlers Selects(Action toexe)
        {
            Selected += (e, m) => toexe();
            return this;
        }

        public HighlightCanvasItemContextHandlers Unselects(Action toexe)
        {
            Unselected += (e, m) => toexe();
            return this;
        }

        private void SetupMoveEvents(MoveableBodyPart bodyPart)
        {
            _canvas.WhenMoved(bodyPart, (x, y, m) =>
            {
                foreach (var element in _elements)
                {
                    DecideWhetherSelection(element, Hits(_canvas, element, x, y));
                }
            });
        }

        private void DecideWhetherSelection(UIElement element, bool hits)
        {
            if (hits && _selectionStates[element] == false)
            {
                Selected(element, _bodyPart);
                _selectionStates[element] = true;
            }
            if (!hits && _selectionStates[element] == true)
            {
                Unselected(element, _bodyPart);
                _selectionStates[element] = false;
            }
        }

        private bool Hits(UIElement area, UIElement element, double x, double y)
        {
            var htr = VisualTreeHelper.HitTest(area, new Point(x, y));
            return htr != null && htr.VisualHit.Equals(element);
        }
    }
}
