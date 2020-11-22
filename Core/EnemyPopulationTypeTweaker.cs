using GameData;
using GTFO_Difficulty_Tweaker;
using GTFO_Difficulty_Tweaker.Data;
using GTFO_DIfficulty_Tweaker.Data;
using GTFO_DIfficulty_Tweaker.Util;
using LevelGeneration;
using MelonLoader;
using SNetwork;
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

            switch (SpawnTweakSettings.mode)
            {
                case (SpawnTweakerMode.SCOUTS_PLEASE):
                    ReplaceAllEnemiesWithScouts(__instance);
                    break;
                case (SpawnTweakerMode.EXTRA_SCOUTS):
                    RandomlyReplaceWithScouts(__instance);
                    break;
                case (SpawnTweakerMode.ONLY_SHOOTERS):
                    enemyIDs = new List<int> { (int)EnemyID.Shooter_Big, (int)EnemyID.Shooter_Hibernate, (int)EnemyID.Shooter_Wave };
                    SetupRandomizedGroupsByIDs(__instance, enemyIDs);
                    break;
                case (SpawnTweakerMode.ONLY_CHARGERS):
                    enemyIDs = new List<int> { (int)EnemyID.Striker_Bullrush, (int)EnemyID.Striker_Big_Bullrush, (int)EnemyID.Tank };
                    SetupRandomizedGroupsByIDs(__instance, enemyIDs);
                    break;
                case (SpawnTweakerMode.ONLY_SHADOWS):
                    enemyIDs = new List<int> { (int)EnemyID.Shadow, (int)EnemyID.Scout_Shadow, (int)EnemyID.Striker_Big_Shadow };
                    SetupRandomizedGroupsByIDs(__instance, enemyIDs);
                    break;
                case (SpawnTweakerMode.RANDOM):
                    RandomlyRandomizeEnemies(__instance);
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
            if (SpawnTweakSettings.mode == SpawnTweakerMode.RANDOM)
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
            }

            return populationDataID;
        }

        private static void RandomlyRandomizeEnemies(LG_PopulateArea __instance)
        {
            if (UnityEngine.Random.value > 0.4f)
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

                if (role.Equals(eEnemyRole.Melee) && UnityEngine.Random.value > 0.8f)
                {
                    __instance.m_groupPlacement.groupData.Difficulty = eEnemyRoleDifficulty.Biss;
                }

                if (role.Equals(eEnemyRole.Boss))
                {
                    __instance.m_groupPlacement.groupData.Difficulty = eEnemyRoleDifficulty.Boss;
                }
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

        private static void SetupRandomizedGroupsByIDs(LG_PopulateArea __instance, List<int> enemyIDS)
        {
            List<pAvailableEnemyTypes> validTypes = GetValidGroups(enemyIDS);
            pAvailableEnemyTypes dataGroup = GetRandomEnemyGroup(validTypes);

            foreach (EnemyGroupCompositionData enemyGroupCompositionData in __instance.m_groupPlacement.groupData.Roles)
            {
                enemyGroupCompositionData.Role = dataGroup.role;
            }
            __instance.m_groupPlacement.groupData.Type = GetValidGroupTypeFromRole(dataGroup.role);
            __instance.m_groupPlacement.groupData.Difficulty = dataGroup.difficulty;

            LoggerWrapper.Log($"Setting enemyDataGroup: Diff:{dataGroup.difficulty} Role:{__instance.m_groupPlacement.groupData.Type} Type:{dataGroup.role}", LogLevel.Debug);
        }

        private static pAvailableEnemyTypes GetRandomEnemyGroup(List<pAvailableEnemyTypes> validGroups)
        {
            Dictionary<pAvailableEnemyTypes, float> weightedPairs = new Dictionary<pAvailableEnemyTypes, float>();

            foreach (pAvailableEnemyTypes type in validGroups)
            {
                weightedPairs.Add(type, (float)type.difficulty);
            }

            pAvailableEnemyTypes dataGroup = weightedPairs.RandomElementByWeight(e => e.Value).Key;
            return dataGroup;
        }

        private static List<pAvailableEnemyTypes> GetValidGroups(List<int> enemies)
        {
            List<pAvailableEnemyTypes> valid = new List<pAvailableEnemyTypes>();

            foreach (pAvailableEnemyTypes type in EnemyGroupData.enemyTypes)
            {
                if (enemies.Contains((int)type.enemyID))
                {
                    valid.Add(type);
                }
            }

            List<pAvailableEnemyTypes> trueValid = new List<pAvailableEnemyTypes>();
            
            foreach(pAvailableEnemyTypes validType in valid)
            {
                foreach (pAvailableEnemyTypes type in EnemyGroupData.enemyTypes)
                {
                    if (type.role == validType.role && type.difficulty == validType.difficulty)
                    {
                        if(!enemies.Contains((int)type.enemyID))
                        {

                        }
                    }
                }
            }
            
            return valid;
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

