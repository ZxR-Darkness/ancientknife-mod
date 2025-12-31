using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using experimentalmod.Items.Equipment;
using experimentalmod.Items;
using experimentalmod.Items.Minerals;
using HarmonyLib;

namespace experimentalmod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }
        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();

        private void Awake()
        {
            Logger = base.Logger;

            InitializePrefabs();

            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void InitializePrefabs()
        {
            // Достаточно вызвать только это. 
            // Внутри YeetKnifePrefab.Register() мы уже добавили StoryGoalHandler.RegisterItemGoal
            TechKnifePrefab.Register();
            //AlterraBlackBox.Register();
            ShadowRebreather.Register();
            UnknownMinerales.Register();
        }
    }
}