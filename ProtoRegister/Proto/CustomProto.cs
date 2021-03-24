using UnityEngine;

namespace ProtoRegister.Proto {
    
    public class StringProtoJP : StringProto {
        public string JAJP;

        public StringProtoJP() {}
        
        public StringProtoJP(int id, string key, string enus, string jajp) : this(id, key, enus, enus, enus, jajp) {}

        public StringProtoJP(int id, string key, string zhcn, string enus, string frfr, string jajp) {
            ID = id;
            Name = key;
            name = key;
            ZHCN = zhcn;
            ENUS = enus;
            FRFR = frfr;
            JAJP = jajp;
        }
    }

    public class CustomRecipeProto : RecipeProto {
        public int preTechID;
    }
}