using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace PatchAnything.SkillsAndProfessions {
    class FlexibleLevelUpMenu : StardewValley.Menus.LevelUpMenu {

        public FlexibleLevelUpMenu() : base() { }

        public FlexibleLevelUpMenu(LevelUpInfo levelUp) {
            // TODO generalize this.sourceRectForLevelIcon
            // TODO create ClickableComponents for all professions
            // TODO redo update() and draw()
            // TODO create/find UI framework?
            // TODO generalize extra info
        }

        public override void update(GameTime time) {
            base.update(time);
        }

        public override void draw(SpriteBatch b) {
            base.draw(b);
        }

    }
}

