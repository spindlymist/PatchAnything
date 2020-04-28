using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchAnything.SkillsAndProfessions {
    class Profession {

        public int ID { get; }
        public string Name { get; }
        public int SkillID { get; }
        public int SkillLevel { get; }
        public int[] Prerequisites { get; }

        public Profession(int id, string name, int skillID, int skillLevel, int[] prerequisites) {
            ID = id;
            Name = name;
            SkillID = skillID;
            SkillLevel = skillLevel;
            Prerequisites = prerequisites;
        }

    }
}
