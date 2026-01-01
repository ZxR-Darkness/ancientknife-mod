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
        //private static string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.Location), "Assets");

        private void Awake()
        {
            Logger = base.Logger;
            //string bundlePath = Path.Combine(AssetsFolder, "myassetbundle");

            //if (File.Exists(bundlePath))
            //{
            //    StaticStructures.Bundle = AssetBundle.LoadFromFile(bundlePath);
            //    Logger.LogInfo("Бандл успешно загружен в память!");
            //}
            //else
            //{
            //    Logger.LogError($"Файл бандла не найден! Проверь путь: {bundlePath}");
            //    return;
            //}

            InitializePrefabs();
            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void InitializePrefabs()
        {
            //var shadowLevInfo = PrefabInfo.WithTechType("ShadowLev", "Shadow Leviathan", "Теневой ужас.");
            //var shadowLev = new ShadowLeviathan(shadowLevInfo);
            //shadowLev.Register();
            StaticStructures.Register();
            UnknownMinerales.Register();
            TechKnifePrefab.Register();
            ShadowRebreather.Register();
        }
    }
}