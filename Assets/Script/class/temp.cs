using System.Collections;
using System.Collections.Generic;
public enum pos_type { close_dist, middle_dist, long_dist }
public enum cond_oper { equal, greater, less }
public enum stage_parse { draw, move, attack, defeals }
//  NOTE impletement rust_libs later @Rust

public class EvntCond {
    public type_opt opt;
    public int val;
    public cond_oper cond;
}
public class CC_Skill {
    public string Id;
    public string Name;
    public string Desp;
    public List<pos_type> AvailPos;
    public EvntCond Condition;
    public stage_parse in_parse;
    public SkillEffect effect;
}
public class SkillEffect {
    public int self_dmg;
    public int duel_dmg;
}
public class Status {

}