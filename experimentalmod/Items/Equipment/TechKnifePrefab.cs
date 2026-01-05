using System.IO;
using System.Reflection;
using experimentalmod.Items.Minerals;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Extensions;
using Nautilus.Handlers;
using Nautilus.Utility;
using Story;
using UnityEngine;

namespace experimentalmod.Items.Equipment
{
    public static class TechKnifePrefab
    {
        public static string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("ShadowKnife", "Теневой Нож", "тайная разработка альтерры")
            .WithIcon(SpriteManager.Get(TechType.HeatBlade));

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);
            var ShadowKnifeObj = new CloneTemplate(Info, TechType.HeatBlade);

            ShadowKnifeObj.ModifyPrefab += obj =>
            {

                var heatBlade = obj.GetComponent<HeatBlade>();
                var shadowKnife = obj.AddComponent<TechKnife>().CopyComponent(heatBlade);
                Object.DestroyImmediate(heatBlade);
                shadowKnife.damage *= 10f;

                string diffusePath = Path.Combine(modPath, "Assets", "knife_01_hot.png");
                string illumPath = Path.Combine(modPath, "Assets", "knife_01_hot_illum.png");


                var renderers = obj.GetComponentsInChildren<Renderer>(true);
                foreach (var renderer in renderers)
                {

                    if (File.Exists(diffusePath))
                    {
                        renderer.material.mainTexture = ImageUtils.LoadTextureFromFile(diffusePath);
                    }


                    if (File.Exists(illumPath))
                    {

                        renderer.material.SetTexture("_Illum", ImageUtils.LoadTextureFromFile(illumPath));
                    }
                }
            };

            customPrefab.SetGameObject(ShadowKnifeObj);

            // Рецепт
            var recipe = new RecipeData(
                new Ingredient(TechType.Uranium, 2),
                new Ingredient(UnknownMinerales.Info.TechType, 2),
                new Ingredient(TechType.HeatBlade, 1)
            );
            customPrefab.SetRecipe(recipe).WithFabricatorType(CraftTree.Type.Fabricator);

            // Настройки оборудования
            customPrefab.SetEquipment(EquipmentType.Hand);
            customPrefab.SetUnlock(TechType.HeatBlade);

            // Блок энциклопедии
            RegisterEncyclopedia();

            customPrefab.Register();
        }

        private static void RegisterEncyclopedia()
        {
            LanguageHandler.SetLanguageLine("EncyPath_Tech/Shadow Protocol", "PROJECT SHADOW");

            string keyancient = "alterrakey";
            PDAHandler.AddEncyclopediaEntry(
                keyancient,
                "Tech/Secrets",
                "СТАТУС:Теневой доступ",
                "<color=#ff0000ff>ВНИМАНИЕ:</color> Обнаружено незаконное проникновение в зашифрованный сектор данных.\n\n" +
                "Вы просматриваете файл под грифом <b>'Shadow-Protocol'</b>. Любая попытка передачи этих координат на сервера Альтерры будет заблокирована вашим ИИ.\n\n" +
                "<i>«Некоторые вещи должны оставаться во тьме».</i>"
            );
            StoryGoalHandler.RegisterItemGoal(keyancient, Story.GoalType.Encyclopedia, Info.TechType);

            string encyclopediaKey = "ShadowKnife_Info_Page";

            string shadowDescription =
                "<color=#555555ff>Прототип: Теневого Ножа </color>\n\n" +
                "Данный образец холодного оружия использует технологию <b>«светопоглощающего резонанса»</b>. " +
                "Лезвие ножа практически не отражает фотоны, что создает эффект «дрожащей тени» при движении.\n\n" +
                "<b>Технические особенности:</b>\n" +
                "• <b>Гравитационный ускоритель:</b> Позволяет лезвию игнорировать сопротивление воды.\n" +
                "• <b>Резонансный модуль:</b> Настроен на разрушение биологических связей на атомном уровне.\n\n" +
                "<color=#ffff00ff>ПРЕДУПРЕЖДЕНИЕ:</color>\n" +
                "При контакте с крупными биологическими формами (класс Левиафан) возникает эффект сверхмассового импульса. " +
                "Объект может быть мгновенно вытеснен из текущего пространства в неопределенном направлении.\n\n" +
                "<i>«Один удар — и тишина».</i>";

            PDAHandler.AddEncyclopediaEntry(
                encyclopediaKey,
                "Tech/Secrets",
                "Теневой Нож",
                shadowDescription
            );
            StoryGoalHandler.RegisterItemGoal(encyclopediaKey, Story.GoalType.Encyclopedia, Info.TechType);
        }
    }

    public class TechKnife : HeatBlade
    {
        public float hitForce = 500;
        public ForceMode forceMode = ForceMode.Acceleration;

        public override string animToolName { get; } = TechType.HeatBlade.AsString(true);

        public override void OnToolUseAnim(GUIHand hand)
        {
            base.OnToolUseAnim(hand);

            GameObject hitObj = null;
            Vector3 hitPosition = default;
            UWE.Utils.TraceFPSTargetPosition(Player.main.gameObject, attackDist, ref hitObj, ref hitPosition);
            if (!hitObj) return;

            var liveMixin = hitObj.GetComponentInParent<LiveMixin>();
            if (liveMixin && IsValidTarget(liveMixin))
            {
                var rigidbody = hitObj.GetComponentInParent<Rigidbody>();
                if (rigidbody)
                {
                    rigidbody.AddForce(MainCamera.camera.transform.forward * hitForce, forceMode);
                }
            }
        }
    }
}