using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MKinect.Body.Actions;

namespace MKinectUIExtensions.Trackers.HighlightCanvas
{
    public abstract class HighlightCanvas : CenterAwareCanvas
    {
        private Dictionary<MoveableBodyPart, HighlightCanvasHighlight> _highlights;

        public HighlightCanvas()
            : base()
        {
            this._highlights = new Dictionary<MoveableBodyPart, HighlightCanvasHighlight>();
        }

        public void StartTracking(params MoveableBodyPart[] bodyParts)
        {
            foreach (var bodyPart in bodyParts) this._highlights.Add(bodyPart,
                StartTrackingForHighlight(this.NewHighlight(), bodyPart));
        }

        public void WhenMoved(MoveableBodyPart bodyPart, Action<double, double, MoveableBodyPart> handler)
        {
            RegisterMovingHandler(bodyPart, handler);
        }

        protected HighlightCanvasHighlight GetHighlightOf(MoveableBodyPart bodyPart)
        {
            return this._highlights[bodyPart];
        }

        protected void ForceActivision(HighlightCanvasHighlight hl)
        {
            hl.ForceActivision(this.ActivateHightlight, this.GetPrimaryHighlight);
        }

        protected abstract HighlightCanvasHighlight NewHighlight();

        protected abstract void MoveHighlightOnCanvas(HighlightCanvasHighlight hl, double x, double y);

        #region internals
        private void RegisterMovingHandler(MoveableBodyPart bodyPart, Action<double, double, MoveableBodyPart> handler)
        {
            _highlights[bodyPart].OnMove += (x, y) => handler(x, y, bodyPart);
        }

        private HighlightCanvasHighlight StartTrackingForHighlight(HighlightCanvasHighlight hl, MoveableBodyPart bodyPart)
        {
            hl.StartTracking(bodyPart).WhenMoved((center) => this.MoveHighlightToPoint(hl, center(this.CurrentCenter)));
            hl.WhenActivated(this.ActivateHightlight).RelativeTo(this.GetPrimaryHighlight);
            hl.WhenDeactivated(this.DeactivateHightlight);
            return hl;
        }

        private HighlightCanvasHighlight GetPrimaryHighlight()
        {
            return this._highlights.Values.First();
        }

        private void MoveHighlightToPoint(HighlightCanvasHighlight hl, Point p)
        {
            this.DispatchConditional(() => this.MoveHighlight(hl, p.X, p.Y),
                this.AlreadyActivated(hl));
        }

        private void MoveHighlight(HighlightCanvasHighlight hl, double x, double y)
        {
            this.MoveHighlightOnCanvas(hl, x, y);
        }

        private void ActivateHightlight(HighlightCanvasHighlight hl)
        {
            this.DispatchConditionalNot(() => this.AddHighlightToCanvasAndCenterIt(hl),
                this.AlreadyActivated(hl));
        }

        private void DeactivateHightlight(HighlightCanvasHighlight hl)
        {
            this.DispatchConditional(() => this.RemoveHightlightFromCanvas(hl),
                this.AlreadyActivated(hl));
        }

        private void AddHighlightToCanvasAndCenterIt(HighlightCanvasHighlight hl)
        {
            this.MoveHighlight(hl, this.CurrentCenter.X, this.CurrentCenter.Y);
            base.Children.Add(hl.AsUi);
        }

        private void RemoveHightlightFromCanvas(HighlightCanvasHighlight hl)
        {
            base.Children.Remove(hl.AsUi);
        }

        protected Action<Action<bool>> AlreadyActivated(HighlightCanvasHighlight hl)
        {
            return (a) => this.HighlightAlreadyActivated(hl, (b) => a(b));
        }

        private void HighlightAlreadyActivated(HighlightCanvasHighlight hl, Action<bool> activated)
        {
            this.Dispatch(() => activated(base.Children.Contains(hl.AsUi)));
        }
        #endregion
    }
}
