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

using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;
using UnityEngine;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Static definitions for enemies and their animations.
    /// Remaining data is read from MONSTER.BSA.
    /// </summary>
    public static class EnemyBasics
    {
        #region Enemy Animations

        // Speeds in frames-per-second
        public static int MoveAnimSpeed = 6;
        public static int FlyAnimSpeed = 10;
        public static int PrimaryAttackAnimSpeed = 10;
        public static int HurtAnimSpeed = 4;
        public static int IdleAnimSpeed = 4;
        public static int RangedAttack1AnimSpeed = 10;
        public static int RangedAttack2AnimSpeed = 10;

        /// <summary>Struct for return values of method that alters enemy stat values for formula purposes on the fly.</summary>
        public struct CustomEnemyStatValues
        {
            public int weaponSkillCustom;
            public int critSkillCustom;
            public int dodgeSkillCustom;
            public int strengthCustom; // Not currently used
            public int agilityCustom;
            public int speedCustom;
            public int willpowerCustom; // Not currently used
            public int luckCustom;
        }

        // Move animations (double as idle animations for swimming and flying enemies, and enemies without idle animations)
        public static MobileAnimation[] MoveAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 0, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 1, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south-west
            new MobileAnimation() {Record = 2, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing west
            new MobileAnimation() {Record = 3, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing north-west
            new MobileAnimation() {Record = 4, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing north (back facing player)
            new MobileAnimation() {Record = 3, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true},              // Facing north-east
            new MobileAnimation() {Record = 2, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true},              // Facing east
            new MobileAnimation() {Record = 1, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true},              // Facing south-east
        };

        // PrimaryAttack animations
        public static MobileAnimation[] PrimaryAttackAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 5, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing south (front facing player)
            new MobileAnimation() {Record = 6, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing south-west
            new MobileAnimation() {Record = 7, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing west
            new MobileAnimation() {Record = 8, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing north-west
            new MobileAnimation() {Record = 9, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing north (back facing player)
            new MobileAnimation() {Record = 8, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = true},     // Facing north-east
            new MobileAnimation() {Record = 7, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = true},     // Facing east
            new MobileAnimation() {Record = 6, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = true},     // Facing south-east
        };

        // Hurt animations
        public static MobileAnimation[] HurtAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 10, FramePerSecond = HurtAnimSpeed, FlipLeftRight = false},            // Facing south (front facing player)
            new MobileAnimation() {Record = 11, FramePerSecond = HurtAnimSpeed, FlipLeftRight = false},            // Facing south-west
            new MobileAnimation() {Record = 12, FramePerSecond = HurtAnimSpeed, FlipLeftRight = false},            // Facing west
            new MobileAnimation() {Record = 13, FramePerSecond = HurtAnimSpeed, FlipLeftRight = false},            // Facing north-west
            new MobileAnimation() {Record = 14, FramePerSecond = HurtAnimSpeed, FlipLeftRight = false},            // Facing north (back facing player)
            new MobileAnimation() {Record = 13, FramePerSecond = HurtAnimSpeed, FlipLeftRight = true},             // Facing north-east
            new MobileAnimation() {Record = 12, FramePerSecond = HurtAnimSpeed, FlipLeftRight = true},             // Facing east
            new MobileAnimation() {Record = 11, FramePerSecond = HurtAnimSpeed, FlipLeftRight = true},             // Facing south-east
        };

        // Idle animations (most monsters have a static idle sprite)
        public static MobileAnimation[] IdleAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 15, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing south (front facing player)
            new MobileAnimation() {Record = 16, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing south-west
            new MobileAnimation() {Record = 17, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing west
            new MobileAnimation() {Record = 18, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing north-west
            new MobileAnimation() {Record = 19, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing north (back facing player)
            new MobileAnimation() {Record = 18, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing north-east
            new MobileAnimation() {Record = 17, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing east
            new MobileAnimation() {Record = 16, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing south-east
        };

        // RangedAttack1 animations (humanoid mobiles only)
        public static MobileAnimation[] RangedAttack1Anims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 20, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = false},   // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = false},   // Facing south-west
            new MobileAnimation() {Record = 22, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = false},   // Facing west
            new MobileAnimation() {Record = 23, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = false},   // Facing north-west
            new MobileAnimation() {Record = 24, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = false},   // Facing north (back facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = true},    // Facing north-east
            new MobileAnimation() {Record = 22, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = true},    // Facing east
            new MobileAnimation() {Record = 21, FramePerSecond = RangedAttack1AnimSpeed, FlipLeftRight = true},    // Facing south-east
        };

        // RangedAttack2 animations (475, 489, 490 humanoid mobiles only)
        public static MobileAnimation[] RangedAttack2Anims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 25, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = false},   // Facing south (front facing player)
            new MobileAnimation() {Record = 26, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = false},   // Facing south-west
            new MobileAnimation() {Record = 27, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = false},   // Facing west
            new MobileAnimation() {Record = 28, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = false},   // Facing north-west
            new MobileAnimation() {Record = 29, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = false},   // Facing north (back facing player)
            new MobileAnimation() {Record = 28, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = true},    // Facing north-east
            new MobileAnimation() {Record = 27, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = true},    // Facing east
            new MobileAnimation() {Record = 26, FramePerSecond = RangedAttack2AnimSpeed, FlipLeftRight = true},    // Facing south-east
        };

        // Female thief idle animations
        public static MobileAnimation[] FemaleThiefIdleAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 15, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing south (front facing player)
            new MobileAnimation() {Record = 11, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing south-west
            new MobileAnimation() {Record = 17, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing west
            new MobileAnimation() {Record = 18, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing north-west
            new MobileAnimation() {Record = 19, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing north (back facing player)
            new MobileAnimation() {Record = 18, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing north-east
            new MobileAnimation() {Record = 17, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing east
            new MobileAnimation() {Record = 11, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing south-east
        };

        // Rat idle animations
        public static MobileAnimation[] RatIdleAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 15, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing south (front facing player)
            new MobileAnimation() {Record = 16, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing south-west
            new MobileAnimation() {Record = 17, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing west
            new MobileAnimation() {Record = 18, FramePerSecond = IdleAnimSpeed, FlipLeftRight = true},             // Facing north-west
            new MobileAnimation() {Record = 19, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing north (back facing player)
            new MobileAnimation() {Record = 18, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing north-east
            new MobileAnimation() {Record = 17, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing east
            new MobileAnimation() {Record = 16, FramePerSecond = IdleAnimSpeed, FlipLeftRight = false},            // Facing south-east
        };

        // Wraith and ghost idle/move animations
        public static MobileAnimation[] GhostWraithMoveAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 0, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 1, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south-west
            new MobileAnimation() {Record = 2, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true},              // Facing west
            new MobileAnimation() {Record = 3, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing north-west
            new MobileAnimation() {Record = 4, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing north (back facing player)
            new MobileAnimation() {Record = 3, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true},              // Facing north-east
            new MobileAnimation() {Record = 2, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing east
            new MobileAnimation() {Record = 1, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true},              // Facing south-east
        };

        // Ghost and Wraith attack animations
        public static MobileAnimation[] GhostWraithAttackAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 5, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing south (front facing player)
            new MobileAnimation() {Record = 6, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing south-west
            new MobileAnimation() {Record = 7, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = true},     // Facing west
            new MobileAnimation() {Record = 8, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing north-west
            new MobileAnimation() {Record = 9, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing north (back facing player)
            new MobileAnimation() {Record = 8, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = true},     // Facing north-east
            new MobileAnimation() {Record = 7, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = false},    // Facing east
            new MobileAnimation() {Record = 6, FramePerSecond = PrimaryAttackAnimSpeed, FlipLeftRight = true},     // Facing south-east
        };

        // Seducer special animations - has player-facing orientation only
        public static MobileAnimation[] SeducerTransform1Anims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 23, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
        };
        public static MobileAnimation[] SeducerTransform2Anims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 22, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
        };
        public static MobileAnimation[] SeducerIdleMoveAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 21, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
        };
        public static MobileAnimation[] SeducerAttackAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
            new MobileAnimation() {Record = 20, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false},             // Facing south (front facing player)
        };

        // Slaughterfish special idle/move animation - needs to bounce back and forth between frame 0-N rather than loop
        // Move animations (double as idle animations for swimming and flying enemies, and enemies without idle animations)
        public static MobileAnimation[] SlaughterfishMoveAnims = new MobileAnimation[]
        {
            new MobileAnimation() {Record = 0, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false, BounceAnim = true},   // Facing south (front facing player)
            new MobileAnimation() {Record = 1, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false, BounceAnim = true},   // Facing south-west
            new MobileAnimation() {Record = 2, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false, BounceAnim = true},   // Facing west
            new MobileAnimation() {Record = 3, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false, BounceAnim = true},   // Facing north-west
            new MobileAnimation() {Record = 4, FramePerSecond = MoveAnimSpeed, FlipLeftRight = false, BounceAnim = true},   // Facing north (back facing player)
            new MobileAnimation() {Record = 3, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true, BounceAnim = true},    // Facing north-east
            new MobileAnimation() {Record = 2, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true, BounceAnim = true},    // Facing east
            new MobileAnimation() {Record = 1, FramePerSecond = MoveAnimSpeed, FlipLeftRight = true, BounceAnim = true},    // Facing south-east
        };

        #endregion

        #region Enemy Definitions

        // Defines additional data for known enemy types
        // Fills in the blanks where source of data in game files is unknown
        // Suspect at least some of this data is also hard-coded in Daggerfall
        public static MobileEnemy[] Enemies = new MobileEnemy[]
        {
            // Rat
            new MobileEnemy()
            {
                ID = 0,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 255,
                FemaleTexture = 255,
                CorpseTexture = CorpseTexture(401, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyRatMove,
                BarkSound = (int)SoundClips.EnemyRatBark,
                AttackSound = (int)SoundClips.EnemyRatAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 3,
                MinHealth = 15,
                MaxHealth = 35,
                Level = 1,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                Weight = 2,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4, 5 },
                Team = MobileTeams.Vermin,
            },

            // Imp
            new MobileEnemy()
            {
                ID = 1,
                Behaviour = MobileBehaviour.Flying,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 256,
                FemaleTexture = 256,
                CorpseTexture = CorpseTexture(406, 5),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyImpMove,
                BarkSound = (int)SoundClips.EnemyImpBark,
                AttackSound = (int)SoundClips.EnemyImpAttack,
                MinMetalToHit = WeaponMaterialTypes.Steel,
                MinDamage = 2,
                MaxDamage = 13,
                MinHealth = 10,
                MaxHealth = 20,
                Level = 2,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 1,
                Weight = 40,
                SeesThroughInvisibility = true,
                LootTableKey = "D",
                SoulPts = 1000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 1 },
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 1 },
                Team = MobileTeams.Magic,
            },

            // Spriggan
            new MobileEnemy()
            {
                ID = 2,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 257,
                FemaleTexture = 257,
                CorpseTexture = CorpseTexture(406, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemySprigganMove,
                BarkSound = (int)SoundClips.EnemySprigganBark,
                AttackSound = (int)SoundClips.EnemySprigganAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 5,
                MaxDamage = 11,
                MinHealth = 30,
                MaxHealth = 55,
                Level = 3,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                Weight = 240,
                LootTableKey = "B",
                SoulPts = 1000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 3, 3 },
                Team = MobileTeams.Spriggans,
            },

            // Giant Bat
            new MobileEnemy()
            {
                ID = 3,
                Behaviour = MobileBehaviour.Flying,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 258,
                FemaleTexture = 258,
                CorpseTexture = CorpseTexture(401, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyGiantBatMove,
                BarkSound = (int)SoundClips.EnemyGiantBatBark,
                AttackSound = (int)SoundClips.EnemyGiantBatAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 1,
                MaxDamage = 4,
                MinHealth = 5,
                MaxHealth = 13,
                Level = 2,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                Weight = 80,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3 },
                Team = MobileTeams.Vermin,
            },

            // Grizzly Bear
            new MobileEnemy()
            {
                ID = 4,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 259,
                FemaleTexture = 259,
                CorpseTexture = CorpseTexture(401, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyBearMove,
                BarkSound = (int)SoundClips.EnemyBearBark,
                AttackSound = (int)SoundClips.EnemyBearAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 6,
                MaxDamage = 14,
                MinHealth = 55,
                MaxHealth = 110,
                Level = 4,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                Weight = 1000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 0 },
                Team = MobileTeams.Bears,
            },

            // Sabertooth Tiger
            new MobileEnemy()
            {
                ID = 5,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 260,
                FemaleTexture = 260,
                CorpseTexture = CorpseTexture(401, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyTigerMove,
                BarkSound = (int)SoundClips.EnemyTigerBark,
                AttackSound = (int)SoundClips.EnemyTigerAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 8,
                MaxDamage = 18,
                MinHealth = 35,
                MaxHealth = 60,
                Level = 4,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                Weight = 1000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4, 5 },
                Team = MobileTeams.Tigers,
            },

            // Spider
            new MobileEnemy()
            {
                ID = 6,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 261,
                FemaleTexture = 261,
                CorpseTexture = CorpseTexture(401, 4),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemySpiderMove,
                BarkSound = (int)SoundClips.EnemySpiderBark,
                AttackSound = (int)SoundClips.EnemySpiderAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 3,
                MaxDamage = 9,
                MinHealth = 12,
                MaxHealth = 28,
                Level = 2,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                Weight = 400,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                Team = MobileTeams.Spiders,
            },

            // Orc
            new MobileEnemy()
            {
                ID = 7,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 262,
                FemaleTexture = 262,
                CorpseTexture = CorpseTexture(96, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyOrcMove,
                BarkSound = (int)SoundClips.EnemyOrcBark,
                AttackSound = (int)SoundClips.EnemyOrcAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 6,
                MaxDamage = 13,
                MinHealth = 40,
                MaxHealth = 70,
                Level = 6,
                ArmorValue = 0,
                ParrySounds = true,
                MapChance = 0,
                Weight = 600,
                LootTableKey = "A",
                SoulPts = 1000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 4, -1, 5, 0 },
                Team = MobileTeams.Orcs,
            },

            // Centaur
            new MobileEnemy()
            {
                ID = 8,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 263,
                FemaleTexture = 263,
                CorpseTexture = CorpseTexture(406, 0),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyCentaurMove,
                BarkSound = (int)SoundClips.EnemyCentaurBark,
                AttackSound = (int)SoundClips.EnemyCentaurAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 7,
                MaxDamage = 16,
                MinHealth = 35,
                MaxHealth = 65,
                Level = 5,
                ArmorValue = 0,
                ParrySounds = true,
                MapChance = 1,
                Weight = 1200,
                LootTableKey = "C",
                SoulPts = 3000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 1, 1, 2, -1, 3, 3, 4 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, 1, 1, 2, -1, 3, 3, 2, 1, 1, -1, 2, 3, 3, 4 },
                Team = MobileTeams.Centaurs,
            },

            // Werewolf
            new MobileEnemy()
            {
                ID = 9,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 264,
                FemaleTexture = 264,
                CorpseTexture = CorpseTexture(96, 5),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyWerewolfMove,
                BarkSound = (int)SoundClips.EnemyWerewolfBark,
                AttackSound = (int)SoundClips.EnemyWerewolfAttack,
                MinMetalToHit = WeaponMaterialTypes.Silver,
                MinDamage = 8,
                MaxDamage = 19,
                MinHealth = 30,
                MaxHealth = 55,
                Level = 8,
                ArmorValue = 0,
                MapChance = 0,
                ParrySounds = false,
                Weight = 480,
                SoulPts = 1000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, -1, 2 },
                Team = MobileTeams.Werecreatures,
            },

            // Nymph
            new MobileEnemy()
            {
                ID = 10,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 265,
                FemaleTexture = 265,
                CorpseTexture = CorpseTexture(406, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyNymphMove,
                BarkSound = (int)SoundClips.EnemyNymphBark,
                AttackSound = (int)SoundClips.EnemyNymphAttack,
                MinMetalToHit = WeaponMaterialTypes.Silver,
                MinDamage = 2,
                MaxDamage = 9,
                MinHealth = 25,
                MaxHealth = 45,
                Level = 6,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 1,
                Weight = 200,
                LootTableKey = "C",
                SoulPts = 10000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, 3, 4, -1, 5 },
                Team = MobileTeams.Nymphs,
            },

            // Slaughterfish
            new MobileEnemy()
            {
                ID = 11,
                Behaviour = MobileBehaviour.Aquatic,
                Affinity = MobileAffinity.Water,
                MaleTexture = 266,
                FemaleTexture = 266,
                CorpseTexture = CorpseTexture(305, 1),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyEelMove,
                BarkSound = (int)SoundClips.EnemyEelBark,
                AttackSound = (int)SoundClips.EnemyEelAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 4,
                MaxDamage = 12,
                MinHealth = 25,
                MaxHealth = 50,
                Level = 7,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                Weight = 400,
                PrimaryAttackAnimFrames = new int[] { 0, -1, 1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 3, -1, 5, 4, 3, 3, -1, 5, 4, 3, -1, 5, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 3, -1, 5, 0 },
                Team = MobileTeams.Aquatic,
            },

            // Orc Sergeant
            new MobileEnemy()
            {
                ID = 12,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 267,
                FemaleTexture = 267,
                CorpseTexture = CorpseTexture(96, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyOrcSergeantMove,
                BarkSound = (int)SoundClips.EnemyOrcSergeantBark,
                AttackSound = (int)SoundClips.EnemyOrcSergeantAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 8,
                MaxDamage = 18,
                MinHealth = 50,
                MaxHealth = 85,
                Level = 9,
                ArmorValue = 0,
                ParrySounds = true,
                MapChance = 1,
                Weight = 600,
                LootTableKey = "A",
                SoulPts = 1000,
                PrimaryAttackAnimFrames = new int[] { 0, -1, 1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 5, 4, 3, -1, 2, 1, 0 },
                Team = MobileTeams.Orcs,
            },

            // Harpy
            new MobileEnemy()
            {
                ID = 13,
                Behaviour = MobileBehaviour.Flying,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 268,
                FemaleTexture = 268,
                CorpseTexture = CorpseTexture(406, 4),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHarpyMove,
                BarkSound = (int)SoundClips.EnemyHarpyBark,
                AttackSound = (int)SoundClips.EnemyHarpyAttack,
                MinMetalToHit = WeaponMaterialTypes.Dwarven,
                MinDamage = 7,
                MaxDamage = 15,
                MinHealth = 25,
                MaxHealth = 60,
                Level = 8,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                Weight = 200,
                LootTableKey = "D",
                SoulPts = 3000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3 },
                Team = MobileTeams.Harpies,
            },

            // Wereboar
            new MobileEnemy()
            {
                ID = 14,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 269,
                FemaleTexture = 269,
                CorpseTexture = CorpseTexture(96, 5),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyWereboarMove,
                BarkSound = (int)SoundClips.EnemyWereboarBark,
                AttackSound = (int)SoundClips.EnemyWereboarAttack,
                MinMetalToHit = WeaponMaterialTypes.Silver,
                MinDamage = 7,
                MaxDamage = 17,
                MinHealth = 65,
                MaxHealth = 95,
                Level = 8,
                ArmorValue = 0,
                MapChance = 0,
                ParrySounds = false,
                Weight = 560,
                SoulPts = 1000,
                PrimaryAttackAnimFrames = new int[] { 0, -1, 1, 2, 2 },
                Team = MobileTeams.Werecreatures,
            },

            // Skeletal Warrior
            new MobileEnemy()
            {
                ID = 15,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 270,
                FemaleTexture = 270,
                CorpseTexture = CorpseTexture(306, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemySkeletonMove,
                BarkSound = (int)SoundClips.EnemySkeletonBark,
                AttackSound = (int)SoundClips.EnemySkeletonAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 7,
                MaxDamage = 14,
                MinHealth = 20,
                MaxHealth = 40,
                Level = 11,
                ArmorValue = 0,
                ParrySounds = true,
                MapChance = 1,
                Weight = 80,
                SeesThroughInvisibility = true,
                LootTableKey = "H",
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, 3, -1, 4, 5 },
                Team = MobileTeams.Undead,
            },

            // Giant
            new MobileEnemy()
            {
                ID = 16,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 271,
                FemaleTexture = 271,
                CorpseTexture = CorpseTexture(406, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyGiantMove,
                BarkSound = (int)SoundClips.EnemyGiantBark,
                AttackSound = (int)SoundClips.EnemyGiantAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 8,
                MaxDamage = 18,
                MinHealth = 65,
                MaxHealth = 100,
                Level = 10,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 1,
                LootTableKey = "F",
                Weight = 3000,
                SoulPts = 3000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5 },
                Team = MobileTeams.Giants,
            },

            // Zombie
            new MobileEnemy()
            {
                ID = 17,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 272,
                FemaleTexture = 272,
                CorpseTexture = CorpseTexture(306, 4),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyZombieMove,
                BarkSound = (int)SoundClips.EnemyZombieBark,
                AttackSound = (int)SoundClips.EnemyZombieAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 4,
                MaxDamage = 8,
                MinHealth = 65,
                MaxHealth = 125,
                Level = 5,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 1,
                Weight = 4000,
                LootTableKey = "G",
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 0, 2, -1, 3, 4 },
                Team = MobileTeams.Undead,
            },

            // Ghost
            new MobileEnemy()
            {
                ID = 18,
                Behaviour = MobileBehaviour.Spectral,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 273,
                FemaleTexture = 273,
                CorpseTexture = CorpseTexture(306, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyGhostMove,
                BarkSound = (int)SoundClips.EnemyGhostBark,
                AttackSound = (int)SoundClips.EnemyGhostAttack,
                MinMetalToHit = WeaponMaterialTypes.Silver,
                MinDamage = 7,
                MaxDamage = 14,
                MinHealth = 20,
                MaxHealth = 40,
                Level = 11,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 1,
                Weight = 0,
                SeesThroughInvisibility = true,
                LootTableKey = "I",
                NoShadow = true,
                SoulPts = 30000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3 },
                SpellAnimFrames = new int[] { 0, 0, 0, 0, 0, 0 },
                Team = MobileTeams.Undead,
            },

            // Mummy
            new MobileEnemy()
            {
                ID = 19,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 274,
                FemaleTexture = 274,
                CorpseTexture = CorpseTexture(306, 5),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyMummyMove,
                BarkSound = (int)SoundClips.EnemyMummyBark,
                AttackSound = (int)SoundClips.EnemyMummyAttack,
                MinMetalToHit = WeaponMaterialTypes.Silver,
                MinDamage = 6,
                MaxDamage = 14,
                MinHealth = 75,
                MaxHealth = 110,
                Level = 15,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 1,
                Weight = 300,
                SeesThroughInvisibility = true,
                LootTableKey = "E",
                SoulPts = 10000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4 },
                Team = MobileTeams.Undead,
            },

            // Giant Scorpion
            new MobileEnemy()
            {
                ID = 20,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Animal,
                MaleTexture = 275,
                FemaleTexture = 275,
                CorpseTexture = CorpseTexture(401, 5),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyScorpionMove,
                BarkSound = (int)SoundClips.EnemyScorpionBark,
                AttackSound = (int)SoundClips.EnemyScorpionAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 7,
                MaxDamage = 16,
                MinHealth = 22,
                MaxHealth = 40,
                Level = 4,
                ParrySounds = false,
                ArmorValue = 0,
                MapChance = 0,
                Weight = 600,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 3, 2, 1, 0 },
                Team = MobileTeams.Scorpions,
            },

            // Orc Shaman
            new MobileEnemy()
            {
                ID = 21,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 276,
                FemaleTexture = 276,
                CorpseTexture = CorpseTexture(96, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyOrcShamanMove,
                BarkSound = (int)SoundClips.EnemyOrcShamanBark,
                AttackSound = (int)SoundClips.EnemyOrcShamanAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 7,
                MaxDamage = 15,
                MinHealth = 45,
                MaxHealth = 70,
                Level = 15,
                ArmorValue = 0,
                ParrySounds = true,
                MapChance = 3,
                Weight = 400,
                LootTableKey = "U",
                SoulPts = 3000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 3, 2, 1, 0 },
                ChanceForAttack2 = 20,
                PrimaryAttackAnimFrames2 = new int[] { 0, -1, 4, 5, 0 },
                ChanceForAttack3 = 20,
                PrimaryAttackAnimFrames3 = new int[] { 0, 1, -1, 3, 2, 1, 0, -1, 4, 5, 0 },
                ChanceForAttack4 = 20,
                PrimaryAttackAnimFrames4 = new int[] { 0, 1, -1, 3, 2, -1, 3, 2, 1, 0 }, // Not used in classic. Fight stance used instead.
                ChanceForAttack5 = 20,
                PrimaryAttackAnimFrames5 = new int[] { 0, -1, 4, 5, -1, 4, 5, 0 }, // Not used in classic. Spell animation played instead.
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 0, 1, 2, 3, 3, 3 },
                Team = MobileTeams.Orcs,
            },

            // Gargoyle
            new MobileEnemy()
            {
                ID = 22,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 277,
                FemaleTexture = 277,
                CorpseTexture = CorpseTexture(96, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyGargoyleMove,
                BarkSound = (int)SoundClips.EnemyGargoyleBark,
                AttackSound = (int)SoundClips.EnemyGargoyleAttack,
                MinMetalToHit = WeaponMaterialTypes.Mithril,
                MinDamage = 16,
                MaxDamage = 32,
                MinHealth = 50,
                MaxHealth = 100,
                Level = 14,
                ArmorValue = 0,
                MapChance = 0,
                ParrySounds = false,
                Weight = 300,
                SoulPts = 3000,
                PrimaryAttackAnimFrames = new int[] { 0, 2, 1, 2, 3, -1, 4, 0 },
                Team = MobileTeams.Magic,
            },

            // Wraith
            new MobileEnemy()
            {
                ID = 23,
                Behaviour = MobileBehaviour.Spectral,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 278,
                FemaleTexture = 278,
                CorpseTexture = CorpseTexture(306, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyWraithMove,
                BarkSound = (int)SoundClips.EnemyWraithBark,
                AttackSound = (int)SoundClips.EnemyWraithAttack,
                MinMetalToHit = WeaponMaterialTypes.Silver,
                MinDamage = 16,
                MaxDamage = 32,
                MinHealth = 30,
                MaxHealth = 50,
                Level = 15,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 1,
                Weight = 0,
                SeesThroughInvisibility = true,
                LootTableKey = "I",
                NoShadow = true,
                SoulPts = 30000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3 },
                SpellAnimFrames = new int[] { 0, 0, 0, 0, 0 },
                Team = MobileTeams.Undead,
            },

            // Orc Warlord
            new MobileEnemy()
            {
                ID = 24,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 279,
                FemaleTexture = 279,
                CorpseTexture = CorpseTexture(96, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyOrcWarlordMove,
                BarkSound = (int)SoundClips.EnemyOrcWarlordBark,
                AttackSound = (int)SoundClips.EnemyOrcWarlordAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 20,
                MaxDamage = 36,
                MinHealth = 80,
                MaxHealth = 125,
                Level = 19,
                ArmorValue = 0,
                ParrySounds = true,
                MapChance = 2,
                Weight = 700,
                LootTableKey = "T",
                SoulPts = 1000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, -1, 5, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 1, -1, 2, 3, 4 -1, 5, 0, 4, -1, 5, 0 },
                Team = MobileTeams.Orcs,
            },

            // Frost Daedra
            new MobileEnemy()
            {
                ID = 25,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daedra,
                MaleTexture = 280,
                FemaleTexture = 280,
                CorpseTexture = CorpseTexture(400, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyFrostDaedraMove,
                BarkSound = (int)SoundClips.EnemyFrostDaedraBark,
                AttackSound = (int)SoundClips.EnemyFrostDaedraAttack,
                MinMetalToHit = WeaponMaterialTypes.Mithril,
                MinDamage = 25,
                MaxDamage = 40,
                MinHealth = 90,
                MaxHealth = 160,
                Level = 17,
                ArmorValue = 0,
                ParrySounds = true,
                MapChance = 0,
                Weight = 800,
                SeesThroughInvisibility = true,
                LootTableKey = "J",
                NoShadow = true,
                GlowColor = new Color(18, 68, 88) * 0.1f,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, -1, 4, 5, 0 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { -1, 4, 5, 0 },
                SpellAnimFrames = new int[] { 1, 1, 3, 3 },
                Team = MobileTeams.Daedra,
            },

            // Fire Daedra
            new MobileEnemy()
            {
                ID = 26,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daedra,
                MaleTexture = 281,
                FemaleTexture = 281,
                CorpseTexture = CorpseTexture(400, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyFireDaedraMove,
                BarkSound = (int)SoundClips.EnemyFireDaedraBark,
                AttackSound = (int)SoundClips.EnemyFireDaedraAttack,
                MinMetalToHit = WeaponMaterialTypes.Mithril,
                MinDamage = 35,
                MaxDamage = 55,
                MinHealth = 60,
                MaxHealth = 100,
                Level = 17,
                ArmorValue = 0,
                ParrySounds = true,
                MapChance = 0,
                Weight = 800,
                SeesThroughInvisibility = true,
                LootTableKey = "J",
                NoShadow = true,
                GlowColor = new Color(243, 239, 44) * 0.05f,
                SoulPts = 50000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, -1, 4 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 3, -1, 4 },
                SpellAnimFrames = new int[] { 1, 1, 3, 3 },
                Team = MobileTeams.Daedra,
            },

            // Daedroth
            new MobileEnemy()
            {
                ID = 27,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daedra,
                MaleTexture = 282,
                FemaleTexture = 282,
                CorpseTexture = CorpseTexture(400, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyLesserDaedraMove,
                BarkSound = (int)SoundClips.EnemyLesserDaedraBark,
                AttackSound = (int)SoundClips.EnemyLesserDaedraAttack,
                MinMetalToHit = WeaponMaterialTypes.Mithril,
                MinDamage = 20,
                MaxDamage = 32,
                MinHealth = 70,
                MaxHealth = 120,
                Level = 18,
                ArmorValue = 0,
                ParrySounds = true,
                MapChance = 0,
                Weight = 400,
                SeesThroughInvisibility = true,
                LootTableKey = "E",
                SoulPts = 10000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, -1, 5, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0, 4, -1, 5, 0 },
                SpellAnimFrames = new int[] { 1, 1, 3, 3 },
                Team = MobileTeams.Daedra,
            },

            // Vampire
            new MobileEnemy()
            {
                ID = 28,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 283,
                FemaleTexture = 283,
                CorpseTexture = CorpseTexture(96, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyFemaleVampireMove,
                BarkSound = (int)SoundClips.EnemyFemaleVampireBark,
                AttackSound = (int)SoundClips.EnemyFemaleVampireAttack,
                MinMetalToHit = WeaponMaterialTypes.Silver,
                MinDamage = 15,
                MaxDamage = 32,
                MinHealth = 70,
                MaxHealth = 105,
                Level = 17,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 3,
                Weight = 400,
                SeesThroughInvisibility = true,
                LootTableKey = "Q",
                NoShadow = true,
                SoulPts = 70000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, 3, -1, 4, 5 },
                SpellAnimFrames = new int[] { 1, 1, 5, 5 },
                Team = MobileTeams.Undead,
            },

            // Daedra Seducer
            new MobileEnemy()
            {
                ID = 29,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daedra,
                MaleTexture = 284,
                FemaleTexture = 284,
                CorpseTexture = CorpseTexture(400, 6),          // Has a winged and unwinged corpse, only using unwinged here
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                HasSeducerTransform1 = true,
                HasSeducerTransform2 = true,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemySeducerMove,
                BarkSound = (int)SoundClips.EnemySeducerBark,
                AttackSound = (int)SoundClips.EnemySeducerAttack,
                MinMetalToHit = WeaponMaterialTypes.Mithril,
                MinDamage = 30,
                MaxDamage = 60,
                MinHealth = 70,
                MaxHealth = 95,
                Level = 19,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 1,
                Weight = 200,
                SeesThroughInvisibility = true,
                LootTableKey = "Q",
                SoulPts = 150000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2 },
                SpellAnimFrames = new int[] { 0, 1, 2 },
                SeducerTransform1Frames = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                SeducerTransform2Frames = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 },
                Team = MobileTeams.Daedra,
            },

            // Vampire Ancient
            new MobileEnemy()
            {
                ID = 30,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Darkness,
                MaleTexture = 285,
                FemaleTexture = 285,
                CorpseTexture = CorpseTexture(96, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyVampireMove,
                BarkSound = (int)SoundClips.EnemyVampireBark,
                AttackSound = (int)SoundClips.EnemyVampireAttack,
                MinMetalToHit = WeaponMaterialTypes.Mithril,
                MinDamage = 18,
                MaxDamage = 35,
                MinHealth = 85,
                MaxHealth = 140,
                Level = 20,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 3,
                Weight = 400,
                SeesThroughInvisibility = true,
                LootTableKey = "Q",
                NoShadow = true,
                SoulPts = 100000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, 3, -1, 4, 5 },
                SpellAnimFrames = new int[] { 1, 1, 5, 5 },
                Team = MobileTeams.Undead,
            },

            // Daedra Lord
            new MobileEnemy()
            {
                ID = 31,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Daedra,
                MaleTexture = 286,
                FemaleTexture = 286,
                CorpseTexture = CorpseTexture(400, 4),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyDaedraLordMove,
                BarkSound = (int)SoundClips.EnemyDaedraLordBark,
                AttackSound = (int)SoundClips.EnemyDaedraLordAttack,
                MinMetalToHit = WeaponMaterialTypes.Mithril,
                MinDamage = 26,
                MaxDamage = 42,
                MinHealth = 170,
                MaxHealth = 285,
                Level = 21,
                ArmorValue = 0,
                ParrySounds = true,
                MapChance = 0,
                Weight = 1000,
                SeesThroughInvisibility = true,
                LootTableKey = "S",
                SoulPts = 800000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, -1, 4 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 3, -1, 4, 0, -1, 4, 3, -1, 4, 0, -1, 4, 3 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 1, -1, 2, 1, 0, 1, -1, 2, 1, 0 },
                SpellAnimFrames = new int[] { 1, 1, 3, 3 },
                Team = MobileTeams.Daedra,
            },

            // Lich
            new MobileEnemy()
            {
                ID = 32,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 287,
                FemaleTexture = 287,
                CorpseTexture = CorpseTexture(306, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyLichMove,
                BarkSound = (int)SoundClips.EnemyLichBark,
                AttackSound = (int)SoundClips.EnemyLichAttack,
                MinMetalToHit = WeaponMaterialTypes.Mithril,
                MinDamage = 25,
                MaxDamage = 45,
                MinHealth = 85,
                MaxHealth = 135,
                Level = 20,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 4,
                Weight = 300,
                SeesThroughInvisibility = true,
                LootTableKey = "S",
                SoulPts = 100000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 1, 2, -1, 3, 4, 4 },
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 4 },
                Team = MobileTeams.Undead,
            },

            // Ancient Lich
            new MobileEnemy()
            {
                ID = 33,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Undead,
                MaleTexture = 288,
                FemaleTexture = 288,
                CorpseTexture = CorpseTexture(306, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyLichKingMove,
                BarkSound = (int)SoundClips.EnemyLichKingBark,
                AttackSound = (int)SoundClips.EnemyLichKingAttack,
                MinMetalToHit = WeaponMaterialTypes.Mithril,
                MinDamage = 35,
                MaxDamage = 55,
                MinHealth = 115,
                MaxHealth = 195,
                Level = 21,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 4,
                Weight = 300,
                LootTableKey = "S",
                SoulPts = 250000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 1, 2, -1, 3, 4, 4 },
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 4 },
                Team = MobileTeams.Undead,
            },

            // Dragonling
            new MobileEnemy()
            {
                ID = 34,
                Behaviour = MobileBehaviour.Flying,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 289,
                FemaleTexture = 289,
                CorpseTexture = CorpseTexture(96, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyFaeryDragonMove,
                BarkSound = (int)SoundClips.EnemyFaeryDragonBark,
                AttackSound = (int)SoundClips.EnemyFaeryDragonAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 12,
                MaxDamage = 24,
                MinHealth = 35,
                MaxHealth = 60,
                Level = 16,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                Weight = 10000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3 },
                Team = MobileTeams.Dragonlings,
            },

            // Fire Atronach
            new MobileEnemy()
            {
                ID = 35,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Golem,
                MaleTexture = 290,
                FemaleTexture = 290,
                CorpseTexture = CorpseTexture(405, 2),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyFireAtronachMove,
                BarkSound = (int)SoundClips.EnemyFireAtronachBark,
                AttackSound = (int)SoundClips.EnemyFireAtronachAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 9,
                MaxDamage = 17,
                MinHealth = 40,
                MaxHealth = 60,
                Level = 16,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                NoShadow = true,
                GlowColor = new Color(243, 150, 44) * 0.05f,
                Weight = 1000,
                SoulPts = 30000,
                PrimaryAttackAnimFrames = new int[] { 0, -1, 1, 2, 3, 4 },
                Team = MobileTeams.Magic,
            },

            // Iron Atronach
            new MobileEnemy()
            {
                ID = 36,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Golem,
                MaleTexture = 291,
                FemaleTexture = 291,
                CorpseTexture = CorpseTexture(405, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyIronAtronachMove,
                BarkSound = (int)SoundClips.EnemyIronAtronachBark,
                AttackSound = (int)SoundClips.EnemyIronAtronachAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 12,
                MaxDamage = 23,
                MinHealth = 95,
                MaxHealth = 155,
                Level = 21,
                ArmorValue = 0,
                ParrySounds = true,
                MapChance = 0,
                Weight = 1000,
                SoulPts = 30000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4 },
                Team = MobileTeams.Magic,
            },

            // Flesh Atronach
            new MobileEnemy()
            {
                ID = 37,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Golem,
                MaleTexture = 292,
                FemaleTexture = 292,
                CorpseTexture = CorpseTexture(405, 0),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyFleshAtronachMove,
                BarkSound = (int)SoundClips.EnemyFleshAtronachBark,
                AttackSound = (int)SoundClips.EnemyFleshAtronachAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 3,
                MaxDamage = 8,
                MinHealth = 120,
                MaxHealth = 225,
                Level = 16,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                Weight = 1000,
                SoulPts = 30000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4 },
                Team = MobileTeams.Magic,
            },

            // Ice Atronach
            new MobileEnemy()
            {
                ID = 38,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Golem,
                MaleTexture = 293,
                FemaleTexture = 293,
                CorpseTexture = CorpseTexture(405, 3),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                BloodIndex = 2,
                MoveSound = (int)SoundClips.EnemyIceAtronachMove,
                BarkSound = (int)SoundClips.EnemyIceAtronachBark,
                AttackSound = (int)SoundClips.EnemyIceAtronachAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 5,
                MaxDamage = 13,
                MinHealth = 70,
                MaxHealth = 110,
                Level = 16,
                ArmorValue = 0,
                ParrySounds = true,
                MapChance = 0,
                Weight = 1000,
                SoulPts = 30000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 0, -1, 3, 4 },
                Team = MobileTeams.Magic,
            },

            // Weights in classic (From offset 0x1BD8D9 in FALL.EXE) only have entries
            // up through Horse. Dragonling, Dreugh and Lamia use nonsense values from
            // the adjacent data. For Daggerfall Unity, using values inferred from
            // other enemy types.

            // Horse (unused, but can appear in merchant-sold soul traps)
            new MobileEnemy()
            {
                ID = 39,
            },

            // Dragonling
            new MobileEnemy()
            {
                ID = 40,
                Behaviour = MobileBehaviour.Flying,
                Affinity = MobileAffinity.Daylight,
                MaleTexture = 295,
                FemaleTexture = 295,
                CorpseTexture = CorpseTexture(96, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                MoveSound = (int)SoundClips.EnemyFaeryDragonMove,
                BarkSound = (int)SoundClips.EnemyFaeryDragonBark,
                AttackSound = (int)SoundClips.EnemyFaeryDragonAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 35,
                MaxDamage = 95,
                MinHealth = 125,
                MaxHealth = 230,
                Level = 21,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                Weight = 10000, // Using same value as other dragonling
                SoulPts = 500000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3 },
                Team = MobileTeams.Dragonlings,
            },

            // Dreugh
            new MobileEnemy()
            {
                ID = 41,
                Behaviour = MobileBehaviour.Aquatic,
                Affinity = MobileAffinity.Water,
                MaleTexture = 296,
                FemaleTexture = 296,
                CorpseTexture = CorpseTexture(305, 0),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyDreughMove,
                BarkSound = (int)SoundClips.EnemyDreughBark,
                AttackSound = (int)SoundClips.EnemyDreughAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 6,
                MaxDamage = 16,
                MinHealth = 45,
                MaxHealth = 90,
                Level = 16,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                Weight = 600, // Using same value as orc
                LootTableKey = "R",
                SoulPts = 10000,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, 3, -1, 4, 5, -1, 6, 7 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, 2, 3, -1, 4 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 5, -1, 6, 7 },
                Team = MobileTeams.Aquatic,
            },

            // Lamia
            new MobileEnemy()
            {
                ID = 42,
                Behaviour = MobileBehaviour.Aquatic,
                Affinity = MobileAffinity.Water,
                MaleTexture = 297,
                FemaleTexture = 297,
                CorpseTexture = CorpseTexture(305, 2),
                HasIdle = false,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyLamiaMove,
                BarkSound = (int)SoundClips.EnemyLamiaBark,
                AttackSound = (int)SoundClips.EnemyLamiaAttack,
                MinMetalToHit = WeaponMaterialTypes.None,
                MinDamage = 5,
                MaxDamage = 13,
                MinHealth = 35,
                MaxHealth = 65,
                Level = 16,
                ArmorValue = 0,
                ParrySounds = false,
                MapChance = 0,
                LootTableKey = "R",
                Weight = 200, // Using same value as nymph
                SoulPts = 10000,
                PrimaryAttackAnimFrames = new int[] { 0, -1, 1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 3, -1, 5, 4, 3, 3, -1, 5, 4, 3, -1, 5, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 3, -1, 5, 0 },
                Team = MobileTeams.Aquatic,
            },

            // Mage
            new MobileEnemy()
            {
                ID = 128,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 486,
                FemaleTexture = 485,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = false,
                MapChance = 3,
                LootTableKey = "U",
                CastsMagic = true,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 3, 2, 1, 0, -1, 5, 4, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 3, 2, 1, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, -1, 5, 4, 0 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Spellsword
            new MobileEnemy()
            {
                ID = 129,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 476,
                FemaleTexture = 475,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,       // Female has RangedAttack2, male variant does not. Setting false for consistency.
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "P",
                CastsMagic = true,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 5, 4, 3, -1, 2, 1, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, 1, -1, 2, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Battlemage
            new MobileEnemy()
            {
                ID = 130,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 490,
                FemaleTexture = 489,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = true,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 2,
                LootTableKey = "U",
                CastsMagic = true,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Sorcerer
            new MobileEnemy()
            {
                ID = 131,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 478,
                FemaleTexture = 477,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = false,
                MapChance = 3,
                LootTableKey = "U",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, 2, -1, 3, 4, 5 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 4, 5, -1, 3, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Healer
            new MobileEnemy()
            {
                ID = 132,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 486,
                FemaleTexture = 485,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = false,
                MapChance = 1,
                LootTableKey = "U",
                CastsMagic = true,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 3, 2, 1, 0, -1, 5, 4, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 3, 2, 1, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 0, -1, 5, 4, 0 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Nightblade
            new MobileEnemy()
            {
                ID = 133,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 490,
                FemaleTexture = 489,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = true,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "U",
                CastsMagic = true,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                HasSpellAnimation = true,
                SpellAnimFrames = new int[] { 0, 1, 2, 3, 3 },
                Team = MobileTeams.Criminals,
            },

            // Bard
            new MobileEnemy()
            {
                ID = 134,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 484,
                FemaleTexture = 483,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 2,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Burglar
            new MobileEnemy()
            {
                ID = 135,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 484,
                FemaleTexture = 483,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
            },

            // Rogue
            new MobileEnemy()
            {
                ID = 136,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 480,
                FemaleTexture = 479,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
            },

            // Acrobat
            new MobileEnemy()
            {
                ID = 137,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 484,
                FemaleTexture = 483,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 0,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Thief
            new MobileEnemy()
            {
                ID = 138,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 484,
                FemaleTexture = 483,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 2,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
            },

            // Assassin
            new MobileEnemy()
            {
                ID = 139,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 480,
                FemaleTexture = 479,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 0,
                LootTableKey = "O",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 4, 4, -1, 5, 0, 0 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 4, -1, 5, 0, 0, 1, -1, 2, 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
            },

            // Monk
            new MobileEnemy()
            {
                ID = 140,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 488,
                FemaleTexture = 487,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "T",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 0, 1, -1, 2, 2, 1, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 5, 5, 3, -1, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Archer
            new MobileEnemy()
            {
                ID = 141,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 482,
                FemaleTexture = 481,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                PrefersRanged = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 0,
                LootTableKey = "C",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Ranger
            new MobileEnemy()
            {
                ID = 142,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 482,
                FemaleTexture = 481,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "C",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4, -1, 5 },
                ChanceForAttack2 = 50,
                PrimaryAttackAnimFrames2 = new int[] { 3, 4, -1, 5, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Barbarian
            new MobileEnemy()
            {
                ID = 143,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 488,
                FemaleTexture = 487,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 0,
                LootTableKey = "T",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 0, 1, -1, 2, 2, 1, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 5, 5, 3, -1, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.Criminals,
            },

            // Warrior
            new MobileEnemy()
            {
                ID = 144,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 488,
                FemaleTexture = 487,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 0,
                LootTableKey = "T",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 0, 1, -1, 2, 2, 1, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 5, 5, 3, -1, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // Knight
            new MobileEnemy()
            {
                ID = 145,
                Behaviour = MobileBehaviour.General,
                Affinity = MobileAffinity.Human,
                MaleTexture = 488,
                FemaleTexture = 487,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = true,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.EnemyHumanMove,
                BarkSound = (int)SoundClips.EnemyHumanBark,
                AttackSound = (int)SoundClips.EnemyHumanAttack,
                ParrySounds = true,
                MapChance = 1,
                LootTableKey = "T",
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 0, 1, -1, 2, 2, 1, 0 },
                ChanceForAttack2 = 33,
                PrimaryAttackAnimFrames2 = new int[] { 0, 1, -1, 2, 3, 4, 5 },
                ChanceForAttack3 = 33,
                PrimaryAttackAnimFrames3 = new int[] { 5, 5, 3, -1, 2, 1, 0 },
                RangedAttackAnimFrames = new int[] { 3, 2, 0, 0, 0, -1, 1, 1, 2, 3 },
                Team = MobileTeams.KnightsAndMages,
            },

            // City Watch - The Haltmeister
            new MobileEnemy()
            {
                ID = 146,
                Behaviour = MobileBehaviour.Guard,
                Affinity = MobileAffinity.Human,
                MaleTexture = 399,
                FemaleTexture = 399,
                CorpseTexture = CorpseTexture(380, 1),
                HasIdle = true,
                HasRangedAttack1 = false,
                HasRangedAttack2 = false,
                CanOpenDoors = true,
                MoveSound = (int)SoundClips.None,
                BarkSound = (int)SoundClips.Halt,
                AttackSound = (int)SoundClips.None,
                ParrySounds = true,
                MapChance = 0,
                CastsMagic = false,
                PrimaryAttackAnimFrames = new int[] { 0, 1, -1, 2, 3, 4 },
                Team = MobileTeams.CityWatch,
            },
        };

        #endregion

        #region Enemy Damage Type Resistance

        public static float HumanClassPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {


            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                if (armor != null && armor.shear >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else if (damType == 3)
            {
                if (armor != null && armor.density >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float RatPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1.50f;
            }
            else if (damType == 2)
            {
                return 1.50f;
            }
            else if (damType == 3)
            {
                return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float GiantBatPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1.50f;
            }
            else if (damType == 2)
            {
                return 1.50f;
            }
            else if (damType == 3)
            {
                return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float GrizzlyBearPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                return 1.50f;
            }
            else if (damType == 3)
            {
                return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float SabertoothTigerPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                return 1.50f;
            }
            else if (damType == 3)
            {
                return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float SpiderPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1.50f;
            }
            else if (damType == 2)
            {
                return 1f;
            }
            else if (damType == 3)
            {
                return 0.65f;
            }
            else
            {
                return 1f;
            }
        }

        public static float SlaughterfishPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                return 1f;
            }
            else if (damType == 3)
            {
                return 1f;
            }
            else
            {
                return 1f;
            }
        }

        public static float GiantScorpionPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1.50f;
            }
            else if (damType == 2)
            {
                return 1f;
            }
            else if (damType == 3)
            {
                return 0.65f;
            }
            else
            {
                return 1f;
            }
        }

        public static float ImpPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                if (armor != null && armor.fracture >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else if (damType == 2)
            {
                if (armor != null && armor.shear >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else if (damType == 3)
            {
                if (armor != null && armor.density >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float SprigganPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 0.65f;
            }
            else if (damType == 2)
            {
                return 1.50f;
            }
            else if (damType == 3)
            {
                return 0.32f;
            }
            else
            {
                return 1f;
            }
        }

        public static float CentaurPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                if (armor != null && armor.shear >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else if (damType == 3)
            {
                if (armor != null && armor.density >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float NymphTigerPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                if (armor != null && armor.fracture >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else if (damType == 2)
            {
                if (armor != null && armor.shear >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else if (damType == 3)
            {
                if (armor != null && armor.density >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float HarpyPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                return 1.50f;
            }
            else if (damType == 3)
            {
                return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float GiantPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                if (armor != null && armor.shear >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else if (damType == 3)
            {
                if (armor != null && armor.density >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float GargoylePhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 2f;
            }
            else if (damType == 2)
            {
                return 0.65f;
            }
            else if (damType == 3)
            {
                return 0.32f;
            }
            else
            {
                return 1f;
            }
        }

        public static float DragonlingPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1.50f;
            }
            else if (damType == 2)
            {
                return 1f;
            }
            else if (damType == 3)
            {
                return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float LargeDragonlingPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                return 0.65f;
            }
            else if (damType == 3)
            {
                return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float DreughPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1.50f;
            }
            else if (damType == 2)
            {
                return 0.65f;
            }
            else if (damType == 3)
            {
                return 1f;
            }
            else
            {
                return 1f;
            }
        }

        public static float LamiaPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1.50f;
            }
            else if (damType == 2)
            {
                return 1f;
            }
            else if (damType == 3)
            {
                return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float OrcPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                return 0.65f;
            }
            else if (damType == 3)
            {
                return 1f;
            }
            else
            {
                return 1f;
            }
        }

        public static float OrcSergeantPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                return 0.65f;
            }
            else if (damType == 3)
            {
                return 1f;
            }
            else
            {
                return 1f;
            }
        }

        public static float OrcShamanPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                if (armor != null && armor.fracture >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else if (damType == 2)
            {
                if (armor != null && armor.shear >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else if (damType == 3)
            {
                if (armor != null && armor.density >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float OrcWarlordPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                return 0.65f;
            }
            else if (damType == 3)
            {
                return 1f;
            }
            else
            {
                return 1f;
            }
        }

        public static float WerewolfPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                return 1.50f;
            }
            else if (damType == 3)
            {
                return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float WereboarPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                return 1.50f;
            }
            else if (damType == 3)
            {
                return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float FireAtronachPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                return 1f;
            }
            else if (damType == 3)
            {
                return 1f;
            }
            else
            {
                return 1f;
            }
        }

        public static float IronAtronachPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 0.32f;
            }
            else if (damType == 2)
            {
                return 1f;
            }
            else if (damType == 3)
            {
                return 0.65f;
            }
            else
            {
                return 1f;
            }
        }

        public static float FleshAtronachPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                return 1.50f;
            }
            else if (damType == 3)
            {
                return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float IceAtronachPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 2f;
            }
            else if (damType == 2)
            {
                return 1f;
            }
            else if (damType == 3)
            {
                return 0.65f;
            }
            else
            {
                return 1f;
            }
        }

        public static float SkeletalWarriorPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                if (armor != null && armor.fracture >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 2f;
            }
            else if (damType == 2)
            {
                return 1f;
            }
            else if (damType == 3)
            {
                return 0.32f;
            }
            else
            {
                return 1f;
            }
        }

        public static float ZombiePhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                if (armor != null && armor.shear >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else if (damType == 3)
            {
                return 0.65f;
            }
            else
            {
                return 1f;
            }
        }

        public static float GhostPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 0.65f;
            }
            else if (damType == 2)
            {
                return 0.65f;
            }
            else if (damType == 3)
            {
                return 0.65f;
            }
            else
            {
                return 1f;
            }
        }

        public static float MummyPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                return 1.50f;
            }
            else if (damType == 3)
            {
                return 0.65f;
            }
            else
            {
                return 1f;
            }
        }

        public static float WraithPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 0.65f;
            }
            else if (damType == 2)
            {
                return 0.65f;
            }
            else if (damType == 3)
            {
                return 0.65f;
            }
            else
            {
                return 1f;
            }
        }

        public static float VampirePhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                if (armor != null && armor.shear >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else if (damType == 3)
            {
                if (armor != null && armor.density >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float VampireAncientPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                if (armor != null && armor.shear >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else if (damType == 3)
            {
                if (armor != null && armor.density >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float LichPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                if (armor != null && armor.fracture >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 2f;
            }
            else if (damType == 2)
            {
                return 1f;
            }
            else if (damType == 3)
            {
                return 0.32f;
            }
            else
            {
                return 1f;
            }
        }

        public static float AncientLichPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                if (armor != null && armor.fracture >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 2f;
            }
            else if (damType == 2)
            {
                return 1f;
            }
            else if (damType == 3)
            {
                return 0.32f;
            }
            else
            {
                return 1f;
            }
        }

        public static float FrostDaedraPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                return 0.65f;
            }
            else if (damType == 3)
            {
                return 1f;
            }
            else
            {
                return 1f;
            }
        }

        public static float FireDaedraPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 0.65f;
            }
            else if (damType == 2)
            {
                return 0.65f;
            }
            else if (damType == 3)
            {
                return 1f;
            }
            else
            {
                return 1f;
            }
        }

        public static float DaedrothPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 1f;
            }
            else if (damType == 2)
            {
                return 0.65f;
            }
            else if (damType == 3)
            {
                return 1f;
            }
            else
            {
                return 1f;
            }
        }

        public static float DaedraSeducerPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                if (armor != null && armor.fracture >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else if (damType == 2)
            {
                if (armor != null && armor.shear >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else if (damType == 3)
            {
                if (armor != null && armor.density >= 300 || shieldBlockSuccess)
                    return 1f;
                else
                    return 1.50f;
            }
            else
            {
                return 1f;
            }
        }

        public static float DaedraLordPhysicalDamTypeWeaknesses(DaggerfallEntity tarEnemy, int struckBodyPart, int damType, bool shieldBlockSuccess, DaggerfallUnityItem armor = null)
        {
            

            if (damType == 1)
            {
                return 0.65f;
            }
            else if (damType == 2)
            {
                return 0.65f;
            }
            else if (damType == 3)
            {
                return 1f;
            }
            else
            {
                return 1f;
            }
        }

        #endregion

        #region Enemy Stat Hack Edits

        /// <summary>
        /// Custom values assigned to specific enemy entities on the fly for use in combat formula.
        /// </summary>
        public static CustomEnemyStatValues EnemyCustomAttributeCalculator(DaggerfallEntity enemy)
        {
            CustomEnemyStatValues values = new CustomEnemyStatValues();
            values.weaponSkillCustom = 30;
            values.critSkillCustom = 30;
            values.dodgeSkillCustom = 0;
            values.strengthCustom = 50;
            values.agilityCustom = 50;
            values.speedCustom = 50;
            values.willpowerCustom = 50;
            values.luckCustom = 50;

            EnemyEntity AITarget = enemy as EnemyEntity;

            if (AITarget.EntityType == EntityTypes.EnemyClass)
            {
                switch (AITarget.CareerIndex)
                {
                    case (int)ClassCareers.Mage:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -30;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case (int)ClassCareers.Spellsword:
                    case (int)ClassCareers.Battlemage:
                    case (int)ClassCareers.Sorcerer:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -20;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case (int)ClassCareers.Healer:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -30;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case (int)ClassCareers.Nightblade:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -15;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case (int)ClassCareers.Bard:
                    case (int)ClassCareers.Burglar:
                    case (int)ClassCareers.Rogue:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -10;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case (int)ClassCareers.Acrobat:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 5;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case (int)ClassCareers.Thief:
                    case (int)ClassCareers.Assassin:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -5;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case (int)ClassCareers.Monk:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 0;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case (int)ClassCareers.Archer:
                    case (int)ClassCareers.Ranger:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -15;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case (int)ClassCareers.Barbarian:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -20;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case (int)ClassCareers.Warrior:
                    case (int)ClassCareers.Knight:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -25;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    default:
                        return values;
                }
            }
            else
            {
                switch (AITarget.CareerIndex)
                {
                    case 0:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -30;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 3:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -10;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 4:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -35;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 5:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -25;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 6:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -20;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 11:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -20;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 20:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -20;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 1:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -15;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 2:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 10;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 8:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -30;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 10:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 5;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 13:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -15;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 16:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -35;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 22:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -15;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 34:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 0;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 40:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 10;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 41:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -20;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 42:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -10;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 7:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -35;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 12:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -25;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 21:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -15;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 24:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -10;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 9:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -15;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 14:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -20;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 35:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -15;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 36:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -25;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 37:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -35;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 38:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -20;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 15:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -20;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 17:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -40;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 18:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -5;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 19:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = -15;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 23:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 0;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 28:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 15;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 30:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 35;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 32:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 10;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 33:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 20;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 25:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 30;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 26:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 15;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 27:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 5;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 29:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 40;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    case 31:
                        values.weaponSkillCustom = 30;
                        values.critSkillCustom = 30;
                        values.dodgeSkillCustom = 40;
                        values.strengthCustom = 50;
                        values.agilityCustom = 50;
                        values.speedCustom = 50;
                        values.willpowerCustom = 50;
                        values.luckCustom = 50;
                        return values;
                    default:
                        return values;
                }
            }
        }

        /// <summary>
        /// This is literally there just to initialize this struct in another method, because I can't figure out how to do what I want with it otherwise, hopefully it works out.
        /// </summary>
        public static CustomEnemyStatValues EnemyCustomAttributeInitializer(DaggerfallEntity enemy)
        {
            CustomEnemyStatValues values = new CustomEnemyStatValues();
            values.weaponSkillCustom = 30;
            values.critSkillCustom = 30;
            values.dodgeSkillCustom = 0;
            values.strengthCustom = 50;
            values.agilityCustom = 50;
            values.speedCustom = 50;
            values.willpowerCustom = 50;
            values.luckCustom = 50;
            return values;
        }

        #endregion

        #region Helpers

        public static int CorpseTexture(int archive, int record)
        {
            return ((archive << 16) + record);
        }

        public static void ReverseCorpseTexture(int corpseTexture, out int archive, out int record)
        {
            archive = corpseTexture >> 16;
            record = corpseTexture & 0xffff;
        }

        /// <summary>
        /// Build a dictionary of enemies keyed by ID.
        /// Use this once and store for faster enemy lookups.
        /// </summary>
        /// <returns>Resulting dictionary of mobile enemies.</returns>
        public static Dictionary<int, MobileEnemy> BuildEnemyDict()
        {
            Dictionary<int, MobileEnemy> enemyDict = new Dictionary<int, MobileEnemy>();
            foreach (var enemy in Enemies)
            {
                enemyDict.Add(enemy.ID, enemy);
            }

            return enemyDict;
        }

        /// <summary>
        /// Gets enemy definition based on type.
        /// Runs a brute force search for ID, so use sparingly.
        /// Store a dictionary from GetEnemyDict() for faster lookups.
        /// </summary>
        /// <param name="enemyType">Enemy type to extract definition.</param>
        /// <param name="mobileEnemyOut">Receives details of enemy type.</param>
        /// <returns>True if successful.</returns>
        public static bool GetEnemy(MobileTypes enemyType, out MobileEnemy mobileEnemyOut)
        {
            // Cast type enum to ID.
            // You can add additional IDs to enum to create new enemies.
            int id = (int)enemyType;

            // Search for matching definition in enemy list.
            // Don't forget to add new enemy IDs to Enemies definition array.
            for (int i = 0; i < Enemies.Length; i++)
            {
                if (Enemies[i].ID == id)
                {
                    mobileEnemyOut = Enemies[i];
                    return true;
                }
            }

            // No match found, just return an empty definition
            mobileEnemyOut = new MobileEnemy();
            return false;
        }

        /// <summary>
        /// Gets enemy definition based on name.
        /// Runs a brute force search for ID, so use sparingly.
        /// </summary>
        /// <param name="name">Enemy name to extract definition.</param>
        /// <param name="mobileEnemyOut">Receives details of enemy type if found.</param>
        /// <returns>True if successful.</returns>
        public static bool GetEnemy(string name, out MobileEnemy mobileEnemyOut)
        {
            for (int i = 0; i < Enemies.Length; i++)
            {
                if (0 == string.Compare(TextManager.Instance.GetLocalizedEnemyName(Enemies[i].ID), name, StringComparison.InvariantCultureIgnoreCase))
                {
                    mobileEnemyOut = Enemies[i];
                    return true;
                }
            }

            // No match found, just return an empty definition
            mobileEnemyOut = new MobileEnemy();
            return false;
        }

        #endregion

    }
}
