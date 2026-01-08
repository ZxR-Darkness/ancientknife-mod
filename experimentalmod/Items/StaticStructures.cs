using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Handlers;
using UnityEngine;
using System.IO;
using System.Reflection;
using Nautilus.Utility; 
namespace experimentalmod.Items
{
    public static class StaticStructures
    {
        public static PrefabInfo TitanicInfo { get; } = PrefabInfo
            .WithTechType("TitanicStructure", "Titanic", "Огромный обломок древнего судна.");

        public static PrefabInfo ServerInfo { get; } = PrefabInfo.WithTechType("ServerStructure", "Server", "Старый Сервер");

        public static void Register()
        {
            var titanicPrefab = CreateBasePrefab(TitanicInfo, "Assets/titanic.prefab");
            var serverPrefab = CreateBasePrefab(ServerInfo, "Assets/serverV3.prefab");
            EncyPda();

            serverPrefab.Register();
            titanicPrefab.Register();

            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(ServerInfo.TechType, new Vector3Int(0, 10, 0)));
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(TitanicInfo.TechType, new Vector3(-1745f, -420f, 0f)));


            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(TechType.PrecursorIonCrystal, new Vector3(0, 30, 0)));
        }

        private static void EncyPda()
        {
            LanguageHandler.SetLanguageLine("EncyPath_Tech/Shadow Protocol", "PROJECT SHADOW");
            string titanicEncy = "Titanic";

            string description = "Запись: Объект \"Титаник\" — Находка на глубине\n" +
                                 "\"Данное судно не числится в реестрах 'Альтерры' или других транскорпораций. Судя по анализу материалов, корпус состоит из примитивных сплавов железа и углерода, которые должны были сгнить за десятилетия, однако структура осталась нетронутой.\n" +
                                 "Сонар фиксирует странные пустоты внутри корпуса. Похоже, корабль переместился сюда не через гиперпространство, а буквально 'выпал' из локального временного разлома. Внутренние помещения заполнены органикой планеты 4546B, но в центральном отсеке всё еще слышны ритмичные звуки, напоминающие работу старого сервера...\"\n\n" +
                                 "Восстановленный лог: 14 апреля...\n" +
                                 "\"Мы видели лед, но теперь вокруг только бесконечная толща воды. Температура за бортом упала, но это не океан Земли. Датчики показывают аномальный уровень радиации и присутствие гигантских форм жизни.\n" +
                                 "Что-то бьется в обшивку с той стороны. Это не спасатели. Мы не знаем, как попали сюда, но корпус начинает сдаваться под давлением. Если кто-то найдет эту запись... знайте, 'Титаник' не утонул. Он просто сменил океан.\"";

            PDAHandler.AddEncyclopediaEntry(
                titanicEncy,
                "Tech/Shadow Protocol",
                "Старый Корабль",
                description
            );

            string serverEncy = "Server";
            PDAHandler.AddEncyclopediaEntry(
                serverEncy,
                "Tech/Shadow Protocol",
                "TEST",
                "TEST"
             );

            PDAHandler.AddCustomScannerEntry(
                TitanicInfo.TechType,
                scanTime: 2f,
                destroyAfterScan: false,
                encyclopediaKey: titanicEncy
            );
            PDAHandler.AddCustomScannerEntry(
                ServerInfo.TechType,
                scanTime: 2f,
                destroyAfterScan: true,
                encyclopediaKey: serverEncy
            );
        }

        private static CustomPrefab CreateBasePrefab(PrefabInfo info, string assetPath)
        {
            var customPrefab = new CustomPrefab(info);

            customPrefab.SetGameObject(() =>
            {
                GameObject prefab = Plugin.Bundle.LoadAsset<GameObject>(assetPath);
                if (prefab == null) return new GameObject("Empty (Bundle Error)");

                GameObject instance = Object.Instantiate(prefab);


                MaterialUtils.ApplySNShaders(instance);

                instance.AddComponent<PrefabIdentifier>().ClassId = info.ClassID;


                var lwe = instance.EnsureComponent<LargeWorldEntity>();
                lwe.cellLevel = LargeWorldEntity.CellLevel.Global;

                var rb = instance.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                return instance;
            });

            return customPrefab;
        }
    }
}