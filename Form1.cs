using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private List<Point> points = new List<Point>();

        //Đây là points để test nhé Comments cái trên lại
        //private List<Point> points = new List<Point>
        //{
        //    new Point(10, 10),
        //    new Point(100, 200),
        //    new Point(300, 400),
        //};
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Cho phép: số (0-9), phím Backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Không cho nhập
            }
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Cho phép: số (0-9), phím Backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // Không cho nhập
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Chọn ảnh";
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(ofd.FileName);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; // Co dãn ảnh vừa khung
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                DialogResult result = MessageBox.Show("Image not Found can't not  be performed", "Error", MessageBoxButtons.OK);
                return;
            }
            Console.WriteLine("Toi click vao day ");
            if (points.Count <= 0)
            {
                DialogResult result = MessageBox.Show("Nhập điểm ảnh hạt giống vào listbox ", "Warring", MessageBoxButtons.OK);
                return;
            }

            foreach (var item in points)
            {
                if (item.X > pictureBox1.Image.Width || item.Y > pictureBox2.Image.Height
                    || item.X <= 0 || item.Y <= 0)
                {
                    DialogResult result = MessageBox.Show("Điểm ảnh hạt giống trong listbox  không hợp lệ", "Warring", MessageBoxButtons.OK);
                    return;
                }
            }
            Bitmap bmp = (Bitmap)pictureBox1.Image;
            pictureBox2.Image = Engine.ToGrayScales(bmp);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

            pictureBox3.Image = Engine.regionGrow((Bitmap)pictureBox2.Image, points, 15);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                DialogResult result = MessageBox.Show("Không có ảnh để xóa", "Error", MessageBoxButtons.OK);
                return;
            }
            pictureBox1.Image.Dispose();
            pictureBox1.Image = null;
            pictureBox2.Image.Dispose();
            pictureBox2.Image = null;
            pictureBox3.Image.Dispose();
            pictureBox3.Image = null;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string txt1 = textBox1.Text;
            txt1 = txt1.Trim();
            Console.WriteLine(txt1);
            String txt2 = textBox2.Text;
            txt2 = txt2.Trim();
            if (string.IsNullOrEmpty(txt1) || string.IsNullOrEmpty(txt2))
            {
                DialogResult result = MessageBox.Show("Nhập tọa độ điểm vào đây.", "Error", MessageBoxButtons.OK);
                return;
            }
            int x, y;
            x = Convert.ToInt32(txt1);
            y = Convert.ToInt32(txt2);
            textBox1.Text = "";
            textBox2.Text = "";

            Point a = new Point(x, y);
            listBox1.Items.Add(a);
            points.Add(a);
            listBox1.Update();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            if (index == -1)
            {
                DialogResult result = MessageBox.Show("Vui lòng chọn điểm cần xóa trong listbox bên cạnh", "Warring", MessageBoxButtons.OK);
                return;
            }
            Point a = (Point)listBox1.SelectedItem;
            listBox1.Items.Remove(a);
            points.Remove(a);
            listBox1.Update();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null) 
            {
                Point a = (Point)listBox1.SelectedItem;
                textBox1.Text = a.X.ToString();
                textBox2.Text = a.Y.ToString();
            }
        }
    }
}
