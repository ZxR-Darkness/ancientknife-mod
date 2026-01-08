using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Assets;
using Nautilus.Handlers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace experimentalmod.Items
{
    internal class ShadowMeleeAttack : MeleeAttack
    {
        public override void OnTouch(Collider collider)
        {
            base.OnTouch(collider);
        }
    }

    public class DeepLeviathan : CreatureAsset
    {
        public static PrefabInfo Info { get; } = PrefabInfo
            .WithTechType("Deeplev", "Deep Leviathan", "Глубинный ужас.")
            .WithIcon(SpriteManager.Get(TechType.ReaperLeviathan));

        public DeepLeviathan(PrefabInfo info) : base(info) { }

        public static void RegisterEntity()
        {
            var deepLev = new DeepLeviathan(Info);
            deepLev.Register();

            SetupEncyclopedia(Info.TechType);

            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(Info.TechType, new Vector3(1800f, -100f, 0f)));
            CoordinatedSpawnsHandler.RegisterCoordinatedSpawn(new SpawnInfo(Info.TechType, new Vector3(-2000f, -100f, 0f)));
        }

        protected override CreatureTemplate CreateTemplate()
        {
            GameObject model = Plugin.Bundle.LoadAsset<GameObject>("Assets/ShadowFish.prefab");

            if (model == null)
            {
                Debug.LogError("O.S. TEAM: ShadowFish.prefab не найден!");
                return null;
            }

            var template = new CreatureTemplate(model, BehaviourType.Leviathan, EcoTargetType.Shark, 5000f)
            {
                CellLevel = LargeWorldEntity.CellLevel.Far,
                Mass = 5000f,
                EyeFOV = -1.0f,
                AcidImmune = true,
                LocomotionData = new LocomotionData(3f, 0.5f, 0.5f, 0.1f),
                AnimateByVelocityData = new AnimateByVelocityData(4f),
                SwimRandomData = new SwimRandomData(0.2f, 10f, new Vector3(30f, 10f, 30f)),
            };

            template.AttackLastTargetData = new AttackLastTargetData(1f, 15f, 0.5f, 5f);
            template.AddAggressiveWhenSeeTargetData(new AggressiveWhenSeeTargetData(EcoTargetType.Tech, 1f, 50f, 2));
            template.AddAggressiveWhenSeeTargetData(new AggressiveWhenSeeTargetData(EcoTargetType.Leviathan, 0.5f, 30f, 1));
            template.AddAggressiveWhenSeeTargetData(new AggressiveWhenSeeTargetData(EcoTargetType.Shark, 1f, 80f, 3));

            return template;
        }

        protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
        {
            Rigidbody rb = prefab.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.mass = 5000f;
            }

            var mouth = prefab.transform.SearchChild("Mouth");
            if (mouth != null)
            {
                var attack = CreaturePrefabUtils.AddMeleeAttack<ShadowMeleeAttack>(prefab, components, mouth.gameObject, true, 50f);
                attack.biteInterval = 2f;
                attack.biteDamage = 100f;
            }

            Animator anim = prefab.GetComponentInChildren<Animator>();
            if (anim != null)
            {
                anim.updateMode = AnimatorUpdateMode.Normal;
            }

            yield break;
        }

        public static void SetupEncyclopedia(TechType techType)
        {
            LanguageHandler.SetLanguageLine("EncyPath_Lifeforms/Fauna/Shadow-Protocol", "PROJECT SHADOW");

            string keyLev = "ShadowLev_Info_Page";

            string levDescription =
                "<color=#555555ff>Объект: Глубинный Левиафан (Deep Leviathan)</color>\n\n" +
                "Крупнейший представитель фауны, обнаруженный в рамках протокола <b>«Shadow»</b>. " +
                "Данный хищник эволюционировал в условиях полного отсутствия света, превратив свою чешую в <b>«абсолютную ловушку для фотонов»</b>.\n\n" +
                "<b>Тактический анализ:</b>\n" +
                "• <b>Сферический обзор:</b> Органы чувств объекта настроены на 360°, что исключает возможность незаметного сближения.\n" +
                "• <b>Агрессия:</b> Проявляет аномальную враждебность к любым источникам энергии Альтерры.\n" +
                "• <b>Светопоглощение:</b> Стандартные осветительные приборы теряют 90% эффективности при приближении к объекту.\n" +
                "• <b>Биологическое доминирование:</b> Исследование поведения показало, что Призрачные левиафаны избегают контакта с объектом на инстинктивном уровне. Глубинный левиафан излучает низкочастотный инфразвук, который воспринимается другими сверххищниками как сигнал <b>биологического коллапса</b>. Для них он не конкурент, а предвестник гибели экосистемы.\n\n" +
                "<color=#ff0000ff>ВНИМАНИЕ:</color>\n" +
                "В случае физического контакта рекомендуется использование <b>Теневого Ножа</b> для дестабилизации атомных связей существа. " +
                "Обычное оружие не способно пробить резонансную защиту чешуи.\n\n" +
                "<i>«Оно видит тебя, даже когда ты закрываешь глаза».</i>";

            PDAHandler.AddEncyclopediaEntry(
                keyLev,
                "Lifeforms/Fauna/Shadow-Protocol",
                "Глубинный Левиафан",
                levDescription
            );
            PDAHandler.AddCustomScannerEntry(techType, 5f, false, keyLev);
            StoryGoalHandler.RegisterItemGoal(keyLev, Story.GoalType.Encyclopedia, techType);
        }
    }
}