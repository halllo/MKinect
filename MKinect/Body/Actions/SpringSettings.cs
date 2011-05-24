
namespace MKinect.Body.Actions
{
    public class SpringSettings
    {
        public int CloseThreshold { get; set; }
        public int DistantThreshold { get; set; }

        public SpringSettings()
        {
            this.CloseThreshold = 40;
            this.DistantThreshold = 300;
        }
    }
}
