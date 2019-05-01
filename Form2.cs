using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
namespace PSO_Final
{
    public partial class Form2 : Form
    {
        private int iterations;
        private int iterations2;
        private int iterations3;
        List<double> fitnesses;
        List<double> fitnesses2;
        List<double> fitnesses3;
        int runs;
        string problemString;
        public Form2(ref List<double> fitnesses, int iterations, int runs, string problemString)
        {
            InitializeComponent();
            this.iterations = iterations;
            this.fitnesses = fitnesses;
            this.runs = runs;
            this.problemString = problemString;
        }
        public Form2(ref List<double> fitnesses, ref List<double> fitnesses2, int iterations, int iterations2, int runs)
        {
            InitializeComponent();
            this.iterations = iterations;
            this.iterations2 = iterations2;
            this.fitnesses = fitnesses;
            this.fitnesses2 = fitnesses2;
            this.runs = runs;
        }
        public Form2(ref List<double> fitnesses, ref List<double> fitnesses2, ref List<double>fitnesses3, int iterations, int iterations2, int iterations3, int runs)
        {
            InitializeComponent();
            this.iterations = iterations;
            this.iterations2 = iterations2;
            this.iterations3 = iterations3;
            this.fitnesses = fitnesses;
            this.fitnesses2 = fitnesses2;
            this.fitnesses3 = fitnesses3;
            this.runs = runs;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Lets generate sine and cosine wave
            double[] x = new double[iterations];
            double[] y = new double[fitnesses.Count()];
            

            for (int i = 0; i < iterations; ++i)
            {
                x[i] = i;
                y[i] = fitnesses[i];
            }

            // This is to remove all plots
            zedGraphControl1.GraphPane.CurveList.Clear();

            // GraphPane object holds one or more Curve objects (or plots)
            GraphPane myPane = zedGraphControl1.GraphPane;

            // PointPairList holds the data for plotting, X and Y arrays 
            PointPairList spl1 = new PointPairList(x, y);
           // PointPairList spl2 = new PointPairList(x, z);

            // Add cruves to myPane object
            LineItem myCurve1 = myPane.AddCurve("Sine Wave", spl1, Color.Blue, SymbolType.None);
           // LineItem myCurve2 = myPane.AddCurve("Cosine Wave", spl2, Color.Red, SymbolType.None);

            myCurve1.Line.Width = 3.0F;
           // myCurve2.Line.Width = 3.0F;
            myPane.Title.Text = "My First Plot";

            // I add all three functions just to be sure it refeshes the plot.   
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
            zedGraphControl1.Refresh();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (fitnesses3 != null)
            {
                double[] x = new double[iterations];
                double[] x2 = new double[iterations2];
                double[] y = new double[iterations];
                double[] y2 = new double[iterations2];
                double[] x3 = new double[iterations3];
                double[] y3 = new double[iterations3];

                for (int i = 0; (i < iterations); ++i)
                {
                    x[i] = i;
                    y[i] = fitnesses[i];
                }
                for (int i = 0; (i < iterations2); ++i)
                {
                    x2[i] = i;
                    y2[i] = fitnesses2[i];
                }
                for (int i = 0; (i < iterations3); ++i)
                {
                    x3[i] = i;
                    y3[i] = fitnesses3[i];
                }
                // This is to remove all plots
                zedGraphControl1.GraphPane.CurveList.Clear();

                // GraphPane object holds one or more Curve objects (or plots)
                GraphPane myPane = zedGraphControl1.GraphPane;

                // PointPairList holds the data for plotting, X and Y arrays 
                PointPairList spl1 = new PointPairList(x, y);
                PointPairList spl2 = new PointPairList(x2, y2);
                PointPairList spl3 = new PointPairList(x3, y3);
                // Add cruves to myPane object
                string curve1_str = "c1 = 0.1, c2 = 1.9";
                string curve2_str = "c1 = 1.9, c2 = 0.1";
                string curve3_str = "c1 = 1.9, c2 = 1.4";
                LineItem myCurve1 = myPane.AddCurve(curve1_str, spl1, Color.Blue, SymbolType.None);
                LineItem myCurve2 = myPane.AddCurve(curve2_str, spl2, Color.Red, SymbolType.None);
                LineItem myCurve3 = myPane.AddCurve(curve3_str, spl3, Color.Green, SymbolType.None);

                myCurve1.Line.Width = 1.2F;
                myCurve2.Line.Width = 1.2F;
                myCurve3.Line.Width = 1.2F;
                // myCurve2.Line.IsSmooth = true;
                // myCurve2.Line.IsAntiAlias = true;
                // myCurve1.Line.IsSmooth = true;
                // myCurve1.Line.IsAntiAlias = true;
                myPane.Title.Text = "Knapsack Problem";

                // I add all three functions just to be sure it refeshes the plot. 

                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
                zedGraphControl1.Refresh();
            }
            else if (fitnesses2 != null)
            {
                //double[] x = new double[iterations];
                //double[] x2 = new double[iterations2];
                //double[] y = new double[fitnesses.Count()];
                //double[] y2 = new double[fitnesses2.Count()];
            
                double[] x = new double[iterations];
                double[] x2 = new double[iterations2];
                double[] y = new double[iterations];
                double[] y2 = new double[iterations2];

                for (int i = 0; (i < iterations); ++i)
                {
                    x[i] = i;
                    y[i] = fitnesses[i];
                }
                for (int i = 0; (i < iterations2); ++i)
                {
                    x2[i] = i;
                    y2[i] = fitnesses2[i];
                }
                // This is to remove all plots
                zedGraphControl1.GraphPane.CurveList.Clear();

                // GraphPane object holds one or more Curve objects (or plots)
                GraphPane myPane = zedGraphControl1.GraphPane;

                // PointPairList holds the data for plotting, X and Y arrays 
                PointPairList spl1 = new PointPairList(x, y);
                PointPairList spl2 = new PointPairList(x2, y2);

                // Add cruves to myPane object
                string curve1_str = "1D Circle Problem" + "(" + (iterations - 1) + ") iterations)";
                string curve2_str = "2D Circle Problem" + "(" + (iterations2 - 1) + ") iterations)";
                LineItem myCurve1 = myPane.AddCurve(curve1_str, spl1, Color.Blue, SymbolType.None);
                LineItem myCurve2 = myPane.AddCurve(curve2_str, spl2, Color.Red, SymbolType.None);

                myCurve1.Line.Width = 1.2F;
                myCurve2.Line.Width = 1.2F;
               // myCurve2.Line.IsSmooth = true;
               // myCurve2.Line.IsAntiAlias = true;
               // myCurve1.Line.IsSmooth = true;
               // myCurve1.Line.IsAntiAlias = true;
                myPane.Title.Text = "Circle Problem";

                // I add all three functions just to be sure it refeshes the plot. 
       
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
                zedGraphControl1.Refresh();
            }
            else
            {
                double[] x = new double[iterations];
                double[] y = new double[fitnesses.Count()];

                for (int i = 0; i < iterations; ++i)
                {
                    x[i] = i;
                    y[i] = fitnesses[i];
                }
          
                // This is to remove all plots
                zedGraphControl1.GraphPane.CurveList.Clear();

                // GraphPane object holds one or more Curve objects (or plots)
                GraphPane myPane = zedGraphControl1.GraphPane;

                // PointPairList holds the data for plotting, X and Y arrays 
                PointPairList spl1 = new PointPairList(x, y);

                // Add cruves to myPane object
                LineItem myCurve1 = myPane.AddCurve(problemString, spl1, Color.Blue, SymbolType.None);

                myCurve1.Line.Width = 1.0F;
                myPane.Title.Text = "Knapsack Problem";

                // I add all three functions just to be sure it refeshes the plot.   
                zedGraphControl1.AxisChange();
                zedGraphControl1.Invalidate();
                zedGraphControl1.Refresh();
            }
        }
    }
}
