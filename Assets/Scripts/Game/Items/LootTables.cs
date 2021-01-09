// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:
//
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.Entity;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallConnect;

namespace DaggerfallWorkshop.Game.Items
{
    /// <summary>
    /// Built-in loot tables.
    /// Currently just for testing during early implementation.
    /// These approximate the loot tables on page 156 of Daggerfall Chronicles but are
    /// different in several ways (e.g. Chronicles uses "WP" for both "Warm Plant" and "Weapon").
    /// May diverge substantially over time during testing and future implementation.
    /// </summary>
    public static class LootTables
    {
        /// <summary>
        /// Default loot table chance matrices.
        /// Note: Temporary implementation. Will eventually be moved to an external file and loaded as keyed dict.
        /// Note: Many loot tables are defined with a lower chance for magic items in FALL.EXE's tables than is
        /// shown in Daggerfall Chronicles.
        /// These are shown below in the order "Key", "Chronicles chance", "FALL.EXE chance".
        /// E, 5, 3
        /// F, 2, 1
        /// G, 3, 1
        /// H, 2, 1
        /// I, 10, 2
        /// J, 20, 3
        /// K, 5, 3
        /// L, 2, 1
        /// N, 2, 1
        /// O, 3, 2
        /// P, 3, 2
        /// Q, 10, 3
        /// R, 5, 2
        /// S, 20, 3
        /// T, 3, 1
        /// U, 3, 2
        /// </summary>
        public static LootChanceMatrix[] DefaultLootTables = {
            new LootChanceMatrix() {key = "-", MinGold = 0, MaxGold = 0, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 0, MI = 0, CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "A", MinGold = 1, MaxGold = 10, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 5, WP = 5, MI = 2, CL = 4, BK = 0, M2 = 2, RL = 0 },
            // Chronicles says B has 10 for Warm Plant and Misc. Monster, but in FALL.EXE it is Temperate Plant and Warm Plant.
            new LootChanceMatrix() {key = "B", MinGold = 0, MaxGold = 0, P1 = 10, P2 = 10, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 0, MI = 0, CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "C", MinGold = 2, MaxGold = 20, P1 = 10, P2 = 10, C1 = 5, C2 = 5, C3 = 5, M1 = 5, AM = 5, WP = 25, MI = 3, CL = 0, BK = 2, M2 = 2, RL = 2 },
            new LootChanceMatrix() {key = "D", MinGold = 1, MaxGold = 4, P1 = 6, P2 = 6, C1 = 6, C2 = 6, C3 = 6, M1 = 6, AM = 0, WP = 0, MI = 0, CL = 0, BK = 0, M2 = 0, RL = 4 },
            new LootChanceMatrix() {key = "E", MinGold = 20, MaxGold = 80, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 10, WP = 10, MI = 3, CL = 4, BK = 2, M2 = 1, RL = 15 },
            new LootChanceMatrix() {key = "F", MinGold = 4, MaxGold = 30, P1 = 2, P2 = 2, C1 = 5, C2 = 5, C3 = 5, M1 = 2, AM = 50, WP = 50, MI = 1, CL = 0, BK = 0, M2 = 3, RL = 0 },
            new LootChanceMatrix() {key = "G", MinGold = 3, MaxGold = 15, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 50, WP = 50, MI = 1, CL = 5, BK = 0, M2 = 3, RL = 0 },
            new LootChanceMatrix() {key = "H", MinGold = 2, MaxGold = 10, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 100, MI = 1, CL = 2, BK = 0, M2 = 0, RL = 0 },
            // Chronicles is missing "I" but lists its data in table "J." All the tables from here are off by one compared to Chronicles.
            new LootChanceMatrix() {key = "I", MinGold = 0, MaxGold = 0, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 0, WP = 0, MI = 2, CL = 0, BK = 0, M2 = 0, RL = 5 },
            new LootChanceMatrix() {key = "J", MinGold = 50, MaxGold = 150, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 5, WP = 5, MI = 3, CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "K", MinGold = 1, MaxGold = 10, P1 = 3, P2 = 3, C1 = 3, C2 = 3, C3 = 3, M1 = 3, AM = 5, WP = 5, MI = 3, CL = 0, BK = 5, M2 = 2, RL = 100 },
            new LootChanceMatrix() {key = "L", MinGold = 1, MaxGold = 20, P1 = 0, P2 = 0, C1 = 3, C2 = 3, C3 = 3, M1 = 3, AM = 50, WP = 50, MI = 1, CL = 75, BK = 0, M2 = 5, RL = 3 },
            new LootChanceMatrix() {key = "M", MinGold = 1, MaxGold = 15, P1 = 1, P2 = 1, C1 = 1, C2 = 1, C3 = 1, M1 = 2, AM = 10, WP = 10, MI = 1, CL = 15, BK = 2, M2 = 3, RL = 1 },
            new LootChanceMatrix() {key = "N", MinGold = 1, MaxGold = 80, P1 = 5, P2 = 5, C1 = 5, C2 = 5, C3 = 5, M1 = 5, AM = 5, WP = 5, MI = 1, CL = 20, BK = 5, M2 = 2, RL = 5 },
            new LootChanceMatrix() {key = "O", MinGold = 5, MaxGold = 20, P1 = 1, P2 = 1, C1 = 1, C2 = 1, C3 = 1, M1 = 1, AM = 10, WP = 15, MI = 2, CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "P", MinGold = 5, MaxGold = 20, P1 = 5, P2 = 5, C1 = 5, C2 = 5, C3 = 5, M1 = 5, AM = 5, WP = 10, MI = 2, CL = 0, BK = 10, M2 = 5, RL = 0 },
            new LootChanceMatrix() {key = "Q", MinGold = 20, MaxGold = 80, P1 = 2, P2 = 2, C1 = 8, C2 = 8, C3 = 8, M1 = 2, AM = 10, WP = 25, MI = 3, CL = 35, BK = 5, M2 = 3, RL = 0 },
            new LootChanceMatrix() {key = "R", MinGold = 5, MaxGold = 20, P1 = 0, P2 = 0, C1 = 3, C2 = 3, C3 = 3, M1 = 5, AM = 5, WP = 15, MI = 2, CL = 0, BK = 0, M2 = 0, RL = 0 },
            new LootChanceMatrix() {key = "S", MinGold = 50, MaxGold = 125, P1 = 5, P2 = 5, C1 = 5, C2 = 5, C3 = 5, M1 = 15, AM = 10, WP = 10, MI = 3, CL = 0, BK = 5, M2 = 5, RL = 0 },
            new LootChanceMatrix() {key = "T", MinGold = 20, MaxGold = 80, P1 = 0, P2 = 0, C1 = 0, C2 = 0, C3 = 0, M1 = 0, AM = 100, WP = 100, MI = 1, CL = 0, BK = 0, M2 = 0, RL = 0},
            new LootChanceMatrix() {key = "U", MinGold = 7, MaxGold = 30, P1 = 5, P2 = 5, C1 = 5, C2 = 5, C3 = 5, M1 = 10, AM = 10, WP = 10, MI = 2, CL = 0, BK = 2, M2 = 2, RL = 10 },
        };

        /// <summary>
        /// Gets loot matrix by key.
        /// Note: Temporary implementation. Will eventually be moved to an external file and loaded as keyed dict.
        /// </summary>
        /// <param name="key">Key of matrix to get.</param>
        /// <returns>LootChanceMatrix.</returns>
        public static LootChanceMatrix GetMatrix(string key)
        {
            for (int i = 0; i < DefaultLootTables.Length; i++)
            {
                if (DefaultLootTables[i].key == key)
                    return DefaultLootTables[i];
            }

            return DefaultLootTables[0];
        }

        public static bool GenerateBuildingLoot(DaggerfallLoot loot, int locationIndex)
        {
            int[] traits = { -1, -1, -1 };

            DaggerfallLoot.GenerateBuildingItems(loot.Items, traits);

            return true;
        }

        public static bool GenerateDungeonLoot(DaggerfallLoot loot, int locationIndex)
        {
            DaggerfallLoot.GenerateDungeonItems(loot.Items, locationIndex);

            return true;
        }

        public static DaggerfallUnityItem[] GenerateDungeonLootItems(int dungeonIndex)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            int playerLuck = player.Stats.LiveLuck - 50;
            float basicLuckMod = (playerLuck * 0.02f) + 1f;
            float chance = 1f;
            int condModMin = 100;
            int condModMax = 100;
            List<DaggerfallUnityItem> items = new List<DaggerfallUnityItem>();

            // Reseed random
            Random.InitState(items.GetHashCode());

            switch (dungeonIndex)
            {
                case (int)DFRegion.DungeonTypes.Crypt:
                    items.Add(ItemBuilder.CreateGoldPieces((int)Mathf.Floor(Random.Range(15, 30 + 1) * basicLuckMod)));
                    condModMin = Mathf.Clamp((int)Mathf.Floor(10 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(35 * basicLuckMod), 1, 100);
                    AddGems(35, 0.6f, 30, basicLuckMod, items);
                    AddMagicItems(5, 0.5f, condModMin, condModMax, items);
                    AddMaps(5, 0.4f, items);
                    AddClothing(20, 0.6f, condModMin, condModMax, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.OrcStronghold:
                    items.Add(ItemBuilder.CreateGoldPieces((int)Mathf.Floor(Random.Range(15, 30 + 1) * basicLuckMod)));
                    condModMin = Mathf.Clamp((int)Mathf.Floor(35 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(75 * basicLuckMod), 1, 100);
                    AddArrows(1, 15, basicLuckMod, items);
                    AddWeapons(50, 0.7f, condModMin, condModMax, items);
                    AddArmors(30, 0.6f, condModMin, condModMax, 100, items);
                    AddIngots(60, 0.6f, items);
                    AddGems(5, 0.5f, 10, basicLuckMod, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.HumanStronghold:
                    items.Add(ItemBuilder.CreateGoldPieces((int)Mathf.Floor(Random.Range(30, 45 + 1) * basicLuckMod)));
                    condModMin = Mathf.Clamp((int)Mathf.Floor(40 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(80 * basicLuckMod), 1, 100);
                    AddArrows(1, 35, basicLuckMod, items);
                    AddWeapons(30, 0.6f, condModMin, condModMax, items);
                    AddArmors(55, 0.55f, condModMin, condModMax, 60, items);
                    AddIngots(35, 0.4f, items);
                    AddGems(10, 0.5f, 15, basicLuckMod, items);
                    AddMaps(2, 0.5f, items);
                    AddClothing(25, 0.4f, condModMin, condModMax, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.Prison:
                    items.Add(ItemBuilder.CreateGoldPieces((int)Mathf.Floor(Random.Range(0, 15 + 1) * basicLuckMod)));
                    condModMin = Mathf.Clamp((int)Mathf.Floor(15 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(55 * basicLuckMod), 1, 100);
                    chance = 40f;
                    while (Dice100.SuccessRoll((int)chance))
                    {
                        DaggerfallUnityItem Weapon = ItemBuilder.CreateWeapon((Weapons)PickOneOf((int)Weapons.Tanto, (int)Weapons.Tanto, (int)Weapons.Tanto, (int)Weapons.Tanto, (int)Weapons.Dagger, (int)Weapons.Dagger, (int)Weapons.Shortsword, (int)Weapons.Wakazashi), (WeaponMaterialTypes)PickOneOf((int)WeaponMaterialTypes.Iron, (int)WeaponMaterialTypes.Iron, (int)WeaponMaterialTypes.Iron, (int)WeaponMaterialTypes.Steel, 
                          (int)WeaponMaterialTypes.Steel, (int)WeaponMaterialTypes.Silver));
                        float condPercentMod = Random.Range(condModMin, condModMax + 1) / 100f;
                        Weapon.currentCondition = (int)Mathf.Ceil(Weapon.maxCondition * condPercentMod);
                        items.Add(Weapon);
                        chance *= 0.6f;
                    }
                    AddArmors(20, 0.6f, condModMin, condModMax, 0, items);
                    AddBooks(10, 0.4f, items, 37);
                    AddClothing(20, 0.4f, condModMin, condModMax, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.DesecratedTemple:
                    items.Add(ItemBuilder.CreateGoldPieces((int)Mathf.Floor(Random.Range(15, 30 + 1) * basicLuckMod)));
                    condModMin = Mathf.Clamp((int)Mathf.Floor(35 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(60 * basicLuckMod), 1, 100);
                    AddPotions(30, 0.5f, items);
                    AddBooks(50, 0.5f, items, 38);
                    AddGems(10, 0.5f, 20, basicLuckMod, items);
                    AddMagicItems(3, 0.3f, condModMin, condModMax, items);
                    AddPotionRecipes(5, 0.1f, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.Mine:
                    items.Add(ItemBuilder.CreateGoldPieces((int)Mathf.Floor(Random.Range(0, 15 + 1) * basicLuckMod)));
                    AddIngots(40, 0.5f, items);
                    AddGems(35, 0.6f, 15, basicLuckMod, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.NaturalCave:
                    condModMin = Mathf.Clamp((int)Mathf.Floor(5 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(30 * basicLuckMod), 1, 100);
                    AddClothing(8, 0.8f, condModMin, condModMax, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.Coven:
                    items.Add(ItemBuilder.CreateGoldPieces((int)Mathf.Floor(Random.Range(0, 15 + 1) * basicLuckMod)));
                    condModMin = Mathf.Clamp((int)Mathf.Floor(25 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(40 * basicLuckMod), 1, 100);
                    AddPotions(25, 0.6f, items);
                    AddBooks(20, 0.6f, items, 30);
                    AddMagicItems(6, 0.6f, condModMin, condModMax, items);
                    AddMaps(3, 0.1f, items);
                    AddPotionRecipes(8, 0.5f, items);
                    AddClothing(10, 0.5f, condModMin, condModMax, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.VampireHaunt:
                    items.Add(ItemBuilder.CreateGoldPieces((int)Mathf.Floor(Random.Range(0, 15 + 1) * basicLuckMod)));
                    condModMin = Mathf.Clamp((int)Mathf.Floor(15 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(40 * basicLuckMod), 1, 100);
                    AddMaps(6, 0.5f, items);
                    AddClothing(20, 0.7f, condModMin, condModMax, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.Laboratory:
                    items.Add(ItemBuilder.CreateGoldPieces((int)Mathf.Floor(Random.Range(15, 30 + 1) * basicLuckMod)));
                    condModMin = Mathf.Clamp((int)Mathf.Floor(30 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(55 * basicLuckMod), 1, 100);
                    AddPotions(35, 0.5f, items);
                    AddBooks(50, 0.4f, items, 33);
                    AddMagicItems(8, 0.2f, condModMin, condModMax, items);
                    AddPotionRecipes(10, 0.6f, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.HarpyNest:
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.RuinedCastle:
                    items.Add(ItemBuilder.CreateGoldPieces((int)Mathf.Floor(Random.Range(30, 45 + 1) * basicLuckMod)));
                    condModMin = Mathf.Clamp((int)Mathf.Floor(15 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(50 * basicLuckMod), 1, 100);
                    AddArrows(1, 11, basicLuckMod, items);
                    AddWeapons(25, 0.4f, condModMin, condModMax, items);
                    AddArmors(45, 0.5f, condModMin, condModMax, 50, items);
                    AddBooks(10, 0.5f, items, 28);
                    AddBooks(15, 0.4f, items, 31);
                    AddIngots(25, 0.4f, items);
                    AddGems(15, 0.5f, 20, basicLuckMod, items);
                    AddMaps(3, 0.2f, items);
                    AddClothing(10, 0.5f, condModMin, condModMax, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.SpiderNest:
                    condModMin = Mathf.Clamp((int)Mathf.Floor(5 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(30 * basicLuckMod), 1, 100);
                    AddClothing(15, 0.7f, condModMin, condModMax, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.GiantStronghold:
                    items.Add(ItemBuilder.CreateGoldPieces((int)Mathf.Floor(Random.Range(0, 15 + 1) * basicLuckMod)));
                    condModMin = Mathf.Clamp((int)Mathf.Floor(15 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(55 * basicLuckMod), 1, 100);
                    AddWeapons(20, 0.5f, condModMin, condModMax, items);
                    AddArmors(20, 0.35f, condModMin, condModMax, 40, items);
                    AddGems(10, 0.5f, 5, basicLuckMod, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.DragonsDen:
                    items.Add(ItemBuilder.CreateGoldPieces((int)Mathf.Floor(Random.Range(45, 60 + 1) * basicLuckMod)));
                    condModMin = Mathf.Clamp((int)Mathf.Floor(5 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(40 * basicLuckMod), 1, 100);
                    AddArrows(1, 6, basicLuckMod, items);
                    AddWeapons(15, 0.6f, condModMin, condModMax, items);
                    AddArmors(30, 0.5f, condModMin, condModMax, 60, items);
                    AddPotions(15, 0.3f, items);
                    AddIngots(15, 0.5f, items);
                    AddGems(15, 0.3f, 55, basicLuckMod, items);
                    AddMagicItems(2, 0.5f, condModMin, condModMax, items);
                    AddMaps(3, 0.3f, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.BarbarianStronghold:
                    items.Add(ItemBuilder.CreateGoldPieces((int)Mathf.Floor(Random.Range(15, 30 + 1) * basicLuckMod)));
                    condModMin = Mathf.Clamp((int)Mathf.Floor(30 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(60 * basicLuckMod), 1, 100);
                    AddArrows(1, 25, basicLuckMod, items);
                    AddWeapons(40, 0.6f, condModMin, condModMax, items);
                    AddArmors(30, 0.35f, condModMin, condModMax, 15, items);
                    AddIngots(25, 0.35f, items);
                    AddGems(10, 0.5f, 15, basicLuckMod, items);
                    AddMaps(3, 0.5f, items);
                    AddClothing(5, 0.6f, condModMin, condModMax, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.VolcanicCaves:
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.ScorpionNest:
                    condModMin = Mathf.Clamp((int)Mathf.Floor(5 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(30 * basicLuckMod), 1, 100);
                    AddClothing(15, 0.7f, condModMin, condModMax, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                case (int)DFRegion.DungeonTypes.Cemetery:
                    items.Add(ItemBuilder.CreateGoldPieces((int)Mathf.Floor(Random.Range(15, 30 + 1) * basicLuckMod)));
                    condModMin = Mathf.Clamp((int)Mathf.Floor(10 * basicLuckMod), 1, 100);
                    condModMax = Mathf.Clamp((int)Mathf.Floor(25 * basicLuckMod), 1, 100);
                    AddGems(25, 0.6f, 20, basicLuckMod, items);
                    AddMagicItems(2, 0.3f, condModMin, condModMax, items);
                    AddMaps(5, 0.5f, items);
                    AddClothing(25, 0.5f, condModMin, condModMax, items);
                    AddMiscDungeonSpecificItems(dungeonIndex, basicLuckMod, condModMin, condModMax, items);
                    break;
                default:
                    break;
            }

            return items.ToArray();
        }

        public static DaggerfallUnityItem[] GenerateEnemyLoot(DaggerfallEntity enemy, int[] traits, int[] predefLootProps, int[] extraLootProps)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            int level = enemy.Level;
            int[] enemyLootCondMods = EnemyLootConditionCalculator(enemy, traits);
            float condPercentMod = 1f;
            EnemyEntity AITarget = enemy as EnemyEntity;
            List<DaggerfallUnityItem> items = new List<DaggerfallUnityItem>();

            // Reseed random
            Random.InitState(items.GetHashCode());

            // Add gold
            if (predefLootProps[0] > 0)
            {
                items.Add(ItemBuilder.CreateGoldPieces(predefLootProps[0])); // Will have to work on this amount, it feels a bit off, too low. 
            }

            // Arrows
            if (extraLootProps[0] > 0)
            {
                DaggerfallUnityItem arrowPile = ItemBuilder.CreateWeapon(Weapons.Arrow, WeaponMaterialTypes.Iron);
                arrowPile.stackCount = extraLootProps[0];
                items.Add(arrowPile);
            }

            // Random Potions
            if (extraLootProps[1] > 0)
            {
                for (int i = 0; i < extraLootProps[1]; i++)
                {
                    items.Add(ItemBuilder.CreateRandomPotion()); // The whole Potion Recipe ID thing is a bit too confusing for me at this moment, so I can't specify what potions should be allowed, will work for now though. 
                }
            }

            // Random books
            if (predefLootProps[8] > 0)
            {
                AddBooksBasedOnSubject(AITarget, predefLootProps[8], items);
            }

            // Random Ingots
            float ingotChance = IngotDropChance(AITarget);
            while (Dice100.SuccessRoll((int)ingotChance))
            {
                items.Add(ItemBuilder.CreateRandomIngot(AITarget.Level));
                ingotChance *= 0.5f;
            }

            // Random Gems
            if (extraLootProps[2] > 0)
            {
                for (int i = 0; i < extraLootProps[2]; i++)
                {
                    items.Add(ItemBuilder.CreateRandomGem());
                }
            }

            // Random magic items
            if (extraLootProps[3] > 0)
            {
                for (int i = 0; i < extraLootProps[3]; i++)
                {
                    DaggerfallUnityItem magicItem = ItemBuilder.CreateRandomMagicItem(player.Gender, player.Race, level);

                    if (magicItem != null)
                    {
                        condPercentMod = Random.Range(enemyLootCondMods[0], enemyLootCondMods[1] + 1) / 100f;
                        magicItem.currentCondition = (int)Mathf.Ceil(magicItem.maxCondition * condPercentMod);
                    }

                    items.Add(magicItem);
                }
            }

            // Food/Ration Items
            if (extraLootProps[4] > 0)
            {
                // Items not yet implemented. 
            }

            // Light sources
            if (extraLootProps[5] > 0)
            {
                for (int i = 0; i < extraLootProps[5]; i++)
                {
                    if (i == 1 && Dice100.SuccessRoll(15))
                    {
                        items.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)UselessItems2.Lantern));
                        DaggerfallUnityItem lampOil = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)UselessItems2.Oil);
                        lampOil.stackCount = UnityEngine.Random.Range(0, 4 + 1);
                        items.Add(lampOil);
                        continue;
                    }
                    items.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf((int)UselessItems2.Candle, (int)UselessItems2.Torch)));
                }
            }

            // Random religious items
            if (extraLootProps[6] > 0)
            {
                for (int i = 0; i < extraLootProps[6]; i++)
                {
                    items.Add(ItemBuilder.CreateRandomReligiousItem());
                }
            }

            // Bandages
            if (extraLootProps[7] > 0)
            {
                DaggerfallUnityItem bandages = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)UselessItems2.Bandage);
                bandages.stackCount = extraLootProps[7];
                items.Add(bandages);
            }

            // Repair tools
            if (extraLootProps[8] > 0)
            {
                // Items not yet implemented.
            }

            // Drugs
            if (extraLootProps[9] > 0)
            {
                for (int i = 0; i < extraLootProps[9]; i++)
                {
                    items.Add(ItemBuilder.CreateRandomDrug());
                }
            }

            // Extra weapon
            if (extraLootProps[10] > 0)
            {
                for (int i = 0; i < extraLootProps[10]; i++)
                {
                    if (i == 1 && AITarget.EntityType == EntityTypes.EnemyClass && AITarget.CareerIndex == (int)ClassCareers.Nightblade)
                    {
                        DaggerfallUnityItem extraWep = ItemBuilder.CreateWeapon((Weapons)PickOneOf((int)Weapons.Dagger, (int)Weapons.Tanto, (int)Weapons.Shortsword, (int)Weapons.Longsword), WeaponMaterialTypes.Silver);
                        condPercentMod = Random.Range(enemyLootCondMods[0], enemyLootCondMods[1] + 1) / 100f;
                        extraWep.currentCondition = (int)Mathf.Ceil(extraWep.maxCondition * condPercentMod);
                        continue;
                    }

                    if (traits[2] == (int)MobilePersonalityInterests.Survivalist || traits[2] == (int)MobilePersonalityInterests.Hunter)
                    {
                        DaggerfallUnityItem extraWep = ItemBuilder.CreateWeapon((Weapons)PickOneOf((int)Weapons.Short_Bow, (int)Weapons.Long_Bow), FormulaHelper.RandomMaterial(level));
                        condPercentMod = Random.Range(enemyLootCondMods[0], enemyLootCondMods[1] + 1) / 100f;
                        extraWep.currentCondition = (int)Mathf.Ceil(extraWep.maxCondition * condPercentMod);
                        continue;
                    }
                    else
                    {
                        DaggerfallUnityItem extraWep = ItemBuilder.CreateRandomWeapon(level);
                        condPercentMod = Random.Range(enemyLootCondMods[0], enemyLootCondMods[1] + 1) / 100f;
                        extraWep.currentCondition = (int)Mathf.Ceil(extraWep.maxCondition * condPercentMod);
                    }
                }
            }

            // Maps
            if (extraLootProps[11] > 0)
            {
                for (int i = 0; i < extraLootProps[11]; i++)
                {
                    items.Add(new DaggerfallUnityItem(ItemGroups.MiscItems, 8));
                }
            }

            // Potion Recipes
            if (traits[2] == (int)MobilePersonalityInterests.Brewer)
            {
                DaggerfallLoot.RandomlyAddPotionRecipe(100, items); // I'll expand on this later on, probably add another parameter to extra loot method, but for now it's good enough. 
            }

            // Random clothes
            if (predefLootProps[9] > 0)
            {
                AddClothesBasedOnEnemy(player.Gender, player.Race, AITarget, enemyLootCondMods, items); // Will obviously have to change this later on when I add the location specific context variables of this loot system. 
            }

            // Ingredients
            bool customIngredCheck = TargetedIngredients(AITarget, predefLootProps, items);

            if (!customIngredCheck)
            {
                if (predefLootProps[1] > 0)
                {
                    for (int i = 0; i < predefLootProps[1]; i++)
                        items.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.MiscPlantIngredients));
                }

                if (predefLootProps[2] > 0)
                {
                    for (int i = 0; i < predefLootProps[2]; i++)
                        items.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.FlowerPlantIngredients));
                }

                if (predefLootProps[3] > 0)
                {
                    for (int i = 0; i < predefLootProps[3]; i++)
                        items.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.FruitPlantIngredients));
                }

                if (predefLootProps[4] > 0)
                {
                    for (int i = 0; i < predefLootProps[4]; i++)
                        items.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.AnimalPartIngredients));
                }

                if (predefLootProps[5] > 0)
                {
                    for (int i = 0; i < predefLootProps[5]; i++)
                        items.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.CreatureIngredients));
                }

                if (predefLootProps[6] > 0)
                {
                    for (int i = 0; i < predefLootProps[6]; i++)
                        items.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.SolventIngredients));
                }

                if (predefLootProps[7] > 0)
                {
                    for (int i = 0; i < predefLootProps[7]; i++)
                        items.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.MetalIngredients));
                }
            }

            // Extra flavor/junk items (mostly based on personality traits, if present)
            /*if (traits[0] > -1 || traits[1] > -1 || traits[2] > -1)
            {
                PersonalityTraitFlavorItemsGenerator(AITarget, traits, items); // Items not yet implemented.
            }*/

            return items.ToArray();
        }

        public static void AddArrows(int minArrows, int maxArrows, float luckMod, List<DaggerfallUnityItem> targetItems)
        {
            if (Dice100.SuccessRoll(50))
            {
                DaggerfallUnityItem arrowPile = ItemBuilder.CreateWeapon(Weapons.Arrow, WeaponMaterialTypes.Iron);
                arrowPile.stackCount = (int)Mathf.Floor(Random.Range(minArrows, maxArrows + 1) * luckMod);
                targetItems.Add(arrowPile);
            }
        }

        public static void AddWeapons(float chance, float chanceMod, int condModMin, int condModMax, List<DaggerfallUnityItem> targetItems)
        {
            int playerLuck = GameManager.Instance.PlayerEntity.Stats.LiveLuck;

            while (Dice100.SuccessRoll((int)chance))
            {
                DaggerfallUnityItem Weapon = (ItemBuilder.CreateRandomWeapon(-1, -1, playerLuck));
                float condPercentMod = Random.Range(condModMin, condModMax + 1) / 100f;
                Weapon.currentCondition = (int)Mathf.Ceil(Weapon.maxCondition * condPercentMod);
                targetItems.Add(Weapon);
                chance *= chanceMod;
            }
        }

        public static void AddArmors(float chance, float chanceMod, int condModMin, int condModMax, int metalChance, List<DaggerfallUnityItem> targetItems)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            int playerLuck = player.Stats.LiveLuck;

            while (Dice100.SuccessRoll((int)chance))
            {
                if (Dice100.SuccessRoll(metalChance))
                {
                    DaggerfallUnityItem Armor = ItemBuilder.CreateRandomArmor(player.Gender, player.Race, -1, -1, playerLuck, 2);
                    float condPercentMod = Random.Range(condModMin, condModMax + 1) / 100f;
                    Armor.currentCondition = (int)Mathf.Ceil(Armor.maxCondition * condPercentMod);
                    targetItems.Add(Armor);
                    chance *= chanceMod;
                }
                else
                {
                    DaggerfallUnityItem Armor = ItemBuilder.CreateRandomArmor(player.Gender, player.Race, -1, -1, playerLuck, PickOneOf(0, 1));
                    float condPercentMod = Random.Range(condModMin, condModMax + 1) / 100f;
                    Armor.currentCondition = (int)Mathf.Ceil(Armor.maxCondition * condPercentMod);
                    targetItems.Add(Armor);
                    chance *= chanceMod;
                }
            }
        }

        public static void AddPotions(float chance, float chanceMod, List<DaggerfallUnityItem> targetItems)
        {
            int playerLuck = GameManager.Instance.PlayerEntity.Stats.LiveLuck;

            while (Dice100.SuccessRoll((int)chance))
            {
                targetItems.Add(ItemBuilder.CreateRandomPotion()); // The whole Potion Recipe ID thing is a bit too confusing for me at this moment, so I can't specify what potions should be allowed, will work for now though.
                chance *= chanceMod;
            }
        }

        public static void AddBooks(float chance, float chanceMod, List<DaggerfallUnityItem> targetItems, int bookSubject = -1)
        {
            int playerLuck = GameManager.Instance.PlayerEntity.Stats.LiveLuck;

            while (Dice100.SuccessRoll((int)chance))
            {
                if (bookSubject == -1)
                {
                    targetItems.Add(ItemBuilder.CreateRandomBookOfRandomSubject());
                    chance *= chanceMod;
                }
                else
                {
                    targetItems.Add(ItemBuilder.CreateRandomBookOfSpecificSubject((ItemGroups)bookSubject));
                    chance *= chanceMod;
                }
            }
        }

        public static void AddIngots(float chance, float chanceMod, List<DaggerfallUnityItem> targetItems)
        {
            int playerLuck = GameManager.Instance.PlayerEntity.Stats.LiveLuck;

            while (Dice100.SuccessRoll((int)chance))
            {
                targetItems.Add(ItemBuilder.CreateRandomIngot(-1, -1, playerLuck));
                chance *= chanceMod;
            }
        }

        public static void AddGems(float chance, float chanceMod, int rareGemChance, float luckMod, List<DaggerfallUnityItem> targetItems)
        {
            while (Dice100.SuccessRoll((int)chance))
            {
                if (Dice100.SuccessRoll(rareGemChance))
                {
                    targetItems.Add(ItemBuilder.CreateItem(ItemGroups.Gems, PickOneOf((int)Gems.Ruby, (int)Gems.Sapphire, (int)Gems.Emerald, (int)Gems.Diamond)));
                    chance *= chanceMod;
                }
                else
                {
                    targetItems.Add(ItemBuilder.CreateItem(ItemGroups.Gems, PickOneOf((int)Gems.Ruby, (int)Gems.Sapphire, (int)Gems.Emerald, (int)Gems.Diamond)));
                    chance *= chanceMod;
                }
            }
        }

        public static void AddMagicItems(float chance, float chanceMod, int condModMin, int condModMax, List<DaggerfallUnityItem> targetItems)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            int playerLuck = player.Stats.LiveLuck;

            while (Dice100.SuccessRoll((int)chance))
            {
                DaggerfallUnityItem magicItem = ItemBuilder.CreateRandomMagicItem(player.Gender, player.Race, -1, -1, playerLuck);
                float condPercentMod = Random.Range(condModMin, condModMax + 1) / 100f;
                magicItem.currentCondition = (int)Mathf.Ceil(magicItem.maxCondition * condPercentMod);
                targetItems.Add(magicItem);
                chance *= chanceMod;
            }
        }

        public static void AddMaps(float chance, float chanceMod, List<DaggerfallUnityItem> targetItems)
        {
            while (Dice100.SuccessRoll((int)chance))
            {
                targetItems.Add(new DaggerfallUnityItem(ItemGroups.MiscItems, 8));
                chance *= chanceMod;
            }
        }

        public static void AddPotionRecipes(float chance, float chanceMod, List<DaggerfallUnityItem> targetItems)
        {
            while (Dice100.SuccessRoll((int)chance))
            {
                DaggerfallLoot.RandomlyAddPotionRecipe(100, targetItems);
                chance *= chanceMod;
            }
        }

        public static void AddClothing(float chance, float chanceMod, int condModMin, int condModMax, List<DaggerfallUnityItem> targetItems)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            while (Dice100.SuccessRoll((int)chance))
            {
                if (Dice100.SuccessRoll(50))
                {
                    DaggerfallUnityItem Cloths = ItemBuilder.CreateRandomClothing(Genders.Male, player.Race);
                    float condPercentMod = Random.Range(condModMin, condModMax + 1) / 100f;
                    Cloths.currentCondition = (int)Mathf.Ceil(Cloths.maxCondition * condPercentMod);
                    targetItems.Add(Cloths);
                    chance *= chanceMod;
                }
                else
                {
                    DaggerfallUnityItem Cloths = ItemBuilder.CreateRandomClothing(Genders.Female, player.Race);
                    float condPercentMod = Random.Range(condModMin, condModMax + 1) / 100f;
                    Cloths.currentCondition = (int)Mathf.Ceil(Cloths.maxCondition * condPercentMod);
                    targetItems.Add(Cloths);
                    chance *= chanceMod;
                }
            }
        }

        public static void AddIngredGroupRandomItems(ItemGroups itemGroup, float chance, float chanceMod, List<DaggerfallUnityItem> targetItems)
        {
            while (Dice100.SuccessRoll((int)chance))
            {
                targetItems.Add(ItemBuilder.CreateRandomIngredient(itemGroup));
                chance *= chanceMod;
            }
        }

        public static void AddMiscDungeonSpecificItems(int dungeonIndex, float luckMod, int condModMin, int condModMax, List<DaggerfallUnityItem> targetItems)
        {
            switch (dungeonIndex)
            {
                case (int)DFRegion.DungeonTypes.Crypt:
                    AddSpecificRandomItems(ItemGroups.Jewellery, 30, 0.6f, targetItems, (int)Jewellery.Amulet, (int)Jewellery.Bracelet, (int)Jewellery.Ring, (int)Jewellery.Torc);
                    AddIngredGroupRandomItems(ItemGroups.ReligiousItems, 20, 0.4f, targetItems);
                    break;
                case (int)DFRegion.DungeonTypes.OrcStronghold:
                    break;
                case (int)DFRegion.DungeonTypes.HumanStronghold:
                    AddIngredGroupRandomItems(ItemGroups.ReligiousItems, 10, 0.5f, targetItems);
                    break;
                case (int)DFRegion.DungeonTypes.Prison:
                    AddSpecificRandomItems(ItemGroups.ReligiousItems, 25, 0.5f, targetItems, (int)ReligiousItems.Common_symbol, (int)ReligiousItems.Prayer_beads, (int)ReligiousItems.Bell);
                    break;
                case (int)DFRegion.DungeonTypes.DesecratedTemple:
                    AddSpecificRandomItems(ItemGroups.Jewellery, 15, 0.5f, targetItems, (int)Jewellery.Amulet, (int)Jewellery.Bracelet, (int)Jewellery.Ring, (int)Jewellery.Torc);
                    AddIngredGroupRandomItems(ItemGroups.ReligiousItems, 45, 0.6f, targetItems);
                    break;
                case (int)DFRegion.DungeonTypes.Mine:
                    AddIngredGroupRandomItems(ItemGroups.MetalIngredients, 65, 0.4f, targetItems);
                    break;
                case (int)DFRegion.DungeonTypes.NaturalCave:
                    AddIngredGroupRandomItems(ItemGroups.MiscPlantIngredients, 45, 0.6f, targetItems);
                    AddIngredGroupRandomItems(ItemGroups.FruitPlantIngredients, 25, 0.6f, targetItems);
                    AddSpecificRandomItems(ItemGroups.SolventIngredients, 25, 0.5f, targetItems, (int)SolventIngredients.Rain_water);
                    AddSpecificRandomItems(ItemGroups.AnimalPartIngredients, 25, 0.5f, targetItems, (int)AnimalPartIngredients.Small_tooth, (int)AnimalPartIngredients.Big_tooth, (int)AnimalPartIngredients.Snake_venom, (int)AnimalPartIngredients.Small_scorpion_stinger);
                    break;
                case (int)DFRegion.DungeonTypes.Coven:
                    AddIngredGroupRandomItems(ItemGroups.CreatureIngredients, 20, 0.5f, targetItems);
                    AddIngredGroupRandomItems(ItemGroups.FruitPlantIngredients, 40, 0.4f, targetItems);
                    AddIngredGroupRandomItems(ItemGroups.MiscPlantIngredients, 50, 0.4f, targetItems);
                    AddIngredGroupRandomItems(ItemGroups.FlowerPlantIngredients, 30, 0.5f, targetItems);
                    AddSpecificRandomItems(ItemGroups.Jewellery, 20, 0.3f, targetItems, (int)Jewellery.Amulet, (int)Jewellery.Bracelet, (int)Jewellery.Ring, (int)Jewellery.Torc, (int)Jewellery.Mark, (int)Jewellery.Cloth_amulet);
                    break;
                case (int)DFRegion.DungeonTypes.VampireHaunt:
                    AddSpecificRandomItems(ItemGroups.Jewellery, 15, 0.5f, targetItems, (int)Jewellery.Amulet, (int)Jewellery.Bracelet, (int)Jewellery.Ring, (int)Jewellery.Torc);
                    break;
                case (int)DFRegion.DungeonTypes.Laboratory:
                    AddIngredGroupRandomItems(ItemGroups.MetalIngredients, 20, 0.5f, targetItems);
                    AddSpecificRandomItems(ItemGroups.CreatureIngredients, 25, 0.5f, targetItems, (int)CreatureIngredients.Basilisk_eye, (int)CreatureIngredients.Ectoplasm, (int)CreatureIngredients.Mummy_wrappings, (int)CreatureIngredients.Troll_blood, (int)CreatureIngredients.Wraith_essence, (int)CreatureIngredients.Orcs_blood, (int)CreatureIngredients.Nymph_hair, (int)CreatureIngredients.Gorgon_snake);
                    AddSpecificRandomItems(ItemGroups.SolventIngredients, 35, 0.6f, targetItems, (int)SolventIngredients.Rain_water, (int)SolventIngredients.Pure_water, (int)SolventIngredients.Nectar);
                    AddSpecificRandomItems(ItemGroups.AnimalPartIngredients, 20, 0.6f, targetItems, (int)AnimalPartIngredients.Snake_venom, (int)AnimalPartIngredients.Spider_venom);
                    break;
                case (int)DFRegion.DungeonTypes.HarpyNest:
                    AddSpecificRandomItems(ItemGroups.CreatureIngredients, 75, 0.6f, targetItems, (int)CreatureIngredients.Harpy_Feather);
                    break;
                case (int)DFRegion.DungeonTypes.RuinedCastle:
                    AddSpecificRandomItems(ItemGroups.Jewellery, 20, 0.7f, targetItems, (int)Jewellery.Amulet, (int)Jewellery.Bracelet, (int)Jewellery.Ring, (int)Jewellery.Torc);
                    AddIngredGroupRandomItems(ItemGroups.ReligiousItems, 15, 0.4f, targetItems);
                    break;
                case (int)DFRegion.DungeonTypes.SpiderNest:
                    AddSpecificRandomItems(ItemGroups.AnimalPartIngredients, 45, 0.5f, targetItems, (int)AnimalPartIngredients.Spider_venom);
                    break;
                case (int)DFRegion.DungeonTypes.GiantStronghold:
                    break;
                case (int)DFRegion.DungeonTypes.DragonsDen:
                    AddSpecificRandomItems(ItemGroups.CreatureIngredients, 25, 0.5f, targetItems, (int)CreatureIngredients.Dragons_scales, (int)CreatureIngredients.Fairy_dragon_scales);
                    AddSpecificRandomItems(ItemGroups.Jewellery, 15, 0.6f, targetItems, (int)Jewellery.Amulet, (int)Jewellery.Bracelet, (int)Jewellery.Ring, (int)Jewellery.Torc);
                    break;
                case (int)DFRegion.DungeonTypes.BarbarianStronghold:
                    AddSpecificRandomItems(ItemGroups.Jewellery, 25, 0.5f, targetItems, (int)Jewellery.Bracelet, (int)Jewellery.Ring, (int)Jewellery.Torc, (int)Jewellery.Bracer, (int)Jewellery.Cloth_amulet);
                    break;
                case (int)DFRegion.DungeonTypes.VolcanicCaves:
                    AddIngredGroupRandomItems(ItemGroups.MetalIngredients, 45, 0.6f, targetItems);
                    break;
                case (int)DFRegion.DungeonTypes.ScorpionNest:
                    AddSpecificRandomItems(ItemGroups.AnimalPartIngredients, 50, 0.5f, targetItems, (int)AnimalPartIngredients.Small_scorpion_stinger, (int)AnimalPartIngredients.Giant_scorpion_stinger);
                    break;
                case (int)DFRegion.DungeonTypes.Cemetery:
                    AddSpecificRandomItems(ItemGroups.Jewellery, 25, 0.6f, targetItems, (int)Jewellery.Bracer, (int)Jewellery.Cloth_amulet, (int)Jewellery.Mark);
                    AddIngredGroupRandomItems(ItemGroups.ReligiousItems, 10, 0.6f, targetItems);
                    break;
                default:
                    break;
            }
        }

        public static void AddSpecificRandomItems(ItemGroups itemGroup, float chance, float chanceMod, List<DaggerfallUnityItem> targetItems, params int[] itemIndices) // params is extremely handy for my purposes.
        {
            while (Dice100.SuccessRoll((int)chance))
            {
                int itemIndex = itemIndices[Random.Range(0, itemIndices.Length)];
                targetItems.Add(ItemBuilder.CreateItem(itemGroup, itemIndex));
                chance *= chanceMod;
            }
        }

        public static int PickOneOf(params int[] values) // Pango provided assistance in making this much cleaner way of doing the random value choice part, awesome.
        {
            return values[Random.Range(0, values.Length)];
        }

        #region Private Methods

        static void PersonalityTraitFlavorItemsGenerator(EnemyEntity AITarget, int[] traits, List<DaggerfallUnityItem> targetItems)
        {
            int level = AITarget.Level;

            if (traits[0] > -1 || traits[1] > -1)
            {
                if (traits[0] == (int)MobilePersonalityQuirks.Curious || traits[1] == (int)MobilePersonalityQuirks.Curious)
                {
                    int randRange = Random.Range(1, 3 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[0] == (int)MobilePersonalityQuirks.Addict || traits[1] == (int)MobilePersonalityQuirks.Addict)
                {
                    int randRange = Random.Range(2, 6 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[0] == (int)MobilePersonalityQuirks.Hoarder || traits[1] == (int)MobilePersonalityQuirks.Hoarder)
                {
                    int randRange = Random.Range(6, 18 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[0] == (int)MobilePersonalityQuirks.Vain || traits[1] == (int)MobilePersonalityQuirks.Vain)
                {
                    int randRange = Random.Range(2, 7 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[0] == (int)MobilePersonalityQuirks.Untrusting || traits[1] == (int)MobilePersonalityQuirks.Untrusting)
                {
                    int randRange = Random.Range(1, 3 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished.
                    // This won't work the same as the others, since it in theory will be placing existing items into a seperate lock-box inventory type of item, will need a lot of work on this one eventually.
                }

                if (traits[0] == (int)MobilePersonalityQuirks.Sadistic || traits[1] == (int)MobilePersonalityQuirks.Sadistic)
                {
                    int randRange = Random.Range(1, 4 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[0] == (int)MobilePersonalityQuirks.Romantic || traits[1] == (int)MobilePersonalityQuirks.Romantic)
                {
                    int randRange = Random.Range(2, 4 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[0] == (int)MobilePersonalityQuirks.Alcoholic || traits[1] == (int)MobilePersonalityQuirks.Alcoholic)
                {
                    int randRange = Random.Range(2, 6 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }
            }

            if (traits[2] > -1)
            {
                if (traits[2] == (int)MobilePersonalityInterests.God_Fearing)
                {
                    int randRange = Random.Range(1, 3 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[2] == (int)MobilePersonalityInterests.Occultist)
                {
                    int randRange = Random.Range(2, 4 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[2] == (int)MobilePersonalityInterests.Childish)
                {
                    int randRange = Random.Range(1, 3 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[2] == (int)MobilePersonalityInterests.Artistic)
                {
                    int randRange = Random.Range(1, 3 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[2] == (int)MobilePersonalityInterests.Collector)
                {
                    int randRange = Random.Range(4, 11 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[2] == (int)MobilePersonalityInterests.Survivalist)
                {
                    int randRange = Random.Range(1, 3 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[2] == (int)MobilePersonalityInterests.Hunter)
                {
                    int randRange = Random.Range(1, 3 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[2] == (int)MobilePersonalityInterests.Fetishist)
                {
                    int randRange = Random.Range(2, 4 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[2] == (int)MobilePersonalityInterests.Brewer)
                {
                    int randRange = Random.Range(1, 3 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[2] == (int)MobilePersonalityInterests.Cartographer)
                {
                    int randRange = Random.Range(1, 3 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[2] == (int)MobilePersonalityInterests.Fisher)
                {
                    int randRange = Random.Range(1, 3 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[2] == (int)MobilePersonalityInterests.Diver)
                {
                    int randRange = Random.Range(1, 3 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[2] == (int)MobilePersonalityInterests.Writer)
                {
                    int randRange = Random.Range(1, 3 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }

                if (traits[2] == (int)MobilePersonalityInterests.Handy)
                {
                    int randRange = Random.Range(1, 3 + 1);
                    for (int i = 0; i < randRange; i++)
                        targetItems.Add(ItemBuilder.CreateItem(ItemGroups.UselessItems2, PickOneOf(811, 812, 813, 814))); // Item ID will be whatever the respective item IDs are in their respective itemgroup Enum, when finished. 
                }
            }

            return;
        }

        static bool TargetedIngredients(EnemyEntity AITarget, int[] predefLootProps, List<DaggerfallUnityItem> targetItems)
        {
            DaggerfallUnityItem ingredients = null;

            if (AITarget.EntityType == EntityTypes.EnemyClass)
            {
                return false;
            }
            else
            {
                switch (AITarget.CareerIndex)
                {
                    case 0:
                    case 3:
                        if (predefLootProps[4] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.AnimalPartIngredients, (int)AnimalPartIngredients.Small_tooth);
                            ingredients.stackCount = predefLootProps[4];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 4:
                    case 5:
                        if (predefLootProps[4] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.AnimalPartIngredients, (int)AnimalPartIngredients.Big_tooth);
                            ingredients.stackCount = predefLootProps[4];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 6:
                        if (predefLootProps[4] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.AnimalPartIngredients, (int)AnimalPartIngredients.Spider_venom);
                            ingredients.stackCount = predefLootProps[4];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 11:
                        for (int i = 0; i < predefLootProps[4]; i++)
                        {
                            if (Dice100.SuccessRoll(95))
                                targetItems.Add(ItemBuilder.CreateItem(ItemGroups.AnimalPartIngredients, (int)AnimalPartIngredients.Small_tooth));
                            else
                                targetItems.Add(ItemBuilder.CreateItem(ItemGroups.AnimalPartIngredients, (int)AnimalPartIngredients.Pearl));
                        }
                        return true;
                    case 20:
                        if (predefLootProps[4] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.AnimalPartIngredients, (int)AnimalPartIngredients.Giant_scorpion_stinger);
                            ingredients.stackCount = predefLootProps[4];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 2:
                        for (int i = 0; i < predefLootProps[1]; i++)
                            targetItems.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.MiscPlantIngredients));
                        for (int i = 0; i < predefLootProps[2]; i++)
                            targetItems.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.FlowerPlantIngredients));
                        for (int i = 0; i < predefLootProps[3]; i++)
                        {
                            if (Dice100.SuccessRoll(90))
                                targetItems.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.FruitPlantIngredients));
                            else
                                targetItems.Add(ItemBuilder.CreateItem(ItemGroups.Gems, (int)Gems.Amber));
                        }
                        return true;
                    case 10:
                        for (int i = 0; i < predefLootProps[2]; i++)
                            targetItems.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.FlowerPlantIngredients));
                        if (predefLootProps[5] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Nymph_hair);
                            ingredients.stackCount = predefLootProps[5];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 13:
                        if (predefLootProps[5] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Harpy_Feather);
                            ingredients.stackCount = predefLootProps[5];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 16:
                        if (predefLootProps[5] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Giant_blood);
                            ingredients.stackCount = predefLootProps[5];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 22:
                        if (predefLootProps[1] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.MiscPlantIngredients, (int)MiscPlantIngredients.Root_tendrils);
                            ingredients.stackCount = predefLootProps[1];
                            targetItems.Add(ingredients);
                        }
                        if (predefLootProps[7] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.MetalIngredients, (int)MetalIngredients.Lodestone);
                            ingredients.stackCount = predefLootProps[7];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 34:
                        if (predefLootProps[4] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.AnimalPartIngredients, (int)AnimalPartIngredients.Small_tooth);
                            ingredients.stackCount = predefLootProps[4];
                            targetItems.Add(ingredients);
                        }
                        for (int i = 0; i < predefLootProps[5]; i++)
                        {
                            if (Dice100.SuccessRoll(40))
                                targetItems.Add(ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Dragons_scales));
                            else
                                targetItems.Add(ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Fairy_dragon_scales));
                        }
                        return true;
                    case 40:
                        if (predefLootProps[4] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.AnimalPartIngredients, (int)AnimalPartIngredients.Big_tooth);
                            ingredients.stackCount = predefLootProps[4];
                            targetItems.Add(ingredients);
                        }
                        for (int i = 0; i < predefLootProps[5]; i++)
                        {
                            if (Dice100.SuccessRoll(80))
                                targetItems.Add(ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Dragons_scales));
                            else
                                targetItems.Add(ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Fairy_dragon_scales));
                        }
                        return true;
                    case 41:
                        if (predefLootProps[4] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.AnimalPartIngredients, (int)AnimalPartIngredients.Pearl);
                            ingredients.stackCount = predefLootProps[4];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 42:
                        for (int i = 0; i < predefLootProps[5]; i++)
                        {
                            if (Dice100.SuccessRoll(35))
                                targetItems.Add(ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Nymph_hair));
                            else
                                targetItems.Add(ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Gorgon_snake));
                        }
                        return true;
                    case 7:
                    case 12:
                    case 24:
                        if (predefLootProps[5] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Orcs_blood);
                            ingredients.stackCount = predefLootProps[5];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 21:
                        for (int i = 0; i < predefLootProps[1]; i++)
                            targetItems.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.MiscPlantIngredients));
                        for (int i = 0; i < predefLootProps[2]; i++)
                            targetItems.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.FlowerPlantIngredients));
                        for (int i = 0; i < predefLootProps[3]; i++)
                            targetItems.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.FruitPlantIngredients));
                        for (int i = 0; i < predefLootProps[4]; i++)
                            targetItems.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.AnimalPartIngredients));
                        if (predefLootProps[5] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Orcs_blood);
                            ingredients.stackCount = predefLootProps[5];
                            targetItems.Add(ingredients);
                        }
                        for (int i = 0; i < predefLootProps[6]; i++)
                            targetItems.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.SolventIngredients));
                        for (int i = 0; i < predefLootProps[7]; i++)
                            targetItems.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.MetalIngredients));
                        return true;
                    case 9:
                        if (predefLootProps[5] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Werewolfs_blood);
                            ingredients.stackCount = predefLootProps[5];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 14:
                        if (predefLootProps[5] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Wereboar_tusk);
                            ingredients.stackCount = predefLootProps[5];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 35:
                        if (predefLootProps[7] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.MetalIngredients, (int)MetalIngredients.Sulphur);
                            ingredients.stackCount = predefLootProps[7];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 36:
                        if (predefLootProps[7] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.MetalIngredients, (int)MetalIngredients.Iron);
                            ingredients.stackCount = predefLootProps[7];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 37:
                        if (predefLootProps[6] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.SolventIngredients, (int)SolventIngredients.Ichor);
                            ingredients.stackCount = predefLootProps[6];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 38:
                        if (predefLootProps[6] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.SolventIngredients, (int)SolventIngredients.Pure_water);
                            ingredients.stackCount = predefLootProps[6];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 18:
                        if (predefLootProps[5] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Ectoplasm);
                            ingredients.stackCount = predefLootProps[5];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 19:
                        if (predefLootProps[5] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Mummy_wrappings);
                            ingredients.stackCount = predefLootProps[5];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 23:
                        for (int i = 0; i < predefLootProps[5]; i++)
                        {
                            if (Dice100.SuccessRoll(70))
                                targetItems.Add(ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Ectoplasm));
                            else
                                targetItems.Add(ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Wraith_essence));
                        }
                        return true;
                    case 28:
                    case 30:
                        if (predefLootProps[4] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.AnimalPartIngredients, (int)AnimalPartIngredients.Small_tooth);
                            ingredients.stackCount = predefLootProps[4];
                            targetItems.Add(ingredients);
                        }
                        for (int i = 0; i < predefLootProps[5]; i++)
                            targetItems.Add(ItemBuilder.CreateRandomIngredient(ItemGroups.CreatureIngredients));
                        return true;
                    case 32:
                    case 33:
                        if (predefLootProps[5] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Lich_dust);
                            ingredients.stackCount = predefLootProps[5];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 25:
                        if (predefLootProps[5] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Daedra_heart);
                            ingredients.stackCount = predefLootProps[5];
                            targetItems.Add(ingredients);
                        }
                        if (predefLootProps[6] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.SolventIngredients, (int)SolventIngredients.Pure_water);
                            ingredients.stackCount = predefLootProps[6];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 26:
                        if (predefLootProps[5] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Daedra_heart);
                            ingredients.stackCount = predefLootProps[5];
                            targetItems.Add(ingredients);
                        }
                        if (predefLootProps[7] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.MetalIngredients, (int)MetalIngredients.Sulphur);
                            ingredients.stackCount = predefLootProps[7];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 27:
                        if (predefLootProps[4] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.AnimalPartIngredients, (int)AnimalPartIngredients.Big_tooth);
                            ingredients.stackCount = predefLootProps[4];
                            targetItems.Add(ingredients);
                        }
                        if (predefLootProps[5] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Daedra_heart);
                            ingredients.stackCount = predefLootProps[5];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 29:
                        if (predefLootProps[5] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Daedra_heart);
                            ingredients.stackCount = predefLootProps[5];
                            targetItems.Add(ingredients);
                        }
                        if (predefLootProps[6] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.SolventIngredients, (int)SolventIngredients.Elixir_vitae);
                            ingredients.stackCount = predefLootProps[6];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    case 31:
                        if (predefLootProps[5] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.CreatureIngredients, (int)CreatureIngredients.Daedra_heart);
                            ingredients.stackCount = predefLootProps[5];
                            targetItems.Add(ingredients);
                        }
                        if (predefLootProps[6] > 0)
                        {
                            ingredients = ItemBuilder.CreateItem(ItemGroups.SolventIngredients, (int)SolventIngredients.Ichor);
                            ingredients.stackCount = predefLootProps[6];
                            targetItems.Add(ingredients);
                        }
                        return true;
                    default:
                        return false;
                }
            }
        }

        static void AddBooksBasedOnSubject(EnemyEntity AITarget, int bookAmount, List<DaggerfallUnityItem> targetItems)
        {
            if (AITarget.EntityType == EntityTypes.EnemyClass)
            {
                switch (AITarget.CareerIndex)
                {
                    case (int)ClassCareers.Mage:
                        for (int i = 0; i < bookAmount; i++)
                            targetItems.Add(ItemBuilder.CreateRandomBookOfSpecificSubject((ItemGroups)PickOneOf(33, 33, 33, 33, 33, 33, 33, 33, 33, 28, 30, 30, 30, 31, 31, 32, 34, 36, 36, 39)));
                        return;
                    case (int)ClassCareers.Spellsword:
                        for (int i = 0; i < bookAmount; i++)
                            targetItems.Add(ItemBuilder.CreateRandomBookOfSpecificSubject((ItemGroups)PickOneOf(33, 33, 33, 33, 33, 33, 28, 28, 30, 30, 30, 30, 30, 31, 31, 32, 34, 36, 36, 39)));
                        return;
                    case (int)ClassCareers.Battlemage:
                        for (int i = 0; i < bookAmount; i++)
                            targetItems.Add(ItemBuilder.CreateRandomBookOfSpecificSubject((ItemGroups)PickOneOf(33, 33, 33, 33, 28, 28, 28, 30, 30, 30, 30, 30, 30, 31, 31, 32, 34, 36, 36, 39)));
                        return;
                    case (int)ClassCareers.Sorcerer:
                        for (int i = 0; i < bookAmount; i++)
                            targetItems.Add(ItemBuilder.CreateRandomBookOfSpecificSubject((ItemGroups)PickOneOf(33, 33, 33, 28, 28, 30, 30, 30, 30, 30, 30, 30, 31, 31, 31, 32, 34, 36, 36, 39)));
                        return;
                    case (int)ClassCareers.Healer:
                    case (int)ClassCareers.Monk:
                        for (int i = 0; i < bookAmount; i++)
                            targetItems.Add(ItemBuilder.CreateRandomBookOfSpecificSubject((ItemGroups)PickOneOf(38, 38, 38, 38, 38, 38, 38, 38, 33, 28, 30, 30, 31, 31, 31, 32, 34, 36, 39, 39)));
                        return;
                    case (int)ClassCareers.Nightblade:
                        for (int i = 0; i < bookAmount; i++)
                            targetItems.Add(ItemBuilder.CreateRandomBookOfSpecificSubject((ItemGroups)PickOneOf(28, 29, 30, 30, 30, 30, 30, 30, 31, 31, 33, 33, 34, 35, 36, 36, 37, 38, 38, 39)));
                        return;
                    case (int)ClassCareers.Bard:
                        for (int i = 0; i < bookAmount; i++)
                            targetItems.Add(ItemBuilder.CreateRandomBookOfSpecificSubject((ItemGroups)PickOneOf(28, 28, 29, 30, 30, 30, 31, 31, 31, 32, 32, 34, 35, 36, 36, 36, 36, 38, 38, 39)));
                        return;
                    case (int)ClassCareers.Assassin:
                        for (int i = 0; i < bookAmount; i++)
                            targetItems.Add(ItemBuilder.CreateRandomBookOfSpecificSubject((ItemGroups)PickOneOf(28, 28, 29, 29, 29, 30, 30, 31, 31, 31, 31, 33, 33, 34, 35, 36, 37, 37, 38, 39)));
                        return;
                    case (int)ClassCareers.Ranger:
                        for (int i = 0; i < bookAmount; i++)
                            targetItems.Add(ItemBuilder.CreateRandomBookOfSpecificSubject((ItemGroups)PickOneOf(28, 28, 29, 30, 30, 31, 31, 31, 32, 33, 33, 33, 33, 34, 35, 36, 38, 39, 39, 39)));
                        return;
                    case (int)ClassCareers.Knight:
                        for (int i = 0; i < bookAmount; i++)
                            targetItems.Add(ItemBuilder.CreateRandomBookOfSpecificSubject((ItemGroups)PickOneOf(28, 28, 28, 30, 30, 30, 31, 31, 31, 36, 36, 36, 36, 37, 37, 38, 38, 38, 38, 39)));
                        return;
                    default:
                        for (int i = 0; i < bookAmount; i++)
                            targetItems.Add(ItemBuilder.CreateRandomBookOfRandomSubject());
                        return;
                }
            }
            else
            {
                switch (AITarget.CareerIndex)
                {
                    case 21:
                        for (int i = 0; i < bookAmount; i++)
                            targetItems.Add(ItemBuilder.CreateRandomBookOfSpecificSubject((ItemGroups)PickOneOf(28, 30, 30, 30, 30, 31, 31, 33, 33, 36, 38, 38, 38, 38, 38, 38, 38, 38, 38, 39)));
                        return;
                    case 32:
                    case 33:
                        for (int i = 0; i < bookAmount; i++)
                            targetItems.Add(ItemBuilder.CreateRandomBookOfSpecificSubject((ItemGroups)PickOneOf(28, 30, 30, 31, 33, 33, 33, 33, 33, 33, 33, 33, 33, 33, 34, 35, 35, 36, 38, 39)));
                        return;
                    default:
                        for (int i = 0; i < bookAmount; i++)
                            targetItems.Add(ItemBuilder.CreateRandomBookOfRandomSubject());
                        return;
                }
            }
        }

        static void AddClothesBasedOnEnemy(Genders playerGender, Races playerRace, EnemyEntity AITarget, int[] condMods, List<DaggerfallUnityItem> targetItems)
        {
            Genders enemyGender = AITarget.Gender;

            if (AITarget.EntityType == EntityTypes.EnemyClass)
            {
                switch (AITarget.CareerIndex)
                {
                    case (int)ClassCareers.Mage:
                        if (enemyGender == Genders.Male)
                        {
                            targetItems.Add(ItemBuilder.CreateMensClothing(MensClothing.Plain_robes, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            targetItems.Add(ItemBuilder.CreateRandomShoes(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateRandomPants(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateMensClothing(MensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        else
                        {
                            targetItems.Add(ItemBuilder.CreateWomensClothing(WomensClothing.Plain_robes, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            targetItems.Add(ItemBuilder.CreateRandomShoes(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateRandomPants(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateWomensClothing(WomensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        return;
                    case (int)ClassCareers.Spellsword:
                    case (int)ClassCareers.Battlemage:
                    case (int)ClassCareers.Sorcerer:
                    case (int)ClassCareers.Bard:
                    case (int)ClassCareers.Burglar:
                    case (int)ClassCareers.Rogue:
                    case (int)ClassCareers.Thief:
                    case (int)ClassCareers.Archer:
                    case (int)ClassCareers.Warrior:
                    case (int)ClassCareers.Knight:
                        if (enemyGender == Genders.Male)
                        {
                            targetItems.Add(ItemBuilder.CreateRandomShirt(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateRandomPants(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateRandomShoes(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateMensClothing(MensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        else
                        {
                            targetItems.Add(ItemBuilder.CreateRandomShirt(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateRandomPants(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateRandomShoes(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateRandomBra(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateWomensClothing(WomensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        return;
                    case (int)ClassCareers.Healer:
                        if (enemyGender == Genders.Male)
                        {
                            targetItems.Add(ItemBuilder.CreateMensClothing(MensClothing.Priest_robes, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            targetItems.Add(ItemBuilder.CreateRandomShoes(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateRandomPants(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateMensClothing((MensClothing)PickOneOf((int)MensClothing.Casual_cloak, (int)MensClothing.Casual_cloak, (int)MensClothing.Formal_cloak), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        else
                        {
                            targetItems.Add(ItemBuilder.CreateWomensClothing(WomensClothing.Priestess_robes, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            targetItems.Add(ItemBuilder.CreateRandomShoes(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateRandomPants(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateWomensClothing((WomensClothing)PickOneOf((int)WomensClothing.Casual_cloak, (int)WomensClothing.Casual_cloak, (int)WomensClothing.Formal_cloak), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        return;
                    case (int)ClassCareers.Nightblade:
                    case (int)ClassCareers.Assassin:
                    case (int)ClassCareers.Ranger:
                        if (enemyGender == Genders.Male)
                        {
                            targetItems.Add(ItemBuilder.CreateRandomShirt(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateRandomPants(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateRandomShoes(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateMensClothing(MensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        else
                        {
                            targetItems.Add(ItemBuilder.CreateRandomShirt(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateRandomPants(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateRandomShoes(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateWomensClothing(WomensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateRandomBra(enemyGender, playerRace, condMods[0], condMods[1]));
                        }
                        return;
                    case (int)ClassCareers.Acrobat:
                        if (enemyGender == Genders.Male)
                        {
                            targetItems.Add(ItemBuilder.CreateMensClothing(MensClothing.Khajiit_suit, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            targetItems.Add(ItemBuilder.CreateMensClothing((MensClothing)PickOneOf((int)MensClothing.Shoes, (int)MensClothing.Sandals), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateMensClothing(MensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        else
                        {
                            targetItems.Add(ItemBuilder.CreateWomensClothing(WomensClothing.Khajiit_suit, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            targetItems.Add(ItemBuilder.CreateWomensClothing((WomensClothing)PickOneOf((int)WomensClothing.Shoes, (int)WomensClothing.Sandals), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateWomensClothing(WomensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        return;
                    case (int)ClassCareers.Monk:
                        if (enemyGender == Genders.Male)
                        {
                            targetItems.Add(ItemBuilder.CreateRandomPants(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateMensClothing((MensClothing)PickOneOf((int)MensClothing.Sash, (int)MensClothing.Toga, (int)MensClothing.Kimono, (int)MensClothing.Armbands, (int)MensClothing.Vest), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateMensClothing((MensClothing)PickOneOf((int)MensClothing.Shoes, (int)MensClothing.Sandals), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            if (Dice100.SuccessRoll(25))
                                targetItems.Add(ItemBuilder.CreateMensClothing(MensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        else
                        {
                            targetItems.Add(ItemBuilder.CreateRandomPants(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateRandomBra(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateWomensClothing((WomensClothing)PickOneOf((int)WomensClothing.Shoes, (int)WomensClothing.Sandals), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            if (Dice100.SuccessRoll(25))
                                targetItems.Add(ItemBuilder.CreateWomensClothing(WomensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        return;
                    case (int)ClassCareers.Barbarian:
                        if (enemyGender == Genders.Male)
                        {
                            targetItems.Add(ItemBuilder.CreateRandomShoes(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateMensClothing((MensClothing)PickOneOf((int)MensClothing.Short_skirt, (int)MensClothing.Long_Skirt, (int)MensClothing.Loincloth, (int)MensClothing.Wrap), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateMensClothing((MensClothing)PickOneOf((int)MensClothing.Sash, (int)MensClothing.Armbands, (int)MensClothing.Fancy_Armbands, (int)MensClothing.Straps, (int)MensClothing.Challenger_Straps, (int)MensClothing.Champion_straps), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateMensClothing(MensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        else
                        {
                            targetItems.Add(ItemBuilder.CreateRandomBra(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateRandomShoes(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateWomensClothing((WomensClothing)PickOneOf((int)WomensClothing.Loincloth, (int)WomensClothing.Wrap, (int)WomensClothing.Long_skirt), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateWomensClothing(WomensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        return;
                    default:
                        if (enemyGender == Genders.Male)
                        {
                            targetItems.Add(ItemBuilder.CreateRandomShirt(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateRandomPants(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateRandomShoes(enemyGender, playerRace, condMods[0], condMods[1]));
                        }
                        else
                        {
                            targetItems.Add(ItemBuilder.CreateRandomShirt(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateRandomPants(enemyGender, playerRace, condMods[0], condMods[1]));
                            targetItems.Add(ItemBuilder.CreateRandomShoes(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateRandomBra(enemyGender, playerRace, condMods[0], condMods[1]));
                        }
                        return;
                }
            }
            else
            {
                switch (AITarget.CareerIndex)
                {
                    case 7:
                    case 12:
                    case 21:
                        targetItems.Add(ItemBuilder.CreateRandomShoes(Genders.Male, playerRace, condMods[0], condMods[1]));
                        targetItems.Add(ItemBuilder.CreateMensClothing((MensClothing)PickOneOf((int)MensClothing.Short_skirt, (int)MensClothing.Long_Skirt, (int)MensClothing.Loincloth, (int)MensClothing.Wrap), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        if (Dice100.SuccessRoll(50))
                            targetItems.Add(ItemBuilder.CreateMensClothing((MensClothing)PickOneOf((int)MensClothing.Straps, (int)MensClothing.Challenger_Straps), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        if (Dice100.SuccessRoll(50))
                            targetItems.Add(ItemBuilder.CreateMensClothing(MensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        return;
                    case 24:
                        targetItems.Add(ItemBuilder.CreateRandomShoes(Genders.Male, playerRace, condMods[0], condMods[1]));
                        targetItems.Add(ItemBuilder.CreateMensClothing((MensClothing)PickOneOf((int)MensClothing.Short_skirt, (int)MensClothing.Long_Skirt, (int)MensClothing.Loincloth, (int)MensClothing.Wrap), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        if (Dice100.SuccessRoll(50))
                            targetItems.Add(ItemBuilder.CreateMensClothing((MensClothing)PickOneOf((int)MensClothing.Challenger_Straps, (int)MensClothing.Champion_straps), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        if (Dice100.SuccessRoll(50))
                            targetItems.Add(ItemBuilder.CreateMensClothing(MensClothing.Formal_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        return;
                    case 15:
                    case 17:
                        if (playerGender == Genders.Male)
                        {
                            if (Dice100.SuccessRoll(20))
                                targetItems.Add(ItemBuilder.CreateRandomShirt(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(20))
                                targetItems.Add(ItemBuilder.CreateRandomPants(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(20))
                                targetItems.Add(ItemBuilder.CreateRandomShoes(enemyGender, playerRace, condMods[0], condMods[1]));
                        }
                        else
                        {
                            if (Dice100.SuccessRoll(20))
                                targetItems.Add(ItemBuilder.CreateRandomShirt(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(20))
                                targetItems.Add(ItemBuilder.CreateRandomPants(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(20))
                                targetItems.Add(ItemBuilder.CreateRandomShoes(enemyGender, playerRace, condMods[0], condMods[1]));
                            if (Dice100.SuccessRoll(10))
                                targetItems.Add(ItemBuilder.CreateRandomBra(enemyGender, playerRace, condMods[0], condMods[1]));
                        }
                        return;
                    case 28:
                        targetItems.Add(ItemBuilder.CreateRandomShirt(Genders.Female, playerRace, condMods[0], condMods[1]));
                        targetItems.Add(ItemBuilder.CreateRandomPants(Genders.Female, playerRace, condMods[0], condMods[1]));
                        targetItems.Add(ItemBuilder.CreateRandomShoes(Genders.Female, playerRace, condMods[0], condMods[1]));
                        targetItems.Add(ItemBuilder.CreateWomensClothing(WomensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1], DyeColors.Grey));
                        if (Dice100.SuccessRoll(50))
                            targetItems.Add(ItemBuilder.CreateRandomBra(Genders.Female, playerRace, condMods[0], condMods[1]));
                        return;
                    case 30:
                        targetItems.Add(ItemBuilder.CreateRandomShirt(Genders.Male, playerRace, condMods[0], condMods[1]));
                        targetItems.Add(ItemBuilder.CreateRandomPants(Genders.Male, playerRace, condMods[0], condMods[1]));
                        targetItems.Add(ItemBuilder.CreateRandomShoes(Genders.Male, playerRace, condMods[0], condMods[1]));
                        targetItems.Add(ItemBuilder.CreateMensClothing(MensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1], DyeColors.Grey));
                        return;
                    case 32:
                    case 33:
                        if (playerGender == Genders.Male)
                        {
                            targetItems.Add(ItemBuilder.CreateMensClothing(MensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateMensClothing((MensClothing)PickOneOf((int)MensClothing.Boots, (int)MensClothing.Tall_Boots), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        else
                        {
                            targetItems.Add(ItemBuilder.CreateWomensClothing(WomensClothing.Casual_cloak, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            if (Dice100.SuccessRoll(50))
                                targetItems.Add(ItemBuilder.CreateWomensClothing((WomensClothing)PickOneOf((int)WomensClothing.Boots, (int)WomensClothing.Tall_boots), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        return;
                    case 27:
                        if (playerGender == Genders.Male)
                        {
                            targetItems.Add(ItemBuilder.CreateMensClothing(MensClothing.Loincloth, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            targetItems.Add(ItemBuilder.CreateMensClothing((MensClothing)PickOneOf((int)MensClothing.Boots, (int)MensClothing.Tall_Boots), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        else
                        {
                            targetItems.Add(ItemBuilder.CreateWomensClothing(WomensClothing.Loincloth, playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                            targetItems.Add(ItemBuilder.CreateWomensClothing((WomensClothing)PickOneOf((int)WomensClothing.Boots, (int)WomensClothing.Tall_boots), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        }
                        return;
                    case 29:
                        if (Dice100.SuccessRoll(50))
                            targetItems.Add(ItemBuilder.CreateWomensClothing((WomensClothing)PickOneOf((int)WomensClothing.Eodoric, (int)WomensClothing.Formal_eodoric, (int)WomensClothing.Strapless_dress), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        if (Dice100.SuccessRoll(20))
                            targetItems.Add(ItemBuilder.CreateRandomBra(Genders.Female, playerRace, condMods[0], condMods[1]));
                        return;
                    case 31:
                        targetItems.Add(ItemBuilder.CreateRandomPants(Genders.Male, playerRace, condMods[0], condMods[1]));
                        targetItems.Add(ItemBuilder.CreateMensClothing((MensClothing)PickOneOf((int)MensClothing.Boots, (int)MensClothing.Tall_Boots), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        targetItems.Add(ItemBuilder.CreateMensClothing((MensClothing)PickOneOf((int)MensClothing.Casual_cloak, (int)MensClothing.Formal_cloak), playerRace, -1, condMods[0], condMods[1],  ItemBuilder.RandomClothingDye()));
                        if (Dice100.SuccessRoll(50))
                            targetItems.Add(ItemBuilder.CreateRandomShirt(Genders.Male, playerRace, condMods[0], condMods[1]));
                        return;
                    default:
                        return;
                }
            }
        }

        public static float IngotDropChance(EnemyEntity AITarget)
        {
            int level = AITarget.Level;

            if (AITarget.EntityType == EntityTypes.EnemyClass)
            {
                switch (AITarget.CareerIndex)
                {
                    case (int)ClassCareers.Spellsword:
                    case (int)ClassCareers.Battlemage:
                        return 7.5f + (0.5f * level);
                    case (int)ClassCareers.Burglar:
                    case (int)ClassCareers.Rogue:
                    case (int)ClassCareers.Thief:
                        return 15f + (1.5f * level);
                    case (int)ClassCareers.Archer:
                    case (int)ClassCareers.Barbarian:
                    case (int)ClassCareers.Warrior:
                    case (int)ClassCareers.Knight:
                        return 10f + (1f * level);
                    default:
                        return 0f;
                }
            }
            else
            {
                switch (AITarget.CareerIndex)
                {
                    case 8:
                    case 16:
                        return 2f;
                    case 7:
                        return 5f;
                    case 12:
                        return 10f;
                    case 21:
                        return 3f;
                    case 24:
                        return 20f;
                    case 31:
                        return 40f;
                    default:
                        return 0f;
                }
            }
        }

        public static int[] EnemyLootConditionCalculator(DaggerfallEntity enemy, int[] traits)
        {
            // Index meanings: 0 = minCond%, 1 = maxCond%.
            int[] equipTableProps = { -1, -1 };

            EnemyEntity AITarget = enemy as EnemyEntity;

            if (EnemyEntity.EquipmentUser(AITarget))
            {
                if (AITarget.EntityType == EntityTypes.EnemyClass)
                {
                    switch (AITarget.CareerIndex)
                    {
                        case (int)ClassCareers.Mage:
                            equipTableProps[0] = 20;
                            equipTableProps[1] = 60;
                            break;
                        case (int)ClassCareers.Spellsword:
                            equipTableProps[0] = 30;
                            equipTableProps[1] = 70;
                            break;
                        case (int)ClassCareers.Battlemage:
                            equipTableProps[0] = 30;
                            equipTableProps[1] = 70;
                            break;
                        case (int)ClassCareers.Sorcerer:
                            equipTableProps[0] = 20;
                            equipTableProps[1] = 60;
                            break;
                        case (int)ClassCareers.Healer:
                            equipTableProps[0] = 25;
                            equipTableProps[1] = 65;
                            break;
                        case (int)ClassCareers.Nightblade:
                            equipTableProps[0] = 40;
                            equipTableProps[1] = 80;
                            break;
                        case (int)ClassCareers.Bard:
                            equipTableProps[0] = 30;
                            equipTableProps[1] = 70;
                            break;
                        case (int)ClassCareers.Burglar:
                            equipTableProps[0] = 20;
                            equipTableProps[1] = 60;
                            break;
                        case (int)ClassCareers.Rogue:
                            equipTableProps[0] = 20;
                            equipTableProps[1] = 60;
                            break;
                        case (int)ClassCareers.Acrobat:
                            equipTableProps[0] = 25;
                            equipTableProps[1] = 65;
                            break;
                        case (int)ClassCareers.Thief:
                            equipTableProps[0] = 20;
                            equipTableProps[1] = 60;
                            break;
                        case (int)ClassCareers.Assassin:
                            equipTableProps[0] = 40;
                            equipTableProps[1] = 80;
                            break;
                        case (int)ClassCareers.Monk:
                            equipTableProps[0] = 25;
                            equipTableProps[1] = 65;
                            break;
                        case (int)ClassCareers.Archer:
                            equipTableProps[0] = 35;
                            equipTableProps[1] = 75;
                            break;
                        case (int)ClassCareers.Ranger:
                            equipTableProps[0] = 30;
                            equipTableProps[1] = 70;
                            break;
                        case (int)ClassCareers.Barbarian:
                            equipTableProps[0] = 20;
                            equipTableProps[1] = 60;
                            break;
                        case (int)ClassCareers.Warrior:
                            equipTableProps[0] = 40;
                            equipTableProps[1] = 80;
                            break;
                        case (int)ClassCareers.Knight:
                            equipTableProps[0] = 45;
                            equipTableProps[1] = 85;
                            break;
                        default:
                            return equipTableProps;
                    }
                }
                else
                {
                    switch (AITarget.CareerIndex)
                    {
                        case 7:
                            equipTableProps[0] = 30;
                            equipTableProps[1] = 70;
                            break;
                        case 8:
                            equipTableProps[0] = 15;
                            equipTableProps[1] = 55;
                            break;
                        case 12:
                            equipTableProps[0] = 35;
                            equipTableProps[1] = 75;
                            break;
                        case 15:
                            equipTableProps[0] = 5;
                            equipTableProps[1] = 45;
                            break;
                        case 17:
                            equipTableProps[0] = 5;
                            equipTableProps[1] = 45;
                            break;
                        case 21:
                            equipTableProps[0] = 20;
                            equipTableProps[1] = 60;
                            break;
                        case 23:
                            equipTableProps[0] = 10;
                            equipTableProps[1] = 50;
                            break;
                        case 24:
                            equipTableProps[0] = 40;
                            equipTableProps[1] = 80;
                            break;
                        case 25:
                            equipTableProps[0] = 15;
                            equipTableProps[1] = 55;
                            break;
                        case 26:
                            equipTableProps[0] = 15;
                            equipTableProps[1] = 55;
                            break;
                        case 27:
                            equipTableProps[0] = 15;
                            equipTableProps[1] = 55;
                            break;
                        case 29:
                            equipTableProps[0] = 25;
                            equipTableProps[1] = 65;
                            break;
                        case 31:
                            equipTableProps[0] = 35;
                            equipTableProps[1] = 75;
                            break;
                        case 28:
                        case 30:
                            equipTableProps[0] = 25;
                            equipTableProps[1] = 65;
                            break;
                        case 32:
                        case 33:
                            equipTableProps[0] = 20;
                            equipTableProps[1] = 60;
                            break;
                        default:
                            return equipTableProps;
                    }
                }
            }
            else
            {
                return equipTableProps;
            }

            equipTableProps = TraitLootConditionCalculator(enemy, traits, equipTableProps);

            return equipTableProps;
        }

        public static int[] TraitLootConditionCalculator(DaggerfallEntity enemy, int[] traits, int[] equipTableProps)
        {
            if (traits[0] == (int)MobilePersonalityQuirks.Prepared || traits[1] == (int)MobilePersonalityQuirks.Prepared)
            {
                equipTableProps[0] = (int)Mathf.Clamp(equipTableProps[0] + 15, 1, 95);
                equipTableProps[1] = (int)Mathf.Clamp(equipTableProps[1] + 15, 1, 95);
            }

            if (traits[0] == (int)MobilePersonalityQuirks.Reckless || traits[1] == (int)MobilePersonalityQuirks.Reckless)
            {
                equipTableProps[0] = (int)Mathf.Clamp(equipTableProps[0] - 15, 1, 95);
                equipTableProps[1] = (int)Mathf.Clamp(equipTableProps[1] - 15, 1, 95);
            }

            if (traits[0] == (int)MobilePersonalityQuirks.Cautious || traits[1] == (int)MobilePersonalityQuirks.Cautious)
            {
                equipTableProps[0] = (int)Mathf.Clamp(equipTableProps[0] + 5, 1, 95);
                equipTableProps[1] = (int)Mathf.Clamp(equipTableProps[1] + 5, 1, 95);
            }

            if (traits[0] == (int)MobilePersonalityQuirks.Hoarder || traits[1] == (int)MobilePersonalityQuirks.Hoarder)
            {
                equipTableProps[0] = (int)Mathf.Clamp(equipTableProps[0] - 10, 1, 95);
                equipTableProps[1] = (int)Mathf.Clamp(equipTableProps[1] - 10, 1, 95);
            }

            if (traits[2] == (int)MobilePersonalityInterests.Collector)
            {
                equipTableProps[0] = (int)Mathf.Clamp(equipTableProps[0] + 5, 1, 95);
                equipTableProps[1] = (int)Mathf.Clamp(equipTableProps[1] + 5, 1, 95);
            }

            if (traits[2] == (int)MobilePersonalityInterests.Survivalist)
            {
                equipTableProps[0] = (int)Mathf.Clamp(equipTableProps[0] + 10, 1, 95);
                equipTableProps[1] = (int)Mathf.Clamp(equipTableProps[1] + 10, 1, 95);
            }

            if (traits[2] == (int)MobilePersonalityInterests.Diver)
            {
                equipTableProps[0] = (int)Mathf.Clamp(equipTableProps[0] - 5, 1, 95);
                equipTableProps[1] = (int)Mathf.Clamp(equipTableProps[1] - 5, 1, 95);
            }

            if (traits[2] == (int)MobilePersonalityInterests.Handy)
            {
                equipTableProps[0] = (int)Mathf.Clamp(equipTableProps[0] + 15, 1, 95);
                equipTableProps[1] = (int)Mathf.Clamp(equipTableProps[1] + 15, 1, 95);
            }

            return equipTableProps;
        }

        static void RandomIngredient(float chance, ItemGroups ingredientGroup, List<DaggerfallUnityItem> targetItems)
        {
            while (Dice100.SuccessRoll((int)chance))
            {
                targetItems.Add(ItemBuilder.CreateRandomIngredient(ingredientGroup));
                chance *= 0.5f;
            }
        }

        #endregion
    }
}
