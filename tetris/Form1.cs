using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;

namespace tetris
{
    public partial class Form1 : Form
    {
        const int MAXX = 10;
        const int MAXY = 15;
        const int ps = 24;
        bool started = false;
        Brush[] bru = {Brushes.White, Brushes.Orange, Brushes.Blue, Brushes.Red, Brushes.Green, Brushes.Honeydew,
            Brushes.Violet, Brushes.Tomato, Brushes.SteelBlue, Brushes.PapayaWhip};
        
        int score;
        int[,] ar;
        Figure figure;
        Graphics gr;
        SoundPlayer simpleSound = new SoundPlayer("chimes.wav");

        public Form1()
        {
            InitializeComponent();
        }

        private void upd_score()
        {
            label1.Text = score.ToString();
        }

        private void draw(int x, int y, int clr)
        {
            if (clr == 0 || x < 0 || x >= MAXX || y < 0 || y >= MAXY) return;
            gr.FillEllipse(bru[clr], new Rectangle(2 + x * ps, 2 + y * ps, ps, ps));
        }
        
        private void draw_field()
        {
            int i, j;
            for (i = 0; i < MAXX; ++i)
                for (j = 0; j < MAXY; ++j)
                    draw(i, j, ar[i, j]);
        }

        private void draw_figure(Figure f)
        {
            int i, j;
            for (i = 0; i < 4; ++i)
                for (j = 0; j < 4; ++j)
                    draw(f.x + i, f.y + j, f.pix[i, j]);
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
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
                int sc = (goo * (goo + 1) / 2) * 100;
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
                    MessageBox.Show("Game Over!");
                }
            }
            figure_down();
            panel1.Refresh();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (!started) return;
            gr = e.Graphics;
            gr.DrawRectangle(Pens.LightBlue, new Rectangle(0, 0, ps * MAXX + 4, ps * MAXY + 4));
            gr.FillRectangle(Brushes.Black, new Rectangle(1, 1, ps * MAXX + 3, ps * MAXY + 3));
            draw_field();
            if (figure != null) draw_figure(figure);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (figure == null) return;
            if (e.KeyCode == Keys.NumPad4)
            {
                figure.x -= 1;
                if (!figure_ok()) figure.x += 1;
            }
            else if (e.KeyCode == Keys.NumPad6)
            {
                figure.x += 1;
                if (!figure_ok()) figure.x -= 1;
            }
            else if (e.KeyCode == Keys.NumPad2)
            {
                figure.y += 1;
                if (!figure_ok()) figure.y -= 1;
            }
            else if (e.KeyCode == Keys.NumPad8)
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
            panel1.enableDB();
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