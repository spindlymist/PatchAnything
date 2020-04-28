using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace PatchAnything.SkillsAndProfessions {
    class LevelUpInfo {

        public Skill Skill { get; }
        public int Level { get; }
        public IEnumerable<string> ExtraInformationLines { get; }
        public IEnumerable<CraftingRecipe> CraftingRecipes { get; }
        public IEnumerable<CraftingRecipe> CookingRecipes { get; }
        public IEnumerable<Profession> Professions { get; }

        public LevelUpInfo(
            Skill skill,
            int level,
            IEnumerable<string> extraInformationLines,
            IEnumerable<CraftingRecipe> craftingRecipes,
            IEnumerable<CraftingRecipe> cookingRecipes,
            IEnumerable<Profession> professions) {
            Skill = skill;
            Level = level;
            ExtraInformationLines = extraInformationLines;
            CraftingRecipes = craftingRecipes;
            CookingRecipes = cookingRecipes;
            Professions = professions;
        }

    }
}
