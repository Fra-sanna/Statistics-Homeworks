using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HMW_8_p_2
{
    public partial class Form1 : Form
    {
        Random r = new Random();
        Pen PenTrajectoryG = new Pen(Color.Black);
        Bitmap bHistogram;
        Graphics gHistogram;

        double minValue;
        double maxValue;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.bHistogram = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            this.gHistogram = Graphics.FromImage(bHistogram);
            this.gHistogram.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.gHistogram.Clear(Color.White);

            Rectangle VirtualWindow = new Rectangle(0, 0, this.bHistogram.Width - 1, this.bHistogram.Height - 1);
            gHistogram.DrawRectangle(Pens.Black, VirtualWindow);

            double delta = maxValue - minValue;
            double nintervals = 150;
            double intervalsSize = delta / nintervals;

            int nRows = (int)nintervals;
            int nCols = (int)nintervals;

            int nTrials = (int)numericUpDown1.Value;

            Dictionary<double, int> istogramDict = new Dictionary<double, int>();
            double tempValue = minValue;
            for (int i = 0; i <= nintervals; i++)
            {
                tempValue = minValue + (intervalsSize * i);
                tempValue = Math.Round(tempValue, 2);
                istogramDict[tempValue] = 0;
            }

            int total = 0;

            for (int x = 0; x < nTrials; x++)
            {
                double xRnd = (r.NextDouble() * 2) - 1;
                double value = 0;
                double yRnd = (r.NextDouble() * 2) - 1;

                double s = (xRnd * xRnd) + (yRnd * yRnd);

                while (s < 0 || s > 1)
                {
                    xRnd = (r.NextDouble() * 2) - 1;
                    yRnd = (r.NextDouble() * 2) - 1;
                    s = (xRnd * xRnd) + (yRnd * yRnd);
                }

                xRnd = xRnd * Math.Sqrt(-2 * Math.Log(s) / s);
                yRnd = yRnd * Math.Sqrt(-2 * Math.Log(s) / s);

                if (this.radioButton1.Checked) value = xRnd;
                else if (this.radioButton2.Checked) value = xRnd / yRnd; 
                else if (this.radioButton3.Checked) value = xRnd * xRnd; 
                else if (this.radioButton4.Checked) value = (xRnd * xRnd) / (yRnd * yRnd);
                else if (this.radioButton5.Checked) value = xRnd / (yRnd * yRnd);

                foreach (double key in istogramDict.Keys)
                {
                    double range = key + intervalsSize;
                    if (range > maxValue) range = maxValue;
                    if (value < range && value > key)
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

            gHistogram.TranslateTransform(0, this.bHistogram.Height);
            gHistogram.ScaleTransform(1, -1);

            int idIstogram = 0;
            int widthIstogram = (int)(this.bHistogram.Width / nintervals);
            double lastKeyY = 0;

            foreach (double key in istogramDict.Keys)
            {
                lastKeyY = key;
                int newHeight = istogramDict[key] * this.bHistogram.Height / total;
                int newX = (widthIstogram * idIstogram) + 1;
                Rectangle isto = new Rectangle(newX, 0, widthIstogram, newHeight);
                idIstogram++;

                int nextWidthIstogram = (int)(widthIstogram * idIstogram * 1);

                gHistogram.DrawRectangle(Pens.Black, isto);
                gHistogram.FillRectangle(Brushes.Green, isto);

                if ((idIstogram - 1) % 10 != 0 && idIstogram != istogramDict.Keys.Count()) continue;

                Label label = new Label();
                label.Name = "tempLabel";
                label.Location = new Point(newX + this.pictureBox2.Location.X - 7, this.pictureBox2.Height + this.pictureBox2.Location.Y);
                label.Text = ((double)(key)).ToString("N2");
                label.Visible = true;
                label.AutoSize = true;
                label.Font = new Font("Calibri", 7);
                label.ForeColor = Color.Black;
                this.Controls.Add(label);
            }

            int inverseI = nRows;
            for (int i = 0; i <= nRows; i++)
            {
                if (i % 10 != 0)
                {
                    inverseI--;
                    continue;
                }

                Point p1 = new Point(0, (int)this.pictureBox2.Height / nRows * i);
                Point p2 = new Point(this.pictureBox2.Width, (int)this.pictureBox2.Height / nRows * i);

                gHistogram.DrawLine(PenTrajectoryG, p1, p2);

                Label label = new Label();
                label.Name = "tempLabel";

                label.Location = new Point(this.pictureBox2.Location.X - 40, (int)this.pictureBox2.Location.Y + (this.pictureBox2.Height / nRows * i) - 5);

                label.Text = (total / nintervals * inverseI).ToString("N2");
                inverseI--;
                label.Visible = true;
                label.AutoSize = true;
                label.Font = new Font("Calibri", 7);

                label.ForeColor = Color.Black;
                this.Controls.Add(label);
            }

            for (int i = 0; i <= nCols; i++)
            {
                if (i % 10 != 0) continue;

                Point p1 = new Point((int)this.pictureBox2.Width / nCols * i, 0);
                Point p2 = new Point((int)this.pictureBox2.Width / nCols * i, (int)this.pictureBox2.Height);

                gHistogram.DrawLine(PenTrajectoryG, p1, p2);

            }

            this.pictureBox2.Image = bHistogram;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            minValue = 0;
            maxValue = 5;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            minValue = 0;
            maxValue = 5;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            minValue = -5;
            maxValue = 5;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            minValue = -10;
            maxValue = 10;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            minValue = -10;
            maxValue = 10;
        }
    }
}
