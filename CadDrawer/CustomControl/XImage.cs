using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;

namespace CadDrawer.CustomControl
{
    public class LiveImage : Grid
    {
        private const double SCALE_RATIO = 1.1;
        private double Scale = 1;

        private Canvas imageCanvas = new Canvas();
        private Image imageData { get; set; } = new Image();
        private Image grayImageData { get; set; } = new Image();

        private Canvas crossCanvas = new Canvas() { Visibility = Visibility.Collapsed };
        private Canvas centerRect = new Canvas() { Visibility = Visibility.Collapsed };

        private Canvas focusSquareCanvas = new Canvas() { Visibility = Visibility.Visible };

        private double FovImagePositionX;
        private double FovImagePositionY;


        private Point mouseStart = new Point(0, 0);

        private ScaleTransform scaleTransform = new ScaleTransform(1, 1);

        public LiveImage()
        {
            RenderOptions.SetBitmapScalingMode(imageData, BitmapScalingMode.NearestNeighbor);
            imageCanvas.ClipToBounds = true;
            Children.Add(imageCanvas);
            imageCanvas.Children.Add(imageData);
            imageData.RenderTransform = scaleTransform;
            imageData.Stretch = Stretch.UniformToFill;
            Children.Add(crossCanvas);
            Children.Add(centerRect);
            Children.Add(focusSquareCanvas);
            MouseWheel += OnMouseWheel;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;

        
        }

        ~LiveImage()
        {
            Messenger.Default.Unregister(this);
        }

       

        

       
        private void CreateCenterRect(double _width, double _height, double _ratio, double xdiff, double ydiff)
        {
            centerRect.Children.Clear();


            centerRect.Width = _width;
            centerRect.Height = _height;

            Rectangle rect = new Rectangle()
            {
                IsHitTestVisible = false,
                Fill = Brushes.Transparent,
                Stroke = Brushes.YellowGreen,
                StrokeThickness = 3 * _ratio,
                Width = _width * 0.5 * _ratio,
                Height = _height * 0.5 * _ratio
            };

            Canvas.SetLeft(rect, xdiff + rect.Width * 0.5);
            Canvas.SetTop(rect, ydiff + rect.Height * 0.5);

            centerRect.Children.Add(rect);
        }

        private void CreateCrossCanvasData(double _width, double _height, double _ratio, double xdiff, double ydiff)
        {
            crossCanvas.Children.Clear();
            crossCanvas.Width = _width;
            crossCanvas.Height = _height;

            Line horizonLine = new Line()
            {
                X1 = 0,
                Y1 = ydiff + _height / 2 * _ratio,
                X2 = _width,
                Y2 = ydiff + _height / 2 * _ratio,
                Stroke = Brushes.Salmon,
                StrokeThickness = 3 * _ratio
            };

            Line verticleLine = new Line()
            {
                X1 = xdiff + _width / 2 * _ratio,
                Y1 = 0,
                X2 = xdiff + _width / 2 * _ratio,
                Y2 = _height,
                Stroke = Brushes.Salmon,
                StrokeThickness = 3 * _ratio
            };

            crossCanvas.Children.Add(horizonLine);
            crossCanvas.Children.Add(verticleLine);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(this);
            UpdateFovInfo(p);
            if (e.LeftButton == MouseButtonState.Released) return;


            var shiftX = p.X - mouseStart.X;
            var shiftY = p.Y - mouseStart.Y;

            if (!IsRepairedFovImagePosition(shiftX, shiftY))
            {
                FovImagePositionX = FovImagePositionX + shiftX;
                FovImagePositionY = FovImagePositionY + shiftY;
            }

            mouseStart = p;


            UpdateUI();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseStart = e.GetPosition(this);
        }



        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var p = e.GetPosition(this);

            var dx = p.X - FovImagePositionX;
            var dy = p.Y - FovImagePositionY;

            if (e.Delta >= 0)
            {
                Scale = Scale * SCALE_RATIO;
                dx = dx * SCALE_RATIO;
                dy = dy * SCALE_RATIO;
            }
            else
            {
                Scale = Scale / SCALE_RATIO;
                dx = dx / SCALE_RATIO;
                dy = dy / SCALE_RATIO;
            }

            if (Scale < 1)
            {
                Scale = 1;
                FovImagePositionX = 0;
                FovImagePositionY = 0;
            }
            else
            {
                FovImagePositionX = p.X - dx;
                FovImagePositionY = p.Y - dy;
            }

            IsRepairedFovImagePosition();

            UpdateUI();
        }


        private bool IsRepairedFovImagePosition(double shiftX = 0, double shiftY = 0)
        {
            bool isRepaired = false;
            if (FovImagePositionX + shiftX > 0)
            {
                FovImagePositionX = 0;
                isRepaired = true;
            }
            else if (FovImagePositionX + shiftX < imageData.Width - ((WriteableBitmap)imageData.Source).PixelWidth * Scale)
            {
                FovImagePositionX = imageData.Width - ((WriteableBitmap)imageData.Source).PixelWidth * Scale;
                isRepaired = true;
            }

            if (FovImagePositionY + shiftY > 0)
            {
                FovImagePositionY = 0;
                isRepaired = true;
            }
            else if (FovImagePositionY + shiftY < imageData.Height - ((WriteableBitmap)imageData.Source).PixelHeight * Scale)
            {
                FovImagePositionY = imageData.Height - ((WriteableBitmap)imageData.Source).PixelHeight * Scale;
                isRepaired = true;
            }
            return isRepaired;
        }

        private void UpdateUI()
        {
            Canvas.SetLeft(imageData, FovImagePositionX);
            Canvas.SetTop(imageData, FovImagePositionY);

            scaleTransform.ScaleX = Scale;
            scaleTransform.ScaleY = Scale;

            CreateCenterRect(Width, Height, Scale, FovImagePositionX, FovImagePositionY);
            CreateCrossCanvasData(Width, Height, Scale, FovImagePositionX, FovImagePositionY); 
        }

        public void UpdateFovInfo(Point p)
        {
            var x = (int)((p.X - FovImagePositionX) / Scale);
            var y = (int)((p.Y - FovImagePositionY) / Scale);
            
            SetValue(CurrentMousePointProperty, new Point(x, y));
        }

        public void SetCenterRectVisibility(bool isVisibility)
        {
            centerRect.Visibility = isVisibility ? Visibility.Visible : Visibility.Collapsed;
        }

        public void SetCrossVisibility(bool isVisibility)
        {
            crossCanvas.Visibility = isVisibility ? Visibility.Visible : Visibility.Collapsed;
        }
         

        public void SetImageSouce(ImageSource source)
        {
            imageData.Source = source;

            if (source == null)
                return;

            imageCanvas.Width = source.Width;
            imageCanvas.Height = source.Height;

            imageData.Width = source.Width;
            imageData.Height = source.Height;

            CreateCrossCanvasData(Width, Height, Scale, FovImagePositionX, FovImagePositionY);
            CreateCenterRect(Width, Height, Scale, FovImagePositionX, FovImagePositionY);
        }

        public void SetGrayImageSource(ImageSource source)
        {
            if (source == null)
                return;

            grayImageData.Source = source;
            grayImageData.Width = source.Width;
            grayImageData.Height = source.Height;
        }

        #region CurrentMousePoint
        public static readonly DependencyProperty CurrentMousePointProperty = DependencyProperty.RegisterAttached(
            "CurrentMousePoint",
            typeof(Point),
            typeof(LiveImage)
        );
        public static void SetCurrentMousePoint(UIElement element, Point value)
        {
            element.SetValue(CurrentMousePointProperty, value);
        }
        public static Point GetCurrentMousePoint(UIElement element)
        {
            return (Point)element.GetValue(CurrentMousePointProperty);
        }

        #endregion

        #region CurrentGrayScale
        public static readonly DependencyProperty CurrentGrayScaleProperty = DependencyProperty.RegisterAttached(
            "CurrentGrayScale",
            typeof(int),
            typeof(LiveImage)
        );
        public static void SetCurrentGrayScale(UIElement element, int value)
        {
            element.SetValue(CurrentMousePointProperty, value);
        }
        public static int GetCurrentGrayScale(UIElement element)
        {
            return (int)element.GetValue(CurrentMousePointProperty);
        }

        #endregion

        #region LiveSource
        public static readonly DependencyProperty LiveSourceProperty = DependencyProperty.RegisterAttached(
            "LiveSource",
            typeof(WriteableBitmap),
            typeof(LiveImage),

            new FrameworkPropertyMetadata(LiveSourcePropertyChanged)
        );
        public static void SetLiveSource(UIElement element, WriteableBitmap value)
        {
            element.SetValue(LiveSourceProperty, value);
        }
        public static WriteableBitmap GetLiveSource(UIElement element)
        {
            return (WriteableBitmap)element.GetValue(LiveSourceProperty);
        }
        public static void LiveSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((LiveImage)sender).SetImageSouce((ImageSource)e.NewValue);
        }

        #endregion 

        #region GrayImageSource
        public static readonly DependencyProperty GrayImageSourceProperty = DependencyProperty.RegisterAttached(
            "GrayImageSource",
            typeof(WriteableBitmap),
            typeof(LiveImage),

            new FrameworkPropertyMetadata(GrayImageSourcePropertyChanged)
        );
        public static void SetGrayImageSource(UIElement element, WriteableBitmap value)
        {
            element.SetValue(GrayImageSourceProperty, value);
        }
        public static WriteableBitmap GetGrayImageSource(UIElement element)
        {
            return (WriteableBitmap)element.GetValue(GrayImageSourceProperty);
        }
        public static void GrayImageSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((LiveImage)sender).SetGrayImageSource((ImageSource)e.NewValue);
        }

        #endregion

        #region IsCrossVisibility

        public static readonly DependencyProperty IsCrossVisibilityProperty = DependencyProperty.RegisterAttached(
            "IsCrossVisibility",
            typeof(bool),
            typeof(LiveImage),

            new FrameworkPropertyMetadata(IsCrossVisibilityPropertyChanged)
        );
        public static void SetIsCrossVisibility(UIElement element, bool value)
        {
            element.SetValue(IsCrossVisibilityProperty, value);
        }
        public static bool GetIsCrossVisibility(UIElement element)
        {
            return (bool)element.GetValue(IsCrossVisibilityProperty);
        }
        public static void IsCrossVisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((LiveImage)sender).SetCrossVisibility((bool)e.NewValue);
        }

        #endregion

        #region IsCenterRectVisibility

        public static readonly DependencyProperty IsCenterRectVisibilityProperty = DependencyProperty.RegisterAttached(
            "IsCenterRectVisibility",
            typeof(bool),
            typeof(LiveImage),

            new FrameworkPropertyMetadata(IsCenterRectVisibilityPropertyChanged)
        );
        public static void SetIsCenterRectVisibility(UIElement element, bool value)
        {
            element.SetValue(IsCenterRectVisibilityProperty, value);
        }
        public static bool GetIsCenterRectVisibility(UIElement element)
        {
            return (bool)element.GetValue(IsCenterRectVisibilityProperty);
        }
        public static void IsCenterRectVisibilityPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((LiveImage)sender).SetCenterRectVisibility((bool)e.NewValue);
        }

        #endregion

    }
}
