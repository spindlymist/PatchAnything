using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace PatchAnything.SkillsAndProfessions {
    class SkillsAndProfessionsDataManager {

        // Skill Format          "id": "name"
        // Example               "0": "Farming"
        const int FIELD_COUNT_SKILLS = 1;
        const int FIELD_SKILLS_NAME  = 0;

        // Profession Format     "id": "name/skill_id/skill_level/prereq0,prereq1,..."
        // Example               "4": "Artisan/0/10/1"
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
        IDictionary<int, Profession> professions = new Dictionary<int, Profession>();

        public SkillsAndProfessionsDataManager() { }

        public void LoadData(bool forceReload = false) {
            if (!forceReload && skills.Count != 0) {
                return;
            }

            skills.Clear();
            professions.Clear();

            IDictionary<int, string> skillsData = ModEntry.Instance.Helper.Content.Load<Dictionary<int, string>>("Data/Skills", ContentSource.GameContent);
            ModEntry.Instance.Monitor.Log($"Found {skillsData.Count} skill entries", LogLevel.Info);
            foreach (var skillKVP in skillsData) {
                ModEntry.Instance.Monitor.Log($"Parsing {skillKVP.Key}: {skillKVP.Value}", LogLevel.Info);
                Skill skill;
                if (ParseSkill(skillKVP.Key, skillKVP.Value, out skill)) {
                    ModEntry.Instance.Monitor.Log($"Sucessfully parsed skill: {skill.Name}", LogLevel.Info);
                    skills.Add(skill.ID, skill);
                }
                else {
                    ModEntry.Instance.Monitor.Log($"Failed to parse skill {skillKVP.Key}: {skillKVP.Value}", LogLevel.Warn);
                }
            }

            IDictionary<int, string> profsData = ModEntry.Instance.Helper.Content.Load<Dictionary<int, string>>("Data/Professions", ContentSource.GameContent);
            ModEntry.Instance.Monitor.Log($"Found {skillsData.Count} prof entries", LogLevel.Info);
            foreach (var profKVP in profsData) {
                ModEntry.Instance.Monitor.Log($"Parsing {profKVP.Key}: {profKVP.Value}", LogLevel.Info);
                Profession prof;
                if (ParseProfession(profKVP.Key, profKVP.Value, out prof)) {
                    ModEntry.Instance.Monitor.Log($"Sucessfully parsed prof: {prof.Name}", LogLevel.Info);
                    professions.Add(prof.ID, prof);
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
            return professions.TryGetValue(id, out Profession prof) ? prof : null;
        }

        public LevelUpInfo GetLevelUpInfo(Farmer who, Skill skill, int skillLevel) {
            string dataKey = $"{skill.ID}/{skillLevel}";
            IDictionary<string, string> levelUpData = ModEntry.Instance.Helper.Content.Load<Dictionary<string, string>>("Data/LevelUps", ContentSource.GameContent);

            ICollection<string> extraInformationLines = null;
            ICollection<Profession> professions = null;

            if (levelUpData.TryGetValue(dataKey, out string data)) {
                string[] parts = data.Split('/');

                if(parts.Length < FIELD_COUNT_LEVELS) {
                    ModEntry.Instance.Monitor.Log($"Failed to parse level up {dataKey}: {data}", LogLevel.Info);
                }
                else {
                    extraInformationLines = FindExtraInformationLines(parts[FIELD_LEVELS_EXTRA_INFO]);
                    professions = FindProfessions(who, parts[FIELD_LEVELS_PROFS]);
                    // TODO handle mail
                }
            }

            int bigCraftableCount = 0;
            List<CraftingRecipe> recipes = FindRecipes(skill, skillLevel, false, ref bigCraftableCount);
            recipes.AddRange(FindRecipes(skill, skillLevel, true, ref bigCraftableCount));

            LevelUpInfo info = new LevelUpInfo(skill, skillLevel, extraInformationLines, recipes, bigCraftableCount, professions);

            return info;
        }

        ICollection<string> FindExtraInformationLines(string extraInfoData) {
            IList<string> extraInfo = new List<string>();

            string[] parts = extraInfoData.Split(',');
            foreach (string stringPath in parts) {
                string infoString = Game1.content.LoadString(stringPath);
                extraInfo.Add(infoString);
            }

            return extraInfo;
        }

        List<CraftingRecipe> FindRecipes(Skill skill, int skillLevel, bool cooking, ref int bigCraftableCount) {
            Dictionary<string, string> allRecipes = cooking ? CraftingRecipe.cookingRecipes : CraftingRecipe.craftingRecipes;
            int recipeSourceIndex = cooking ? FIELD_COOKING_RECIPE_SKILL : FIELD_CRAFTING_RECIPE_SKILL;

            List<CraftingRecipe> recipes = new List<CraftingRecipe>();

            foreach (var recipeKVP in allRecipes) {
                string[] parts = recipeKVP.Value.Split('/');

                if (parts.Length < recipeSourceIndex) {
                    ModEntry.Instance.Monitor.Log($"Failed to parse recipe {recipeKVP.Key}: {recipeKVP.Value}", LogLevel.Warn);
                    continue;
                }

                parts = parts[recipeSourceIndex].Split(' ');
                bool hasLeadingChar = parts[0].Length == 1;

                if (parts.Length < 2 || (hasLeadingChar && parts.Length < 3)) {
                    ModEntry.Instance.Monitor.Log($"Failed to parse recipe {recipeKVP.Key}: {recipeKVP.Value}", LogLevel.Warn);
                    continue;
                }

                string recipeSkillName = parts[hasLeadingChar ? 1 : 0];
                int recipeSkillLevel;
                if (!int.TryParse(parts[hasLeadingChar ? 2 : 1], out recipeSkillLevel)) {
                    ModEntry.Instance.Monitor.Log($"Failed to parse recipe {recipeKVP.Key}: {recipeKVP.Value}", LogLevel.Warn);
                    continue;
                }

                if (skill.Name == recipeSkillName && skillLevel == recipeSkillLevel) {
                    CraftingRecipe recipe = new CraftingRecipe(recipeKVP.Key, cooking);
                    recipes.Add(recipe);

                    if(recipe.bigCraftable) {
                        bigCraftableCount++;
                    }
                }
            }

            return recipes;
        }

        ICollection<Profession> FindProfessions(Farmer who, string profsData) {
            IList<Profession> profs = new List<Profession>();
            IEnumerable<int> profIDs = ParseIntList(profsData);
            
            foreach(int id in profIDs) {
                if(professions.TryGetValue(id, out Profession prof)) {
                    bool hasPrereqs = true;

                    foreach(int prereq in prof.Prerequisites) {
                        if(!who.professions.Contains(prereq)) {
                            hasPrereqs = false;
                            break;
                        }
                    }

                    if(hasPrereqs) {
                        profs.Add(prof);
                    }
                }
            }

            return profs;
        }

    }
}
