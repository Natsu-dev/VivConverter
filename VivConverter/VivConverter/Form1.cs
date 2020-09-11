using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VivConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) //開くボタンの処理
        {
            DialogResult diar = openFileDialog1.ShowDialog();
            if (diar == System.Windows.Forms.DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e) //変換ボタンの処理
        {
            StreamReader strr = new StreamReader(textBox1.Text, Encoding.GetEncoding("UTF-8"));

            string viv = strr.ReadToEnd();
            strr.Close();
            label1.Text = viv;
        }
    }
}
