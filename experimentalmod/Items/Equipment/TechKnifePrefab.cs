using System.IO;
using System.Reflection;
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
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("TechKnife", "Tech Knife", "Powerful knife that makes me go yes.")
            .WithIcon(SpriteManager.Get(TechType.HeatBlade));

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);
            var techKnifeObj = new CloneTemplate(Info, TechType.HeatBlade);

            techKnifeObj.ModifyPrefab += obj =>
            {
                // 1. ЛОГИКА КОМПОНЕНТОВ (Твой оригинальный код)
                var heatBlade = obj.GetComponent<HeatBlade>();
                var techKnife = obj.AddComponent<TechKnife>().CopyComponent(heatBlade);
                Object.DestroyImmediate(heatBlade);
                techKnife.damage *= 10f;

                // 2. ЗАМЕНА ТЕКСТУР (Логика O.S. TEAM)
                // Получаем путь к папке, где лежит твоя DLL
                string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                // Пути к твоим картинкам в папке Assets
                string diffusePath = Path.Combine(modPath, "Assets", "knife_01_hot.png");
                string illumPath = Path.Combine(modPath, "Assets", "knife_01_hot_illum.png");

                // Находим все части модели ножа
                var renderers = obj.GetComponentsInChildren<Renderer>(true);
                foreach (var renderer in renderers)
                {
                    // Меняем основную текстуру (цвет лезвия и рукояти)
                    if (File.Exists(diffusePath))
                    {
                        renderer.material.mainTexture = ImageUtils.LoadTextureFromFile(diffusePath);
                    }

                    // Меняем карту свечения (чтобы горел нужным цветом в темноте)
                    if (File.Exists(illumPath))
                    {
                        // В Subnautica этот параметр часто называется _Illum или _EmissionMap
                        renderer.material.SetTexture("_Illum", ImageUtils.LoadTextureFromFile(illumPath));
                    }
                }
            };

            customPrefab.SetGameObject(techKnifeObj);

            // Рецепт
            var recipe = new RecipeData(
                new Ingredient(TechType.Uranium, 2),
                new Ingredient(TechType.Diamond, 2),
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
            LanguageHandler.SetLanguageLine("EncyPath_Tech/Secrets", "Ancients");

            string keyancient = "alterrakey";
            PDAHandler.AddEncyclopediaEntry(
                keyancient,
                "Tech/Secrets",
                "Получен Доступ",
                "Внимание, вы только что незаконно получили доступ к конфиденциальной информации,\n составляющей коммерческую тайну компании «Альтерра».\nВ случае обнаружения данных в вашем КПК будет проведено расследование.\n"
            );
            StoryGoalHandler.RegisterItemGoal(keyancient, Story.GoalType.Encyclopedia, Info.TechType);

            string encyclopediaKey = "TechKnife_Info_Page";

            PDAHandler.AddEncyclopediaEntry(
                encyclopediaKey,
                "Tech/Secrets",
                "Неизвестный Нож",
                "Этот нож был обнаружен в заброшенных данных 'Альтерры'. Его лезвие настроено на гравитационный резонанс.\n Нож оснащён встроенным ускорителем, позволяющим нанести значительный урон.\n\nПредупреждение: использование против Левиафанов может привести к их внезапному исчезновению за горизонтом."
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