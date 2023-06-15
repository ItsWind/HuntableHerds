using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.InputSystem;
using HuntableHerds.AgentComponents;
using HuntableHerds.Models;

namespace HuntableHerds {
    public class HerdMissionLogic : MissionLogic {
        private Dictionary<Agent, HerdAgentComponent> animals = new();

        public override void AfterStart() {
            SpawnPlayer();
            SubModule.PrintDebugMessage("Press Q nearby slain animals to skin and loot them!");
        }

        public override void OnMissionTick(float dt) {
            if (Agent.Main == null)
                return;

            if (Input.IsKeyPressed(InputKey.Q))
                LootArea(10f);

            if (animals.Count >= HerdBuildData.CurrentHerdBuildData.TotalAmountInHerd)
                return;

            Vec3 position = GetTrueRandomPosition(Agent.Main.Position, 20f, 500f);
            SpawnAnimalToHunt(position);
        }

        private void LootArea(float maxDistance) {
            ItemRoster fullItemRoster = new ItemRoster();
            List<HerdAgentComponent> huntableAgentsLooted = new();

            foreach (KeyValuePair<Agent, HerdAgentComponent> pair in animals) {
                if (pair.Key.IsActive() || pair.Value.GetItemDrops().IsEmpty() || pair.Key.Position.Distance(Agent.Main.Position) > maxDistance)
                    continue;
                fullItemRoster.Add(pair.Value.GetItemDrops());
                huntableAgentsLooted.Add(pair.Value);
            }

            if (huntableAgentsLooted.Count == 0) {
                SubModule.PrintDebugMessage("There are no unlooted animals nearby.");
                return;
            }

            InventoryManager.OpenScreenAsReceiveItems(fullItemRoster, new TextObject("Loot"), () => {
                foreach (HerdAgentComponent component in huntableAgentsLooted)
                    component.ClearItemDrops();
            });
        }

        private Agent SpawnPlayer() {
            MatrixFrame matrixFrame = MatrixFrame.Identity;
            CharacterObject playerCharacter = CharacterObject.PlayerCharacter;
            Vec3 centerPos = matrixFrame.origin.NormalizedCopy();
            base.Mission.Scene.GetNavMeshCenterPosition(0, ref centerPos);
            Vec3 playerSpawnPos = GetTrueRandomPosition(centerPos, 20f, 200f);
            AgentBuildData agentBuildData = new AgentBuildData(playerCharacter).Team(base.Mission.PlayerTeam).InitialPosition(playerSpawnPos);

            Vec2 vec = matrixFrame.rotation.f.AsVec2;
            vec = vec.Normalized();

            AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(vec).CivilianEquipment(false).NoHorses(false).NoWeapons(false).ClothingColor1(base.Mission.PlayerTeam.Color).ClothingColor2(base.Mission.PlayerTeam.Color2).TroopOrigin(new PartyAgentOrigin(PartyBase.MainParty, playerCharacter, -1, default(UniqueTroopDescriptor), false)).MountKey(MountCreationKey.GetRandomMountKeyString(playerCharacter.Equipment[EquipmentIndex.ArmorItemEndSlot].Item, playerCharacter.GetMountKeySeed())).Controller(Agent.ControllerType.Player);
            Hero heroObject = playerCharacter.HeroObject;

            if (((heroObject != null) ? heroObject.ClanBanner : null) != null) {
                agentBuildData2.Banner(playerCharacter.HeroObject.ClanBanner);
            }

            Agent agent = base.Mission.SpawnAgent(agentBuildData2);

            for (int i = 0; i < 3; i++) {
                Agent.Main.AgentVisuals.GetSkeleton().TickAnimations(0.1f, Agent.Main.AgentVisuals.GetGlobalFrame(), true);
            }

            return agent;
        }

        private void SpawnAnimalToHunt(Vec3 position) {
            MatrixFrame frame = MatrixFrame.Identity;

            ItemObject spawnObject = Game.Current.ObjectManager.GetObject<ItemObject>(HerdBuildData.CurrentHerdBuildData.SpawnId);
            ItemRosterElement rosterElement = new ItemRosterElement(spawnObject);
            Vec2 initialDirection = frame.rotation.f.AsVec2;
            Agent agent = base.Mission.SpawnMonster(rosterElement, default(ItemRosterElement), in position, in initialDirection);

            HerdAgentComponent huntAgentComponent = HerdBuildData.CurrentHerdBuildData.IsPassive ? new PassiveHerdAgentComponent(agent) : new AggressiveHerdAgentComponent(agent);

            agent.AddComponent(huntAgentComponent);

            animals.Add(agent, huntAgentComponent);

            for (int i = 0; i < 3; i++) {
                agent.AgentVisuals.GetSkeleton().TickAnimations(0.1f, agent.AgentVisuals.GetGlobalFrame(), true);
            }
        }

        private Vec3 GetTrueRandomPosition(Vec3 center, float minDistance, float maxDistance, bool nearFirst = false) {
            Vec3 randomPos = base.Mission.GetRandomPositionAroundPoint(center, minDistance, maxDistance, nearFirst);
            while (randomPos == center)
                randomPos = base.Mission.GetRandomPositionAroundPoint(center, minDistance, maxDistance, nearFirst);
            return randomPos;
        }
    }
}
