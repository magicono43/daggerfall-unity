// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2020 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Allofich, Hazelnut, ifkopifko, Numidium, TheLacus
// 
// Notes:
//

using UnityEngine;
using System;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using System.Collections.Generic;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Save;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Utility.ModSupport;

namespace DaggerfallWorkshop.Game.Formulas
{
    /// <summary>
    /// Common formulas used throughout game.
    /// Where the exact formula is unknown, a "best effort" approximation will be used.
    /// Most formula can be overridden by registering a new method matching the appropriate delegate signature.
    /// Other signatures can be added upon demand.
    /// </summary>
    public static class FormulaHelper
    {
        private struct FormulaOverride
        {
            internal readonly Delegate Formula;
            internal readonly Mod Provider;

            internal FormulaOverride(Delegate formula, Mod provider)
            {
                Formula = formula;
                Provider = provider;
            }
        }

        readonly static Dictionary<string, FormulaOverride> overrides = new Dictionary<string, FormulaOverride>();

        public static float specialInfectionChance = 0.6f;

        // Approximation of classic frame updates
        public const int classicFrameUpdate = 980;

        /// <summary>Struct for return values of formula that affect damage and to-hit chance.</summary>
        public struct ToHitAndDamageMods
        {
            public int damageMod;
            public int toHitMod;
            public int damType;
        }

        #region Basic Formulas

        public static int DamageModifier(int strength)
        {
            Func<int, int> del;
            if (TryGetOverride("DamageModifier", out del))
                return del(strength);

            return (int)Mathf.Floor((float)(strength - 50) / 5f);
        }

        public static int MaxEncumbrance(int strength)
        {
            Func<int, int> del;
            if (TryGetOverride("MaxEncumbrance", out del))
                return del(strength);

            return (int)Mathf.Floor(strength * 1f);
        }

        public static int SpellPoints(int intelligence, float multiplier)
        {
            Func<int, float, int> del;
            if (TryGetOverride("SpellPoints", out del))
                return del(intelligence, multiplier);

            return (int)Mathf.Floor((float)intelligence * multiplier);
        }

        public static int MagicResist(int willpower)
        {
            Func<int, int> del;
            if (TryGetOverride("MagicResist", out del))
                return del(willpower);

            return (int)Mathf.Floor((float)willpower / 10f);
        }

        public static int ToHitModifier(int agility)
        {
            Func<int, int> del;
            if (TryGetOverride("ToHitModifier", out del))
                return del(agility);

            return (int)Mathf.Floor((float)agility / 10f) - 5;
        }

        public static int HitPointsModifier(int endurance)
        {
            Func<int, int> del;
            if (TryGetOverride("HitPointsModifier", out del))
                return del(endurance);

            return (int)Mathf.Floor((float)endurance / 10f) - 5;
        }

        public static int HealingRateModifier(int endurance)
        {
            Func<int, int> del;
            if (TryGetOverride("HealingRateModifier", out del))
                return del(endurance);

            // Original Daggerfall seems to have a bug where negative endurance modifiers on healing rate
            // are applied as modifier + 1. Not recreating that here.
            return (int)Mathf.Floor((float)endurance / 10f) - 5;
        }

        public static int MaxStatValue()
        {
            Func<int> del;
            if (TryGetOverride("MaxStatValue", out del))
                return del();
            else
                return 100;
        }

        public static int BonusPool()
        {
            Func<int> del;
            if (TryGetOverride("BonusPool", out del))
                return del();

            const int minBonusPool = 4;        // The minimum number of free points to allocate on level up
            const int maxBonusPool = 6;        // The maximum number of free points to allocate on level up

            // Roll bonus pool for player to distribute
            // Using maxBonusPool + 1 for inclusive range
            return UnityEngine.Random.Range(minBonusPool, maxBonusPool + 1);
        }

        #endregion

        #region Player

        // Generates player health based on level and career hit points per level
        public static int RollMaxHealth(PlayerEntity player)
        {
            Func<PlayerEntity, int> del;
            if (TryGetOverride("RollMaxHealth", out del))
                return del(player);

            const int baseHealth = 25;
            int maxHealth = baseHealth + player.Career.HitPointsPerLevel;

            for (int i = 1; i < player.Level; i++)
            {
                maxHealth += CalculateHitPointsPerLevelUp(player);
            }

            return maxHealth;
        }

        // Calculate how much health the player should recover per hour of rest
        public static int CalculateHealthRecoveryRate(PlayerEntity player)
        {
            Func<PlayerEntity, int> del;
            if (TryGetOverride("CalculateHealthRecoveryRate", out del))
                return del(player);

            short medical = player.Skills.GetLiveSkillValue(DFCareer.Skills.Medical);
            int endurance = player.Stats.LiveEndurance;
            int maxHealth = player.MaxHealth;
            PlayerEnterExit playerEnterExit;
            playerEnterExit = GameManager.Instance.PlayerGPS.GetComponent<PlayerEnterExit>();
            DFCareer.RapidHealingFlags rapidHealingFlags = player.Career.RapidHealing;

            short addToMedical = 60;

            if (rapidHealingFlags == DFCareer.RapidHealingFlags.Always)
                addToMedical = 100;
            else if (DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.IsDay && !playerEnterExit.IsPlayerInside)
            {
                if (rapidHealingFlags == DFCareer.RapidHealingFlags.InLight)
                    addToMedical = 100;
            }
            else if (rapidHealingFlags == DFCareer.RapidHealingFlags.InDarkness)
                addToMedical = 100;

            medical += addToMedical;

            return Mathf.Max((int)Mathf.Floor(HealingRateModifier(endurance) + medical * maxHealth / 1000), 1);
        }

        // Calculate how much fatigue the player should recover per hour of rest
        public static int CalculateFatigueRecoveryRate(int maxFatigue)
        {
            Func<int, int> del;
            if (TryGetOverride("HealingRateModifier", out del))
                return del(maxFatigue);

            return Mathf.Max((int)Mathf.Floor(maxFatigue / 8), 1);
        }

        // Calculate how many spell points the player should recover per hour of rest
        public static int CalculateSpellPointRecoveryRate(PlayerEntity player)
        {
            Func<PlayerEntity, int> del;
            if (TryGetOverride("HealingRateModifier", out del))
                return del(player);

            if (player.Career.NoRegenSpellPoints)
                return 0;

            return Mathf.Max((int)Mathf.Floor(player.MaxMagicka / 8), 1);
        }

        // Calculate chance of successfully lockpicking a door in an interior (an animating door). If this is higher than a random number between 0 and 100 (inclusive), the lockpicking succeeds.
        public static int CalculateInteriorLockpickingChance(int level, int lockvalue, int lockpickingSkill)
        {
            Func<int, int, int, int> del;
            if (TryGetOverride("CalculateInteriorLockpickingChance", out del))
                return del(level, lockvalue, lockpickingSkill);

            int chance = (5 * (level - lockvalue) + lockpickingSkill);
            return Mathf.Clamp(chance, 5, 95);
        }

        // Calculate chance of successfully lockpicking a door in an exterior (a door that leads to an interior). If this is higher than a random number between 0 and 100 (inclusive), the lockpicking succeeds.
        public static int CalculateExteriorLockpickingChance(int lockvalue, int lockpickingSkill)
        {
            Func<int, int, int> del;
            if (TryGetOverride("CalculateExteriorLockpickingChance", out del))
                return del(lockvalue, lockpickingSkill);

            int chance = lockpickingSkill - (5 * lockvalue);
            return Mathf.Clamp(chance, 5, 95);
        }

        // Calculate chance of successfully pickpocketing a target
        public static int CalculatePickpocketingChance(PlayerEntity player, EnemyEntity target)
        {
            Func<PlayerEntity, EnemyEntity, int> del;
            if (TryGetOverride("CalculatePickpocketingChance", out del))
                return del(player, target);

            int chance = player.Skills.GetLiveSkillValue(DFCareer.Skills.Pickpocket);
            // If target is an enemy mobile, apply level modifier.
            if (target != null)
            {
                chance += 5 * ((player.Level) - (target.Level));
            }
            return Mathf.Clamp(chance, 5, 95);
        }

        // Calculate chance of being caught shoplifting items
        public static int CalculateShopliftingChance(PlayerEntity player, int shopQuality, int weightAndNumItems)
        {
            Func<PlayerEntity, int, int, int> del;
            if (TryGetOverride("CalculateShopliftingChance", out del))
                return del(player, shopQuality, weightAndNumItems);

            int chance = 100 - player.Skills.GetLiveSkillValue(DFCareer.Skills.Pickpocket);
            chance += shopQuality + weightAndNumItems;
            return Mathf.Clamp(chance, 5, 95);
        }

        // Calculate chance of successfully climbing - checked repeatedly while climbing
        public static int CalculateClimbingChance(PlayerEntity player, int basePercentSuccess)
        {
            Func<PlayerEntity, int, int> del;
            if (TryGetOverride("CalculateClimbingChance", out del))
                return del(player, basePercentSuccess);

            int skill = player.Skills.GetLiveSkillValue(DFCareer.Skills.Climbing);
            int luck = player.Stats.GetLiveStatValue(DFCareer.Stats.Luck);
            if (player.Race == Races.Khajiit)
                skill += 30;

            // Climbing effect states "target can climb twice as well" - doubling effective skill after racial applied
            if (player.IsEnhancedClimbing)
                skill *= 2;

            // Clamp skill range
            skill = Mathf.Clamp(skill, 5, 95);
            float luckFactor = Mathf.Lerp(0, 10, luck * 0.01f);

            // Skill Check
            int chance = (int) (Mathf.Lerp(basePercentSuccess, 100, skill * .01f) + luckFactor);

            return chance;
        }

        // Calculate how many uses a skill needs before its value will rise.
        public static int CalculateSkillUsesForAdvancement(int skillValue, int skillAdvancementMultiplier, float careerAdvancementMultiplier, int level)
        {
            double levelMod = Math.Pow(1.04, level);
            return (int)Math.Floor((skillValue * skillAdvancementMultiplier * careerAdvancementMultiplier * levelMod * 2 / 5) + 1);
        }

        // Calculate player level.
        public static int CalculatePlayerLevel(int startingLevelUpSkillsSum, int currentLevelUpSkillsSum)
        {
            Func<int, int, int> del;
            if (TryGetOverride("CalculatePlayerLevel", out del))
                return del(startingLevelUpSkillsSum, currentLevelUpSkillsSum);

            return (int)Mathf.Floor((currentLevelUpSkillsSum - startingLevelUpSkillsSum + 28) / 15);
        }

        // Calculate hit points player gains per level.
        public static int CalculateHitPointsPerLevelUp(PlayerEntity player)
        {
            Func<PlayerEntity, int> del;
            if (TryGetOverride("CalculateHitPointsPerLevelUp", out del))
                return del(player);

            int minRoll = player.Career.HitPointsPerLevel / 2;
            int maxRoll = player.Career.HitPointsPerLevel;
            int addHitPoints = UnityEngine.Random.Range(minRoll, maxRoll + 1); // Adding +1 as Unity Random.Range(int,int) is exclusive of maximum value
            addHitPoints += HitPointsModifier(player.Stats.PermanentEndurance);
            if (addHitPoints < 1)
                addHitPoints = 1;
            return addHitPoints;
        }

        // Calculate whether the player is successful at pacifying an enemy.
        public static bool CalculateEnemyPacification(PlayerEntity player, DFCareer.Skills languageSkill)
        {
            Func<PlayerEntity, DFCareer.Skills, bool> del;
            if (TryGetOverride("CalculateEnemyPacification", out del))
                return del(player, languageSkill);

            double chance = 0;
            if (languageSkill == DFCareer.Skills.Etiquette ||
                languageSkill == DFCareer.Skills.Streetwise)
            {
                chance += player.Skills.GetLiveSkillValue(languageSkill) / 10;
                chance += player.Stats.LivePersonality / 5;
            }
            else
            {
                chance += player.Skills.GetLiveSkillValue(languageSkill);
                chance += player.Stats.LivePersonality / 10;
            }
            chance += GameManager.Instance.WeaponManager.Sheathed ? 10 : -25;

            // Add chance from Comprehend Languages effect if present
            ComprehendLanguages languagesEffect = (ComprehendLanguages)GameManager.Instance.PlayerEffectManager.FindIncumbentEffect<ComprehendLanguages>();
            if (languagesEffect != null)
                chance += languagesEffect.ChanceValue(); // Possibly remove skill tally if comprehend language effect is active, since it was not your skill, but magic that did the work, maybe. 

            int roll = UnityEngine.Random.Range(0, 200);
            bool success = (roll < chance);
            //if (success)
            //    player.TallySkill(languageSkill, 1);
            //else if (languageSkill != DFCareer.Skills.Etiquette && languageSkill != DFCareer.Skills.Streetwise)
            //    player.TallySkill(languageSkill, 1);

            Debug.LogFormat("Pacification {3} using {0} skill: chance= {1}  roll= {2}", languageSkill, chance, roll, success ? "success" : "failure");
            return success;
        }

        // Calculate whether the player is blessed when donating to a Temple.
        public static int CalculateTempleBlessing(int donationAmount, int deityRep)
        {
            return 1;   // TODO Amount of stat boost, guessing what this formula might need...
        }

        // Gets vampire clan based on region
        public static VampireClans GetVampireClan(int regionIndex)
        {
            FactionFile.FactionData factionData;
            GameManager.Instance.PlayerEntity.FactionData.GetRegionFaction(regionIndex, out factionData);
            switch ((FactionFile.FactionIDs) factionData.vam)
            {
                case FactionFile.FactionIDs.The_Vraseth:
                    return VampireClans.Vraseth;
                case FactionFile.FactionIDs.The_Haarvenu:
                    return VampireClans.Haarvenu;
                case FactionFile.FactionIDs.The_Thrafey:
                    return VampireClans.Thrafey;
                case FactionFile.FactionIDs.The_Lyrezi:
                    return VampireClans.Lyrezi;
                case FactionFile.FactionIDs.The_Montalion:
                    return VampireClans.Montalion;
                case FactionFile.FactionIDs.The_Khulari:
                    return VampireClans.Khulari;
                case FactionFile.FactionIDs.The_Garlythi:
                    return VampireClans.Garlythi;
                case FactionFile.FactionIDs.The_Anthotis:
                    return VampireClans.Anthotis;
                case FactionFile.FactionIDs.The_Selenu:
                    return VampireClans.Selenu;
            }

            // The Lyrezi are the default like in classic
            return VampireClans.Lyrezi;
        }

        #endregion

        #region Combat & Damage

        public static int CalculateHandToHandMinDamage(int handToHandSkill)
        {
            Func<int, int> del;
            if (TryGetOverride("CalculateHandToHandMinDamage", out del))
                return del(handToHandSkill);

            return (handToHandSkill / 10) + 1;
        }

        public static int CalculateHandToHandMaxDamage(int handToHandSkill)
        {
            Func<int, int> del;
            if (TryGetOverride("CalculateHandToHandMaxDamage", out del))
                return del(handToHandSkill);

            // Daggerfall Chronicles table lists hand-to-hand skills of 80 and above (45 through 79 are omitted)
            // as if they give max damage of (handToHandSkill / 5) + 2, but the hand-to-hand damage display in the character sheet
            // in classic Daggerfall shows it as continuing to be (handToHandSkill / 5) + 1
            return (handToHandSkill / 5) + 1;
        }

        public static int CalcWeaponMinDamBaseBludgeoning(Weapons weapon)
        {
            Func<Weapons, int> del;
            if (TryGetOverride("CalculateWeaponMinDamage", out del))
                return del(weapon);

            switch (weapon)
            {
                case Weapons.Dagger:
                case Weapons.Tanto:
                case Weapons.Katana:
                case Weapons.Longsword:
                case Weapons.Long_Bow:
                case Weapons.Short_Bow:
                    return 0;
                case Weapons.Shortsword:
                case Weapons.Wakazashi:
                case Weapons.Dai_Katana:
                case Weapons.War_Axe:
                    return 1;
                case Weapons.Broadsword:
                case Weapons.Claymore:
                case Weapons.Battle_Axe:
                    return 2;
                case Weapons.Saber:
                case Weapons.Staff:
                    return 3;
                case Weapons.Flail:
                case Weapons.Mace:
                    return 5;
                case Weapons.Warhammer:
                    return 6;
                default:
                    return 0;
            }
        }

        public static int CalcWeaponMaxDamBaseBludgeoning(Weapons weapon)
        {
            Func<Weapons, int> del;
            if (TryGetOverride("CalculateWeaponMaxDamage", out del))
                return del(weapon);

            switch (weapon)
            {
                case Weapons.Long_Bow:
                case Weapons.Short_Bow:
                    return 0;
                case Weapons.Dagger:
                case Weapons.Tanto:
                    return 1;
                case Weapons.Katana:
                case Weapons.Wakazashi:
                    return 2;
                case Weapons.Shortsword:
                case Weapons.Dai_Katana:
                case Weapons.Longsword:
                case Weapons.War_Axe:
                    return 3;
                case Weapons.Broadsword:
                case Weapons.Saber:
                    return 4;
                case Weapons.Battle_Axe:
                    return 5;
                case Weapons.Claymore:
                    return 6;
                case Weapons.Staff:
                    return 7;
                case Weapons.Mace:
                    return 10;
                case Weapons.Flail:
                case Weapons.Warhammer:
                    return 13;
                default:
                    return 0;
            }
        }

        public static int CalcWeaponMinDamBaseSlashing(Weapons weapon)
        {
            Func<Weapons, int> del;
            if (TryGetOverride("CalculateWeaponMinDamage", out del))
                return del(weapon);

            switch (weapon)
            {
                case Weapons.Dagger:
                case Weapons.Tanto:
                case Weapons.Flail:
                case Weapons.Mace:
                case Weapons.Staff:
                case Weapons.Warhammer:
                case Weapons.Long_Bow:
                case Weapons.Short_Bow:
                    return 0;
                case Weapons.Shortsword:
                case Weapons.Wakazashi:
                case Weapons.Longsword:
                    return 2;
                case Weapons.Broadsword:
                case Weapons.Dai_Katana:
                case Weapons.Katana:
                case Weapons.Saber:
                    return 3;
                case Weapons.Claymore:
                case Weapons.Battle_Axe:
                    return 4;
                case Weapons.War_Axe:
                    return 5;
                default:
                    return 0;
            }
        }

        public static int CalcWeaponMaxDamBaseSlashing(Weapons weapon)
        {
            Func<Weapons, int> del;
            if (TryGetOverride("CalculateWeaponMaxDamage", out del))
                return del(weapon);

            switch (weapon)
            {
                case Weapons.Long_Bow:
                case Weapons.Short_Bow:
                    return 0;
                case Weapons.Flail:
                case Weapons.Mace:
                case Weapons.Staff:
                case Weapons.Warhammer:
                    return 1;
                case Weapons.Tanto:
                    return 2;
                case Weapons.Dagger:
                    return 4;
                case Weapons.Shortsword:
                case Weapons.Longsword:
                    return 5;
                case Weapons.Wakazashi:
                case Weapons.Saber:
                    return 7;
                case Weapons.Broadsword:
                    return 8;
                case Weapons.Claymore:
                case Weapons.Battle_Axe:
                    return 9;
                case Weapons.Katana:
                    return 10;
                case Weapons.War_Axe:
                    return 11;
                case Weapons.Dai_Katana:
                    return 13;
                default:
                    return 0;
            }
        }

        public static int CalcWeaponMinDamBasePiercing(Weapons weapon)
        {
            Func<Weapons, int> del;
            if (TryGetOverride("CalculateWeaponMinDamage", out del))
                return del(weapon);

            switch (weapon)
            {
                case Weapons.Saber:
                case Weapons.Battle_Axe:
                    return 0;
                case Weapons.Wakazashi:
                case Weapons.Broadsword:
                case Weapons.Claymore:
                case Weapons.Katana:
                case Weapons.War_Axe:
                case Weapons.Mace:
                    return 1;
                case Weapons.Shortsword:
                case Weapons.Dai_Katana:
                case Weapons.Flail:
                case Weapons.Staff:
                case Weapons.Warhammer:
                    return 2;
                case Weapons.Dagger:
                case Weapons.Tanto:
                case Weapons.Longsword:
                    return 3;
                case Weapons.Long_Bow:
                    return 5;
                case Weapons.Short_Bow:
                    return 7;
                default:
                    return 0;
            }
        }

        public static int CalcWeaponMaxDamBasePiercing(Weapons weapon)
        {
            Func<Weapons, int> del;
            if (TryGetOverride("CalculateWeaponMaxDamage", out del))
                return del(weapon);

            switch (weapon)
            {
                case Weapons.Battle_Axe:
                    return 1;
                case Weapons.Saber:
                    return 2;
                case Weapons.Shortsword:
                case Weapons.Wakazashi:
                case Weapons.Broadsword:
                case Weapons.Claymore:
                case Weapons.Katana:
                case Weapons.Mace:
                    return 3;
                case Weapons.Dai_Katana:
                case Weapons.Flail:
                case Weapons.Warhammer:
                    return 4;
                case Weapons.Dagger:
                case Weapons.Longsword:
                case Weapons.War_Axe:
                    return 5;
                case Weapons.Staff:
                    return 6;
                case Weapons.Tanto:
                    return 7;
                case Weapons.Short_Bow:
                    return 11;
                case Weapons.Long_Bow:
                    return 14;
                default:
                    return 0;
            }
        }

        public static int CalculateWeaponMinDamage(Weapons weapon)
        {
            Func<Weapons, int> del;
            if (TryGetOverride("CalculateWeaponMinDamage", out del))
                return del(weapon);

            switch (weapon)
            {
                case Weapons.Dagger:
                case Weapons.Tanto:
                case Weapons.Wakazashi:
                case Weapons.Shortsword:
                case Weapons.Broadsword:
                case Weapons.Staff:
                case Weapons.Mace:
                    return 1;
                case Weapons.Longsword:
                case Weapons.Claymore:
                case Weapons.Battle_Axe:
                case Weapons.War_Axe:
                case Weapons.Flail:
                    return 2;
                case Weapons.Saber:
                case Weapons.Katana:
                case Weapons.Dai_Katana:
                case Weapons.Warhammer:
                    return 3;
                case Weapons.Short_Bow:
                case Weapons.Long_Bow:
                    return 4;
                default:
                    return 0;
            }
        }

        public static int CalculateWeaponMaxDamage(Weapons weapon)
        {
            Func<Weapons, int> del;
            if (TryGetOverride("CalculateWeaponMaxDamage", out del))
                return del(weapon);

            switch (weapon)
            {
                case Weapons.Dagger:
                    return 6;
                case Weapons.Tanto:
                case Weapons.Shortsword:
                case Weapons.Staff:
                    return 8;
                case Weapons.Wakazashi:
                    return 10;
                case Weapons.Broadsword:
                case Weapons.Saber:
                case Weapons.Battle_Axe:
                case Weapons.Mace:
                    return 12;
                case Weapons.Flail:
                    return 14;
                case Weapons.Longsword:
                case Weapons.Katana:
                case Weapons.War_Axe:
                case Weapons.Short_Bow:
                    return 16;
                case Weapons.Claymore:
                case Weapons.Warhammer:
                case Weapons.Long_Bow:
                    return 18;
                case Weapons.Dai_Katana:
                    return 21;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Calculate the damage caused by an attack.
        /// </summary>
        /// <param name="attacker">Attacking entity</param>
        /// <param name="target">Target entity</param>
        /// <param name="enemyAnimStateRecord">Record # of the target, used for backstabbing</param>
        /// <param name="weaponAnimTime">Time the weapon animation lasted before the attack in ms, used for bow drawing </param>
        /// <param name="weapon">The weapon item being used</param>
        /// <returns>Damage inflicted to target, can be 0 for a miss or ineffective hit</returns>
        public static int CalculateAttackDamage(DaggerfallEntity attacker, DaggerfallEntity target, int enemyAnimStateRecord, int weaponAnimTime, DaggerfallUnityItem weapon, out bool shieldBlockSuccess, out int mainDamType, out bool critStrikeSuccess, out bool armorPartAbsorbed, out bool armorCompleteAbsorbed, out DaggerfallUnityItem addedAIWeapon, out bool hitSuccess, out bool metalShield, out bool metalArmor, int enemyDamType = 0)
        {
            // Pre-defininng "out" parameters, depending on how horrible this looks and such, I might consider using a "Tuple" instead of this out parameter thing.
            shieldBlockSuccess = false;     // Good
            mainDamType = 0;                // Good
            critStrikeSuccess = false;      // Good
            armorPartAbsorbed = false;      // Good
            armorCompleteAbsorbed = false;  // Good
            addedAIWeapon = null;           // Good
            hitSuccess = false;             // Good
            metalShield = false;            // Good
            metalArmor = false;             // Good

            if (attacker == null || target == null)
                return 0;

            int damageModifiers = 0;
            int damage = 0;
            int chanceToHitMod = 0;
            int backstabChance = 0;
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            short skillID = 0;
            bool specialMonsterWeapon = false;
            bool specialMonsterShield = false;
            float matResistMod = 1f;
            float damTypeResistMod = 1f;
            int playerDamType = 0;
            bool critSuccess = false;
            float critDamMulti = 1f;
            float critDamPen = 0;
            bool critIgnoreShield = false;
            EnemyBasics.CustomEnemyStatValues enemyAttackerStats = EnemyBasics.EnemyCustomAttributeInitializer(attacker);
            EnemyBasics.CustomEnemyStatValues enemyTargetStats = EnemyBasics.EnemyCustomAttributeInitializer(target);

            EnemyEntity AITarget = null;
            AITarget = target as EnemyEntity;
            EnemyEntity AIAttacker = attacker as EnemyEntity;

            if (AIAttacker != null)
            {
                // Get enemy attacker entity custom stats for later use in formula
                enemyAttackerStats = EnemyBasics.EnemyCustomAttributeCalculator(attacker);
                int atkEnemyWepSkill = enemyAttackerStats.weaponSkillCustom;
                int atkEnemyCritSkill = enemyAttackerStats.critSkillCustom;
                int atkEnemyDodSkill = enemyAttackerStats.dodgeSkillCustom;
                int atkEnemyStrength = enemyAttackerStats.strengthCustom;
                int atkEnemyAgility = enemyAttackerStats.agilityCustom;
                int atkEnemySpeed = enemyAttackerStats.speedCustom;
                int atkEnemyWillpower = enemyAttackerStats.willpowerCustom;
                int atkEnemyLuck = enemyAttackerStats.luckCustom;
            }

            if (AITarget != null)
            {
                // Get enemy target entity custom stats for later use in formula
                enemyTargetStats = EnemyBasics.EnemyCustomAttributeCalculator(target);
                int tarEnemyWepSkill = enemyTargetStats.weaponSkillCustom;
                int tarEnemyCritSkill = enemyTargetStats.critSkillCustom;
                int tarEnemyDodSkill = enemyTargetStats.dodgeSkillCustom;
                int tarEnemyStrength = enemyTargetStats.strengthCustom;
                int tarEnemyAgility = enemyTargetStats.agilityCustom;
                int tarEnemySpeed = enemyTargetStats.speedCustom;
                int tarEnemyWillpower = enemyTargetStats.willpowerCustom;
                int tarEnemyLuck = enemyTargetStats.luckCustom;
            }

            // Enemies will choose to use their weaponless attack if it is more damaging.
            if (AIAttacker != null && weapon != null)
            {
                int weaponAverage = (weapon.GetBaseDamageMin() + weapon.GetBaseDamageMax()) / 2;
                int noWeaponAverage = (AIAttacker.MobileEnemy.MinDamage + AIAttacker.MobileEnemy.MaxDamage) / 2;

                if (noWeaponAverage > weaponAverage)
                {
                    // Use hand-to-hand
                    weapon = null;
                }
            }

            if (weapon != null)
            {
                int weaponMatValue = weapon.NativeMaterialValue;

                matResistMod = DaggerfallEntity.EntityMaterialResistanceCalculator(target, weaponMatValue);

                //if (attacker == player)
                //Debug.LogFormat("1. matResistMod = {0}", matResistMod);

                // Get weapon skill used
                skillID = weapon.GetWeaponSkillIDAsShort();
            }
            else
            {
                skillID = (short)DFCareer.Skills.HandToHand;
            }

            if (attacker == player)
            {
                int playerWeaponSkill = attacker.Skills.GetLiveSkillValue(skillID);
                playerWeaponSkill = (int)Mathf.Ceil(playerWeaponSkill * 1.1f); // Makes it so player weapon skill has 110% of the effect it normally would on hit chance. So now instead of 50 weapon skill adding +50 to the end, 50 will now add +55.
                chanceToHitMod = playerWeaponSkill;
            }
            else
                chanceToHitMod = enemyAttackerStats.weaponSkillCustom;

            if (attacker == player)
            {
                if (weapon != null) // When a weapon is being used
                {
                    // Apply swing modifiers
                    ToHitAndDamageMods swingMods = CalculateSwingModifiers(GameManager.Instance.WeaponManager.ScreenWeapon, weapon);
                    damageModifiers += swingMods.damageMod;
                    chanceToHitMod += swingMods.toHitMod;
                    playerDamType = swingMods.damType;
                }
                else // When unarmed is being used
                {
                    // Apply swing modifiers
                    ToHitAndDamageMods swingMods = CalculateSwingModifiers(GameManager.Instance.WeaponManager.ScreenWeapon);
                    damageModifiers += swingMods.damageMod;
                    chanceToHitMod += swingMods.toHitMod;
                    playerDamType = swingMods.damType;
                }

                // Apply proficiency modifiers
                ToHitAndDamageMods proficiencyMods = CalculateProficiencyModifiers(attacker, weapon); // Will likely have to balance this out later.
                damageModifiers += proficiencyMods.damageMod;
                chanceToHitMod += proficiencyMods.toHitMod;

                // Apply racial bonuses
                ToHitAndDamageMods racialMods = CalculateRacialModifiers(attacker, weapon, player); // Will likely have to balance this out later.
                damageModifiers += racialMods.damageMod;
                chanceToHitMod += racialMods.toHitMod;

                backstabChance = CalculateBackstabChance(player, null, enemyAnimStateRecord);
                chanceToHitMod += backstabChance;
            }

            mainDamType = Mathf.Max(playerDamType, enemyDamType); // Takes the damage types and picks whatever what is largest, to keep it simple over rest of method.

            if (attacker == player) // Crit modifiers, if true, for the player.
            {
                critSuccess = CriticalStrikeHandler(attacker, enemyAttackerStats); // Rolls for if the attacker is sucessful with a critical strike, if yes, critSuccess is set to 'true'.

                if (critSuccess)
                {
                    critStrikeSuccess = true; // For "out" return value later.
                    if (mainDamType == 1) // Bludgeoning Crit
                    {
                        critDamMulti = (attacker.Skills.GetLiveSkillValue(DFCareer.Skills.CriticalStrike) / 4);
                        critDamMulti = (critDamMulti * .05f) + 1;
                        critIgnoreShield = true;
                        critDamPen = 0.15f;
                    }
                    else if (mainDamType == 2) // Slashing Crit
                    {
                        critDamMulti = (attacker.Skills.GetLiveSkillValue(DFCareer.Skills.CriticalStrike) / 4);
                        critDamMulti = (critDamMulti * .0625f) + 1;
                    }
                    else if (mainDamType == 3) // Piercing Crit
                    {
                        critDamMulti = (attacker.Skills.GetLiveSkillValue(DFCareer.Skills.CriticalStrike) / 4);
                        critDamMulti = (critDamMulti * .05f) + 1;
                        critDamPen = 0.40f;
                    }
                    else // Undefined Type Crit
                    {
                        critDamMulti = (attacker.Skills.GetLiveSkillValue(DFCareer.Skills.CriticalStrike) / 4);
                        critDamMulti = (critDamMulti * .05f) + 1;
                    }
                    //Debug.LogFormat("1. critDamMulti From PLAYER Skills = {0}", critDamMulti);
                    //Debug.LogFormat("2. Final critDamMulti From PLAYER Skills = {0}", critDamMulti);
                }
            }
            else // Crit modifiers, if true, for monsters/enemies.
            {
                critSuccess = CriticalStrikeHandler(attacker, enemyAttackerStats); // Rolls for if the attacker is sucessful with a critical strike, if yes, critSuccess is set to 'true'.

                if (critSuccess)
                {
                    critStrikeSuccess = true; // For "out" return value later.
                    if (mainDamType == 1) // Bludgeoning Crit
                    {
                        critDamMulti = (enemyAttackerStats.critSkillCustom / 4);
                        critDamMulti = (critDamMulti * .025f) + 1;
                        critIgnoreShield = true;
                        critDamPen = 0.10f;
                    }
                    else if (mainDamType == 2) // Slashing Crit
                    {
                        critDamMulti = (enemyAttackerStats.critSkillCustom / 4);
                        critDamMulti = (critDamMulti * .0325f) + 1;
                    }
                    else if (mainDamType == 3) // Piercing Crit
                    {
                        critDamMulti = (enemyAttackerStats.critSkillCustom / 4);
                        critDamMulti = (critDamMulti * .025f) + 1;
                        critDamPen = 0.30f;
                    }
                    else if (mainDamType == 4) // Special Crit
                    {
                        critDamMulti = (enemyAttackerStats.critSkillCustom / 4);
                        critDamMulti = (critDamMulti * .0125f) + 1;
                        critIgnoreShield = true;
                    }
                    else // Undefined Type Crit
                    {
                        critDamMulti = (enemyAttackerStats.critSkillCustom / 4);
                        critDamMulti = (critDamMulti * .025f) + 1;
                    }
                    //Debug.LogFormat("1. critDamMulti From MONSTER Skills = {0}", critDamMulti);
                    //Debug.LogFormat("2. Final critDamMulti From MONSTER Skills = {0}", critDamMulti);
                }
            }

            // Choose struck body part
            int struckBodyPart = CalculateStruckBodyPart();
            EquipSlots hitSlot = DaggerfallUnityItem.GetEquipSlotForBodyPart((BodyParts)struckBodyPart);
            DaggerfallUnityItem armor = target.ItemEquipTable.GetItem(hitSlot);
            DaggerfallUnityItem shield = target.ItemEquipTable.GetItem(EquipSlots.LeftHand);
            if (shield != null && !shield.IsShield)
                shield = null;

            if (armor != null)
                if (DaggerfallUnityItem.MaterialIdentification(armor) <= 9)
                    metalArmor = true;

            if (AITarget != null && AITarget.EntityType != EntityTypes.EnemyClass) // target is a monster
            {
                specialMonsterShield = EnemyEntity.SpecialShieldCheckForMonsters(target);

                if (specialMonsterShield)
                    shield = EnemyEntity.MonsterShieldAssign(target);
            }

            bool shieldStrongSpot = false;
            if (shield != null)
            {
                BodyParts[] protectedBodyParts = shield.GetShieldProtectedBodyParts();

                if (DaggerfallUnityItem.MaterialIdentification(shield) <= 9)
                    metalShield = true;

                for (int i = 0; (i < protectedBodyParts.Length) && !shieldStrongSpot; i++)
                {
                    if (protectedBodyParts[i] == (BodyParts)struckBodyPart)
                        shieldStrongSpot = true;
                }
                shieldBlockSuccess = ShieldBlockChanceCalculation(target, shieldStrongSpot, shield, enemyTargetStats);

                if (shieldBlockSuccess)
                    shieldBlockSuccess = CompareShieldToUnderArmor(attacker, target, mainDamType, struckBodyPart, critDamPen, shield);
            }

            if (critIgnoreShield)
                shieldBlockSuccess = false;

            damTypeResistMod = DaggerfallEntity.EntityDamageTypeResistanceCalculator(target, mainDamType, struckBodyPart, shieldBlockSuccess, armor);

            // Get damage for weaponless attacks
            if (skillID == (short)DFCareer.Skills.HandToHand)
            {
                if (attacker == player || (AIAttacker != null && AIAttacker.EntityType == EntityTypes.EnemyClass))
                {
                    hitSuccess = CalculateSuccessfulHit(attacker, target, chanceToHitMod, struckBodyPart, enemyAttackerStats, enemyTargetStats);
                    if (hitSuccess)
                    {
                        damage = CalculateHandToHandAttackDamage(attacker, target, damageModifiers, matResistMod, critDamMulti, attacker == player);

                        damage = CalculateBackstabDamage(damage, backstabChance, mainDamType);
                    }
                }
                else if (AIAttacker != null && AIAttacker.EntityType != EntityTypes.EnemyClass) // attacker is a monster
                {
                    specialMonsterWeapon = EnemyEntity.SpecialWeaponCheckForMonsters(attacker);

                    if (specialMonsterWeapon)
                    {
                        weapon = EnemyEntity.MonsterWeaponAssign(attacker);
                        addedAIWeapon = weapon; // For "out" return value later.
                    }

                    // Handle attacks by AI
                    int minBaseDamage = AIAttacker.MobileEnemy.MinDamage;
                    int maxBaseDamage = AIAttacker.MobileEnemy.MaxDamage;

                    hitSuccess = CalculateSuccessfulHit(attacker, target, chanceToHitMod, struckBodyPart, enemyAttackerStats, enemyTargetStats);
                    if (hitSuccess)
                    {
                        int hitDamage = UnityEngine.Random.Range(minBaseDamage, maxBaseDamage + 1);
                        // Apply special monster attack effects
                        if (hitDamage > 0)
                            FormulaHelper.OnMonsterHit(AIAttacker, target, hitDamage); // I'm very likely going to mess with and tweek around with this later. Could add a few diseases, one I thought of was Tetinus which would be potentially contracted from iron golems and enemies using iron weapons with lower condition maybe. Rabies as well perhaps. Also see how the disease contraction works in general, if it does not already, probably make endurance have a chance of resisting a successful attempt or something.

                        damage += hitDamage;
                    }

                    if (damage >= 1)
                        damage = CalculateHandToHandAttackDamage(attacker, target, damage, matResistMod, critDamMulti, attacker == player); // Added my own, non-overriden version of this method for modification.
                }
            }
            // Handle weapon attacks
            else if (weapon != null)
            {
                //chanceToHitMod += CalculateWeaponToHit(weapon); // Apply weapon material modifier. I'm removing this for now, possibly forever, don't like how it works really.

                // Mod hook for adjusting final hit chance mod. (is a no-op in DFU)
                chanceToHitMod = AdjustWeaponHitChanceMod(attacker, target, chanceToHitMod, weaponAnimTime, weapon); // Does Nothing

                hitSuccess = CalculateSuccessfulHit(attacker, target, chanceToHitMod, struckBodyPart, enemyAttackerStats, enemyTargetStats);
                if (hitSuccess)
                {
                    damage = CalculateWeaponAttackDamage(attacker, target, damageModifiers, weaponAnimTime, weapon, mainDamType, matResistMod, critDamMulti, damTypeResistMod, enemyAttackerStats);

                    damage = CalculateBackstabDamage(damage, backstabChance, mainDamType, weapon);
                }

                // Handle poisoned weapons
                if (damage > 0 && weapon.poisonType != Poisons.None)
                {
                    FormulaHelper.InflictPoison(target, weapon.poisonType, false); // Will likely play around with this later on.
                    weapon.poisonType = Poisons.None;
                }
            }

            damage = Mathf.Max(0, damage); // I think this is just here to keep damage from outputting a negative value.

            if (damage < 1) // Cut off the execution if the damage is still not anything higher than 1 at this point in the method.
                return 0;

            if (attacker == player)
            {
                if (matResistMod <= 0.5f) // Probably add a book to the game that describes these resistances and weaknesses of monsters. Maybe title "Know Thy Enemy" or something.
                    DaggerfallUI.AddHUDText("This Weapon Is Not Very Effective Against This Creature.", 1.00f);
            }

            //Debug.Log("------------------------------------------------------------------------------------------");
            //Debug.LogFormat("Here is damage value before armor reduction is applied = {0}", damage);
            int damBefore = damage; // Something i'll likely want to implement later, that being taking enemy damage resistances into account for the damage reduction for calculating damage done to attacker weapon. Currently only worn armor is taken into consideration here in that regard.

            damage = CalculateArmorDamageReduction(attacker, target, damage, mainDamType, struckBodyPart, shieldBlockSuccess, critDamPen, weapon, shield);

            int damAfter = damage;
            //Debug.LogFormat("Here is damage value after armor reduction = {0}", damage);
            if (damBefore > 0)
            {
                int damReduPercent = ((100 * damAfter / damBefore) - 100) * -1;
                //Debug.LogFormat("Here is damage reduction percent = {0}%", damReduPercent);
                if (damReduPercent > 0 && damAfter == 0)
                    armorCompleteAbsorbed = true;   // For "out" return value later.
                else if (damReduPercent >= 35)
                    armorPartAbsorbed = true;       // For "out" return value later.
            }
            //Debug.Log("------------------------------------------------------------------------------------------");

            DamageEquipment(attacker, target, struckBodyPart, shieldBlockSuccess, mainDamType, damBefore, damAfter, enemyAttackerStats, weapon);

            if (damage < 1) // Cut off the execution if the damage is still not anything higher than 1 at this point in the method.
                return 0;

            // Apply Ring of Namira effect
            if (target == player)
            {
                DaggerfallUnityItem[] equippedItems = target.ItemEquipTable.EquipTable;
                DaggerfallUnityItem item = null;
                if (equippedItems.Length != 0)
                {
                    if (IsRingOfNamira(equippedItems[(int)EquipSlots.Ring0]) || IsRingOfNamira(equippedItems[(int)EquipSlots.Ring1]))
                    {
                        IEntityEffect effectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(RingOfNamiraEffect.EffectKey);
                        effectTemplate.EnchantmentPayloadCallback(EnchantmentPayloadFlags.None,
                            targetEntity: AIAttacker.EntityBehaviour,
                            sourceItem: item,
                            sourceDamage: damage);
                    }
                }
            }

            //Debug.LogFormat("Damage {0} applied, animTime={1}  ({2})", damage, weaponAnimTime, GameManager.Instance.WeaponManager.ScreenWeapon.WeaponState);

            return damage;
        }

        private static bool IsRingOfNamira(DaggerfallUnityItem item)
        {
            return item != null && item.ContainsEnchantment(DaggerfallConnect.FallExe.EnchantmentTypes.SpecialArtifactEffect, (int)ArtifactsSubTypes.Ring_of_Namira);
        }

        public static int CalculateWeaponAttackDamage(DaggerfallEntity attacker, DaggerfallEntity target, int damageModifier, int weaponAnimTime, DaggerfallUnityItem weapon, int mainDamType, float matResistMod, float critDamMulti, float damTypeResistMod, EnemyBasics.CustomEnemyStatValues enemyAttackerStats)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            int damage = 0;
            float conditionMulti = 1f;
            int minDamLowerLimit = CalculateWeaponMinDamTypeLowerLimit(weapon, mainDamType);
            int minDamUpperLimit = CalculateWeaponMinDamTypeUpperLimit(weapon, mainDamType);
            int maxDamLowerLimit = CalculateWeaponMaxDamTypeLowerLimit(weapon, mainDamType);
            int maxDamUpperLimit = CalculateWeaponMaxDamTypeUpperLimit(weapon, mainDamType);

            if (attacker == GameManager.Instance.PlayerEntity) // Only the player has weapon damage effected by condition value.
            {
                conditionMulti = AlterDamageBasedOnWepCondition(weapon, mainDamType);
                //Debug.LogFormat("Damage Multiplier Due To Weapon Condition = {0}", conditionMulti);
            }

            if (mainDamType == 1)
                damage = UnityEngine.Random.Range(Mathf.Clamp((int)Mathf.Round((weapon.GetBaseBludgeoningDamageMin() + weapon.GetWeaponMaterialModDensity()) * conditionMulti * damTypeResistMod), minDamLowerLimit, minDamUpperLimit), Mathf.Clamp((int)Mathf.Round((weapon.GetBaseBludgeoningDamageMax() + weapon.GetWeaponMaterialModDensity()) * conditionMulti * damTypeResistMod), maxDamLowerLimit, maxDamUpperLimit)) + damageModifier;
            else if (mainDamType == 2)
                damage = UnityEngine.Random.Range(Mathf.Clamp((int)Mathf.Round((weapon.GetBaseSlashingDamageMin() + weapon.GetWeaponMaterialModShear()) * conditionMulti * damTypeResistMod), minDamLowerLimit, minDamUpperLimit), Mathf.Clamp((int)Mathf.Round((weapon.GetBaseSlashingDamageMax() + weapon.GetWeaponMaterialModShear()) * conditionMulti * damTypeResistMod), maxDamLowerLimit, maxDamUpperLimit)) + damageModifier;
            else if (mainDamType == 3)
                damage = UnityEngine.Random.Range(Mathf.Clamp((int)Mathf.Round((weapon.GetBasePiercingDamageMin() + weapon.GetWeaponMaterialModFracture()) * conditionMulti * damTypeResistMod), minDamLowerLimit, minDamUpperLimit), Mathf.Clamp((int)Mathf.Round((weapon.GetBasePiercingDamageMax() + weapon.GetWeaponMaterialModFracture()) * conditionMulti * damTypeResistMod), maxDamLowerLimit, maxDamUpperLimit)) + damageModifier;
            else
                damage = UnityEngine.Random.Range(weapon.GetBaseDamageMin(), weapon.GetBaseDamageMax() + 1) + damageModifier;

            // Apply strength modifier
            if (attacker == player)
            {
                if (ItemEquipTable.GetItemHands(weapon) == ItemHands.Both && weapon.TemplateIndex != (int)Weapons.Short_Bow && weapon.TemplateIndex != (int)Weapons.Long_Bow)
                    damage += (DamageModifier(attacker.Stats.LiveStrength)) * 2; // Multiplying by 2, so that two-handed weapons gets double the damage mod from Strength, except bows.
                else
                    damage += DamageModifier(attacker.Stats.LiveStrength);
            }
            else
            {
                if (ItemEquipTable.GetItemHands(weapon) == ItemHands.Both && weapon.TemplateIndex != (int)Weapons.Short_Bow && weapon.TemplateIndex != (int)Weapons.Long_Bow)
                    damage += (DamageModifier(enemyAttackerStats.strengthCustom)) * 2; // Multiplying by 2, so that two-handed weapons gets double the damage mod from Strength, except bows.
                else
                    damage += DamageModifier(enemyAttackerStats.strengthCustom);
            }

            if (damage < 1)
                damage = 0;

            if (damage >= 1)
                damage += GetBonusOrPenaltyByEnemyType(attacker, target);

            damage = (int)Mathf.Round(damage * critDamMulti);
            damage = (int)Mathf.Round(damage * matResistMod);
            if (damage < 1)
                damage = 0;

            // Mod hook for adjusting final damage. (is a no-op in DFU)
            damage = AdjustWeaponAttackDamage(attacker, target, damage, weaponAnimTime, weapon);

            return damage;
        }

        public static int CalculateHandToHandAttackDamage(DaggerfallEntity attacker, DaggerfallEntity target, int damageModifier, float matResistMod, float critDamMulti, bool player)
        {
            int damage = 0;

            if (player)
            {
                int minBaseDamage = FormulaHelper.CalculateHandToHandMinDamage(attacker.Skills.GetLiveSkillValue(DFCareer.Skills.HandToHand));
                int maxBaseDamage = FormulaHelper.CalculateHandToHandMaxDamage(attacker.Skills.GetLiveSkillValue(DFCareer.Skills.HandToHand));
                damage = UnityEngine.Random.Range(minBaseDamage, maxBaseDamage + 1);

                // Apply damage modifiers.
                damage += damageModifier;

                // Apply strength modifier for players. It is not applied in classic despite what the in-game description for the Strength attribute says.
                damage += (DamageModifier(attacker.Stats.LiveStrength)) * 2;
            }
            else
                damage += damageModifier;

            if (damage < 1)
                damage = 0;

            if (damage >= 1)
                damage += GetBonusOrPenaltyByEnemyType(attacker, target);

            damage = (int)Mathf.Round(damage * critDamMulti);
            damage = (int)Mathf.Round(damage * matResistMod);
            if (damage < 1)
                damage = 0;

            return damage;
        }

        /// Calculates whether an attack on a target is successful or not.
		private static bool CalculateSuccessfulHit(DaggerfallEntity attacker, DaggerfallEntity target, int chanceToHitMod, int struckBodyPart, EnemyBasics.CustomEnemyStatValues enemyAttackerStats, EnemyBasics.CustomEnemyStatValues enemyTargetStats)
		{
			PlayerEntity player = GameManager.Instance.PlayerEntity;
			
			if (attacker == null || target == null)
                return false;

            int chanceToHit = chanceToHitMod;
			//Debug.LogFormat("Starting chanceToHitMod = {0}", chanceToHit);
			
			// Get armor value for struck body part
            chanceToHit += CalculateArmorToHit(target, struckBodyPart);
			
			// Apply adrenaline rush modifiers.
            chanceToHit += CalculateAdrenalineRushToHit(attacker, target);

            // Apply enchantment modifier. 
            chanceToHit += attacker.ChanceToHitModifier;
			//Debug.LogFormat("Attacker Chance To Hit Mod 'Enchantment' = {0}", attacker.ChanceToHitModifier); // No idea what this does, always seeing 0.
			
			// Apply stat differential modifiers. (default: luck and agility)
			chanceToHit += CalculateStatDiffsToHit(attacker, target, enemyAttackerStats, enemyTargetStats);
			
			// Apply skill modifiers. (default: dodge and crit strike)
            chanceToHit += CalculateSkillsToHit(attacker, target, enemyTargetStats);
			//Debug.LogFormat("After Dodge = {0}", chanceToHitMod);
			
			// Apply monster modifier and biography adjustments.
            chanceToHit += CalculateAdjustmentsToHit(attacker, target);
			//Debug.LogFormat("Final chanceToHitMod = {0}", chanceToHitMod);

            Mathf.Clamp(chanceToHit, 3, 97);

            return Dice100.SuccessRoll(chanceToHit);
        }

        public static float GetMeleeWeaponAnimTime(PlayerEntity player, WeaponTypes weaponType, ItemHands weaponHands)
        {
            Func<PlayerEntity, WeaponTypes, ItemHands, float> del;
            if (TryGetOverride("GetMeleeWeaponAnimTime", out del))
                return del(player, weaponType, weaponHands);

            float speed = 3 * (115 - player.Stats.LiveSpeed);
            return speed / classicFrameUpdate;
        }

        public static float GetBowCooldownTime(PlayerEntity player)
        {
            Func<PlayerEntity, float> del;
            if (TryGetOverride("GetBowCooldownTime", out del))
                return del(player);

            float cooldown = 10 * (100 - player.Stats.LiveSpeed) + 800;
            return cooldown / classicFrameUpdate;
        }

        #endregion

        #region Combat & Damage: component sub-formula

        public static int CalculateStruckBodyPart()
        {
            Func<int> del;
            if (TryGetOverride("CalculateStruckBodyPart", out del))
                return del();

            int[] bodyParts = { 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5, 5, 5, 6, 6, 6 };
            return bodyParts[UnityEngine.Random.Range(0, bodyParts.Length)];
        }

        public static ToHitAndDamageMods CalculateSwingModifiers(FPSWeapon onscreenWeapon, DaggerfallUnityItem weapon = null)
        {
            Func<FPSWeapon, ToHitAndDamageMods> del;
            if (TryGetOverride("CalculateSwingModifiers", out del))
                return del(onscreenWeapon);

            ToHitAndDamageMods mods = new ToHitAndDamageMods();
            mods.damageMod = 0;
            mods.toHitMod = 0;
            mods.damType = 0;

            if (weapon != null) // With Weapon
            {
                if (onscreenWeapon != null)
                {
                    switch (weapon.TemplateIndex)
                    {
                        case (int)Weapons.Dagger:
                        case (int)Weapons.Tanto:
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                            {
                                mods.damageMod = 0;
                                mods.toHitMod = 0;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                            {
                                mods.damageMod = 3;
                                mods.toHitMod = -10;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                            {
                                mods.damageMod = -1;
                                mods.toHitMod = 5;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft)
                            {
                                mods.damageMod = 1;
                                mods.toHitMod = -5;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                            {
                                mods.damageMod = -1;
                                mods.toHitMod = 5;
                                mods.damType = 1;
                                break;
                            }
                            break;
                        case (int)Weapons.Shortsword:
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                            {
                                mods.damageMod = 0;
                                mods.toHitMod = 0;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                            {
                                mods.damageMod = 2;
                                mods.toHitMod = -10;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                            {
                                mods.damageMod = 0;
                                mods.toHitMod = 5;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft)
                            {
                                mods.damageMod = 1;
                                mods.toHitMod = 0;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                            {
                                mods.damageMod = 0;
                                mods.toHitMod = 5;
                                mods.damType = 1;
                                break;
                            }
                            break;
                        case (int)Weapons.Wakazashi:
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                            {
                                mods.damageMod = -1;
                                mods.toHitMod = 0;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                            {
                                mods.damageMod = 3;
                                mods.toHitMod = -10;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                            {
                                mods.damageMod = 1;
                                mods.toHitMod = 0;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft)
                            {
                                mods.damageMod = 2;
                                mods.toHitMod = -5;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                            {
                                mods.damageMod = -1;
                                mods.toHitMod = 5;
                                mods.damType = 1;
                                break;
                            }
                            break;
                        case (int)Weapons.Broadsword:
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                            {
                                mods.damageMod = -1;
                                mods.toHitMod = 5;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                            {
                                mods.damageMod = 3;
                                mods.toHitMod = -10;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                            {
                                mods.damageMod = 1;
                                mods.toHitMod = 0;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft)
                            {
                                mods.damageMod = 2;
                                mods.toHitMod = -5;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                            {
                                mods.damageMod = 2;
                                mods.toHitMod = 0;
                                mods.damType = 1;
                                break;
                            }
                            break;
                        case (int)Weapons.Claymore:
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                            {
                                mods.damageMod = 0;
                                mods.toHitMod = 5;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                            {
                                mods.damageMod = 4;
                                mods.toHitMod = -10;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                            {
                                mods.damageMod = 2;
                                mods.toHitMod = 0;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft)
                            {
                                mods.damageMod = 3;
                                mods.toHitMod = -5;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                            {
                                mods.damageMod = 2;
                                mods.toHitMod = 5;
                                mods.damType = 1;
                                break;
                            }
                            break;
                        case (int)Weapons.Dai_Katana:
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                            {
                                mods.damageMod = -1;
                                mods.toHitMod = 0;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                            {
                                mods.damageMod = 5;
                                mods.toHitMod = -10;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                            {
                                mods.damageMod = 3;
                                mods.toHitMod = -5;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft)
                            {
                                mods.damageMod = 2;
                                mods.toHitMod = 0;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                            {
                                mods.damageMod = -1;
                                mods.toHitMod = 5;
                                mods.damType = 1;
                                break;
                            }
                            break;
                        case (int)Weapons.Katana:
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                            {
                                mods.damageMod = -1;
                                mods.toHitMod = 0;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                            {
                                mods.damageMod = 4;
                                mods.toHitMod = -10;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                            {
                                mods.damageMod = 1;
                                mods.toHitMod = 0;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft)
                            {
                                mods.damageMod = 2;
                                mods.toHitMod = -5;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                            {
                                mods.damageMod = -1;
                                mods.toHitMod = 5;
                                mods.damType = 1;
                                break;
                            }
                            break;
                        case (int)Weapons.Longsword:
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                            {
                                mods.damageMod = -1;
                                mods.toHitMod = 0;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                            {
                                mods.damageMod = 2;
                                mods.toHitMod = -10;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                            {
                                mods.damageMod = 0;
                                mods.toHitMod = 5;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft)
                            {
                                mods.damageMod = 1;
                                mods.toHitMod = -5;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                            {
                                mods.damageMod = 0;
                                mods.toHitMod = 5;
                                mods.damType = 1;
                                break;
                            }
                            break;
                        case (int)Weapons.Saber:
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                            {
                                mods.damageMod = -1;
                                mods.toHitMod = 0;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                            {
                                mods.damageMod = 3;
                                mods.toHitMod = -10;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                            {
                                mods.damageMod = 1;
                                mods.toHitMod = 5;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft)
                            {
                                mods.damageMod = 2;
                                mods.toHitMod = 0;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                            {
                                mods.damageMod = 2;
                                mods.toHitMod = 0;
                                mods.damType = 1;
                                break;
                            }
                            break;
                        case (int)Weapons.Battle_Axe:
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                            {
                                mods.damageMod = 1;
                                mods.toHitMod = 0;
                                mods.damType = 1;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                            {
                                mods.damageMod = 4;
                                mods.toHitMod = -10;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                            {
                                mods.damageMod = 1;
                                mods.toHitMod = 0;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft)
                            {
                                mods.damageMod = 3;
                                mods.toHitMod = -5;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                            {
                                mods.damageMod = 0;
                                mods.toHitMod = 5;
                                mods.damType = 1;
                                break;
                            }
                            break;
                        case (int)Weapons.War_Axe:
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                            {
                                mods.damageMod = 0;
                                mods.toHitMod = 5;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                            {
                                mods.damageMod = 5;
                                mods.toHitMod = -10;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                            {
                                mods.damageMod = 2;
                                mods.toHitMod = 0;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft)
                            {
                                mods.damageMod = 3;
                                mods.toHitMod = -5;
                                mods.damType = 2;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                            {
                                mods.damageMod = 0;
                                mods.toHitMod = -5;
                                mods.damType = 1;
                                break;
                            }
                            break;
                        case (int)Weapons.Flail:
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                            {
                                mods.damageMod = 1;
                                mods.toHitMod = 5;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                            {
                                mods.damageMod = 5;
                                mods.toHitMod = -10;
                                mods.damType = 1;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                            {
                                mods.damageMod = 2;
                                mods.toHitMod = 0;
                                mods.damType = 1;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                            {
                                mods.damageMod = 3;
                                mods.toHitMod = -5;
                                mods.damType = 1;
                                break;
                            }
                            break;
                        case (int)Weapons.Mace:
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                            {
                                mods.damageMod = 1;
                                mods.toHitMod = 5;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                            {
                                mods.damageMod = 4;
                                mods.toHitMod = -10;
                                mods.damType = 1;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                            {
                                mods.damageMod = 1;
                                mods.toHitMod = 0;
                                mods.damType = 1;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                            {
                                mods.damageMod = 3;
                                mods.toHitMod = -5;
                                mods.damType = 1;
                                break;
                            }
                            break;
                        case (int)Weapons.Staff:
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                            {
                                mods.damageMod = 1;
                                mods.toHitMod = 10;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                            {
                                mods.damageMod = 2;
                                mods.toHitMod = -10;
                                mods.damType = 1;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                            {
                                mods.damageMod = 0;
                                mods.toHitMod = 0;
                                mods.damType = 1;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                            {
                                mods.damageMod = 1;
                                mods.toHitMod = -5;
                                mods.damType = 1;
                                break;
                            }
                            break;
                        case (int)Weapons.Warhammer:
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                            {
                                mods.damageMod = 0;
                                mods.toHitMod = 5;
                                mods.damType = 3;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                            {
                                mods.damageMod = 5;
                                mods.toHitMod = -10;
                                mods.damType = 1;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                            {
                                mods.damageMod = 3;
                                mods.toHitMod = 0;
                                mods.damType = 1;
                                break;
                            }
                            if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                            {
                                mods.damageMod = 4;
                                mods.toHitMod = -5;
                                mods.damType = 1;
                                break;
                            }
                            break;
                        case (int)Weapons.Long_Bow:
                        case (int)Weapons.Short_Bow:
                            mods.damageMod = 0;
                            mods.toHitMod = 0;
                            mods.damType = 3;
                            break;
                        default:
                            mods.damageMod = 0;
                            mods.toHitMod = 0;
                            mods.damType = 0;
                            break;
                    }
                    return mods;
                }
                else
                    return mods;
            }
            else // Unarmed, I'll return to this later, right now this is all mostly just placeholder values and such, not final product at all, need more context for overall final image.
            {
                if (onscreenWeapon != null)
                {
                    if (onscreenWeapon.WeaponState == WeaponStates.StrikeUp)
                    {
                        mods.damageMod = 0;
                        mods.toHitMod = 10;
                        mods.damType = 1;
                    }
                    if (onscreenWeapon.WeaponState == WeaponStates.StrikeDown)
                    {
                        mods.damageMod = 3;
                        mods.toHitMod = -10;
                        mods.damType = 1;
                    }
                    if (onscreenWeapon.WeaponState == WeaponStates.StrikeLeft || onscreenWeapon.WeaponState == WeaponStates.StrikeRight)
                    {
                        mods.damageMod = 1;
                        mods.toHitMod = 0;
                        mods.damType = 1;
                    }
                    if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownLeft)
                    {
                        mods.damageMod = 2;
                        mods.toHitMod = -5;
                        mods.damType = 1;
                    }
                    if (onscreenWeapon.WeaponState == WeaponStates.StrikeDownRight)
                    {
                        mods.damageMod = -1;
                        mods.toHitMod = 5;
                        mods.damType = 2;
                    }
                    return mods;
                }
                else
                {
                    mods.damageMod = 0;
                    mods.toHitMod = 0;
                    mods.damType = 0;
                    return mods;
                }
            }
        }

        public static int CalculateWeaponMinDamTypeLowerLimit(DaggerfallUnityItem weapon, int damType)
        {
            if (damType == 1)
            {
                switch (weapon.TemplateIndex)
                {
                    case (int)Weapons.Dagger:
                    case (int)Weapons.Shortsword:
                    case (int)Weapons.Longsword:
                        return 1;
                    case (int)Weapons.Broadsword:
                    case (int)Weapons.Claymore:
                    case (int)Weapons.Battle_Axe:
                    case (int)Weapons.War_Axe:
                        return 2;
                    case (int)Weapons.Saber:
                    case (int)Weapons.Staff:
                        return 3;
                    case (int)Weapons.Mace:
                    case (int)Weapons.Flail:
                    case (int)Weapons.Warhammer:
                        return 4;
                    default:
                        return 0;
                }
            }
            else if (damType == 2)
            {
                switch (weapon.TemplateIndex)
                {
                    case (int)Weapons.Dagger:
                    case (int)Weapons.Tanto:
                        return 1;
                    case (int)Weapons.Shortsword:
                    case (int)Weapons.Wakazashi:
                    case (int)Weapons.Longsword:
                        return 2;
                    case (int)Weapons.Broadsword:
                    case (int)Weapons.Claymore:
                    case (int)Weapons.Saber:
                    case (int)Weapons.Battle_Axe:
                    case (int)Weapons.War_Axe:
                        return 3;
                    case (int)Weapons.Katana:
                        return 4;
                    case (int)Weapons.Dai_Katana:
                        return 5;
                    default:
                        return 0;
                }
            }
            else if (damType == 3)
            {
                switch (weapon.TemplateIndex)
                {
                    case (int)Weapons.Shortsword:
                    case (int)Weapons.Wakazashi:
                    case (int)Weapons.Broadsword:
                    case (int)Weapons.Claymore:
                    case (int)Weapons.Saber:
                    case (int)Weapons.Flail:
                    case (int)Weapons.Mace:
                    case (int)Weapons.Staff:
                        return 1;
                    case (int)Weapons.Katana:
                    case (int)Weapons.Dai_Katana:
                    case (int)Weapons.War_Axe:
                    case (int)Weapons.Warhammer:
                        return 2;
                    case (int)Weapons.Dagger:
                    case (int)Weapons.Longsword:
                        return 3;
                    case (int)Weapons.Tanto:
                        return 4;
                    case (int)Weapons.Long_Bow:
                    case (int)Weapons.Short_Bow:
                        return 5;
                    default:
                        return 0;
                }
            }
            else
                return 0;
        }

        public static int CalculateWeaponMinDamTypeUpperLimit(DaggerfallUnityItem weapon, int damType)
        {
            if (damType == 1)
            {
                switch (weapon.TemplateIndex)
                {
                    case (int)Weapons.Tanto:
                    case (int)Weapons.Wakazashi:
                    case (int)Weapons.Dai_Katana:
                    case (int)Weapons.Katana:
                        return 2;
                    case (int)Weapons.Dagger:
                    case (int)Weapons.Shortsword:
                    case (int)Weapons.Longsword:
                        return 3;
                    case (int)Weapons.Broadsword:
                    case (int)Weapons.Claymore:
                    case (int)Weapons.Battle_Axe:
                    case (int)Weapons.War_Axe:
                        return 4;
                    case (int)Weapons.Saber:
                    case (int)Weapons.Staff:
                        return 5;
                    case (int)Weapons.Mace:
                    case (int)Weapons.Flail:
                    case (int)Weapons.Warhammer:
                        return 6;
                    default:
                        return 0;
                }
            }
            else if (damType == 2)
            {
                switch (weapon.TemplateIndex)
                {
                    case (int)Weapons.Tanto:
                        return 3;
                    case (int)Weapons.Dagger:
                    case (int)Weapons.Shortsword:
                        return 4;
                    case (int)Weapons.Wakazashi:
                    case (int)Weapons.Longsword:
                        return 5;
                    case (int)Weapons.Broadsword:
                    case (int)Weapons.Claymore:
                    case (int)Weapons.Saber:
                    case (int)Weapons.Battle_Axe:
                    case (int)Weapons.War_Axe:
                        return 6;
                    case (int)Weapons.Katana:
                        return 7;
                    case (int)Weapons.Dai_Katana:
                        return 8;
                    default:
                        return 0;
                }
            }
            else if (damType == 3)
            {
                switch (weapon.TemplateIndex)
                {
                    case (int)Weapons.Broadsword:
                    case (int)Weapons.Claymore:
                    case (int)Weapons.Saber:
                    case (int)Weapons.Flail:
                    case (int)Weapons.Mace:
                    case (int)Weapons.Staff:
                        return 2;
                    case (int)Weapons.Shortsword:
                    case (int)Weapons.Wakazashi:
                        return 3;
                    case (int)Weapons.Katana:
                    case (int)Weapons.Dai_Katana:
                    case (int)Weapons.War_Axe:
                    case (int)Weapons.Warhammer:
                        return 4;
                    case (int)Weapons.Longsword:
                        return 5;
                    case (int)Weapons.Dagger:
                        return 6;
                    case (int)Weapons.Tanto:
                        return 7;
                    case (int)Weapons.Long_Bow:
                    case (int)Weapons.Short_Bow:
                        return 8;
                    default:
                        return 0;
                }
            }
            else
                return 0;
        }

        public static int CalculateWeaponMaxDamTypeLowerLimit(DaggerfallUnityItem weapon, int damType)
        {
            if (damType == 1)
            {
                switch (weapon.TemplateIndex)
                {
                    case (int)Weapons.Tanto:
                    case (int)Weapons.Wakazashi:
                    case (int)Weapons.Dai_Katana:
                    case (int)Weapons.Katana:
                        return 2;
                    case (int)Weapons.Dagger:
                    case (int)Weapons.Shortsword:
                    case (int)Weapons.Longsword:
                        return 3;
                    case (int)Weapons.Broadsword:
                    case (int)Weapons.Claymore:
                    case (int)Weapons.Battle_Axe:
                    case (int)Weapons.War_Axe:
                        return 4;
                    case (int)Weapons.Saber:
                    case (int)Weapons.Staff:
                        return 5;
                    case (int)Weapons.Mace:
                    case (int)Weapons.Flail:
                    case (int)Weapons.Warhammer:
                        return 6;
                    default:
                        return 0;
                }
            }
            else if (damType == 2)
            {
                switch (weapon.TemplateIndex)
                {
                    case (int)Weapons.Tanto:
                        return 3;
                    case (int)Weapons.Dagger:
                    case (int)Weapons.Shortsword:
                        return 4;
                    case (int)Weapons.Wakazashi:
                    case (int)Weapons.Longsword:
                        return 5;
                    case (int)Weapons.Broadsword:
                    case (int)Weapons.Claymore:
                    case (int)Weapons.Saber:
                    case (int)Weapons.Battle_Axe:
                    case (int)Weapons.War_Axe:
                        return 6;
                    case (int)Weapons.Katana:
                        return 7;
                    case (int)Weapons.Dai_Katana:
                        return 8;
                    default:
                        return 0;
                }
            }
            else if (damType == 3)
            {
                switch (weapon.TemplateIndex)
                {
                    case (int)Weapons.Broadsword:
                    case (int)Weapons.Claymore:
                    case (int)Weapons.Saber:
                    case (int)Weapons.Flail:
                    case (int)Weapons.Mace:
                    case (int)Weapons.Staff:
                        return 2;
                    case (int)Weapons.Shortsword:
                    case (int)Weapons.Wakazashi:
                        return 3;
                    case (int)Weapons.Katana:
                    case (int)Weapons.Dai_Katana:
                    case (int)Weapons.War_Axe:
                    case (int)Weapons.Warhammer:
                        return 4;
                    case (int)Weapons.Longsword:
                        return 5;
                    case (int)Weapons.Dagger:
                        return 6;
                    case (int)Weapons.Tanto:
                        return 7;
                    case (int)Weapons.Long_Bow:
                    case (int)Weapons.Short_Bow:
                        return 8;
                    default:
                        return 0;
                }
            }
            else
                return 0;
        }

        public static int CalculateWeaponMaxDamTypeUpperLimit(DaggerfallUnityItem weapon, int damType)
        {
            if (damType == 1)
            {
                switch (weapon.TemplateIndex)
                {
                    case (int)Weapons.Tanto:
                    case (int)Weapons.Wakazashi:
                    case (int)Weapons.Dai_Katana:
                    case (int)Weapons.Katana:
                        return 4;
                    case (int)Weapons.Dagger:
                    case (int)Weapons.Shortsword:
                    case (int)Weapons.Longsword:
                        return 5;
                    case (int)Weapons.Broadsword:
                        return 6;
                    case (int)Weapons.Claymore:
                    case (int)Weapons.Saber:
                    case (int)Weapons.Battle_Axe:
                    case (int)Weapons.War_Axe:
                        return 7;
                    case (int)Weapons.Staff:
                    case (int)Weapons.Mace:
                    case (int)Weapons.Flail:
                    case (int)Weapons.Warhammer:
                        return 60;
                    default:
                        return 0;
                }
            }
            else if (damType == 2)
            {
                switch (weapon.TemplateIndex)
                {
                    case (int)Weapons.Tanto:
                        return 6;
                    case (int)Weapons.Dagger:
                        return 8;
                    case (int)Weapons.Shortsword:
                    case (int)Weapons.Wakazashi:
                    case (int)Weapons.Broadsword:
                    case (int)Weapons.Longsword:
                    case (int)Weapons.Claymore:
                    case (int)Weapons.Saber:
                    case (int)Weapons.Battle_Axe:
                    case (int)Weapons.War_Axe:
                    case (int)Weapons.Katana:
                    case (int)Weapons.Dai_Katana:
                        return 60;
                    default:
                        return 0;
                }
            }
            else if (damType == 3)
            {
                switch (weapon.TemplateIndex)
                {
                    case (int)Weapons.Broadsword:
                    case (int)Weapons.Claymore:
                    case (int)Weapons.Saber:
                    case (int)Weapons.Flail:
                    case (int)Weapons.Mace:
                    case (int)Weapons.Staff:
                        return 7;
                    case (int)Weapons.Wakazashi:
                    case (int)Weapons.Katana:
                    case (int)Weapons.Dai_Katana:
                        return 8;
                    case (int)Weapons.War_Axe:
                    case (int)Weapons.Warhammer:
                        return 9;
                    case (int)Weapons.Shortsword:
                    case (int)Weapons.Dagger:
                    case (int)Weapons.Longsword:
                    case (int)Weapons.Tanto:
                    case (int)Weapons.Long_Bow:
                    case (int)Weapons.Short_Bow:
                        return 60;
                    default:
                        return 0;
                }
            }
            else
                return 0;
        }

        public static ToHitAndDamageMods CalculateProficiencyModifiers(DaggerfallEntity attacker, DaggerfallUnityItem weapon)
        {
            ToHitAndDamageMods mods = new ToHitAndDamageMods(); // If I feel that 50 starting points is too much for a level 1 character, I could always make the benefits only start past that 50 mark or something, maybe 40.
            if (weapon != null)
            {
                // Apply weapon proficiency
                if (((int)attacker.Career.ExpertProficiencies & weapon.GetWeaponSkillUsed()) != 0)
                {
                    switch (weapon.GetWeaponSkillIDAsShort())
                    {
                        case (short)DFCareer.Skills.Archery:
                            mods.damageMod = (attacker.Stats.LiveStrength / 33) + (attacker.Stats.LiveAgility / 33) + 1; //7
                            mods.toHitMod = (attacker.Stats.LiveAgility / 7) + (attacker.Stats.LiveSpeed / 20) + (attacker.Stats.LiveLuck / 10); //29
                            break;
                        case (short)DFCareer.Skills.Axe:
                            mods.damageMod = (attacker.Stats.LiveStrength / 33) + (attacker.Stats.LiveAgility / 33) + 1; //7
                            mods.toHitMod = (attacker.Stats.LiveStrength / 9) + (attacker.Stats.LiveAgility / 9) + (attacker.Stats.LiveLuck / 14); //29
                            break;
                        case (short)DFCareer.Skills.BluntWeapon:
                            mods.damageMod = (attacker.Stats.LiveStrength / 33) + (attacker.Stats.LiveEndurance / 33) + 1; //7
                            mods.toHitMod = (attacker.Stats.LiveStrength / 7) + (attacker.Stats.LiveAgility / 12) + (attacker.Stats.LiveLuck / 15); //29
                            break;
                        case (short)DFCareer.Skills.LongBlade:
                            mods.damageMod = (attacker.Stats.LiveAgility / 33) + (attacker.Stats.LiveStrength / 33) + 1; //7
                            mods.toHitMod = (attacker.Stats.LiveAgility / 7) + (attacker.Stats.LiveSpeed / 12) + (attacker.Stats.LiveLuck / 15); //29
                            break;
                        case (short)DFCareer.Skills.ShortBlade:
                            mods.damageMod = (attacker.Stats.LiveAgility / 33) + (attacker.Stats.LiveSpeed / 33) + 1; //7
                            mods.toHitMod = (attacker.Stats.LiveAgility / 7) + (attacker.Stats.LiveSpeed / 12) + (attacker.Stats.LiveLuck / 15); //29
                            break;
                        default:
                            break;
                    }
                }
            }
            // Apply hand-to-hand proficiency. Hand-to-hand proficiency is not applied in classic.
            else if (((int)attacker.Career.ExpertProficiencies & (int)DFCareer.ProficiencyFlags.HandToHand) != 0)
            {
                mods.damageMod = (attacker.Stats.LiveStrength / 50) + (attacker.Stats.LiveEndurance / 50) + (attacker.Stats.LiveAgility / 50) + (attacker.Stats.LiveSpeed / 50) + 1; //9
                mods.toHitMod = (attacker.Stats.LiveAgility / 14) + (attacker.Stats.LiveSpeed / 14) + (attacker.Stats.LiveStrength / 14) + (attacker.Stats.LiveEndurance / 14) + (attacker.Stats.LiveLuck / 14); //35
            }
            //Debug.LogFormat("Here is the damage modifier for this proficiency = {0}", mods.damageMod);
            //Debug.LogFormat("Here is the accuracy modifier for this proficiency = {0}", mods.toHitMod);
            return mods;
        }

        public static ToHitAndDamageMods CalculateRacialModifiers(DaggerfallEntity attacker, DaggerfallUnityItem weapon, PlayerEntity player)
        {
            ToHitAndDamageMods mods = new ToHitAndDamageMods();
            if (weapon != null)
            {
                switch (player.RaceTemplate.ID)
                {
                    case (int)Races.Argonian:
                        if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.ShortBlade)
                        {
                            mods.damageMod = (attacker.Stats.LiveAgility / 50) + (attacker.Stats.LiveSpeed / 50); //4
                            mods.toHitMod = (attacker.Stats.LiveAgility / 16) + (attacker.Stats.LiveSpeed / 16) + (attacker.Stats.LiveLuck / 33); //15
                        }
                        break;
                    case (int)Races.DarkElf:
                        if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.LongBlade)
                        {
                            mods.damageMod = (attacker.Stats.LiveAgility / 33) + (attacker.Stats.LiveStrength / 50); //5
                            mods.toHitMod = (attacker.Stats.LiveAgility / 25) + (attacker.Stats.LiveSpeed / 33) + (attacker.Stats.LiveLuck / 33); //10
                        }
                        else if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.ShortBlade)
                        {
                            mods.damageMod = (attacker.Stats.LiveAgility / 75) + (attacker.Stats.LiveSpeed / 75); //2
                            mods.toHitMod = (attacker.Stats.LiveAgility / 20) + (attacker.Stats.LiveSpeed / 20) + (attacker.Stats.LiveLuck / 50); //12
                        }
                        break;
                    case (int)Races.Khajiit:
                        if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.ShortBlade)
                        {
                            mods.damageMod = (attacker.Stats.LiveAgility / 50) + (attacker.Stats.LiveSpeed / 50); //4
                            mods.toHitMod = (attacker.Stats.LiveAgility / 20) + (attacker.Stats.LiveSpeed / 20) + (attacker.Stats.LiveLuck / 33); //13
                        }
                        break;
                    case (int)Races.Nord:
                        if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.Axe)
                        {
                            mods.damageMod = (attacker.Stats.LiveStrength / 33) + (attacker.Stats.LiveAgility / 50); //5
                            mods.toHitMod = (attacker.Stats.LiveStrength / 33) + (attacker.Stats.LiveAgility / 25) + (attacker.Stats.LiveLuck / 33); //10
                        }
                        else if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.BluntWeapon)
                        {
                            mods.damageMod = (attacker.Stats.LiveStrength / 33) + (attacker.Stats.LiveEndurance / 33); //6
                            mods.toHitMod = (attacker.Stats.LiveStrength / 33) + (attacker.Stats.LiveAgility / 33) + (attacker.Stats.LiveLuck / 33); //9
                        }
                        break;
                    case (int)Races.Redguard:
                        if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.LongBlade)
                        {
                            mods.damageMod = (attacker.Stats.LiveAgility / 33) + (attacker.Stats.LiveStrength / 50); //5
                            mods.toHitMod = (attacker.Stats.LiveAgility / 20) + (attacker.Stats.LiveSpeed / 20) + (attacker.Stats.LiveLuck / 20); //15
                        }
                        else if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.BluntWeapon)
                        {
                            mods.damageMod = (attacker.Stats.LiveStrength / 50) + (attacker.Stats.LiveEndurance / 50); //4
                            mods.toHitMod = (attacker.Stats.LiveStrength / 33) + (attacker.Stats.LiveAgility / 25) + (attacker.Stats.LiveLuck / 33); //10
                        }
                        else if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.Axe)
                        {
                            mods.damageMod = (attacker.Stats.LiveStrength / 50) + (attacker.Stats.LiveAgility / 50); //4
                            mods.toHitMod = (attacker.Stats.LiveStrength / 33) + (attacker.Stats.LiveAgility / 25) + (attacker.Stats.LiveLuck / 33); //10
                        }
                        else if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.Archery)
                        {
                            mods.damageMod = (attacker.Stats.LiveStrength / 50) + (attacker.Stats.LiveAgility / 75); //3
                            mods.toHitMod = (attacker.Stats.LiveAgility / 33) + (attacker.Stats.LiveSpeed / 50) + (attacker.Stats.LiveLuck / 33); //8
                        }
                        break;
                    case (int)Races.WoodElf:
                        if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.ShortBlade)
                        {
                            mods.damageMod = (attacker.Stats.LiveAgility / 50) + (attacker.Stats.LiveSpeed / 75); //3
                            mods.toHitMod = (attacker.Stats.LiveAgility / 20) + (attacker.Stats.LiveSpeed / 33) + (attacker.Stats.LiveLuck / 50); //10
                        }
                        else if (weapon.GetWeaponSkillIDAsShort() == (short)DFCareer.Skills.Archery)
                        {
                            mods.damageMod = (attacker.Stats.LiveStrength / 33) + (attacker.Stats.LiveAgility / 33); //6
                            mods.toHitMod = (attacker.Stats.LiveAgility / 12) + (attacker.Stats.LiveSpeed / 20) + (attacker.Stats.LiveLuck / 20); //18
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (weapon == null)
            {
                if (player.RaceTemplate.ID == (int)Races.Khajiit)
                {
                    mods.damageMod = (attacker.Stats.LiveStrength / 50) + (attacker.Stats.LiveEndurance / 50) + (attacker.Stats.LiveAgility / 75) + (attacker.Stats.LiveSpeed / 75); //6
                    mods.toHitMod = (attacker.Stats.LiveAgility / 20) + (attacker.Stats.LiveSpeed / 33) + (attacker.Stats.LiveStrength / 33) + (attacker.Stats.LiveEndurance / 33) + (attacker.Stats.LiveLuck / 50); //16
                }
                else if (player.RaceTemplate.ID == (int)Races.Nord)
                {
                    mods.damageMod = (attacker.Stats.LiveStrength / 50) + (attacker.Stats.LiveEndurance / 50); //4
                }
            }
            //Debug.LogFormat("Here is the damage modifier for this Race and Weapon = {0}", mods.damageMod);
            //Debug.LogFormat("Here is the accuracy modifier for this Race and Weapon = {0}", mods.toHitMod);
            return mods;
        }

        private static int CalculateBackstabChance(PlayerEntity player, DaggerfallEntity target, int enemyAnimStateRecord)
        {
            // If enemy is facing away from player
            if (enemyAnimStateRecord % 5 > 2)
            {
                player.TallySkill(DFCareer.Skills.Backstabbing, 1);
                return player.Skills.GetLiveSkillValue(DFCareer.Skills.Backstabbing);
            }
            else
                return 0;
        }

        public static int CalculateBackstabDamage(int damage, int backstabbingLevel, int damageType = 0, DaggerfallUnityItem weapon = null)
        {
            Func<int, int, int> del;
            if (TryGetOverride("CalculateBackstabDamage", out del))
                return del(damage, backstabbingLevel);

            if (backstabbingLevel > 1 && Dice100.SuccessRoll(backstabbingLevel))
            {
                if (weapon != null && damageType == 3 && (weapon.TemplateIndex == (int)Weapons.Dagger || weapon.TemplateIndex == (int)Weapons.Tanto))
                    damage *= 5;
                else if (weapon != null && (weapon.TemplateIndex == (int)Weapons.Dagger || weapon.TemplateIndex == (int)Weapons.Tanto))
                    damage *= 4;
                else if (weapon != null && damageType == 3)
                    damage *= 3;
                else
                    damage *= 2;
                string backstabMessage = TextManager.Instance.GetLocalizedText("successfulBackstab");
                DaggerfallUI.Instance.PopupMessage(backstabMessage);
            }
            return damage;
        }

        static int GetBonusOrPenaltyByEnemyType(DaggerfallEntity attacker, DaggerfallEntity target) // Possibly update at some point like 10.26 did so vampirism of the player is taken into account.
        {
            if (attacker == null || target == null) // So after observing the effects of adding large amounts of weight to an enemy, it does not seem to have that much of an effect on their ability to be stun-locked. As the knock-back/hurt state is probably the real issue here, as well as other parts of the AI choices. So I think this comes down a lot more to AI behavior than creature weight values. So with that, I will mostly likely make an entirely seperate mod to try and deal with this issue and continue on non-AI related stuff in this already large mod. So yeah, start another "proof of concept" mod project where I attempt to change the AI to make it more challenging/smarter.
                return 0;

            int attackerWillpMod = 0;
            int confidenceMod = 0;
            int courageMod = 0;
            EnemyEntity AITarget = null;
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            if (target != player)
                AITarget = target as EnemyEntity;
            else
                player = target as PlayerEntity;

            if (player == attacker) // When attacker is the player
            {
                attackerWillpMod = (int)Mathf.Round((attacker.Stats.LiveWillpower - 50) / 5);
                confidenceMod = Mathf.Max(3 + attackerWillpMod - (target.Level / 3), 0);
                courageMod = Mathf.Max((target.Level / 4) - attackerWillpMod, 0);

                confidenceMod = UnityEngine.Random.Range(0, confidenceMod);
            }
            else // When attacker is anything other than the player // Apparently "32" is the maximum possible level cap for the player without cheating.
            {
                attackerWillpMod = (int)Mathf.Round((attacker.Stats.LiveWillpower - 50) / 5);
                confidenceMod = Mathf.Max(5 + attackerWillpMod + (attacker.Level / 4), 0);
                courageMod = Mathf.Max(target.Level - (attacker.Level + attackerWillpMod), 0);

                confidenceMod = UnityEngine.Random.Range(0, confidenceMod);
            }

            int damage = 0;
            // Apply bonus or penalty by opponent type.
            // In classic this is broken and only works if the attack is done with a weapon that has the maximum number of enchantments.
            if (AITarget != null && AITarget.GetEnemyGroup() == DFCareer.EnemyGroups.Undead)
            {
                if (((int)attacker.Career.UndeadAttackModifier & (int)DFCareer.AttackModifier.Bonus) != 0)
                {
                    damage += confidenceMod;
                }
                if (((int)attacker.Career.UndeadAttackModifier & (int)DFCareer.AttackModifier.Phobia) != 0)
                {
                    damage -= courageMod;
                }
            }
            else if (AITarget != null && AITarget.GetEnemyGroup() == DFCareer.EnemyGroups.Daedra)
            {
                if (((int)attacker.Career.DaedraAttackModifier & (int)DFCareer.AttackModifier.Bonus) != 0)
                {
                    damage += confidenceMod;
                }
                if (((int)attacker.Career.DaedraAttackModifier & (int)DFCareer.AttackModifier.Phobia) != 0)
                {
                    damage -= courageMod;
                }
            }
            else if ((AITarget != null && AITarget.GetEnemyGroup() == DFCareer.EnemyGroups.Humanoid) || player == target) // Apparently human npcs already are in the humanoid career, so "|| AITarget.EntityType == EntityTypes.EnemyClass" is unneeded.
            {
                if (((int)attacker.Career.HumanoidAttackModifier & (int)DFCareer.AttackModifier.Bonus) != 0)
                {
                    damage += confidenceMod;
                }
                if (((int)attacker.Career.HumanoidAttackModifier & (int)DFCareer.AttackModifier.Phobia) != 0)
                {
                    damage -= courageMod;
                }
            }
            else if (AITarget != null && AITarget.GetEnemyGroup() == DFCareer.EnemyGroups.Animals)
            {
                if (((int)attacker.Career.AnimalsAttackModifier & (int)DFCareer.AttackModifier.Bonus) != 0)
                {
                    damage += confidenceMod;
                }
                if (((int)attacker.Career.AnimalsAttackModifier & (int)DFCareer.AttackModifier.Phobia) != 0)
                {
                    damage -= courageMod;
                }
            }
            return damage;
        }

        public static int AdjustWeaponHitChanceMod(DaggerfallEntity attacker, DaggerfallEntity target, int hitChanceMod, int weaponAnimTime, DaggerfallUnityItem weapon)
        {
            Func<DaggerfallEntity, DaggerfallEntity, int, int, DaggerfallUnityItem, int> del;
            if (TryGetOverride("AdjustWeaponHitChanceMod", out del))
                return del(attacker, target, hitChanceMod, weaponAnimTime, weapon);

            return hitChanceMod;
        }

        public static int AdjustWeaponAttackDamage(DaggerfallEntity attacker, DaggerfallEntity target, int damage, int weaponAnimTime, DaggerfallUnityItem weapon)
        {
            Func<DaggerfallEntity, DaggerfallEntity, int, int, DaggerfallUnityItem, int> del;
            if (TryGetOverride("AdjustWeaponAttackDamage", out del))
                return del(attacker, target, damage, weaponAnimTime, weapon);

            return damage;
        }

        /// Allocate any equipment damage from a strike, and reduce item condition.
		public static void DamageEquipment(DaggerfallEntity attacker, DaggerfallEntity target, int struckBodyPart, bool shieldBlockSuccess, int damType, int damBefore, int damAfter, EnemyBasics.CustomEnemyStatValues enemyAttackerStats, DaggerfallUnityItem weapon = null)
        {
            if (damType == 4) // Special damage type won't deal damage to equipment, since it ignores it.
                return;
            if (damBefore <= 0)
                return;

            PlayerEntity player = GameManager.Instance.PlayerEntity;
            int damAbsorbed = damBefore - damAfter;
            float damTypeMulti = 1f;
            float strengthMulti = 0f;
            float armorDensityMulti = 1f;
            bool missileWep = false;
            int startItemCondPer = 0;
            bool weaponUsed = false;
            if (attacker == player)
                strengthMulti = Mathf.Clamp((attacker.Stats.LiveStrength - 50) / 5, 0, 100) * 0.025f;
            else
                strengthMulti = Mathf.Clamp((enemyAttackerStats.strengthCustom - 50) / 5, 0, 100) * 0.025f;

            if (weapon != null)
                weaponUsed = true;

            if (damType == 1)
                damTypeMulti = 0.7f;
            else if (damType == 2)
                damTypeMulti = 1.3f;
            else if (damType == 3)
                damTypeMulti = 1.1f;

            if (shieldBlockSuccess)
            {
                DaggerfallUnityItem shield = target.ItemEquipTable.GetItem(EquipSlots.LeftHand); // Checks if entity is using a shield or not.
                if (shield != null)
                {
                    armorDensityMulti = ((shield.density - 300) / 50 * 0.1f) + 1;
                    startItemCondPer = shield.ConditionPercentage;
                    ApplyConditionDamageThroughPhysicalDamage(attacker, damAbsorbed, damAfter, damTypeMulti, strengthMulti, armorDensityMulti, missileWep, shield, weaponUsed);

                    if (target == GameManager.Instance.PlayerEntity)
                        WarningMessagePlayerEquipmentCondition(shield, startItemCondPer);
                }
            }
            else
            {
                EquipSlots hitSlot = DaggerfallUnityItem.GetEquipSlotForBodyPart((BodyParts)struckBodyPart);
                DaggerfallUnityItem armor = target.ItemEquipTable.GetItem(hitSlot);
                if (armor != null)
                {
                    armorDensityMulti = ((armor.density - 300) / 50 * 0.1f) + 1;
                    startItemCondPer = armor.ConditionPercentage;
                    ApplyConditionDamageThroughPhysicalDamage(attacker, damAbsorbed, damAfter, damTypeMulti, strengthMulti, armorDensityMulti, missileWep, armor, weaponUsed);

                    if (target == GameManager.Instance.PlayerEntity)
                        WarningMessagePlayerEquipmentCondition(armor, startItemCondPer);
                }
            }

            // If damage was done by a weapon, damage the weapon and armor of the hit body part.
            if (weapon != null)
            {
                if (weapon.GetWeaponSkillIDAsShort() == 33) // Checks if the weapon being used is in the Missile Weapon category, then sets a bool value to true.
                    missileWep = true;
                startItemCondPer = weapon.ConditionPercentage;
                ApplyConditionDamageThroughPhysicalDamage(attacker, damAbsorbed, damAfter, damTypeMulti, strengthMulti, armorDensityMulti, missileWep, weapon, weaponUsed); // Does condition damage to the attackers weapon.

                if (attacker == GameManager.Instance.PlayerEntity)
                    WarningMessagePlayerEquipmentCondition(weapon, startItemCondPer);
            }
        }

        public static int CalculateArmorToHit(DaggerfallEntity target, int struckBodyPart)
        {
            EnemyEntity AITarget = null;
            AITarget = target as EnemyEntity;
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            int armorValue = 0; // taking out the whole "innate armor class" from enemies and just replacing that with an easier to work with dodge chance value I have control of 1 to 1, simple.

            // Sets the armorValue so that armor does not have any effect on the hit chance, it just defaults to the "naked" amount for the player and humanoid enemies, other monsters still have their normal AC score factored in.
            if (target == player)
                armorValue = 45 - target.IncreasedArmorValueModifier - target.DecreasedArmorValueModifier;

            return armorValue;
        }

        public static int CalculateAdrenalineRushToHit(DaggerfallEntity attacker, DaggerfallEntity target)
        {
            const int adrenalineRushModifier = 12;
            const int improvedAdrenalineRushModifier = 18;

            int chanceToHitMod = 0;
            if (attacker.Career.AdrenalineRush && attacker.CurrentHealth < (attacker.MaxHealth / 6)) //Made adrenaline rush effect come into effect earlier, I.E. at higher health percent. From /8 to /6
            {
                chanceToHitMod += (attacker.ImprovedAdrenalineRush) ? improvedAdrenalineRushModifier : adrenalineRushModifier;
            }

            if (target.Career.AdrenalineRush && target.CurrentHealth < (target.MaxHealth / 6)) //Made adrenaline rush effect come into effect earlier, I.E. at higher health percent. From /8 to /6
            {
                chanceToHitMod -= (target.ImprovedAdrenalineRush) ? improvedAdrenalineRushModifier : adrenalineRushModifier;
            }
            return chanceToHitMod;
        }

        public static int CalculateStatDiffsToHit(DaggerfallEntity attacker, DaggerfallEntity target, EnemyBasics.CustomEnemyStatValues enemyAttackerStats, EnemyBasics.CustomEnemyStatValues enemyTargetStats)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            int chanceToHitMod = 0;

            // Apply luck modifier.
            if (attacker == player)
                chanceToHitMod += ((attacker.Stats.LiveLuck - enemyTargetStats.luckCustom) / 8);
            else if (target == player)
                chanceToHitMod += ((enemyAttackerStats.luckCustom - target.Stats.LiveLuck) / 8);
            else
                chanceToHitMod += ((enemyAttackerStats.luckCustom - enemyTargetStats.luckCustom) / 8);
            //Debug.LogFormat("After Luck = {0}", chanceToHitMod);

            // Apply agility modifier.
            if (attacker == player)
                chanceToHitMod += ((attacker.Stats.LiveAgility - enemyTargetStats.agilityCustom) / 3); //Made Agility have twice as much effect on final hit chance.
            else if (target == player)
                chanceToHitMod += ((enemyAttackerStats.agilityCustom - target.Stats.LiveAgility) / 3);
            else
                chanceToHitMod += ((enemyAttackerStats.agilityCustom - enemyTargetStats.agilityCustom) / 3);
            //Debug.LogFormat("After Agility = {0}", chanceToHitMod);

            // Possibly make the Speed Stat a small factor as well, seems like it would make sense.
            if (attacker == player)
                chanceToHitMod += ((attacker.Stats.LiveSpeed - enemyTargetStats.speedCustom) / 7);
            else if (target == player)
                chanceToHitMod += ((enemyAttackerStats.speedCustom - target.Stats.LiveSpeed) / 7);
            else
                chanceToHitMod += ((enemyAttackerStats.speedCustom - enemyTargetStats.speedCustom) / 7);
            //Debug.LogFormat("After Speed = {0}", chanceToHitMod);

            // When I think about it, I might want to get some of the other stats into this formula as well, to help casters somewhat, as well as explain it like a more intelligent character notices patterns in enemy movement and uses to to get in more hits, maybe even strength, the character strikes with such force that they pierce through armor easier.

            // Apply flat Luck factor for the target's chance of being hit. Higher luck above 50 means enemies will miss you more, and below 50 will mean they hit you more often.
            if (target == player)
                chanceToHitMod -= (int)Mathf.Round((float)(target.Stats.LiveLuck - 50) / 10); // With this, at most Luck will effect chances by either -5 or +5.
            else
                chanceToHitMod -= (int)Mathf.Round((float)(enemyTargetStats.luckCustom - 50) / 10);

            return chanceToHitMod;
        }

        public static int CalculateSkillsToHit(DaggerfallEntity attacker, DaggerfallEntity target, EnemyBasics.CustomEnemyStatValues enemyTargetStats)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            int chanceToHitMod = 0;

            // Apply dodging modifier.
            // This modifier is bugged in classic and the attacker's dodging skill is used rather than the target's.
            // DF Chronicles says the dodging calculation is (dodging / 10), but it actually seems to be (dodging / 4).
            // Apply dodging modifier.
            if (target == player)
                chanceToHitMod -= (target.Skills.GetLiveSkillValue(DFCareer.Skills.Dodging) / 2); // Changing 4 to a 2, so 100 dodge will give -50 to hit chance, very powerful.
            else
                chanceToHitMod -= enemyTargetStats.dodgeSkillCustom; // So for monsters, dodge skill is 1 to 1 for how much it effects chances to not be hit.

            return chanceToHitMod;
        }

        public static int CalculateAdjustmentsToHit(DaggerfallEntity attacker, DaggerfallEntity target)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            EnemyEntity AITarget = target as EnemyEntity;

            int chanceToHitMod = 0;

            // Apply hit mod from character biography. This gives -5 to player chances to not be hit if they say they have trouble "Fighting and Parrying"
            if (target == player)
            {
                chanceToHitMod -= player.BiographyAvoidHitMod;
            }

            return chanceToHitMod;
        }

        /// Applies condition damage to an item based on physical hit damage.
        public static void ApplyConditionDamageThroughPhysicalDamage(DaggerfallEntity owner, int damAbsorbed, int damAfter, float damTypeMulti, float strengthMulti, float armorDensityMulti, bool missileWep, DaggerfallUnityItem item, bool weaponUsed)
        {
            ItemCollection playerItems = GameManager.Instance.PlayerEntity.Items;
            int amount = 0;

            //Debug.LogFormat("Item Group Index is {0}", item.GroupIndex);
            //Debug.LogFormat("Item Template Index is {0}", item.TemplateIndex);

            if (item.ItemGroup == ItemGroups.Armor) // Target gets their armor/shield condition damaged.
            {
                if (weaponUsed)
                    amount = (int)Mathf.Round((damAbsorbed * 3) / armorDensityMulti); // Might alter this later to account for other properties, including weapon properties doing the attack, etc.
                else
                    amount = (int)Mathf.Round((damAbsorbed * 2) / armorDensityMulti); // Unarmed attacks do less condition damage to armor.

                if (owner == GameManager.Instance.PlayerEntity && item.IsEnchanted) // If the Weapon or Armor piece is enchanted, when broken it will be Destroyed from the player inventory.
                    item.LowerCondition(amount, owner, playerItems);
                else
                    item.LowerCondition(amount, owner);

                /*int percentChange = 100 * amount / item.maxCondition;
                if (owner == GameManager.Instance.PlayerEntity){
                    Debug.LogFormat("Target Had {0} Damaged by {1}, cond={2}", item.LongName, amount, item.currentCondition);
					Debug.LogFormat("Had {0} Damaged by {1}%, of Total Maximum. There Remains {2}% of Max Cond.", item.LongName, percentChange, item.ConditionPercentage);} // Percentage Change */
            }
            else // Attacker gets their weapon damaged, if they are using one, otherwise this method is not called.
            {
                if (damAbsorbed == 0 || (damAfter * (damTypeMulti - strengthMulti) * 0.20f) > damAbsorbed)
                    amount = (int)Mathf.Round(damAfter * (damTypeMulti - strengthMulti) * 0.20f); // Weapon gets damaged at least 20% of the damage dealt if there was no or little damage absorbed from other sources, I.E blade still dulls cutting flesh.
                else
                    amount = (int)Mathf.Round(damAbsorbed * (damTypeMulti + armorDensityMulti - strengthMulti) );

                if ((amount == 0) && Dice100.SuccessRoll(40))
                    amount = 1;

                if (missileWep)
                    amount = 1; // I'll have to figure out a way to deal damage to a bow when an arrow is shot, even if there was no target hit, will do later.

                if (owner == GameManager.Instance.PlayerEntity && item.IsEnchanted) // If the Weapon or Armor piece is enchanted, when broken it will be Destroyed from the player inventory.
                    item.LowerCondition(amount, owner, playerItems);
                else
                    item.LowerCondition(amount, owner);

                /*int percentChange = 100 * amount / item.maxCondition;
                if (owner == GameManager.Instance.PlayerEntity){
                    Debug.LogFormat("Attacker Damaged {0} by {1}, cond={2}", item.LongName, amount, item.currentCondition);
                    Debug.LogFormat("Had {0} Damaged by {1}%, of Total Maximum. There Remains {2}% of Max Cond.", item.LongName, percentChange, item.ConditionPercentage);} // Percentage Change */
            }
        }

        /// Does a roll for based on the critical strike chance of the attacker, if this roll is successful critSuccess is returned as 'true'.
		public static bool CriticalStrikeHandler(DaggerfallEntity attacker, EnemyBasics.CustomEnemyStatValues enemyAttackerStats)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            int attackerLuckBonus = 0;

            if (attacker == player)
                attackerLuckBonus = (int)Mathf.Floor((float)(attacker.Stats.LiveLuck - 50) / 25f);
            else
                attackerLuckBonus = (int)Mathf.Floor((float)(enemyAttackerStats.luckCustom - 50) / 25f);

            Mathf.Clamp(attackerLuckBonus, -2, 2); // This is meant to disallow crit odds from going higher than 50%, incase luck is allowed to go over 100 points.

            if (attacker == player)
            {
                if (Dice100.SuccessRoll(attacker.Skills.GetLiveSkillValue(DFCareer.Skills.CriticalStrike) / (4 - attackerLuckBonus))) // Player has a 25% chance of critting at level 100. 33% with 75 luck, and 50% with 100 luck.
                    return true;
                else
                    return false;
            }
            else
            {
                if (Dice100.SuccessRoll(enemyAttackerStats.critSkillCustom / (5 - attackerLuckBonus))) // Monsters have a 20% chance of critting at level 100, or level 14.
                    return true;
                else
                    return false;
            }

        }

        #endregion

        #region Effects and Resistances

        // If the player has equipment that is below a certain percentage of condition, this will check if they should be warned with a pop-up message about said piece of equipment.
        public static void WarningMessagePlayerEquipmentCondition(DaggerfallUnityItem item, int startItemCondPer)
        {
            string roughItemMessage = "";
            string damagedItemMessage = "";
            string majorDamageItemMessage = "";
            int condDiff = startItemCondPer - item.ConditionPercentage;

            if (item.ConditionPercentage <= 49 || condDiff >= 15)
            {
                if (item.IsEnchanted) // All Magically Enchanted Items Text
                {
                    if (item.customMagic != null)
                    {
                        string shortMagicItemName = item.shortName;
                        roughItemMessage = String.Format("My {0} Is Flickering Slightly", shortMagicItemName);
                        damagedItemMessage = String.Format("My {0} Is Going In And Out Of Existence", shortMagicItemName);
                        majorDamageItemMessage = String.Format("My {0} Was Drained Significantly By That", shortMagicItemName);
                    }
                    else
                    {
                        string longMagicItemName = item.LongName;
                        roughItemMessage = String.Format("My {0} Is Flickering Slightly", longMagicItemName);
                        damagedItemMessage = String.Format("My {0} Is Going In And Out Of Existence", longMagicItemName);
                        majorDamageItemMessage = String.Format("My {0} Was Drained Significantly By That", longMagicItemName);
                    }
                }
                else
                {
                    string shortItemName = item.shortName;
                    switch (item.TemplateIndex)
                    {
                        case (int)Armor.Boots:
                        case (int)Armor.Gauntlets:  // Armor With Plural Names Text
                        case (int)Armor.Greaves:
                        case 516:                   // RPR:I, Chausses Index Value
                        case 519:                   // RPR:I, Sollerets Index Value
                            roughItemMessage = String.Format("My {0} Are In Rough Shape", shortItemName);
                            damagedItemMessage = String.Format("My {0} Are Falling Apart", shortItemName);
                            majorDamageItemMessage = String.Format("My {0} Were Shredded Heavily By That", shortItemName);
                            break;
                        case (int)Weapons.Broadsword:
                        case (int)Weapons.Claymore:
                        case (int)Weapons.Dai_Katana:
                        case (int)Weapons.Katana:
                        case (int)Weapons.Longsword:
                        case (int)Weapons.Saber:    // Bladed Weapons Text
                        case (int)Weapons.Dagger:
                        case (int)Weapons.Shortsword:
                        case (int)Weapons.Tanto:
                        case (int)Weapons.Wakazashi:
                        case (int)Weapons.Battle_Axe:
                        case (int)Weapons.War_Axe:
                        case 513:                   // RPR:I, Archer's Axe Index Value
                            roughItemMessage = String.Format("My {0} Could Use A Sharpening", shortItemName);
                            damagedItemMessage = String.Format("My {0} Looks As Dull As A Butter Knife", shortItemName);
                            majorDamageItemMessage = String.Format("My {0} Lost A lot Of Edge From That Swipe", shortItemName);
                            break;
                        case (int)Weapons.Flail:
                        case (int)Weapons.Mace:     // Blunt Weapoons Text
                        case (int)Weapons.Staff:
                        case (int)Weapons.Warhammer:
                        case 514:                   // RPR:I, Light Flail Index Value
                            roughItemMessage = String.Format("My {0}'s Shaft Has Some Small Cracks", shortItemName);
                            damagedItemMessage = String.Format("My {0}'s Shaft Is Nearly Split In Two", shortItemName);
                            majorDamageItemMessage = String.Format("My {0} Shaft Was Cracked By That Swing", shortItemName);
                            break;
                        case (int)Weapons.Long_Bow: // Archery Weapons Text
                        case (int)Weapons.Short_Bow:
                            roughItemMessage = String.Format("The Bowstring On My {0} Is Losing Its Twang", shortItemName);
                            damagedItemMessage = String.Format("The Bowstring On My {0} Looks Ready To Snap", shortItemName);
                            majorDamageItemMessage = String.Format("The Bowstring On My {0} Nearly Snapped From That", shortItemName);
                            break;
                        default:                    // Text for any other Valid Items
                            roughItemMessage = String.Format("My {0} Is In Rough Shape", shortItemName);
                            damagedItemMessage = String.Format("My {0} Is Falling Apart", shortItemName);
                            majorDamageItemMessage = String.Format("My {0} Was Shredded Heavily By That", shortItemName);
                            break;
                    }
                }

                if (item.ConditionPercentage == 48) // 49 & 45 // This will work for now, until I find a more elegant solution.
                    DaggerfallUI.AddHUDText(roughItemMessage, 2.00f); // Possibly make a random between a few of these lines to mix it up or something.				
                else if (item.ConditionPercentage == 15) // 16 & 12
                    DaggerfallUI.AddHUDText(damagedItemMessage, 2.00f);
                else if (condDiff >= 15)
                    DaggerfallUI.AddHUDText(majorDamageItemMessage, 2.00f);
            }
        }

        // Check whether the struck body part of the target was covered by armor or shield, returns true if yes, false if no.
        private static bool ArmorStruckVerification(DaggerfallEntity target, int struckBodyPart, bool shieldBlockSuccess)
        {
            if (shieldBlockSuccess)
                return true;
            else
            {
                EquipSlots hitSlot = DaggerfallUnityItem.GetEquipSlotForBodyPart((BodyParts)struckBodyPart);
                DaggerfallUnityItem armor = target.ItemEquipTable.GetItem(hitSlot);
                if (armor != null)
                    return true;
            }
            return false;
        }

        // Checks for if a shield block was successful and returns true if so, false if not.
        public static bool ShieldBlockChanceCalculation(DaggerfallEntity target, bool shieldStrongSpot, DaggerfallUnityItem shield, EnemyBasics.CustomEnemyStatValues enemyTargetStats)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;

            float hardBlockChance = 0f;
            float softBlockChance = 0f;
            int targetAgili = 0;
            int targetSpeed = 0;
            int targetStren = 0;
            int targetEndur = 0;
            int targetWillp = 0;
            int targetLuck = 0;

            if (target == player)
            {
                targetAgili = target.Stats.LiveAgility - 50;
                targetSpeed = target.Stats.LiveSpeed - 50;
                targetStren = target.Stats.LiveStrength - 50;
                targetEndur = target.Stats.LiveEndurance - 50;
                targetWillp = target.Stats.LiveWillpower - 50;
                targetLuck = target.Stats.LiveLuck - 50;
            }
            else
            {
                targetAgili = enemyTargetStats.agilityCustom - 50;
                targetSpeed = enemyTargetStats.speedCustom - 50;
                targetStren = enemyTargetStats.strengthCustom - 50;
                targetEndur = target.Stats.LiveEndurance - 50;
                targetWillp = enemyTargetStats.willpowerCustom - 50;
                targetLuck = enemyTargetStats.luckCustom - 50;
            }

            switch (shield.TemplateIndex)
            {
                case (int)Armor.Buckler:
                    hardBlockChance = 30f;
                    softBlockChance = 20f;
                    break;
                case (int)Armor.Round_Shield:
                    hardBlockChance = 35f;
                    softBlockChance = 10f;
                    break;
                case (int)Armor.Kite_Shield:
                    hardBlockChance = 45f;
                    softBlockChance = 5f;
                    break;
                case (int)Armor.Tower_Shield:
                    hardBlockChance = 55f;
                    softBlockChance = -5f;
                    break;
                default:
                    hardBlockChance = 40f;
                    softBlockChance = 0f;
                    break;
            }

            softBlockChance += (ItemBuilder.weightMultipliersByMaterial[DaggerfallUnityItem.MaterialIdentification(shield)] - 6) * -2.5f; // I don't know why, but the math here is fucking with me, I think I would have to do this in a few steps to get the results i'm after, i'll just leave it simple like this for now.

            if (shieldStrongSpot)
            {
                hardBlockChance += (targetAgili * .3f);
                hardBlockChance += (targetSpeed * .3f);
                hardBlockChance += (targetStren * .3f);
                hardBlockChance += (targetEndur * .2f);
                hardBlockChance += (targetWillp * .1f);
                hardBlockChance += (targetLuck * .1f);

                Mathf.Clamp(hardBlockChance, 7f, 95f);
                int blockChanceInt = (int)Mathf.Round(hardBlockChance);

                if (Dice100.SuccessRoll(blockChanceInt))
                {
                    //Debug.LogFormat("$$$. Shield Blocked A Hard-Point, Chance Was {0}%", blockChanceInt);
                    return true;
                }
                else
                {
                    //Debug.LogFormat("!!!. Shield FAILED To Block A Hard-Point, Chance Was {0}%", blockChanceInt);
                    return false;
                }
            }
            else
            {
                softBlockChance += (targetAgili * .3f);
                softBlockChance += (targetSpeed * .2f);
                softBlockChance += (targetStren * .2f);
                softBlockChance += (targetEndur * .1f);
                softBlockChance += (targetWillp * .1f);
                softBlockChance += (targetLuck * .1f);

                Mathf.Clamp(softBlockChance, 0f, 50f);
                int blockChanceInt = (int)Mathf.Round(softBlockChance);

                if (Dice100.SuccessRoll(blockChanceInt))
                {
                    //Debug.LogFormat("$$$. Shield Blocked A Soft-Point, Chance Was {0}%", blockChanceInt);
                    return true;
                }
                else
                {
                    //Debug.LogFormat("!!!. Shield FAILED To Block A Soft-Point, Chance Was {0}%", blockChanceInt);
                    return false;
                }
            }
        }

        // Compares the damage reduction of the struck shield, with the armor under the part that was struck, and returns true if the shield has the higher reduction value, or false if the armor under has a higher reduction value. This is to keep a full-suit of daedric armor from being worse while wearing a leather shield, which when a block is successful, would actually take more damage than if not wearing a shield.
        public static bool CompareShieldToUnderArmor(DaggerfallEntity attacker, DaggerfallEntity target, int damType, int struckBodyPart, float critDamPen, DaggerfallUnityItem shield)
        {
            int redDamShield = 1;
            int redDamUnderArmor = 1;

            redDamShield = CalculateArmorDamageReduction(attacker, target, 100, damType, struckBodyPart, true, critDamPen, null, shield); // Inefficient Repeating of armor and shield finding code.

            EquipSlots hitSlot = DaggerfallUnityItem.GetEquipSlotForBodyPart((BodyParts)struckBodyPart);
            DaggerfallUnityItem armor = target.ItemEquipTable.GetItem(hitSlot);
            if (armor != null)
                redDamUnderArmor = CalculateArmorDamageReduction(attacker, target, 100, damType, struckBodyPart, false, critDamPen, null, shield); // Inefficient Repeating of armor and shield finding code.
            else // If the body part struck in 'naked' IE has no armor protecting it.
            {
                //Debug.Log("$$$: Shield Is Stronger Than Under Armor, Shield Being Used");
                return true;
            }

            if (redDamShield <= redDamUnderArmor)
            {
                //Debug.Log("$$$: Shield Is Stronger Than Under Armor, Shield Being Used");
                return true;
            }
            else
            {
                //Debug.Log("!!!: Shield Is Weaker Than Under Armor, Armor Being Used Instead");
                return false;
            }
        }

        // Multiplies the damage of an attack with a weapon, based on the current condition of said weapon, blunt less effected, but also does not benefit as much from higher condition.
        public static float AlterDamageBasedOnWepCondition(DaggerfallUnityItem weapon, int damType)
        {
            int condPerc = weapon.ConditionPercentage;

            if (damType == 1) // Bludgeoning
            {
                if (condPerc >= 92)                         // New
                    return 1.1f;
                else if (condPerc <= 91 && condPerc >= 76)  // Almost New
                    return 1f;
                else if (condPerc <= 75 && condPerc >= 61)  // Slightly Used
                    return 1f;
                else if (condPerc <= 60 && condPerc >= 41)  // Used
                    return 0.90f;
                else if (condPerc <= 40 && condPerc >= 16)  // Worn
                    return 0.80f;
                else if (condPerc <= 15 && condPerc >= 6)   // Battered
                    return 0.65f;
                else if (condPerc <= 5)                     // Useless, Broken
                    return 0.50f;
                else                                        // Other
                    return 1f;
            }
            else if (damType == 2) // Slashing
            {
                if (condPerc >= 92)                         // New
                    return 1.3f;
                else if (condPerc <= 91 && condPerc >= 76)  // Almost New
                    return 1.1f;
                else if (condPerc <= 75 && condPerc >= 61)  // Slightly Used
                    return 1f;
                else if (condPerc <= 60 && condPerc >= 41)  // Used
                    return 0.85f;
                else if (condPerc <= 40 && condPerc >= 16)  // Worn
                    return 0.70f;
                else if (condPerc <= 15 && condPerc >= 6)   // Battered
                    return 0.45f;
                else if (condPerc <= 5)                     // Useless, Broken
                    return 0.25f;
                else                                        // Other
                    return 1f;
            }
            else if (damType == 3) // Piercing
            {
                if (condPerc >= 92)                         // New
                    return 1.2f;
                else if (condPerc <= 91 && condPerc >= 76)  // Almost New
                    return 1f;
                else if (condPerc <= 75 && condPerc >= 61)  // Slightly Used
                    return 1f;
                else if (condPerc <= 60 && condPerc >= 41)  // Used
                    return 0.90f;
                else if (condPerc <= 40 && condPerc >= 16)  // Worn
                    return 0.75f;
                else if (condPerc <= 15 && condPerc >= 6)   // Battered
                    return 0.55f;
                else if (condPerc <= 5)                     // Useless, Broken
                    return 0.35f;
                else                                        // Other
                    return 1f;
            }
            else // Other
                return 1f;
        }

        // Multiplies the damage reduction of a piece of armor, based on the current condition of said armor.
        public static float AlterDamageReductionBasedOnArmorCondition(DaggerfallUnityItem item, bool shieldBlockSuccess, int damType)
        {
            int condPerc = item.ConditionPercentage;

            if (shieldBlockSuccess) // Shield
            {
                if (condPerc >= 92)                         // New
                    return 1.1f;
                else if (condPerc <= 91 && condPerc >= 76)  // Almost New
                    return 1.05f;
                else if (condPerc <= 75 && condPerc >= 61)  // Slightly Used
                    return 1f;
                else if (condPerc <= 60 && condPerc >= 41)  // Used
                    return 0.90f;
                else if (condPerc <= 40 && condPerc >= 16)  // Worn
                    return 0.80f;
                else if (condPerc <= 15 && condPerc >= 6)   // Battered
                    return 0.70f;
                else if (condPerc <= 5)                     // Useless, Broken
                    return 0.50f;
                else                                        // Other
                    return 1f;
            }
            else // Other Armor
            {
                if (condPerc >= 92)                         // New
                    return 1.1f;
                else if (condPerc <= 91 && condPerc >= 76)  // Almost New
                    return 1.05f;
                else if (condPerc <= 75 && condPerc >= 61)  // Slightly Used
                    return 1f;
                else if (condPerc <= 60 && condPerc >= 41)  // Used
                    return 0.80f;
                else if (condPerc <= 40 && condPerc >= 16)  // Worn
                    return 0.70f;
                else if (condPerc <= 15 && condPerc >= 6)   // Battered
                    return 0.60f;
                else if (condPerc <= 5)                     // Useless, Broken
                    return 0.45f;
                else                                        // Other
                    return 1f;
            }
        }

        public static int CalculateArmorDamageReduction(DaggerfallEntity attacker, DaggerfallEntity target, int damage, int damType, int struckBodyPart, bool shieldBlockSuccess, float critDamPen, DaggerfallUnityItem weapon = null, DaggerfallUnityItem shield = null)
        {
            float reductionPercent = 1f;

            if (shieldBlockSuccess)
            {
                reductionPercent = PercentageDamageReductionCalculation(shield, shieldBlockSuccess, critDamPen, damType);
            }
            else
            {
                EquipSlots hitSlot = DaggerfallUnityItem.GetEquipSlotForBodyPart((BodyParts)struckBodyPart);
                DaggerfallUnityItem armor = target.ItemEquipTable.GetItem(hitSlot);
                if (armor != null)
                {
                    reductionPercent = PercentageDamageReductionCalculation(armor, shieldBlockSuccess, critDamPen, damType);

                    if (armor.ItemGroup == ItemGroups.Jewellery)
                        reductionPercent = 1f;
                }
            }
            return (int)Mathf.Round(damage * reductionPercent);
        }

        public static float PercentageDamageReductionCalculation(DaggerfallUnityItem item, bool shieldBlockSuccess, float critDamPen, int damType)
        {
            if (shieldBlockSuccess) // For Shield
            {
                if (damType == 1)
                {
                    float weightMod = ((ItemBuilder.weightMultipliersByMaterial[DaggerfallUnityItem.MaterialIdentification(item)]) - 6) * 0.030f;
                    float conditionMod = AlterDamageReductionBasedOnArmorCondition(item, shieldBlockSuccess, damType);
                    float reductionMod = ((item.fracture - 300) / 50) * 0.05f;
                    float resultMod = Mathf.Clamp((0.50f + reductionMod + weightMod - critDamPen) * conditionMod, 0f, 1f);
                    return (resultMod - 1f) * -1f;
                }
                else if (damType == 2)
                {
                    float weightMod = ((ItemBuilder.weightMultipliersByMaterial[DaggerfallUnityItem.MaterialIdentification(item)]) - 6) * 0.040f;
                    float conditionMod = AlterDamageReductionBasedOnArmorCondition(item, shieldBlockSuccess, damType);
                    float reductionMod = ((item.shear - 300) / 50) * 0.05f;
                    float resultMod = Mathf.Clamp((0.50f + reductionMod + weightMod - critDamPen) * conditionMod, 0f, 1f);
                    return (resultMod - 1f) * -1f;
                }
                else if (damType == 3)
                {
                    float weightMod = ((ItemBuilder.weightMultipliersByMaterial[DaggerfallUnityItem.MaterialIdentification(item)]) - 6) * 0.045f;
                    float conditionMod = AlterDamageReductionBasedOnArmorCondition(item, shieldBlockSuccess, damType);
                    float reductionMod = ((item.density - 300) / 50) * 0.05f;
                    float resultMod = Mathf.Clamp((0.50f + reductionMod + weightMod - critDamPen) * conditionMod, 0f, 1f);
                    return (resultMod - 1f) * -1f;
                }
                else if (damType == 4) // Special Attacks Ignore Armor Reductions
                    return 1f;
                else
                    return 1f;
            }
            else // For Armor that is not a shield
            {
                if (damType == 1)
                {
                    float weightMod = ((ItemBuilder.weightMultipliersByMaterial[DaggerfallUnityItem.MaterialIdentification(item)]) - 6) * 0.025f;
                    float conditionMod = AlterDamageReductionBasedOnArmorCondition(item, shieldBlockSuccess, damType);
                    float reductionMod = ((item.fracture - 300) / 50) * 0.04f;
                    float resultMod = Mathf.Clamp((0.40f + reductionMod + weightMod - critDamPen) * conditionMod, 0f, 1f);
                    return (resultMod - 1f) * -1f;
                }
                else if (damType == 2)
                {
                    float weightMod = ((ItemBuilder.weightMultipliersByMaterial[DaggerfallUnityItem.MaterialIdentification(item)]) - 6) * 0.035f;
                    float conditionMod = AlterDamageReductionBasedOnArmorCondition(item, shieldBlockSuccess, damType);
                    float reductionMod = ((item.shear - 300) / 50) * 0.04f;
                    float resultMod = Mathf.Clamp((0.40f + reductionMod + weightMod - critDamPen) * conditionMod, 0f, 1f);
                    return (resultMod - 1f) * -1f;
                }
                else if (damType == 3)
                {
                    float weightMod = ((ItemBuilder.weightMultipliersByMaterial[DaggerfallUnityItem.MaterialIdentification(item)]) - 6) * 0.040f;
                    float conditionMod = AlterDamageReductionBasedOnArmorCondition(item, shieldBlockSuccess, damType);
                    float reductionMod = ((item.density - 300) / 50) * 0.04f;
                    float resultMod = Mathf.Clamp((0.40f + reductionMod + weightMod - critDamPen) * conditionMod, 0f, 1f);
                    return (resultMod - 1f) * -1f;
                }
                else if (damType == 4) // Special Attacks Ignore Armor Reductions
                    return 1f;
                else
                    return 1f;
            }
        }

        public static float ShieldBlockChance(DaggerfallUnityItem shield, DaggerfallEntity entity, bool covered)
        {
            float hardBlockChance = 0f;
            float softBlockChance = 0f;
            int targetAgili = entity.Stats.LiveAgility - 50;
            int targetSpeed = entity.Stats.LiveSpeed - 50;
            int targetStren = entity.Stats.LiveStrength - 50;
            int targetEndur = entity.Stats.LiveEndurance - 50;
            int targetWillp = entity.Stats.LiveWillpower - 50;
            int targetLuck = entity.Stats.LiveLuck - 50;

            switch (shield.TemplateIndex)
            {
                case (int)Armor.Buckler:
                    hardBlockChance = 30f;
                    softBlockChance = 20f;
                    break;
                case (int)Armor.Round_Shield:
                    hardBlockChance = 35f;
                    softBlockChance = 10f;
                    break;
                case (int)Armor.Kite_Shield:
                    hardBlockChance = 45f;
                    softBlockChance = 5f;
                    break;
                case (int)Armor.Tower_Shield:
                    hardBlockChance = 55f;
                    softBlockChance = -5f;
                    break;
                default:
                    hardBlockChance = 40f;
                    softBlockChance = 0f;
                    break;
            }

            softBlockChance += (ItemBuilder.weightMultipliersByMaterial[DaggerfallUnityItem.MaterialIdentification(shield)] - 6) * -2.5f; // I don't know why, but the math here is fucking with me, I think I would have to do this in a few steps to get the results i'm after, i'll just leave it simple like this for now.

            if (covered)
            {
                hardBlockChance += (targetAgili * .3f);
                hardBlockChance += (targetSpeed * .3f);
                hardBlockChance += (targetStren * .3f);
                hardBlockChance += (targetEndur * .2f);
                hardBlockChance += (targetWillp * .1f);
                hardBlockChance += (targetLuck * .1f);

                return Mathf.Clamp(hardBlockChance, 7f, 95f);
            }
            else
            {
                softBlockChance += (targetAgili * .3f);
                softBlockChance += (targetSpeed * .2f);
                softBlockChance += (targetStren * .2f);
                softBlockChance += (targetEndur * .1f);
                softBlockChance += (targetWillp * .1f);
                softBlockChance += (targetLuck * .1f);

                return Mathf.Clamp(softBlockChance, 0f, 50f);
            }
        }

        /// <summary>
        /// Execute special monster attack effects.
        /// </summary>
        /// <param name="attacker">Attacking entity</param>
        /// <param name="target">Target entity</param>
        /// <param name="damage">Damage done by the hit</param>
        public static void OnMonsterHit(EnemyEntity attacker, DaggerfallEntity target, int damage)
        {
            Func<EnemyEntity, DaggerfallEntity, int, bool> del;
            if (TryGetOverride("OnMonsterHit", out del))
                del(attacker, target, damage);

            byte[] diseaseListA = { 1 };
            byte[] diseaseListB = { 1, 3, 5 };
            byte[] diseaseListC = { 1, 2, 3, 4, 5, 6, 8, 9, 11, 13, 14 };
            float random;
            switch (attacker.CareerIndex)
            {
                case (int)MonsterCareers.Rat:
                    // In classic rat can only give plague (diseaseListA), but DF Chronicles says plague, stomach rot and brain fever (diseaseListB).
                    // Don't know which was intended. Using B since it has more variety.
                    if (Dice100.SuccessRoll(5))
                        InflictDisease(target, diseaseListB);
                    break;
                case (int)MonsterCareers.GiantBat:
                    // Classic uses 2% chance, but DF Chronicles says 5% chance. Not sure which was intended.
                    if (Dice100.SuccessRoll(2))
                        InflictDisease(target, diseaseListB);
                    break;
                case (int)MonsterCareers.Spider:
                case (int)MonsterCareers.GiantScorpion:
                    EntityEffectManager targetEffectManager = target.EntityBehaviour.GetComponent<EntityEffectManager>();
                    if (targetEffectManager.FindIncumbentEffect<Paralyze>() == null)
                    {
                        SpellRecord.SpellRecordData spellData;
                        GameManager.Instance.EntityEffectBroker.GetClassicSpellRecord(66, out spellData);
                        EffectBundleSettings bundle;
                        GameManager.Instance.EntityEffectBroker.ClassicSpellRecordDataToEffectBundleSettings(spellData, BundleTypes.Spell, out bundle);
                        EntityEffectBundle spell = new EntityEffectBundle(bundle, attacker.EntityBehaviour);
                        EntityEffectManager attackerEffectManager = attacker.EntityBehaviour.GetComponent<EntityEffectManager>();
                        attackerEffectManager.SetReadySpell(spell, true);
                    }
                    break;
                case (int)MonsterCareers.Werewolf:
                    random = UnityEngine.Random.Range(0f, 100f);
                    if (random <= specialInfectionChance && target.EntityBehaviour.EntityType == EntityTypes.Player)
                    {
                        // Werewolf
                        EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateLycanthropyDisease(LycanthropyTypes.Werewolf);
                        GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.SpecialInfection);
                        Debug.Log("Player infected by werewolf.");
                    }
                    break;
                case (int)MonsterCareers.Nymph:
                    FatigueDamage(target, damage);
                    break;
                case (int)MonsterCareers.Wereboar:
                    random = UnityEngine.Random.Range(0f, 100f);
                    if (random <= specialInfectionChance && target.EntityBehaviour.EntityType == EntityTypes.Player)
                    {
                        // Wereboar
                        EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateLycanthropyDisease(LycanthropyTypes.Wereboar);
                        GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.SpecialInfection);
                        Debug.Log("Player infected by wereboar.");
                    }
                    break;
                case (int)MonsterCareers.Zombie:
                    // Nothing in classic. DF Chronicles says 2% chance of disease, which seems like it was probably intended.
                    // Diseases listed in DF Chronicles match those of mummy (except missing cholera, probably a mistake)
                    if (Dice100.SuccessRoll(2))
                        InflictDisease(target, diseaseListC);
                    break;
                case (int)MonsterCareers.Mummy:
                    if (Dice100.SuccessRoll(5))
                        InflictDisease(target, diseaseListC);
                    break;
                case (int)MonsterCareers.Vampire:
                case (int)MonsterCareers.VampireAncient:
                    random = UnityEngine.Random.Range(0f, 100f);
                    if (random <= specialInfectionChance && target.EntityBehaviour.EntityType == EntityTypes.Player)
                    {
                        // Inflict stage one vampirism disease
                        EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateVampirismDisease();
                        GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.SpecialInfection);
                        Debug.Log("Player infected by vampire.");
                    }
                    else if (random <= 2.0f)
                    {
                        InflictDisease(target, diseaseListA);
                    }
                    break;
                case (int)MonsterCareers.Lamia:
                    // Nothing in classic, but DF Chronicles says 2 pts of fatigue damage per health damage
                    FatigueDamage(target, damage);
                    break;
                default:
                    break;
            }
        }

        public static void InflictPoison(DaggerfallEntity target, Poisons poisonType, bool bypassResistance)
        {
            // Target must have an entity behaviour and effect manager
            EntityEffectManager effectManager = null;
            if (target.EntityBehaviour != null)
            {
                effectManager = target.EntityBehaviour.GetComponent<EntityEffectManager>();
                if (effectManager == null)
                    return;
            }
            else
            {
                return;
            }

            // Note: In classic, AI characters' immunity to poison is ignored, although the level 1 check below still gives rats immunity
            DFCareer.Tolerance toleranceFlags = target.Career.Poison;
            if (toleranceFlags == DFCareer.Tolerance.Immune)
                return;

            // Handle player with racial resistance to poison
            if (target is PlayerEntity)
            {
                RaceTemplate raceTemplate = (target as PlayerEntity).GetLiveRaceTemplate();
                if ((raceTemplate.ImmunityFlags & DFCareer.EffectFlags.Poison) == DFCareer.EffectFlags.Poison)
                    return;
            }

            if (bypassResistance || SavingThrow(DFCareer.Elements.DiseaseOrPoison, DFCareer.EffectFlags.Poison, target, 0) != 0)
            {
                // Infect target
                EntityEffectBundle bundle = effectManager.CreatePoison(poisonType);
                effectManager.AssignBundle(bundle, AssignBundleFlags.BypassSavingThrows);
            }
            else
            {
                Debug.LogFormat("Poison resisted by {0}.", target.EntityBehaviour.name);
            }
        }

        public static DFCareer.ToleranceFlags GetToleranceFlag(DFCareer.Tolerance tolerance)
        {
            DFCareer.ToleranceFlags flag = DFCareer.ToleranceFlags.Normal;
            switch (tolerance)
            {
                case DFCareer.Tolerance.Immune:
                    flag = DFCareer.ToleranceFlags.Immune;
                    break;
                case DFCareer.Tolerance.Resistant:
                    flag = DFCareer.ToleranceFlags.Resistant;
                    break;
                case DFCareer.Tolerance.LowTolerance:
                    flag = DFCareer.ToleranceFlags.LowTolerance;
                    break;
                case DFCareer.Tolerance.CriticalWeakness:
                    flag = DFCareer.ToleranceFlags.CriticalWeakness;
                    break;
            }

            return flag;
        }

        public static int SavingThrow(DFCareer.Elements elementType, DFCareer.EffectFlags effectFlags, DaggerfallEntity target, int modifier, bool hasMagnitude = false, DFCareer.MagicSkills spellSchool = DFCareer.MagicSkills.None, int baseAmount = 0, TargetTypes targetType = TargetTypes.None)
        {
            bool singlePartHit = false;
            int equipSaveThrowMod = 0;

            // Handle resistances granted by magical effects
            if (target.HasResistanceFlag(elementType))
            {
                int chance = target.GetResistanceChance(elementType);
                if (Dice100.SuccessRoll(chance))
                    return 0;
            }

            // Magic effect resistances did not stop the effect. Try with career flags and biography modifiers
            int savingThrow = 50;
            DFCareer.ToleranceFlags toleranceFlags = DFCareer.ToleranceFlags.Normal;
            int biographyMod = 0;

            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            if ((effectFlags & DFCareer.EffectFlags.Paralysis) != 0)
            {
                toleranceFlags |= GetToleranceFlag(target.Career.Paralysis);
                // Innate immunity if high elf. Start with 100 saving throw, but can be modified by
                // tolerance flags. Note this differs from classic, where high elves have 100% immunity
                // regardless of tolerance flags.
                if (target == playerEntity && playerEntity.Race == Races.HighElf)
                    savingThrow = 100;
            }
            if ((effectFlags & DFCareer.EffectFlags.Magic) != 0)
            {
                toleranceFlags |= GetToleranceFlag(target.Career.Magic);
                if (target == playerEntity)
                    biographyMod += playerEntity.BiographyResistMagicMod;
            }
            if ((effectFlags & DFCareer.EffectFlags.Poison) != 0)
            {
                toleranceFlags |= GetToleranceFlag(target.Career.Poison);
                if (target == playerEntity)
                    biographyMod += playerEntity.BiographyResistPoisonMod;
            }
            if ((effectFlags & DFCareer.EffectFlags.Fire) != 0)
                toleranceFlags |= GetToleranceFlag(target.Career.Fire);
            if ((effectFlags & DFCareer.EffectFlags.Frost) != 0)
                toleranceFlags |= GetToleranceFlag(target.Career.Frost);
            if ((effectFlags & DFCareer.EffectFlags.Shock) != 0)
                toleranceFlags |= GetToleranceFlag(target.Career.Shock);
            if ((effectFlags & DFCareer.EffectFlags.Disease) != 0)
            {
                toleranceFlags |= GetToleranceFlag(target.Career.Disease);
                if (target == playerEntity)
                    biographyMod += playerEntity.BiographyResistDiseaseMod;
            }

            // Note: Differing from classic implementation here. In classic
            // immune grants always 100% resistance and critical weakness is
            // always 0% resistance if there is no immunity. Here we are using
            // a method that allows mixing different tolerance flags, getting
            // rid of related exploits when creating a character class.
            if ((toleranceFlags & DFCareer.ToleranceFlags.Immune) != 0)
                savingThrow += 50;
            if ((toleranceFlags & DFCareer.ToleranceFlags.CriticalWeakness) != 0)
                savingThrow -= 50;
            if ((toleranceFlags & DFCareer.ToleranceFlags.LowTolerance) != 0)
                savingThrow -= 25;
            if ((toleranceFlags & DFCareer.ToleranceFlags.Resistant) != 0)
                savingThrow += 25;

            savingThrow += biographyMod + modifier;
            if (elementType == DFCareer.Elements.Frost && target == playerEntity && playerEntity.Race == Races.Nord)
                savingThrow += 30;
            else if (elementType == DFCareer.Elements.Magic && target == playerEntity && playerEntity.Race == Races.Breton)
                savingThrow += 30;

            // Handle perfect immunity of 100% or greater
            // Otherwise clamping to 5-95 allows a perfectly immune character to sometimes receive incoming payload
            // This doesn't seem to match immunity intent or player expectations from classic
            if (savingThrow >= 100)
                return 0;

            if (hasMagnitude && spellSchool == DFCareer.MagicSkills.Destruction)
            {
                // Equipment modifier here for addition/reduction of chances of completely negating spell package or not. Will possibly only do this for certain effects and not all.
                if (targetType == TargetTypes.ByTouch || targetType == TargetTypes.SingleTargetAtRange)
                    singlePartHit = true;

                equipSaveThrowMod = EquipmentMaterialSavingThrowMod(elementType, target, singlePartHit, baseAmount);
                savingThrow += equipSaveThrowMod;
            }
            else if (spellSchool != DFCareer.MagicSkills.Restoration) // This is here so that magic effects with no magnitude, but don't do any "healing" effects to the target still get taken into account for spells like paralyze. So adamantium armor will make you more resistant to various magical effects, and iron, etc will make you more likely to be effected by these spells, if you are not outright immune at least.
            {
                // Equipment modifier here for addition/reduction of chances of completely negating spell package or not. Will possibly only do this for certain effects and not all.
                if (targetType == TargetTypes.ByTouch || targetType == TargetTypes.SingleTargetAtRange)
                    singlePartHit = true;

                equipSaveThrowMod = EquipmentMaterialSavingThrowMod(elementType, target, singlePartHit, baseAmount);
                savingThrow += equipSaveThrowMod;
            }

            savingThrow = Mathf.Clamp(savingThrow, 5, 95);

            int percentDamageOrDuration = 100;
            int roll = Dice100.Roll();

            // Handle halving spell magnitude based on magical resistance effects
            if (target.HasResistanceFlag(elementType))
                percentDamageOrDuration = percentDamageOrDuration / 2;

            if (hasMagnitude && spellSchool == DFCareer.MagicSkills.Destruction)
            {
                // Equipment modifier here for addition/reduction of magnitude modifier if a spell package does hit, so basically a multiplier determining how much more or less than 100% a spell with magnitude will do.
                percentDamageOrDuration = Mathf.Clamp(percentDamageOrDuration + (-3 * equipSaveThrowMod), 0, 1000);
                percentDamageOrDuration = (int)Mathf.Round(percentDamageOrDuration * DaggerfallEntity.EntityElementalTypeResistanceCalculator(elementType, target, singlePartHit));
            }

            if (roll <= savingThrow)
            {
                // Percent damage/duration is prorated at within 20 of failed roll, as described in DF Chronicles
                if (savingThrow - 20 <= roll)
                    percentDamageOrDuration = Mathf.Clamp(percentDamageOrDuration - (5 * (savingThrow - roll)), 0, 1000);
                else
                    percentDamageOrDuration = 0;
            }

            return Mathf.Clamp(percentDamageOrDuration, 0, 1000);
        }

        public static int SavingThrow(IEntityEffect sourceEffect, DaggerfallEntity target, int baseAmount = 0)
        {
            if (sourceEffect == null || sourceEffect.ParentBundle == null)
                return 100;
            bool hasMagnitude = false;

            DFCareer.EffectFlags effectFlags = GetEffectFlags(sourceEffect);
            DFCareer.Elements elementType = GetElementType(sourceEffect);
            int modifier = GetResistanceModifier(effectFlags, target);
            TargetTypes targetType = sourceEffect.ParentBundle.targetType;
            hasMagnitude = sourceEffect.Properties.SupportMagnitude;
            DFCareer.MagicSkills spellSchool = sourceEffect.Properties.MagicSkill;

            return SavingThrow(elementType, effectFlags, target, modifier, hasMagnitude, spellSchool, baseAmount, targetType);
        }

        public static int ModifyEffectAmount(IEntityEffect sourceEffect, DaggerfallEntity target, int amount)
        {
            if (sourceEffect == null || sourceEffect.ParentBundle == null)
                return amount;

            int percentDamageOrDuration = SavingThrow(sourceEffect, target, amount);
            float percent = percentDamageOrDuration / 100f;

            return (int)(amount * percent);
        }

        /// <summary>
        /// Gets DFCareer.EffectFlags from an effect.
        /// Note: If effect is not instanced by a bundle then it will not have an element type.
        /// </summary>
        /// <param name="effect">Source effect.</param>
        /// <returns>DFCareer.EffectFlags.</returns>
        public static DFCareer.EffectFlags GetEffectFlags(IEntityEffect effect)
        {
            DFCareer.EffectFlags result = DFCareer.EffectFlags.None;

            // Paralysis/Disease
            if (effect is Paralyze)
                result |= DFCareer.EffectFlags.Paralysis;
            if (effect is DiseaseEffect)
                result |= DFCareer.EffectFlags.Disease;

            // Elemental
            switch (effect.ParentBundle.elementType)
            {
                case ElementTypes.Fire:
                    result |= DFCareer.EffectFlags.Fire;
                    break;
                case ElementTypes.Cold:
                    result |= DFCareer.EffectFlags.Frost;
                    break;
                case ElementTypes.Poison:
                    result |= DFCareer.EffectFlags.Poison;
                    break;
                case ElementTypes.Shock:
                    result |= DFCareer.EffectFlags.Shock;
                    break;
                case ElementTypes.Magic:
                    result |= DFCareer.EffectFlags.Magic;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets a resistance element based on effect element.
        /// </summary>
        /// <param name="effect">Source effect.</param>
        /// <returns>DFCareer.Elements</returns>
        public static DFCareer.Elements GetElementType(IEntityEffect effect)
        {
            // Always return magic for non-elemental (i.e. magic-only) effects
            if (effect.Properties.AllowedElements == ElementTypes.Magic)
                return DFCareer.Elements.Magic;

            // Otherwise return element selected by parent spell bundle
            switch (effect.ParentBundle.elementType)
            {
                case ElementTypes.Fire:
                    return DFCareer.Elements.Fire;
                case ElementTypes.Cold:
                    return DFCareer.Elements.Frost;
                case ElementTypes.Poison:
                    return DFCareer.Elements.DiseaseOrPoison;
                case ElementTypes.Shock:
                    return DFCareer.Elements.Shock;
                case ElementTypes.Magic:
                    return DFCareer.Elements.Magic;
                default:
                    return DFCareer.Elements.None;
            }
        }

        public static int GetResistanceModifier(DFCareer.EffectFlags effectFlags, DaggerfallEntity target)
        {
            int result = 0;

            // Will only read best matching resistance modifier from flags - priority is given to disease/poison over elemental
            // Note disease/poison resistance are both the same here for purposes of saving throw
            if ((effectFlags & DFCareer.EffectFlags.Disease) == DFCareer.EffectFlags.Disease || (effectFlags & DFCareer.EffectFlags.Poison) == DFCareer.EffectFlags.Poison)
                result = target.Resistances.LiveDiseaseOrPoison;
            else if ((effectFlags & DFCareer.EffectFlags.Fire) == DFCareer.EffectFlags.Fire)
                result = target.Resistances.LiveFire;
            else if ((effectFlags & DFCareer.EffectFlags.Frost) == DFCareer.EffectFlags.Frost)
                result = target.Resistances.LiveFrost;
            else if ((effectFlags & DFCareer.EffectFlags.Shock) == DFCareer.EffectFlags.Shock)
                result = target.Resistances.LiveShock;
            else if ((effectFlags & DFCareer.EffectFlags.Magic) == DFCareer.EffectFlags.Magic)
                result = target.Resistances.LiveMagic;

            return result;
        }

        public static int EquipmentMaterialSavingThrowMod(DFCareer.Elements elementType, DaggerfallEntity target, bool singlePartHit, int baseAmount)
        {
            if (singlePartHit)
            {
                // Choose struck body part
                int struckBodyPart = CalculateStruckBodyPart();
                EquipSlots hitSlot = DaggerfallUnityItem.GetEquipSlotForBodyPart((BodyParts)struckBodyPart);
                DaggerfallUnityItem armor = target.ItemEquipTable.GetItem(hitSlot);
                DaggerfallUnityItem shield = target.ItemEquipTable.GetItem(EquipSlots.LeftHand); // I could even add a spell projectile blocking chance/mechanic or something. Perhaps even a spell reflection with some materials?
                if (shield != null && !shield.IsShield)
                    shield = null;

                if (armor != null)
                {
                    int armorSaveThrowMod = 0;
                    int startItemCondPer = armor.ConditionPercentage;
                    switch (elementType) // Damage equipment in the same the values are returned, just before that, using likely the baseAmount multiplied by some amount based on the material properties. 
                    {
                        case DFCareer.Elements.Fire:
                            armorSaveThrowMod = armor.GetSaveThrowModAgainstFire();
                            ApplyConditionDamageThroughMagicDamage(target, armor, singlePartHit, armorSaveThrowMod, baseAmount);
                            if (target == GameManager.Instance.PlayerEntity)
                                WarningMessagePlayerEquipmentCondition(armor, startItemCondPer);
                            return 7 * armorSaveThrowMod;
                        case DFCareer.Elements.Frost:
                            armorSaveThrowMod = armor.GetSaveThrowModAgainstCold();
                            ApplyConditionDamageThroughMagicDamage(target, armor, singlePartHit, armorSaveThrowMod, baseAmount);
                            if (target == GameManager.Instance.PlayerEntity)
                                WarningMessagePlayerEquipmentCondition(armor, startItemCondPer);
                            return 7 * armorSaveThrowMod;
                        case DFCareer.Elements.Magic:
                            armorSaveThrowMod = armor.GetSaveThrowModAgainstMagic();
                            ApplyConditionDamageThroughMagicDamage(target, armor, singlePartHit, armorSaveThrowMod, baseAmount);
                            if (target == GameManager.Instance.PlayerEntity)
                                WarningMessagePlayerEquipmentCondition(armor, startItemCondPer);
                            return 7 * armorSaveThrowMod;
                        case DFCareer.Elements.Shock:
                            armorSaveThrowMod = armor.GetSaveThrowModAgainstShock();
                            ApplyConditionDamageThroughMagicDamage(target, armor, singlePartHit, armorSaveThrowMod, baseAmount);
                            if (target == GameManager.Instance.PlayerEntity)
                                WarningMessagePlayerEquipmentCondition(armor, startItemCondPer);
                            return 7 * armorSaveThrowMod;
                        default:
                            return 0;
                    }
                }
                else
                    return 0;
            }
            else
            {
                DaggerfallUnityItem[] equipment = { target.ItemEquipTable.GetItem(EquipSlots.Head), target.ItemEquipTable.GetItem(EquipSlots.RightArm), target.ItemEquipTable.GetItem(EquipSlots.LeftArm),
                target.ItemEquipTable.GetItem(EquipSlots.ChestArmor), target.ItemEquipTable.GetItem(EquipSlots.Gloves), target.ItemEquipTable.GetItem(EquipSlots.LegsArmor),
                target.ItemEquipTable.GetItem(EquipSlots.Feet), target.ItemEquipTable.GetItem(EquipSlots.RightHand), target.ItemEquipTable.GetItem(EquipSlots.LeftHand)};

                return GetSaveThrowModAgainstAOESpell(elementType, equipment, target, baseAmount);
            }
        }

        public static int GetSaveThrowModAgainstAOESpell(DFCareer.Elements elementType, DaggerfallUnityItem[] equipment, DaggerfallEntity owner, int baseAmount)
        {
            int itemSaveThrowMod = 0;
            float finalSaveThrowMod = 0f;

            if (elementType == DFCareer.Elements.Fire)
            {
                for (int i = 0; i < equipment.Length; i++)
                {
                    if (equipment[i] != null)
                    {
                        int startItemCondPer = equipment[i].ConditionPercentage;
                        itemSaveThrowMod = equipment[i].GetSaveThrowModAgainstFire();
                        ApplyConditionDamageThroughMagicDamage(owner, equipment[i], false, itemSaveThrowMod, baseAmount);
                        if (owner == GameManager.Instance.PlayerEntity)
                            WarningMessagePlayerEquipmentCondition(equipment[i], startItemCondPer);
                        finalSaveThrowMod += 1f * itemSaveThrowMod;
                    }
                }
            }
            else if (elementType == DFCareer.Elements.Frost)
            {
                for (int i = 0; i < equipment.Length; i++)
                {
                    if (equipment[i] != null)
                    {
                        int startItemCondPer = equipment[i].ConditionPercentage;
                        itemSaveThrowMod = equipment[i].GetSaveThrowModAgainstCold();
                        ApplyConditionDamageThroughMagicDamage(owner, equipment[i], false, itemSaveThrowMod, baseAmount);
                        if (owner == GameManager.Instance.PlayerEntity)
                            WarningMessagePlayerEquipmentCondition(equipment[i], startItemCondPer);
                        finalSaveThrowMod += 1f * itemSaveThrowMod;
                    }
                }
            }
            else if (elementType == DFCareer.Elements.Shock)
            {
                for (int i = 0; i < equipment.Length; i++)
                {
                    if (equipment[i] != null)
                    {
                        int startItemCondPer = equipment[i].ConditionPercentage;
                        itemSaveThrowMod = equipment[i].GetSaveThrowModAgainstShock();
                        ApplyConditionDamageThroughMagicDamage(owner, equipment[i], false, itemSaveThrowMod, baseAmount);
                        if (owner == GameManager.Instance.PlayerEntity)
                            WarningMessagePlayerEquipmentCondition(equipment[i], startItemCondPer);
                        finalSaveThrowMod += 1f * itemSaveThrowMod;
                    }
                }
            }
            else if (elementType == DFCareer.Elements.Magic)
            {
                for (int i = 0; i < equipment.Length; i++)
                {
                    if (equipment[i] != null)
                    {
                        int startItemCondPer = equipment[i].ConditionPercentage;
                        itemSaveThrowMod = equipment[i].GetSaveThrowModAgainstMagic();
                        ApplyConditionDamageThroughMagicDamage(owner, equipment[i], false, itemSaveThrowMod, baseAmount);
                        if (owner == GameManager.Instance.PlayerEntity)
                            WarningMessagePlayerEquipmentCondition(equipment[i], startItemCondPer);
                        finalSaveThrowMod += 1f * itemSaveThrowMod;
                    }
                }
            }
            return (int)Mathf.Round(finalSaveThrowMod);
        }

        /// Applies condition damage to an item based on magic damage.
        public static void ApplyConditionDamageThroughMagicDamage(DaggerfallEntity owner, DaggerfallUnityItem item, bool singlePartHit, int armorSaveThrowMod, int baseAmount)
        {
            if (item == null || owner == null)
                return;

            ItemCollection playerItems = GameManager.Instance.PlayerEntity.Items;
            int conditionDamValue = 0;

            if (singlePartHit)
            {
                conditionDamValue = (int)Mathf.Round(baseAmount * ((-armorSaveThrowMod * 0.24f) + 1));
                //Debug.Log("Starting Magnitude of Spell = " + baseAmount);
                //Debug.Log("Spell Magnitude After Armor Resist Mod = " + conditionDamValue);

                if (conditionDamValue > 0)
                {
                    if (owner == GameManager.Instance.PlayerEntity && item.IsEnchanted) // If the Weapon or Armor piece is enchanted, when broken it will be Destroyed from the player inventory.
                        item.LowerCondition(conditionDamValue, owner, playerItems);
                    else
                        item.LowerCondition(conditionDamValue, owner);

                    /*int percentChange = 100 * conditionDamValue / item.maxCondition;
                    if (owner == GameManager.Instance.PlayerEntity){
                        Debug.LogFormat("Target Had {0} Damaged by {1}, cond={2}", item.LongName, conditionDamValue, item.currentCondition);
                        Debug.LogFormat("Had {0} Damaged by {1}%, of Total Maximum. There Remains {2}% of Max Cond.", item.LongName, percentChange, item.ConditionPercentage);} // Percentage Change */
                }
            }
            else
            {
                conditionDamValue = (int)Mathf.Round(baseAmount * ((-armorSaveThrowMod * 0.24f) + 1) / 5);
                //Debug.Log("Starting Magnitude of Spell = " + baseAmount);
                //Debug.Log("Spell Magnitude After Armor Resist Mod = " + conditionDamValue);

                if (conditionDamValue > 0)
                {
                    if (owner == GameManager.Instance.PlayerEntity && item.IsEnchanted) // If the Weapon or Armor piece is enchanted, when broken it will be Destroyed from the player inventory.
                        item.LowerCondition(conditionDamValue, owner, playerItems);
                    else
                        item.LowerCondition(conditionDamValue, owner);

                    /*int percentChange = 100 * conditionDamValue / item.maxCondition;
                    if (owner == GameManager.Instance.PlayerEntity){
                        Debug.LogFormat("Target Had {0} Damaged by {1}, cond={2}", item.LongName, conditionDamValue, item.currentCondition);
                        Debug.LogFormat("Had {0} Damaged by {1}%, of Total Maximum. There Remains {2}% of Max Cond.", item.LongName, percentChange, item.ConditionPercentage);} // Percentage Change */
                }
            }
        }

            #endregion

            #region Enemies

        /// <summary>
        /// Inflict a classic disease onto player.
        /// </summary>
        /// <param name="target">Target entity - must be player.</param>
        /// <param name="diseaseList">Array of disease indices matching Diseases enum.</param>
        public static void InflictDisease(DaggerfallEntity target, byte[] diseaseList)
        {
            // Must have a valid disease list
            if (diseaseList == null || diseaseList.Length == 0 || target.EntityBehaviour.EntityType != EntityTypes.Player)
                return;

            // Only allow player to catch a disease this way
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            if (target != playerEntity)
                return;

            // Return if disease resisted
            if (SavingThrow(DFCareer.Elements.DiseaseOrPoison, DFCareer.EffectFlags.Disease, target, 0) == 0)
                return;

            // Select a random disease from disease array and validate range
            int diseaseIndex = UnityEngine.Random.Range(0, diseaseList.Length);
            if (diseaseIndex < 0 || diseaseIndex > 16)
                return;

            // Infect player
            Diseases diseaseType = (Diseases)diseaseList[diseaseIndex];
            EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateDisease(diseaseType);
            GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.BypassSavingThrows);

            Debug.LogFormat("Infected player with disease {0}", diseaseType.ToString());
        }

        public static void InflictCustomDisease(DaggerfallEntity target, Diseases disease, bool bypassResistance = false)
        {
            // Target must be player
            if (target.EntityBehaviour.EntityType != EntityTypes.Player)
                return;

            // Only allow player to catch a disease this way
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            if (target != playerEntity)
                return;

            if (!bypassResistance)
            {
                // Return if disease resisted
                if (SavingThrow(DFCareer.Elements.DiseaseOrPoison, DFCareer.EffectFlags.Disease, target, 0) == 0)
                    return;
            }

            // Infect player
            EntityEffectBundle bundle = GameManager.Instance.PlayerEffectManager.CreateDisease(disease);
            GameManager.Instance.PlayerEffectManager.AssignBundle(bundle, AssignBundleFlags.BypassSavingThrows);

            Debug.LogFormat("Infected player with disease {0}", disease.ToString());
        }

        public static void FatigueDamage(DaggerfallEntity target, int damage)
        {
            // In classic, nymphs do 10-30 fatigue damage per hit, and lamias don't do any.
            // DF Chronicles says nymphs have "Energy Leech", which is a spell in
            // the game and not what they use, and for lamias "Every 1 pt of health damage = 2 pts of fatigue damage".
            // Lamia health damage is 5-15. Multiplying this by 2 may be where 10-30 came from. Nymph health damage is 1-5.
            // Not sure what was intended here, but using the "Every 1 pt of health damage = 2 pts of fatigue damage"
            // seems sensible, since the fatigue damage will scale to the health damage and lamias are a higher level opponent
            // than nymphs and will do more fatigue damage.
            target.SetFatigue(target.CurrentFatigue - ((damage * 2) * 64));

            // TODO: When nymphs drain the player's fatigue level to 0, the player is supposed to fall asleep for 14 days
            // and then wake up, according to DF Chronicles. This doesn't work correctly in classic. Classic does advance
            // time 14 days but the player dies like normal because of the "collapse from exhaustion near monsters = die" code.
        }

        // Generates health for enemy classes based on level and class
        public static int RollEnemyClassMaxHealth(int level, int hitPointsPerLevel)
        {
            Func<int, int, int> del;
            if (TryGetOverride("RollEnemyClassMaxHealth", out del))
                return del(level, hitPointsPerLevel);

            const int baseHealth = 20; // Increased from 10 to 20, since I felt that 10 was far to low at least for a base-line for all human enemy classes.
            int maxHealth = baseHealth;

            for (int i = 0; i < level; i++)
            {
                maxHealth += UnityEngine.Random.Range(1, hitPointsPerLevel + 1);
            }
            return maxHealth;
        }

        /// <summary>
        /// Roll for random spawn in location area at night.
        /// </summary>
        /// <returns>0 to generate a spawn. >0 to not generate a spawn.</returns>
        public static int RollRandomSpawn_LocationNight()
        {
            Func<int> del;
            if (TryGetOverride("RollRandomSpawn_LocationNight", out del))
                return del();
            else
                return UnityEngine.Random.Range(0, 24);
        }

        /// <summary>
        /// Roll for random spawn in wilderness during daylight hours.
        /// </summary>
        /// <returns>0 to generate a spawn. >0 to not generate a spawn.</returns>
        public static int RollRandomSpawn_WildernessDay()
        {
            Func<int> del;
            if (TryGetOverride("RollRandomSpawn_WildernessDay", out del))
                return del();
            else
                return UnityEngine.Random.Range(0, 36);
        }

        /// <summary>
        /// Roll for random spawn in wilderness at night.
        /// </summary>
        /// <returns>0 to generate a spawn. >0 to not generate a spawn.</returns>
        public static int RollRandomSpawn_WildernessNight()
        {
            Func<int> del;
            if (TryGetOverride("RollRandomSpawn_WildernessNight", out del))
                return del();
            else
                return UnityEngine.Random.Range(0, 24);
        }

        /// <summary>
        /// Roll for random spawn in dungeons.
        /// </summary>
        /// <returns>0 to generate a spawn. >0 to not generate a spawn.</returns>
        public static int RollRandomSpawn_Dungeon()
        {
            Func<int> del;
            if (TryGetOverride("RollRandomSpawn_Dungeon", out del))
                return del();
            else if (GameManager.Instance.PlayerEntity.EnemyAlertActive)
                return UnityEngine.Random.Range(0, 36);

            return 1; // >0 is do not generate a spawn
        }

        #endregion

        #region Holidays & Conversation

        public static int GetHolidayId(uint gameMinutes, int regionIndex)
        {
            // Gives which regions celebrate which holidays.
            // Values are region IDs, index is holiday ID. 0xFF means all regions celebrate the holiday.
            byte[] regionIndexCelebratingHoliday = { 0xFF, 0x19, 0x01, 0xFF, 0x1D, 0x05, 0x19, 0x06, 0x3C, 0xFF, 0x29, 0x1A,
                0xFF, 0x02, 0x19, 0x01, 0x0E, 0x12, 0x14, 0xFF, 0xFF, 0x1C, 0x21, 0x1F, 0x2C, 0xFF, 0x12,
                0x23, 0xFF, 0x38, 0xFF, 0x01, 0x30, 0x29, 0x0B, 0x16, 0xFF, 0xFF, 0x11, 0x17, 0x14, 0x01,
                0xFF, 0x13, 0xFF, 0x33, 0x3C, 0x2E, 0xFF, 0xFF, 0x01, 0x2D, 0x18 };

            // Gives the day of the year that holidays are celebrated on.
            // Value are days of the year, index is holiday ID.
            short[] holidayDaysOfYear = { 0x01, 0x02, 0x0C, 0x0F, 0x10, 0x12, 0x20, 0x23, 0x26, 0x2E, 0x39, 0x3A,
                0x43, 0x45, 0x55, 0x56, 0x5B, 0x67, 0x6E, 0x76, 0x7F, 0x81, 0x8C, 0x96, 0x97, 0xA6, 0xAD,
                0xAE, 0xBE, 0xC0, 0xC8, 0xD1, 0xD4, 0xDD, 0xE0, 0xE7, 0xED, 0xF3, 0xF6, 0xFC, 0x103, 0x113,
                0x11B, 0x125, 0x12C, 0x12F, 0x134, 0x13E, 0x140, 0x159, 0x15C, 0x162, 0x163 };

            int holidayID = 0;
            uint dayOfYear = gameMinutes % 518400 / 1440 + 1;
            if (dayOfYear <= 355)
            {
                while (holidayID < 53)
                {
                    if ((regionIndexCelebratingHoliday[holidayID] == 0xFF || regionIndexCelebratingHoliday[holidayID] == regionIndex + 1)
                        && dayOfYear == holidayDaysOfYear[holidayID])
                    {
                        return holidayID + 1;
                    }
                    ++holidayID;
                }
            }

            // Not a holiday
            return 0;
        }

        public static float BonusChanceToKnowWhereIs(float bonusPerBlockLess = 0.0078f)
        {
            const int maxArea = 64;

            // Must be in a location
            if (!GameManager.Instance.PlayerGPS.HasCurrentLocation)
                return 0;

            // Get area of current location
            DFLocation location = GameManager.Instance.PlayerGPS.CurrentLocation;
            int locationArea = location.Exterior.ExteriorData.Width * location.Exterior.ExteriorData.Height;

            // The largest possible location has an area of 64 (e.g. Daggerfall/Wayrest/Sentinel)
            // The smallest possible location has an area of 1 (e.g. a tavern town)
            // In a big city NPCs could be ignorant of all buildings, but in a small town it's unlikely they don't know the local tavern or smith
            // So we apply a bonus that INCREASES the more city area size DECREASES
            // With default inputs, a tiny 1x1 town NPC will get a +0.4914 to the default 0.5 chance for a total of 0.9914 chance to know building
            // This is a big help as small towns also have less NPCs, and it gets frustrating when multiple NPCs don't knows where something is
            float bonus = (maxArea - locationArea) * bonusPerBlockLess;

            return bonus;
        }

        #endregion

        #region Commerce

        public static int CalculateRoomCost(int daysToRent)
        {
            Func<int, int> del;
            if (TryGetOverride("CalculateRoomCost", out del))
                return del(daysToRent);

            int cost = 0;
            int dayOfYear = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.DayOfYear;
            if (dayOfYear <= 46 && dayOfYear + daysToRent > 46)
                cost = 7 * (daysToRent - 1);  // No charge for Heart's Day
            else
                cost = 7 * daysToRent;

            if (cost == 0) // Only renting for Heart's Day
                DaggerfallUI.MessageBox(TextManager.Instance.GetLocalizedText("roomFreeDueToHeartsDay"));

            return cost;
        }

        /// <summary>
        /// Calculate the cost of something in a given shop.
        /// </summary>
        /// <param name="baseValue">Base value</param>
        /// <param name="shopQuality">Shop quality 0-20</param>
        /// <param name="conditionPercentage">Condition of item as a percentage, -1 indicates condition not applicable</param>
        /// <returns>Shop specific cost</returns>
        public static int CalculateCost(int baseValue, int shopQuality, int conditionPercentage = -1)
        {
            Func<int, int, int, int> del;
            if (TryGetOverride("CalculateCost", out del))
                return del(baseValue, shopQuality, conditionPercentage);

            int cost = baseValue;

            if (cost < 1)
                cost = 1;

            cost = ApplyRegionalPriceAdjustment(cost);
            cost = 2 * (cost * (shopQuality - 10) / 100 + cost);

            return cost;
        }

        public static int CalculateItemIngotMaterial(DaggerfallUnityItem item)
        {
            if (item.ItemGroup == ItemGroups.Weapons || item.ItemGroup == ItemGroups.Armor || (item.ItemGroup == ItemGroups.UselessItems2 && item.TemplateIndex == 810))
            {
                switch(item.nativeMaterialValue)
                {
                    case (int)WeaponMaterialTypes.Iron:
                    case (int)ArmorMaterialTypes.Iron:
                        if (item.ItemGroup == ItemGroups.Armor && item.nativeMaterialValue == (int)ArmorMaterialTypes.Leather)
                            return -1;
                        else
                            return 0;
                    case (int)WeaponMaterialTypes.Steel:
                    case (int)ArmorMaterialTypes.Steel:
                        return 1;
                    case (int)WeaponMaterialTypes.Silver:
                    case (int)ArmorMaterialTypes.Silver:
                        return 2;
                    case (int)WeaponMaterialTypes.Elven:
                    case (int)ArmorMaterialTypes.Elven:
                        return 3;
                    case (int)WeaponMaterialTypes.Dwarven:
                    case (int)ArmorMaterialTypes.Dwarven:
                        return 4;
                    case (int)WeaponMaterialTypes.Mithril:
                    case (int)ArmorMaterialTypes.Mithril:
                        return 5;
                    case (int)WeaponMaterialTypes.Adamantium:
                    case (int)ArmorMaterialTypes.Adamantium:
                        return 6;
                    case (int)WeaponMaterialTypes.Ebony:
                    case (int)ArmorMaterialTypes.Ebony:
                        return 7;
                    case (int)WeaponMaterialTypes.Orcish:
                    case (int)ArmorMaterialTypes.Orcish:
                        return 8;
                    case (int)WeaponMaterialTypes.Daedric:
                    case (int)ArmorMaterialTypes.Daedric:
                        return 9;
                    default:
                        return -1;
                }
            }
            return -1;
        }

        public static int CalculateItemIngotCost(DaggerfallUnityItem item)
        {
            if (item.ItemGroup == ItemGroups.Weapons)
            {
                switch(item.TemplateIndex)
                {
                    case (int)Weapons.Dagger:
                    case (int)Weapons.Tanto:
                        if (item.ConditionPercentage <= 40)
                            return 1;
                        else
                            return 0;
                    case (int)Weapons.Shortsword:
                    case (int)Weapons.Wakazashi:
                    case (int)Weapons.Staff:
                    case (int)Weapons.Short_Bow:
                    case (int)Weapons.Long_Bow:
                        if (item.ConditionPercentage <= 55)
                            return 1;
                        else
                            return 0;
                    case (int)Weapons.Broadsword:
                    case (int)Weapons.Katana:
                    case (int)Weapons.Longsword:
                    case (int)Weapons.Saber:
                    case (int)Weapons.Battle_Axe:
                    case (int)Weapons.Mace:
                        if (item.ConditionPercentage >= 76)
                            return 0;
                        else if (item.ConditionPercentage <= 75 && item.ConditionPercentage >= 41)
                            return 1;
                        else
                            return 2;
                    case (int)Weapons.Claymore:
                    case (int)Weapons.Dai_Katana:
                    case (int)Weapons.War_Axe:
                    case (int)Weapons.Flail:
                    case (int)Weapons.Warhammer:
                        if (item.ConditionPercentage >= 76)
                            return 0;
                        else if (item.ConditionPercentage <= 75 && item.ConditionPercentage >= 61)
                            return 1;
                        else if (item.ConditionPercentage <= 60 && item.ConditionPercentage >= 41)
                            return 2;
                        else
                            return 3;
                    default:
                        return 0;
                }
            }
            else if (item.ItemGroup == ItemGroups.Armor)
            {
                switch(item.TemplateIndex)
                {
                    case (int)Armor.Helm:
                    case (int)Armor.Right_Pauldron:
                    case (int)Armor.Left_Pauldron:
                    case (int)Armor.Gauntlets:
                    case (int)Armor.Boots:
                    case (int)Armor.Buckler:
                        if (item.ConditionPercentage >= 61)
                            return 0;
                        else if (item.ConditionPercentage <= 60 && item.ConditionPercentage >= 41)
                            return 1;
                        else
                            return 2;
                    case (int)Armor.Cuirass:
                    case (int)Armor.Greaves:
                    case (int)Armor.Round_Shield:
                    case (int)Armor.Kite_Shield:
                        if (item.ConditionPercentage >= 61)
                            return 0;
                        else if (item.ConditionPercentage <= 60 && item.ConditionPercentage >= 41)
                            return 1;
                        else if (item.ConditionPercentage <= 40 && item.ConditionPercentage >= 16)
                            return 2;
                        else
                            return 3;
                    case (int)Armor.Tower_Shield:
                        if (item.ConditionPercentage >= 76)
                            return 0;
                        else if (item.ConditionPercentage <= 75 && item.ConditionPercentage >= 61)
                            return 1;
                        else if (item.ConditionPercentage <= 60 && item.ConditionPercentage >= 41)
                            return 2;
                        else if (item.ConditionPercentage <= 40 && item.ConditionPercentage >= 16)
                            return 3;
                        else
                            return 4;
                    default:
                        return 0;
                }
            }
            return 0;
        }

        public static int CalculateItemRepairCost(int baseItemValue, int shopQuality, int condition, int max, IGuild guild)
        {
            Func<int, int, int, int, IGuild, int> del;
            if (TryGetOverride("CalculateItemRepairCost", out del))
                return del(baseItemValue, shopQuality, condition, max, guild);

            // Don't cost already repaired item
            if (condition == max)
                return 0;

            int cost = 10 * baseItemValue / 100;

            if (cost < 1)
                cost = 1;

            cost = CalculateCost(cost, shopQuality);

            if (guild != null)
                cost = guild.ReducedRepairCost(cost);

            return cost;
        }

        public static int CalculateItemRepairTime(int condition, int max)
        {
            int damage = max - condition;
            int repairTime = (damage * DaggerfallDateTime.SecondsPerHour / 20); // This seems to work for now. I obviously want to do a lot more to this value as well later on. Such as factoring in simulated "busyness" of the smith to increase or decrease repair times. Also change times based on different factors like the rarity/exoticness of the material as well as location such as smiths in the Orsinium area having shorter repair times or something like that, etc.
            return Mathf.Max(repairTime, DaggerfallDateTime.SecondsPerHour);
        }

        public static int CalculateItemIdentifyCost(int baseItemValue, IGuild guild)
        {
            // Free on Witches Festival
            uint minutes = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            if (gps.HasCurrentLocation)
            {
                int holidayId = GetHolidayId(minutes, gps.CurrentRegionIndex);
                if (holidayId == (int)DFLocation.Holidays.Witches_Festival)
                    return 0;
            }
            int cost = (25 * baseItemValue) >> 8;

            if (guild != null)
                cost = guild.ReducedIdentifyCost(cost);

            return cost;
        }

        public static int CalculateDaedraSummoningCost(int npcRep)
        {
            Func<int, int> del;
            if (TryGetOverride("CalculateDaedraSummoningCost", out del))
                return del(npcRep);

            return 200000 - (npcRep * 1000);
        }

        public static int CalculateDaedraSummoningChance(int daedraRep, int bonus)
        {
            Func<int, int, int> del;
            if (TryGetOverride("CalculateDaedraSummoningChance", out del))
                return del(daedraRep, bonus);

            return 30 + daedraRep + bonus;
        }

        public static int CalculateTradePrice(int cost, int shopQuality, bool selling)
        {
            Func<int, int, bool, int> del;
            if (TryGetOverride("CalculateTradePrice", out del))
                return del(cost, shopQuality, selling);

            PlayerEntity player = GameManager.Instance.PlayerEntity;
            int merchant_mercantile_level = 5 * (shopQuality - 10) + 50;
            int merchant_personality_level = 5 * (shopQuality - 10) + 50;

            int delta_mercantile;
            int delta_personality;
            int amount = 0;

            if (selling)
            {
                delta_mercantile = (((100 - merchant_mercantile_level) << 8) / 200 + 128) * (((player.Skills.GetLiveSkillValue(DFCareer.Skills.Mercantile)) << 8) / 200 + 128) >> 8;
                delta_personality = (((100 - merchant_personality_level) << 8) / 200 + 128) * ((player.Stats.LivePersonality << 8) / 200 + 128) >> 8;
                amount = ((((179 * delta_mercantile) >> 8) + ((51 * delta_personality) >> 8)) * cost) >> 8;
            }
            else // buying
            {
                delta_mercantile = ((merchant_mercantile_level << 8) / 200 + 128) * (((100 - (player.Skills.GetLiveSkillValue(DFCareer.Skills.Mercantile))) << 8) / 200 + 128) >> 8;
                delta_personality = ((merchant_personality_level << 8) / 200 + 128) * (((100 - player.Stats.LivePersonality) << 8) / 200 + 128) >> 8 << 6;
                amount = ((((192 * delta_mercantile) >> 8) + (delta_personality >> 8)) * cost) >> 8;
            }

            return amount;
        }

        public static int ApplyRegionalPriceAdjustment(int cost)
        {
            Func<int, int> del;
            if (TryGetOverride("ApplyRegionalPriceAdjustment", out del))
                return del(cost);

            int adjustedCost;
            int currentRegionIndex;
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            if (gps.HasCurrentLocation)
                currentRegionIndex = gps.CurrentRegionIndex;
            else
                return cost;

            adjustedCost = cost * player.RegionData[currentRegionIndex].PriceAdjustment / 1000;
            if (adjustedCost < 1)
                adjustedCost = 1;
            return adjustedCost;
        }

        public static void RandomizeInitialRegionalPrices(ref PlayerEntity.RegionDataRecord[] regionData)
        {
            for (int i = 0; i < regionData.Length; i++)
                regionData[i].PriceAdjustment = (ushort)(UnityEngine.Random.Range(0, 500 + 1) + 750);
        }

        public static void UpdateRegionalPrices(ref PlayerEntity.RegionDataRecord[] regionData, int times)
        {
            PlayerEntity player = GameManager.Instance.PlayerEntity;
            FactionFile.FactionData merchantsFaction;
            if (!player.FactionData.GetFactionData((int)FactionFile.FactionIDs.The_Merchants, out merchantsFaction))
                return;

            for (int i = 0; i < regionData.Length; ++i)
            {
                FactionFile.FactionData regionFaction;
                if (player.FactionData.FindFactionByTypeAndRegion(7, i, out regionFaction))
                {
                    for (int j = 0; j < times; ++j)
                    {
                        int chanceOfPriceRise = ((merchantsFaction.power) - (regionFaction.power)) / 5
                            + 50 - (regionData[i].PriceAdjustment - 1000) / 25;
                        if (Dice100.FailedRoll(chanceOfPriceRise))
                            regionData[i].PriceAdjustment = (ushort)(49 * regionData[i].PriceAdjustment / 50);
                        else
                            regionData[i].PriceAdjustment = (ushort)(51 * regionData[i].PriceAdjustment / 50);

                        Mathf.Clamp(regionData[i].PriceAdjustment, 250, 4000);
                        if (regionData[i].PriceAdjustment <= 2000)
                        {
                            if (regionData[i].PriceAdjustment >= 500)
                            {
                                player.TurnOffConditionFlag(i, PlayerEntity.RegionDataFlags.PricesHigh);
                                player.TurnOffConditionFlag(i, PlayerEntity.RegionDataFlags.PricesLow);
                            }
                            else
                                player.TurnOnConditionFlag(i, PlayerEntity.RegionDataFlags.PricesLow);
                        }
                        else
                            player.TurnOnConditionFlag(i, PlayerEntity.RegionDataFlags.PricesHigh);
                    }
                }
            }
        }

        #endregion

        #region Items

        public static bool IsItemStackable(DaggerfallUnityItem item)
        {
            Func<DaggerfallUnityItem, bool> del;
            if (TryGetOverride("IsItemStackable", out del))
                if (del(item))
                    return true; // Only return if override returns true

            if (item.IsIngredient || item.IsPotion || item.IsBook || item.IsDrug ||
                item.IsOfTemplate(ItemGroups.Currency, (int)Currency.Gold_pieces) ||
                item.IsOfTemplate(ItemGroups.Weapons, (int)Weapons.Arrow) ||
                item.IsOfTemplate(ItemGroups.UselessItems2, (int)UselessItems2.Oil) ||
                item.IsOfTemplate(ItemGroups.UselessItems2, 810)) // Template Index of my custom Ingot items are 810
                return true;
            else
                return false;
        }

        /// <summary>
        /// Allows loot found in containers and enemy corpses to be modified.
        /// </summary>
        /// <param name="lootItems">An array of the loot items</param>
        /// <returns>The number of items modified.</returns>
        public static int ModifyFoundLootItems(ref DaggerfallUnityItem[] lootItems)
        {
            Func<DaggerfallUnityItem[], int> del;
            if (TryGetOverride("ModifyFoundLootItems", out del))
                return del(lootItems);

            // DFU does no post-processing of loot items hence report zero changes, this is solely for mods to override.
            return 0;
        }

        /*public static WeaponMaterialTypes RandomMaterial(int enemyLevel = -1, int buildingQuality = -1, int playerLuck = -1)
        {
            float[] regionMods = PlayerGPS.RegionMaterialSupplyCreator();
            float[] enemyLevelLootMods = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            float[] buildingQualityLootMods = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            float[] playerLuckLootMods = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            int [] matRolls = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            if (enemyLevel > 1)
            {
                int eLev = enemyLevel;
                enemyLevelLootMods = new float[] { 1, 0.03f*eLev+1, 1, 1, 0.03f*eLev+1, 0.03f*eLev+1, 0.02f*eLev+1, 0.02f*eLev+1, 0.02f*eLev+1, 0.01f*eLev+1 };
            }

            if (buildingQuality > 1)
            {
                int bQ = buildingQuality;
                buildingQualityLootMods = new float[] { 1, 0.05f*bQ+1, 1, 1, 0.05f*bQ+1, 0.05f*bQ+1, 0.03f*bQ+1, 0.03f*bQ+1, 0.03f*bQ+1, 0.02f*bQ+1 };
            }

            if (playerLuck != 0)
            {
                int pL = playerLuck;
                if (pL < 50)
                {
                    pL -= 50;
                    pL = Mathf.Clamp((int)Mathf.Round(pL / 5), -10, -1) * -1;
                    playerLuckLootMods = new float[] { 0.05f*pL+1, -0.02f*pL+1, 0.05f*pL+1, 0.05f*pL+1, -0.02f*pL+1, -0.02f*pL+1, -0.04f*pL+1, -0.04f*pL+1, -0.04f*pL+1, -0.06f*pL+1 };
                }
                else if (pL > 50)
                {
                    pL -= 50;
                    pL = Mathf.Clamp((int)Mathf.Round(pL / 5), 1, 10);
                    playerLuckLootMods = new float[] { -0.02f*pL+1, 0.03f*pL+1, -0.02f*pL+1, -0.02f*pL+1, 0.03f*pL+1, 0.03f*pL+1, 0.02f*pL+1, 0.02f*pL+1, 0.02f*pL+1, 0.01f*pL+1 };
                }
            }

            for (int i = 0; i < ItemBuilder.materialsByRarity.Length; i++)
            {
                int randomModifier = UnityEngine.Random.Range(0, (int)Mathf.Ceil(ItemBuilder.materialsByRarity[i] * regionMods[i] * enemyLevelLootMods[i] * buildingQualityLootMods[i] * playerLuckLootMods[i]));
                randomModifier = Mathf.Clamp(randomModifier, 1, 100);
                matRolls[i] = randomModifier;
            }

            int max = matRolls[0];
            int index = 0;

            for (int i = 0; i < matRolls.Length; i++)
            {
                if (index == i)
                    continue;

                if (max == matRolls[i])
                {
                    int coinFlip = Dice100.Roll();
                    if (coinFlip <= 50)
                        continue;
                    else
                    {
                        max = matRolls[i];
                        index = i;
                        continue;
                    }
                }

                if (max < matRolls[i])
                {
                    max = matRolls[i];
                    index = i;
                }
            }

            return (WeaponMaterialTypes)index;
        }*/

        public static WeaponMaterialTypes RandomMaterial(int enemyLevel = -1, int buildingQuality = -1, int playerLuck = -1)
        {
            float[] regionMods = PlayerGPS.RegionMaterialSupplyCreator();
            float[] enemyLevelLootMods = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            float[] buildingQualityLootMods = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            float[] playerLuckLootMods = new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
            int[] matRolls = new int[] { };
            List<int> matRollsList = new List<int>();

            if (enemyLevel > 1)
            {
                int eLev = enemyLevel;
                enemyLevelLootMods = new float[] { 1, 0.03f * eLev + 1, 1, 1, 0.03f * eLev + 1, 0.03f * eLev + 1, 0.02f * eLev + 1, 0.02f * eLev + 1, 0.02f * eLev + 1, 0.01f * eLev + 1 };
            }

            if (buildingQuality > 1)
            {
                int bQ = buildingQuality;
                buildingQualityLootMods = new float[] { 1, 0.05f * bQ + 1, 1, 1, 0.05f * bQ + 1, 0.05f * bQ + 1, 0.03f * bQ + 1, 0.03f * bQ + 1, 0.03f * bQ + 1, 0.02f * bQ + 1 };
            }

            if (playerLuck != 0)
            {
                int pL = playerLuck;
                if (pL < 50)
                {
                    pL -= 50;
                    pL = Mathf.Clamp((int)Mathf.Round(pL / 5), -10, -1) * -1;
                    playerLuckLootMods = new float[] { 0.05f * pL + 1, -0.02f * pL + 1, 0.05f * pL + 1, 0.05f * pL + 1, -0.02f * pL + 1, -0.02f * pL + 1, -0.04f * pL + 1, -0.04f * pL + 1, -0.04f * pL + 1, -0.06f * pL + 1 };
                }
                else if (pL > 50)
                {
                    pL -= 50;
                    pL = Mathf.Clamp((int)Mathf.Round(pL / 5), 1, 10);
                    playerLuckLootMods = new float[] { -0.02f * pL + 1, 0.03f * pL + 1, -0.02f * pL + 1, -0.02f * pL + 1, 0.03f * pL + 1, 0.03f * pL + 1, 0.02f * pL + 1, 0.02f * pL + 1, 0.02f * pL + 1, 0.01f * pL + 1 };
                }
            }

            for (int i = 0; i < ItemBuilder.materialsByRarity.Length; i++)
            {
                int arraystart = matRollsList.Count;
                int fillElements = (int)Mathf.Ceil(ItemBuilder.materialsByRarity[i] * regionMods[i] * enemyLevelLootMods[i] * buildingQualityLootMods[i] * playerLuckLootMods[i]);
                matRolls = FillArray(matRollsList, arraystart, fillElements, i);
            }

            return (WeaponMaterialTypes)PickOneOf(matRolls);
        }

        public static T[] FillArray<T>(List<T> list, int start, int count, T value)
        {
            for (var i = start; i < start + count; i++)
            {
                list.Add(value);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Gets a random armor material based on player level.
        /// </summary>
        /// <param name="playerLevel">Player level, possibly adjusted.</param>
        /// <returns>ArmorMaterialTypes value of material selected.</returns>
        public static ArmorMaterialTypes RandomArmorMaterial(int enemyLevel = -1, int buildingQuality = -1, int playerLuck = -1, int armorType = -1)
        {
            if (armorType == -1)
            {
                armorType = PickOneOf(0, 1, 2); // I'll likely want to do a more complicated system for this, but for now it's simple and will work for this purpose. 
            }

            if (armorType == 0)
            {
                return ArmorMaterialTypes.Leather;
            }
            else if (armorType == 1)
            {
                return ArmorMaterialTypes.Chain;
            }
            else
            {
                WeaponMaterialTypes plateMaterial = RandomMaterial(enemyLevel, buildingQuality, playerLuck);
                return (ArmorMaterialTypes)(0x0200 + plateMaterial);
            }
        }

        public static int PickOneOf(params int[] values) // Pango provided assistance in making this much cleaner way of doing the random value choice part, awesome.
        {
            return values[UnityEngine.Random.Range(0, values.Length)];
        }

        public static int PickOneOfCompact(params int[] values)
        {
            List<int> rollNums = new List<int>();

            for (int i = 0; i < values.Length; i += 2) // Even parameter index = value put in random list, Odd parameter index = number of times even param is put into list.
            {
                for (int h = 0; h < values[i + 1]; h++)
                    rollNums.Add(values[i]);
            }

            int[] rollThese = rollNums.ToArray();

            return rollThese[UnityEngine.Random.Range(0, rollThese.Length)];
        }

        #endregion

        #region Spell Costs

        /// <summary>
        /// Performs complete gold and spellpoint costs for an array of effects.
        /// Also calculates multipliers for target type.
        /// </summary>
        /// <param name="effectEntries">EffectEntry array for spell.</param>
        /// <param name="targetType">Target type of spell.</param>
        /// <param name="totalGoldCostOut">Total gold cost out.</param>
        /// <param name="totalSpellPointCostOut">Total spellpoint cost out.</param>
        /// <param name="casterEntity">Caster entity. Assumed to be player if null.</param>
        /// <param name="minimumCastingCost">Spell point always costs minimum (e.g. from vampirism). Do not set true for reflection/absorption cost calculations.</param>
        public static void CalculateTotalEffectCosts(EffectEntry[] effectEntries, TargetTypes targetType, out int totalGoldCostOut, out int totalSpellPointCostOut, DaggerfallEntity casterEntity = null, bool minimumCastingCost = false)
        {
            const int castCostFloor = 5;

            totalGoldCostOut = 0;
            totalSpellPointCostOut = 0;

            // Must have effect entries
            if (effectEntries == null || effectEntries.Length == 0)
                return;

            // Add costs for each active effect slot
            for (int i = 0; i < effectEntries.Length; i++)
            {
                if (string.IsNullOrEmpty(effectEntries[i].Key))
                    continue;

                int goldCost, spellPointCost;
                CalculateEffectCosts(effectEntries[i], out goldCost, out spellPointCost, casterEntity);
                totalGoldCostOut += goldCost;
                totalSpellPointCostOut += spellPointCost;
            }

            // Multipliers for target type
            totalGoldCostOut = ApplyTargetCostMultiplier(totalGoldCostOut, targetType);
            totalSpellPointCostOut = ApplyTargetCostMultiplier(totalSpellPointCostOut, targetType);

            // Set vampire spell cost
            if (minimumCastingCost)
                totalSpellPointCostOut = castCostFloor;

            // Enforce minimum
            if (totalSpellPointCostOut < castCostFloor)
                totalSpellPointCostOut = castCostFloor;
        }

        /// <summary>
        /// Calculate effect costs from an EffectEntry.
        /// </summary>
        public static void CalculateEffectCosts(EffectEntry effectEntry, out int goldCostOut, out int spellPointCostOut, DaggerfallEntity casterEntity = null)
        {
            goldCostOut = 0;
            spellPointCostOut = 0;

            // Get effect template
            IEntityEffect effectTemplate = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(effectEntry.Key);
            if (effectTemplate == null)
                return;

            CalculateEffectCosts(effectTemplate, effectEntry.Settings, out goldCostOut, out spellPointCostOut, casterEntity);
        }

        /// <summary>
        /// Calculates effect costs from an IEntityEffect and custom settings.
        /// </summary>
        public static void CalculateEffectCosts(IEntityEffect effect, EffectSettings settings, out int goldCostOut, out int spellPointCostOut, DaggerfallEntity casterEntity = null)
        {
            int activeComponents = 0;
            goldCostOut = 0;
            spellPointCostOut = 0;

            // Get related skill
            int skillValue = 0;
            if (casterEntity == null)
            {
                // From player
                skillValue = GameManager.Instance.PlayerEntity.Skills.GetLiveSkillValue((DFCareer.Skills)effect.Properties.MagicSkill);
            }
            else
            {
                // From another entity
                skillValue = casterEntity.Skills.GetLiveSkillValue((DFCareer.Skills)effect.Properties.MagicSkill);
            }

            // Duration costs
            int durationGoldCost = 0;
            if (effect.Properties.SupportDuration)
            {
                activeComponents++;
                GetEffectComponentCosts(
                    out durationGoldCost,
                    effect.Properties.DurationCosts,
                    settings.DurationBase,
                    settings.DurationPlus,
                    settings.DurationPerLevel,
                    skillValue);

                //Debug.LogFormat("Duration: gold {0} spellpoints {1}", durationGoldCost, durationSpellPointCost);
            }

            // Chance costs
            int chanceGoldCost = 0;
            if (effect.Properties.SupportChance)
            {
                activeComponents++;
                GetEffectComponentCosts(
                    out chanceGoldCost,
                    effect.Properties.ChanceCosts,
                    settings.ChanceBase,
                    settings.ChancePlus,
                    settings.ChancePerLevel,
                    skillValue);

                //Debug.LogFormat("Chance: gold {0} spellpoints {1}", chanceGoldCost, chanceSpellPointCost);
            }

            // Magnitude costs
            int magnitudeGoldCost = 0;
            if (effect.Properties.SupportMagnitude)
            {
                activeComponents++;
                int magnitudeBase = (settings.MagnitudeBaseMax + settings.MagnitudeBaseMin) / 2;
                int magnitudePlus = (settings.MagnitudePlusMax + settings.MagnitudePlusMin) / 2;
                GetEffectComponentCosts(
                    out magnitudeGoldCost,
                    effect.Properties.MagnitudeCosts,
                    magnitudeBase,
                    magnitudePlus,
                    settings.MagnitudePerLevel,
                    skillValue);

                //Debug.LogFormat("Magnitude: gold {0} spellpoints {1}", magnitudeGoldCost, magnitudeSpellPointCost);
            }

            // If there are no active components (e.g. Teleport) then fudge some costs
            // This gives the same casting cost outcome as classic and supplies a reasonable gold cost
            // Note: Classic does not assign a gold cost when a zero-component effect is the only effect present, which seems like a bug
            int fudgeGoldCost = 0;
            if (activeComponents == 0)
                GetEffectComponentCosts(out fudgeGoldCost, BaseEntityEffect.MakeEffectCosts(60, 100, 160), 1, 1, 1, skillValue);

            // Add gold costs together and calculate spellpoint cost from the result
            goldCostOut = durationGoldCost + chanceGoldCost + magnitudeGoldCost + fudgeGoldCost;
            spellPointCostOut = goldCostOut * (110 - skillValue) / 400;

            //Debug.LogFormat("Costs: gold {0} spellpoints {1}", finalGoldCost, finalSpellPointCost);
        }

        public static int ApplyTargetCostMultiplier(int cost, TargetTypes targetType)
        {
            switch (targetType)
            {
                default:
                case TargetTypes.CasterOnly:                // x1.0
                case TargetTypes.ByTouch:
                    // These do not change costs, just including here for completeness
                    break;
                case TargetTypes.SingleTargetAtRange:       // x1.5
                    cost = (int)(cost * 1.5f);
                    break;
                case TargetTypes.AreaAroundCaster:          // x2.0
                    cost = (int)(cost * 2.0f);
                    break;
                case TargetTypes.AreaAtRange:               // x2.5
                    cost = (int)(cost * 2.5f);
                    break;
            }

            return cost;
        }

        static void GetEffectComponentCosts(
            out int goldCost,
            EffectCosts costs,
            int starting,
            int increase,
            int perLevel,
            int skillValue)
        {
            //Calculate effect gold cost, spellpoint cost is calculated from gold cost after adding up for duration, chance and magnitude
            goldCost = trunc(costs.OffsetGold + costs.CostA * starting + costs.CostB * trunc(increase / perLevel));
        }

        /// <summary>
        /// Reversed from classic. Calculates enchantment point/gold value for a spell being attached to an item.
        /// </summary>
        /// <param name="spellIndex">Index of spell in SPELLS.STD.</param>
        public static int GetSpellEnchantPtCost(int spellIndex)
        {
            List<SpellRecord.SpellRecordData> spells = DaggerfallSpellReader.ReadSpellsFile();
            int cost = 0;

            foreach (SpellRecord.SpellRecordData spell in spells)
            {
                if (spell.index == spellIndex)
                {
                    cost = 10 * CalculateCastingCost(spell);
                    break;
                }
            }

            return cost;
        }

        /// <summary>
        /// Reversed from classic. Calculates cost of casting a spell. This cost is also used
        /// to lower item condition when equipping an item whith a "Cast when held" effect.
        /// For now this is only being used for enchanted items, because there is other code for entity-cast spells.
        /// </summary>
        /// <param name="spell">Spell record read from SPELLS.STD.</param>
        /// <param name="enchantingItem">True if the method is used from the magic item maker.</param>
        public static int CalculateCastingCost(SpellRecord.SpellRecordData spell, bool enchantingItem= true)
        {
            // Indices into effect settings array for each effect and its subtypes
            byte[] effectIndices = {    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Paralysis
                                        0x01, 0x02, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Continuous Damage
                                        0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Create Item
                                        0x05, 0x05, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Cure
                                        0x07, 0x07, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Damage
                                        0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Disintegrate
                                        0x09, 0x08, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Dispel
                                        0x0A, 0x0A, 0x0A, 0x0A, 0x0A, 0x0A, 0x0A, 0x0A, 0x00, 0x00, 0x00, 0x00, // Drain
                                        0x0B, 0x0B, 0x0B, 0x0B, 0x0B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Elemental Resistance
                                        0x0C, 0x0C, 0x0C, 0x0C, 0x0C, 0x0C, 0x0C, 0x0C, 0x00, 0x00, 0x00, 0x00, // Fortify Attribute
                                        0x0D, 0x0D, 0x0D, 0x0D, 0x0D, 0x0D, 0x0D, 0x0D, 0x07, 0x0E, 0x00, 0x00, // Heal
                                        0x0F, 0x0F, 0x0F, 0x0F, 0x0F, 0x0F, 0x0F, 0x0F, 0x0F, 0x0F, 0x00, 0x00, // Transfer
                                        0x26, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Soul Trap
                                        0x10, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Invisibility
                                        0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Levitate
                                        0x13, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Light
                                        0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Lock
                                        0x28, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Open
                                        0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Regenerate
                                        0x16, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Silence
                                        0x17, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Spell Absorption
                                        0x17, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Spell Reflection
                                        0x16, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Spell Resistance
                                        0x18, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Chameleon
                                        0x18, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Shadow
                                        0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Slowfall
                                        0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Climbing
                                        0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Jumping
                                        0x19, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Free Action
                                        0x1A, 0x1A, 0x1A, 0x1A, 0x1A, 0x1A, 0x1A, 0x00, 0x00, 0x00, 0x00, 0x00, // Lycanthropy/Polymorph
                                        0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Water Breathing
                                        0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Water Walking
                                        0x1B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Dimunition
                                        0x1A, 0x1C, 0x1C, 0x1D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Calm
                                        0x1E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Charm
                                        0x1F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Shield
                                        0x27, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Telekinesis
                                        0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Astral Travel
                                        0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Etherealness
                                        0x21, 0x21, 0x22, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Detect
                                        0x23, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Identify
                                        0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Wizard Sight
                                        0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Darkness
                                        0x25, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Recall
                                        0x29, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Comprehend Languages
                                        0x2A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Intensify Fire
                                        0x2A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Diminish Fire
                                        0x2A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Wall of Stone?
                                        0x2A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Wall of Fire?
                                        0x2A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Wall of Frost?
                                        0x2A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }; // Wall of Poison?

            // These are coefficients for each effect type and subtype. They affect casting cost, enchantment point cost and magic item worth.
            // There are 4 coefficients, used together with duration, chance and magnitude settings.
            // Which one they are used with depends on which "settings type" the effect is classified as.
            ushort[] effectCoefficients = {
                                        0x07, 0x19, 0x07, 0x19, // Paralysis / Cure Magic?
                                        0x07, 0x02, 0x0A, 0x07, // Continuous Damage - Health
                                        0x05, 0x02, 0x0A, 0x07, // Continuous Damage - Stamina / Climbing / Jumping / Water Breathing / Water Walking
                                        0x0A, 0x02, 0x0A, 0x07, // Continuous Damage - Spell Points
                                        0x0F, 0x1E, 0x00, 0x00, // Create Item
                                        0x02, 0x19, 0x00, 0x00, // Cure Disease / Cure Poison
                                        0x05, 0x23, 0x00, 0x00, // Cure Paralysis
                                        0x05, 0x07, 0x00, 0x00, // Damage Health / Damage Stamina / Damage Spell Points / Heal Health / Darkness
                                        0x14, 0x23, 0x00, 0x00, // Disintegrate / Dispel Undead
                                        0x1E, 0x2D, 0x00, 0x00, // Dispel Magic / Dispel Daedra
                                        0x04, 0x19, 0x02, 0x19, // Drain Attribute
                                        0x19, 0x19, 0x02, 0x19, // Elemental Resistance
                                        0x07, 0x19, 0x0A, 0x1E, // Fortify Attribute
                                        0x0A, 0x07, 0x00, 0x00, // Heal Attribute
                                        0x02, 0x07, 0x00, 0x00, // Heal Stamina
                                        0x05, 0x05, 0x0F, 0x19, // Transfer
                                        0x0A, 0x1E, 0x00, 0x00, // Invisibility
                                        0x0F, 0x23, 0x00, 0x00, // True Invisibility
                                        0x0F, 0x19, 0x00, 0x00, // Levitate
                                        0x02, 0x0A, 0x00, 0x00, // Light
                                        0x05, 0x19, 0x07, 0x1E, // Lock / Slowfall
                                        0x19, 0x05, 0x02, 0x02, // Regenerate
                                        0x05, 0x19, 0x05, 0x19, // Silence / Spell Resistance
                                        0x07, 0x23, 0x07, 0x23, // Spell Absorption / Spell Reflection
                                        0x05, 0x14, 0x00, 0x00, // Chameleon / Shadow
                                        0x05, 0x05, 0x00, 0x00, // Free Action
                                        0x0F, 0x19, 0x0F, 0x19, // Lycanthropy / Polymorph / Calm Animal
                                        0x0A, 0x14, 0x14, 0x28, // Diminution
                                        0x0A, 0x05, 0x14, 0x23, // Calm Undead / Calm Humanoid
                                        0x07, 0x02, 0x0F, 0x1E, // Calm Daedra? (Unused)
                                        0x05, 0x02, 0x0A, 0x0F, // Charm
                                        0x07, 0x02, 0x14, 0x0F, // Shield
                                        0x23, 0x02, 0x0A, 0x19, // Astral Travel / Etherealness
                                        0x05, 0x02, 0x14, 0x1E, // Detect Magic / Detect Enemy
                                        0x05, 0x02, 0x0F, 0x19, // Detect Treasure
                                        0x05, 0x02, 0x0A, 0x19, // Identify
                                        0x07, 0x0C, 0x05, 0x05, // Wizard Sight
                                        0x23, 0x2D, 0x00, 0x00, // Recall
                                        0x0F, 0x11, 0x0A, 0x11, // Soul Trap
                                        0x14, 0x11, 0x19, 0x23, // Telekinesis
                                        0x05, 0x19, 0x00, 0x00, // Open
                                        0x0F, 0x11, 0x0A, 0x11, // Comprehend Languages
                                        0x0F, 0x0F, 0x05, 0x05 }; // Intensify Fire / Diminish Fire / Wall of --

            // Used to know which Magic School an effect belongs to
            byte[] effectMagicSchools = { 0, 2, 3, 1, 2, 2, 3, 2, 0, 1,
                                          1, 2, 3, 5, 4, 5, 3, 3, 1, 3,
                                          1, 4, 4, 5, 5, 0, 1, 0, 0, 5,
                                          0, 4, 0, 4, 4, 0, 3, 3, 0, 4,
                                          4, 4, 5, 3, 3, 0, 0, 4, 4, 4,
                                          4 };

            // Used to get the skill corresponding to each of the above magic school
            DFCareer.Skills[] magicSkills = { DFCareer.Skills.Alteration,
                                              DFCareer.Skills.Restoration,
                                              DFCareer.Skills.Destruction,
                                              DFCareer.Skills.Mysticism,
                                              DFCareer.Skills.Thaumaturgy,
                                              DFCareer.Skills.Illusion };

            // All effects have one of 6 types for their settings depending on which settings (duration, chance, magnitude)
            // they use, which determine how the coefficient values are used with their data to determine spell casting
            // cost /magic item value/enchantment point cost.
            // There is also a 7th type that is supported in classic (see below) but no effect is defined to use it.
            byte[] settingsTypes = { 1, 2, 3, 4, 5, 4, 4, 2, 1, 6,
                                     5, 6, 1, 3, 3, 3, 1, 4, 2, 1,
                                     1, 1, 1, 3, 3, 3, 3, 3, 3, 1,
                                     3, 3, 1, 1, 1, 2, 2, 1, 1, 1,
                                     1, 1, 3, 4, 1, 2, 2, 2, 2, 2,
                                     2 };

            // Modifiers for casting ranges
            byte[] rangeTypeModifiers = { 2, 2, 3, 4, 5 };

            int cost = 0;
            int skill = 50; // 50 is used for item enchantments
            
            for (int i = 0; i < 3; ++i)
            {
                if (spell.effects[i].type != -1)
                {
                    // Get the coefficients applied to settings for this effect and copy them into the temporary variable
                    ushort[] coefficientsForThisEffect = new ushort[4];

                    if (spell.effects[i].subType == -1) // No subtype
                    {
                        Array.Copy(effectCoefficients, 4 * effectIndices[12 * spell.effects[i].type], coefficientsForThisEffect, 0, 4);
                    }
                    else // Subtype exists
                    {
                        Array.Copy(effectCoefficients, 4 * effectIndices[12 * spell.effects[i].type + spell.effects[i].subType], coefficientsForThisEffect, 0, 4);
                    }

                    if (!enchantingItem)
                    {
                        // If not using the item maker, then player skill corresponding to the effect magic school must be used
                        skill = GameManager.Instance.PlayerEntity.Skills.GetLiveSkillValue(magicSkills[effectMagicSchools[spell.effects[i].type]]);
                    }

                    // Add to the cost based on this effect's settings
                    cost += getCostFromSettings(settingsTypes[spell.effects[i].type], i, spell, coefficientsForThisEffect) * (110 - skill) / 100;
                }
            }

            cost = cost * rangeTypeModifiers[spell.rangeType] >> 1;
            if (cost < 5)
                cost = 5;

            return cost;
        }

        /// <summary>
        /// Reversed from classic. Used wih calculating cost of casting a spell.
        /// This uses the spell's settings for chance, duration and magnitude together with coefficients for that effect
        /// to get the cost of the effect, before the range type modifier is applied.
        /// </summary>
        public static int getCostFromSettings(int settingsType, int effectNumber, SpellRecord.SpellRecordData spellData, ushort[] coefficients)
        {
            int cost = 0;

            switch (settingsType)
            {
                case 1:
                    // Coefficients used with:
                    // 0 = durationBase, 1 = durationMod / durationPerLevel, 2 = chanceBase, 3 = chanceMod / chancePerLevel
                    cost =    coefficients[0] * spellData.effects[effectNumber].durationBase
                            + spellData.effects[effectNumber].durationMod / spellData.effects[effectNumber].durationPerLevel * coefficients[1]
                            + coefficients[2] * spellData.effects[effectNumber].chanceBase
                            + spellData.effects[effectNumber].chanceMod / spellData.effects[effectNumber].chancePerLevel * coefficients[3];
                    break;
                case 2:
                    // Coefficients used with:
                    // 0 = durationBase, 1 = durationMod / durationPerLevel, 2 = (magnitudeBaseHigh + magnitudeBaseLow) / 2, 3 = (magnitudeLevelBase + magnitudeLevelHigh) / 2 / magnitudePerLevel
                    cost =    coefficients[0] * spellData.effects[effectNumber].durationBase
                            + spellData.effects[effectNumber].durationMod / spellData.effects[effectNumber].durationPerLevel * coefficients[1]
                            + (spellData.effects[effectNumber].magnitudeBaseHigh + spellData.effects[effectNumber].magnitudeBaseLow) / 2 * coefficients[2]
                            + (spellData.effects[effectNumber].magnitudeLevelBase + spellData.effects[effectNumber].magnitudeLevelHigh) / 2 / spellData.effects[effectNumber].magnitudePerLevel * coefficients[3];
                    break;
                case 3:
                    // Coefficients used with:
                    // 0 = durationBase, 1 = durationMod / durationPerLevel
                    cost =    coefficients[0] * spellData.effects[effectNumber].durationBase
                            + spellData.effects[effectNumber].durationMod / spellData.effects[effectNumber].durationPerLevel * coefficients[1];
                    break;
                case 4:
                    // Coefficients used with:
                    // 0 = chanceBase, 1 = chanceMod / chancePerLevel
                    cost =    coefficients[0] * spellData.effects[effectNumber].chanceBase
                            + spellData.effects[effectNumber].chanceMod / spellData.effects[effectNumber].chancePerLevel * coefficients[1];
                    break;
                case 5:
                    // Coefficients used with:
                    // 0 = (magnitudeBaseHigh + magnitudeBaseLow) / 2, 1 = (magnitudeLevelBase + magnitudeLevelHigh) / 2 / magnitudePerLevel
                    cost =    coefficients[0] * ((spellData.effects[effectNumber].magnitudeBaseHigh + spellData.effects[effectNumber].magnitudeBaseLow) / 2)
                            + (spellData.effects[effectNumber].magnitudeLevelBase + spellData.effects[effectNumber].magnitudeLevelHigh) / 2 / spellData.effects[effectNumber].magnitudePerLevel * coefficients[1];
                    break;
                case 6:
                    // Coefficients used with:
                    // 0 = durationBase, 1 = durationMod / durationPerLevel, 2 = (magnitudeBaseHigh + magnitudeBaseLow) / 2, 3 = (magnitudeLevelBase + magnitudeLevelHigh) / 2 / magnitudePerLevel
                    cost =    coefficients[0] * spellData.effects[effectNumber].durationBase
                            + coefficients[1] * spellData.effects[effectNumber].durationMod / spellData.effects[effectNumber].durationPerLevel
                            + ((spellData.effects[effectNumber].magnitudeBaseHigh + spellData.effects[effectNumber].magnitudeBaseLow) / 2) * coefficients[2]
                            + coefficients[3] / spellData.effects[effectNumber].magnitudePerLevel * ((spellData.effects[effectNumber].magnitudeLevelBase + spellData.effects[effectNumber].magnitudeLevelHigh) / 2);
                    break;
                case 7: // Not used
                    // Coefficients used with:
                    // 0 = (magnitudeBaseHigh + magnitudeBaseLow) / 2, 1 = (magnitudeLevelBase + magnitudeLevelHigh) / 2 / magnitudePerLevel * durationBase / durationMod
                    cost =    (spellData.effects[effectNumber].magnitudeBaseHigh + spellData.effects[effectNumber].magnitudeBaseLow) / 2 * coefficients[0]
                            + coefficients[1] * (spellData.effects[effectNumber].magnitudeLevelBase + spellData.effects[effectNumber].magnitudeLevelHigh) / 2 / spellData.effects[effectNumber].magnitudePerLevel * spellData.effects[effectNumber].durationBase / spellData.effects[effectNumber].durationMod;
                    break;
            }
            return cost;
        }

        // Just makes formulas more readable
        static int trunc(double value) { return (int)Math.Truncate(value); }

        #endregion

        #region Enchanting

        /// <summary>
        /// Gets the maximum enchantment capacity for any item.
        /// </summary>
        /// <param name="item">Source item.</param>
        /// <returns>Item max enchantment power.</returns>
        public static int GetItemEnchantmentPower(DaggerfallUnityItem item)
        {
            Func<DaggerfallUnityItem, int> del;
            if (TryGetOverride("GetItemEnchantmentPower", out del))
                return del(item);

            if (item == null)
                throw new Exception("GetItemEnchantmentPower: item is null");

            float multiplier = 0f;
            if (item.ItemGroup == ItemGroups.Weapons)
                multiplier = GetWeaponEnchantmentMultiplier((WeaponMaterialTypes)item.NativeMaterialValue);
            else if (item.ItemGroup == ItemGroups.Armor)
                multiplier = GetArmorEnchantmentMultiplier((ArmorMaterialTypes)item.NativeMaterialValue);

            // Final enchantment power is basePower + basePower*multiplier (rounded down)
            int basePower = item.ItemTemplate.enchantmentPoints;
            return basePower + Mathf.FloorToInt(basePower * multiplier);
        }

        public static float GetWeaponEnchantmentMultiplier(WeaponMaterialTypes weaponMaterial)
        {
            // UESP lists regular material power progression in weapon matrix: https://en.uesp.net/wiki/Daggerfall:Enchantment_Power#Weapons
            // Enchantment power values for staves are inaccurate in UESP weapon matrix (confirmed in classic)
            // The below yields correct enchantment power for staves matching classic
            switch(weaponMaterial)
            {
                default:       
                case WeaponMaterialTypes.Iron:
                    return -0.50f;
                case WeaponMaterialTypes.Steel:
                    return -0.25f;
                case WeaponMaterialTypes.Silver:
                    return -0.25f;
                case WeaponMaterialTypes.Orcish:
                    return 0f;
                case WeaponMaterialTypes.Ebony:
                    return 0.25f;
                case WeaponMaterialTypes.Mithril:
                    return 0.50f;
                case WeaponMaterialTypes.Elven:
                    return 0.75f;
                case WeaponMaterialTypes.Daedric:
                    return 1.00f;
                case WeaponMaterialTypes.Dwarven:
                    return 1.25f;
                case WeaponMaterialTypes.Adamantium:
                    return 2.25f;
            }
        }

        public static float GetArmorEnchantmentMultiplier(ArmorMaterialTypes armorMaterial)
        {
            // UESP lists highly variable material power progression in armour matrix: https://en.uesp.net/wiki/Daggerfall:Enchantment_Power#Armor
            // This indicates certain armour types don't follow the same general material progression patterns for enchantment point multipliers
            // Yet to confirm this in classic - but not entirely confident in accuracy of UESP information here either
            // For now using consistent progression for enchantment point multipliers and can improve later if required
            switch (armorMaterial)
            {
                default:
                case ArmorMaterialTypes.Iron:
                    return -0.50f;
                case ArmorMaterialTypes.Leather:
                case ArmorMaterialTypes.Chain:
                case ArmorMaterialTypes.Chain2:
                case ArmorMaterialTypes.Steel:
                    return -0.25f;
                case ArmorMaterialTypes.Silver:
                    return -0.25f;
                case ArmorMaterialTypes.Orcish:
                    return 0f;
                case ArmorMaterialTypes.Ebony:
                    return 0.25f;
                case ArmorMaterialTypes.Mithril:
                    return 0.50f;
                case ArmorMaterialTypes.Elven:
                    return 0.75f;
                case ArmorMaterialTypes.Daedric:
                    return 1.00f;
                case ArmorMaterialTypes.Dwarven:
                    return 1.25f;
                case ArmorMaterialTypes.Adamantium:
                    return 2.25f;
            }
        }

        #endregion

        #region Formula Overrides

        /// <summary>
        /// Registers an override for a formula using a generic `System.Func{T}` callback
        /// with the same signature as the method it overrides
        /// (i.e. `RegisterOverride{Func{int, int, float}}("FormulaName", (a, b) => (float)a / b);`).
        /// The invocation will fail if signature is not correct, meaning if the delegate
        /// is not one of the variation of `Func` with the expected arguments.
        /// </summary>
        /// <param name="provider">The mod that provides this override; used to enforce load order.</param>
        /// <param name="formulaName">The name of the method that provides the formula.</param>
        /// <param name="formula">A callback that implements the formula.</param>
        /// <typeparam name="TDelegate">One of the available generic Func delegates.</typeparam>
        /// <exception cref="ArgumentNullException">`formulaName` or `formula` is null.</exception>
        /// <exception cref="InvalidCastException">Type is not a delegate.</exception>
        public static void RegisterOverride<TDelegate>(Mod provider, string formulaName, TDelegate formula)
            where TDelegate : class
        {
            if (formulaName == null)
                throw new ArgumentNullException("formulaName");

            if (formula == null)
                throw new ArgumentNullException("formula");

            var del = formula as Delegate;
            if (del == null)
                throw new InvalidCastException("formula is not a delegate.");

            FormulaOverride formulaOverride;
            if (!overrides.TryGetValue(formulaName, out formulaOverride) || formulaOverride.Provider.LoadPriority < provider.LoadPriority)
                overrides[formulaName] = new FormulaOverride(del, provider);
        }

        /// <summary>
        /// Gets an override for a formula.
        /// </summary>
        /// <param name="formulaName">The name of the method that provides the formula.</param>
        /// <param name="formula">A callback that implements the formula.</param>
        /// <typeparam name="TDelegate">One of the available generic Func delegates.</typeparam>
        /// <returns>True if formula is found.</returns>
        private static bool TryGetOverride<TDelegate>(string formulaName, out TDelegate formula)
            where TDelegate : class
        {
            FormulaOverride formulaOverride;
            if (overrides.TryGetValue(formulaName, out formulaOverride))
            {
                if ((formula = formulaOverride.Formula as TDelegate) != null)
                    return true;

                const string errorMessage = "Removed override for formula {0} provided by {1} because signature doesn't match (expected {2} and got {3}).";
                Debug.LogErrorFormat(errorMessage, formulaName, formulaOverride.Provider.Title, typeof(TDelegate), formulaOverride.Formula.GetType());
                overrides.Remove(formulaName);
            }

            formula = default(TDelegate);
            return false;
        }

        #endregion
    }
}
