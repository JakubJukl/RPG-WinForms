using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class World
    {
        public static readonly List<Item> Items = new List<Item>();
        public static readonly List<Mob> Mobs = new List<Mob>();
        public static readonly List<Quest> Quests = new List<Quest>();
        public static readonly List<Location> Locations = new List<Location>();

        public const int ITEM_ID_RUSTY_SWORD = 1;
        public const int ITEM_ID_RAT_TAIL = 2;
        public const int ITEM_ID_PIECE_OF_RAT_FUR = 3;
        public const int ITEM_ID_SNAKE_FANG = 4;
        public const int ITEM_ID_SNAKESKIN = 5;
        public const int ITEM_ID_CLUB = 6;
        public const int ITEM_ID_HEALING_POTION = 7;
        public const int ITEM_ID_SPIDER_FANG = 8;
        public const int ITEM_ID_SPIDER_SILK = 9;
        public const int ITEM_ID_ADVENTURER_PASS = 10;

        public const int MOB_ID_RAT = 1;
        public const int MOB_ID_SNAKE = 2;
        public const int MOB_ID_GIANT_SPIDER = 3;

        public const int QUEST_ID_CLEAR_ALCHEMIST_GARDEN = 1;
        public const int QUEST_ID_CLEAR_FARMERS_FIELD = 2;
        public const int QUEST_ID_BRING_ME_THEIR_SKINS = 3;

        public const int LOCATION_ID_HOME = 1;
        public const int LOCATION_ID_TOWN_SQUARE = 2;
        public const int LOCATION_ID_GUARD_POST = 3;
        public const int LOCATION_ID_ALCHEMIST_HUT = 4;
        public const int LOCATION_ID_ALCHEMISTS_GARDEN = 5;
        public const int LOCATION_ID_FARMHOUSE = 6;
        public const int LOCATION_ID_FARM_FIELD = 7;
        public const int LOCATION_ID_BRIDGE = 8;
        public const int LOCATION_ID_SPIDER_FIELD = 9;
        public const int LOCATION_ID_SHININING_VALLEY = 10;

        static World()
        {
            PopulateItems();
            PopulateMobs();
            PopulateQuests();
            PopulateLocations();
        }

        private static void PopulateItems()
        {
            Items.Add(new Weapon(ITEM_ID_RUSTY_SWORD, "Rezavý meč", "Rezavé meče", 0, 5));
            Items.Add(new Item(ITEM_ID_RAT_TAIL, "Krysí ocas", "Krysí ocasy"));
            Items.Add(new Item(ITEM_ID_PIECE_OF_RAT_FUR, "Kus krysí kůže", "Kusy krysí kůže"));
            Items.Add(new Item(ITEM_ID_SNAKE_FANG, "Hadí zub", "Hadí zuby"));
            Items.Add(new Item(ITEM_ID_SNAKESKIN, "Hadí kůže", "Hadí kůže"));
            Items.Add(new Weapon(ITEM_ID_CLUB, "Pálka", "Pálky", 3, 10));
            Items.Add(new HealingPotion(ITEM_ID_HEALING_POTION, "Healovací nápoj", "Healovací nápoje", 5));
            Items.Add(new Item(ITEM_ID_SPIDER_FANG, "Pavoučí zub", "Pavoučí zuby"));
            Items.Add(new Item(ITEM_ID_SPIDER_SILK, "Pavoučí sukno", "Pavoučí sukna"));
            Items.Add(new Item(ITEM_ID_ADVENTURER_PASS, "Průvodce dobrodruha", "Průvodcové dobrodruha"));
            
        }

        private static void PopulateMobs()
        {
            Mob rat = new Mob(MOB_ID_RAT, "Krysa", 5, 3, 10, 3, 3, false);
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_RAT_TAIL), 75, false));
            rat.LootTable.Add(new LootItem(ItemByID(ITEM_ID_PIECE_OF_RAT_FUR), 75, true));

            Mob snake = new Mob(MOB_ID_SNAKE, "Had", 5, 3, 10, 3, 3, false);
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKE_FANG), 75, false));
            snake.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SNAKESKIN), 75, true));

            Mob giantSpider = new Mob(MOB_ID_GIANT_SPIDER, "Obrovský pavouk (Boss)", 20, 5, 40, 10, 10, true);
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_FANG), 75, true));
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_SPIDER_SILK), 25, false));
            giantSpider.LootTable.Add(new LootItem(ItemByID(ITEM_ID_CLUB), 100, false));

            Mobs.Add(rat);
            Mobs.Add(snake);
            Mobs.Add(giantSpider);
        }

        private static void PopulateQuests()
        {
            Quest clearAlchemistGarden =
                new Quest(
                    QUEST_ID_CLEAR_ALCHEMIST_GARDEN,
                    "Vyčisti Zahradu Alchymisty",
                    "Zabij krysy v Alchymistově Zahradě a přines mi 3 krysí ocasy. Dostaneš za svou odvahu healovací nápoj a 10 zlatých.", 20, 10);

            clearAlchemistGarden.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_RAT_TAIL), 3));

            clearAlchemistGarden.RewardItem = ItemByID(ITEM_ID_HEALING_POTION);

            Quest clearFarmersField =
                new Quest(
                    QUEST_ID_CLEAR_FARMERS_FIELD,
                    "Vyčisti Farmářovo Pole",
                    "Zabij hady na Farmářově Poli a dones mi jako důkaz 3 hadí zuby. Odměním tě za to Průvodcem Dobrodruha a 20 zlatými.", 20, 20);

            clearFarmersField.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_SNAKE_FANG), 3));

            clearFarmersField.RewardItem = ItemByID(ITEM_ID_ADVENTURER_PASS);

            Quest bringmetheirskins =
                new Quest(
                    QUEST_ID_BRING_ME_THEIR_SKINS,
                    "Přines mi jejich kůže!",
                    "Zabij hady na Farmářově Poli a krysy v Alchymistově Zahradě a dones mi 20 hadích kůží a 20 kusů krysí kůže. ZN: Odměna jistá.", 200, 0);

            bringmetheirskins.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_SNAKESKIN), 20));
            bringmetheirskins.QuestCompletionItems.Add(new QuestCompletionItem(ItemByID(ITEM_ID_PIECE_OF_RAT_FUR), 20));

            Quests.Add(clearAlchemistGarden);
            Quests.Add(clearFarmersField);
            Quests.Add(bringmetheirskins);
        }

        private static void PopulateLocations()
        {
            // Create each location
            Location home = new Location(LOCATION_ID_HOME, "Domov", "Tvůj dům, měl by sis tady vážně uklidit.");

            Location townSquare = new Location(LOCATION_ID_TOWN_SQUARE, "Náměstí", "Vidíš fontánu.");

            Location alchemistHut = new Location(LOCATION_ID_ALCHEMIST_HUT, "Alchymistova Bouda", "Na poličkách je spousta podivných rostlin.");
            alchemistHut.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_ALCHEMIST_GARDEN);

            Location alchemistsGarden = new Location(LOCATION_ID_ALCHEMISTS_GARDEN, "Alchymistova Zahrada", "Roste tady spousta rostlin.");
            alchemistsGarden.MonsterLivingHere = MobByID(MOB_ID_RAT);

            Location farmhouse = new Location(LOCATION_ID_FARMHOUSE, "Dům Farmáře", "Je tu malý dům s farmářem, který stojí před ním.");
            farmhouse.QuestAvailableHere = QuestByID(QUEST_ID_CLEAR_FARMERS_FIELD);

            Location farmersField = new Location(LOCATION_ID_FARM_FIELD, "Farmářovo Pole", "Rostou tu řádky zeleniny.");
            farmersField.MonsterLivingHere = MobByID(MOB_ID_SNAKE);

            Location guardPost = new Location(LOCATION_ID_GUARD_POST, "Strážnice", "Je tady obrovský, hrozivě vypadající strážný.", ItemByID(ITEM_ID_ADVENTURER_PASS));
            guardPost.QuestAvailableHere = QuestByID(QUEST_ID_BRING_ME_THEIR_SKINS);

            Location bridge = new Location(LOCATION_ID_BRIDGE, "Most", "Kamenný most přes širokou řeku.");

            Location spiderField = new Location(LOCATION_ID_SPIDER_FIELD, "Les", "Vidíš pavoučí sítě zakrývající stromy. Co tu asi může žít za potvoru?");
            spiderField.MonsterLivingHere = MobByID(MOB_ID_GIANT_SPIDER);

            Location shiningValley = new Location(LOCATION_ID_SHININING_VALLEY, "Zářící údolí", "Vidíš krásné sluneční odlesky z údolí, které se před tebeou rozkládá.");
            shiningValley.KillRequiredToEnter = true;

            // Link the locations together
            home.LocationToNorth = townSquare;

            townSquare.LocationToNorth = alchemistHut;
            townSquare.LocationToSouth = home;
            townSquare.LocationToEast = guardPost;
            townSquare.LocationToWest = farmhouse;

            farmhouse.LocationToEast = townSquare;
            farmhouse.LocationToWest = farmersField;

            farmersField.LocationToEast = farmhouse;

            alchemistHut.LocationToSouth = townSquare;
            alchemistHut.LocationToNorth = alchemistsGarden;

            alchemistsGarden.LocationToSouth = alchemistHut;

            guardPost.LocationToEast = bridge;
            guardPost.LocationToWest = townSquare;

            bridge.LocationToWest = guardPost;
            bridge.LocationToEast = spiderField;

            spiderField.LocationToWest = bridge;
            spiderField.LocationToEast = shiningValley;

            shiningValley.LocationToWest = spiderField;

            // Add the locations to the static list
            Locations.Add(home);
            Locations.Add(townSquare);
            Locations.Add(guardPost);
            Locations.Add(alchemistHut);
            Locations.Add(alchemistsGarden);
            Locations.Add(farmhouse);
            Locations.Add(farmersField);
            Locations.Add(bridge);
            Locations.Add(spiderField);
            Locations.Add(shiningValley);
        }

        public static Item ItemByID(int id)
        {
            foreach (Item item in Items)
            {
                if (item.ID == id)
                {
                    return item;
                }
            }

            return null;
        }

        public static Mob MobByID(int id)
        {
            foreach (Mob mob in Mobs)
            {
                if (mob.ID == id)
                {
                    return mob;
                }
            }

            return null;
        }
        public static int IDByMob(Mob monster)
        {
            foreach (Mob mob in Mobs)
            {
                if (mob.ID == monster.ID)
                {
                    return mob.ID;
                }
            }

            return 0;
        }

        public static Quest QuestByID(int id)
        {
            foreach (Quest quest in Quests)
            {
                if (quest.ID == id)
                {
                    return quest;
                }
            }

            return null;
        }

        public static Location LocationByID(int id)
        {
            foreach (Location location in Locations)
            {
                if (location.ID == id)
                {
                    return location;
                }
            }

            return null;
        }
    }
}
