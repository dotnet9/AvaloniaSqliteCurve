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

            for (var i = 0; i < valueCount; i++)
            {
                x[i] = i;
                if (ChkBoxCreateNaN.Checked && i % 3 == 0)
                {
                    y[i] = double.NaN;
                }
                else
                {
                    y[i] = Random.Shared.Next(0, 100);
                }
            }
            MyFormsPlot.Plot.Clear();
            var scatter = MyFormsPlot.Plot.Add.Scatter(x, y);
            MyFormsPlot.Plot.Axes.SetLimitsX(x.Min() - offset, x.Max() + offset);
            MyFormsPlot.Plot.Axes.SetLimitsY(x.Min() - offset, x.Max() + offset);
            MyFormsPlot.Refresh();
            //scatter.o
        }
    }
}