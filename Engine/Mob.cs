using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Mob : LivingCreature
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int MaxDmg { get; set; }
        public int XPReward { get; set; }
        public int GoldReward { get; set; }
        public List<LootItem> LootTable { get; set; }
        public bool Boss { get; set; }

        public Mob(int id, string name, int maxDmg, int xPReward, int godlReward, int currentHP, int maxHP, bool boss) : base(currentHP, maxHP)
        {
            ID = id;
            Name = name;
            MaxDmg = maxDmg;
            XPReward = xPReward;
            GoldReward = godlReward;
            LootTable = new List<LootItem>();
            Boss = boss;
            
        }
    }
}
