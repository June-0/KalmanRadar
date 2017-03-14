using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WinFormKalmanRadar {
    public class DrawPlot {
        /// <summary>
        /// picture's width and height
        /// </summary>
        public static int rate = 10;
        public static int width = 800 * rate;
        public static int height = 500 * rate;
        // per step to draw one point
        public static int step = 8 * rate;
        public static Pen pen1 = new Pen(Brushes.Black, rate);
        public static Pen pen2 = new Pen(Brushes.Blue, rate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="from"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static Image getImage(List<int> line1, List<int> line2, int from, int center) {
            Bitmap img = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(img);
            for (int i = from; i < line1.Count - 1; ++i) {
                int curX1 = (i - from) * step;
                int curY1 = line1[i] - center;
                int curX2 = (i - from + 1) * step;
                int curY2 = line1[i + 1] - center;
                g.DrawLine(pen1, tp(curX1, curY1 * rate * 4), tp(curX2, curY2 * rate * 4));
                curY1 = line2[i] - center;
                curY2 = line2[i + 1] - center;
                g.DrawLine(pen2, tp(curX1, curY1 * rate * 4), tp(curX2, curY2 * rate * 4));
            }

            return img;
        }

        public static Image addLineOnImage(Image img, List<int> line1, List<int> line2, int from, int center, int cur) {
            if (line1.Count < 2) {
                return img;
            }
            Graphics g = Graphics.FromImage(img);
            int i = cur - 1;
            int curX1 = (i - from) * step;
            int curY1 = line1[i] - center;
            int curX2 = (i - from + 1) * step;
            int curY2 = line1[i + 1] - center;
            g.DrawLine(pen1, tp(curX1, curY1 * rate * 4), tp(curX2, curY2 * rate * 4));
            curY1 = line2[i] - center;
            curY2 = line2[i + 1] - center;
            g.DrawLine(pen2, tp(curX1, curY1 * rate * 4), tp(curX2, curY2 * rate * 4));
            return img;
        }

        public static Point tp(int x, int y) {
            return new Point(x, height / 2 - y);
        }
    }
}
