using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace homework6
{
    public partial class Form1 : Form
    {
        Bitmap b;
        Graphics g;

        Bitmap bIsto;
        Graphics gIsto;
        Boolean boolWeight = true;
        String tipo;

        public Form1()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.g = Graphics.FromImage(b);
            this.g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g.Clear(Color.White);
            this.richTextBox1.Clear();
            this.richTextBox1.AppendText("\n" + tipo + "\n\n");

            Rectangle VirtualWindow = new Rectangle(0, 0, this.b.Width - 1, this.b.Height - 1);
            g.DrawRectangle(Pens.DarkSlateGray, VirtualWindow);
            int index = 1;
            Random r = new Random();
            if (boolWeight) index = 1; 
            else index = 2;

            int trialsnumber = (int)numericUpDown1.Value;
            int samplesize = (int)numericUpDown2.Value; 
            var valuestring = new List<double>();

            double minX = 0;
            double maxX = samplesize;
            double maxValue = 0;
            double minValue = 0;
            double minY = minValue;
            double maxY = maxValue;

            using (TextFieldParser parser = new TextFieldParser(@"./../../weight-height.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    double key = double.Parse(fields[index], CultureInfo.InvariantCulture);
                    valuestring.Add(key);
                }
            }

            int length = valuestring.Count;
            List<double> avglist = new List<double>();

            for (int i = 0; i < trialsnumber; i++)
            {
                List<Point> Punti = new List<Point>();
                List<int> selectorList = new List<int>();


                double tempmean = 0;
                double mean = 0;

                for (int j = 0; j <= samplesize; j++)
                {
                    int selector = r.Next(length);
                    selectorList.Add(selector);

                    double value = valuestring[selector];
                    
                    mean = (mean + value);
                    tempmean = mean / (j+1);


                    if (minValue == 0 || tempmean < minValue)
                    {
                        minValue = tempmean;
                    }

                    if (minValue == 0 || tempmean > maxValue)
                    {
                        maxValue = tempmean;
                    }

                    minY = minValue;
                    maxY = maxValue;

                }
                avglist.Add(tempmean);

                tempmean = 0;
                mean = 0;

                for (int j = 0; j < selectorList.Count; j++)
                {
                    int random = selectorList[j];
                    double value = valuestring[random];

                    mean = (mean + value);
                    tempmean = mean / (j + 1);

                    int xDevice = (int)(FromXRealToXVirtual(j, minX, maxX, VirtualWindow.Left, VirtualWindow.Width));
                    int YDevice = (int)(FromYRealToYVirtual(tempmean, minY, maxY, VirtualWindow.Top, VirtualWindow.Height));
                    Punti.Add(new Point(xDevice, YDevice));
                }

            }
            double help = 0.0;
            int n = 1;
                foreach (double j in avglist)
                {
                    this.richTextBox1.AppendText("average of sample " + n.ToString() + ": " + j.ToString() + "\n");
                    help += j;
                    n++;
                }
            help /= trialsnumber;
            this.richTextBox1.AppendText("AVG: " + help.ToString());
            this.pictureBox1.Image = b;

            this.bIsto = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.gIsto = Graphics.FromImage(bIsto);

            this.gIsto.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.gIsto.Clear(Color.White);

            Rectangle VirtualWindow2 = new Rectangle(0, 0, this.bIsto.Width - 1, this.bIsto.Height - 1);
            gIsto.DrawRectangle(Pens.Black, VirtualWindow2);

            double minAvg = avglist.Min();
            double maxAvg = avglist.Max();
            double delta = maxAvg - minAvg;
            double nintervals = 25;
            double intervalsSize = delta / nintervals;
            Dictionary<double, int> istoDict = new Dictionary<double, int>();

            double tempValue = minAvg;
            for (int i = 0; i < nintervals; i++)
            {
                istoDict[tempValue] = 0;
                tempValue = tempValue + intervalsSize;
            }

            int total = 0;

            foreach (double value in avglist)
            {
                foreach (double key in istoDict.Keys)
                {
                    if (value < key + intervalsSize)
                    {
                        total++;
                        istoDict[key] += 1;
                        break;
                    }
                }
            }

            gIsto.TranslateTransform(0, this.bIsto.Height);
            gIsto.ScaleTransform(1, -1);

            int idIsto = 0;
            int widthIsto = (int)(this.bIsto.Width / nintervals);
            foreach (double key in istoDict.Keys)
            {
                int newHeight = istoDict[key] * this.bIsto.Height / total;
                int newX = (widthIsto * idIsto) + 1;
                Rectangle isto = new Rectangle(newX, 0, widthIsto, newHeight);
                idIsto++;

                int nextWidthIstogram = (int)(widthIsto * idIsto * 1);
                
                SolidBrush the_brush = new SolidBrush(Color.Green);
                gIsto.DrawRectangle(Pens.Black, isto);
                gIsto.FillRectangle(the_brush, isto);
            }
            this.pictureBox1.Image = bIsto;


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

            public double FromXRealToXVirtual(double X, double minX, double maxX, int Left, int W)
            {
                if ((maxX - minX) == 0)
                    return 0;

                return Left + W * (X - minX) / (maxX - minX);
            }

            public double FromYRealToYVirtual(double Y, double minY, double maxY, int Top, int H)
            {
                if ((maxY - minY) == 0)
                    return 0;

                return Top + H - H * (Y - minY) / (maxY - minY);
            }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.b = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.g = Graphics.FromImage(b);
            this.g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.g.Clear(Color.White);
            this.richTextBox1.Clear();
            this.richTextBox1.AppendText("\n" + tipo + "\n\n");


            Rectangle VirtualWindow = new Rectangle(0, 0, this.b.Width - 1, this.b.Height - 1);
            g.DrawRectangle(Pens.DarkSlateGray, VirtualWindow);

            Random r = new Random();
            int index;

            if (boolWeight) index = 1;
            else index = 2;
            int trialsnumber = (int)numericUpDown1.Value;
            int samplesize = (int)numericUpDown2.Value;
            var valuestring = new List<double>();

            double minX = 0;
            double maxX = samplesize;
            double maxValue = 0;
            double minValue = 0;
            double minY = minValue;
            double maxY = maxValue;

            using (TextFieldParser parser = new TextFieldParser(@"./../../weight-height.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    double key = double.Parse(fields[index], CultureInfo.InvariantCulture);
                    valuestring.Add(key);
                }
            }

            int length = valuestring.Count;
            List<double> variancelist = new List<double>();

            for (int i = 0; i < trialsnumber; i++)
            {
                List<Point> Punti = new List<Point>();
                List<int> selectorList = new List<int>();


                double tempmean = 0;
                double mean = 0;
                double variance = 0.0;

                for (int j = 0; j <= samplesize; j++)
                {
                    int selector = r.Next(length);
                    selectorList.Add(selector);

                    double value = valuestring[selector];

                    mean = (mean + value);
                    tempmean = mean / (j + 1);

                    variance += Math.Pow(value - tempmean, 2.0);
                    variance= (variance/(j + 1))*3;

                    if (minValue == 0 || variance < minValue)
                    {
                        minValue = variance;
                    }

                    if (minValue == 0 || variance > maxValue)
                    {
                        maxValue = variance;
                    }

                    minY = minValue;
                    maxY = maxValue;

                }
                variancelist.Add(variance);


                tempmean = 0;
                mean = 0;
                double variance1 = 0.0;

                for (int j = 0; j < selectorList.Count; j++)
                {
                    int itm = selectorList[j];
                    double value = valuestring[itm];

                    mean = (mean + value);
                    tempmean = mean / (j + 1);

                    variance1 += Math.Pow(value - tempmean, 2.0);
                    variance1 = variance1 / (j + 1);

                    int xDevice = (int)(FromXRealToXVirtual(j, minX, maxX, VirtualWindow.Left, VirtualWindow.Width));
                    int YDevice = (int)(FromYRealToYVirtual(variance1, minY, maxY, VirtualWindow.Top, VirtualWindow.Height));
                    Punti.Add(new Point(xDevice, YDevice));
                }
                



            }
            double help = 0.0;
            int n = 1;
                foreach (double j in variancelist)
                {
                    this.richTextBox1.AppendText("variance of sample " + n.ToString() + ": " + j.ToString() + "\n");
                    help += j;
                    n++;
                }
            help /= trialsnumber;
            this.richTextBox1.AppendText("VAR: " + help.ToString());
            this.pictureBox1.Image = b;

            this.bIsto = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            this.gIsto = Graphics.FromImage(bIsto);

            this.gIsto.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            this.gIsto.Clear(Color.White);

            Rectangle VirtualWindow2 = new Rectangle(0, 0, this.bIsto.Width - 1, this.bIsto.Height - 1);
            gIsto.DrawRectangle(Pens.Black, VirtualWindow2);

            double minAvg = variancelist.Min();
            double maxAvg = variancelist.Max();
            double delta = maxAvg - minAvg;
            double nintervals = 25;
            double intervalsSize = delta / nintervals;

            Dictionary<double, int> istoDict = new Dictionary<double, int>();

            double tempValue = minAvg;
            for (int i = 0; i < nintervals; i++)
            {
                istoDict[tempValue] = 0;
                tempValue = tempValue + intervalsSize;
            }

            int total = 0;

            foreach (double value in variancelist)
            {
                foreach (double key in istoDict.Keys)
                {
                    if (value < key + intervalsSize)
                    {
                        total++;
                        istoDict[key] += 1;
                        break;
                    }
                }
            }

            gIsto.TranslateTransform(0, this.bIsto.Height);
            gIsto.ScaleTransform(1, -1);

            int idIsto = 0;
            int widthIsto = (int)(this.bIsto.Width / nintervals);
            foreach (double key in istoDict.Keys)
            {
                int newHeight = istoDict[key] * this.bIsto.Height / total;
                int newX = (widthIsto * idIsto) + 1;
                Rectangle isto = new Rectangle(newX, 0, widthIsto, newHeight);
                idIsto++;

                int nextWidthIstogram = (int)(widthIsto * idIsto * 1);

                SolidBrush the_brush = new SolidBrush(Color.Green);
                gIsto.DrawRectangle(Pens.Black, isto);
                gIsto.FillRectangle(the_brush, isto);
            }
            this.pictureBox1.Image = bIsto;
 
        }

        private void richTextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            boolWeight = true;
            tipo = "WEIGHT";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            boolWeight = false;
            tipo = "HEIGHT";
        }
    }
}

