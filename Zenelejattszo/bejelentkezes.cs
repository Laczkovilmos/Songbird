using System;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Zenelejattszo
{
    public partial class bejelentkezes : Form
    {
        private string felhasznalokFajl = "felhasznalok.txt";

        public bejelentkezes()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
            string username = textBox1.Text.Trim(); 
            string password = textBox2.Text.Trim(); 

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Kérlek, töltsd ki a felhasználónevet és a jelszót!");
                return;
            }

            if (File.Exists(felhasznalokFajl))
            {
                var felhasznalok = File.ReadAllLines(felhasznalokFajl);
                foreach (var sor in felhasznalok)
                {
                    var parts = sor.Split(',');
                    if (parts.Length == 2 && parts[0] == username && parts[1] == password)
                    {
                        MessageBox.Show("Sikeres bejelentkezés!");
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        return;
                    }
                }
            }
            MessageBox.Show("Hibás felhasználónév vagy jelszó!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();         
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Kérlek, töltsd ki a felhasználónevet és a jelszót!");
                return;
            }

            if (!File.Exists(felhasznalokFajl))
            {
                File.Create(felhasznalokFajl).Close();
            }

            var felhasznalok = File.ReadAllLines(felhasznalokFajl);

            foreach (var sor in felhasznalok)
            {
                var parts = sor.Split(',');
                if (parts.Length == 2 && parts[0] == username)
                {
                    MessageBox.Show("Ez a felhasználónév már foglalt!");
                    return;
                }
            }

            File.AppendAllText(felhasznalokFajl, $"{username},{password}{Environment.NewLine}");
            MessageBox.Show("Sikeres regisztráció!");
        }

        private void bejelentkezes_Load(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        
    }
}