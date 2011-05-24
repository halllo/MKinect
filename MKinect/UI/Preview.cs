using System.Windows.Forms;
using System.Drawing;

namespace MKinect.UI
{
    public partial class Preview : Form
    {
        private Kinect _kinect;
        
        public bool Annotated { get; set; }

        public Preview(Kinect kinect)
        {
            this._kinect = kinect;
            this.Annotated = true;
            InitializeComponent();
        }

        public void UpdatePreviewEvery(int ms)
        {
            Timer timer = new Timer();
            timer.Interval = ms;
            timer.Tick += (s, e) => this.SetNewKinectImage();
            timer.Start();
        }

        private void SetNewKinectImage()
        {
            var img = this.pictureBox1.Image;
            if (img != null) img.Dispose();
            this.pictureBox1.Image = this.GetKinectImage();
        }

        private Bitmap GetKinectImage()
        {
            if (this.Annotated)
                return this._kinect.GetScene();
            else
                return this._kinect.GetCameraImage();
        }
    }
}
