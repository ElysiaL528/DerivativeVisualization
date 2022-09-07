using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DerivativeVisualization2
{
    public partial class Form1 : Form
    {
        Graphics graphics;
        Bitmap map;
        PictureBox Graph;

        List<PointF> points = new List<PointF>();

        /// <summary>
        /// PB coordinates
        /// </summary>
        float leftmostX;
        /// <summary>
        /// PB coordinates
        /// </summary>
        float rightmostX;

        float centerX;
        float centerY;
        float ogY;

        private float a = 1 / 10f;
        float h = 0;
        float k = 0;

        /// <summary>
        /// Raw coordinates
        /// </summary>
        float deltaX;

        TrackBar derivativePointBar;
        NumericUpDown aUpDown;
        Label otherPointLabel;

        Button visualizeButton = new Button();
        System.Windows.Forms.Timer visualizationTimer = new System.Windows.Forms.Timer();
        public Form1()
        {
            InitializeComponent();

            Graph = new PictureBox()
            {
                Location = new Point(0, 0),
                BackColor = Color.AliceBlue,
                Size = new Size(700, ClientSize.Height)
            };
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            Label derivativeLabel = new Label()
            {
                Text = "Tangent point location:",
                AutoSize = true,
                Location = new Point(0, 80)
            };

            centerX = Graph.Width / 2;
            centerY = Graph.Height - 100;
            ogY = centerY;

            Controls.Add(Graph);

            map = new Bitmap(Graph.Width, Graph.Height);
            graphics = Graphics.FromImage(map);

            points.Add(new Point(0, 0));

            Graph.Controls.Add(derivativeLabel);
            derivativePointBar = new TrackBar()
            {
                Location = new Point(0, derivativeLabel.Bottom),
                TickStyle = TickStyle.None,
                TickFrequency = 1,
                Value = 0,
                AutoSize = true
            };
            derivativePointBar.ValueChanged += DerivativePointBar_ValueChanged;


            Graph.Controls.Add(derivativePointBar);


            Label aLabel = new Label()
            {
                Location = new Point(0, derivativePointBar.Bottom),
                AutoSize = true,
                Text = "A: "
            };
            Graph.Controls.Add(aLabel);
            aUpDown = new NumericUpDown()
            {
                Location = new Point(aLabel.Right, derivativePointBar.Bottom),
                Minimum = 0.01M,
                Maximum = 1.5M,
                DecimalPlaces = 2,
                Increment = 0.01M,
                Value = (decimal)a,
                AutoSize = true
            };
            Graph.Controls.Add(aUpDown);

            otherPointLabel = new Label();
            otherPointLabel.Text = "hello";
            otherPointLabel.Location = new Point(0, aUpDown.Bottom);
            otherPointLabel.AutoSize = true;
            Graph.Controls.Add(otherPointLabel);

            aUpDown.ValueChanged += AUpDown_ValueChanged;

            visualizeButton.Location = new Point(Graph.Width + Graph.Location.X + 20, 50);
            visualizeButton.Text = "Visualize";
            visualizeButton.Click += Visualize_Click;
            Controls.Add(visualizeButton);

            visualizationTimer.Tick += Visualization_Tick;
            

            Draw();
            //DrawFinalDerivative();
        }

        PointF leftPoint = new PointF();
        PointF rightPoint = new PointF();

        float leftXInt = 0;
        float rightXInt = 0;
        private void Visualize_Click(object sender, EventArgs e)
        {

            leftPoint = new PointF(leftmostX, centerY - f(leftmostX - centerX));
            rightPoint = new PointF(rightmostX, centerY - f(rightmostX - centerX));

            //graphics.DrawRectangle(new Pen(Brushes.Green, 5), leftPoint.X - 3, leftPoint.Y - 3, 6, 6);
            //graphics.DrawRectangle(new Pen(Brushes.Green, 5), rightPoint.X - 3, rightPoint.Y - 3, 6, 6);

            Graph.Image = map;

            float x = derivativePointBar.Value + centerX;
            float y = centerY - f(derivativePointBar.Value);

            visualizationTimer.Enabled = true;

            if (derivativePointBar.Value < 0)
            {

                //otherPoint = new PointF(x, toPbY(f(toRawX(x))));
                deltaX = rightmostX - x;
            }
            else
            {
                //x = leftmostX + deltaX;
                deltaX = leftmostX - x;
            }

        }
        /// <summary>
        /// PB coordinates
        /// </summary>
        /// <returns></returns>
        

        

        private void Visualization_Tick(object sender, EventArgs e)
        {
            var chosenPoint = GetSelectedPoint();

            Text = $"{derivativePointBar.Value}";

            //PointF otherPoint;

            //bool leftSide; //chosen point is on left side of vertex

            float x = toRawX(chosenPoint.X) + deltaX;

            //if (derivativePointBar.Value < 0)
            //{

            //    //otherPoint = new PointF(x, toPbY(f(toRawX(x))));
            //    leftSide = true;
            //}
            //else
            //{
            //    x = leftmostX + deltaX;   
            //    leftSide = false;
            //}
            
            float y = f(x);

            

            

            //graphics.DrawLine(new Pen(Brushes.Black, 5), chosenPoint, otherPoint);

            //float otherX = leftSide ? rightmostX : leftmostX;

            float slope = (y - toRawY(chosenPoint.Y)) / (x - toRawX(chosenPoint.X));

            graphics.Clear(Graph.BackColor);
            DrawWholeGraph();
            graphics.DrawRectangle(new Pen(Brushes.Goldenrod, 5), toPbX(x), toPbY(y), 5, 5);
            DrawLine(chosenPoint, slope);
            DrawChosenPoint();
            //graphics.DrawLine(new Pen(Brushes.Purple, 5), chosenPoint, toPBPoint(new PointF(x, y)));

            Graph.Image = map;

            otherPointLabel.Text = $"Δx: {deltaX} \nSlope: {Math.Round(slope, 4)}";

            float increment = 0.7f;

            deltaX = deltaX < 0 ? deltaX + increment : deltaX - increment;

            //graphics.DrawString($"\n\nSlope: {Math.Round(slope, 4)}", this.Font, Brushes.Black, new PointF(0, 0));

            if (Math.Abs(deltaX) <= increment)
            {
                visualizationTimer.Enabled = false;
                Draw();
                otherPointLabel.Text = $"Δx: 0 \nSlope: {Math.Round(slope, 4)}";
                //DrawFinalDerivative();
            }
        }

        private void DrawLine(PointF point, float slope)
        {
            //y = mx + b
            //y - ? = slope(x - leftmostX)
            //? = -slope(x - leftmostX) + y

            //if(slope > 0)
            //{

            //}

            float leftY = -1 * slope * (toRawX(point.X) - toRawX(leftmostX)) + toRawY(point.Y);
            float rightY = -1 * slope * (toRawX(point.X) - toRawX(rightmostX)) + toRawY(point.Y);


            PointF point1 = toPBPoint(new PointF(toRawX(leftmostX), leftY));
            PointF point2 = toPBPoint(new PointF(toRawX(rightmostX), rightY));

            graphics.DrawLine(new Pen(Brushes.Purple, 3), point1, point2);
        }

        private PointF GetSelectedPoint()
        {
            float selectedX = derivativePointBar.Value + h;
            float y = f(selectedX);

            PointF chosenPoint = new PointF(selectedX + centerX, centerY - y);

            return chosenPoint;
        }

        float changeAmount = 3f;

        [DllImport("user32")]
        public static extern int GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        public static extern int GetKeyboardState(byte[] keystate);

        byte[] keys = new byte[256];
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {

            GetKeyboardState(keys);

            if ((keys[(int)Keys.A] & 128) == 128)
            {
                h -= changeAmount;
            }
            if ((keys[(int)Keys.D] & 128) == 128)
            {
                h += changeAmount;
            }
            if ((keys[(int)Keys.W] & 128) == 128)
            {
                k += changeAmount;
            }
            if ((keys[(int)Keys.S] & 128) == 128)
            {
                k -= changeAmount;
            }
            Draw();
        }

        private void AUpDown_ValueChanged(object sender, EventArgs e)
        {
            a = (float)aUpDown.Value;
            Draw();
        }

        void DrawWholeGraph()
        {
            DrawBackgroundGraph();
            DrawGraph();
        }

        //void DrawFinalDerivative()
        //{
        //    float x = centerX + derivativePointBar.Value + h;
        //    float newX = x - centerX;
        //    PointF clickedPoint = new PointF(newX, f(newX));
        //    float slope = fprime(clickedPoint.X);
        //    float b = clickedPoint.Y - slope * clickedPoint.X;
            
        //    float leftmostY = centerY - ((leftmostX - centerX) * slope + b);
        //    float rightmostY = centerY - ((rightmostX - centerX) * slope + b);
        //    graphics.DrawLine(slopePen, new PointF(leftmostX, leftmostY), new PointF(rightmostX, rightmostY));
        //}

        void DrawChosenPoint()
        {
            float x = centerX + derivativePointBar.Value + h;
            float newX = x - centerX;
            PointF clickedPoint = new PointF(newX, f(newX));
            graphics.FillEllipse(Brushes.Orange, new RectangleF(x - circleRadius, centerY - clickedPoint.Y - circleRadius, circleRadius * 2, circleRadius * 2));
        }

        void Draw()
        {
            graphics.Clear(Graph.BackColor);
            DrawWholeGraph();

            derivativePointBar.Minimum = (int)((leftmostX - (centerX + h)));
            derivativePointBar.Maximum = (int)((rightmostX - (centerX + h)));
//            Text = $"Use arrow keys to move the graph around.";

            float x = centerX + derivativePointBar.Value + h;
            float newX = x - centerX;
            PointF clickedPoint = new PointF(newX, f(newX));

            float slope = fprime(clickedPoint.X);

            float b = clickedPoint.Y - slope * clickedPoint.X;

            string output = $"Location of point: X:{clickedPoint.X}, Y:{clickedPoint.Y}\nEquation of line: y = {Math.Round(slope, 4)}*x + {Math.Round(b, 4)}\nSlope: {Math.Round(slope, 4)}\nEquation of Parabola: y = {a}(x - {h})^2 + {k}";
            DrawOuputStr(output);

            float leftmostY = centerY - ((leftmostX - centerX) * slope + b);
            float rightmostY = centerY - ((rightmostX - centerX) * slope + b);


            graphics.DrawLine(slopePen, new PointF(leftmostX, leftmostY), new PointF(rightmostX, rightmostY));
            //            graphics.FillEllipse(Brushes.Orange, new RectangleF(x - circleRadius, centerY - clickedPoint.Y - circleRadius, circleRadius * 2, circleRadius * 2));
            DrawChosenPoint();
            Graph.Image = map;
        }

        float circleRadius = 5f;
        private void DerivativePointBar_ValueChanged(object sender, EventArgs e)
        {
            Draw();
            //DrawFinalDerivative();

        }

        Pen linePen = new Pen(Brushes.Red, 3);
        Pen slopePen = new Pen(Brushes.Blue, 5);
        Pen backgroundGraphPen = new Pen(Brushes.Black, 2);


        void DrawGraph()
        {
            leftmostX = float.MaxValue;
            rightmostX = float.MaxValue;

            points.Clear();
            for (float x = 0; x < Graph.Width; x++)
            {
                float y = f(x - centerX);
                PointF point = new PointF(x, centerY - y);

                if (point.Y >= 0 && leftmostX == float.MaxValue)
                {
                    leftmostX = x;
                }
                if (point.Y >= 0 && leftmostX != float.MaxValue)// && rightmostX == float.MaxValue)
                {
                    rightmostX = x;
                }
                if (points.Count > 0)
                {
                    graphics.DrawLine(linePen, points[points.Count - 1], point);
                }

                points.Add(point);
            }

            Graph.Image = map;
        }

        void DrawBackgroundGraph()
        {
            graphics.DrawLine(backgroundGraphPen, new PointF(0, ogY), new PointF(Graph.Width, ogY));
            graphics.DrawLine(backgroundGraphPen, new PointF(centerX, 0), new PointF(centerX, Graph.Height));
        }
        void DrawOuputStr(string output)
        {
            graphics.DrawString(output, this.Font, Brushes.Black, new PointF(0, 0));
        }
        public float f(float x)
            => a * (float)Math.Pow(x - h, 2) + k;

        public float fprime(float x)
            => 2 * a * (x - h);
        public float inverseF(float y)
            => (float)Math.Sqrt((y - k) / a) + h;

        private float toPbX(float rawX) => rawX + centerX;
        private float toRawX(float pBX) => pBX - centerX;
        private float toPbY(float rawY) => centerY - rawY;
        private float toRawY(float pBY) => centerY - pBY;
        private PointF toPBPoint(PointF rawPoint) => new PointF(toPbX(rawPoint.X), toPbY(rawPoint.Y));
        private PointF toRawPoint(PointF pbPoint) => new PointF(toRawX(pbPoint.X), toRawY(pbPoint.Y));
        
    }
}