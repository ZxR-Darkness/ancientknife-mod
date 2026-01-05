using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using UnityEngine;
using Nautilus.Utility;

namespace experimentalmod.Items.Equipment
{
    public static class ShadowRebreather
    {
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("ShadowRebreather", "Ребризер 'Тень'", "Экспериментальный прототип. Стабилизирует кислород за счет психики.")
            .WithIcon(SpriteManager.Get(TechType.Rebreather));

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);
            var clone = new CloneTemplate(Info, TechType.Rebreather);

            customPrefab.SetGameObject(clone);

            var recipe = new RecipeData(
                new Ingredient(TechType.Rebreather, 1),
                new Ingredient(TechType.Magnetite, 2),
                new Ingredient(TechType.Uranium, 1)
            );

            customPrefab.SetRecipe(recipe).WithFabricatorType(CraftTree.Type.Fabricator);
            customPrefab.SetEquipment(EquipmentType.Head);
            customPrefab.SetUnlock(TechType.Rebreather);
            // Энциклопедия
            string keyrebreather = "ShadowRebreatherEncy";
            LanguageHandler.SetLanguageLine("EncyPath_Tech/Secrets", "Тайные разработки");
            PDAHandler.AddEncyclopediaEntry(
                keyrebreather, 
                "Tech/Secrets", 
                "Теневой Ребризер",
                "Устройство компенсирует давление на глубине...\n\n'Оно дышит за тебя...'");
            StoryGoalHandler.RegisterItemGoal(keyrebreather, Story.GoalType.Encyclopedia, Info.TechType);

            customPrefab.Register();

            var controller = new GameObject("ShadowRebreatherController");
            controller.AddComponent<ShadowRebreatherLogic>();
            Object.DontDestroyOnLoad(controller);
        }
    }

    public class ShadowRebreatherLogic : MonoBehaviour
    {
        private float nextEffectTime;

        void Update()
        {
            if (Player.main == null || Inventory.main == null) return;

            bool isEquipped = Inventory.main.equipment.GetCount(ShadowRebreather.Info.TechType) > 0;
            if (!isEquipped) return;


            if (Player.main.IsUnderwater() && Player.main.GetDepth() > 100f)
            {
                Player.main.oxygenMgr.AddOxygen(Time.deltaTime * 0.5f);
            }

            if (Time.time > nextEffectTime)
            {
                float depth = Player.main.GetDepth();
                float depthFactor = Mathf.Clamp01(depth / 600f);
                float delay = Mathf.Lerp(90f, 20f, depthFactor);

                TriggerEffect(depth);
                nextEffectTime = Time.time + delay;
            }
        }

        private void TriggerEffect(float depth)
        {
            int rnd = Random.Range(0, 100);

            if (rnd < 40)
            {
                ErrorMessage.AddMessage("Шепот: 'Ты зашел слишком далеко...'");
            }
            else if (rnd < 70)
            {
                if (MainCameraControl.main != null)
                    MainCameraControl.main.ShakeCamera(1.5f, 0.8f);
            }
            else if (depth > 400f)
            {
                ErrorMessage.AddMessage("СИСТЕМА: Обнаружены фантомные сигналы биомассы.");
            }
        }
    }
}