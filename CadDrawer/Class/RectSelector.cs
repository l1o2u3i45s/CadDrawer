using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;

namespace CadDrawer.Class
{
    public class RectSelector : ObservableObject
    {
        public int ClickUpTime { get; set; } = 0;

        private Visibility isRectVisible = Visibility.Collapsed;

        public Visibility IsRectVisible
        {
            get { return isRectVisible; }
            set { Set(() => IsRectVisible, ref isRectVisible, value); }
        }

        private double rectWidth;

        public double RectWidth
        {
            get { return rectWidth; }
            set { Set(() => RectWidth, ref rectWidth, value); }
        }

        private double rectHeight;

        public double RectHeight
        {
            get { return rectHeight; }
            set { Set(() => RectHeight, ref rectHeight, value); }
        }


        private double rectCanvasLeft;

        public double RectCanvasLeft
        {
            get { return rectCanvasLeft; }
            set { Set(() => RectCanvasLeft, ref rectCanvasLeft, value); }
        }

        private double rectCanvasTop;

        public double RectCanvasTop
        {
            get { return rectCanvasTop; }
            set { Set(() => RectCanvasTop, ref rectCanvasTop, value); }
        }

        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public bool IsMousePress { get; set; }
        public Rect SelectionRectangle { get; set; }


    }
}
