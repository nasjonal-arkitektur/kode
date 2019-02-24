using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsAppFellesløsninger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString;
            SqlConnection cnn;

            connectionString = @"Server=O-EHA-X260-3\SQLEXPRESS;Integrated Security=SSPI;" + "Initial Catalog=fellesløsninger";

            cnn = new SqlConnection(connectionString);
            cnn.Open();
            MessageBox.Show("Connection Open!");

            HasRows(cnn);

            cnn.Close();

        }

        static void HasRows(SqlConnection connection)
        {
            using (connection)
            {
                SqlCommand command = new SqlCommand(
                  "SELECT navn, kort_beskrivelse FROM løsning;",
                  connection);
                //connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("{0}\t{1}", reader.GetString(0),
                            reader.GetString(1));
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();
            }
        }
    }
}
