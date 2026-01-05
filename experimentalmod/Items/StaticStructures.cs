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
        public static AssetBundle Bundle;
        public static string ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static PrefabInfo titanicInfo { get; } = PrefabInfo
            .WithTechType("TitanicStructure", "Titanic", "Огромный обломок древнего судна.");

        public static void Register()
        {
            string bundlePath = Path.Combine(ModPath, "Assets", "myassetbundle");

            if (!File.Exists(bundlePath)) return;


            if (Bundle == null)
                Bundle = AssetBundle.LoadFromFile(bundlePath);

            var titanicPrefab = CreateBasePrefab(titanicInfo, "Assets/GameObject.prefab");

            EncyPda();

            titanicPrefab.Register();

            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(titanicInfo.TechType, new Vector3(-1745f, -420f, 0f)));
        }

        private static void EncyPda()
        {
            string titanicEncy = "Titanic";

            // [ИЗМЕНЕНО] Здесь теперь твой расширенный текст
            string description = "Запись: Объект \"Титаник\" — Находка на глубине\n" +
                                 "\"Данное судно не числится в реестрах 'Альтерры' или других транскорпораций. Судя по анализу материалов, корпус состоит из примитивных сплавов железа и углерода, которые должны были сгнить за десятилетия, однако структура осталась нетронутой.\n" +
                                 "Сонар фиксирует странные пустоты внутри корпуса. Похоже, корабль переместился сюда не через гиперпространство, а буквально 'выпал' из локального временного разлома. Внутренние помещения заполнены органикой планеты 4546B, но в центральном отсеке всё еще слышны ритмичные звуки, напоминающие работу старого сервера...\"\n\n" +
                                 "Восстановленный лог: 14 апреля...\n" +
                                 "\"Мы видели лед, но теперь вокруг только бесконечная толща воды. Температура за бортом упала, но это не океан Земли. Датчики показывают аномальный уровень радиации и присутствие гигантских форм жизни.\n" +
                                 "Что-то бьется в обшивку с той стороны. Это не спасатели. Мы не знаем, как попали сюда, но корпус начинает сдаваться под давлением. Если кто-то найдет эту запись... знайте, 'Титаник' не утонул. Он просто сменил океан.\"";

            PDAHandler.AddEncyclopediaEntry(
                titanicEncy,
                "Tech/Secrets",
                "Старый Корабль",
                description
            );

            PDAHandler.AddCustomScannerEntry(
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