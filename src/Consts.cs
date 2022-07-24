namespace GolemAutomation
{
    static class Consts
    {
        public const string PREFIX = "golemautomation.";
        public const string IDEA = "_blueprint";

        public const string PAPER = PREFIX + "paper";
        public const string STORAGE_PLACE = PREFIX + "storage";
        public const string FILTER = PREFIX + "filter";
        public const string LOCATION_GLYPH = PREFIX + "location_glyph";
        public const string GOLEM = PREFIX + "golem";
        public const string GOLEM_L = GOLEM + "_l";
        public const string GOLEM_XL = GOLEM + "_xl";
        public const string GOLEM_MOD_SELL = GOLEM + "_module_sell";
        public const string GOLEM_MOD_SPEED = GOLEM + "_module_speed";
        public const string GOLEM_MOD_COUNTER  = GOLEM + "_module_counter";
        public const string GOLEM_MOD_CRAFTER  = GOLEM + "_module_crafter";

        public const string SUGAR = "sugar";
        public const string STICK = "stick";
        public const string ROPE = "rope";
        public const string CHARCOAL = "charcoal";
        public const string IRON_BAR = "iron_bar";
        public const string ANY_VILL = "any_villager";

        public static string Idea(string id)
        {
            return id + IDEA;
        }
    }
}
