using ECCLibrary;
using ECCLibrary.Data;
using Nautilus.Assets;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace experimentalmod.Items
{
    // Компонент для атаки: при касании игрока выводит сообщение
    internal class ShadowMeleeAttack : MeleeAttack
    {
        public override void OnTouch(Collider collider)
        {
            base.OnTouch(collider);
            ErrorMessage.AddMessage("Shadow Leviathan");
        }
    }

    public class ShadowLeviathan : CreatureAsset
    {
        public ShadowLeviathan(PrefabInfo info) : base(info) { }

        protected override CreatureTemplate CreateTemplate()
        {

            GameObject model = StaticStructures.Bundle.LoadAsset<GameObject>("Assets/Worm_Root.prefab");

            // 2. Создаем шаблон (модель, тип поведения, тип цели, радиус патрулирования)
            var template = new CreatureTemplate(model, BehaviourType.Leviathan, EcoTargetType.Leviathan, 300f)
            {
                CellLevel = LargeWorldEntity.CellLevel.Far,
                Mass = 5000,
                EyeFOV = -0.75f,
                AcidImmune = true,

                // Настройка физики движения
                LocomotionData = new LocomotionData(5f, 0.2f),

                // Синхронизация скорости с аниматором
                AnimateByVelocityData = new AnimateByVelocityData(15f),

                // Настройки случайного плавания
                SwimRandomData = new SwimRandomData(0.2f, 10f, new Vector3(50, 20, 50), 4f, 1f, true),

                // Настройки ИИ атаки
                AttackLastTargetData = new AttackLastTargetData(0.8f, 15f, 0.6f, 10f)
            };

            // Добавляем агрессию при обнаружении цели (игрок/рыбы)
            template.AddAggressiveWhenSeeTargetData(new AggressiveWhenSeeTargetData(EcoTargetType.Shark, 2, 100, 3));

            return template;
        }

        protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
        {
            // 1. Поиск рта для атаки
            var mouth = prefab.transform.SearchChild("Mouth");

            if (mouth != null)
            {
                // Добавляем компонент атаки
                CreaturePrefabUtils.AddMeleeAttack<ShadowMeleeAttack>(prefab, components, mouth.gameObject, true, 50f);
            }

            yield break;
        }
    }
}