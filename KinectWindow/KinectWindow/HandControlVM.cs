using System.ComponentModel;
using System.Windows.Media;
using MKinect;
using MKinect.Body;
using MKinect.Body.Actions;
using MKinectUIExtensions.Fluent;
using MKinectUIExtensions.Trackers.HighlightCanvas;
using MKinectUIExtensions.Trackers.HighlightCanvas.Viewport;

namespace KinectWindow
{
    class HandControlVM : INotifyPropertyChanged
    {
        public Kinect KSensor { private set; get; }

        #region bindable properties
        private HighlightCanvasItemContext _KinectSelectContext1;
        public HighlightCanvasItemContext LeftKinectBox
        {
            get
            {
                return _KinectSelectContext1;
            }
            private set
            {
                _KinectSelectContext1 = value;
                PropertyChanged(this, new PropertyChangedEventArgs("LeftKinectBox"));
            }
        }
        private HighlightCanvasItemContext _KinectSelectContext2;
        public HighlightCanvasItemContext RightKinectBox
        {
            get
            {
                return _KinectSelectContext2;
            }
            private set
            {
                _KinectSelectContext2 = value;
                PropertyChanged(this, new PropertyChangedEventArgs("RightKinectBox"));
            }
        }
        private SolidColorBrush _Brush1;
        public SolidColorBrush Brush1
        {
            get { return _Brush1; }
            set
            {
                _Brush1 = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Brush1"));
            }
        }
        private SolidColorBrush _Brush2;
        public SolidColorBrush Brush2
        {
            get { return _Brush2; }
            set
            {
                _Brush2 = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Brush2"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public HandControlVM()
        {
            _Brush1 = Brushes.Gray;
            _Brush2 = Brushes.Gray;
        }

        public void Init()
        {
            KSensor = new Kinect();
        }

        public void StartTracking(TrackingEllipseCanvas canvas)
        {
            var head = GenericActionFactory.Create<MoveableBodyPart>(KSensor, (s) => s.Head);
            var lHand = GenericActionFactory.Create<MoveableBodyPart>(KSensor, (s) => s.LeftHand);
            var rHand = GenericActionFactory.Create<MoveableBodyPart>(KSensor, (s) => s.RightHand);
            var lShoulder = GenericActionFactory.Create<MoveableBodyPart>(KSensor, (s) => s.LeftShoulder);
            var rShoulder = GenericActionFactory.Create<MoveableBodyPart>(KSensor, (s) => s.RightShoulder);

            SetupCanvas(canvas, head, lHand, rHand, lShoulder, rShoulder);
            SetupSelectionContext(canvas, lHand, rHand);
            SetupContextEvents(lHand, rHand);
        }

        private void SetupSelectionContext(TrackingEllipseCanvas canvas, MoveableBodyPart lHand, MoveableBodyPart rHand)
        {
            LeftKinectBox = new HighlightCanvasItemContext(canvas);
            RightKinectBox = new HighlightCanvasItemContext(canvas);
        }

        private void SetupContextEvents(MoveableBodyPart lHand, MoveableBodyPart rHand)
        {
            When.The(lHand).Enters(LeftKinectBox).Do(() => this.Brush1 = Brushes.Red);
            When.The(lHand).Enters(RightKinectBox).Do(() => this.Brush2 = Brushes.Red);
            When.The(rHand).Enters(LeftKinectBox).Do(() => this.Brush1 = Brushes.Blue);
            When.The(rHand).Enters(RightKinectBox).Do(() => this.Brush2 = Brushes.Blue);

            When.The(lHand).Leaves(LeftKinectBox).Do(() => this.Brush1 = Brushes.Gray);
            When.The(lHand).Leaves(RightKinectBox).Do(() => this.Brush2 = Brushes.Gray);
            When.The(rHand).Leaves(LeftKinectBox).Do(() => this.Brush1 = Brushes.Gray);
            When.The(rHand).Leaves(RightKinectBox).Do(() => this.Brush2 = Brushes.Gray);
        }

        private void SetupCanvas(TrackingEllipseCanvas canvas, MoveableBodyPart head, MoveableBodyPart lHand, MoveableBodyPart rHand, MoveableBodyPart lShoulder, MoveableBodyPart rShoulder)
        {
            canvas.StartTracking(lHand, rHand, head, lShoulder, rShoulder);
            canvas.ChangeColor(lHand, Brushes.Blue);
            canvas.ChangeColor(rHand, Brushes.Blue);
            canvas.ChangeColor(head, Brushes.LightGray);
            canvas.ChangeColor(lShoulder, Brushes.LightGray);
            canvas.ChangeColor(rShoulder, Brushes.LightGray);
            canvas.ForceActivision(head);
            canvas.ForceActivision(lShoulder);
            canvas.ForceActivision(rShoulder);
            canvas.Viewport = TrackingWindow.WithRadiusBetween(lShoulder, head).ResizedBy(2).Within(canvas);
        }
    }
}
