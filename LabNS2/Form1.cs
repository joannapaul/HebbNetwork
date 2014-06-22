using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LabNS2
{
    public partial class Form1 : Form
    {
        string Path;
        List<byte> points;

        Holder objects;

        public Form1()
        {
            InitializeComponent();
            objects = Holder.GetInstance(Preview.Width, Preview.Height);
            ReadFile();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Result.Text = "";
            try
            {
                openFileDialog1.Title = "Open Picture";
                openFileDialog1.InitialDirectory = System.Environment.CurrentDirectory + "/test/";
                openFileDialog1.ShowDialog();
                Path = openFileDialog1.FileName;
                Preview.Load(Path);
                points = new List<byte>(Preview.Width * Preview.Height + 1);
            }
            catch { MessageBox.Show("Error open file"); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Result.Text = "";
            points.Clear();
            points.Add(1);

            Bitmap temp = new Bitmap(Preview.Image, new Size(320, 240));

            for (int i = 0; i < temp.Width; i++)
                for (int j = 0; j < temp.Height; j++)
                {
                    Color current = temp.GetPixel(i, j);
                    if (current.R > 127 && current.G > 127 && current.B > 127)
                        temp.SetPixel(i, j, Color.White);
                    else
                        temp.SetPixel(i, j, Color.Black);
                }
            for (int i = 1; i < points.Capacity; i++)
                points.Add(temp.GetPixel((i - 1) % temp.Width, (i - 1) / temp.Width).ToArgb() != -1 ? (byte)1 : (byte)0);
            Result.Text = objects.Recognize(points);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            char smb;
            if (Result.Text == "")
                smb = '\n';
            else
                smb = Result.Text[0];
            Result.Text = objects.Correct(smb, points);
            Result.Text = "";

            this.Refresh();
            objects.Save();
            Result.Text = "Сохранен";

        }

        void ReadFile()
        {
            //чтение файла chars.txt, который содержит список существующих символов
            if (File.Exists("chars.txt"))
            {
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader("chars.txt");
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        line = sr.ReadLine();
                        if (line == "") break;
                        else
                        {
                            char smb = line[0];
                            objects.AddNeuron(smb);
                        }
                    }
                }

                finally
                {
                    sr.Close();
                }
            }
            else
            {

                StreamWriter write = new StreamWriter("chars.txt");
                write.Close();
            }

        }
        void WriteFile(char a)
        {
            StreamWriter sw;
            sw = new StreamWriter("chars.txt", true);

            sw.WriteLine(a.ToString());
            sw.Flush();
            sw.Close();
        }
    }
}
