﻿using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapNotificationTypes;
using TaleWorlds.Library;

namespace HuntableHerds.Models {
    public class HerdMapNotificationItemVM : MapNotificationItemBaseVM {
        public HerdMapNotificationItemVM(HerdMapNotification data) : base(data) {
            base.NotificationIdentifier = "ransom";
            this._onInspect = () => {
                OpenHuntingMessageBox();
                base.ExecuteRemove();
            };

            CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, party => {
                if (party.IsMainParty)
                    base.ExecuteRemove();
            });
        }

        public override void OnFinalize() {
            CampaignEventDispatcher.Instance.RemoveListeners(this);
        }

        private void OpenHuntingMessageBox() {
            string sceneName = PlayerEncounter.GetBattleSceneForMapPatch(Campaign.Current.MapSceneWrapper.GetMapPatchAtPosition(MobileParty.MainParty.Position2D));

            InquiryData inquiry = new InquiryData("Herd Spotted", HerdBuildData.CurrentHerdBuildData.Message, true, true, "Yes", "No", () => {
                CustomMissions.StartHuntingMission(sceneName);
            }, null);

            InformationManager.ShowInquiry(inquiry, true, true);

        }
    }
}
