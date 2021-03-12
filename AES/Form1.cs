using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace AES
{
    public partial class Form1 : Form
    {
        private byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }; // 16 BITU SIFRAVIMO RAKTAS
        private int BlockSize = 128; // BLOKAS - MINIMALUS RAKTO DYDIS 128
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // ATIDAROME LANGA - PASIRENKAME FAILA - UZDAROME LANGA

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            if (openFileDialog1.ShowDialog() == DialogResult.Cancel) return;
            textBox1.Text = openFileDialog1.FileName;

            FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            richTextBox1.Text = sr.ReadToEnd();

            sr.Close();
            fs.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // TEKSTO ISSAUGIJIMAS I FAILA

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel) return;

            FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);

            sw.Write(richTextBox1.Text);

            sw.Close();
            fs.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "") return;
            byte[] bytes = Encoding.Unicode.GetBytes(richTextBox1.Text);
            //SIFRAVIMAS
            SymmetricAlgorithm crypt = Aes.Create(); // SUKURIAM NAUJA AES EGZEMPLIORIU
            HashAlgorithm hash = MD5.Create(); // (HASH) SUKURIAM ALGORITMA KURIS APSKAICIUOS
            crypt.BlockSize = BlockSize; // PAIMAM 128 BITU BLOKA
            crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes(textBox2.Text)); // SLAPTAZODIS
            crypt.IV = IV; // PRADINIS VEKTORIUS

            using (MemoryStream memoryStream = new MemoryStream())
            {
                // PAIMA BAITU MASYVA IR SIUNCIA UZSIFRUOTUS BAITUS I MEMORYSTREAM
                // KURIA MES PERSKAITOME IR KONVERTUOJAME I BASE64 EILUTE, KAD GALIMA BUTU PERSKAITYTI
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, crypt.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                }

                // GAUTAS UZSIFRUOTAS TEKSTAS YRA IRASOMAS I RECHTEXTBOX
                richTextBox1.Text = Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "") return;
            // DESIFRAVIMAS
            // UZSIFRAVIMO TEKSTA TURIME GRIZTI ATGAL - PAVERCIAME BAITU MASYVA FROMBASE64STRING
            byte[] bytes = Convert.FromBase64String(richTextBox1.Text);
            SymmetricAlgorithm crypt = Aes.Create(); // SUKURIAM NAUJA AES EGZEMPLIORIU
            HashAlgorithm hash = MD5.Create(); // (HASH) SUKURIAM ALGORITMA KURIS APSKAICIUOS 128 BITU ILGIO PARASA
            crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes(textBox2.Text)); // SLAPTAZODIS
            crypt.IV = IV; // PRADINIS VEKTORIUS

            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                // CRYPTOSTREAM PERSKAITO MEMORYSTREAM
                // DESIFRUOJA BAITU DUOMENIS
                // GRAZINA DESIFRUOTA BAITU MASYVA 
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] decryptedBytes = new byte[bytes.Length];
                    cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                    richTextBox1.Text = Encoding.Unicode.GetString(decryptedBytes);
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
