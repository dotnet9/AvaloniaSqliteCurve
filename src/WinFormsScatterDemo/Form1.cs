using ScottPlot;
using ScottPlot.Plottable;

namespace WinFormsScatterDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            var valueCount = Random.Shared.Next(10, 200);
            var x = new double[valueCount];
            var y = new double[valueCount];
            var offset = 10;
            double xMin;
            double xMax;
            var yMin = double.MaxValue;
            var yMax = double.MinValue;

            xMin = 0 - offset;
            xMax = valueCount + offset;
            for (var i = 0; i < valueCount; i++)
            {
                x[i] = i;
                if (ChkBoxCreateNaN.Checked && i % 6 >= 2)
                {
                    y[i] = double.NaN;
                }
                else
                {
                    y[i] = Random.Shared.Next(0, 100);
                    if (y[i] > yMax)
                    {
                        yMax = y[i];
                    }

                    if (y[i] < yMin)
                    {
                        yMin = y[i];
                    }
                }
            }

            MyFormsPlot.Plot.Clear();
            var scatter = MyFormsPlot.Plot.AddScatter(x, y);
            MyFormsPlot.Plot.SetAxisLimits(xMin, xMax, yMin - offset, yMax + offset);
            // 只在4.X版本有效
            scatter.OnNaN = ScatterPlot.NanBehavior.Ignore;
            MyFormsPlot.Refresh();
        }
    }
}