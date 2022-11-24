using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HMW_8_p_1
{
    public partial class Form1 : Form
    {
        Bitmap b;
        Graphics g;
        Random r = new Random();
        Pen PenTrajectory = new Pen(Color.Green);

        Bitmap bhistogramX;
        Graphics ghistogramX;

        Bitmap bhistogramY;
        Graphics ghistogramY;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private int FromXRealToXVirtual(double X, double minX, double maxX, int Left, int W)
        {
            if (maxX - minX == 0)
            {
                return 0;
            }
            else
            {
                return (int)(Left + W * (X - minX) / (maxX - minX));
            }
        }

        private int FromYRealToYVirtual(double Y, double minY, double maxY, int Top, int H)
        {
            if (maxY - minY == 0)
            {
                return 0;
            }
            else
            {
                return (int)(Top + H - H * (Y - minY) / (maxY - minY));
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.g = Graphics.FromImage(b);
            this.g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g.Clear(Color.White);

            Rectangle VirtualWindow = new Rectangle(0, 0, this.b.Width - 1, this.b.Height - 1);
            g.DrawRectangle(Pens.Black, VirtualWindow);

            int nPoints = (int)numericUpDown1.Value;
            int rayDefault = (int)numericUpDown2.Value;

            double minX = -150;
            double maxX = 150;
            double minY = -150;
            double maxY = 150;

            List<double> PuntiX = new List<double>();
            List<double> PuntiY = new List<double>();

            for (int i = 0; i < nPoints; i++)
            {
                double ray = r.NextDouble() * rayDefault;
                double angle = r.Next(0, 360);
                double xCoord = ray * Math.Cos(angle);
                double yCoord = ray * Math.Sin(angle);

                int xDevice = FromXRealToXVirtual(xCoord, minX, maxX, VirtualWindow.Left, VirtualWindow.Width);
                int yDevice = FromYRealToYVirtual(yCoord, minY, maxY, VirtualWindow.Top, VirtualWindow.Height);

                Rectangle rect = new Rectangle(xDevice, yDevice, 1, 1);
                g.DrawRectangle(PenTrajectory, rect);
                g.FillRectangle(Brushes.Black, rect);

                PuntiX.Add(xDevice);
                PuntiY.Add(yDevice);

            }

            this.pictureBox1.Image = b;

            // Istogramma delle X

            this.bhistogramX = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            this.ghistogramX = Graphics.FromImage(bhistogramX);
            this.ghistogramX.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.ghistogramX.Clear(Color.White);

            Rectangle VirtualWindow2 = new Rectangle(0, 0, this.bhistogramX.Width - 1, this.bhistogramX.Height - 1);
            ghistogramX.DrawRectangle(Pens.Black, VirtualWindow2);

            double minValueX = PuntiX.Min();
            double maxValueX = PuntiX.Max();
            double delta = maxValueX - minValueX;
            double nintervals = 25;
            double intervalsSize = delta / nintervals;

            Dictionary<double, int> istogramDict = new Dictionary<double, int>();

            double tempValue = minValueX;
            for (int i = 0; i < nintervals; i++)
            {
                istogramDict[tempValue] = 0;
                tempValue = tempValue + intervalsSize;
            }

            int total = 0;

            foreach (double value in PuntiX)
            {
                foreach (double key in istogramDict.Keys)
                {
                    if (value < key + intervalsSize)
                    {
                        istogramDict[key] += 1;
                        if (total < istogramDict[key])
                        {
                            total = istogramDict[key];
                        }
                        break;
                    }
                }
            }

            List<Control> labelList = new List<Control>();

            foreach (Control ctrl in this.Controls.OfType<Label>().Where(x => x.Name.Contains("tempLabel")))
            {
                labelList.Add(ctrl);
            }

            foreach (Control ctrl in labelList)
            {
                this.Controls.Remove(ctrl);
            }


            ghistogramX.TranslateTransform(0, this.bhistogramX.Height);
            ghistogramX.ScaleTransform(1, -1);

            int idIstogram = 0;
            int widthIstogram = (int)(this.bhistogramX.Width / nintervals);
            double lastKey = 0;

            foreach (double key in istogramDict.Keys)
            {
                lastKey = key;
                int newHeight = istogramDict[key] * this.bhistogramX.Height / total;
                int newX = (widthIstogram * idIstogram) + 1;
                Rectangle isto = new Rectangle(newX, 0, widthIstogram, newHeight);
                idIstogram++;

                Label label = new Label();
                label.Name = "tempLabel";
                if (key < 100) label.Location = new Point(newX + this.pictureBox2.Location.X - 5, this.pictureBox2.Height + this.pictureBox2.Location.Y);
                else label.Location = new Point(newX + this.pictureBox2.Location.X - 10, this.pictureBox2.Height + this.pictureBox2.Location.Y);

                label.Text = ((int)(key)).ToString();
                label.Visible = true;
                label.AutoSize = true;
                label.Font = new Font("Calibri", 6.5F);
                label.ForeColor = Color.Black;
                this.Controls.Add(label);

                ghistogramX.DrawRectangle(Pens.Black, isto);
                ghistogramX.FillRectangle(Brushes.Green, isto);
            }

            Label label2 = new Label();
            label2.Name = "tempLabel";
            label2.Location = new Point(this.pictureBox2.Width + this.pictureBox2.Location.X - 10, this.pictureBox2.Height + this.pictureBox2.Location.Y);
            label2.Text = ((int)(lastKey + intervalsSize)).ToString();
            label2.Visible = true;
            label2.AutoSize = true;
            label2.Font = new Font("Calibri", 6.5F);
            label2.ForeColor = Color.Black;
            this.Controls.Add(label2);

            this.pictureBox2.Image = bhistogramX;

            // Istogramma delle Y

            this.bhistogramY = new Bitmap(this.pictureBox3.Width, this.pictureBox3.Height);
            this.ghistogramY = Graphics.FromImage(bhistogramY);
            this.ghistogramY.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.ghistogramY.Clear(Color.White);

            Rectangle VirtualWindow3 = new Rectangle(0, 0, this.bhistogramY.Width - 1, this.bhistogramY.Height - 1);
            ghistogramY.DrawRectangle(Pens.Black, VirtualWindow3);

            double minValueY = PuntiY.Min();
            double maxValueY = PuntiY.Max();
            double deltaY = maxValueY - minValueY;
            double intervalsYSize = deltaY / nintervals;

            Dictionary<double, int> istogramDictY = new Dictionary<double, int>();

            double tempValueY = minValueY;
            for (int i = 0; i < nintervals; i++)
            {
                istogramDictY[tempValueY] = 0;
                tempValueY = tempValueY + intervalsYSize;
            }

            int totalY = 0;

            foreach (double value in PuntiY)
            {
                foreach (double key in istogramDictY.Keys)
                {
                    if (value < key + intervalsYSize)
                    {
                        istogramDictY[key] += 1;
                        if (totalY < istogramDictY[key])
                        {
                            totalY = istogramDictY[key];
                        }
                        break;
                    }
                }
            }

            ghistogramY.TranslateTransform(0, this.bhistogramY.Height);
            ghistogramY.ScaleTransform(1, -1);

            idIstogram = 0;
            int widthIstogramY = (int)(this.bhistogramY.Width / nintervals);
            double lastKeyY = 0;

            foreach (double key in istogramDictY.Keys)
            {
                lastKeyY = key;
                int newHeight = istogramDictY[key] * this.bhistogramY.Height / totalY;
                int newX = (widthIstogramY * idIstogram) + 1;
                Rectangle isto = new Rectangle(newX, 0, widthIstogramY, newHeight);
                idIstogram++;

                Label label = new Label();
                label.Name = "tempLabel";
                if (key < 100) label.Location = new Point(newX + this.pictureBox3.Location.X - 5, this.pictureBox3.Height + this.pictureBox3.Location.Y);
                else label.Location = new Point(newX + this.pictureBox3.Location.X - 10, this.pictureBox3.Height + this.pictureBox3.Location.Y);

                label.Text = ((int)(key)).ToString();
                label.Visible = true;
                label.AutoSize = true;
                label.Font = new Font("Calibri", 6.5F);
                label.ForeColor = Color.Black;
                this.Controls.Add(label);

                ghistogramY.DrawRectangle(Pens.Black, isto);
                ghistogramY.FillRectangle(Brushes.Green, isto);


            }

            Label label3 = new Label();
            label3.Name = "tempLabel";
            label3.Location = new Point(this.pictureBox3.Width + this.pictureBox3.Location.X - 10, this.pictureBox3.Height + this.pictureBox3.Location.Y);
            label3.Text = ((int)(lastKeyY + intervalsYSize)).ToString();
            label3.Visible = true;
            label3.AutoSize = true;
            label3.Font = new Font("Calibri", 6.5F);
            label3.ForeColor = Color.Black;
            this.Controls.Add(label3);

            this.pictureBox3.Image = bhistogramY;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
