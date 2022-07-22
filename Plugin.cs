using BepInEx;
using HarmonyLib;

namespace GolemAutomation
{
    [BepInPlugin("de.benediktwerner.stacklands.golemautomation", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(Patches));
        }
    }
}
