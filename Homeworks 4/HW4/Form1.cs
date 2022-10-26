using System.Diagnostics.Metrics;

namespace HW4
{
    public partial class Form1 : Form
    {
        Bitmap b;
        Graphics g;
        Random r = new Random();
        Pen PenTrajectoryR = new Pen(Color.YellowGreen,2);
        Pen PenTrajectoryA = new Pen(Color.Purple, 2);
        Pen PenTrajectoryN = new Pen(Color.DarkRed, 2);

        Bitmap bIstogram;
        Graphics gIstogram;


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //COMPUTE RELATIVE FREQUENCY
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.g = Graphics.FromImage(b);
            this.g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g.Clear(Color.White);

            int Trials = (int) numericUpDown2.Value;
            int NumberOfTrajectories = (int) numericUpDown3.Value;
            double SuccessProbability = ((Convert.ToDouble(numericUpDown1.Value)) / 100);

            double minX = 0;
            double maxX = Trials;
            double minY = 0;
            double maxY = Trials;

            Rectangle VirtualWindow = new Rectangle(0, 0, this.b.Width - 1, this.b.Height - 1);

            g.DrawRectangle(Pens.Black, VirtualWindow);

            List<int> LastY = new List<int>();


            for (int i = 0; i < NumberOfTrajectories; i++)
            {
                List<Point> Punti = new List<Point>();

                double y = 0;
                for (int x = 0; x <= Trials; x++)
                {
                    double cointoss = r.NextDouble();
                    if (cointoss <= SuccessProbability)
                    {
                        y = y + 1;
                    }
                    double yRelative = y * Trials / (x + 1);
                    int xDevice = FromXRealToXVirtual(x, minX, maxX, VirtualWindow.Left, VirtualWindow.Width);
                    int yDevice = FromYRealToYVirtual(yRelative, minY, maxY, VirtualWindow.Top, VirtualWindow.Height);
                    Punti.Add(new Point(xDevice, yDevice));

                    //to create the istogram
                    if (x == Trials)
                    {
                        LastY.Add(yDevice);
                    }
                }
                g.DrawLines(PenTrajectoryR, Punti.ToArray());
            }
            this.pictureBox1.Image = b;


            this.bIstogram = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            this.gIstogram = Graphics.FromImage(bIstogram);
            this.gIstogram.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.gIstogram.Clear(Color.White);

            Dictionary<double, int> intervals = new Dictionary<double, int>();

            //create the intervals first [0,4] second [5,9] then [10,14].... 
            double startingInter = 2.5;
            double inter = startingInter;
            int debug = 0;
            while (inter <= this.bIstogram.Height)
            {
                intervals[inter] = 0;
                inter = inter + (startingInter * 2);
            }

            //for each y in the List, create intervals.
            foreach (int coordY in LastY)
            {
                foreach (double key in intervals.Keys)
                {
                    if (coordY >= key - startingInter && coordY < key + startingInter)
                    {
                        intervals[key] += 1;
                        break;
                    }
                }
            }

            //proporzionare

            int max = 0;

            foreach (double key in intervals.Keys)
            {
                max += intervals[key];
            }

            //intervals[key] / max = x / width 
            //x = intervals[key] * width / max 

            //create the rectangles
            int numberofinterval = 0;
            foreach (double key in intervals.Keys)
            {
                Rectangle VirtualWindow1 = new Rectangle(0, 5 * numberofinterval, intervals[key] * this.bIstogram.Width / max, (int)startingInter * 2);
                numberofinterval++;

                gIstogram.DrawRectangle(Pens.Black, VirtualWindow1);
                gIstogram.FillRectangle(Brushes.YellowGreen, VirtualWindow1);
            }


            this.pictureBox2.Image = bIstogram;

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

                return (int) (Top + H - H * (Y - minY) / (maxY - minY));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //COMPUTE ABSOLUTE FREQUENCY
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.g = Graphics.FromImage(b);
            this.g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g.Clear(Color.White);

            int Trials = (int)numericUpDown2.Value;
            int NumberOfTrajectories = (int)numericUpDown3.Value;
            double SuccessProbability = ((Convert.ToDouble(numericUpDown1.Value)) / 100);

            double minX = 0;
            double maxX = Trials;
            double minY = 0;
            double maxY = Trials;

            Rectangle VirtualWindow = new Rectangle(0, 0, this.b.Width - 1, this.b.Height - 1);

            g.DrawRectangle(Pens.Black, VirtualWindow);

            List<int> LastY = new List<int>();


            for (int i = 0; i < NumberOfTrajectories; i++)
            {
                List<Point> Punti = new List<Point>();
                double y = 0;
                for (int x = 0; x <= Trials; x++)
                {
                    double cointoss = r.NextDouble();
                    if (cointoss <= SuccessProbability)
                    {
                        y = y + 1;
                    }
                    int xDevice = FromXRealToXVirtual(x, minX, maxX, VirtualWindow.Left, VirtualWindow.Width);
                    int yDevice = FromYRealToYVirtual(y, minY, maxY, VirtualWindow.Top, VirtualWindow.Height);
                    Punti.Add(new Point(xDevice, yDevice));

                    //to create the istogram
                    if (x == Trials)
                    {
                        LastY.Add(yDevice);
                    }
                }
                g.DrawLines(PenTrajectoryA, Punti.ToArray());
            }
            this.pictureBox1.Image = b;

            this.bIstogram = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            this.gIstogram = Graphics.FromImage(bIstogram);
            this.gIstogram.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.gIstogram.Clear(Color.White);

            Dictionary<double, int> intervals = new Dictionary<double, int>();

            //create the intervals first [0,4] second [5,9] then [10,14].... 
            double startingInter = 2.5;
            double inter = startingInter;
            int debug = 0;
            while (inter <= this.bIstogram.Height)
            {
                intervals[inter] = 0;
                inter = inter + (startingInter * 2);
            }

            //for each y in the List, create intervals.
            foreach (int coordY in LastY)
            {
                foreach (double key in intervals.Keys)
                {
                    if (coordY >= key - startingInter && coordY < key + startingInter)
                    {
                        intervals[key] += 1;
                        break;
                    }
                }
            }

            //proporzionare

            int max = 0;

            foreach (double key in intervals.Keys)
            {
                max += intervals[key];
            }

            //intervals[key] / max = x / width 
            //x = intervals[key] * width / max 

            //create the rectangles
            int numberofinterval = 0;
            foreach (double key in intervals.Keys)
            {
                Rectangle VirtualWindow1 = new Rectangle(0, 5 * numberofinterval, intervals[key] * this.bIstogram.Width / max, (int)startingInter * 2);
                numberofinterval++;

                gIstogram.DrawRectangle(Pens.Black, VirtualWindow1);
                gIstogram.FillRectangle(Brushes.Purple, VirtualWindow1);
            }


            this.pictureBox2.Image = bIstogram;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //COMPUTE NORMALIZED FREQUENCY
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.g = Graphics.FromImage(b);
            this.g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g.Clear(Color.White);

            int Trials = (int)numericUpDown2.Value;
            int NumberOfTrajectories = (int)numericUpDown3.Value;
            double SuccessProbability = ((Convert.ToDouble(numericUpDown1.Value)) / 100);

            double minX = 0;
            double maxX = Trials;
            double minY = 0;
            double maxY = Trials / Math.Sqrt(Trials);

            Rectangle VirtualWindow = new Rectangle(0, 0, this.b.Width-1, this.b.Height-1);

            g.DrawRectangle(Pens.Black, VirtualWindow);

            List<int> LastY = new List<int>();

            for (int i = 0; i < NumberOfTrajectories; i++)
            {
                List<Point> Punti = new List<Point>();
                double y = 0;
                for (int x = 0; x <= Trials; x++)
                {
                    double cointoss = r.NextDouble();
                    if (cointoss <= SuccessProbability)
                    {
                        y = y + 1;
                    }
                    double yNormalized = y / Math.Sqrt(x + 1);
                    int xDevice = FromXRealToXVirtual(x, minX, maxX, VirtualWindow.Left, VirtualWindow.Width);
                    int yDevice = FromYRealToYVirtual(yNormalized, minY, maxY, VirtualWindow.Top, VirtualWindow.Height);
                    Punti.Add(new Point(xDevice, yDevice));

                    //to create the istogram
                    if (x == Trials)
                    {
                        LastY.Add(yDevice);
                    }
                }
                g.DrawLines(PenTrajectoryN, Punti.ToArray());
            }
            this.pictureBox1.Image = b;

            this.bIstogram = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            this.gIstogram = Graphics.FromImage(bIstogram);
            this.gIstogram.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.gIstogram.Clear(Color.White);

            Dictionary<double, int> intervals = new Dictionary<double, int>();

            //create the intervals first [0,4] second [5,9] then [10,14].... 
            double startingInter = 2.5;
            double inter = startingInter;
            int debug = 0;
            while (inter <= this.bIstogram.Height)
            {
                intervals[inter] = 0;
                inter = inter + (startingInter * 2);
            }

            //for each y in the List, create intervals.
            foreach (int coordY in LastY)
            {
                foreach (double key in intervals.Keys)
                {
                    if (coordY >= key - startingInter && coordY < key + startingInter)
                    {
                        intervals[key] += 1;
                        break;
                    }
                }
            }
            //proporzionare

            int max = 0;

            foreach (double key in intervals.Keys)
            {
                max += intervals[key];
            }

            //intervals[key] / max = x / width 
            //x = intervals[key] * width / max 

            //create the rectangles
            int numberofinterval = 0;
            foreach (double key in intervals.Keys)
            {
                Rectangle VirtualWindow1 = new Rectangle(0, 5 * numberofinterval, intervals[key] * this.bIstogram.Width / max, (int)startingInter * 2);
                numberofinterval++;

                gIstogram.DrawRectangle(Pens.Black, VirtualWindow1);
                gIstogram.FillRectangle(Brushes.DarkRed, VirtualWindow1);
            }


            this.pictureBox2.Image = bIstogram;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.g = Graphics.FromImage(b);
            this.g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g.Clear(Color.White);

            int Trials = (int)numericUpDown2.Value;
            int NumberOfTrajectories = (int)numericUpDown3.Value;
            double SuccessProbability = ((Convert.ToDouble(numericUpDown1.Value)) / 100);

            double minX = 0;
            double maxX = Trials;
            double minY = 0;
            double maxY = Trials;

            Rectangle VirtualWindow = new Rectangle(0, 0, this.b.Width - 1, this.b.Height - 1);

            g.DrawRectangle(Pens.Black, VirtualWindow);

            List<int> LastY = new List<int>();


            for (int i = 0; i < NumberOfTrajectories; i++)
            {
                List<Point> PuntiA = new List<Point>();
                List<Point> PuntiR = new List<Point>();
                List<Point> PuntiN = new List<Point>();

                double y = 0;
                for (int x = 0; x <= Trials; x++)
                {
                    double cointoss = r.NextDouble();
                    if (cointoss <= SuccessProbability)
                    {
                        y = y + 1;
                    }

                    int xDevice = FromXRealToXVirtual(x, minX, maxX, VirtualWindow.Left, VirtualWindow.Width);


                    double yRelative = y * Trials / (x + 1);
                    int yDeviceR = FromYRealToYVirtual(yRelative, minY, maxY, VirtualWindow.Top, VirtualWindow.Height);
                    PuntiR.Add(new Point(xDevice, yDeviceR));


                    int yDeviceA = FromYRealToYVirtual(y, minY, maxY, VirtualWindow.Top, VirtualWindow.Height);
                    PuntiA.Add(new Point(xDevice, yDeviceA));


                    double yNormalized = y / Math.Sqrt(x + 1);
                    int yDeviceN = FromYRealToYVirtual(yNormalized, minY, maxY/Math.Sqrt(Trials), VirtualWindow.Top, VirtualWindow.Height);
                    PuntiN.Add(new Point(xDevice, yDeviceN));

                    //to create the istogram
                    if (x == Trials)
                    {
                        LastY.Add(yDeviceR);
                        LastY.Add(yDeviceA);
                        LastY.Add(yDeviceN);

                    }

                }
                g.DrawLines(PenTrajectoryA, PuntiA.ToArray());
                g.DrawLines(PenTrajectoryR, PuntiR.ToArray());
                g.DrawLines(PenTrajectoryN, PuntiN.ToArray());

            }

            this.pictureBox1.Image = b;


            this.bIstogram = new Bitmap(this.pictureBox2.Width, this.pictureBox2.Height);
            this.gIstogram = Graphics.FromImage(bIstogram);
            this.gIstogram.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.gIstogram.Clear(Color.White);

            Dictionary<double, int> intervals = new Dictionary<double, int>();

            //create the intervals first [0,4] second [5,9] then [10,14].... 
            double startingInter = 2.5;
            double inter = startingInter;
            int debug = 0;
            while (inter <= this.bIstogram.Height)
            {
                intervals[inter] = 0;
                inter = inter + (startingInter * 2);
            }

            //for each y in the List, create intervals.
            foreach (int coordY in LastY)
            {
                foreach (double key in intervals.Keys)
                {
                    if (coordY >= key - startingInter && coordY < key + startingInter)
                    {
                        intervals[key] += 1;
                        break;
                    }
                }
            }

            //proporzionare

            int max = 0;

            foreach (double key in intervals.Keys)
            {
                max += intervals[key];
            }

            //intervals[key] / max = x / width 
            //x = intervals[key] * width / max 

            //create the rectangles
            int numberofinterval = 0;
            foreach (double key in intervals.Keys)
            {
                Rectangle VirtualWindow1 = new Rectangle(0, 5 * numberofinterval, intervals[key] * this.bIstogram.Width / max, (int)startingInter * 2);
                numberofinterval++;

                gIstogram.DrawRectangle(Pens.Black, VirtualWindow1);
                gIstogram.FillRectangle(Brushes.Blue, VirtualWindow1);
            }


            this.pictureBox2.Image = bIstogram;

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}