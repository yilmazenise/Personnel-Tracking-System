using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace Personel_Tanima
{
    public partial class Form1 : Form
    {
        public static string portnum, banthizi;
        string[] ports = SerialPort.GetPortNames();

        public Form1()
        {
            InitializeComponent();
        }
        static string constring = ("Data Source=ENISE;Initial Catalog=veri;Integrated Security=True");
        SqlConnection baglan = new SqlConnection(constring);

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string port in ports)
            {
                comboBox1.Items.Add(port); // bilgisayarımıza bağlı olan portları ekliyoruz comboboxa
            }


            comboBox2.Items.Add("9600"); // baud rate olarak sadece 9600 değerini atadık


            comboBox1.SelectedIndex = 0; // seçili açılmasını istediğim için index atadım
            comboBox2.SelectedIndex = 0; // seçili açılmasını istediğim için index atadım
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            // timer1.Start();
            portnum = comboBox1.Text;
            banthizi = comboBox2.Text;

            if (!serialPort1.IsOpen) // Eğer port zaten açık değilse bağlantıyı yeniden kur
            {
                serialPort1.PortName = portnum;
                serialPort1.BaudRate = Convert.ToInt16(banthizi);
                serialPort1.Open();
                label1.Text = "Bağlantı Sağlandı!";
                label1.ForeColor = Color.Green;

            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (serialPort1.IsOpen == true)
            {
                serialPort1.Close();
            }
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            /* string kartokumasi;
               kartokumasi = serialPort1.ReadExisting();
               if(kartokumasi != "")
               {
                   label2.Text = kartokumasi;
               }
            */
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (portnum == null || banthizi == null)
            {
                MessageBox.Show("Bağlantınızı kontrol ediniz.");
            }
            else
            {
                timer1.Stop();
                serialPort1.Close();
                // label1.Text = "Bağlantı kapalıdır.";
                //label1.ForeColor = Color.Red;

                kayit_formu kayit = new kayit_formu();
                kayit.ShowDialog(); // diğer pencere kapanmasın diye.
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            baglan.Open();
            string getir = "Select *From Personel";

            SqlCommand komut = new SqlCommand(getir, baglan);
            SqlDataAdapter ad = new SqlDataAdapter(komut);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            dataGridView1.DataSource = dt;
            baglan.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            if (portnum == null || banthizi == null)
            {
                MessageBox.Show("Bağlantınızı kontrol ediniz.");
            }

            else
            {
                kartGoruntuleme kart = new kartGoruntuleme();
                kart.ShowDialog();
                timer1.Stop();

            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (portnum == null || banthizi == null)
            {
                MessageBox.Show("Bağlantınızı kontrol ediniz.");
            }
            else
            {
                // timer1.Stop();
                serialPort1.Close();
                // label1.Text = "Bağlantı kapalıdır.";
                //label1.ForeColor = Color.Red;

                YetkilendirmeEkrani yetki = new YetkilendirmeEkrani();
                yetki.ShowDialog(); // diğer pencere kapanmasın diye.
            }
        }

        private void button7_Click(object sender, EventArgs e)
         {
             if (baglan.State == ConnectionState.Closed)
             {
                 baglan.Open();
             }

             foreach (DataGridViewRow row in dataGridView1.Rows)
             {
                 if (!row.IsNewRow)
                 {
                     string yeniAd = row.Cells["Ad"].Value.ToString();
                     string yeniSoyad = row.Cells["Soyad"].Value.ToString();
                     string yeniTCKimlikNo = row.Cells["TCKimlikNo"].Value.ToString();
                     string yeniDepartman = row.Cells["Departman"].Value.ToString();
                     string yeniTelefon = row.Cells["Telefon"].Value.ToString();
                     string yeniRFIDNo = row.Cells["RFIDNo"].Value.ToString();

                     // Güncelleme sorgusu
                     string updateQuery = "UPDATE Personel SET Ad=@Ad, Soyad=@Soyad, TCKimlikNo=@TCKimlikNo, Departman=@Departman, Telefon=@Telefon,  RFIDNo=@RFIDNo WHERE TCKimlikNo=@TCKimlikNo";

                     SqlCommand cmd = new SqlCommand(updateQuery, baglan);

                     cmd.Parameters.AddWithValue("@Ad", yeniAd);
                     cmd.Parameters.AddWithValue("@Soyad", yeniSoyad);
                     cmd.Parameters.AddWithValue("@TCKimlikNo", yeniTCKimlikNo);
                     cmd.Parameters.AddWithValue("@Departman", yeniDepartman);
                     cmd.Parameters.AddWithValue("@Telefon", yeniTelefon);
                     cmd.Parameters.AddWithValue("@RFIDNo", yeniRFIDNo);

                     try
                     {
                         int rowsAffected = cmd.ExecuteNonQuery();
                         if (rowsAffected > 0)
                         {
                             MessageBox.Show("Kayıt güncellendi.");
                         }
                         else
                         {
                             MessageBox.Show("Güncellenecek kayıt bulunamadı.");
                         }
                     }
                     catch (Exception ex)
                     {
                         MessageBox.Show("Hata oluştu: " + ex.Message);
                     }
                 }
             }

             baglan.Close();
         }

        

        private void button2_Click_1(object sender, EventArgs e)
        {
            timer1.Stop(); // durdurmamız lazım yoksa hata alıyoruz.
            if (serialPort1.IsOpen == true)
            {
                try
                {
                    serialPort1.Close();
                    label1.Text = "Bağlantı Kesildi!";
                    label1.ForeColor = Color.Red;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bağlantı kapatılırken bir hata oluştu: " + ex.Message);
                }
            }
            else
            {
                serialPort1.Close();
                label1.Text = "Bağlantı Kesildi!";
                label1.ForeColor = Color.Red;
                MessageBox.Show("Zaten bağlantı kapalı.");
            }
        }
    }
}

