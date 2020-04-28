using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace PatchAnything
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry: Mod {

        public static IModHelper Helper;
        private static IMonitor monitor;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            Helper = helper;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;

            monitor = this.Monitor;
        }

        public static void Log(string message, LogLevel level) {
            monitor.Log(message, level);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e) {
            // ignore if player hasn't loaded a save yet
            if(!Context.IsWorldReady)
                return;

            if(e.NewMenu is StardewValley.Menus.LevelUpMenu levelUpMenu) {
                int currentSkill = Helper.Reflection.GetField<int>(levelUpMenu, "currentSkill").GetValue();
                int currentLevel = Helper.Reflection.GetField<int>(levelUpMenu, "currentLevel").GetValue();

                Game1.activeClickableMenu = new SkillsAndProfessions.Menus.FlexibleLevelUpMenu(currentSkill, currentLevel);
            }
        }
    }
}
