using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace tourfirma
{
    public partial class Form3 : Form
    {
        private NpgsqlConnection con;
        private string connString = "Host=127.0.0.1;Username=postgres;Password=123;Database=TourFirma";
        private int touristId;

        public Form3(int id, string name, string surname, string otch)
        {
            InitializeComponent();
            con = new NpgsqlConnection(connString);
            con.Open();

            touristId = id;
            textBox1.Text = name;
            textBox2.Text = surname;
            textBox3.Text = otch;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string sql = "UPDATE tourists SET tourist_name = @name, tourist_surname = @surname, tourist_otch = @otch WHERE tourist_id = @id";
            NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("id", touristId);
            cmd.Parameters.AddWithValue("name", textBox1.Text);
            cmd.Parameters.AddWithValue("surname", textBox2.Text);
            cmd.Parameters.AddWithValue("otch", textBox3.Text);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
