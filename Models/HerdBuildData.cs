using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace HuntableHerds.Models {
    public class HerdBuildData {
        public string NotifMessage;
        public string Message;
        public string SpawnId;
        public int TotalAmountInHerd;
        public bool IsPassive;
        public float StartingHealth;
        public float HitboxRange;
        public int DamageToPlayer;
        public float SightRange;
        public bool FleeOnAttacked;
        public List<(string, int)> ItemDrops;

        private static List<HerdBuildData> allHuntableAgentBuildDatas = new();
        public static HerdBuildData CurrentHerdBuildData;

        public HerdBuildData(string notifMessage, string message, string spawnId, int totalAmountInHerd, bool isPassive, float startingHealth, float hitboxRange, int damageToPlayer, float sightRange, bool fleeOnAttacked, List<(string, int)> itemDropsIdAndCount) {
            NotifMessage = notifMessage;
            Message = message;
            SpawnId = spawnId;
            TotalAmountInHerd = totalAmountInHerd;
            IsPassive = isPassive;
            StartingHealth = startingHealth;
            HitboxRange = hitboxRange;
            DamageToPlayer = damageToPlayer;
            SightRange = sightRange;
            FleeOnAttacked = fleeOnAttacked;
            ItemDrops = itemDropsIdAndCount;

            CurrentHerdBuildData = this;
        }

        public ItemRoster GetCopyOfItemDrops() {
            ItemRoster itemRoster = new ItemRoster();
            foreach ((string, int) pair in ItemDrops) {
                ItemObject? item = null;
                try {
                    item = Game.Current.ObjectManager.GetObject<ItemObject>(pair.Item1);
                }
                catch (NullReferenceException) {
                    continue;
                }
                if (item == null)
                    continue;
                itemRoster.AddToCounts(item, pair.Item2);
            }
            return itemRoster;
        }

        public static void BuildAll() {
            allHuntableAgentBuildDatas.Clear();

            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string xmlFileName = Path.Combine(assemblyFolder, "hunting_herds.xml");

            XElement huntingHerds = XElement.Load(xmlFileName);

            foreach (XElement element in huntingHerds.Descendants("Herd")) {
                string notifMessage = element.Element("notifMessage").Value;
                string message = element.Element("message").Value;
                string spawnId = element.Element("spawnId").Value;
                int totalAmountInHerd = (int)element.Element("totalAmountInHerd");
                bool isPassive = element.Element("isPassive").Value == "true" ? true : false;
                float startingHealth = (float)element.Element("startingHealth");
                float hitboxRange = (float)element.Element("hitboxRange");
                int damageToPlayer = (int)element.Element("damageToPlayer");
                float sightRange = (float)element.Element("sightRange");
                bool fleeOnAttacked = element.Element("fleeOnAttacked").Value == "true" ? true : false;
                List<(string, int)> itemDrops = new();
                foreach (XElement itemDrop in element.Element("ItemDrops").Descendants("ItemDrop")) {
                    itemDrops.Add((itemDrop.Element("itemId").Value, (int)itemDrop.Element("amount")));
                }

                HerdBuildData buildData = new HerdBuildData(notifMessage, message, spawnId, totalAmountInHerd, isPassive, startingHealth, hitboxRange, damageToPlayer, sightRange, fleeOnAttacked, itemDrops);
                allHuntableAgentBuildDatas.Add(buildData);
            }
        }

        public static void Randomize() {
            int randomIndex = MBRandom.RandomInt(0, allHuntableAgentBuildDatas.Count);
            CurrentHerdBuildData = allHuntableAgentBuildDatas[randomIndex];
        }
    }
}
