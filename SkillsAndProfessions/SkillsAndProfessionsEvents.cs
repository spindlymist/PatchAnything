using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace PatchAnything.SkillsAndProfessions {
    static class SkillsAndProfessionsEvents {

        public static void ReplaceLevelUpMenu(SkillsAndProfessionsDataManager dataManager, StardewValley.Menus.LevelUpMenu levelUpMenu) {
            int skillID = ModEntry.Instance.Helper.Reflection.GetField<int>(levelUpMenu, "currentSkill").GetValue();
            int skillLevel = ModEntry.Instance.Helper.Reflection.GetField<int>(levelUpMenu, "currentLevel").GetValue();

            dataManager.LoadData();
            Skill skill = dataManager.GetSkillByID(skillID);

            if (skill == null) {
                ModEntry.Instance.Monitor.Log($"Can't level up. Failed to find skill with ID {skillID}.", LogLevel.Error);
            }
            else {
                LevelUpInfo levelUp = dataManager.GetLevelUpInfo(Game1.player, skill, skillLevel);
                Game1.activeClickableMenu = new LevelUpInfoMenu(levelUp);
            }
        }

    }
}
