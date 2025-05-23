using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Npgsql;

namespace tourfirma
{
    public partial class UniversalEditForm : Form
    {
        private readonly string _activeTab;
        private readonly DataGridViewRow _selectedRow;
        private readonly NpgsqlConnection _connection;
        private int _currentTop = 20;
        private const int ControlWidth = 250;
        private const int LabelWidth = 120;

        public UniversalEditForm(string activeTab, DataGridViewRow selectedRow, NpgsqlConnection connection)
        {
            InitializeComponent();
            _activeTab = activeTab;
            _selectedRow = selectedRow;
            _connection = connection;
            ConfigureForm();
        }

        private void ConfigureForm()
        {
            this.Text = _selectedRow == null ? "Добавить запись" : "Редактировать запись";
            this.ClientSize = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            switch (_activeTab)
            {
                case "tabPage2":
                    ConfigureTouristTab();
                    break;

                case "tabPage3": // Сезоны
                    ConfigureSeasonsTab();
                    break;

                case "tabPage4": // Путевки
                    ConfigurePutevkiTab();
                    break;

                case "tabPage5": // Оплата
                    ConfigurePaymentTab();
                    break;
            }

            AddSaveButton();

            if (_selectedRow != null)
            {
                LoadDataFromRow();
            }
        }

        private void ConfigureTouristTab()
        {
            AddTextField("Фамилия*", "tourist_surname");
            AddTextField("Имя*", "tourist_name");  
            AddTextField("Отчество", "tourist_otch");  
            AddTextField("Паспорт*", "passport", 10);
            AddTextField("Город*", "city");
            AddTextField("Страна*", "country");
            AddTextField("Телефон*", "phone", 13);
            AddRequiredFieldsHint();
        }

        private void ConfigureSeasonsTab()
        {
            AddComboBoxField("Тур*", "tour_id", "SELECT tour_id, tour_name FROM tours");
            AddNumericField("Цена*", "price", decimal.MaxValue);  // Добавляем поле цены
            AddDateField("Дата начала*", "start_date");
            AddDateField("Дата окончания*", "end_date");
            AddCheckBoxField("Закрыт", "closed");
            AddNumericField("Количество*", "amount");
            AddRequiredFieldsHint();
        }

        private void ConfigurePutevkiTab()
        {
            AddComboBoxField("Турист*", "tourist_id",
                "SELECT t.tourist_id, t.tourist_surname || ' ' || t.tourist_name || ' (паспорт: ' || t.passport || ')' AS display_info " +
                "FROM tourists t " +
                "ORDER BY t.tourist_surname");

            AddComboBoxField("Сезон*", "season_id",
                "SELECT s.season_id, t.tour_name || ' (' || s.start_date || ' - ' || s.end_date || ')' AS display_info " +
                "FROM seasons s " +
                "JOIN tours t ON s.tour_id = t.tour_id");

            // Добавляем поле для цены путевки
            AddNumericField("Цена путевки*", "price", decimal.MaxValue);

            AddRequiredFieldsHint();
            AddAddNewTouristButton();
        }

        private void ConfigurePaymentTab()
        {
            AddComboBoxField("Путевка*", "putevki_id",
                "SELECT p.putevki_id, t.tourist_surname || ' ' || t.tourist_name || ' (' || s.start_date || ')' AS display_info " +
                "FROM putevki p " +
                "JOIN tourists t ON p.tourist_id = t.tourist_id " +
                "JOIN seasons s ON p.season_id = s.season_id");

            AddDateField("Дата оплаты*", "payment_date", DateTime.Today);
            AddNumericField("Сумма*", "summa", decimal.MaxValue);
            AddRequiredFieldsHint();
        }

        private void AddTextField(string labelText, string fieldName, int maxLength = 0)
        {
            var label = new Label
            {
                Text = labelText,
                Top = _currentTop,
                Left = 10,
                Width = LabelWidth,
                TextAlign = ContentAlignment.MiddleRight
            };

            var textBox = new TextBox
            {
                Name = fieldName,
                Tag = fieldName,
                Top = _currentTop,
                Left = LabelWidth + 20,
                Width = ControlWidth,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            if (maxLength > 0) textBox.MaxLength = maxLength;

            this.Controls.Add(label);
            this.Controls.Add(textBox);
            _currentTop += 30;
        }

        private void AddComboBoxField(string labelText, string fieldName, string query)
        {
            var label = new Label
            {
                Text = labelText,
                Top = _currentTop,
                Left = 10,
                Width = LabelWidth,
                TextAlign = ContentAlignment.MiddleRight
            };

            var comboBox = new ComboBox
            {
                Name = fieldName,
                Tag = fieldName,
                Top = _currentTop,
                Left = LabelWidth + 20,
                Width = ControlWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            try
            {
                var dt = new DataTable();
                new NpgsqlDataAdapter(query, _connection).Fill(dt);
                comboBox.DisplayMember = dt.Columns.Count > 1 ? dt.Columns[1].ColumnName : dt.Columns[0].ColumnName;
                comboBox.ValueMember = dt.Columns[0].ColumnName;
                comboBox.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }

            this.Controls.Add(label);
            this.Controls.Add(comboBox);
            _currentTop += 30;
        }

        private void AddDateField(string labelText, string fieldName, DateTime? defaultDate = null)
        {
            var label = new Label
            {
                Text = labelText,
                Top = _currentTop,
                Left = 10,
                Width = LabelWidth,
                TextAlign = ContentAlignment.MiddleRight
            };

            var datePicker = new DateTimePicker
            {
                Name = fieldName,
                Tag = fieldName,
                Top = _currentTop,
                Left = LabelWidth + 20,
                Width = ControlWidth,
                Format = DateTimePickerFormat.Short,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            if (defaultDate.HasValue) datePicker.Value = defaultDate.Value;

            this.Controls.Add(label);
            this.Controls.Add(datePicker);
            _currentTop += 30;
        }

        private void AddCheckBoxField(string labelText, string fieldName)
        {
            var checkBox = new CheckBox
            {
                Text = labelText,
                Name = fieldName,
                Tag = fieldName,
                Top = _currentTop,
                Left = LabelWidth + 20,
                Width = ControlWidth,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            this.Controls.Add(checkBox);
            _currentTop += 30;
        }

        private void AddNumericField(string labelText, string fieldName, decimal maxValue = 1000000)
        {
            var label = new Label
            {
                Text = labelText,
                Top = _currentTop,
                Left = 10,
                Width = LabelWidth,
                TextAlign = ContentAlignment.MiddleRight
            };

            var numericUpDown = new NumericUpDown
            {
                Name = fieldName,
                Tag = fieldName,
                Top = _currentTop,
                Left = LabelWidth + 20,
                Width = ControlWidth,
                Maximum = maxValue,
                DecimalPlaces = 2,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            this.Controls.Add(label);
            this.Controls.Add(numericUpDown);
            _currentTop += 30;
        }

        private void AddRequiredFieldsHint()
        {
            var hint = new Label
            {
                Text = "* - обязательные поля",
                Top = _currentTop,
                Left = 10,
                ForeColor = Color.Gray,
                Font = new Font(this.Font, FontStyle.Italic)
            };
            this.Controls.Add(hint);
            _currentTop += 25;
        }

        private void AddAddNewTouristButton()
        {
            var btn = new Button
            {
                Text = "Добавить нового туриста",
                Top = _currentTop,
                Left = LabelWidth + 20,
                Width = ControlWidth,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            btn.Click += (s, e) => {
                var touristForm = new UniversalEditForm("tabPage2", null, _connection);
                if (touristForm.ShowDialog() == DialogResult.OK)
                {
                    var combo = this.Controls.Find("tourist_id", true).FirstOrDefault() as ComboBox;
                    if (combo != null)
                    {
                        var dt = new DataTable();
                        new NpgsqlDataAdapter(
                            "SELECT tourist_id, tourist_surname || ' ' || tourist_name || ' (паспорт: ' || passport || ')' AS display_info " +
                            "FROM tourists ORDER BY tourist_id DESC LIMIT 1",
                            _connection).Fill(dt);
                        combo.DataSource = dt;
                        combo.SelectedValue = dt.Rows[0]["tourist_id"];
                    }
                }
            };

            this.Controls.Add(btn);
            _currentTop += 35;
        }

        private void AddSaveButton()
        {
            var btnSave = new Button
            {
                Text = "Сохранить",
                DialogResult = DialogResult.OK,
                Top = _currentTop + 10,
                Left = LabelWidth + 20,
                Width = ControlWidth / 2 - 5,
                Height = 35,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            btnSave.Click += btnSave_Click;
            this.Controls.Add(btnSave);

            var btnCancel = new Button
            {
                Text = "Отмена",
                DialogResult = DialogResult.Cancel,
                Top = _currentTop + 10,
                Left = LabelWidth + 20 + ControlWidth / 2 + 5,
                Width = ControlWidth / 2 - 5,
                Height = 35,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            this.Controls.Add(btnCancel);
        }

        private void LoadDataFromRow()
        {
            foreach (Control control in this.Controls)
            {
                if (control.Tag != null)
                {
                    // Проверяем существование столбца перед доступом
                    if (_selectedRow.DataGridView.Columns.Contains(control.Tag.ToString()))
                    {
                        var value = _selectedRow.Cells[control.Tag.ToString()].Value;

                        if (control is TextBox textBox)
                            textBox.Text = value?.ToString() ?? "";
                        else if (control is ComboBox comboBox)
                            comboBox.SelectedValue = value;
                        else if (control is DateTimePicker datePicker && value is DateTime)
                            datePicker.Value = (DateTime)value;
                        else if (control is CheckBox checkBox)
                            checkBox.Checked = Convert.ToBoolean(value);
                        else if (control is NumericUpDown numericUpDown)
                            numericUpDown.Value = Convert.ToDecimal(value);
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateBeforeSave()) return;

            try
            {
                using (var transaction = _connection.BeginTransaction())
                {
                    if (_selectedRow == null)
                    {
                        InsertData(transaction);
                    }
                    else
                    {
            
                        object id = _selectedRow.Cells[GetIdColumnName()].Value;

                    
                        if (_activeTab == "tabPage4" && this.Controls["price"] is NumericUpDown priceControl)
                        {
                            decimal newPrice = priceControl.Value;
                            decimal oldPrice = Convert.ToDecimal(_selectedRow.Cells["price"].Value);

                            if (newPrice != oldPrice)
                            {
                                var confirmResult = MessageBox.Show(
                                    "Вы уверены, что хотите изменить цену путевки?",
                                    "Подтверждение изменения цены",
                                    MessageBoxButtons.YesNo);

                                if (confirmResult != DialogResult.Yes) return;
                            }
                        }

                        UpdateData(transaction, id);
                    }
                    transaction.Commit();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "23503")
            {
                ShowForeignKeyError(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}\n\nДетали: {ex.InnerException?.Message}",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateBeforeSave()
        {
            foreach (Control control in this.Controls)
            {
                if (control is Label label && label.Text.EndsWith("*"))
                {
                    string fieldName = label.Text.Replace("*", "").Trim();
                    var inputControl = this.Controls
                        .OfType<Control>()
                        .FirstOrDefault(c => c.Tag?.ToString() == fieldName);

                    if (inputControl is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        MessageBox.Show($"Поле '{fieldName}' обязательно для заполнения!", "Ошибка",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                    else if (inputControl is ComboBox comboBox && comboBox.SelectedValue == null)
                    {
                        MessageBox.Show($"Необходимо выбрать значение для '{fieldName}'!", "Ошибка",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            return true;
        }

        private void ShowForeignKeyError(Npgsql.PostgresException ex)
        {
            string errorMessage = "Ошибка целостности данных:\n";

            if (ex.Message.Contains("putevki_tourist_id_fkey"))
            {
                errorMessage += "Выбранный турист не найден в системе.\n";
                errorMessage += "Пожалуйста, выберите существующего туриста.";
            }
            else if (ex.Message.Contains("putevki_season_id_fkey"))
            {
                errorMessage += "Выбранный сезон не найден в системе.\n";
                errorMessage += "Пожалуйста, выберите существующий сезон.";
            }
            else
            {
                errorMessage += ex.Message;
                if (ex.Detail != null) errorMessage += "\n\nДетали: " + ex.Detail;
            }

            MessageBox.Show(errorMessage, "Ошибка сохранения",
                          MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void InsertData(NpgsqlTransaction transaction)
        {
            string tableName = GetTableName();
            var cmd = new NpgsqlCommand(GetInsertSql(tableName), _connection, transaction);

            foreach (Control control in this.Controls)
            {
                if (control.Tag != null)
                {
                    cmd.Parameters.AddWithValue($"@{control.Tag}", GetControlValue(control, false));
                }
            }

            var result = cmd.ExecuteScalar();
            MessageBox.Show($"Запись успешно добавлена с ID: {result}", "Успех",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateData(NpgsqlTransaction transaction, object id)
        {
            string tableName = GetTableName();
            string idColumnName = GetIdColumnName();

            var cmd = new NpgsqlCommand(GetUpdateSql(tableName, idColumnName), _connection, transaction);

        
            foreach (Control control in this.Controls)
            {
                if (control.Tag != null)
                {
                    cmd.Parameters.AddWithValue($"@{control.Tag}", GetControlValue(control, false));
                }
            }

     
            cmd.Parameters.AddWithValue($"@{idColumnName}", id);

            int affected = cmd.ExecuteNonQuery();
            if (affected > 0)
            {
                MessageBox.Show("Запись успешно обновлена", "Успех",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Не удалось обновить запись. Возможно, она была удалена.", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string GetInsertSql(string tableName)
        {
            string columns = string.Join(", ", this.Controls
                .OfType<Control>()
                .Where(c => c.Tag != null)
                .Select(c => c.Tag.ToString()));

            string values = string.Join(", ", this.Controls
                .OfType<Control>()
                .Where(c => c.Tag != null)
                .Select(c => $"@{c.Tag}"));

            return $"INSERT INTO {tableName} ({columns}) VALUES ({values}) RETURNING {GetIdColumnName()}";
        }
        private string GetUpdateSql(string tableName, string idColumnName)
        {
            string setClause = string.Join(", ", this.Controls
                .OfType<Control>()
                .Where(c => c.Tag != null && c.Tag.ToString() != idColumnName)
                .Select(c => $"{c.Tag} = @{c.Tag}"));

            return $"UPDATE {tableName} SET {setClause} WHERE {idColumnName} = @{idColumnName}";
        }
        private string GetTableName()
        {
            return _activeTab switch
            {
                "tabPage2" => "tourists",
                "tabPage3" => "seasons",
                "tabPage4" => "putevki",
                "tabPage5" => "payment",
                _ => throw new Exception("Неизвестная таблица")
            };
        }

        private string GetIdColumnName()
        {
            return _activeTab switch
            {
                "tabPage2" => "tourist_id",
                "tabPage3" => "season_id",
                "tabPage4" => "putevki_id",
                "tabPage5" => "payment_id",
                _ => throw new Exception("Неизвестный ID столбец")
            };
        }

        private object GetControlValue(Control control, bool forSql)
        {
            if (control is TextBox textBox)
                return textBox.Text;
            if (control is ComboBox comboBox)
                return comboBox.SelectedValue ?? DBNull.Value;
            if (control is DateTimePicker datePicker)
                return datePicker.Value;
            if (control is CheckBox checkBox)
                return checkBox.Checked;
            if (control is NumericUpDown numericUpDown)
                return numericUpDown.Value;

            return DBNull.Value;
        }
    }
}