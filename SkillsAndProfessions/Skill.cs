using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatchAnything.SkillsAndProfessions {
    class Skill {

        public int ID { get; }
        public string Name { get; }

        public Skill(int id, string name) {
            ID = id;
            Name = name;
        }

    }
}
