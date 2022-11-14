using BepInEx;
using HarmonyLib;

namespace GolemAutomation
{
    [BepInPlugin(
        "de.benediktwerner.stacklands.golemautomation",
        PluginInfo.PLUGIN_NAME,
        PluginInfo.PLUGIN_VERSION
    )]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;

        private void Awake()
        {
            Instance = this;
            Harmony.CreateAndPatchAll(typeof(Patches));
        }
    }
}
