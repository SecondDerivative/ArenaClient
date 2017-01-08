using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFMLApp
{
    public class Arena
    {
        //public const int WaitRespawnDrop = 30000;
        public Map map { get; private set; }
        public Dictionary<int, Player> players { get; private set; }
        public Dictionary<int, AArrow> Arrows { get; private set; }
        public Dictionary<int, ADrop> Drops { get; private set; }
        public Dictionary<int, APlayer> ArenaPlayer { get; private set; }
        //private Stopwatch timer;
        //Queue<Tuple<long, int>> DropForRespawn;

        public Arena()
        {
            ArenaPlayer = new Dictionary<int, APlayer>();
            players = new Dictionary<int, Player>();
            Arrows = new Dictionary<int, AArrow>();
            Drops = new Dictionary<int, ADrop>();
          //  timer = new Stopwatch();
        //    DropForRespawn = new Queue<Tuple<long, int>>();
        }

        public void NewMap(string name)
        {
            map = new Map("./data/Maps/" + name + ".txt");
            foreach (var i in players)
            {
                map.AddPlayer(i.Key);
                //players[i.Key].respawn();
            }
            Arrows.Clear();
            Drops.Clear();
          //  DropForRespawn.Clear();
            for (int i = 0; i < map.dropSpawners.Count; ++i)
            {
                int tag = Utily.GetTag();
                map.SpawnDrops(i, tag);
                Drops.Add(tag, new ADrop(map.dropSpawners[i].count, map.dropSpawners[i].id));
            }
            //timer.Restart();
        }
        /*
        public void MovePlayer(int tag, Tuple<double, double> speed)
        {
            double m = players[tag].Speed();
            Tuple<double, double> NewVect = Utily.Normalizing(speed, players[tag].Speed());
            map.MovePlayer(tag, NewVect);
        }
        */
        /*
        public int FirePlayer(int tag, Tuple<double, double> vect)
        {
            int dmg = players[tag].attack();
            if (dmg <= 0)
                return -1;
            Tuple<double, double> NewVect = Utily.Normalizing(vect, players[tag].ArrowSpeed());
            int arTag = Utily.GetTag();
            Arrows[arTag] = new AArow(tag, dmg, players[tag].inventory.getCurrentArrow().id);
            map.FirePlayer(tag, arTag, NewVect.Item1, NewVect.Item2);
            return arTag;
        }
        */
        /*
        public void StopPlayer(int tag)
        {
            map.StopPlayer(tag);
        }
        */
        public void Update()
        {
            map.UpDate();
          /*  MEvent Event;
            while ((Event = map.NextEvent()) != null)
            {
                if (Event.Type == MEvents.PlayerArrow)
                {
                    int TagArrow = ((MEventArrowHit)Event).TagArrow;
                    int TagPlayer = ((MEventArrowHit)Event).TagPlayer;
                    players[TagPlayer].recieveDamage(Arrows[TagArrow].dmg);
                    if (players[TagPlayer].isDead())
                    {
                        ArenaPlayer[Arrows[TagArrow].creater].AddKill();
                        ArenaPlayer[TagPlayer].AddDeath();
                        int NewTag = Utily.GetTag();
                        map.SpawnDrops(NewTag, map.players[TagPlayer].x, map.players[TagPlayer].y);
                        map.SpawnPlayer(TagPlayer);
                        Drops.Add(NewTag, new ADrop(1, players[TagPlayer].rightHand));
                        players[TagPlayer].respawn();
                    }
                    Arrows.Remove(TagArrow);
                }
                if (Event.Type == MEvents.PlayerDrop)
                {
                    var drop = Drops[((MEventDrop)Event).TagDrop];
                    players[((MEventDrop)Event).TagPlayer].pickedUpItem(drop.id, drop.Count);
                    Drops.Remove(((MEventDrop)Event).TagDrop);
                    if (((MEventDrop)Event).BySpawner)
                    {
                        DropForRespawn.Enqueue(Utily.MakePair<long, int>(timer.ElapsedMilliseconds, ((MEventDrop)Event).NumSpawner));
                    }
                }
                if (Event.Type == MEvents.DestroyArrow)
                {
                    Arrows.Remove(((MEventDestroyArrow)Event).TagArrow);
                }
            }
            while (DropForRespawn.Count > 0 && DropForRespawn.Peek().Item1 + WaitRespawnDrop < timer.ElapsedMilliseconds)
            {
                int num = DropForRespawn.Dequeue().Item2;
                int tag = Utily.GetTag();
                map.SpawnDrops(num, tag);
                Drops.Add(tag, new ADrop(map.dropSpawners[num].count, map.dropSpawners[num].id));
            }*/
        }

        public bool TagIsUse(int tag)
        {
            return false;//need change;;
        }
        /*
        public int AddPlayer(string name)
        {
            int tag = Utily.GetTag();
            players[tag] = new Player();
            players[tag].respawn();
            ArenaPlayer[tag] = new APlayer(name);
            map.AddPlayer(tag);
            map.SpawnPlayer(tag);
            return tag;
        }
        public void RemovePlayer(int tag)
        {
            if (!TagIsUse(tag))
                return;
            map.RemovePlayer(tag);
            players.Remove(tag);
            ArenaPlayer.Remove(tag);
        }
        */
        /*public void ChangeItem(int tagPlayer, int type)
        {
            if (type == 1)
                players[tagPlayer].NextItem();
            else
                players[tagPlayer].PrevItem();
        }
        public void ChangeArrow(int tagPlayer, int type)
        {
            if (type == 1)
                players[tagPlayer].NextArrow();
            else
                players[tagPlayer].PrevArrow();
        }
        */
        public void Pause()
        {
            //map.Pause();
            //timer.Stop();
        }
        public void Run()
        {
            //map.Run();
            //timer.Start();
        }
        public void TakeString(string Arrow, string Drop, string Player)
        {
            var arr = Arrow.Split(',');
            HashSet<int> IsUsed = new HashSet<int>();
            for (int i = 0; i < arr.Length; i++)
            {
                var small = arr[i].Split(' ');
                int tag = Utily.Parse(small[0]);
                int dmg = Utily.Parse(small[1]);
                int id = Utily.Parse(small[2]);
                if (Arrows.ContainsKey(tag))
                {
                    Arrows[tag].dmg = dmg;
                    Arrows[tag].id = id;
                    IsUsed.Add(tag);
                }
                else
                    Arrows.Add(tag, new AArrow(dmg, id));
            }
            List<int> ForRemove = new List<int>();
            foreach (var i in Arrows)
            {
                if (!IsUsed.Contains(i.Key))
                    ForRemove.Add(i.Key);
            }
            foreach (var i in ForRemove)
                Arrows.Remove(i);

            var dro = Drop.Split(',');
            IsUsed.Clear();
            for (int i = 0; i < arr.Length; i++)
            {
                var small = dro[i].Split(' ');
                int tag = Utily.Parse(small[0]);
                int cnt = Utily.Parse(small[1]);
                int id = Utily.Parse(small[2]);
                if (Drops.ContainsKey(tag))
                {
                    Drops[tag].Count = cnt;
                    Drops[tag].id = id;
                    IsUsed.Add(tag);
                }
                else
                    Drops.Add(tag, new ADrop(cnt, id));
            }
            ForRemove.Clear();
            foreach (var i in Drops)
            {
                if (!IsUsed.Contains(i.Key))
                    ForRemove.Add(i.Key);
            }
            foreach (var i in ForRemove)
                Drops.Remove(i);

            var pla = Player.Split(',');
            IsUsed.Clear();
            for (int i = 0; i < arr.Length; i++)
            {
                var small = pla[i].Split(' ');
                int tag = Utily.Parse(small[0]);
                int kill = Utily.Parse(small[1]);
                int death = Utily.Parse(small[2]);
                if (ArenaPlayer.ContainsKey(tag))
                {
                    ArenaPlayer[tag].RealName = small[3];
                    IsUsed.Add(tag);
                }
                else
                    ArenaPlayer.Add(tag, new APlayer(small[3]));
                ArenaPlayer[tag].Kill = kill;
                ArenaPlayer[tag].Death = death;
            }
            ForRemove.Clear();
            foreach (var i in ArenaPlayer)
            {
                if (!IsUsed.Contains(i.Key))
                    ForRemove.Add(i.Key);
            }
            foreach (var i in ForRemove)
                ArenaPlayer.Remove(i);
        }
        public int TakeAllString(string s)
        {
            var BigBlock = s.Split('#');
            //map.TakeString(BigBlock[0]);
            TakeString(BigBlock[1], BigBlock[2], BigBlock[3]);
            var pla = BigBlock[4].Split(';');
            HashSet<int> IsUsed = new HashSet<int>();
            for (int i = 0; i < pla.Length; i++)
            {
                int ind = pla[i].IndexOf(' ');
                int tag = Utily.Parse(pla[i].Substring(0, ind));
                if (!players.ContainsKey(tag))
                    players.Add(tag, new Player());
                else
                    IsUsed.Add(tag);
                players[tag].TakeSmallString(pla[i].Substring(ind + 1));
            }
            int MainPlayerTag;
            {
                int ind = BigBlock[5].IndexOf(' ');
                int tag = Utily.Parse(BigBlock[5].Substring(0, ind));
                if (!players.ContainsKey(tag))
                    players.Add(tag, new Player());
                else
                    IsUsed.Add(tag);
                players[tag].TakeLargeString(BigBlock[5].Substring(ind + 1));
                MainPlayerTag = tag;
            }
            List<int> ForRemove = new List<int>();
            foreach (var i in players)
            {
                if (!IsUsed.Contains(i.Key))
                    ForRemove.Add(i.Key);
            }
            foreach (var i in ForRemove)
                players.Remove(i);
            return MainPlayerTag;
        }
        /*
        public Dictionary<int, string> GetAllInfo()
        {
            var ans = new Dictionary<int, string>();
            string mapinfo = map.getData();
            var small = new Dictionary<int, string>();
            foreach (var i in players)
                small.Add(i.Key, i.Value.SmallString());
            string arenainfo = GetInfo();
            foreach (var i in players)
            {
                StringBuilder now = new StringBuilder();
                now.Append(mapinfo);
                now.Append("#");
                now.Append(arenainfo);
                now.Append("#");
                bool wasWrite = false;
                foreach (var j in players)
                {
                    if (j.Key != i.Key)
                    {
                        if (wasWrite)
                            now.Append(";");
                        else
                            wasWrite = false;
                        now.Append(j.Key);
                        now.Append(" ");
                        now.Append(small);
                    }
                }
                now.Append("#");
                now.Append(i.Value.LargeString());
                ans.Add(i.Key, now.ToString());
            }
            return ans;
        }
        public string GetInfo()
        {
            StringBuilder ans = new StringBuilder();
            bool wasWrite = false;
            foreach (var j in Arrows)
            {
                if (map.arrows.ContainsKey(j.Key) && map.arrows[j.Key].Exist)
                    if (wasWrite)
                        ans.Append(",");
                    else
                        wasWrite = true;
                ans.Append(j.Key);
                ans.Append(" ");
                ans.Append(j.Value.GetInfo());
            }
            ans.Append("#");
            wasWrite = false;
            foreach (var j in Drops)
            {
                if (map.drops.ContainsKey(j.Key) && map.drops[j.Key].Exist)
                    if (wasWrite)
                        ans.Append(",");
                    else
                        wasWrite = true;
                ans.Append(j.Key);
                ans.Append(" ");
                ans.Append(j.Value.GetInfo());
            }
            ans.Append("#");
            wasWrite = false;
            foreach (var j in ArenaPlayer)
            {
                if (map.players.ContainsKey(j.Key) && map.players[j.Key].Exist)
                    if (wasWrite)
                        ans.Append(",");
                    else
                        wasWrite = true;
                ans.Append(j.Key);
                ans.Append(" ");
                ans.Append(j.Value.GetInfo());
            }
            return ans.ToString();
        }
        */
    }

    public class AArrow
    {
        public int dmg { get; set; }
        public int id { get; set; }

        public AArrow(int dmg, int id)
        {
            this.dmg = dmg;
            this.id = id;
        }
        /*
        public string GetInfo()
        {
            return dmg + " " + id;
        }
        */
    }

    public class ADrop
    {
        public int Count { get; set; }
        public int id { get; set; }
        public ADrop(int cnt, int id)
        {
            Count = cnt;
            this.id = id;
        }
        /*public string GetInfo()
        {
            return Count + " " + id;
        }
        */
    }

    public class APlayer
    {
        public int Kill { get; set; }
        public int Death { get; set; }
        public string RealName { get; set; }
        public APlayer(string name)
        {
            RealName = name;
            Kill = Death = 0;
        }
        public void AddKill()
        {
            ++Kill;
        }
        public void AddDeath()
        {
            ++Death;
        }
        /*public string GetInfo()
        {
            return Kill + " " + Death + " " + RealName;
        }
        */
    }
}
