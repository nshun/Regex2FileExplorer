using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Regex2FileExplorer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            textBoxRegexFrom.KeyDown += TextBoxRegexFrom_KeyDown;
            textBox1.KeyDown += TextBox1_KeyDown;
            textBox2.KeyDown += TextBox2_KeyDown;
            listBox1.MouseDoubleClick += ListBox1_MouseDoubleClick;
            listBox2.MouseDoubleClick += ListBox2_MouseDoubleClick;

        }
        
        private void TextBoxRegexFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                //TODO ハイライト機能
            }
        }

        private void ListBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string path = "";
            var fileName = listBox1.SelectedItem.ToString();
            var dirPath = textBox1.Text;
            if (fileName == "..") path = Directory.GetParent(dirPath).ToString();
            else path = Path.Combine(dirPath, fileName);
            if (Directory.Exists(path))
            {
                textBox1.Text = path;
                updateList(textBox1.Text, listBox1);
            }
            else if(File.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
            
        }
        private void ListBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string path = "";
            var fileName = listBox2.SelectedItem.ToString();
            var dirPath = textBox2.Text;
            if (fileName == "..") path = Directory.GetParent(dirPath).ToString();
            else path = Path.Combine(dirPath, fileName);
            if (Directory.Exists(path))
            {
                textBox2.Text = path;
                updateList(textBox2.Text, listBox2);
            }
            else if (File.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
        }


        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && Directory.Exists(textBox1.Text))
            {
                updateList(textBox1.Text, listBox1);
                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    textBox2.Text = textBox1.Text;
                    updateList(textBox1.Text, listBox2);
                }
            }
        }

        private void TextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && Directory.Exists(textBox1.Text))
            {
                updateList(textBox2.Text, listBox2);
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    textBox2.Text = textBox1.Text;
                    updateList(textBox2.Text, listBox1);
                }
            }
        }

        private void updateList(string path, ListBox listBox)
        {
            string[] filesName = getFileDirNames(path);
            listBox.Items.Clear();
            try
            {
                var parent = Directory.GetParent(path).ToString();
                listBox.Items.Add("..");
            }
            catch
            {

            }
            listBox.Items.AddRange(filesName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fromFiles = getFileNames(textBox1.Text);
            var fromPattern = textBoxRegexFrom.Text;
            var toPattern = textBoxRegexTo.Text;
            foreach (var file in fromFiles)
            {
                if(Regex.IsMatch(file, fromPattern))
                {
                    var toFile = Regex.Replace(file, fromPattern, toPattern);
                    var sourcePath = new Uri(new Uri(textBox1.Text+@"\"), file).LocalPath;
                    var destPath = new Uri(new Uri(textBox2.Text + @"\"), toFile).LocalPath;
                    var destDir = Path.GetDirectoryName(destPath);
                    if (!Directory.Exists(destDir))
                        Directory.CreateDirectory(destDir);
                    File.Move(sourcePath, destPath);
                }
            }
            updateList(textBox1.Text, listBox1);
            updateList(textBox2.Text, listBox2);

        }

        private string[] getFileDirNames(string path)
        {
            string[] files = Directory.GetFileSystemEntries(path, "*", System.IO.SearchOption.TopDirectoryOnly);
            string[] filesName = new string[files.Count()];
            for (int i = 0; i < files.Count(); i++)
            {
                var name = Path.GetFileName(files[i]);
                filesName[i] = name;
            }
            return filesName;
        }

        private string[] getFileNames(string path)
        {
            string[] files = Directory.GetFiles(path, "*", System.IO.SearchOption.TopDirectoryOnly);
            string[] filesName = new string[files.Count()];
            for (int i = 0; i < files.Count(); i++)
            {
                var name = Path.GetFileName(files[i]);
                filesName[i] = name;
            }
            return filesName;
        }
    }
}
