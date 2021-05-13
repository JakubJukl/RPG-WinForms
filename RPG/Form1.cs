using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Engine;
using System.IO;

namespace RPG
{
    public partial class Form1 : Form
    {
       // public int[] y = new int[10];
        private Player player;
        private Mob currentMob;
        private const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";
        
        public Form1()
        {
            InitializeComponent();

            if (File.Exists(PLAYER_DATA_FILE_NAME))
            {
                player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));
            }
            else
            {
                player = Player.CreateDefaultPlayer();
            }
            MoveTo(player.CurrentLocation);

            lblHitPoints.DataBindings.Add("Text", player, "CurrentHP");
            lblGold.DataBindings.Add("Text", player, "Gold");
            lblExperience.DataBindings.Add("Text", player, "XP");
            lblLevel.DataBindings.Add("Text", player, "Level");
        }

        private void btnNorth_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToNorth);
        }

        private void btnEast_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToEast);
        }

        private void btnSouth_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToSouth);
        }

        private void btnWest_Click(object sender, EventArgs e)
        {
            MoveTo(player.CurrentLocation.LocationToWest);
        }

        private void MoveTo(Location newLocation)
        {
            //Does the location have any required items
            if (!player.HasRequiredItemToEnterThisLocation(newLocation))
            {
                rtbMessages.Text += "Musíš mít " + newLocation.ItemRequiredToEnter.Name + " aby si mohl vstoupit do této lokace." + Environment.NewLine;
                ScrollToBottomOfMessages();
                return;
            }
            if ((player.CurrentLocation != null) && (player.CurrentLocation.MonsterLivingHere!=null) && (!player.KilledMobRequiredToEnter(newLocation, player.y[player.CurrentLocation.MonsterLivingHere.ID])))
            {
                rtbMessages.Text += "Musíš zabít " + player.CurrentLocation.MonsterLivingHere.Name + " aby si mohl vstoupit do této lokace." + Environment.NewLine;
                ScrollToBottomOfMessages();
                return;
            }

            // Update the player's current location
            player.CurrentLocation = newLocation;

            // Show/hide available movement buttons
            btnNorth.Visible = (newLocation.LocationToNorth != null);
            btnEast.Visible = (newLocation.LocationToEast != null);
            btnSouth.Visible = (newLocation.LocationToSouth != null);
            btnWest.Visible = (newLocation.LocationToWest != null);

            // Display current location name and description
            rtbLocation.Text = newLocation.Name + Environment.NewLine;
            rtbLocation.Text += newLocation.Description + Environment.NewLine;
            ScrollToBottomOfMessages();

            // Completely heal the player
            player.CurrentHP = player.MaxHP;

            // Does the location have a quest?
            if (newLocation.QuestAvailableHere != null)
            {
                // See if the player already has the quest, and if they've completed it
                bool playerAlreadyHasQuest = player.HasThisQuest(newLocation.QuestAvailableHere);
                bool playerAlreadyCompletedQuest = player.CompletedThisQuest(newLocation.QuestAvailableHere);

                // See if the player already has the quest
                if (playerAlreadyHasQuest)
                {
                    // If the player has not completed the quest yet
                    if (!playerAlreadyCompletedQuest)
                    {
                        // See if the player has all the items needed to complete the quest
                        bool playerHasAllItemsToCompleteQuest = player.HasAllQuestCompletionItems(newLocation.QuestAvailableHere);

                        // The player has all items required to complete the quest
                        if (playerHasAllItemsToCompleteQuest)
                        {
                            // Display message
                            rtbMessages.Text += Environment.NewLine;
                            rtbMessages.Text += "Dokončil si úkol'" + newLocation.QuestAvailableHere.Name + "'." + Environment.NewLine;
                            ScrollToBottomOfMessages();

                            // Remove quest items from inventory
                            player.RemoveQuestCompletionItems(newLocation.QuestAvailableHere);

                            // Give quest rewards
                            rtbMessages.Text += "Dostal si: " + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.XPReward.ToString() + " XP" + Environment.NewLine;
                            rtbMessages.Text += newLocation.QuestAvailableHere.GoldReward.ToString() + " zlatých" + Environment.NewLine;
                            if (newLocation.QuestAvailableHere.RewardItem != null)
                            {
                                rtbMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                                // Add the reward item to the player's inventory
                                player.AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);
                            }
                            rtbMessages.Text += Environment.NewLine;
                            ScrollToBottomOfMessages();

                            player.XP += newLocation.QuestAvailableHere.XPReward;
                            player.Gold += newLocation.QuestAvailableHere.GoldReward;

                            int lastlevel = player.Level;
                            player.Level = player.LevelUp(player.XP, player.Level);
                            levelUp(lastlevel);

                            // Mark the quest as completed
                            // Find the quest in the player's quest list
                            player.MarkQuestCompleted(newLocation.QuestAvailableHere);
                        }
                    }
                }
                else
                {
                    // The player does not already have the quest

                    // Display the messages
                    rtbMessages.Text += "Dostal si úkol '" + newLocation.QuestAvailableHere.Name + "'." + Environment.NewLine;
                    rtbMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
                    rtbMessages.Text += "Aby si ho splnil, vrať se s:" + Environment.NewLine;
                    ScrollToBottomOfMessages();
                    foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                    {
                        if (qci.Quantity == 1)
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine;
                            ScrollToBottomOfMessages();
                        }
                        else
                        {
                            rtbMessages.Text += qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine;
                            ScrollToBottomOfMessages();
                        }
                    }
                    rtbMessages.Text += Environment.NewLine;
                    ScrollToBottomOfMessages();

                    // Add the quest to the player's quest list
                    player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }

            // Does the location have a monster?
            if (newLocation.MonsterLivingHere != null)
            {
                if (newLocation.MonsterLivingHere.Boss == true)
                {
                    if (player.y[newLocation.MonsterLivingHere.ID] != 0)
                    {
                        Nomob();
                    }
                    else
                    {
                        Ismob();

                    }
                }
                else
                {
                    Ismob();
                }
            }
            else
            {
                Nomob();
            }
            // Refresh player's inventory list
            UpdateInventoryListInUI();

            // Refresh player's quest list
            UpdateQuestListInUI();

            // Refresh player's weapons combobox
            UpdateWeaponListInUI();

            // Refresh player's potions combobox
            UpdatePotionListInUI();

            //function for "summoning" mob 
            void Ismob()
            {
                rtbMessages.Text += "Před tebou se vynořil divoký " + newLocation.MonsterLivingHere.Name + Environment.NewLine;
                ScrollToBottomOfMessages();

                // Make a new monster, using the values from the standard monster in the World.Monster list
                Mob standardMob = World.MobByID(newLocation.MonsterLivingHere.ID);

                currentMob = new Mob(standardMob.ID, standardMob.Name, standardMob.MaxDmg,
                    standardMob.XPReward, standardMob.GoldReward, standardMob.CurrentHP, standardMob.MaxHP, standardMob.Boss);

                foreach (LootItem lootItem in standardMob.LootTable)
                {
                    currentMob.LootTable.Add(lootItem);
                }

                cboWeapons.Visible = true;
                cboPotions.Visible = true;
                btnUseWeapon.Visible = true;
                btnUsePotion.Visible = true;
            }

        }
        private void btnUseWeapon_Click(object sender, EventArgs e)
        {
            // Get the currently selected weapon from the cboWeapons ComboBox
            Weapon currentWeapon = (Weapon)cboWeapons.SelectedItem;

            // Determine the amount of damage to do to the monster
            int damageToMob = RNG.Numberbetween(currentWeapon.MinDmg, currentWeapon.MaxDmg);

            // Apply the damage to the monster's CurrentHP
            currentMob.CurrentHP -= damageToMob;

            // Display message
            rtbMessages.Text += "Udeřil si " + currentMob.Name + " za " + damageToMob.ToString() + " poškození." + Environment.NewLine;
            ScrollToBottomOfMessages();

            // Check if the monster is dead
            if (currentMob.CurrentHP <= 0)
            {
                // Monster is dead
                rtbMessages.Text += Environment.NewLine;
                rtbMessages.Text += "Zabil si " + currentMob.Name + Environment.NewLine;
                ScrollToBottomOfMessages();

                // Give player experience points for killing the monster
                player.XP += currentMob.XPReward;
                rtbMessages.Text += "Získáváš " + currentMob.XPReward.ToString() + " XP" + Environment.NewLine;
                ScrollToBottomOfMessages();

                // Give player gold for killing the monster 
                player.Gold += currentMob.GoldReward;
                rtbMessages.Text += "Získáváš " + currentMob.GoldReward.ToString() + " zlatých" + Environment.NewLine;
                ScrollToBottomOfMessages();

                // Get random loot items from the monster
                List<InventoryItem> lootedItems = new List<InventoryItem>();

                // Number of times, we killed the mob
                player.y[currentMob.ID]++;
                //rtbMessages.Text+="FUnguje to"+player.y[currentMob.ID];

                // Add items to the lootedItems list, comparing a random number to the drop percentage
                foreach (LootItem lootItem in currentMob.LootTable)
                {
                    if (RNG.Numberbetween(1, 100) <= lootItem.DropPercentage)
                    {
                        lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                    }
                }

                // If no items were randomly selected, then add the default loot item(s).
                if (lootedItems.Count == 0)
                {
                    foreach (LootItem lootItem in currentMob.LootTable)
                    {
                        if (lootItem.IsDefaultItem)
                        {
                            lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                        }
                    }
                }

                // Add the looted items to the player's inventory
                foreach (InventoryItem inventoryItem in lootedItems)
                {
                    player.AddItemToInventory(inventoryItem.Details);

                    if (inventoryItem.Quantity == 1)
                    {
                        rtbMessages.Text += "Našel si " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.Name + Environment.NewLine;
                        ScrollToBottomOfMessages();
                    }
                    else
                    {
                        rtbMessages.Text += "Našel si " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.NamePlural + Environment.NewLine;
                        ScrollToBottomOfMessages();
                    }
                }

                // Refresh player information and inventory controls
                int lastlevel = player.Level;
                player.Level = player.LevelUp(player.XP, player.Level);
                levelUp(lastlevel);

                UpdateInventoryListInUI();
                UpdateWeaponListInUI();
                UpdatePotionListInUI();

                // Add a blank line to the messages box, just for appearance.
                rtbMessages.Text += Environment.NewLine;
                ScrollToBottomOfMessages();

                // Move player to current location (to heal player and create a new monster to fight)
                MoveTo(player.CurrentLocation);
            }
            else
            {
                // Mob is still alive

                // Determine the amount of damage the monster does to the player
                int damageToPlayer = RNG.Numberbetween(0, currentMob.MaxDmg);

                // Display message
                rtbMessages.Text += currentMob.Name + " ti udělil " + damageToPlayer.ToString() + " poškození." + Environment.NewLine;
                ScrollToBottomOfMessages();

                // Subtract damage from player
                player.CurrentHP -= damageToPlayer;

                if (player.CurrentHP <= 0)
                {
                    // Display message
                    rtbMessages.Text += currentMob.Name + " tě zabil." + Environment.NewLine;
                    ScrollToBottomOfMessages();

                    // Move player to "Home"
                    MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
                }
            }
        }


        private void btnUsePotion_Click(object sender, EventArgs e)
        {
            // Get the currently selected potion from the combobox
            HealingPotion potion = (HealingPotion)cboPotions.SelectedItem;

            // Add healing amount to the player's current hit points
            player.CurrentHP = (player.CurrentHP + potion.AmountToHeal);

            // CurrentHP cannot exceed player's MaxHP
            if (player.CurrentHP > player.MaxHP)
            {
                player.CurrentHP = player.MaxHP;
            }

            // Remove the potion from the player's inventory
            foreach (InventoryItem ii in player.Inventory)
            {
                if (ii.Details.ID == potion.ID)
                {
                    ii.Quantity--;
                    break;
                }
            }

            // Display message
            rtbMessages.Text += "Vypil si " + potion.Name + Environment.NewLine;
            ScrollToBottomOfMessages();

            // Monster gets their turn to attack

            // Determine the amount of damage the monster does to the player
            int damageToPlayer = RNG.Numberbetween(0, currentMob.MaxDmg);

            // Display message
            rtbMessages.Text += currentMob.Name + " ti udělil" + damageToPlayer.ToString() + " poškození." + Environment.NewLine;
            ScrollToBottomOfMessages();

            // Subtract damage from player
            player.CurrentHP -= damageToPlayer;

            if (player.CurrentHP <= 0)
            {
                // Display message
                rtbMessages.Text += currentMob.Name + " tě zabil." + Environment.NewLine;
                ScrollToBottomOfMessages();

                // Move player to "Home"
                MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            }

            UpdateInventoryListInUI();
            UpdatePotionListInUI();
        }
        private void UpdateInventoryListInUI()
        {
            dgvInventory.RowHeadersVisible = false;

            dgvInventory.ColumnCount = 2;
            dgvInventory.Columns[0].Name = "Jméno";
            dgvInventory.Columns[0].Width = 197;
            dgvInventory.Columns[1].Name = "Množství";

            dgvInventory.Rows.Clear();

            foreach (InventoryItem inventoryItem in player.Inventory)
            {
                if (inventoryItem.Quantity > 0)
                {
                    dgvInventory.Rows.Add(new[] { inventoryItem.Details.Name, inventoryItem.Quantity.ToString() });
                }
            }
        }

        private void UpdateQuestListInUI()
        {
            dgvQuests.RowHeadersVisible = false;

            dgvQuests.ColumnCount = 2;
            dgvQuests.Columns[0].Name = "Jméno";
            dgvQuests.Columns[0].Width = 197;
            dgvQuests.Columns[1].Name = "Splnil si to?";

            dgvQuests.Rows.Clear();

            foreach (PlayerQuest playerQuest in player.Quests)
            {
                dgvQuests.Rows.Add(new[] { playerQuest.Details.Name, playerQuest.IsCompleted.ToString() });
            }
        }
            private void UpdateWeaponListInUI()
            {
                List<Weapon> weapons = new List<Weapon>();

                foreach (InventoryItem inventoryItem in player.Inventory)
                {
                    if (inventoryItem.Details is Weapon)
                    {
                        if (inventoryItem.Quantity > 0)
                        {
                            weapons.Add((Weapon)inventoryItem.Details);
                        }
                    }
                }

                if (weapons.Count == 0)
                {
                    // The player doesn't have any weapons, so hide the weapon combobox and "Use" button
                    cboWeapons.Visible = false;
                    btnUseWeapon.Visible = false;
                }
                else
                {
                    cboWeapons.SelectedIndexChanged -= cboWeapons_SelectedIndexChanged;
                    cboWeapons.DataSource = weapons;
                    cboWeapons.SelectedIndexChanged += cboWeapons_SelectedIndexChanged;
                    cboWeapons.DisplayMember = "Name";
                    cboWeapons.ValueMember = "ID";

                    if (player.CurrentWeapon != null)
                    {
                        cboWeapons.SelectedItem = player.CurrentWeapon;
                    }
                    else
                    {
                        cboWeapons.SelectedIndex = 0;
                    }
                }
            }

            private void UpdatePotionListInUI()
        {
            List<HealingPotion> healingPotions = new List<HealingPotion>();

            foreach (InventoryItem inventoryItem in player.Inventory)
            {
                if (inventoryItem.Details is HealingPotion)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        healingPotions.Add((HealingPotion)inventoryItem.Details);
                    }
                }
            }

            if (healingPotions.Count == 0)
            {
                // The player doesn't have any potions, so hide the potion combobox and "Use" button
                cboPotions.Visible = false;
                btnUsePotion.Visible = false;
            }
            else
            {
                cboPotions.DataSource = healingPotions;
                cboPotions.DisplayMember = "Name";
                cboPotions.ValueMember = "ID";

                cboPotions.SelectedIndex = 0;
            }
        }
        public void ScrollToBottomOfMessages()
        {
            rtbMessages.SelectionStart = rtbMessages.Text.Length;
            rtbMessages.ScrollToCaret();
        }
        private void levelUp(int lastlevel)
        {
            if (lastlevel != player.Level)
            {
                player.MaxHP = player.HPUp(player.Level);
                player.CurrentHP = player.MaxHP;
                player.XP -= lastlevel * 100;
                rtbMessages.Text += "Dosáhl si nového levelu." + Environment.NewLine;
            }
        }
        private void Nomob()
        {
            currentMob = null;

            cboWeapons.Visible = false;
            cboPotions.Visible = false;
            btnUseWeapon.Visible = false;
            btnUsePotion.Visible = false;
        }
        private void cboWeapons_SelectedIndexChanged(object sender, EventArgs e)
        {
            player.CurrentWeapon = (Weapon)cboWeapons.SelectedItem;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, player.ToXmlString());
        }
    }
}
