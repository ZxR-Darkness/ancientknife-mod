using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Extensions;
using Nautilus.Handlers;
using Story;
using UnityEngine;
using BepInEx.Logging;
using experimentalmod.Items.Equipment;
using HarmonyLib;
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
                var heatBlade = obj.GetComponent<HeatBlade>();
                var techKnife = obj.AddComponent<TechKnife>().CopyComponent(heatBlade);
                Object.DestroyImmediate(heatBlade);
                techKnife.damage *= 4f;
            };
            customPrefab.SetGameObject(techKnifeObj);

            var recipe = new RecipeData(new Ingredient(TechType.Uranium, 2), new Ingredient(TechType.Diamond, 2), new Ingredient(TechType.HeatBlade, 1));
            customPrefab.SetRecipe(recipe)
                .WithFabricatorType(CraftTree.Type.Fabricator);
            customPrefab.SetEquipment(EquipmentType.Hand);
            customPrefab.Register();

            string encyclopediaKey = "TechKnife_Info_Page";
            LanguageHandler.SetLanguageLine("EncyPath_Tech/Secrets", "Ancients");
            PDAHandler.AddEncyclopediaEntry(
                encyclopediaKey,
                "Tech/Secrets",
                "Неизвестный Нож",
                "Этот нож был обнаружен в заброшенных данных 'Альтерры'. Его лезвие настроено на гравитационный резонанс.\n Нож оснащён встроенным ускорителем, позволяющим нанести значительный урон.\n\nПредупреждение: использование против Левиафанов может привести к их внезапному исчезновению за горизонтом."

            );
            StoryGoalHandler.RegisterItemGoal(encyclopediaKey, Story.GoalType.Encyclopedia, Info.TechType);

            string keyancient = "alterrakey";
            PDAHandler.AddEncyclopediaEntry(

                keyancient,
                "Tech/Secrets",
                "Получен Доступ",
                "Внимание, вы только что незаконно получили доступ к конфиденциальной информации,\n составляющей коммерческую тайну компании «Альтерра».\nВ случае обнаружения данных в вашем КПК будет проведено расследование.\n"
                );
            StoryGoalHandler.RegisterItemGoal(keyancient, Story.GoalType.Encyclopedia, Info.TechType);
        }
        
    }

    public class TechKnife : HeatBlade
    {
        public float hitForce = 1000;
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