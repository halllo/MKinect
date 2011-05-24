using System.ComponentModel;
using System.Windows.Media;
using MKinect.Body.Actions;
using MKinectUIExtensions.Fluent;
using MKinectUIExtensions.Trackers.HighlightCanvas;

namespace GestureRun
{
    class UIInteractionVM : INotifyPropertyChanged
    {
        private HighlightCanvasItemContext _LeftColorBox;
        public HighlightCanvasItemContext LeftColorBox
        {
            get
            {
                return _LeftColorBox;
            }
            private set
            {
                _LeftColorBox = value;
                PropertyChanged(this, new PropertyChangedEventArgs("LeftColorBox"));
            }
        }
        private HighlightCanvasItemContext _RightColorBox;
        public HighlightCanvasItemContext RightColorBox
        {
            get
            {
                return _RightColorBox;
            }
            private set
            {
                _RightColorBox = value;
                PropertyChanged(this, new PropertyChangedEventArgs("RightColorBox"));
            }
        }

        private SolidColorBrush _RectColor1;
        public SolidColorBrush RectColor1
        {
            get
            {
                return _RectColor1;
            }
            private set
            {
                _RectColor1 = value;
                PropertyChanged(this, new PropertyChangedEventArgs("RectColor1"));
            }
        }
        private SolidColorBrush _RectColor2;
        public SolidColorBrush RectColor2
        {
            get
            {
                return _RectColor2;
            }
            private set
            {
                _RectColor2 = value;
                PropertyChanged(this, new PropertyChangedEventArgs("RectColor2"));
            }
        }

        public UIInteractionVM()
        {
            _RectColor1 = new SolidColorBrush(Colors.Gray);
            _RectColor2 = new SolidColorBrush(Colors.Gray);
        }

        public void Initialize(MoveableBodyPart leftHand, MoveableBodyPart rightHand, HighlightCanvas canvas)
        {
            LeftColorBox = new HighlightCanvasItemContext(canvas);
            RightColorBox = new HighlightCanvasItemContext(canvas);

            SpecifyActions(leftHand, rightHand);
        }

        private void SpecifyActions(MoveableBodyPart leftHand, MoveableBodyPart rightHand)
        {
            When.The(leftHand).Enters(LeftColorBox).Do(() => RectColor1 = new SolidColorBrush(Colors.Red));
            When.The(leftHand).Leaves(LeftColorBox).Do(() => RectColor1 = new SolidColorBrush(Colors.Gray));
            When.The(rightHand).Enters(LeftColorBox).Do(() => RectColor1 = new SolidColorBrush(Colors.Green));
            When.The(rightHand).Leaves(LeftColorBox).Do(() => RectColor1 = new SolidColorBrush(Colors.Gray));
            When.The(rightHand).Enters(RightColorBox).Do(() => RectColor2 = new SolidColorBrush(Colors.Blue));
            When.The(rightHand).Leaves(RightColorBox).Do(() => RectColor2 = new SolidColorBrush(Colors.Gray));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
