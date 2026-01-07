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

        private void Awake()
        {
            Logger = base.Logger;

            // Сначала патчим гармонию
            Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");

            // Затем инициализируем префабы
            InitializePrefabs();

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void InitializePrefabs()
        {
            StaticStructures.Register();
            UnknownMinerales.Register();
            TechKnifePrefab.Register();
            ShadowRebreather.Register();

            // Загрузка рыб))
            // 1. DEEP LEVIATHAN
            var DeepLevInfo = PrefabInfo.WithTechType("Deeplev", "Deep Leviathan", "Глубинный ужас.")
                .WithIcon(SpriteManager.Get(TechType.ReaperLeviathan));
            var DeepLev = new DeepLeviathan(DeepLevInfo);
            DeepLev.Register();
            DeepLeviathan.SetupEncyclopedia(DeepLevInfo.TechType);
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(DeepLevInfo.TechType, new Vector3(1800f, -100f, 0f)));
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(DeepLevInfo.TechType, new Vector3(-2000f, -100f, 0f)));
        }
    }
}