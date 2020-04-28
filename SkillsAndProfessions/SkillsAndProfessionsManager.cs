using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace PatchAnything.SkillsAndProfessions {
    class SkillsAndProfessionsManager {

        private const int FIELD_COUNT_SKILLS = 1;
        private const int FIELD_SKILLS_NAME = 0;

        private const int FIELD_COUNT_PROFS = 2;
        private const int FIELD_PROFS_NAME = 0;
        private const int FIELD_PROFS_SKILL_ID = 1;

        private static readonly SkillsAndProfessionsManager instance = new SkillsAndProfessionsManager();
        IDictionary<int, Skill> skills = new Dictionary<int, Skill>();
        IDictionary<int, Profession> profs = new Dictionary<int, Profession>();

        public static SkillsAndProfessionsManager Instance {
            get { return instance; }
        }

        static SkillsAndProfessionsManager() { // Prevent beforefieldinit flag
        }

        SkillsAndProfessionsManager() { }

        public void LoadData(bool forceReload = false) {
            if(!forceReload && skills.Count != 0) {
                return;
            }

            skills.Clear();
            profs.Clear();

            IDictionary<int, string> skillsData = ModEntry.Helper.Content.Load<Dictionary<int, string>>("Data/Skills.json", ContentSource.GameContent);
            foreach(var skillKVP in skillsData) {
                Skill skill;
                if(ParseSkill(skillKVP.Key, skillKVP.Value, out skill)) {
                    skills.Add(skill.ID, skill);
                }
                else {
                    ModEntry.Log($"Failed to parse skill {skillKVP.Key}: {skillKVP.Value}", LogLevel.Warn);
                }
            }

            IDictionary<int, string> profsData = ModEntry.Helper.Content.Load<Dictionary<int, string>>("Data/Professions.json", ContentSource.GameContent);
            foreach(var profKVP in profsData) {
                Profession prof;
                if(ParseProfession(profKVP.Key, profKVP.Value, out prof)) {
                    skills[prof.SkillID].AddProfession(prof);
                    profs.Add(prof.ID, prof);
                }
                else {
                    ModEntry.Log($"Failed to parse profession {profKVP.Key}: {profKVP.Value}", LogLevel.Warn);
                }
            }
        }

        bool ParseSkill(int key, string value, out Skill skill) {
            skill = null;
            string[] parts = value.Split('/');

            if(parts.Length < FIELD_COUNT_SKILLS) {
                return false;
            }

            string skillName = parts[FIELD_SKILLS_NAME];
            if(skillName.Length < 1) {
                return false;
            }

            skill = new Skill(key, skillName);

            return true;
        }

        bool ParseProfession(int key, string value, out Profession prof) {
            prof = null;
            string[] parts = value.Split('/');

            if(parts.Length < FIELD_COUNT_PROFS) {
                return false;
            }

            int skillID;
            if(!int.TryParse(parts[FIELD_PROFS_SKILL_ID], out skillID) || !skills.ContainsKey(skillID)) {
                return false;
            }

            string profName = parts[FIELD_PROFS_NAME];
            if(profName.Length < 1) {
                return false;
            }

            prof = new Profession(key, profName, skillID);

            return true;
        }



    }
}
