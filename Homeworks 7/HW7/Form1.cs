using System.Diagnostics.Metrics;
using System.Windows.Forms;

namespace HW7
{
    public partial class Form1 : Form
    {
        Bitmap b;
        Graphics g;
        Random r = new Random();
        Pen PenTrajectoryR = new Pen(Color.Green, 2);
        Pen PenTrajectoryG = new Pen(Color.Black, 0.5F);
        Boolean interArrival = false;

        Bitmap bIstogram;
        Graphics gIstogram;

        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.g = Graphics.FromImage(b);
            this.g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g.Clear(Color.White);


            int nTrials = (int)numericUpDown1.Value;
            int NumberOfTrajectories = (int)numericUpDown3.Value;

            double SuccessProbability = ((Convert.ToDouble(numericUpDown2.Value)) / (int)numericUpDown1.Value);

            double minX = 0;
            double maxX = nTrials;
            double minY = 0;
            double maxY = nTrials;

            Rectangle VirtualWindow = new Rectangle(0, 0, this.b.Width - 1, this.b.Height - 1);
            g.DrawRectangle(Pens.Black, VirtualWindow);

            int nRows = 10;
            int nCols = 10;

            for (int i = 0; i < nRows; i++)
            {
                Point p1 = new Point(0, (int)this.pictureBox1.Height / nRows * i);
                Point p2 = new Point(this.pictureBox1.Width, (int)this.pictureBox1.Height / nRows * i);

                g.DrawLine(PenTrajectoryG, p1,p2);
            }

            for (int i = 0; i < nCols; i++)
            {
                Point p1 = new Point((int)this.pictureBox1.Width / nCols * i,0);
                Point p2 = new Point((int)this.pictureBox1.Width / nCols * i, (int)this.pictureBox1.Height);

                g.DrawLine(PenTrajectoryG, p1, p2);
            }


            List<int> LastY = new List<int>();

            Dictionary<int, int> nLossDict = new Dictionary<int, int>();


            for (int i = 0; i < NumberOfTrajectories; i++)
            {
                List<Point> Punti = new List<Point>();

                double y = 0;
                int nLoss = 0;

                for (int x = 0; x <= nTrials; x++)
                {
                    double cointoss = r.NextDouble();
                    if (cointoss <= SuccessProbability)
                    {
                        y = y + 1;
                        if (nLossDict.ContainsKey(nLoss))
                        {

                            nLossDict[nLoss] = nLossDict[nLoss] + 1;
                        }
                        else
                        {
                            nLossDict[nLoss] = 1;
                        }

                        nLoss = 0;
                    }
                    else
                    {
                        nLoss = nLoss + 1;
                    }

                    double yRelative = y * nTrials / (x + 1);
                    int xDevice = FromXRealToXVirtual(x, minX, maxX, VirtualWindow.Left, VirtualWindow.Width);
                    int yDevice = FromYRealToYVirtual(y, minY, maxY, VirtualWindow.Top, VirtualWindow.Height);
                    Punti.Add(new Point(xDevice, yDevice));

                    if (x == nTrials)
                    {
                        LastY.Add((int) y);
                    }
                }
                g.DrawLines(PenTrajectoryR, Punti.ToArray());
            }
            this.pictureBox1.Image = b;

            // Istogramma

            this.bIstogram = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            this.gIstogram = Graphics.FromImage(bIstogram);

            this.gIstogram.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.gIstogram.Clear(Color.White);

            Rectangle VirtualWindow2 = new Rectangle(0, 0, this.bIstogram.Width - 1, this.bIstogram.Height - 1);
            gIstogram.DrawRectangle(Pens.Black, VirtualWindow2);

            if (interArrival == false)
            {

                double minAvg = LastY.Min();
                double maxAvg = LastY.Max();
                double delta = maxAvg - minAvg;
                double nintervals = 15;
                double intervalsSize = delta / nintervals;
                Dictionary<double, int> istogramDict = new Dictionary<double, int>();

                double tempValue = minAvg;
                for (int i = 0; i < nintervals; i++)
                {
                    istogramDict[tempValue] = 0;
                    tempValue = tempValue + intervalsSize;
                }

                int total = 0;

                foreach (double value in LastY)
                {
                    foreach (double key in istogramDict.Keys)
                    {
                        if (value < key + intervalsSize)
                        {
                            total++;
                            istogramDict[key] += 1;
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


                gIstogram.TranslateTransform(0, this.bIstogram.Height);
                gIstogram.ScaleTransform(1, -1);

                int idIstogram = 0;
                int widthIstogram = (int)(this.bIstogram.Width / nintervals);
                foreach (double key in istogramDict.Keys)
                {
                    int newHeight = istogramDict[key] * this.bIstogram.Height / total;
                    int newX = (widthIstogram * idIstogram) + 1;
                    Rectangle isto = new Rectangle(newX, 0, widthIstogram, newHeight);
                    idIstogram++;

                    int nextWidthIstogram = (int)(widthIstogram * idIstogram * 1);


                    Label label = new Label();
                    label.Name = "tempLabel";

                    label.Location = new Point(newX + this.pictureBox2.Location.X + (widthIstogram * 1 / 4), this.pictureBox2.Height + this.pictureBox2.Location.Y);

                    label.Text = ((int)(key)).ToString() + " - " + ((int)(key + intervalsSize)).ToString();
                    label.Visible = true;
                    label.AutoSize = true;
                    label.Font = new Font("Calibri", 7);

                    label.ForeColor = Color.Black;

                    this.Controls.Add(label);



                    gIstogram.DrawRectangle(Pens.Black, isto);
                    gIstogram.FillRectangle(Brushes.Green, isto);
                }


                this.pictureBox2.Image = bIstogram;
            }
            else
            {
                double minLoss = nLossDict.Keys.Min();
                double maxLoss = nLossDict.Keys.Max();

                double delta = maxLoss - minLoss;
                double nintervals = nLossDict.Keys.Count() - 1;
                if (nLossDict.Keys.Count() > 10)
                {
                    nintervals = 10;
                }

                double intervalsSize = delta / nintervals;

                Dictionary<double, int> istogramDict = new Dictionary<double, int>();

                double tempValue = minLoss;

                for (int i = 0; i < nintervals; i++)
                {
                    istogramDict[tempValue] = 0;
                    tempValue = tempValue + intervalsSize;
                }

                int total = 0;

                foreach (int keyLoss in nLossDict.Keys)
                {
                    int value = nLossDict[keyLoss];
                    foreach (double key in istogramDict.Keys)
                    {
                        if (keyLoss < key + intervalsSize)
                        {
                            total = total + value;
                            istogramDict[key] = value + istogramDict[key];
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


                gIstogram.TranslateTransform(0, this.bIstogram.Height);
                gIstogram.ScaleTransform(1, -1);

                int idIstogram = 0;
                int widthIstogram = (int)(this.bIstogram.Width / nintervals);
                foreach (double key in istogramDict.Keys)
                {
                    int newHeight = istogramDict[key] * this.bIstogram.Height / total;
                    int newX = (widthIstogram * idIstogram) + 1;
                    Rectangle isto = new Rectangle(newX, 0, widthIstogram, newHeight);
                    idIstogram++;

                    int nextWidthIstogram = (int)(widthIstogram * idIstogram * 1);


                    Label label = new Label();
                    label.Name = "tempLabel";
                    label.Location = new Point(newX + this.pictureBox2.Location.X + (widthIstogram * 1 / 4) , this.pictureBox2.Height + this.pictureBox2.Location.Y);

                    if (idIstogram - 1 == 0)
                    {
                        label.Text = ((int)(key)).ToString() + " - " + ((int)(key + intervalsSize)).ToString();
                    }
                    else
                    {
                        label.Text = ((int)(key + 1)).ToString() + " - " + ((int)(key + intervalsSize)).ToString();
                    }


                    label.Visible = true;
                    label.AutoSize = true;
                    label.Font = new Font("Calibri", 7);

                    label.ForeColor = Color.Black;

                    this.Controls.Add(label);

                    gIstogram.DrawRectangle(Pens.Black, isto);
                }


                this.pictureBox2.Image = bIstogram;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.numericUpDown2.Maximum = this.numericUpDown1.Value;
            if (this.numericUpDown1.Value <= this.numericUpDown2.Value)
            {
                this.numericUpDown2.Value = (int)(this.numericUpDown1.Value / 2);
            }

            

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

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
           

        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            interArrival = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            interArrival = true;
        }
    }
}