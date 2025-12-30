using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using UnityEngine;

namespace experimentalmod.Items.Equipment
{
    public static class ShadowRebreather
    {
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("ShadowRebreather", "Ребризер 'Тень'", "Экспериментальный прототип. Убирает штраф на кислород на глубине, но дестабилизирует восприятие.")
            .WithIcon(SpriteManager.Get(TechType.Rebreather));

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);

            var clone = new CloneTemplate(Info, TechType.Rebreather);
            clone.ModifyPrefab += obj =>
            {
                // Добавляем логику работы предмета
                obj.AddComponent<ShadowRebreatherLogic>();
            };

            customPrefab.SetGameObject(clone);

            // Рецепт (дорогой)
            var recipe = new RecipeData(new Ingredient(TechType.Rebreather, 1), new Ingredient(TechType.Diamond, 2), new Ingredient(TechType.Uranium, 1));
            customPrefab.SetRecipe(recipe)
                .WithFabricatorType(CraftTree.Type.Fabricator);

            customPrefab.SetEquipment(EquipmentType.Head);
            string keyrebreather = "RebreatherShad";
            LanguageHandler.SetLanguageLine("EncyPath_Tech/Secrets", "Ancients");
            PDAHandler.AddEncyclopediaEntry(
                keyrebreather,
                "Tech/Secrets",
                "Теневой Ребризер",
                "Этот ребризер не просто поддерживает дыхание — он переписывает способ, которым вы ощущаете глубину.\n\n" +
                "На экстремальных уровнях давления устройство стабилизирует подачу кислорода, но взамен нарушает работу восприятия.\n\n" +
                "Испытуемые сообщали о голосах, предупреждениях, которых нет в системе, и ощущении, что глубина наблюдает за ними.\n\n" +
                "'Alterra' официально отрицает существование данного прототипа."

            );
            StoryGoalHandler.RegisterItemGoal(keyrebreather, Story.GoalType.Encyclopedia, Info.TechType);

            customPrefab.SetUnlock(TechType.Rebreather);
            customPrefab.Register();
        }
    }

    // ЛОГИКА РЕБРИЗЕРА
    public class ShadowRebreatherLogic : MonoBehaviour
    {
        private float nextHallucinationTime;
        private bool isEquipped;

        void Update()
        {
            // Проверяем, надет ли этот предмет на голову игрока
            isEquipped = Inventory.main.equipment.GetCount(ShadowRebreather.Info.TechType) > 0;

            if (!isEquipped) return;

            float depth = Player.main.GetDepth();

            // 1. ЭФФЕКТ: Бесконечный воздух на глубине > 500м
            if (depth > 500f)
            {
                // Каждую секунду восстанавливаем чуть-чуть кислорода, чтобы он не тратился
                Player.main.oxygenMgr.AddOxygen(Time.deltaTime * 1.5f);
            }

            // 2. ЭФФЕКТ: Галлюцинации (раз в 2 минуты)
            if (Time.time > nextHallucinationTime)
            {
                TriggerHallucination();
                nextHallucinationTime = Time.time + 120f; // 120 секунд
            }
        }

        private void TriggerHallucination()
        {
            int effect = Random.Range(0, 3);
            switch (effect)
            {
                case 0:
                    ErrorMessage.AddMessage("Вы слышите шепот: 'Они не должны были найти это...'");
                    break;

                case 1:
                    // Тряска камеры (рабочая)
                    MainCameraControl.main.ShakeCamera(2f, 1f);
                    break;

                case 2:
                    ErrorMessage.AddMessage("Alterra: ВНИМАНИЕ. Обнаружена нейронная нестабильность.");
                    break;
            }
        }

    }
}