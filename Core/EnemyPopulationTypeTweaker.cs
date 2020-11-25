using GameData;
using GTFO_Difficulty_Tweaker;
using GTFO_Difficulty_Tweaker.Data;
using GTFO_DIfficulty_Tweaker.Data;
using GTFO_DIfficulty_Tweaker.Util;
using LevelGeneration;
using MelonLoader;
using SNetwork;
using System;
using System.Collections.Generic;

namespace GTFO_DIfficulty_Tweaker.Core
{
    public static class EnemyPopulationTypeTweaker
    {
        public static void HandleEnemyPopChanges(LG_PopulateArea __instance)
        {
            if (!SNet.IsMaster)
            {
                return;
            }

            List<int> enemyIDs;
            List<int> enemyIDsToIgnore;
            enemyIDsToIgnore = new List<int> { (int)EnemyID.Tank, (int)EnemyID.Birther, (int)EnemyID.Birther_Boss };

            switch (SpawnTweakSettings.mode)
            {
                case SpawnTweakerMode.SCOUTS_PLEASE:
                    ReplaceAllEnemiesWithScouts(__instance);
                    break;
                case SpawnTweakerMode.EXTRA_SCOUTS:
                    RandomlyReplaceWithScouts(__instance);
                    break;
                case SpawnTweakerMode.ONLY_SHOOTERS:
                    enemyIDs = new List<int> { (int)EnemyID.Shooter_Big, (int)EnemyID.Shooter_Hibernate, (int)EnemyID.Shooter_Wave };
                    SetupRandomizedGroupsByIDs(__instance, enemyIDs, enemyIDsToIgnore);
                    break;
                case SpawnTweakerMode.ONLY_CHARGERS:
                    enemyIDs = new List<int> { (int)EnemyID.Striker_Bullrush, (int)EnemyID.Striker_Big_Bullrush };
                    SetupRandomizedGroupsByIDs(__instance, enemyIDs, enemyIDsToIgnore);
                    break;
                case SpawnTweakerMode.ONLY_SHADOWS:
                    enemyIDs = new List<int> { (int)EnemyID.Shadow, (int)EnemyID.Scout_Shadow, (int)EnemyID.Striker_Big_Shadow };
                    SetupRandomizedGroupsByIDs(__instance, enemyIDs, enemyIDsToIgnore);
                    break;
                case SpawnTweakerMode.RANDOM:
                    //ToDO --- This is kinda messy and made by hand, could use some automatization but the game balance needs to be adjusted, otherwise you'd have way too many bosses and the like
                    // Alternative --- Create randomizer without bosses
                    RandomlyRandomizeEnemies(__instance);
                    break;
                case SpawnTweakerMode.BALANCED_RANDOM:
                    enemyIDs = new List<int> { (int)EnemyID.Shadow, (int)EnemyID.Scout_Shadow, (int)EnemyID.Striker_Big_Shadow,
                    (int)EnemyID.Scout, (int)EnemyID.Striker_Big_Bullrush,
                    (int)EnemyID.Shooter_Hibernate,(int)EnemyID.Striker_Hibernate, (int)EnemyID.Striker_Big_Hibernate,(int)EnemyID.Shooter_Big,
                    (int)EnemyID.Striker_Bullrush, (int)EnemyID.Shooter_Big_RapidFire};

                    SetupRandomizedGroupsByIDs(__instance, enemyIDs, enemyIDsToIgnore);
                    break;
                default:
                    break;
            }
        }

        public static ushort GetEnemyWavePopulationTweak(ushort populationDataID)
        {
            switch (SpawnTweakSettings.mode)
            {
                case (SpawnTweakerMode.RANDOM):
                    populationDataID = SetRandomPopulationID(populationDataID);
                    break;
                case (SpawnTweakerMode.BALANCED_RANDOM):
                    populationDataID = SetRandomPopulationID(populationDataID);
                    break;
                case (SpawnTweakerMode.ONLY_SHOOTERS):
                    populationDataID = (ushort)GD.SurvivalWavePopulation.Shooters_Only;
                    break;
                case (SpawnTweakerMode.ONLY_SHADOWS):
                    populationDataID = (ushort)GD.SurvivalWavePopulation.Shadows_only;
                    break;
                default:
                    break;
            }
            return populationDataID;
        }

        private static ushort SetRandomPopulationID(ushort populationDataID)
        {

            int randomVal = UnityEngine.Random.Range(0, 5);

            if (randomVal == 0)
            {
                populationDataID = (ushort)GD.SurvivalWavePopulation.Shooters_Only;
            }

            if (randomVal == 1)
            {
                populationDataID = (ushort)GD.SurvivalWavePopulation.Baseline;
            }

            if (randomVal == 2)
            {
                populationDataID = (ushort)GD.SurvivalWavePopulation.Baseline_big;
            }

            if (randomVal == 3)
            {
                populationDataID = (ushort)GD.SurvivalWavePopulation.Shadows_only;
            }

            if (randomVal == 4)
            {
                populationDataID = (ushort)GD.SurvivalWavePopulation.Bullrush_Only;
            }

            LoggerWrapper.Log("SETTING ALARM POP_ID " + populationDataID, LogLevel.Info);


            return populationDataID;
        }

        private static void RandomlyRandomizeEnemies(LG_PopulateArea __instance)
        {
            eEnemyRole role = GetRandomRole();

            foreach (EnemyGroupCompositionData enemyGroupCompositionData in __instance.m_groupPlacement.groupData.Roles)
            {
                enemyGroupCompositionData.Role = role;

            }
            __instance.m_groupPlacement.groupData.Type = GetValidGroupTypeFromRole(role);
            __instance.m_groupPlacement.groupData.Difficulty = GetRandomDifficulty();

            if (role.Equals(eEnemyRole.Scout))
            {
                __instance.m_groupPlacement.groupData.Difficulty = UnityEngine.Random.value > 0.3f ? eEnemyRoleDifficulty.Boss : eEnemyRoleDifficulty.Hard;
            }

            if (role.Equals(eEnemyRole.MiniBoss))
            {
                __instance.m_groupPlacement.groupData.Difficulty = UnityEngine.Random.value > 0.9 ? eEnemyRoleDifficulty.MiniBoss : eEnemyRoleDifficulty.MegaBoss;
            }

            if (role.Equals(eEnemyRole.Melee) && UnityEngine.Random.value > 0.5f)
            {
                __instance.m_groupPlacement.groupData.Difficulty = eEnemyRoleDifficulty.Biss;
            }

            if (role.Equals(eEnemyRole.Boss))
            {
                __instance.m_groupPlacement.groupData.Difficulty = UnityEngine.Random.value > 0.3f ? eEnemyRoleDifficulty.Boss : eEnemyRoleDifficulty.MegaBoss;
            }
        }

        private static void RandomlyReplaceWithScouts(LG_PopulateArea __instance)
        {
            if (UnityEngine.Random.Range(0f, 1f) < SpawnTweakSettings.scoutChance)
            {
                foreach (EnemyGroupCompositionData enemyGroupCompositionData in __instance.m_groupPlacement.groupData.Roles)
                {
                    enemyGroupCompositionData.Role = eEnemyRole.Scout;
                }
                __instance.m_groupPlacement.groupData.Type = eEnemyGroupType.Patrol;
                __instance.m_groupPlacement.groupData.Difficulty = UnityEngine.Random.value > 0.9f ? eEnemyRoleDifficulty.Hard : eEnemyRoleDifficulty.Medium;

            }
        }

        private static void ReplaceAllEnemiesWithScouts(LG_PopulateArea __instance)
        {
            foreach (EnemyGroupCompositionData enemyGroupCompositionData in __instance.m_groupPlacement.groupData.Roles)
            {
                enemyGroupCompositionData.Role = eEnemyRole.Scout;

            }
            __instance.m_groupPlacement.groupData.Type = eEnemyGroupType.Patrol;
            __instance.m_groupPlacement.groupData.Difficulty = UnityEngine.Random.value > 0.3f ? eEnemyRoleDifficulty.Hard : eEnemyRoleDifficulty.Boss;
        }

        private static void SetupRandomizedGroupsByIDs(LG_PopulateArea __instance, List<int> enemyIDS, List<int> ignoreList = null)
        {
            List<pAvailableEnemyTypes> validTypes = GetValidGroups(enemyIDS);
            List<pAvailableEnemyTypes> ignoredGroups = GetValidGroups(ignoreList);


            pAvailableEnemyTypes dataGroup = GetRandomEnemyGroup(validTypes);

            foreach (EnemyGroupCompositionData enemyGroupCompositionData in __instance.m_groupPlacement.groupData.Roles)
            {
                foreach (pAvailableEnemyTypes type in ignoredGroups)
                {
                    if (__instance.m_groupPlacement.groupData.Difficulty == type.difficulty && enemyGroupCompositionData.Role == type.role)
                    {
                        LoggerWrapper.Log($"Ignoring enemyDataGroup: Diff:{ __instance.m_groupPlacement.groupData.Difficulty} Role:{__instance.m_groupPlacement.groupData.Type} Type:{dataGroup.role}", LogLevel.Debug);
                        return;
                    }
                }

                enemyGroupCompositionData.Role = dataGroup.role;
            }

            __instance.m_groupPlacement.groupData.Type = GetValidGroupTypeFromRole(dataGroup.role);
            __instance.m_groupPlacement.groupData.Difficulty = dataGroup.difficulty;

            LoggerWrapper.Log($"Setting enemyDataGroup: Diff:{__instance.m_groupPlacement.groupData.Difficulty} Role:{__instance.m_groupPlacement.groupData.Type} Type:{dataGroup.role}", LogLevel.Debug);


        }

        private static pAvailableEnemyTypes GetRandomEnemyGroup(List<pAvailableEnemyTypes> validGroups)
        {
            Dictionary<pAvailableEnemyTypes, float> weightedPairs = new Dictionary<pAvailableEnemyTypes, float>();

            foreach (pAvailableEnemyTypes type in validGroups)
            {
                if (!weightedPairs.ContainsKey(type))
                {
                    weightedPairs.Add(type, (float)type.weight);
                }
            }

            pAvailableEnemyTypes dataGroup = weightedPairs.RandomElementByWeight(e => e.Value).Key;
            return dataGroup;
        }

        private static List<pAvailableEnemyTypes> GetValidGroups(List<int> enemies)
        {
            List<pAvailableEnemyTypes> valid = new List<pAvailableEnemyTypes>();

            if (enemies == null || enemies.Count < 1)
            {
                return valid;
            }

            foreach (pAvailableEnemyTypes type in EnemyGroupData.enemyTypes)
            {
                // ToDo Check validity with EnemyGroupDataBlock
                if (enemies.Contains((int)type.enemyID) && type.role != eEnemyRole.Tank)
                {
                    valid.Add(type);
                }
            }

            List<pAvailableEnemyTypes> trueValid = new List<pAvailableEnemyTypes>();

            EnsureGroupsOnlyContainValidIDS(enemies, valid, trueValid);
            CheckValidity(enemies, trueValid);

            return trueValid;
        }

        private static void EnsureGroupsOnlyContainValidIDS(List<int> enemies, List<pAvailableEnemyTypes> valid, List<pAvailableEnemyTypes> trueValid)
        {
            foreach (pAvailableEnemyTypes validType in valid)
            {

                HashSet<Tuple<eEnemyRole, eEnemyRoleDifficulty>> invalidGroups = new HashSet<Tuple<eEnemyRole, eEnemyRoleDifficulty>>();

                foreach (pAvailableEnemyTypes type in EnemyGroupData.enemyTypes)
                {

                    if (type.role == validType.role && type.difficulty == validType.difficulty)
                    {
                        if (!enemies.Contains((int)type.enemyID))
                        {
                            invalidGroups.Add(new Tuple<eEnemyRole, eEnemyRoleDifficulty>(type.role, type.difficulty));
                        }
                    }
                }

                bool isValid = true;
                foreach (Tuple<eEnemyRole, eEnemyRoleDifficulty> invalidGroup in invalidGroups)
                {
                    if (validType.role == invalidGroup.Item1 && validType.difficulty == invalidGroup.Item2)
                    {
                        isValid = false;
                        if (!isValid)
                        {
                            break;
                        }
                    }
                }

                if (isValid)
                {
                    trueValid.Add(validType);
                }
            }
        }

        private static void CheckValidity(List<int> enemies, List<pAvailableEnemyTypes> trueValid)
        {
            if (trueValid.Count < 1)
            {
                LoggerWrapper.Log("Could not find any groups!");
            }

            foreach (int enemyID in enemies)
            {
                bool enemyIDHandled = false;
                foreach (pAvailableEnemyTypes type in trueValid)
                {
                    if (type.enemyID == enemyID)
                    {
                        enemyIDHandled = true;
                    }
                }
                if (!enemyIDHandled)
                {
                    LoggerWrapper.Log($"COULD NOT FIND SPAWN GROUP FOR ENEMY ID {enemyID} !");
                }
            }
        }

        private static eEnemyGroupType GetValidGroupTypeFromRole(eEnemyRole role)
        {
            if (role.Equals(eEnemyRole.Scout))
            {
                return eEnemyGroupType.Patrol;
            }

            if (role.Equals(eEnemyRole.Lurker))
            {
                return eEnemyGroupType.PureSneak;
            }

            return eEnemyGroupType.Hibernate;
        }

        private static eEnemyRoleDifficulty GetRandomDifficulty()
        {
            var val = UnityEngine.Random.value;

            if (val > 0.66f)
            {
                return eEnemyRoleDifficulty.Hard;
            }

            if (val > 0.33f)
            {
                return eEnemyRoleDifficulty.Medium;
            }

            return eEnemyRoleDifficulty.Easy;
        }

        private static eEnemyRole GetRandomRole()
        {
            float rand = UnityEngine.Random.value;

            if (rand <= 0.01)
            {
                return eEnemyRole.Boss;
            }
            if (rand <= 0.035)
            {
                return eEnemyRole.MiniBoss;
            }
            if (rand <= 0.2)
            {
                return eEnemyRole.Scout;
            }

            if (rand <= 0.4)
            {
                return eEnemyRole.Lurker;
            }

            if (rand <= 0.6)
            {
                return eEnemyRole.Ranged;
            }

            if (rand <= 0.8)
            {
                return eEnemyRole.Melee;
            }

            if (rand <= 0.9)
            {
                return eEnemyRole.PureSneak;
            }

            return eEnemyRole.PureSneak;
        }
    }
}

