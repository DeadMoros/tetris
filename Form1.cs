using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Collections;
using System.Xml.Serialization;
using System.IO;

namespace tetris
{
    public partial class Form1 : Form
    {
        Form2 f2 = new Form2();
        bool stopPlay = false;
        public bool gameover = false;
        public string playerName;
        const int MAXX = 10;
        const int MAXY = 20;
        const int ps = 24;
        bool started = false;
        Brush[] bru = {Brushes.Salmon, Brushes.Orange, Brushes.Blue, Brushes.Red, Brushes.Green, Brushes.SeaGreen,
            Brushes.Violet, Brushes.Tomato, Brushes.SteelBlue, Brushes.Silver};
        
        public int score;
        int[,] ar;
        Figure figure;
       
        Graphics gr;
        SoundPlayer simpleSound = new SoundPlayer(tetris.Properties.Resources.chimes);

        public Form1()
        {
            InitializeComponent();
        }

        private void upd_score()
        {
            label2.Text = score.ToString();
        }

        private void draw(int x, int y, int clr,int pnclr)
        {
            if (clr == 0 || x < 0 || x >= MAXX || y < 0 || y >= MAXY) return;
            Pen myPen = new Pen(Color.White);
            gr.FillRectangle(bru[clr], new Rectangle(2 + x * ps, 2 + y * ps, ps, ps));
            gr.DrawRectangle(myPen, new Rectangle(2 + x * ps, 2 + y * ps, ps, ps));
            
        }
        
        private void draw_field()
        {
            int i, j;
            for (i = 0; i < MAXX; ++i)
                for (j = 0; j < MAXY; ++j)
                    draw(i, j, ar[i, j],ar[MAXX-i-1,MAXY-j-1]);
        }

        private void draw_figure(Figure f)
        {
            int i, j;
            for (i = 0; i < 4; ++i)                                   //ЗАПУСКАЕМ СОЗДАНИЕ ФИГУРЫ
                for (j = 0; j < 4; ++j)
                    
                    draw(f.x + i, f.y + j, f.pix[i, j],f.pix[3-i,3-j]);
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            label3.Visible = true;
            stopPlay = false;
            button2.Enabled = true;
            if (radioButton1.Checked == true)
            {
                timer1.Interval = 400;
            }
            else if (radioButton2.Checked == true)
            {
                timer1.Interval = 295;
            }
            else timer1.Interval = 150;
            started = true;
            score = 0;
            ar = new int[MAXX, MAXY];
            upd_score();
            figure = null;
            timer1.Enabled = true;
        }

        private int get_ar(int x, int y)
        {
            if (x < 0 || x >= MAXX || y >= MAXY) return 1;
            if (y < 0) return 0;
            return ar[x, y];
        }

        private void set_ar(int x, int y, int clr)
        {
            if (x < 0 || x >= MAXX || y < 0 || y >= MAXY) return;
            ar[x, y] = clr;
        }
        
        private bool figure_ok()
        {
            int i, j;
            for (i = 0; i < 4; ++i)
                for (j = 0; j < 4; ++j)
                {
                    if (figure.pix[i, j] != 0 && get_ar(figure.x + i, figure.y + j) != 0)
                        return false;
                }
            return true;
        }

        private void apply_figure()
        {
            int i, j;
            for (i = 0; i < 4; ++i)
                for (j = 0; j < 4; ++j)
                    if (figure.pix[i, j] != 0) set_ar(figure.x + i, figure.y + j, figure.pix[i, j]);
            bool[] lines = new bool[MAXY];
            int goo = 0;
            for (i = 0; i < MAXY; ++i)
            {
                bool qq = true;
                for (j = 0; j < MAXX; ++j)
                {
                    if (ar[j, i] == 0)
                    {
                        qq = false;
                        break;
                    }
                }
                lines[i] = qq;
                if (qq) ++goo;
            }
            if (goo != 0)
            {
                int scoreAmount = radioButton1.Checked ? scoreAmount=50 : radioButton2.Checked? scoreAmount=900 : scoreAmount=200;

                int sc = (goo * (goo + 1) / 2) * scoreAmount;
                score += sc;
                upd_score();
                simpleSound.Play();
            }
            int[,] nar = new int[MAXX, MAXY];
            int ty = MAXY;
            for (i = MAXY - 1; i >= 0; --i)
            {
                if (lines[i]) continue;
                --ty;
                for (j = 0; j < MAXX; ++j)
                {
                    nar[j, ty] = ar[j, i];
                }
            }
            ar = nar;
        }

        private void figure_down()
        {
            ++figure.y;
            if (!figure_ok())
            {
                --figure.y;
                apply_figure();
                figure = null;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           
            if (figure == null)
            {
                figure = new Figure();
                if (!figure_ok())
                {
                    timer1.Enabled = false;
                    MessageBox.Show("Game Over!");        ///////////////////////////
                    Form3 f3 = new Form3();
                    f3.Show();
                    f3.Owner = this;
                }
            }
            
            figure_down();
            panel1.Refresh();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (!started) return;                                      //НАЧАЛО РИСОВАНИЯ ТЕТРИСА
            gr = e.Graphics;
            gr.DrawRectangle(Pens.LightBlue, new Rectangle(0, 0, ps * MAXX + 4, ps * MAXY + 4));
            gr.FillRectangle(Brushes.Black, new Rectangle(1, 1, ps * MAXX + 3, ps * MAXY + 3));
            draw_field();
            if (figure != null) draw_figure(figure);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //e.KeyCode+=Keys.Alt
            if (figure == null) return;
            if (e.KeyCode == Keys.Left)
            {
                figure.x -= 1;
                if (!figure_ok()) figure.x += 1;
            }
            else if (e.KeyCode == Keys.Right)
            {
                figure.x += 1;
                if (!figure_ok()) figure.x -= 1;
            }
            else if (e.KeyCode == Keys.Down)
            {
                figure.y += 1;
                if (!figure_ok()) figure.y -= 1;
            }
            else if (e.KeyCode == Keys.Up)
            {
                figure.rotate();
                if (!figure_ok())
                {
                    for (int i = 0; i < 3; ++i) figure.rotate();
                }
            }
            panel1.Refresh();
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            Form1_KeyDown(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            f2.Owner = this;
            panel1.enableDB();
        }

        private void panel1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
          
        }

        private void button1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Up:
                case Keys.Left:
                case Keys.Right:
                    e.IsInputKey = true;
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = stopPlay;
            stopPlay = !stopPlay;
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            f2.Show();
        }
        public void startCheckGrid()
        {
            checkGrid();
        }
        public void checkGrid()
        {
            int i = 0,j=0;
            bool startChecked = true;
            
          
            for (i = 0; i < 10; i++)
            {
                if ((f2.players[9 - i].score > score) && startChecked == true)
                {
                    break;
                }
                else if (f2.players[9 - i].score < score)
                {
                    startChecked = false;

                    if (i==9) {
                        f2.players[0] = new player(playerName, score);
                        f2.updateDataGrid();
                        break;
                    }

                    else continue;
                }
                else if ((f2.players[9 - i].score >= score) && startChecked == false)
                {
                    f2.players[9-i] = new player(playerName, score);
                    f2.updateDataGrid();
                    break;
                }

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            XmlSerializer ser = new XmlSerializer(typeof(playersCollection));
            FileStream fs = new FileStream("save.xml", FileMode.Create, FileAccess.Write);
            ser.Serialize(fs, f2.players);
            fs.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Клавиши управления:\n\n\n←  переместить фигуру влево\n→ переместить фигуру вправо\n↑ повернуть фигуру\n↓ ускорить фигуру","Управление",MessageBoxButtons.OK,MessageBoxIcon.Question);
        }
    }

    public class Figure
    {
        string[] nya = {"....1111........", "....111.1.......", ".....11..11.....", ".1...11...1.....",
            "....111..1......", "....111...1.....", "..1..11..1......"};
        public int x, y;
        public int clr;
        public int[,] pix;
        public Random rnd;
        public Figure()
        {
            x = 3; y = -2;
            rnd = new Random();
            clr = 1 + rnd.Next(9);
            int n = rnd.Next(nya.Length);
            fillpix(out pix, nya[n]);
            n = rnd.Next(4);
            for (int i = 0; i < n; ++i)
                rotate();
        }
        void fillpix(out int[,] pix, string cc)
        {
            pix = new int[4, 4];
            int i, j;
            for (i = 0; i < 4; ++i)
                for (j = 0; j < 4; ++j)
                {
                    if (cc[j * 4 + i] == '1') pix[i, j] = clr;
                    else pix[i, j] = 0;
                }
        }
        public void rotate()
        {
            int[,] p2 = new int[4, 4];
            int i, j;
            for (i = 0; i < 4; ++i)
                for (j = 0; j < 4; ++j)
                {
                    p2[3 - j, i] = pix[i, j];
                }
            pix = p2;
        }

        
    }

}