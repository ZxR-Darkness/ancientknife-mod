using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Handlers;
using UnityEngine;
using System.IO;
using System.Reflection;

namespace experimentalmod.Items
{
    public static class StaticStructures
    {
        public static AssetBundle Bundle;
        public static string ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static PrefabInfo titanicInfo { get; } = PrefabInfo
            .WithTechType("TitanicStructure", "Titanic", "Огромный обломок древнего судна.");
        public static void Register()
        {
            string bundlePath = Path.Combine(ModPath, "Assets", "myassetbundle");

            if (!File.Exists(bundlePath)) return;
            Bundle = AssetBundle.LoadFromFile(bundlePath);
            var titanicPrefab = CreateBasePrefab(titanicInfo, "Assets/GameObject.prefab");

            EncyPda();

            titanicPrefab.Register();

            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(titanicInfo.TechType, new Vector3(-1745f, -420f, 0f)));
        }


        private static void EncyPda()
        {
            string titanicEncy = "Titanic";

            PDAHandler.AddEncyclopediaEntry(
                titanicEncy,
                "Tech/Secrets",
                "Старый Корабль",
                "корабль который неизвестным образом оказался на планете 4546B...\n 'Альтерра' не стала коментировать данный обьект"

            );

            PDAHandler.AddCustomScannerEntry
            (
            titanicInfo.TechType,
            scanTime: 2f,
            destroyAfterScan: false,
            encyclopediaKey: titanicEncy
            );
        }
        private static CustomPrefab CreateBasePrefab(PrefabInfo info, string assetPath)
        {
            var customPrefab = new CustomPrefab(info);

            customPrefab.SetGameObject(() =>
            {
                GameObject prefab = Bundle.LoadAsset<GameObject>(assetPath);
                if (prefab == null) return new GameObject("Empty (Bundle Error)");

                GameObject instance = Object.Instantiate(prefab);

                instance.AddComponent<PrefabIdentifier>().ClassId = info.ClassID;
                var lwe = instance.EnsureComponent<LargeWorldEntity>();
                lwe.cellLevel = LargeWorldEntity.CellLevel.Far;
                var rb = instance.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                return instance;
            });

            return customPrefab;
        }
    }
}