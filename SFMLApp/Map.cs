using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace SFMLApp
{
    public class Entity
    {
        public int r { get; private set; }
        public double x { get; set; }
        public double y { get; set; }
        public int Tag { get; private set; }
        public Entity(int Tag, double x, double y, int r)
        {
            this.r = r;
            this.x = x;
            this.y = y;
            this.Tag = Tag;
        }
    }
    public class MPlayer : Entity
    {
        public MPlayer(int Tag, double x, double y)
            : base(Tag, x, y, Map.RPlayer)
        { }
    }
    public class MArrow : Entity
    {
        public MArrow(int Tag, double x, double y)
            : base(Tag, x, y, Map.RArrow)
        {}
        public static MArrow Load(string save)
        {
            string[] args = save.Split().ToArray();
            int Tag = int.Parse(args[0]);
            double x = double.Parse(args[1]), y = double.Parse(args[2]);
            MArrow Ar = new MArrow(Tag, x, y);
            return Ar;
        }
        public override string ToString()
        {
            return this.Tag + " " + this.x + " " + this.y;
        }
        public string getData()
        {
            StringBuilder ans = new StringBuilder();
            ans.Append(this.Tag);
            ans.Append(".");
            ans.Append(this.x);
            ans.Append(".");
            ans.Append(this.y);
            ans.Append(".");
            ans.Append(this.r);
            return ans.ToString();
        }
    }
    public class Square
    {
        public int x { get; private set; }
        public int y { get; private set; }
        public bool isEmpty { get; private set; }
        public Square(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.isEmpty = true;
        }
        public Square(int x, int y, bool b)
        {
            this.x = x;
            this.y = y;
            this.isEmpty = b;
        }
    }
    public class Map
    {
        public static int RPlayer = 10;
        public static int Rwidth = 10;
        public static int RArrow = 5;
        public static int RDrop = 10;
        public string Name;

        private Stopwatch Timer;
        private int width, height;
        public int Pheight { get; private set; }
        public int Pwidth { get; private set; }
        public Dictionary<int, MPlayer> players { get; private set; }
        public Dictionary<int, MArrow> arrows { get; private set; }
        public List<List<Square>> Field { get; private set; }
        public Dictionary<int, MDrop> drops { get; private set; }

        public void SpawnDrop(int Tag, double x, double y)
        {
            this.drops.Add(Tag, new MDrop(Tag, x, y));
        }
        public void SpawnPlayer(int Tag, double x,double y)
        {
            this.players.Add(Tag, new MPlayer(Tag, x, y));
        }
        public void readData(string path)
        {
            string[] parms = path.Split(';');
            this.Name = parms[0];
            string[] dataDrops = parms[1].Split(',');
            string[] dataArrows = parms[2].Split(',');
            string[] dataPlayers = parms[3].Split(',');
            this.arrows.Clear();
            this.drops.Clear();
            this.players.Clear();
            if(dataDrops[0]!="")
            foreach (var item in dataDrops)
            {
                string[] dataDrop = item.Split('.');
                int Tag = int.Parse(dataDrop[0]), r = int.Parse(dataDrop[3]);
                double x = double.Parse(dataDrop[1]), y = double.Parse(dataDrop[2]);
                drops.Add(Tag, new MDrop(Tag, x, y));
                //drops[Tag].r = r;
            }
            if(dataArrows[0]!="")
            foreach (var item in dataArrows)
            {
                string[] dataArrow = item.Split('.');
                int Tag = int.Parse(dataArrow[0]), r = int.Parse(dataArrow[3]);
                double x = double.Parse(dataArrow[1]), y = double.Parse(dataArrow[2]);
                arrows.Add(Tag, new MArrow(Tag, x, y));
                //drops[Tag].r = r;
            }
            if(dataPlayers[0]!="")
            foreach (var item in dataPlayers)
            {
                string[] dataPlayer = item.Split('.');
                int Tag = int.Parse(dataPlayer[0]), r = int.Parse(dataPlayer[3]);
                double x = double.Parse(dataPlayer[1]), y = double.Parse(dataPlayer[2]);
                players.Add(Tag, new MPlayer(Tag, x, y));
                //players[Tag].r = r;
            }
        }
        public void LoadMap(string path)
        {
            using (StreamReader sr = File.OpenText(path))
            {
                this.arrows.Clear();
                this.drops.Clear();
                this.players.Clear();
                string s = "";
                List<string> args = new List<string>();
                while ((s = sr.ReadLine()) != null)
                {
                    args.Add(s);
                }
                int[] tmp = args[0].Split().Select(x => int.Parse(x)).ToArray();
                this.width = tmp[0]; this.height = tmp[1];
                int nowline = 6 + int.Parse(args[4])+int.Parse(args[5+int.Parse(args[4])]);
                tmp = args[nowline].Split().Select(x => int.Parse(x)).ToArray();
                this.Pwidth = tmp[0] + 2; this.Pheight = tmp[1] + 2;
                this.Field = new List<List<Square>>();
                for (int x = 0; x < this.Pwidth; ++x)
                    this.Field.Add(new List<Square>());

                for (int i = 0; i < this.Pwidth; i++)
                    this.Field[i].Add(new Square(i, 0, false));
                for (int i = 1; i < this.Pheight - 1; i++)
                {
                    this.Field[0].Add(new Square(0, i, false));
                    this.Field[this.Pwidth - 1].Add(new Square(this.Pwidth - 1, i, false));
                }
                for (int y = 1; y < this.Pheight - 1; ++y)
                {
                    string line = args[y + nowline];
                    bool[] bol = line.Split().Select(x => x == "0").ToArray();
                    for (int x = 1; x < Pwidth - 1; ++x)
                        this.Field[x].Add(new Square(x, y, bol[x - 1]));
                }
                for (int i = 0; i < this.Pwidth; i++)
                    this.Field[i].Add(new Square(i, this.Pheight - 1, false));
            }
        }
        public void SaveMap(string path)
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(this.Name);
                sw.WriteLine(this.width + " " + this.height);
                sw.WriteLine(this.players.Count);
                foreach (var item in this.players)
                {
                    sw.WriteLine(item.Value.Tag+" "+item.Value.x+" "+item.Value.y);
                }
                sw.WriteLine(this.arrows.Count);
                foreach (var item in this.arrows)
                {
                    sw.WriteLine(item.Value.Tag + " " + item.Value.x + " " + item.Value.y);
                }
                sw.WriteLine(this.drops.Count);
                foreach (var item in this.drops)
                {
                    sw.WriteLine(item.Value.Tag + " " + item.Value.x + " " + item.Value.y);
                }
                sw.WriteLine(Pwidth + " " + Pheight);
                for (int y = 0; y < Pheight; ++y)
                {
                    for (int x = 0; x < Pwidth - 1; ++x)
                        if (Field[x][y].isEmpty)
                            sw.Write("0 ");
                        else
                            sw.Write("1 ");
                    if (Field[Pwidth - 1][y].isEmpty)
                        sw.WriteLine("0");
                    else
                        sw.WriteLine("1");
                }

            }
        }
        private Square getSquare(double x, double y)
        {
            return Field[(int)Math.Floor(x) / Rwidth][(int)Math.Floor(y) / Rwidth];
        }
        private Square getSquare(int x, int y)
        {
            return Field[x][y];
        }
        public Map(int width, int height)
        {
            this.Name = "DefaultNameMap";
            this.width = width;
            this.height = height;
            this.Pwidth = width / Rwidth;
            this.Pheight = height / Rwidth;
            this.players = new Dictionary<int, MPlayer>();
            this.arrows = new Dictionary<int, MArrow>();
            this.Field = new List<List<Square>>();
            this.drops = new Dictionary<int, MDrop>();
            for (int x = 0; x < Pwidth; ++x)
                this.Field.Add(new List<Square>());
            for (int x = 0; x < Pwidth; ++x)
                for (int y = 0; y < Pheight; ++y)
                    this.Field[x].Add(new Square(x, y));
            this.Timer = new Stopwatch();
        }
        public Map(string path)
        {
            this.Timer = new Stopwatch();
            this.arrows = new Dictionary<int, MArrow>();
            this.drops = new Dictionary<int, MDrop>();
            this.players = new Dictionary<int, MPlayer>();
            this.LoadMap(path);
            this.Name = path;
        }
        public void Pause()
        {
            Timer.Stop();
        }
        public void Run()
        {
            Timer.Start();
        }
    }
    public class MDrop : Entity
    {
        public MDrop(int Tag, double x, double y)
            : base(Tag, x, y, Map.RDrop)
        {}
        public MDrop(int Tag, double x, double y, int NumSpawner)
            : base(Tag, x, y, Map.RDrop)
        {}
        public override string ToString()
        {
            return this.Tag + " " + this.x + " " + this.y;
        }
    }

}