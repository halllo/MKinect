using MKinect.Body.Actions;

namespace MKinectUIExtensions.Fluent
{
    public class When
    {
        public static WhenCondition The(MoveableBodyPart bodyPart)
        {
            return new WhenCondition(bodyPart);
        }
    }
}
