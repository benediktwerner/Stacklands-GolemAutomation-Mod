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
        public const string AREA_GLYPH = PREFIX + "_area_glyph";
        public const string CRASHED_SPACESHIP = PREFIX + "crashed_spaceship";
        public const string ENERGY_COMBOBULATOR = PREFIX + "energy_combobulator";
        public const string ENERGY_CORE = PREFIX + "energy_core";
        public const string BROKEN_GOLEM = PREFIX + "broken_golem";
        public const string BROKEN_GOLEM_XL = PREFIX + "broken_golem_xl";
        public const string GOLEM = PREFIX + "golem";
        public const string GOLEM_L = GOLEM + "_l";
        public const string GOLEM_XL = GOLEM + "_xl";
        public const string GOLEM_XL_LEFT_ARM = GOLEM + "_xl_left_arm";
        public const string GOLEM_XL_RIGHT_ARM = GOLEM + "_xl_right_arm";
        public const string GOLEM_XL_LEGS = GOLEM + "_xl_legs";
        public const string GOLEM_MOD = GOLEM + "_module";
        public const string GOLEM_MOD_SELL = GOLEM_MOD + "_sell";
        public const string GOLEM_MOD_SPEED = GOLEM_MOD + "_speed";
        public const string GOLEM_MOD_COUNTER = GOLEM_MOD + "_counter";
        public const string GOLEM_MOD_CRAFTER = GOLEM_MOD + "_crafter";

        public const string SUGAR = "sugar";
        public const string SUGAR_CANE = "sugar_cane";
        public const string FLINT = "flint";
        public const string STICK = "stick";
        public const string ROPE = "rope";
        public const string CHARCOAL = "charcoal";
        public const string IRON_BAR = "iron_bar";
        public const string GOLD_BAR = "gold_bar";
        public const string GLASS = "glass";
        public const string BRICK = "brick";
        public const string PLANK = "plank";
        public const string COIN = "gold";
        public const string SHELL = "shell";
        public const string JUNGLE = "jungle";
        public const string ANY_VILL = "any_villager";
        public const string OLD_TOME = "old_tome";

        public const string MAINLAND = "main";
        public const string ISLAND = "island";

        public const BlueprintGroup BLUEPRINT_GROUP_GOLEM = (BlueprintGroup)449;

        public static string Idea(string id)
        {
            return id + IDEA;
        }
    }
}
