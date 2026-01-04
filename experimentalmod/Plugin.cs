using BepInEx;
using BepInEx.Logging;
using experimentalmod.Items;
using experimentalmod.Items.Equipment;
using experimentalmod.Items.Minerals;
using HarmonyLib;
using Nautilus.Assets;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace experimentalmod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }
        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        private static string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.Location), "Assets");

        private void Awake()
        {
            Logger = base.Logger;
            InitializePrefabs();
            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void InitializePrefabs()
        {
            StaticStructures.Register();
            UnknownMinerales.Register();
            TechKnifePrefab.Register();
            ShadowRebreather.Register();
        }
    }
}