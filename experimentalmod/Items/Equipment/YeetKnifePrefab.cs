using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Extensions;
using Nautilus.Handlers;
using Story;
using UnityEngine;
using BepInEx.Logging;
using System.Reflection;
using BepInEx;
using experimentalmod.Items.Equipment;
using HarmonyLib;

namespace experimentalmod.Items.Equipment
{
    public static class YeetKnifePrefab
    {
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("YeetKnife", "Yeet Knife", "Powerful knife that makes me go yes.")
            .WithIcon(SpriteManager.Get(TechType.HeatBlade));

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);

            var yeetKnifeObj = new CloneTemplate(Info, TechType.HeatBlade);
            yeetKnifeObj.ModifyPrefab += obj =>
            {
                var heatBlade = obj.GetComponent<HeatBlade>();
                var yeetKnife = obj.AddComponent<YeetKnife>().CopyComponent(heatBlade);
                Object.DestroyImmediate(heatBlade);
                yeetKnife.damage *= 4f;
            };
            customPrefab.SetGameObject(yeetKnifeObj);

            var recipe = new RecipeData(new Ingredient(TechType.Titanium, 4), new Ingredient(TechType.Gold, 2));
            customPrefab.SetRecipe(recipe)
                .WithFabricatorType(CraftTree.Type.Fabricator);
            customPrefab.SetEquipment(EquipmentType.Hand);
            customPrefab.Register();

            string encyclopediaKey = "YeetKnife_Info_Page";
            PDAHandler.AddEncyclopediaEntry(
                encyclopediaKey,
                "Tech/Equipment",
                "[Deleted knife]",
                "Этот нож был обнаружен в заброшенных данных 'Альтерры'. Его лезвие настроено на гравитационный резонанс.\n\nПредупреждение: использование против Левиафанов может привести к их внезапному исчезновению за горизонтом."

            );
            StoryGoalHandler.RegisterItemGoal(encyclopediaKey, Story.GoalType.Encyclopedia, Info.TechType);

            string keyancient = "alterrakey";
            PDAHandler.AddEncyclopediaEntry(

                keyancient,
                "Tech/Equipment",
                "Скрытые данные",
                "test"
                );
            StoryGoalHandler.RegisterItemGoal(keyancient, Story.GoalType.Encyclopedia, Info.TechType);
        }
        
    }

    public class YeetKnife : HeatBlade
    {
        public float hitForce = 3000;
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