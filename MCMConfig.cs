using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntableHerds {
    internal sealed class MCMConfig : AttributeGlobalSettings<MCMConfig> {
        public override string Id => "HuntableHerds";
        public override string DisplayName => "Huntable Herds";
        public override string FolderName => "HuntableHerds";
        public override string FormatType => "xml";

        // GENERAL

        [SettingPropertyBool("Toggle Herds", Order = 1, HintText = "Enable/disable the mod.", RequireRestart = false)]
        [SettingPropertyGroup("General")]
        public bool HerdsEnabled { get; set; } = true;

        [SettingPropertyFloatingInteger("Daily % Chance of Spotting Herd", 0f, 1f, Order = 2, HintText = "Set the daily percent chance of spotting a herd.", RequireRestart = false)]
        [SettingPropertyGroup("General")]
        public float DailyChanceOfSpottingHerd { get; set; } = 0.3f;

        [SettingPropertyBool("Crouch To Field Dress/Loot", Order = 3, HintText = "Enable/disable needing to crouch in order to loot slain animals.", RequireRestart = false)]
        [SettingPropertyGroup("General")]
        public bool CrouchNeededEnabled { get; set; } = true;
    }
}
