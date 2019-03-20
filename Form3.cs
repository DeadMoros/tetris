using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tetris
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 f1 = (Form1)this.Owner;
            if (textBox1.Text != "")
            {
                f1.playerName = textBox1.Text;
                f1.startCheckGrid();
                this.Close();
            }
            else MessageBox.Show("Вам нужно ввести имя что бы продолжить!");
        }
    }
}
