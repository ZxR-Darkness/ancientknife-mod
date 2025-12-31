using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace experimentalmod.Items.Minerals
{
    public static class UnknownMinerales
    {
        public static PrefabInfo Info { get; private set; }

        public static void Register()
        {

            Info = PrefabInfo.WithTechType("UnknownMineral", "Unknown Mineral", "Материал неизвестного происхождения.")
                //Assets/UnknownMineralIcon.png
                .WithIcon(ImageUtils.LoadSpriteFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets", "UnknownMineralIcon.png")));


            var customPrefab = new CustomPrefab(Info);


            var mineralClone = new CloneTemplate(Info, TechType.Nickel);


            mineralClone.ModifyPrefab += obj =>
            {
                string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string texturePath = Path.Combine(modPath, "Assets", "UnknownMineral_diffuse.png");

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
                new LootDistributionData.BiomeData { biome = BiomeType.GrassyPlateaus_Sand, count = 1, probability = 0.1f }
            );


            customPrefab.SetEquipment(EquipmentType.None);


            customPrefab.Register();
        }
    }
}