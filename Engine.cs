using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp2
{
    internal class Engine
    {
        private static readonly Point[] connects = new Point[]
 {
        new Point(-1, -1), new Point(0, -1), new Point(1, -1),
        new Point(1, 0),   new Point(1, 1),   new Point(0, 1),
        new Point(-1, 1),  new Point(-1, 0)
 };
        public static Bitmap ToGrayScale(Bitmap orginal)
        {
            Bitmap grayBmp = new Bitmap(orginal.Width, orginal.Height);

            using (Graphics g = Graphics.FromImage(grayBmp))
            {
                /*
                 Nguồn lượm đâu đó trên Google
                 Link Here: https://stackoverflow.com/questions/2265910/convert-an-image-to-grayscale
                */
                System.Drawing.Imaging.ColorMatrix colorMatrix = new System.Drawing.Imaging.ColorMatrix(
                    new float[][]
                    {
                        new float[] {0.3f, 0.3f, 0.3f, 0, 0},
                        new float[] {0.59f, 0.59f, 0.59f, 0, 0},
                        new float[] {0.11f, 0.11f, 0.11f, 0, 0},
                        new float[] {0,     0,     0,     1, 0},
                        new float[] {0,     0,     0,     0, 1}
                    });
                var attributes = new System.Drawing.Imaging.ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);
                g.DrawImage(orginal, new Rectangle(0, 0, orginal.Width, orginal.Height),
                    0, 0, orginal.Width, orginal.Height,
                    GraphicsUnit.Pixel, attributes);
            }
            return grayBmp;
        }
        public int getGray(Bitmap img, Point a)
        {
            Color color = img.GetPixel(a.X, a.Y);

            return (color.R + color.G + color.B) / 3;
        }
        private static int getGrayDiff(Bitmap img, Point a, Point b)
        {
            // Lấy giá trị màu RGB 
            Color imga = img.GetPixel(a.X, a.Y);
            Color imgb = img.GetPixel(b.X, b.Y);
            return Math.Abs(
                ((imga.R + imga.G + imga.B) / 3) -
                ((imgb.R + imgb.G + imgb.B) / 3)
            );
        }
        public static Bitmap ToGrayScales(Bitmap original)
        {
            int w = original.Width;
            int h = original.Height;

            Bitmap gray = new Bitmap(w, h);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    // Lấy pixel gốc
                    Color pixel = original.GetPixel(x, y);

                    // Công thức chuyển sang xám
                    int grayValue = (int)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);

                    // Tạo pixel màu xám
                    Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);

                    // Gán vào ảnh mới
                    gray.SetPixel(x, y, grayColor);
                }
            }

            return gray;
        }

        public static Bitmap regionGrow(Bitmap original, List<Point> listpoint, int threshold)
        {
            int w = original.Width;
            int h = original.Height;
            int[,] seemask = new int[w, h];
            Bitmap result = new Bitmap(w, h); //  width, height
            using (Graphics g = Graphics.FromImage(result))
            {
                g.Clear(Color.Black);
            }
            Queue<Point> queuepoints = new Queue<Point>();
            foreach (var p in listpoint)
                queuepoints.Enqueue(p);

            int label = 1;
            Point[] connectsPoint = connects;

            while (queuepoints.Count > 0)
            {
                Point current = queuepoints.Dequeue();
                seemask[current.X, current.Y] = label;
                result.SetPixel(current.X, current.Y, Color.White);
                for (int i = 0; i < 8; i++)
                {
                    int tmpX = current.X + connectsPoint[i].X;
                    int tmpY = current.Y + connectsPoint[i].Y;

                    if (tmpX < 0 || tmpY < 0 || tmpX >= w || tmpY >= h)
                    {

                        continue;
                    }

                    int grayDiff = getGrayDiff(original, current, new Point(tmpX, tmpY));

                    if (grayDiff < threshold && seemask[tmpX, tmpY] == 0)
                    {
                        seemask[tmpX, tmpY] = label;
                        queuepoints.Enqueue(new Point(tmpX, tmpY)); //  X,Y
                    }
                }
            }

            return result;
        }


    }
}
