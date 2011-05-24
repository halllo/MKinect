using System;
using System.Threading;
using System.Windows;
using MKinect;
using MKinect.Body;
using MKinect.Body.Actions;
using MKinectUIExtensions;
using MKinectUIExtensions.Trackers.HighlightCanvas.Viewport;

namespace GestureRun
{
    public partial class MainWindow : Window
    {
        Kinect kinect;
        UIInteractionVM vm;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.tracker_canvas.DataContext = vm = new UIInteractionVM();
            ThreadPool.QueueUserWorkItem((o) => this.StartTracking());
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Helper.CloseKinect(this.kinect);
            base.OnClosing(e);
        }

        private void Dip(Action toDispatch)
        {
            this.Dispatcher.BeginInvoke(toDispatch);
        }

        private void StartTracking()
        {
            this.kinect = new Kinect();

            Helper.ShowKinectPreview(this.kinect, this);
            Helper.DisplayKinectStatus(this.kinect, this.status);

            this.tracker_lefthand.StartTracking(
                GenericActionFactory.Create<MoveableBodyPart>(this.kinect, (s) => s.LeftHand));

            this.tracker_handspring.StartTracking(
                GenericActionFactory.Create<SpringBodyParts>(this.kinect, (s) => s.LeftHand, (s) => s.RightHand));

            this.tracker_righthand.StartTracking(
                GenericActionFactory.Create<MoveableBodyPart>(this.kinect, (s) => s.RightHand));

            this.tracker_sizeablesquare.StartTracking(
                GenericActionFactory.Create<SpringBodyParts>(this.kinect, (s) => s.LeftHand, (s) => s.RightHand));

            this.tracker_distance.StartTracking(
                GenericActionFactory.Create<SpringBodyParts>(this.kinect, (s) => s.LeftHand, (s) => s.RightHand));

            this.tracker_rollingdice.StartTracking(
                GenericActionFactory.Create<ReformBodyParts>(this.kinect, (s) => s.LeftHand, (s) => s.RightHand));

            this.Dip(() => this.StartTrackingOnHighlightCanvas());

            this.SetupNext();
        }

        private void SetupNext()
        {
            var foot = GenericActionFactory.Create<MoveableBodyPart>(this.kinect, (s) => s.LeftFoot);
            var ea = new EventAggregator();
            ea.OnAction += () => this.Dip(() => this.tabs.SelectedIndex = (this.tabs.SelectedIndex + 1) % 4);
            foot.Push += () => ea.First();
            foot.NoSignificantMovement += () => ea.Second();
        }

        private void StartTrackingOnHighlightCanvas()
        {
            var head = GenericActionFactory.Create<MoveableBodyPart>(this.kinect, (s) => s.Head); head.Hint = "head";
            var lHand = GenericActionFactory.Create<MoveableBodyPart>(this.kinect, (s) => s.LeftHand); lHand.Hint = "lhand";
            var rHand = GenericActionFactory.Create<MoveableBodyPart>(this.kinect, (s) => s.RightHand); rHand.Hint = "rhand";
            var lShoulder = GenericActionFactory.Create<MoveableBodyPart>(this.kinect, (s) => s.LeftShoulder); lShoulder.Hint = "lshoulder";
            var rShoulder = GenericActionFactory.Create<MoveableBodyPart>(this.kinect, (s) => s.RightShoulder); rShoulder.Hint = "rshoulder";

            this.tracker_canvas.StartTracking(head, lHand, rHand, lShoulder, rShoulder);
            this.vm.Initialize(lHand, rHand, this.tracker_canvas);

            this.tracker_canvas.ChangeColor(head, System.Windows.Media.Brushes.Blue);
            this.tracker_canvas.ChangeColor(lHand, System.Windows.Media.Brushes.Black);
            this.tracker_canvas.ChangeColor(rHand, System.Windows.Media.Brushes.Black);
            this.tracker_canvas.ChangeColor(lShoulder, System.Windows.Media.Brushes.Red);
            this.tracker_canvas.ChangeColor(rShoulder, System.Windows.Media.Brushes.Red);
            this.tracker_canvas.ForceActivision(head);
            this.tracker_canvas.ForceActivision(lShoulder);
            this.tracker_canvas.ForceActivision(rShoulder);

            this.tracker_canvas.Viewport = TrackingWindow.WithRadiusBetween(head, lShoulder).ResizedBy(1.2).Within(this.tracker_canvas);

        }
    }
}
