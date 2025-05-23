using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq; 
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace Zenelejattszo
{
    public partial class Form1 : Form
    {
        private List<string> musicFiles;
        private string currentSong;      
        private bool isPaused;

       
        private bool isChangingPosition;

        public Form1()
        {
            InitializeComponent();
            musicFiles = new List<string>();
            isPaused = false;
            isChangingPosition = false; 

            this.Load += Form1_Load;
            button5.Click += btnTorles_Click;
            button6.Click += btnKedvenc_Click;


            textBox1.TextChanged += textBox1_TextChanged;
            button7.Click += button7_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            bejelentkezes loginForm = new bejelentkezes();
            var result = loginForm.ShowDialog();

            if (result != DialogResult.OK)
            {
               
                this.Close();
                return;

            }

            MessageBox.Show("Üdvözöljük a Songbird zenelejáttszóban!");

            if (File.Exists("lejátszottak.txt"))
            {
                var sorok = File.ReadAllLines("lejátszottak.txt").Distinct();
                foreach (var sor in sorok)
                {
                   
                    if (File.Exists(sor) && !musicFiles.Contains(sor))
                    {
                        musicFiles.Add(sor);
                       
                    }
                }
            }

         
            FilterListBox();

           
           
          
            trackBar1.Minimum = 0;
            trackBar1.Maximum = 100;
           
            musicPlayer.settings.volume = 50;
            trackBar1.Value = musicPlayer.settings.volume;
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "MP3 Files | *mp3";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    if (!musicFiles.Contains(file))
                    {
                        musicFiles.Add(file);
                    }
                }
              
                FilterListBox();
            }
            if (musicFiles.Count > 0)
            {
                button2.Enabled = true; 
            }
        }

      
        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                
                string selectedFileName = listBox1.SelectedItem.ToString();
                string filePathToPlay = musicFiles.FirstOrDefault(f => Path.GetFileName(f) == selectedFileName);

                if (string.IsNullOrEmpty(filePathToPlay))
                {
                    MessageBox.Show("Hiba: A kiválasztott fájl nem található a könyvtárban. Kérlek, válassz másikat.");
                    return; 
                }

               
                if (isPaused && currentSong == filePathToPlay)
                {
                    musicPlayer.Ctlcontrols.play();
                    isPaused = false;
                }
                else 
                {
                    currentSong = filePathToPlay;
                    musicPlayer.URL = currentSong; 
                    musicPlayer.Ctlcontrols.play();
                    isPaused = false;

                    MentLejátszottZene(currentSong);
                   
                   
                }
                timerPlayback.Enabled = true; 
            }
            else
            {
                MessageBox.Show("Kérlek válassz ki egy zenét a lejátszáshoz!");
            }
        }

        
        private void button3_Click(object sender, EventArgs e)
        {
            musicPlayer.Ctlcontrols.stop(); 
            isPaused = false;
            timerPlayback.Enabled = false;
         
            label2.Text = "00:00 / 00:00";
        }

      
        private void button4_Click(object sender, EventArgs e)
        {
            if (musicPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                musicPlayer.Ctlcontrols.pause(); 
                isPaused = true;
            }
            else if (musicPlayer.playState == WMPLib.WMPPlayState.wmppsPaused)
            {
                musicPlayer.Ctlcontrols.play(); 
                isPaused = false;
            }
        }

 
        private void timerPlayback_Tick(object sender, EventArgs e)
        {
          
            if (musicPlayer.currentMedia != null && musicPlayer.playState == WMPLib.WMPPlayState.wmppsPlaying)
            {
                label2.Text = $"Lejátszás: {FormatTime(musicPlayer.Ctlcontrols.currentPosition)} / {FormatTime(musicPlayer.currentMedia.duration)}";
            }
            else if (musicPlayer.currentMedia == null || musicPlayer.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                label2.Text = "00:00 / 00:00"; 
            }
        }


        private string FormatTime(double seconds)
        {
          
            if (double.IsNaN(seconds) || double.IsInfinity(seconds) || seconds < 0)
            {
                return "00:00";
            }
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return time.ToString(@"mm\:ss");
        }

     
        private void musicPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
          
            if (e.newState == (int)WMPLib.WMPPlayState.wmppsMediaEnded)
            {
               
                int currentIndexInFullList = musicFiles.IndexOf(currentSong);
                int nextIndexInFullList = currentIndexInFullList + 1;

         
                if (nextIndexInFullList < musicFiles.Count)
                {
                    string nextFilePath = musicFiles[nextIndexInFullList];

               
                    string nextFileName = Path.GetFileName(nextFilePath);
                    int newListBoxIndex = listBox1.FindStringExact(nextFileName); 
                    if (newListBoxIndex != ListBox.NoMatches)
                    {
                        listBox1.SelectedIndex = newListBoxIndex; 
                    }
                    else
                    {
                        
                        listBox1.SelectedIndex = -1;
                    }

                    currentSong = nextFilePath;
                    musicPlayer.URL = currentSong; 
                    musicPlayer.Ctlcontrols.play();
                    isPaused = false;

                    MentLejátszottZene(currentSong); 
            
                  
                }
                else
                {
                  
                    musicPlayer.Ctlcontrols.stop();
                    isPaused = false;
                    timerPlayback.Enabled = false; 
                    
                    label2.Text = "00:00 / 00:00"; 
                    listBox1.SelectedIndex = -1; 
                }
            }
            
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
        
            musicPlayer.settings.volume = trackBar1.Value;
        }

  
        private void MentLejátszottZene(string fajlUt)
        {
            string filePath = "lejátszottak.txt";

            try
            {
                var letezoSorok = new HashSet<string>();
                if (File.Exists(filePath))
                {
                    
                    letezoSorok = new HashSet<string>(File.ReadAllLines(filePath));
                }

                if (!letezoSorok.Contains(fajlUt))
                {
                    using (StreamWriter sw = File.AppendText(filePath)) 
                    {
                        sw.WriteLine(fajlUt);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt a fájl mentésekor (lejátszottak.txt): " + ex.Message);
            }
        }

       
        private void btnTorles_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox1.SelectedIndex;
            if (selectedIndex >= 0)
            {
               
                string selectedFileName = listBox1.SelectedItem.ToString();
                string filePathToRemove = musicFiles.FirstOrDefault(f => Path.GetFileName(f) == selectedFileName);

                if (string.IsNullOrEmpty(filePathToRemove))
                {
                    MessageBox.Show("Hiba: A kiválasztott elem nem található a listában.");
                    return;
                }

               
                if (currentSong == filePathToRemove)
                {
                    musicPlayer.Ctlcontrols.stop();
                    isPaused = false;
                    timerPlayback.Enabled = false;
                    currentSong = null;
                 
                    label2.Text = "00:00 / 00:00";
                }

                musicFiles.Remove(filePathToRemove); 
                FilterListBox();
            }
            else
            {
                MessageBox.Show("Kérlek válassz ki egy zenét a törléshez!");
            }
        }

       
        private void btnKedvenc_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox1.SelectedIndex;
            if (selectedIndex >= 0)
            {
            
                string selectedFileName = listBox1.SelectedItem.ToString();
                string selectedSongFullPath = musicFiles.FirstOrDefault(f => Path.GetFileName(f) == selectedFileName);

                if (string.IsNullOrEmpty(selectedSongFullPath))
                {
                    MessageBox.Show("Hiba: A kiválasztott zene elérési útja nem található.");
                    return;
                }

                string kedvencekFile = "kedvencek.txt";

                try
                {
                    var kedvencek = new HashSet<string>();
                    if (File.Exists(kedvencekFile))
                    {
                        kedvencek = new HashSet<string>(File.ReadAllLines(kedvencekFile));
                    }

                  
                    if (!kedvencek.Contains(selectedSongFullPath))
                    {
                        using (StreamWriter sw = File.AppendText(kedvencekFile))
                        {
                            sw.WriteLine(selectedSongFullPath);
                        }
                        MessageBox.Show("Zene hozzáadva a kedvencekhez!");
                    }
                    else
                    {
                        MessageBox.Show("Ez a zene már a kedvencek között van.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba a kedvencek mentésekor: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Kérlek válassz ki egy zenét a kedvencekhez!");
            }
        }

       
        private void FilterListBox()
        {
          
            string searchTerm = textBox1.Text.ToLowerInvariant().Trim();

            string selectedFilePathBeforeFilter = null;
            if (listBox1.SelectedIndex >= 0)
            {
                string selectedFileName = listBox1.SelectedItem.ToString();
                selectedFilePathBeforeFilter = musicFiles.FirstOrDefault(f => Path.GetFileName(f) == selectedFileName);
            }

            listBox1.Items.Clear();

            IEnumerable<string> filteredMusicFiles;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
               
                filteredMusicFiles = musicFiles.OrderBy(f => Path.GetFileName(f));
            }
            else
            {
             
                filteredMusicFiles = musicFiles.Where(file =>
                    Path.GetFileNameWithoutExtension(file).ToLowerInvariant().Contains(searchTerm)
                ).OrderBy(f => Path.GetFileName(f));
            }

  
            foreach (string file in filteredMusicFiles)
            {
                listBox1.Items.Add(Path.GetFileName(file));
            }

       
            if (!string.IsNullOrEmpty(selectedFilePathBeforeFilter))
            {
                string selectedFileNameAfterFilter = Path.GetFileName(selectedFilePathBeforeFilter);
   
                int newIndex = listBox1.FindStringExact(selectedFileNameAfterFilter);
                if (newIndex != ListBox.NoMatches)
                {
                    listBox1.SelectedIndex = newIndex;
                }
            }
        }

       
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            FilterListBox(); 
        }

     
        private void button7_Click(object sender, EventArgs e)
        {
            FilterListBox(); 
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        
    }
}