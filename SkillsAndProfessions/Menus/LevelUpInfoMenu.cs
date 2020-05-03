using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace PatchAnything.SkillsAndProfessions {
    class LevelUpInfoMenu : StardewValley.Menus.LevelUpMenu {

        const int DIMENSION_BASE_WIDTH = 960;
        const int DIMENSION_BASE_HEIGHT = 256;
        const int DIMENSION_CRAFTABLE_HEIGHT = 64;
        const int DIMENSION_BIG_CRAFTABLE_HEIGHT = 128;
        const int DIMENSION_EXTRA_INFO_HEIGHT = 48;
        const int DIMENSION_OK_BUTTON_WIDTH = 64;
        const int DIMENSION_OK_BUTTON_HEIGHT = 64;
        const int DIMENSION_OK_BUTTON_MARGIN = 4;

        LevelUpInfo levelUp;

        public LevelUpInfoMenu(LevelUpInfo levelUp) : base() {
            // Outline:
            //      timerBeforeStart
            //      Handle achievements
            //      Handle mail
            //      Add recipes
            //      Calculate size
            //      Load icons/strings

            // TODO generalize this.sourceRectForLevelIcon
            // TODO create ClickableComponents for all professions
            // TODO paginate professions

            this.levelUp = levelUp;

            PopulateBaseFields();
            CalculateDimensions();
            HandleAchievements();

            isProfessionChooser = false;
            informationUp = true;
            isActive = true;
        }

        void PopulateBaseFields() {
            string title = Game1.content.LoadString("Strings\\UI:LevelUp_Title", levelUp.Level, levelUp.Skill.Name);
            ModEntry.Instance.Helper.Reflection.GetField<string>(this, "title").SetValue(title);

            if (levelUp.Recipes != null) {
                ModEntry.Instance.Helper.Reflection.GetField<List<CraftingRecipe>>(this, "newCraftingRecipes").SetValue(levelUp.Recipes.ToList<CraftingRecipe>());
            }
            if (levelUp.ExtraInformationLines != null) {
                ModEntry.Instance.Helper.Reflection.GetField<List<string>>(this, "extraInfoForLevel").SetValue(levelUp.ExtraInformationLines.ToList<string>());
            }
        }

        void CalculateDimensions() {
            width = DIMENSION_BASE_WIDTH;

            height = DIMENSION_BASE_HEIGHT;
            height += levelUp.BigCraftableCount * DIMENSION_BIG_CRAFTABLE_HEIGHT;
            height += (levelUp.Recipes.Count - levelUp.BigCraftableCount) * DIMENSION_CRAFTABLE_HEIGHT;
            height += levelUp.ExtraInformationLines.Count * DIMENSION_EXTRA_INFO_HEIGHT;

            xPositionOnScreen = (Game1.viewport.Width - width) / 2;
            yPositionOnScreen = (Game1.viewport.Height - height) / 2;

            okButton.bounds = new Rectangle(
                xPositionOnScreen + width + DIMENSION_OK_BUTTON_MARGIN,
                yPositionOnScreen + height - DIMENSION_OK_BUTTON_HEIGHT - borderWidth,
                DIMENSION_OK_BUTTON_WIDTH,
                DIMENSION_OK_BUTTON_HEIGHT
            );
        }

        void HandleAchievements() {
            if (levelUp.Level == 10) {
                Game1.getSteamAchievement("Achievement_SingularTalent");
                if (Game1.player.farmingLevel == 10
                    && Game1.player.fishingLevel == 10
                    && Game1.player.foragingLevel == 10
                    && Game1.player.miningLevel == 10
                    && Game1.player.combatLevel == 10) {
                    Game1.getSteamAchievement("Achievement_MasterOfTheFiveWays");
                }
            }
        }

        public override void update(GameTime time) {
            base.update(time);
        }

        public override void draw(SpriteBatch b) {
            base.draw(b);
        }

    }

}
