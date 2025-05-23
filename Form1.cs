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
            if (tabControl1.SelectedTab?.Name == "tabPage1") // Замените на имя вашей вкладки с диаграммами
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
                IsValueShownAsLabel = false, // Отключаем отображение значений на столбцах
                ["PointWidth"] = "0.6",
                ToolTip = $"{name}: #VALY" // Подсказка при наведении
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
        'Выкупленные' as category,
        (sold_seats * 100.0 / total_seats) as percentage
    FROM total_putevki
    UNION ALL
    SELECT 
        'Не выкупленные' as category,
        ((total_seats - sold_seats) * 100.0 / total_seats) as percentage
    FROM total_putevki";

                DataTable dt = new DataTable();
                new NpgsqlDataAdapter(sql, con).Fill(dt);

                Series series = new Series("Туры");
                series.ChartType = SeriesChartType.Pie;
                series.IsValueShownAsLabel = false; // Убрать проценты с секторов
                series.Font = new Font("Arial", 10);

                foreach (DataRow row in dt.Rows)
                {
                    double percentage = Math.Round(Convert.ToDouble(row["percentage"]), 1);
                    DataPoint point = series.Points.Add(percentage);
                    point.LegendText = $"{row["category"]} ({percentage}%)"; // Проценты только в легенде
                }

                chartPie.Series.Add(series);
                chartPie.Legends.Add(new Legend("Legend"));
                chartPie.Legends["Legend"].Docking = Docking.Bottom;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка в круговой диаграмме: {ex.Message}");
            }
        }
        private void InitializeCharts()
        {
            // Инициализация круговой диаграммы (процент выкупа туров)
            chartPie.Series.Clear();
            chartPie.Titles.Clear();
            chartPie.Titles.Add("Процент выкупа туров");
            chartPie.Legends.Clear();
            chartPie.Legends.Add(new Legend("Legend1"));
            chartPie.Legends["Legend1"].Docking = Docking.Bottom;

            // Инициализация столбчатой диаграммы (с 3 сериями)
            chartBar.Series.Clear();
            chartBar.Titles.Clear();
            chartBar.Titles.Add("Статистика по турам");
            chartBar.ChartAreas[0].AxisX.Title = "Туры";
            chartBar.ChartAreas[0].AxisY.Title = "Значение";
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

                // Создаем только две серии
                var series1 = CreateSeries("Сумма", Color.SteelBlue, "total_sum");
                var series2 = CreateSeries("Ср. цена", Color.MediumSeaGreen, "avg_price");

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
                MessageBox.Show($"Ошибка в столбчатой диаграмме: {ex.Message}");
            }
            chartBar.ChartAreas[0].AxisX.LabelStyle.Enabled = false;
        }

        // Вспомогательный метод для заполнения серии данными
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
                // Запрос для получения суммы выкупа по каждому туру
                string sql = @"
            SELECT 
                t.tour_name as Тур,
                COALESCE(SUM(p.price::numeric), 0.0) as Сумма
            FROM tours t
            LEFT JOIN seasons s ON t.tour_id = s.tour_id
            LEFT JOIN putevki p ON s.season_id = p.season_id
            GROUP BY t.tour_id, t.tour_name
            ORDER BY Сумма DESC";

                DataTable dt = new DataTable();
                new NpgsqlDataAdapter(sql, con).Fill(dt);

                // Очищаем предыдущие данные
                chartBar.Series.Clear();
                chartBar.Legends.Clear();

                // Создаем новую серию
                Series series = new Series("Туры");
                series.ChartType = SeriesChartType.Column;
                series.IsValueShownAsLabel = true;
                series.LabelFormat = "N0";
                series["PointWidth"] = "0.6"; // Ширина столбцов
                series.XValueType = ChartValueType.String;

                // Добавляем данные
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string tourName = dt.Rows[i]["Тур"].ToString();
                    decimal amount = Convert.ToDecimal(dt.Rows[i]["Сумма"]);

                    // Добавляем точку данных (столбец) для каждого тура
                    DataPoint point = new DataPoint();
                    point.SetValueXY(tourName, amount);
                    point.Label = amount.ToString("N0");
                    point.LegendText = tourName;
                    series.Points.Add(point);
                }

                // Добавляем серию на диаграмму
                chartBar.Series.Add(series);

                // Настраиваем оси и внешний вид
                chartBar.ChartAreas[0].AxisX.Title = "Туры";
                chartBar.ChartAreas[0].AxisY.Title = "Сумма выкупа";
                chartBar.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                chartBar.ChartAreas[0].AxisY.LabelStyle.Format = "N0";
                chartBar.ChartAreas[0].AxisX.Interval = 1;

                // Разные цвета для каждого столбца
                string[] colors = { "#4E79A7", "#F28E2B", "#E15759", "#76B7B2", "#59A14F" };
                for (int i = 0; i < series.Points.Count; i++)
                {
                    series.Points[i].Color = ColorTranslator.FromHtml(colors[i % colors.Length]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении столбчатой диаграммы: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/
        private void InitializeQueryComboBoxes()
        {
           
            cmbAggregateQuery.Items.AddRange(new object[]
            {
                "Количество туристов|SELECT COUNT(*) FROM tourists",
                "Средняя цена путевки|SELECT AVG(price) FROM putevki",
                "Общая сумма всех путевок|SELECT SUM(price) FROM putevki",
                "Минимальная цена путевки|SELECT MIN(price) FROM putevki",
                "Максимальная цена путевки|SELECT MAX(price) FROM putevki"
            });
            cmbAggregateQuery.SelectedIndex = 0;
            cmbAggregateQuery.DisplayMember = "Text";
            cmbAggregateQuery.ValueMember = "Value";

            cmbParametricQuery.Items.AddRange(new object[]
{
    "Туристы с фамилией на 'Ивано%'|SELECT * FROM tourists WHERE tourist_surname LIKE 'Ивано%'",
    "Путевки дороже 50000|SELECT * FROM putevki WHERE price > 50000::money",
    "Сезоны после 2023 года|SELECT * FROM seasons WHERE start_date > '2023-01-01'",
    "Платежи от 10000 до 50000|SELECT * FROM payment WHERE amount BETWEEN 10000 AND 50000"
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
                    Console.WriteLine($"Выбрана строка с ID: {selectedRow.Cells["tourist_id"].Value}");
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

                dt.Columns["tourist_surname"].Caption = "Фамилия";
                dt.Columns["tourist_name"].Caption = "Имя";
                dt.Columns["tourist_otch"].Caption = "Отчество";
                dt.Columns["passport"].Caption = "Паспорт";
                dt.Columns["city"].Caption = "Город";
                dt.Columns["country"].Caption = "Страна";
                dt.Columns["phone"].Caption = "Телефон";

                dataGridView3.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
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
            dataGridView5.Columns["price"].ReadOnly = false; // Разрешаем редактирование
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
                case "tabPage2": return dataGridView3; // Теперь это туристы
                case "tabPage3": return dataGridView4; // Сезоны
                case "tabPage4": return dataGridView5; // Путевки
                case "tabPage5": return dataGridView6; // Оплата
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
                MessageBox.Show($"Ошибка при открытии формы добавления: {ex.Message}", "Ошибка",
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
                    MessageBox.Show("Выберите строку для редактирования!", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedRow = currentGrid.SelectedRows[0];
                if (selectedRow == null || selectedRow.IsNewRow)
                {
                    MessageBox.Show("Выберите корректную строку для редактирования!", "Ошибка",
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
                MessageBox.Show($"Ошибка при открытии формы редактирования: {ex.Message}", "Ошибка",
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
                string errorMessage = "Нельзя удалить запись, так как существуют связанные данные!\n" +
                                     "Сначала удалите связанные записи.";

                switch (activeTab)
                {
                    case "tabPage2": // Туристы
                        currentGrid = dataGridView3;
                        idColumnName = "tourist_id";
                        tableName = "tourists";
                        // Измененное сообщение, так как теперь есть каскадное удаление
                        errorMessage = "Ошибка при удалении туриста";
                        break;
                    case "tabPage3": // Сезоны
                        currentGrid = dataGridView4;
                        idColumnName = "season_id";
                        tableName = "seasons";
                        errorMessage = "Нельзя удалить сезон, для которого существуют путевки!\n" +
                                      "Сначала удалите связанные путевки.";
                        break;
                    case "tabPage4": // Путевки
                        currentGrid = dataGridView5;
                        idColumnName = "putevki_id";
                        tableName = "putevki";
                        errorMessage = "Нельзя удалить путевку, для которой существуют платежи!\n" +
                                      "Сначала удалите связанные платежи.";
                        break;
                    case "tabPage5": // Оплата
                        currentGrid = dataGridView6;
                        idColumnName = "payment_id";
                        tableName = "payment";
                        break;
                    default:
                        MessageBox.Show("Неизвестная вкладка!", "Ошибка",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                }

                if (currentGrid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Выберите строку для удаления!", "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var selectedRow = currentGrid.SelectedRows[0];
                if (selectedRow.Cells[idColumnName].Value == null)
                {
                    MessageBox.Show("Не удалось определить ID для удаления!", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var result = MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "Подтверждение удаления",
                                           MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes) return;

                int idToDelete = Convert.ToInt32(selectedRow.Cells[idColumnName].Value);

                // Для туристов используем простое удаление, так как каскадное настроено в БД
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
                            MessageBox.Show("Запись успешно удалена!", "Успех",
                                          MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить запись!", "Ошибка",
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
                        MessageBox.Show(errorMessage, "Ошибка",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (cmbAggregateQuery.SelectedItem == null)
            {
                MessageBox.Show("Выберите агрегированный запрос!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Получаем SQL из выбранного элемента (после |)
            string selectedItem = cmbAggregateQuery.SelectedItem.ToString();
            string query = selectedItem.Split('|')[1];

            try
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand(query, con))
                {
                    object result = cmd.ExecuteScalar();
                    MessageBox.Show($"Результат: {result}", "Агрегированный запрос",
                                 MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения запроса: {ex.Message}", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (cmbParametricQuery.SelectedItem == null)
            {
                MessageBox.Show("Выберите параметрический запрос!", "Ошибка",
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
                MessageBox.Show($"Ошибка выполнения запроса: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, есть ли данные в dataGridView
                if (dataGridViewResult.Rows.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Создаем диалог выбора файла
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel файлы (*.xlsx)|*.xlsx",
                    Title = "Сохранить отчет",
                    FileName = "Отчет.xlsx"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    // Создаем новую книгу Excel
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Отчет");

                        // Заголовки столбцов
                        for (int i = 0; i < dataGridViewResult.Columns.Count; i++)
                        {
                            worksheet.Cell(1, i + 1).Value = dataGridViewResult.Columns[i].HeaderText;
                            worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                        }

                        // Данные из DataGridView
                        for (int i = 0; i < dataGridViewResult.Rows.Count; i++)
                        {
                            for (int j = 0; j < dataGridViewResult.Columns.Count; j++)
                            {
                                worksheet.Cell(i + 2, j + 1).Value = dataGridViewResult.Rows[i].Cells[j].Value?.ToString() ?? "";
                            }
                        }

                        // Автоширина колонок
                        worksheet.Columns().AdjustToContents();

                        // Сохранение файла
                        workbook.SaveAs(filePath);
                    }

                    MessageBox.Show($"Файл успешно сохранен: {filePath}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                // Диалог выбора файла
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel файлы (*.xlsx)|*.xlsx",
                    Title = "Выберите файл для импорта"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    // Открываем файл Excel
                    using (var workbook = new XLWorkbook(filePath))
                    {
                        var worksheet = workbook.Worksheet(1); // Берем первый лист
                        var range = worksheet.RangeUsed(); // Получаем используемый диапазон

                        if (range == null)
                        {
                            MessageBox.Show("Файл пуст или не содержит данных!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        DataTable dt = new DataTable();

                        // Читаем заголовки (первую строку)
                        foreach (var cell in range.FirstRow().CellsUsed())
                        {
                            dt.Columns.Add(cell.Value.ToString().Trim());
                        }

                        // Читаем строки (со второй)
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

                        // Загружаем данные в DataGridView
                        dataGridViewResult.DataSource = dt;
                    }

                    MessageBox.Show("Данные успешно импортированы!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при импорте: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                  ('Тестовый', 'Турист', '1234567890', 'Москва', 'Россия', '+79990000000')
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
                        throw new Exception("Не найдены доступные туры");
                    }

                    var cmdCreatePutevka = new NpgsqlCommand(
                        @"INSERT INTO putevki (tourist_id, season_id)
                  VALUES (@touristId, @seasonId)",
                        con, transaction);

                    cmdCreatePutevka.Parameters.AddWithValue("@touristId", newTouristId);
                    cmdCreatePutevka.Parameters.AddWithValue("@seasonId", cheapestSeasonId);
                    cmdCreatePutevka.ExecuteNonQuery();

                    transaction.Commit();

                    MessageBox.Show($"Успешно создан турист с ID: {newTouristId} и назначен тур");
                }

                tabControl1.SelectedTab = tabPage4;
                loadPutevki();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }


    }
}

