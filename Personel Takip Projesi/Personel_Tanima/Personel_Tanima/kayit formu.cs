using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; // Bu isim alanı ise SQL Server için geliştirilmiş sınıfları kullanamaya olanak sağlar.

namespace Personel_Tanima
{
    public partial class kayit_formu : Form
    {
      
        public kayit_formu()
        {
            InitializeComponent();
        }
        static string constring = ("Data Source=ENISE;Initial Catalog=veri;Integrated Security=True");
        SqlConnection baglan = new SqlConnection(constring);

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

       /* private void timer1_Tick(object sender, EventArgs e)
        {
            string kartokumasi;
            kartokumasi = serialPort1.ReadExisting();
            if (kartokumasi != "")
            {
                label8.Text = kartokumasi; 
            }
        }
       */

        private void label7_Click(object sender, EventArgs e)
        {

        }

      
        private void kayit_formu_Load(object sender, EventArgs e)
        {
            serialPort1.PortName = Form1.portnum;
            serialPort1.BaudRate = Convert.ToInt16(Form1.banthizi);

            if(serialPort1.IsOpen == false)
            {   
                try
                {
                    serialPort1.Open();
                    label7.Text = "Bağlantı Sağlandı.";
                    label7.ForeColor = Color.Green;
                }
                catch
                {   
                    label7.Text = "Bağlantı Sağlanamadı.";
                }
            }
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /* timer1.Start();
             textBox1.Text = "";
             textBox2.Text = "";
             textBox3.Text = "";
             textBox4.Text = "";
             textBox5.Text = "";
             textBox6.Text = "";
            */

            if (serialPort1.IsOpen == false)
            {
                serialPort1.Open();
                textBox6.Text = serialPort1.ReadExisting();

            }
            else
            {
                textBox6.Text = serialPort1.ReadExisting();
            }
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


        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox6.Text == "")
            {
                textBox1.BackColor = Color.Yellow;
                textBox2.BackColor = Color.Yellow;
                textBox6.BackColor = Color.Yellow;
                MessageBox.Show("Boş alan bırakamazsınız.");
            }
            else
            {
                // RFID numarasının veritabanında kayıtlı olup olmadığını kontrol ediyorum
                if (IsRFIDNumberExists(textBox6.Text))
                {
                    MessageBox.Show("Bu RFID numarası zaten kaydedilmiş.");
                }
                else
                {
                    MessageBox.Show("Kayıt ekleme işlemi başarılı.");
                    if (baglan.State == ConnectionState.Closed)
                    {
                        baglan.Open();
                        string kayit = "insert into Personel (Ad, Soyad, TCKimlikNo, Departman, Telefon, RFIDNo) values (@Ad, @Soyad, @TCKimlikNo, @Departman, @Telefon, @RFIDNo)";
                        SqlCommand komut = new SqlCommand(kayit, baglan);
                        komut.Parameters.AddWithValue("@Ad", textBox1.Text);
                        komut.Parameters.AddWithValue("@Soyad", textBox2.Text);
                        komut.Parameters.AddWithValue("@TCKimlikNo", textBox3.Text);
                        komut.Parameters.AddWithValue("@Departman", textBox5.Text);
                        komut.Parameters.AddWithValue("@Telefon", textBox4.Text);
                        komut.Parameters.AddWithValue("@RFIDNo", textBox6.Text);
                        komut.ExecuteNonQuery();
                    }
                }
            }
        }


        private void kayit_formu_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
            }
        }
    }
}