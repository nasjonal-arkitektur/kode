using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
//using System.Drawing;

namespace DesktopApp1
{
    class DB
    {

        static string m_connectionString = @"Server=O-EHA-X260-3\SQLEXPRESS;Integrated Security=SSPI;" + "Initial Catalog=fellesløsninger";
        SqlConnection m_cnn;

        public DB()
        {
            m_cnn = new SqlConnection(m_connectionString);
            m_cnn.Open();
            MessageBox.Show("Connection Open!");

            ReadLøsning();

            m_cnn.Close();

        }


        ~DB()
        {
            //m_cnn.Close();
        }

        public string ReadLøsning()
        {
            string result = null;

            using (m_cnn)
            {
                SqlCommand command = new SqlCommand(
                  "SELECT navn, kort_beskrivelse FROM løsning;", m_cnn);


                SqlDataReader reader = command.ExecuteReader();


                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        result += reader.GetString(0) + reader.GetString(1);
                        Console.WriteLine("{0}\t{1}", reader.GetString(0), reader.GetString(1));
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                reader.Close();

                return result;
            }


        }

        public void WriteLøsning()
        {


        }
    }
}

