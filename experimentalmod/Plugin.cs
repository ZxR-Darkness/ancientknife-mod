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
    [BepInDependency("com.lee23.ecclibrary")] // ДОБАВЬ ЭТУ СТРОКУ, чтобы игра знала, что нам нужна ECCLibrary
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
            var shadowLevInfo = PrefabInfo.WithTechType("ShadowLev", "Shadow Leviathan", "Теневой ужас.")
                .WithIcon(SpriteManager.Get(TechType.ReaperLeviathan));

            var shadowLev = new ShadowLeviathan(shadowLevInfo);
            shadowLev.Register();

            ShadowLeviathan.SetupEncyclopedia(shadowLevInfo.TechType);
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(shadowLevInfo.TechType, new Vector3(1500f, -200f, 0f)));
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(shadowLevInfo.TechType, new Vector3(-1500f, -300f, 500f)));
        }
    }
}