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
using System.Xml.Serialization;

namespace tetris
{
    public partial class Form2 : Form
    {
        public DataGridView a = new DataGridView();
        public playersCollection players;
        public Form2()
        {
            InitializeComponent();
            ControlBox = false;
            dataGridView1.RowCount = 10;
            players= new playersCollection();

            if (File.Exists("save.xml"))
            {
                XmlSerializer ser = new XmlSerializer(typeof(playersCollection));
                FileStream fs = new FileStream("save.xml", FileMode.Open, FileAccess.Read);
                players = ser.Deserialize(fs) as playersCollection;
                fs.Close();
            }
            updateDataGrid();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            
        }
        public void updateDataGrid()
        {
            for (int i = 0; i < 10; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = players[i].name;
                dataGridView1.Rows[i].Cells[1].Value = players[i].score.ToString();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
    [XmlRoot("collection")]
    [Serializable]
    public class playersCollection
    {
        [XmlArray("playersCollection")]
        [XmlArrayItem("player")]
        public player[] players = new player[10];
        public playersCollection()
        {

            string[] arrayStr = new string[]
            {

            "Винни пух",
            "Пяточок",
             "Леопольд",
            "Алеша Попович",
            "Колобок",
            "Иван Дурак",
             "Попугай Кеша",
             "Чебурашка",
            "Карлсон",
            "Незнайка" };
            for (int i = 0; i < 10; i++) players[i] = new player(arrayStr[i], (9 - i) * 100 + 500);


        }
        public player this[int index]
        {
            set
            {
                for (int j = 9; j > (index==0 ? index: (index+1)>9?9:index+1); j--)
                {
                    players[j] = players[j - 1];

                }
                players[index == 0 ? index : (index + 1) > 9 ? 9 : index + 1] = value;
            }
            get
            {
                return players[index];
            }
        }
    }
    public class player
    {
        public string name="";
       public int score=0;
        public player() { }
        public player(string name,int score)
        {
            this.name = name;
            this.score = score;
        }
    }
}
