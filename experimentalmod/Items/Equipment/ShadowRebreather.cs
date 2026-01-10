using experimentalmod.Items.Minerals;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace experimentalmod.Items.Equipment
{
    public static class ShadowRebreather
    {
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("ShadowRebreather", "Ребризер 'Тень' WIP", "Экспериментальный прототип. Стабилизирует кислород за счет психики субъекта. WIP")
            .WithIcon(SpriteManager.Get(TechType.Rebreather));

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);
            var clone = new CloneTemplate(Info, TechType.Rebreather);

            customPrefab.SetGameObject(clone);
            var recipe = new RecipeData(
                new Ingredient(TechType.Rebreather, 1),
                new Ingredient(UnknownMinerales.Info.TechType, 2),
                new Ingredient(TechType.Uranium, 2)
            );
            customPrefab.SetRecipe(recipe).WithFabricatorType(CraftTree.Type.Fabricator);
            customPrefab.SetEquipment(EquipmentType.Head);
            customPrefab.SetUnlock(TechType.Rebreather);

            RegisterEncyclopedia();

            customPrefab.Register();
            var controller = new GameObject("ShadowRebreatherController");
            controller.AddComponent<ShadowRebreatherLogic>();
            Object.DontDestroyOnLoad(controller);
        }

        private static void RegisterEncyclopedia()
        {
            string keyrebreather = "ShadowRebreatherEncy";
            LanguageHandler.SetLanguageLine("EncyPath_Tech/Shadow Protocol", "PROJECT SHADOW");

            string entryContent =
                "<b>Экспериментальный образец #09-S. Протокол 'ОБРАТНОЕ ДЫХАНИЕ'.</b>\n\n" +
                "Устройство создает резонансное поле, которое напрямую воздействует на кору головного мозга владельца.\n\n" +
                "<b>Технические данные:</b>\n" +
                "• Глубина > 100м: Активация синтеза О2.\n" +
                "• Пси-связь: Прямая интеграция с когнитивными функциями.\n\n" +
                "<color=#ff0000ff>ВНИМАНИЕ:</color> 'Альтерра' не несет ответственности за шепот в голове.\n\n" +
                "<i>«Он дышит твоими мыслями, когда воздух заканчивается».</i>";

            PDAHandler.AddEncyclopediaEntry(keyrebreather, "Tech/Shadow Protocol", "Теневой Ребризер: Протокол 09", entryContent);
            StoryGoalHandler.RegisterItemGoal(keyrebreather, Story.GoalType.Encyclopedia, Info.TechType);
        }
    }

    public class ShadowRebreatherLogic : MonoBehaviour
    {
        private float nextEffectTime;

        void Update()
        {
            if (Player.main == null || Inventory.main == null || !Player.main.IsAlive()) return;

            // Проверяем надет ли предмет
            bool isEquipped = Inventory.main.equipment.GetCount(ShadowRebreather.Info.TechType) > 0;
            if (!isEquipped) return;

            float depth = Player.main.GetDepth();

            // --- ЛОГИКА КИСЛОРОДА ---
            // Если глубина больше 100м восстанавливаем кислород
            if (Player.main.IsUnderwater() && depth > 100f)
            {

                Player.main.oxygenMgr.AddOxygen(Time.deltaTime * 1.0f); 
            }
            if (Time.time > nextEffectTime)
            {
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
                    MainCameraControl.main.ShakeCamera(2.0f, 1.0f);
            }
            else if (depth > 400f)
            {
                ErrorMessage.AddMessage("СИСТЕМА: Обнаружены фантомные сигналы биомассы.");
            }
        }
    }
}