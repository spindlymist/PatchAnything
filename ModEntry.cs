using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using PatchAnything.SkillsAndProfessions;

namespace PatchAnything {
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod {

        public static ModEntry Instance { get; private set; } = null;

        private SkillsAndProfessionsDataManager skillsAndProfessions;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            ModEntry.Instance = this;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;

            skillsAndProfessions = new SkillsAndProfessionsDataManager();
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e) {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;

            if (!(e.NewMenu is FlexibleLevelUpMenu) && e.NewMenu is StardewValley.Menus.LevelUpMenu levelUpMenu) {
                SkillsAndProfessionsEvents.ReplaceLevelUpMenu(skillsAndProfessions, levelUpMenu);
            }
        }

    }
}
