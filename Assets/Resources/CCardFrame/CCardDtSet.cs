using System.Collections.Generic;

namespace ULZAsset {
    public class CardSet {
        public int id { get; set; }
        public int level { get; set; }
        public int hp { get; set; }
        public int ap { get; set; }

        public int dp { get; set; }
        public int rarity { get; set; }
        public int deck_cost { get; set; }
        public int slot { get; set; }
        public ImgSet stand_image { get; set; }
        public ImgSet chara_image { get; set; }
        public ImgSet artifact_image { get; set; }
        public ImgSet bg_image { get; set; }
        public int next_id { get; set; }
        public int kind { get; set; }
        public string created_at { get; set; }
        public object skill { get; set; }
        public List<int> skill_pointer { get; set; }
    }

    public class MultiLang {
        public string jp { get; set; }
        public string en { get; set; }
        public string fr { get; set; }
        public string kr { get; set; }
        public string scn { get; set; }
        public string tcn { get; set; }
        public string ina { get; set; }
        public string thai { get; set; }
    }

    public class CardObject {
        public int id { get; set; }
        public List<CardSet> card_set { get; set; }
        public MultiLang name { get; set; }
        public MultiLang ab_name { get; set; }
        public MultiLang caption { get; set; }
    }

    public class SkillObject {
        public int id { get; set; }
        public int feat_no { get; set; }
        public int pow { get; set; }
        public string dice_attribute { get; set; }
        public string effect_image { get; set; }
        public string condition { get; set; }
        public string created_at { get; set; }
        public MultiLang name { get; set; }
        public MultiLang caption { get; set; }
    }
    public class StatusObject {
        public int id { get; set; }
        public string img { get; set; }
        public string sprite { get; set; }
        public MultiLang name { get; set; }
        public MultiLang caption { get; set; }
    }
    public class ImgSet {
        public string name ;
        public int height;
        public int width;
        public string type;
    }
}