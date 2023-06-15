using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace HuntableHerds.Models {
    public class HerdMapNotification : InformationData {
        public override TextObject TitleText {
            get {
                return new TextObject("Herd Spotted");
            }
        }

        public override string SoundEventPath {
            get {
                return "";
            }
        }

        public HerdMapNotification(TextObject description) : base(description) {
        }
    }
}
