using BepInEx;
using BepInEx.Logging;
using ECCLibrary;
using experimentalmod.Items;
using experimentalmod.Items.Equipment;
using experimentalmod.Items.Minerals;
using HarmonyLib;
using Nautilus.Assets;
using Nautilus.Handlers;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace experimentalmod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.lee23.ecclibrary")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }
        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        public static AssetBundle Bundle;
        public static string ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private void Awake()
        {
            Logger = base.Logger;
            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");

            string bundlePath = Path.Combine(ModPath, "Assets", "assetbundlev2");

            if (File.Exists(bundlePath))
            {
                Bundle = AssetBundle.LoadFromFile(bundlePath);
                Logger.LogInfo("AssetBundle успешно загружен!");
            }
            else
            {
                Logger.LogError($"Файл бандла не найден по пути: {bundlePath}");
            }

            InitializePrefabs();

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void InitializePrefabs()
        {
            StaticStructures.Register();
            UnknownMinerales.Register();
            TechKnifePrefab.Register();
            //ShadowRebreather.Register();
            DeepLeviathan.RegisterEntity();
        }
    }
}