using System;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CadDrawer.Class;
using CadDrawer.CustomControl;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace CadDrawer.ViewModel
{ 
    public class MainViewModel : ViewModelBase
    {
        private Stopwatch watch = new Stopwatch();

        private LiveImage uiliveImage;

        private int imageWidth = 200;

        public int ImageWidth
        {
            get => imageWidth;
            set { Set(() => ImageWidth, ref imageWidth, value); }
        }

        private int imageHeight = 200;

        public int ImageHeight
        {
            get => imageHeight;
            set { Set(() => ImageHeight, ref imageHeight, value); }
        }

        private int cadRowCount = 20;
        public int CadRowCount
        {
            get => cadRowCount;
            set { Set(() => CadRowCount, ref cadRowCount, value); }
        }

        private int cadColumnCount = 1;
        public int CadColumnCount
        {
            get => cadColumnCount;
            set { Set(() => CadColumnCount, ref cadColumnCount, value); }
        }

        //private int cadWidth = 20;
        //public int CadWidth
        //{
        //    get => cadWidth;
        //    set { Set(() => CadWidth, ref cadWidth, value); }
        //}

        //private int cadHeight = 20;
        //public int CadHeight
        //{
        //    get => cadHeight;
        //    set { Set(() => CadHeight, ref cadHeight, value); }
        //}

        private int newImageTimeMs = 0;
        public int NewImageTimeMs
        {
            get => newImageTimeMs;
            set { Set(() => NewImageTimeMs, ref newImageTimeMs, value); }
        }

        private int drawTimeMs = 0;
        public int DrawTimeMs
        {
            get => drawTimeMs;
            set { Set(() => DrawTimeMs, ref drawTimeMs, value); }
        }

        private int uiRenderTimeMs = 0;
        public int UiRenderTimeMs
        {
            get => uiRenderTimeMs;
            set { Set(() => UiRenderTimeMs, ref uiRenderTimeMs, value); }
        }

        private WriteableBitmap imageBitmap;

        public WriteableBitmap ImageBitmap
        {
            get => imageBitmap;
            set { Set(() => ImageBitmap, ref imageBitmap, value); }
        }

        private Point liveMousePoint;

        public Point LiveMousePoint
        {
            get => liveMousePoint;
            set
            {
                Set(() => LiveMousePoint, ref liveMousePoint, value);
            }
        }

        private RectSelector rectSelector =  new RectSelector();

        public RectSelector RectSelector
        {
            get { return rectSelector; }
            set { Set(() => RectSelector, ref rectSelector, value); }
        }

        public RelayCommand CreateAndDrawImageCommand { get; set; }
        public RelayCommand<LiveImage> MouseRightButtonDownCommand { get; set; }
        public RelayCommand MouseMoveCommand { get; set; }
        public RelayCommand MouseRightButtonUpCommand { get; set; }

        public MainViewModel()
        {
            CreateAndDrawImageCommand = new RelayCommand(CreateAndDrawImageAction);
            MouseRightButtonDownCommand = new RelayCommand<LiveImage>(MouseRightButtonDownAction);
            MouseMoveCommand = new RelayCommand(MouseMoveAction);
            MouseRightButtonUpCommand = new RelayCommand(MouseRightButtonUpAction);
        }

        private void MouseRightButtonUpAction()
        { 
            RectSelector.IsMousePress = false;
            RectSelector.IsRectVisible = Visibility.Collapsed; 
        }

        private void MouseMoveAction()
        {

            if ( RectSelector.IsMousePress == false)
                return;

            RectSelector.EndPoint = Mouse.GetPosition(uiliveImage); ;

            RectSelector.SelectionRectangle = new Rect(Math.Min(RectSelector.EndPoint.X, RectSelector.StartPoint.X),
                Math.Min(RectSelector.EndPoint.Y, RectSelector.StartPoint.Y),
                Math.Abs(RectSelector.EndPoint.X - RectSelector.StartPoint.X),
                Math.Abs(RectSelector.EndPoint.Y - RectSelector.StartPoint.Y));

            RectSelector.RectWidth = RectSelector.SelectionRectangle.Width;
            RectSelector.RectHeight = RectSelector.SelectionRectangle.Height;
            RectSelector.RectCanvasLeft = RectSelector.SelectionRectangle.Left;
            RectSelector.RectCanvasTop = RectSelector.SelectionRectangle.Top;
            RectSelector.IsRectVisible = Visibility.Visible;
        }

        private void MouseRightButtonDownAction(LiveImage liveImage)
        {
            uiliveImage = liveImage;
            
             
            RectSelector.StartPoint = Mouse.GetPosition(uiliveImage);
            RectSelector.EndPoint = Mouse.GetPosition(uiliveImage);
            RectSelector.IsMousePress = true; 
        }

        private void CreateAndDrawImageAction()
        {
            watch.Restart();
            ImageBitmap = new WriteableBitmap(imageWidth, imageHeight, 96,96, PixelFormats.Gray8,null);
            NewImageTimeMs = (int)watch.ElapsedMilliseconds;

           
            watch.Restart();
            unsafe
            {
                var imageBuffer = (byte*)ImageBitmap.BackBuffer.ToPointer();
                DrawCad((byte*)imageBuffer,imageWidth,imageHeight,cadRowCount,cadColumnCount);
            } 
            DrawTimeMs = (int)watch.ElapsedMilliseconds;

            watch.Restart();
            ImageBitmap.Lock();
            ImageBitmap.AddDirtyRect(new Int32Rect(0,0,imageWidth,ImageHeight));
            ImageBitmap.Unlock();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                new Action(delegate { }));

            UiRenderTimeMs = (int)watch.ElapsedMilliseconds;
        }

        private unsafe void DrawCad(byte* image,int width,int height,int cadRowCount,int cadColumnCount)
        {
           int cadWidth = imageWidth / (cadColumnCount * 2 + 1);
           int cadHeight = imageHeight / (cadRowCount * 2 + 1);
              

            for (int i = 0; i < cadRowCount; i++)
            {
                for (int j = 0; j < cadColumnCount; j++)
                {
                    DrawSingCad(image + (2*i+1)*cadHeight*imageWidth + (2*j + 1)*cadWidth ,width,height ,cadWidth, cadHeight);
                   
                     
                } 
              
            }
        }

        private unsafe void DrawSingCad(byte* image,int imageWidth,int imageHeight,int cadWidth,int cadHeight)
        {

            byte* pTempImage = image;
            for (int i = 0; i < cadHeight; i++)
            {
                for (int j = 0; j < cadWidth; j++)
                {
                    *(pTempImage + i * imageWidth +  j) = 255;
                }
                 
                
            }
        }
    }
}