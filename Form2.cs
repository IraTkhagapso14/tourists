using System;
using System.Windows.Forms;
using Npgsql;

namespace tourfirma
{
    public partial class Form2 : Form
    {
        private NpgsqlConnection con;
        private string connString = "Host=127.0.0.1;Username=postgres;Password=123;Database=TourFirma";

        public Form2()
        {
            InitializeComponent();
            con = new NpgsqlConnection(connString);
            con.Open();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (NpgsqlTransaction transaction = con.BeginTransaction())
            {
                try
                {
                    // 1. Добавляем в tourists
                    string sqlTourist = @"
                INSERT INTO tourists (tourist_name, tourist_surname, tourist_otch) 
                VALUES (@name, @surname, @otch) 
                RETURNING tourist_id";

                    int newTouristId;
                    using (var cmd = new NpgsqlCommand(sqlTourist, con, transaction))
                    {
                        cmd.Parameters.AddWithValue("name", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("surname", textBox2.Text.Trim());
                        cmd.Parameters.AddWithValue("otch", textBox3.Text.Trim());
                        newTouristId = (int)cmd.ExecuteScalar();
                    }

                    // 2. Добавляем "заглушку" в tourist_info
                    string sqlInfo = @"
                INSERT INTO tourist_info (tourist_id, passport, city, country, phone)
                VALUES (@id, '0000000000', 'Не указано', 'Не указано', '+70000000000')";

                    using (var cmd = new NpgsqlCommand(sqlInfo, con, transaction))
                    {
                        cmd.Parameters.AddWithValue("id", newTouristId);
                        cmd.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Турист добавлен!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}