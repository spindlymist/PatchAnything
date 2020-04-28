using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchAnything.SkillsAndProfessions {
    class Skill {

        public int ID { get { return id; } }
        public string Name { get { return name; } }

        int id;
        string name;
        IList<Profession> professions;

        public Skill(int id, string name) {
            this.id = id;
            this.name = name;
        }

        public void AddProfession(Profession prof) {
            professions.Add(prof);
        }

    }
}
