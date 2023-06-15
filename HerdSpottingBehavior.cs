using HuntableHerds.Models;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace HuntableHerds {
    public class HerdSpottingBehavior : CampaignBehaviorBase {
        public override void RegisterEvents() {
            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, OnDailyTickParty);

        }

        public override void SyncData(IDataStore dataStore) {
            //
        }

        private void OnDailyTickParty(MobileParty party) {
            if (!party.IsMainParty || party.CurrentSettlement != null)
                return;

            MCMConfig? config = GlobalSettings<MCMConfig>.Instance;
            if (config != null && config.HerdsEnabled && MBRandom.RandomFloat <= config.DailyChanceOfSpottingHerd)
                ShowHuntingHerdNotification();
        }

        private void ShowHuntingHerdNotification() {
            HerdBuildData.Randomize();
            Campaign.Current.CampaignInformationManager.NewMapNoticeAdded(new HerdMapNotification(new TextObject(HerdBuildData.CurrentHerdBuildData.NotifMessage)));
        }
    }
}
