using ULZAsset.ProtoMod.GameDuelService;

namespace ULZAsset.MsgExtension {
    [System.Serializable]
    public class RmChangeCharCard {
        public string user_id;
        public string side;
        public int charcard_id;
        public int cardset_id;
        public int level;
    }

    public static class GDMsg {
        public static ECShortHand ConvertToEC(EventCard ec) {
            return new ECShortHand {
                CardId = ec.Id,
                Position = ec.Position,
                IsInvert = ec.IsInvert,
            };
        }
    }
}