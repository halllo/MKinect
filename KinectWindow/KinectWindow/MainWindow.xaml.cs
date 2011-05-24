using System;
using System.Threading;
using System.Windows;
using MKinectUIExtensions;

namespace KinectWindow
{
    public partial class MainWindow : Window
    {
        HandControlVM handvm;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = handvm = new HandControlVM();
            this.Loaded += (_, __) => StartTracking();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Helper.CloseKinect(handvm.KSensor);
            base.OnClosing(e);
        }

        private void StartTracking()
        {
            ThreadPool.QueueUserWorkItem((_) =>
            {
                handvm.Init();
                Helper.ShowKinectPreview(handvm.KSensor, this);
                Helper.DisplayKinectStatus(handvm.KSensor, statusbox);
                Dispatch(() => handvm.StartTracking(this.trackingcanvas));
            });
        }

        private void Dispatch(Action toDispatch)
        {
            this.Dispatcher.BeginInvoke(toDispatch);
        }
    }
}
