using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp2
{
    internal class Engine
    {
        //       // 8 nhân
        private static readonly Point[] connects = new Point[]
 {
        new Point(-1, -1), new Point(0, -1), new Point(1, -1),
        new Point(1, 0),   new Point(1, 1),   new Point(0, 1),
        new Point(-1, 1),  new Point(-1, 0)
 };

        //4 nhân
//        private static readonly Point[] connects = new Point[]
//{
//                new Point(-1, -1), new Point(0, -1),
//                new Point(1, 0),   new Point(1, 1)
//};
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
            //using (Graphics g = Graphics.FromImage(result))
            //{
            //    g.Clear(Color.Black);
            //}
            Queue<Point> queuepoints = new Queue<Point>();
            foreach (var p in listpoint)
                queuepoints.Enqueue(p);

            int label = 1;
            Point[] connectsPoint = connects;

            while (queuepoints.Count > 0)
            {
                Point current = queuepoints.Dequeue();
                seemask[current.X, current.Y] = label;
                result.SetPixel(current.X, current.Y, Color.Black);
                for (int i = 0; i < connects.Count<Point>(); i++)
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

        //Đây là Multi-resolution Region Growing
        public static Bitmap GrowRegion(Bitmap img, Point seed, int threshold, int levels)
        {
            Bitmap currentImg = img;
            Bitmap result = null;

            // Tạo pyramid đa độ phân giải
            List<Bitmap> pyramid = new List<Bitmap>();
            pyramid.Add(currentImg);
            for (int i = 1; i < levels; i++)
            {
                int w = currentImg.Width / 2;
                int h = currentImg.Height / 2;
                if (w == 0 || h == 0) break;
                Bitmap smaller = new Bitmap(currentImg, new Size(w, h));
                pyramid.Add(smaller);
                currentImg = smaller;
            }

            // Bắt đầu region growing từ mức thấp nhất
            Bitmap mask = new Bitmap(pyramid[pyramid.Count - 1].Width, pyramid[pyramid.Count - 1].Height);
            Queue<Point> q = new Queue<Point>();
            Point scaledSeed = new Point(seed.X / (int)Math.Pow(2, levels - 1),
                                         seed.Y / (int)Math.Pow(2, levels - 1));
            q.Enqueue(scaledSeed);

            Bitmap lowRes = pyramid[pyramid.Count - 1];
            Color seedColor = lowRes.GetPixel(scaledSeed.X, scaledSeed.Y);

            while (q.Count > 0)
            {
                Point p = q.Dequeue();
                if (p.X < 0 || p.Y < 0 || p.X >= lowRes.Width || p.Y >= lowRes.Height)
                    continue;
                if (mask.GetPixel(p.X, p.Y).R > 0) continue;

                Color c = lowRes.GetPixel(p.X, p.Y);
                int diff = Math.Abs(c.R - seedColor.R) +
                           Math.Abs(c.G - seedColor.G) +
                           Math.Abs(c.B - seedColor.B);
                if (diff < threshold)
                {
                    mask.SetPixel(p.X, p.Y, Color.White);
                    q.Enqueue(new Point(p.X + 1, p.Y));
                    q.Enqueue(new Point(p.X - 1, p.Y));
                    q.Enqueue(new Point(p.X, p.Y + 1));
                    q.Enqueue(new Point(p.X, p.Y - 1));
                }
            }

            // Phóng mask lên ảnh gốc
            result = new Bitmap(img.Width, img.Height);
            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    int mx = x / (int)Math.Pow(2, levels - 1);
                    int my = y / (int)Math.Pow(2, levels - 1);
                    if (mx >= mask.Width) mx = mask.Width - 1;
                    if (my >= mask.Height) my = mask.Height - 1;

                    Color orig = img.GetPixel(x, y);
                    if (mask.GetPixel(mx, my).R > 0)
                        result.SetPixel(x, y, Color.Red); // vùng phát triển tô màu
                    else
                        result.SetPixel(x, y, orig);
                }
            }

            return result;
        }
    }
}
