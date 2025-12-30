//using Nautilus.Assets;
//using Nautilus.Assets.PrefabTemplates;
//using Nautilus.Handlers;
//using HarmonyLib;
//using UnityEngine;
//using experimentalmod.Items.Equipment;

//namespace experimentalmod.Items
//{
//    // ===============================
//    // ПРЕФАБ ИОННОГО КУБА
//    // ===============================
//    public static class AlterraBlackBox
//    {
//        public static PrefabInfo Info { get; } = PrefabInfo
//            .WithTechType(
//                "AlterraBlackBox",
//                "Ионный архив",
//                "Модифицированный ионный куб. При подборе открывает секретные чертежи."
//            )
//            .WithIcon(SpriteManager.Get(TechType.PrecursorIonCrystal));

//        public static void Register()
//        {
//            var prefab = new CustomPrefab(Info);

//            var clone = new CloneTemplate(Info, TechType.PrecursorIonCrystal);
//            prefab.SetGameObject(clone);

//            prefab.Register();
//        }
//    }

//    // ===============================
//    // HARMONY: ПЕРЕХВАТ ДОБАВЛЕНИЯ В ИНВЕНТАРЬ
//    // ===============================
//    [HarmonyPatch(typeof(Inventory), nameof(Inventory.AddItem))]
//    internal static class AlterraBlackBox_AddItemPatch
//    {
//        private static bool unlocked;

//        private static void Postfix(Inventory __instance, Pickupable item)
//        {
//            if (unlocked || item == null)
//                return;

//            if (item.GetTechType() != AlterraBlackBox.Info.TechType)
//                return;

//            unlocked = true;

//            KnownTech.Add(TechKnifePrefab.Info.TechType, true);
//            KnownTech.Add(ShadowRebreather.Info.TechType, true);

//            ErrorMessage.AddMessage(
//                "Секретные чертежи загружены в КПК."
//            );
//        }
//    }
//}
