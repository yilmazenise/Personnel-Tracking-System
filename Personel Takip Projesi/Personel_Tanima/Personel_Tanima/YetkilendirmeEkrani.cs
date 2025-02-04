using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Personel_Tanima
{
    public partial class YetkilendirmeEkrani : Form
    {
        private string yetkiler = "";
        public YetkilendirmeEkrani()
        {
            InitializeComponent();
        }

        static string constring = ("Data Source=ENISE;Initial Catalog=veri;Integrated Security=True");
        SqlConnection connection = new SqlConnection(constring);

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            connection.Open();
            SqlCommand komut = new SqlCommand("SELECT Ad, Soyad FROM Personel", connection); 

            SqlDataReader dr = komut.ExecuteReader();

            while (dr.Read())
            {
                string ad = dr["Ad"].ToString();
                string soyad = dr["Soyad"].ToString();
                string tamAd = ad + " " + soyad; // İsim ve soyisim arasına boşluk ekledim
                comboBox1_personel.Items.Add(tamAd);
               
            }

            connection.Close();
        }


        private void checkBox4_model3111_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_personel_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            yetkiler = ""; // Yetkileri temizle

            // Combobox'tan seçilen kişiyi alıyorum
            string secilenKisi = comboBox1_personel.SelectedItem.ToString();

            // Burada checkbox'ların durumuna göre yetkileri oluşturuyorum
            string yetki3101 = checkBox1_model3101.Checked ? "1" : "0";
            string yetki3103 = checkBox2_model3103.Checked ? "1" : "0";
            string yetki3105 = checkBox3_model3105.Checked ? "1" : "0";
            string yetki3111 = checkBox4_model3111.Checked ? "1" : "0";

            // Yetkileri birleştirip string'e atıyorum
            yetkiler = yetki3101 + yetki3103 + yetki3105 + yetki3111;

            // Şimdi yetkiler string'ini kullanabilir veya SQL'e ekleyebilirim
            MessageBox.Show($"Seçilen Kişi: {secilenKisi}\nYetkiler: {yetkiler}");

            // SQL'e eklemek için bir UPDATE sorgusu oluşturuyorum
            connection.Open();
            string updateQuery = "UPDATE Personel SET Yetki = @Yetki WHERE Ad + ' ' + Soyad = @SecilenKisi";

            SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
            updateCommand.Parameters.AddWithValue("@Yetki", yetkiler);
            updateCommand.Parameters.AddWithValue("@SecilenKisi", secilenKisi);

            int affectedRows = updateCommand.ExecuteNonQuery();

            if (affectedRows > 0)
            {
                MessageBox.Show("Yetkiler başarıyla güncellendi.");
            }
            else
            {
                MessageBox.Show("Yetkiler güncellenirken bir hata oluştu.");
            }

            connection.Close();
        }
    }
}