using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace PatchAnything.SkillsAndProfessions {
    class SkillsAndProfessionsDataManager {

        // Skill Format          id: "name"
        // Example               0: "Farming"
        const int FIELD_COUNT_SKILLS = 1;
        const int FIELD_SKILLS_NAME  = 0;

        // Profession Format     id: "name/skill_id/skill_level/prereq0,prereq1,..."
        // Example               4: "Artisan/0/10/1"
        const int FIELD_COUNT_PROFS       = 4;
        const int FIELD_PROFS_NAME        = 0;
        const int FIELD_PROFS_SKILL_ID    = 1;
        const int FIELD_PROFS_SKILL_LEVEL = 2;
        const int FIELD_PROFS_PREREQS     = 3;

        // Level Up Format       "skill_id/skill_level": "extra_info0,extra_info1,.../prof_id0,prof_id1,.../letter0,letter1,..."
        // Example               "0/10": "Strings\\UI:LevelUp_ExtraInfo_Farming1,Strings\\UI:LevelUp_ExtraInfo_Farming2/2,3,4,5/"
        const int FIELD_COUNT_LEVELS      = 3;
        const int FIELD_LEVELS_EXTRA_INFO = 0;
        const int FIELD_LEVELS_PROFS      = 1;
        const int FIELD_LEVELS_LETTERS    = 2;

        const int FIELD_COUNT_CRAFTING_RECIPE = 5;
        const int FIELD_CRAFTING_RECIPE_SKILL = 4;

        const int FIELD_COUNT_COOKING_RECIPE = 4;
        const int FIELD_COOKING_RECIPE_SKILL = 3;

        IDictionary<int, Skill> skills = new Dictionary<int, Skill>();
        IDictionary<int, Profession> profs = new Dictionary<int, Profession>();

        public SkillsAndProfessionsDataManager() { }

        public void LoadData(bool forceReload = false) {
            if (!forceReload && skills.Count != 0) {
                return;
            }

            skills.Clear();
            profs.Clear();

            IDictionary<int, string> skillsData = ModEntry.Instance.Helper.Content.Load<Dictionary<int, string>>("Data/Skills.json", ContentSource.GameContent);
            foreach (var skillKVP in skillsData) {
                Skill skill;
                if (ParseSkill(skillKVP.Key, skillKVP.Value, out skill)) {
                    skills.Add(skill.ID, skill);
                }
                else {
                    ModEntry.Instance.Monitor.Log($"Failed to parse skill {skillKVP.Key}: {skillKVP.Value}", LogLevel.Warn);
                }
            }

            IDictionary<int, string> profsData = ModEntry.Instance.Helper.Content.Load<Dictionary<int, string>>("Data/Professions.json", ContentSource.GameContent);
            foreach (var profKVP in profsData) {
                Profession prof;
                if (ParseProfession(profKVP.Key, profKVP.Value, out prof)) {
                    profs.Add(prof.ID, prof);
                }
                else {
                    ModEntry.Instance.Monitor.Log($"Failed to parse profession {profKVP.Key}: {profKVP.Value}", LogLevel.Warn);
                }
            }
        }

        static IEnumerable<int> ParseIntList(string list) {
            string[] parts = list.Split(',');
            IList<int> entries = new List<int>();

            for (int i = 0; i < parts.Length; i++) {
                if (int.TryParse(parts[i], out int entry)) {
                    entries.Add(entry);
                }
            }

            return entries;
        }

        bool ParseSkill(int key, string value, out Skill skill) {
            skill = null;
            string[] parts = value.Split('/');

            if (parts.Length < FIELD_COUNT_SKILLS) {
                return false;
            }

            string skillName = parts[FIELD_SKILLS_NAME];
            if (skillName.Length < 1) {
                return false;
            }

            skill = new Skill(key, skillName);

            return true;
        }

        bool ParseProfession(int key, string value, out Profession prof) {
            prof = null;
            string[] parts = value.Split('/');

            if (parts.Length < FIELD_COUNT_PROFS) {
                return false;
            }

            int skillID;
            if (!int.TryParse(parts[FIELD_PROFS_SKILL_ID], out skillID) || !skills.ContainsKey(skillID)) {
                return false;
            }

            int skillLevel;
            if (!int.TryParse(parts[FIELD_PROFS_SKILL_LEVEL], out skillLevel)) {
                return false;
            }

            string profName = parts[FIELD_PROFS_NAME];
            if (profName.Length < 1) {
                return false;
            }

            IEnumerable<int> prereqs = ParseIntList(parts[FIELD_PROFS_PREREQS]);
            prof = new Profession(key, profName, skillID, skillLevel, prereqs);

            return true;
        }

        public Skill GetSkillByID(int id) {
            return skills.TryGetValue(id, out Skill skill) ? skill : null;
        }

        public Profession GetProfessionByID(int id) {
            return profs.TryGetValue(id, out Profession prof) ? prof : null;
        }

        public LevelUpInfo GetLevelUpInfo(Farmer who, Skill skill, int skillLevel) {
            string dataKey = $"{skill.ID}/{skillLevel}";
            IDictionary<string, string> levelUpData = ModEntry.Instance.Helper.Content.Load<Dictionary<string, string>>("Data/LevelUps.json", ContentSource.GameContent);

            IEnumerable<string> extraInformationLines;
            IEnumerable<Profession> professions;

            if (levelUpData.TryGetValue(dataKey, out string data)) {
                string[] parts = data.Split('/');

                if(parts.Length < FIELD_COUNT_LEVELS) {
                    extraInformationLines = FindExtraInformationLines(parts[FIELD_LEVELS_EXTRA_INFO]);
                    professions = FindProfessions(who, parts[FIELD_LEVELS_PROFS]);
                }
            }

            IEnumerable<CraftingRecipe> craftingRecipes = FindCraftingRecipes(skill, skillLevel);
            IEnumerable<CraftingRecipe> cookingRecipes = FindCookingRecipes(skill, skillLevel);

            LevelUpInfo info = new LevelUpInfo(skill, skillLevel, extraInformationLines, craftingRecipes, cookingRecipes, professions);

            return info;
        }

        IEnumerable<string> FindExtraInformationLines(string extraInfoData) {
            IList<string> extraInfo = new List<string>();

            string[] parts = extraInfoData.Split(',');
            foreach(string stringPath in parts) {
                string infoString = Game1.content.LoadString(stringPath);
                extraInfo.Add(infoString);
            }

            return extraInfo;
        }

        IEnumerable<CraftingRecipe> FindCraftingRecipes(Skill skill, int skillLevel) {
            ICollection<CraftingRecipe> recipes = new List<CraftingRecipe>();

            foreach (var recipeKVP in CraftingRecipe.craftingRecipes) {
                string[] parts = recipeKVP.Value.Split('/');

                if (parts.Length < FIELD_COUNT_CRAFTING_RECIPE) {
                    ModEntry.Instance.Monitor.Log($"Failed to parse recipe {recipeKVP.Key}: {recipeKVP.Value}", LogLevel.Warn);
                    continue;
                }

                parts = parts[FIELD_CRAFTING_RECIPE_SKILL].Split(' ');

                if (parts.Length < 2) {
                    ModEntry.Instance.Monitor.Log($"Failed to parse recipe {recipeKVP.Key}: {recipeKVP.Value}", LogLevel.Warn);
                    continue;
                }

                string recipeSkillName = parts[0];
                int recipeSkillLevel;
                if (int.TryParse(parts[1], out recipeSkillLevel)) {
                    ModEntry.Instance.Monitor.Log($"Failed to parse recipe {recipeKVP.Key}: {recipeKVP.Value}", LogLevel.Warn);
                    continue;
                }

                if (skill.Name == recipeSkillName && skillLevel == recipeSkillLevel) {
                    recipes.Add(new CraftingRecipe(recipeKVP.Key, false));
                }
            }

            return recipes;
        }

        IEnumerable<CraftingRecipe> FindCookingRecipes(Skill skill, int skillLevel) {
            ICollection<CraftingRecipe> recipes = new List<CraftingRecipe>();

            foreach (var recipeKVP in CraftingRecipe.cookingRecipes) {
                string[] parts = recipeKVP.Value.Split('/');

                if (parts.Length < FIELD_COUNT_COOKING_RECIPE) {
                    ModEntry.Instance.Monitor.Log($"Failed to parse recipe {recipeKVP.Key}: {recipeKVP.Value}", LogLevel.Warn);
                    continue;
                }

                parts = parts[FIELD_COOKING_RECIPE_SKILL].Split(' ');

                if (parts.Length < 2) {
                    ModEntry.Instance.Monitor.Log($"Failed to parse recipe {recipeKVP.Key}: {recipeKVP.Value}", LogLevel.Warn);
                    continue;
                }

                string recipeSkillName = parts[0];
                int recipeSkillLevel;
                if (int.TryParse(parts[1], out recipeSkillLevel)) {
                    ModEntry.Instance.Monitor.Log($"Failed to parse recipe {recipeKVP.Key}: {recipeKVP.Value}", LogLevel.Warn);
                    continue;
                }

                if (skill.Name == recipeSkillName && skillLevel == recipeSkillLevel) {
                    recipes.Add(new CraftingRecipe(recipeKVP.Key, true));
                }
            }

            return recipes;
        }

        IEnumerable<Profession> FindProfessions(Farmer who, string profsData) {

        }

    }
}
