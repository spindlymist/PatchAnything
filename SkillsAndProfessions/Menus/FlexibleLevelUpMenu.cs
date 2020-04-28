using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace PatchAnything.SkillsAndProfessions.Menus {
    class FlexibleLevelUpMenu: StardewValley.Menus.LevelUpMenu {

        public FlexibleLevelUpMenu() : base() { }

        public FlexibleLevelUpMenu(int skill, int level) : base(skill, level) {
            SkillsAndProfessionsManager.Instance.LoadData();

            // TODO generalize this.sourceRectForLevelIcon
            // TODO create ClickableComponents for all professions
            // TODO generalize mail
            // TODO redo update() and draw()
            // TODO create/find UI framework?
        }

        public new List<string> getExtraInfoForLevel(int whichSkill, int whichLevel) {
            // TODO generalize extra info
            return new List<string>();
        }

    }
}
