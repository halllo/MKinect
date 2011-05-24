using System.Windows.Media;
using MKinect.Body.Actions;

namespace MKinectUIExtensions.Trackers
{
    public class MovementTrackerListBox : DispatchableListBox
    {
        private MoveableBodyPart _bodyPart;
        private Color pushColor = Colors.Blue;
        private Color pullColor = Colors.Green;
        private Color leftColor = Colors.Red;
        private Color rightColor = Colors.Brown;
        private Color noMoveColor = Colors.Gray;
        private Color foreground = Colors.White;

        public MovementTrackerListBox()
            : base()
        {

        }

        public void StartTracking(MoveableBodyPart bodyPart)
        {
            this._bodyPart = bodyPart;
            this._bodyPart.Push += Push;
            this._bodyPart.Pull += Pull;
            this._bodyPart.MoveLeft += MoveLeft;
            this._bodyPart.MoveRight += MoveRight;
            this._bodyPart.NoSignificantMovement += NoMove;
        }

        private void Pull()
        {
            base.AddTextBoxToListBox("pull", pullColor, foreground);
        }

        private void Push()
        {
            base.AddTextBoxToListBox("push", pushColor, foreground);
        }

        private void MoveLeft()
        {
            base.AddTextBoxToListBox("left", leftColor, foreground);
        }

        private void MoveRight()
        {
            base.AddTextBoxToListBox("right", rightColor, foreground);
        }

        private void NoMove()
        {
            base.AddTextBoxToListBox("..........", noMoveColor, foreground);
        }
    }
}
