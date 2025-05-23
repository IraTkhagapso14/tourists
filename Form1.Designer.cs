namespace tourfirma
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button8 = new Button();
            tabPage6 = new TabPage();
            cmbParametricQuery = new ComboBox();
            cmbAggregateQuery = new ComboBox();
            button7 = new Button();
            button6 = new Button();
            dataGridViewResult = new DataGridView();
            button5 = new Button();
            label2 = new Label();
            button4 = new Button();
            label1 = new Label();
            tabPage5 = new TabPage();
            dataGridView6 = new DataGridView();
            tabPage4 = new TabPage();
            dataGridView5 = new DataGridView();
            tabPage3 = new TabPage();
            dataGridView4 = new DataGridView();
            tabPage2 = new TabPage();
            dataGridView3 = new DataGridView();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            chartBar = new System.Windows.Forms.DataVisualization.Charting.Chart();
            chartPie = new System.Windows.Forms.DataVisualization.Charting.Chart();
            tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewResult).BeginInit();
            tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView6).BeginInit();
            tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView5).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView4).BeginInit();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView3).BeginInit();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chartBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chartPie).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.BackColor = Color.DarkSeaGreen;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            button1.Location = new Point(27, 386);
            button1.Margin = new Padding(3, 4, 3, 4);
            button1.Name = "button1";
            button1.Size = new Size(170, 64);
            button1.TabIndex = 1;
            button1.Text = "Добавить";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.BackColor = SystemColors.ActiveCaption;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            button2.Location = new Point(593, 386);
            button2.Margin = new Padding(3, 4, 3, 4);
            button2.Name = "button2";
            button2.Size = new Size(170, 64);
            button2.TabIndex = 2;
            button2.Text = "Изменить";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.BackColor = Color.RosyBrown;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            button3.Location = new Point(877, 386);
            button3.Margin = new Padding(3, 4, 3, 4);
            button3.Name = "button3";
            button3.Size = new Size(170, 64);
            button3.TabIndex = 3;
            button3.Text = "Удалить";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // button8
            // 
            button8.BackColor = Color.Khaki;
            button8.FlatStyle = FlatStyle.Flat;
            button8.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            button8.Location = new Point(318, 386);
            button8.Margin = new Padding(2);
            button8.Name = "button8";
            button8.Size = new Size(170, 64);
            button8.TabIndex = 5;
            button8.Text = "Триггер";
            button8.UseVisualStyleBackColor = false;
            button8.Click += button8_Click;
            // 
            // tabPage6
            // 
            tabPage6.Controls.Add(cmbParametricQuery);
            tabPage6.Controls.Add(cmbAggregateQuery);
            tabPage6.Controls.Add(button7);
            tabPage6.Controls.Add(button6);
            tabPage6.Controls.Add(dataGridViewResult);
            tabPage6.Controls.Add(button5);
            tabPage6.Controls.Add(label2);
            tabPage6.Controls.Add(button4);
            tabPage6.Controls.Add(label1);
            tabPage6.Location = new Point(4, 29);
            tabPage6.Margin = new Padding(2);
            tabPage6.Name = "tabPage6";
            tabPage6.Padding = new Padding(2);
            tabPage6.Size = new Size(1012, 289);
            tabPage6.TabIndex = 5;
            tabPage6.Text = "Запросы";
            tabPage6.UseVisualStyleBackColor = true;
            // 
            // cmbParametricQuery
            // 
            cmbParametricQuery.FormattingEnabled = true;
            cmbParametricQuery.Location = new Point(18, 182);
            cmbParametricQuery.Margin = new Padding(2);
            cmbParametricQuery.Name = "cmbParametricQuery";
            cmbParametricQuery.Size = new Size(146, 28);
            cmbParametricQuery.TabIndex = 11;
            // 
            // cmbAggregateQuery
            // 
            cmbAggregateQuery.FormattingEnabled = true;
            cmbAggregateQuery.Location = new Point(19, 55);
            cmbAggregateQuery.Margin = new Padding(2);
            cmbAggregateQuery.Name = "cmbAggregateQuery";
            cmbAggregateQuery.Size = new Size(146, 28);
            cmbAggregateQuery.TabIndex = 10;
            // 
            // button7
            // 
            button7.BackColor = Color.Red;
            button7.FlatAppearance.BorderSize = 3;
            button7.FlatStyle = FlatStyle.Flat;
            button7.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            button7.Location = new Point(221, 85);
            button7.Margin = new Padding(2);
            button7.Name = "button7";
            button7.Size = new Size(200, 63);
            button7.TabIndex = 9;
            button7.Text = "Импорт из MS Excel";
            button7.UseVisualStyleBackColor = false;
            button7.Click += button7_Click;
            // 
            // button6
            // 
            button6.BackColor = Color.Red;
            button6.FlatAppearance.BorderSize = 3;
            button6.FlatStyle = FlatStyle.Flat;
            button6.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            button6.Location = new Point(221, 14);
            button6.Margin = new Padding(2);
            button6.Name = "button6";
            button6.Size = new Size(200, 58);
            button6.TabIndex = 8;
            button6.Text = "Экспорт в MS Excel";
            button6.UseVisualStyleBackColor = false;
            button6.Click += button6_Click;
            // 
            // dataGridViewResult
            // 
            dataGridViewResult.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewResult.Location = new Point(438, 28);
            dataGridViewResult.Margin = new Padding(2);
            dataGridViewResult.Name = "dataGridViewResult";
            dataGridViewResult.RowHeadersWidth = 62;
            dataGridViewResult.RowTemplate.Height = 33;
            dataGridViewResult.Size = new Size(558, 208);
            dataGridViewResult.TabIndex = 7;
            // 
            // button5
            // 
            button5.Location = new Point(18, 218);
            button5.Margin = new Padding(2);
            button5.Name = "button5";
            button5.Size = new Size(148, 48);
            button5.TabIndex = 5;
            button5.Text = "Выполнить";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(14, 150);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(211, 20);
            label2.TabIndex = 3;
            label2.Text = "Параметризованный запрос";
            // 
            // button4
            // 
            button4.Location = new Point(18, 94);
            button4.Margin = new Padding(2);
            button4.Name = "button4";
            button4.Size = new Size(146, 44);
            button4.TabIndex = 2;
            button4.Text = "Выполнить";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 14);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(182, 20);
            label1.TabIndex = 0;
            label1.Text = "Агрегированный запрос";
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(dataGridView6);
            tabPage5.Location = new Point(4, 29);
            tabPage5.Margin = new Padding(3, 4, 3, 4);
            tabPage5.Name = "tabPage5";
            tabPage5.Padding = new Padding(3, 4, 3, 4);
            tabPage5.Size = new Size(1012, 289);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "Оплата";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // dataGridView6
            // 
            dataGridView6.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView6.Location = new Point(17, 18);
            dataGridView6.Margin = new Padding(3, 4, 3, 4);
            dataGridView6.Name = "dataGridView6";
            dataGridView6.RowHeadersWidth = 62;
            dataGridView6.RowTemplate.Height = 25;
            dataGridView6.Size = new Size(970, 222);
            dataGridView6.TabIndex = 0;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(dataGridView5);
            tabPage4.Location = new Point(4, 29);
            tabPage4.Margin = new Padding(3, 4, 3, 4);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3, 4, 3, 4);
            tabPage4.Size = new Size(1012, 289);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Сезоны";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // dataGridView5
            // 
            dataGridView5.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView5.Location = new Point(17, 18);
            dataGridView5.Margin = new Padding(3, 4, 3, 4);
            dataGridView5.Name = "dataGridView5";
            dataGridView5.RowHeadersWidth = 62;
            dataGridView5.RowTemplate.Height = 25;
            dataGridView5.Size = new Size(978, 221);
            dataGridView5.TabIndex = 0;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(dataGridView4);
            tabPage3.Location = new Point(4, 29);
            tabPage3.Margin = new Padding(3, 4, 3, 4);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3, 4, 3, 4);
            tabPage3.Size = new Size(1012, 289);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Туры";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // dataGridView4
            // 
            dataGridView4.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView4.Location = new Point(20, 20);
            dataGridView4.Margin = new Padding(3, 4, 3, 4);
            dataGridView4.Name = "dataGridView4";
            dataGridView4.RowHeadersWidth = 62;
            dataGridView4.RowTemplate.Height = 25;
            dataGridView4.Size = new Size(871, 220);
            dataGridView4.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(dataGridView3);
            tabPage2.Location = new Point(4, 29);
            tabPage2.Margin = new Padding(3, 4, 3, 4);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3, 4, 3, 4);
            tabPage2.Size = new Size(1012, 289);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Информация о туристах";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView3
            // 
            dataGridView3.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView3.Location = new Point(24, 26);
            dataGridView3.Margin = new Padding(3, 4, 3, 4);
            dataGridView3.Name = "dataGridView3";
            dataGridView3.RowHeadersWidth = 62;
            dataGridView3.RowTemplate.Height = 25;
            dataGridView3.Size = new Size(964, 211);
            dataGridView3.TabIndex = 0;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Controls.Add(tabPage6);
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Location = new Point(27, 40);
            tabControl1.Margin = new Padding(3, 4, 3, 4);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1020, 322);
            tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(chartBar);
            tabPage1.Controls.Add(chartPie);
            tabPage1.Location = new Point(4, 29);
            tabPage1.Margin = new Padding(2);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(2);
            tabPage1.Size = new Size(1012, 289);
            tabPage1.TabIndex = 6;
            tabPage1.Text = "Диаграммы";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // chartBar
            // 
            chartArea1.Name = "ChartArea1";
            chartBar.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            chartBar.Legends.Add(legend1);
            chartBar.Location = new Point(302, 12);
            chartBar.Margin = new Padding(2);
            chartBar.Name = "chartBar";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            chartBar.Series.Add(series1);
            chartBar.Size = new Size(570, 198);
            chartBar.TabIndex = 1;
            chartBar.Text = "chart1";
            // 
            // chartPie
            // 
            chartArea2.Name = "ChartArea1";
            chartPie.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            chartPie.Legends.Add(legend2);
            chartPie.Location = new Point(18, 12);
            chartPie.Margin = new Padding(2);
            chartPie.Name = "chartPie";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            chartPie.Series.Add(series2);
            chartPie.Size = new Size(254, 220);
            chartPie.TabIndex = 0;
            chartPie.Text = "chart1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1095, 530);
            Controls.Add(button8);
            Controls.Add(tabControl1);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Тхагапсо_туристы";
            tabPage6.ResumeLayout(false);
            tabPage6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewResult).EndInit();
            tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView6).EndInit();
            tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView5).EndInit();
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView4).EndInit();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView3).EndInit();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)chartBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)chartPie).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button8;
        private TabPage tabPage6;
        private ComboBox cmbParametricQuery;
        private ComboBox cmbAggregateQuery;
        private Button button7;
        private Button button6;
        private DataGridView dataGridViewResult;
        private Button button5;
        private Label label2;
        private Button button4;
        private Label label1;
        private TabPage tabPage5;
        private DataGridView dataGridView6;
        private TabPage tabPage4;
        private DataGridView dataGridView5;
        private TabPage tabPage3;
        private DataGridView dataGridView4;
        private TabPage tabPage2;
        private DataGridView dataGridView3;
        private TabControl tabControl1;
        private Panel panel1;
        private TabPage tabPage1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartPie;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartBar;
    }
}