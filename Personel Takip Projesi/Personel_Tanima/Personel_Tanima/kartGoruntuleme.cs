using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace Personel_Tanima
{
    public partial class kartGoruntuleme : Form
    {
        static string constring = ("Data Source=ENISE;Initial Catalog=veri;Integrated Security=True");
        SqlConnection connection = new SqlConnection(constring);
        //SqlConnection connection = new SqlConnection("Data Source=ENISE;Initial Catalog=veri;Integrated Security=True");

        public kartGoruntuleme()
        {
            InitializeComponent();
        }

        private bool IsRFIDNumberExists(string rfidNumber)
        {
            using (SqlConnection connection = new SqlConnection(constring))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Personel WHERE RFIDNo = @RFIDNo";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RFIDNo", rfidNumber);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        private void kartGoruntuleme_Load(object sender, EventArgs e)
        {
            serialPort1.PortName = Form1.portnum;
            serialPort1.BaudRate = Convert.ToInt16(Form1.banthizi);

            if (serialPort1.IsOpen == false)
            {
                serialPort1.Open();
                // textBox5.Text = serialPort1.ReadExisting();
            }
      
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = null;
            textBox2.Text = null;
            textBox3.Text = null;
            textBox4.Text = null;

            if (serialPort1.IsOpen == true)
            {
                textBox5.Text = serialPort1.ReadExisting();
            }
            else
            {
                serialPort1.Open();
                textBox5.Text = serialPort1.ReadExisting();
            }
            if (IsRFIDNumberExists(textBox5.Text))
            {
                try
                {
                    connection.Open();

                    // Kart numarasına göre ilgili kişiyi seçiyoruz
                    string sorgu = "SELECT * FROM Personel WHERE RFIDNo = @RFIDNo";
                    using (SqlCommand komut = new SqlCommand(sorgu, connection))
                    {
                        komut.Parameters.AddWithValue("@RFIDNo", textBox5.Text); // textBox5.Text değerini parametre olarak ekleriz

                        SqlDataReader reader = komut.ExecuteReader();

                        if (reader.Read())
                        {
                            string ad = reader["Ad"].ToString();
                            string soyad = reader["Soyad"].ToString();
                            string departman = reader["Departman"].ToString();
                            string id = reader["RFIDNo"].ToString();



                            // İlgili bilgileri formda görüntüleriz
                            textBox1.Text = ad;
                            textBox2.Text = soyad;
                            textBox3.Text = departman;
                            textBox4.Text = id;

                            // İlk SqlDataReader'ı kapatın
                            reader.Close();

                            string selectQuery = "SELECT Yetki FROM Personel WHERE RFIDNo = @RFIDNo";
                            SqlCommand yetkiKomut = new SqlCommand(selectQuery, connection);

                            yetkiKomut.Parameters.AddWithValue("@RFIDNo", textBox5.Text);
                            SqlDataReader yetkiReader = yetkiKomut.ExecuteReader();

                            if (yetkiReader.Read())
                            {
                                string yetkiler = yetkiReader["Yetki"].ToString();

                                // Her bir kilidin yetkisini kontrol edip etikete yaz
                                label1.Text = yetkiler[0] == '1' ? "Yetki Var" : "Yetki Yok";
                                label2.Text = yetkiler[1] == '1' ? "Yetki Var" : "Yetki Yok";
                                label3.Text = yetkiler[2] == '1' ? "Yetki Var" : "Yetki Yok";
                                label4.Text = yetkiler[3] == '1' ? "Yetki Var" : "Yetki Yok";

                                yetkiReader.Close();
                            }
                            else
                            {
                                MessageBox.Show("Seçilen kişiye ait yetki bulunamadı.");
                            }


                            connection.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı Hatası: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Tanımsız Kart");
            }
        }

        private void kartGoruntuleme_FormClosed_1(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
            }
        }
    }
}   