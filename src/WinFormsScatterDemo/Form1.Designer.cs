namespace WinFormsScatterDemo
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            PanelOuter = new TableLayoutPanel();
            PanelConfig = new Panel();
            BtnUpdate = new Button();
            ChkBoxCreateNaN = new CheckBox();
            MyFormsPlot = new ScottPlot.FormsPlot();
            PanelOuter.SuspendLayout();
            PanelConfig.SuspendLayout();
            SuspendLayout();
            // 
            // PanelOuter
            // 
            PanelOuter.ColumnCount = 1;
            PanelOuter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            PanelOuter.Controls.Add(PanelConfig, 0, 0);
            PanelOuter.Controls.Add(MyFormsPlot, 0, 1);
            PanelOuter.Dock = DockStyle.Fill;
            PanelOuter.Location = new Point(0, 0);
            PanelOuter.Name = "PanelOuter";
            PanelOuter.RowCount = 2;
            PanelOuter.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            PanelOuter.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            PanelOuter.Size = new Size(800, 450);
            PanelOuter.TabIndex = 1;
            // 
            // PanelConfig
            // 
            PanelConfig.Controls.Add(BtnUpdate);
            PanelConfig.Controls.Add(ChkBoxCreateNaN);
            PanelConfig.Dock = DockStyle.Fill;
            PanelConfig.Location = new Point(3, 3);
            PanelConfig.Name = "PanelConfig";
            PanelConfig.Size = new Size(794, 44);
            PanelConfig.TabIndex = 1;
            // 
            // BtnUpdate
            // 
            BtnUpdate.Location = new Point(120, 7);
            BtnUpdate.Name = "BtnUpdate";
            BtnUpdate.Size = new Size(75, 23);
            BtnUpdate.TabIndex = 1;
            BtnUpdate.Text = "生成曲线";
            BtnUpdate.UseVisualStyleBackColor = true;
            BtnUpdate.Click += BtnUpdate_Click;
            // 
            // ChkBoxCreateNaN
            // 
            ChkBoxCreateNaN.AutoSize = true;
            ChkBoxCreateNaN.Location = new Point(9, 9);
            ChkBoxCreateNaN.Name = "ChkBoxCreateNaN";
            ChkBoxCreateNaN.Size = new Size(90, 21);
            ChkBoxCreateNaN.TabIndex = 0;
            ChkBoxCreateNaN.Text = "含有NaN值";
            ChkBoxCreateNaN.UseVisualStyleBackColor = true;
            // 
            // MyFormsPlot
            // 
            MyFormsPlot.Dock = DockStyle.Fill;
            MyFormsPlot.Location = new Point(4, 53);
            MyFormsPlot.Margin = new Padding(4, 3, 4, 3);
            MyFormsPlot.Name = "MyFormsPlot";
            MyFormsPlot.Size = new Size(792, 394);
            MyFormsPlot.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(PanelOuter);
            Name = "Form1";
            Text = "Form1";
            PanelOuter.ResumeLayout(false);
            PanelConfig.ResumeLayout(false);
            PanelConfig.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel PanelOuter;
        private Panel PanelConfig;
        private Button BtnUpdate;
        private CheckBox ChkBoxCreateNaN;
        private ScottPlot.FormsPlot MyFormsPlot;
    }
}
