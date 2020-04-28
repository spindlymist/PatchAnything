using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchAnything.SkillsAndProfessions {
    class Profession {

        public int ID { get { return id; } }
        public string Name { get { return name; } }
        public int SkillID { get { return skillID; } }

        int id;
        string name;
        int skillID;

        public Profession(int id, string name, int skillID) {
            this.id = id;
            this.name = name;
            this.skillID = skillID;
        }

    }
}
