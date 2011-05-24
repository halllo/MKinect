
namespace MKinect.Body.Actions
{
    public class MovementSettings
    {
        public int XMoveThreshold { get; set; }
        public int YMoveThreshold { get; set; }
        public int ZMoveThreshold { get; set; }

        public MovementSettings()
        {
            this.XMoveThreshold = 20;
            this.YMoveThreshold = 20;
            this.ZMoveThreshold = 80;
        }
    }
}
