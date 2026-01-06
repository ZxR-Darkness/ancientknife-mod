using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Handlers;
using Nautilus.Utility;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace experimentalmod.Items.Minerals
{
    public static class UnknownMinerales
    {
        public static PrefabInfo Info { get; private set; }
        public static string ModPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void Register()
        {

            string iconPath = Path.Combine(ModPath, "Assets", "UnknownMineralIcon.png");

            Info = PrefabInfo.WithTechType("UnknownMineral", "Unknown Mineral", "Материал неизвестного происхождения. нигде не используется")
                .WithIcon(ImageUtils.LoadSpriteFromFile(iconPath));

            var customPrefab = new CustomPrefab(Info);
            var mineralClone = new CloneTemplate(Info, TechType.Nickel);


            mineralClone.ModifyPrefab += obj =>
            {
                string texturePath = Path.Combine(ModPath, "Assets", "UnknownMineral_diffuse.png");

                if (File.Exists(texturePath))
                {
                    var renderer = obj.GetComponentInChildren<Renderer>();
                    if (renderer != null)
                    {
                        Texture2D texture = ImageUtils.LoadTextureFromFile(texturePath);
                        renderer.material.mainTexture = texture;
                    }
                }
            };

            customPrefab.SetGameObject(mineralClone);
            customPrefab.SetSpawns(
                new LootDistributionData.BiomeData { biome = BiomeType.SafeShallows_Grass, count = 1, probability = 0.5f },
                new LootDistributionData.BiomeData { biome = BiomeType.GrassyPlateaus_CaveFloor, count = 1, probability = 0.5f }
            );
            customPrefab.SetEquipment(EquipmentType.None);
            customPrefab.Register();
        }
    }
}