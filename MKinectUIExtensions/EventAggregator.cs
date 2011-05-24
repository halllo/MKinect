using System;

namespace MKinectUIExtensions
{
    public class EventAggregator
    {
        private bool firstDone;

        public event Action OnAction;

        public EventAggregator()
        {
            this.firstDone = false;
            this.OnAction += () => { };
        }

        public void First()
        {
            if (!firstDone) firstDone = true;
        }

        public void Second()
        {
            if (firstDone)
            {
                firstDone = false;
                this.OnAction();
            }
        }
    }
}
