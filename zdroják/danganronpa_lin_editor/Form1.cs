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

namespace danganronpa_lin_editor
{
    public partial class Form1 : Form
    {
        string[] textSoubor;
        int[] poziceSouboru;
        string[] poziceSouboruShort;
        string[] upravenejText;
        string[] postavy = 
            {"Naegi", "Ishimaru", "Togami", "Mondo", "Leon", "Yamada", 
             "Hagakure", "Maizono", "Kirigiri", "Asahina", "Fukawa", 
             "Sakura", "Celes", "Enoshima", "Fujisaki", "Monokuma"};

        public Form1()
        {
            InitializeComponent();
        }

        void cistSoubor()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Lin skript (*.lin)|*.lin*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    dekompilace(openFileDialog.FileName);
                    textSoubor = File.ReadAllLines("temp.txt");
                    poziceSouboru = new int[textSoubor.Length];
                    richTextBox1.Lines = textSoubor;
                    button3.Enabled = true;
                }
            }

        }

        void ulozitSoubor()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Lin skript (*.lin)|*.lin*";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.Delete("temp.txt");
                    using (var s = File.Create("temp.txt"))
                    {
                        using (var sw = new StreamWriter(s, new UnicodeEncoding(false, true)))
                        {
                            foreach (string i in textSoubor)
                            {
                                sw.WriteLine(i);
                            }
                        }
                    }
                    kompilace(saveFileDialog.FileName);
                }
            }
        }

        void dekompilace(string soubor)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C lin_compiler -d " + soubor + " temp.txt";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
        void kompilace(string soubor)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C lin_compiler temp.txt " + soubor + ".lin";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            cistSoubor();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ulozitSoubor();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            int counter = 0;
            for (int i = 0; i < textSoubor.Length; i++)
            {
                if (textSoubor[i].Contains("Sprite"))
                {
                    string templine = textSoubor[i].Replace("Sprite(", "");
                    templine = templine.Replace(templine.Substring(2), "");
                    if (templine.Substring(1) == ",") templine = templine.Replace(templine.Substring(1), "");
                    richTextBox1.AppendText(postavy[Int16.Parse(templine)] + ":\n");

                }
                if (textSoubor[i].Contains("Text"))
                {
                    string templine = textSoubor[i].Replace("Text(\"", "");
                    templine = templine.Replace("\")", "");
                    richTextBox1.AppendText("\t" + templine + "\n");
                    poziceSouboru[counter] = i;
                    counter++;
                }
            }
            upravenejText = richTextBox1.Lines;
            for (int i = 0; i < counter; i++)
            {
                textBox1.Text += poziceSouboru[i] + ", ";
            }
            int len = 0;
            for (int i = 0; i < poziceSouboru.Length; i++)
            {
                if (poziceSouboru[i] != 0)
                    len++;
            }
            poziceSouboruShort = new string[len];
            for (int i = 0, j = 0; i < poziceSouboru.Length; i++)
            {
                if (poziceSouboru[i] != 0)
                {
                    poziceSouboruShort[j] = poziceSouboru[i].ToString();
                    j++;
                }
            }
            button6.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Lines = textSoubor;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            richTextBox1.Lines = upravenejText;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string[] tempText = richTextBox1.Lines;
            richTextBox1.Clear();
            for (int i = 0; i < tempText.Length; i++)
            {
                if (tempText[i].Contains("\t")) richTextBox1.AppendText(tempText[i].Replace("\t", "") + "\n");
            }
            
            upravenejText = richTextBox1.Lines;   

            for (int i = 0; i < textSoubor.Length; i++)
            {
                for (int j = 0; j < (poziceSouboruShort.Length - 1); j++)
                {
                    if (poziceSouboruShort[j] == i.ToString())
                    {
                        textSoubor[i] = "Text(\"" + upravenejText[j] + "\")";
                    }
                }
            }
            richTextBox1.Lines = textSoubor;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            
        }
    }
}
