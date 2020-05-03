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
        public ICollection<string> ExtraInformationLines { get; }
        public ICollection<CraftingRecipe> Recipes { get; }
        public int BigCraftableCount { get; }
        public ICollection<Profession> Professions { get; }

        public LevelUpInfo(
            Skill skill,
            int level,
            ICollection<string> extraInformationLines,
            ICollection<CraftingRecipe> recipes,
            int bigCraftableCount,
            ICollection<Profession> professions) {
            Skill = skill;
            Level = level;
            ExtraInformationLines = extraInformationLines;
            Recipes = recipes;
            BigCraftableCount = bigCraftableCount;
            Professions = professions;
        }

    }
}
