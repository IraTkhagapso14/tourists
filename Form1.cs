using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;
using System.IO;
using ClosedXML.Excel;
using System.Windows.Forms.DataVisualization.Charting;
using DocumentFormat.OpenXml.Packaging;

namespace tourfirma
{
    public partial class Form1 : Form
    {
        private NpgsqlConnection con;
        private string connString = "Host=127.0.0.1;Username=postgres;Password=postpass;Database=Tour;Include Error Detail=true";
        private DataGridViewRow selectedRow;

        public Form1()
        {
            InitializeComponent();
            con = new NpgsqlConnection(connString);
            con.Open();
            loadTouristsCombined();
            loadSeasons();
            loadPutevki();
            loadPayment();
            InitializeQueryComboBoxes();
            InitializeCharts();
            UpdateBarChart();
            UpdatePieChart();

        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab?.Name == "tabPage1") // �������� �� ��� ����� ������� � �����������
            {
                UpdateCharts();
            }
        }
        private Series CreateSeries(string name, Color color, string tooltipField)
        {
            return new Series(name)
            {
                ChartType = SeriesChartType.Column,
                Color = color,
                IsValueShownAsLabel = false, // ��������� ����������� �������� �� ��������
                ["PointWidth"] = "0.6",
                ToolTip = $"{name}: #VALY" // ��������� ��� ���������
            };
        }

        private void UpdateCharts()
        {
            UpdatePieChart();
            UpdateBarChart();
        }

        private void UpdatePieChart()
        {
            try
            {
                chartPie.Series.Clear();
                chartPie.Legends.Clear();

                string sql = @"
    WITH total_putevki AS (
        SELECT 
            SUM(s.amount) as total_seats,
            COUNT(p.putevki_id) as sold_seats
        FROM seasons s
        LEFT JOIN putevki p ON s.season_id = p.season_id
    )
    SELECT 
        '�����������' as category,
        (sold_seats * 100.0 / total_seats) as percentage
    FROM total_putevki
    UNION ALL
    SELECT 
        '�� �����������' as category,
        ((total_seats - sold_seats) * 100.0 / total_seats) as percentage
    FROM total_putevki";

                DataTable dt = new DataTable();
                new NpgsqlDataAdapter(sql, con).Fill(dt);

                Series series = new Series("����");
                series.ChartType = SeriesChartType.Pie;
                series.IsValueShownAsLabel = false; // ������ �������� � ��������
                series.Font = new Font("Arial", 10);

                foreach (DataRow row in dt.Rows)
                {
                    double percentage = Math.Round(Convert.ToDouble(row["percentage"]), 1);
                    DataPoint point = series.Points.Add(percentage);
                    point.LegendText = $"{row["category"]} ({percentage}%)"; // �������� ������ � �������
                }

                chartPie.Series.Add(series);
                chartPie.Legends.Add(new Legend("Legend"));
                chartPie.Legends["Legend"].Docking = Docking.Bottom;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ � �������� ���������: {ex.Message}");
            }
        }
        private void InitializeCharts()
        {
            // ������������� �������� ��������� (������� ������ �����)
            chartPie.Series.Clear();
            chartPie.Titles.Clear();
            chartPie.Titles.Add("������� ������ �����");
            chartPie.Legends.Clear();
            chartPie.Legends.Add(new Legend("Legend1"));
            chartPie.Legends["Legend1"].Docking = Docking.Bottom;

            // ������������� ���������� ��������� (� 3 �������)
            chartBar.Series.Clear();
            chartBar.Titles.Clear();
            chartBar.Titles.Add("���������� �� �����");
            chartBar.ChartAreas[0].AxisX.Title = "����";
            chartBar.ChartAreas[0].AxisY.Title = "��������";
            chartBar.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chartBar.ChartAreas[0].AxisY.LabelStyle.Format = "N0";

            UpdateCharts();
        }

        private void UpdateBarChart()
        {
            try
            {
                chartBar.Series.Clear();
                chartBar.ChartAreas[0].AxisX.Interval = 1;

                string sql = @"
            SELECT 
                t.tour_name,
                COALESCE(SUM(p.price::numeric), 0) as total_sum,
                COALESCE(AVG(p.price::numeric), 0) as avg_price
            FROM tours t
            LEFT JOIN seasons s ON t.tour_id = s.tour_id
            LEFT JOIN putevki p ON s.season_id = p.season_id
            GROUP BY t.tour_name
            ORDER BY t.tour_name";

                DataTable dt = new DataTable();
                new NpgsqlDataAdapter(sql, con).Fill(dt);

                // ������� ������ ��� �����
                var series1 = CreateSeries("�����", Color.SteelBlue, "total_sum");
                var series2 = CreateSeries("��. ����", Color.MediumSeaGreen, "avg_price");

                foreach (DataRow row in dt.Rows)
                {
                    string tourName = row["tour_name"].ToString();
                    series1.Points.AddXY(tourName, Convert.ToDouble(row["total_sum"]));
                    series2.Points.AddXY(tourName, Convert.ToDouble(row["avg_price"]));
                }

                chartBar.Series.Add(series1);
                chartBar.Series.Add(series2);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ � ���������� ���������: {ex.Message}");
            }
            chartBar.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
        }

        // ��������������� ����� ��� ���������� ����� �������
        private void FillSeriesData(Series series, string sql, string valueColumn)
        {
            DataTable dt = new DataTable();
            new NpgsqlDataAdapter(sql, con).Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                string tourName = row["tour_name"].ToString();
                double value = Convert.ToDouble(row[valueColumn]);
                series.Points.AddXY(tourName, value);
            }
        }
        /*private void UpdateBarChart()
        {
            try
            {
                // ������ ��� ��������� ����� ������ �� ������� ����
                string sql = @"
            SELECT 
                t.tour_name as ���,
                COALESCE(SUM(p.price::numeric), 0.0) as �����
            FROM tours t
            LEFT JOIN seasons s ON t.tour_id = s.tour_id
            LEFT JOIN putevki p ON s.season_id = p.season_id
            GROUP BY t.tour_id, t.tour_name
            ORDER BY ����� DESC";

                DataTable dt = new DataTable();
                new NpgsqlDataAdapter(sql, con).Fill(dt);

                // ������� ���������� ������
                chartBar.Series.Clear();
                chartBar.Legends.Clear();

                // ������� ����� �����
                Series series = new Series("����");
                series.ChartType = SeriesChartType.Column;
                series.IsValueShownAsLabel = true;
                series.LabelFormat = "N0";
                series["PointWidth"] = "0.6"; // ������ ��������
                series.XValueType = ChartValueType.String;

                // ��������� ������
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string tourName = dt.Rows[i]["���"].ToString();
                    decimal amount = Convert.ToDecimal(dt.Rows[i]["�����"]);

                    // ��������� ����� ������ (�������) ��� ������� ����
                    DataPoint point = new DataPoint();
                    point.SetValueXY(tourName, amount);
                    point.Label = amount.ToString("N0");
                    point.LegendText = tourName;
                    series.Points.Add(point);
                }

                // ��������� ����� �� ���������
                chartBar.Series.Add(series);

                // ����������� ��� � ������� ���
                chartBar.ChartAreas[0].AxisX.Title = "����";
                chartBar.ChartAreas[0].AxisY.Title = "����� ������";
                chartBar.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                chartBar.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
                chartBar.ChartAreas[0].AxisX.Interval = 1;

                // ������ ����� ��� ������� �������
                string[] colors = { "#4E79A7", "#F28E2B", "#E15759", "#76B7B2", "#59A14F" };
                for (int i = 0; i < series.Points.Count; i++)
                {
                    series.Points[i].Color = ColorTranslator.FromHtml(colors[i % colors.Length]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ���������� ���������� ���������: {ex.Message}", "������",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/
        private void InitializeQueryComboBoxes()
        {
           
            cmbAggregateQuery.Items.AddRange(new object[]
            {
                "���������� ��������|SELECT COUNT(*) FROM tourists",
                "������� ���� �������|SELECT AVG(price) FROM putevki",
                "����� ����� ���� �������|SELECT SUM(price) FROM putevki",
                "����������� ���� �������|SELECT MIN(price) FROM putevki",
                "������������ ���� �������|SELECT MAX(price) FROM putevki"
            });
            cmbAggregateQuery.SelectedIndex = 0;
            cmbAggregateQuery.DisplayMember = "Text";
            cmbAggregateQuery.ValueMember = "Value";

            cmbParametricQuery.Items.AddRange(new object[]
{
    "������� � �������� �� '�����%'|SELECT * FROM tourists WHERE tourist_surname LIKE '�����%'",
    "������� ������ 50000|SELECT * FROM putevki WHERE price > 50000::money",
    "������ ����� 2023 ����|SELECT * FROM seasons WHERE start_date > '2023-01-01'",
    "������� �� 10000 �� 50000|SELECT * FROM payment WHERE amount BETWEEN 10000 AND 50000"
});
            cmbParametricQuery.SelectedIndex = 0;
            cmbParametricQuery.DisplayMember = "Text";
            cmbParametricQuery.ValueMember = "Value";
        }
        private void dataGridViewTourists_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView3.SelectedRows.Count > 0)
            {
                selectedRow = dataGridView3.SelectedRows[0];

                if (selectedRow.Cells["tourist_id"].Value != null)
                {
                    Console.WriteLine($"������� ������ � ID: {selectedRow.Cells["tourist_id"].Value}");
                }
            }
        }

        private void loadTouristsCombined()
        {
            try
            {
                DataTable dt = new DataTable();
                string sql = @"SELECT 
          tourist_id, 
          tourist_surname, 
          tourist_name, 
          tourist_otch,
          passport,
          city,
          country,
          phone
          FROM tourists";

                new NpgsqlDataAdapter(sql, con).Fill(dt);

                dt.Columns["tourist_surname"].Caption = "�������";
                dt.Columns["tourist_name"].Caption = "���";
                dt.Columns["tourist_otch"].Caption = "��������";
                dt.Columns["passport"].Caption = "�������";
                dt.Columns["city"].Caption = "�����";
                dt.Columns["country"].Caption = "������";
                dt.Columns["phone"].Caption = "�������";

                dataGridView3.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ �������� ������: {ex.Message}", "������",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loadSeasons()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT 
              s.season_id,
              t.tour_name,  
              s.start_date,
              s.end_date,
              s.closed,
              s.amount,
              (SELECT COUNT(*) FROM putevki WHERE season_id = s.season_id) as sold_count
              FROM seasons s
              JOIN tours t ON s.tour_id = t.tour_id";
            new NpgsqlDataAdapter(sql, con).Fill(dt);

            dataGridView4.DataSource = dt;
        }
        private void loadPutevki()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT 
                  p.putevki_id,
                  t.tourist_surname || ' ' || t.tourist_name AS tourist,
                  tr.tour_name,
                  p.price,  
                  s.start_date || ' - ' || s.end_date AS period
                  FROM putevki p
                  JOIN tourists t ON p.tourist_id = t.tourist_id
                  JOIN seasons s ON p.season_id = s.season_id
                  JOIN tours tr ON s.tour_id = tr.tour_id";
            new NpgsqlDataAdapter(sql, con).Fill(dt);

            dataGridView5.DataSource = dt;
            dataGridView5.Columns["price"].ReadOnly = false; // ��������� ��������������
        }
        private void loadPayment()
        {
            DataTable dt = new DataTable();
            NpgsqlDataAdapter adap = new NpgsqlDataAdapter("SELECT * FROM payment", con);
            adap.Fill(dt);
            dataGridView6.DataSource = dt;
        }

        private DataGridView GetCurrentDataGridView(string activeTab)
        {
            switch (activeTab)
            {
                case "tabPage2": return dataGridView3; // ������ ��� �������
                case "tabPage3": return dataGridView4; // ������
                case "tabPage4": return dataGridView5; // �������
                case "tabPage5": return dataGridView6; // ������
                default: return null;
            }
        }

        private void RefreshCurrentTab()
        {
            string activeTab = tabControl1.SelectedTab?.Name;
            if (activeTab == null) return;

            switch (activeTab)
            {
                case "tabPage2": loadTouristsCombined(); break;
                case "tabPage3": loadSeasons(); break;
                case "tabPage4": loadPutevki(); break;
                case "tabPage5": loadPayment(); break;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string activeTab = tabControl1.SelectedTab?.Name;
                if (activeTab == null) return;

                var form = new UniversalEditForm(activeTab, null, con);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    RefreshCurrentTab();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� �������� ����� ����������: {ex.Message}", "������",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string activeTab = tabControl1.SelectedTab?.Name;
                if (activeTab == null) return;

                DataGridView currentGrid = GetCurrentDataGridView(activeTab);

                if (currentGrid == null || currentGrid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("�������� ������ ��� ��������������!", "������",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedRow = currentGrid.SelectedRows[0];
                if (selectedRow == null || selectedRow.IsNewRow)
                {
                    MessageBox.Show("�������� ���������� ������ ��� ��������������!", "������",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var form = new UniversalEditForm(activeTab, selectedRow, con);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    RefreshCurrentTab();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� �������� ����� ��������������: {ex.Message}", "������",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string activeTab = tabControl1.SelectedTab?.Name;
                if (activeTab == null) return;

                DataGridView currentGrid = null;
                string idColumnName = "";
                string tableName = "";
                string errorMessage = "������ ������� ������, ��� ��� ���������� ��������� ������!\n" +
                                     "������� ������� ��������� ������.";

                switch (activeTab)
                {
                    case "tabPage2": // �������
                        currentGrid = dataGridView3;
                        idColumnName = "tourist_id";
                        tableName = "tourists";
                        // ���������� ���������, ��� ��� ������ ���� ��������� ��������
                        errorMessage = "������ ��� �������� �������";
                        break;
                    case "tabPage3": // ������
                        currentGrid = dataGridView4;
                        idColumnName = "season_id";
                        tableName = "seasons";
                        errorMessage = "������ ������� �����, ��� �������� ���������� �������!\n" +
                                      "������� ������� ��������� �������.";
                        break;
                    case "tabPage4": // �������
                        currentGrid = dataGridView5;
                        idColumnName = "putevki_id";
                        tableName = "putevki";
                        errorMessage = "������ ������� �������, ��� ������� ���������� �������!\n" +
                                      "������� ������� ��������� �������.";
                        break;
                    case "tabPage5": // ������
                        currentGrid = dataGridView6;
                        idColumnName = "payment_id";
                        tableName = "payment";
                        break;
                    default:
                        MessageBox.Show("����������� �������!", "������",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }

                if (currentGrid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("�������� ������ ��� ��������!", "������",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedRow = currentGrid.SelectedRows[0];
                if (selectedRow.Cells[idColumnName].Value == null)
                {
                    MessageBox.Show("�� ������� ���������� ID ��� ��������!", "������",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("�� �������, ��� ������ ������� ��� ������?", "������������� ��������",
                                           MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;

                int idToDelete = Convert.ToInt32(selectedRow.Cells[idColumnName].Value);

                // ��� �������� ���������� ������� ��������, ��� ��� ��������� ��������� � ��
                string sql = $@"DELETE FROM {tableName} WHERE {idColumnName} = @id;";

                using (var transaction = con.BeginTransaction())
                using (var cmd = new NpgsqlCommand(sql, con, transaction))
                {
                    cmd.Parameters.AddWithValue("id", idToDelete);
                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        transaction.Commit();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("������ ������� �������!", "�����",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("�� ������� ������� ������!", "������",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        switch (activeTab)
                        {
                            case "tabPage2": loadTouristsCombined(); break;
                            case "tabPage3": loadSeasons(); break;
                            case "tabPage4": loadPutevki(); break;
                            case "tabPage5": loadPayment(); break;
                        }
                    }
                    catch (Npgsql.PostgresException ex) when (ex.SqlState == "23503")
                    {
                        transaction.Rollback();
                        MessageBox.Show(errorMessage, "������",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"������ ��� ��������: {ex.Message}", "������",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ��������: {ex.Message}", "������",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (cmbAggregateQuery.SelectedItem == null)
            {
                MessageBox.Show("�������� �������������� ������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // �������� SQL �� ���������� �������� (����� |)
            string selectedItem = cmbAggregateQuery.SelectedItem.ToString();
            string query = selectedItem.Split('|')[1];

            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                {
                    object result = cmd.ExecuteScalar();
                    MessageBox.Show($"���������: {result}", "�������������� ������",
                                 MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ���������� �������: {ex.Message}", "������",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (cmbParametricQuery.SelectedItem == null)
            {
                MessageBox.Show("�������� ��������������� ������!", "������",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedItem = cmbParametricQuery.SelectedItem.ToString();
            string query = selectedItem.Split('|')[1];

            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridViewResult.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ���������� �������: {ex.Message}", "������",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                // ���������, ���� �� ������ � dataGridView
                if (dataGridViewResult.Rows.Count == 0)
                {
                    MessageBox.Show("��� ������ ��� ��������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ������� ������ ������ �����
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel ����� (*.xlsx)|*.xlsx",
                    Title = "��������� �����",
                    FileName = "�����.xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    // ������� ����� ����� Excel
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("�����");

                        // ��������� ��������
                        for (int i = 0; i < dataGridViewResult.Columns.Count; i++)
                        {
                            worksheet.Cell(1, i + 1).Value = dataGridViewResult.Columns[i].HeaderText;
                            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                        }

                        // ������ �� DataGridView
                        for (int i = 0; i < dataGridViewResult.Rows.Count; i++)
                        {
                            for (int j = 0; j < dataGridViewResult.Columns.Count; j++)
                            {
                                worksheet.Cell(i + 2, j + 1).Value = dataGridViewResult.Rows[i].Cells[j].Value?.ToString() ?? "";
                            }
                        }

                        // ���������� �������
                        worksheet.Columns().AdjustToContents();

                        // ���������� �����
                        workbook.SaveAs(filePath);
                    }

                    MessageBox.Show($"���� ������� ��������: {filePath}", "�����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ��������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                // ������ ������ �����
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel ����� (*.xlsx)|*.xlsx",
                    Title = "�������� ���� ��� �������"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    // ��������� ���� Excel
                    using (var workbook = new XLWorkbook(filePath))
                    {
                        var worksheet = workbook.Worksheet(1); // ����� ������ ����
                        var range = worksheet.RangeUsed(); // �������� ������������ ��������

                        if (range == null)
                        {
                            MessageBox.Show("���� ���� ��� �� �������� ������!", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        DataTable dt = new DataTable();

                        // ������ ��������� (������ ������)
                        foreach (var cell in range.FirstRow().CellsUsed())
                        {
                            dt.Columns.Add(cell.Value.ToString().Trim());
                        }

                        // ������ ������ (�� ������)
                        foreach (var row in range.RowsUsed().Skip(1))
                        {
                            DataRow dataRow = dt.NewRow();
                            int columnIndex = 0;

                            foreach (var cell in row.CellsUsed())
                            {
                                dataRow[columnIndex] = cell.Value.ToString().Trim();
                                columnIndex++;
                            }

                            dt.Rows.Add(dataRow);
                        }

                        // ��������� ������ � DataGridView
                        dataGridViewResult.DataSource = dt;
                    }

                    MessageBox.Show("������ ������� �������������!", "�����", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� �������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                using (var transaction = con.BeginTransaction())
                {
                   
                    var cmdInsertTourist = new NpgsqlCommand(
                        @"INSERT INTO tourists 
                  (tourist_surname, tourist_name, passport, city, country, phone)
                  VALUES 
                  ('��������', '������', '1234567890', '������', '������', '+79990000000')
                  RETURNING tourist_id",
                        con, transaction);

                    int newTouristId = Convert.ToInt32(cmdInsertTourist.ExecuteScalar());

                    var cmdFindCheapestTour = new NpgsqlCommand(
    @"SELECT s.season_id 
      FROM seasons s
      JOIN tours t ON s.tour_id = t.tour_id
      ORDER BY s.amount
      LIMIT 1",
    con, transaction);

                    object cheapestSeasonId = cmdFindCheapestTour.ExecuteScalar();

                    if (cheapestSeasonId == null)
                    {
                        throw new Exception("�� ������� ��������� ����");
                    }

                    var cmdCreatePutevka = new NpgsqlCommand(
                        @"INSERT INTO putevki (tourist_id, season_id)
                  VALUES (@touristId, @seasonId)",
                        con, transaction);

                    cmdCreatePutevka.Parameters.AddWithValue("@touristId", newTouristId);
                    cmdCreatePutevka.Parameters.AddWithValue("@seasonId", cheapestSeasonId);
                    cmdCreatePutevka.ExecuteNonQuery();

                    transaction.Commit();

                    MessageBox.Show($"������� ������ ������ � ID: {newTouristId} � �������� ���");
                }

                tabControl1.SelectedTab = tabPage4;
                loadPutevki();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������: {ex.Message}");
            }
        }


    }
}

