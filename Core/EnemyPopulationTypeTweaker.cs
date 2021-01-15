using BepInEx.Logging;
using GameData;
using GTFO_Difficulty_Tweaker;
using GTFO_Difficulty_Tweaker.Data;
using GTFO_DIfficulty_Tweaker.Config;
using GTFO_DIfficulty_Tweaker.Data;
using GTFO_DIfficulty_Tweaker.Util;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;

namespace GTFO_DIfficulty_Tweaker.Core
{
    public static class EnemyPopulationTypeTweaker
    {
        public static void HandleEnemyPopChanges(LG_PopulateArea __instance)
        {
            if (!SNet.IsMaster || SpawnModeConfiguration.currentMode == null)
            {
                return;
            }
            Dictionary<int, float> enemyIDs = SpawnModeConfiguration.currentMode.possibleEnemyTypes;
            if (enemyIDs == null || enemyIDs.Count < 1)
            {
                LoggerWrapper.Log("No enemy population tweaks, aborting setup...");
                return;
            }

            // Not working as of yet, game will require restart if switching back to mode with no population tweaks
            //EnemyGroupData.TryAddDataBlock(__instance.m_groupPlacement.groupData);
            SetupRandomizedGroupsByIDs(__instance, enemyIDs);
        }


        private static void SetupRandomizedGroupsByIDs(LG_PopulateArea area, Dictionary<int, float> enemyIDs)
        {
            if (enemyIDs == null || enemyIDs.Count < 1)
            {
                LoggerWrapper.Log("No enemy population tweaks, aborting setup...");
                return;
            }

            HashSet<pAvailableEnemyTypes> validTypes;
           
            validTypes = GetValidGroups(enemyIDs);
           

            Dictionary<pAvailableEnemyTypes, float> weightedGroups = SetupWeighting(validTypes, enemyIDs);

            pAvailableEnemyTypes dataGroup = weightedGroups.RandomElementByWeight(e => e.Value).Key;
            

            LoggerWrapper.Log(dataGroup.ToString(), LogLevel.Debug);

            if(!enemyIDs.ContainsKey((int)dataGroup.enemyID))
            {
                return;
            }

            area.m_groupPlacement.groupData.Difficulty = dataGroup.difficulty;
            area.m_groupPlacement.groupData.Type = GetValidGroupTypeFromRole(dataGroup.role);

            

            foreach (EnemyGroupCompositionData enemyGroupCompositionData in area.m_groupPlacement.groupData.Roles)
            {
                eEnemyRole role = dataGroup.role;
                EnsureValidRole(area.m_groupPlacement.groupData.Type, area.m_groupPlacement.groupData.Difficulty, ref role);
                enemyGroupCompositionData.Role = role;
            }


            LoggerWrapper.Log($"Setting enemyDataGroup: Diff:{area.m_groupPlacement.groupData.Difficulty} Role:{area.m_groupPlacement.groupData.Type} Type:{dataGroup.role}", LogLevel.Debug);
        }

        private static void EnsureValidRole(eEnemyGroupType type, eEnemyRoleDifficulty difficulty, ref eEnemyRole role)
        {
            if(type == eEnemyGroupType.Patrol)
            {
                role = eEnemyRole.Scout;
            }
        }

        private static Dictionary<pAvailableEnemyTypes, float> SetupWeighting(HashSet<pAvailableEnemyTypes> validTypes, Dictionary<int, float> enemyIDs)
        {
            Dictionary<pAvailableEnemyTypes, float> weightedGroups = new Dictionary<pAvailableEnemyTypes, float>();

            Dictionary<uint, List<pAvailableEnemyTypes>> weightedGroupsByID = new Dictionary<uint, List<pAvailableEnemyTypes>>();

            foreach(pAvailableEnemyTypes type in validTypes)
            {
                if(!weightedGroupsByID.ContainsKey(type.enemyID))
                {
                    weightedGroupsByID.Add(type.enemyID, new List<pAvailableEnemyTypes>());
                }
                weightedGroupsByID[type.enemyID].Add(type);
            }

            foreach (pAvailableEnemyTypes group in validTypes)
            {
                float currWeight = enemyIDs[(int)group.enemyID];
                currWeight /= weightedGroupsByID[group.enemyID].Count;
                if(weightedGroups.ContainsKey(group))
                {
                    weightedGroups[group] = currWeight;
                } else
                {
                    weightedGroups.Add(group, currWeight);
                }
            }
            foreach(KeyValuePair<pAvailableEnemyTypes, float> pair in weightedGroups)
            {
                int groupCount = weightedGroupsByID[pair.Key.enemyID].Count;
            }
            return weightedGroups;
        }

        private static HashSet<pAvailableEnemyTypes> GetValidGroups(Dictionary<int, float> enemies)
        {
            List<pAvailableEnemyTypes> valid = new List<pAvailableEnemyTypes>();

            foreach (pAvailableEnemyTypes type in EnemyPopData.GetAvailableSpawnData())
            {
                // ToDo Check validity with EnemyGroupDataBlock
                if (enemies.ContainsKey((int)type.enemyID))
                {
                    valid.Add(type);
                }
            }

            List<pAvailableEnemyTypes> trueValid = new List<pAvailableEnemyTypes>();

            EnsureGroupsOnlyContainValidIDS(enemies, valid, trueValid);
            CheckValidity(enemies, trueValid);

            return new HashSet<pAvailableEnemyTypes>(trueValid);
        }

        private static void EnsureGroupsOnlyContainValidIDS(Dictionary<int,float> enemies, List<pAvailableEnemyTypes> valid, List<pAvailableEnemyTypes> trueValid)
        {
            foreach (pAvailableEnemyTypes validType in valid)
            {
                HashSet<Tuple<eEnemyRole, eEnemyRoleDifficulty>> invalidGroups = new HashSet<Tuple<eEnemyRole, eEnemyRoleDifficulty>>();

                foreach (pAvailableEnemyTypes type in EnemyPopData.GetAvailableSpawnData())
                {

                    if (type.role == validType.role && type.difficulty == validType.difficulty)
                    {
                        if (!enemies.ContainsKey((int)type.enemyID))
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

        private static void CheckValidity(Dictionary<int, float> enemies, List<pAvailableEnemyTypes> trueValid)
        {
            if (trueValid.Count < 1)
            {
                LoggerWrapper.Log("Could not find any groups!");
            }

            foreach (int enemyID in enemies.Keys)
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

        public static void SetEnemyWavePopulationTweak(ref ushort populationDataID, ref ushort settingsID)
        {
            if (SpawnModeConfiguration.currentMode != null)
            {
                if (settingsID == (ushort) SurvivalSpawnWaveSettings.Scout)
                {
                    populationDataID = GetRandomPopulationForScoutWave(populationDataID);

                    settingsID = ApplyPopulationToSettingsMapping(populationDataID, settingsID, SpawnModeConfiguration.currentMode.scoutWavePopulationToWaveSettingsMapping);
                }
                else
                {
                    populationDataID = GetRandomPopulationForNormalWave(populationDataID);
                    settingsID = ApplyPopulationToSettingsMapping(populationDataID, settingsID, SpawnModeConfiguration.currentMode.anyWavePopulationToWaveSettingsMapping);
                }
            }

        }

        private static ushort GetRandomPopulationForNormalWave(ushort populationDataID)
        {
            if (SpawnModeConfiguration.currentMode.possibleWaveSpawns != null && SpawnModeConfiguration.currentMode.possibleWaveSpawns.Count > 0)
            {
                populationDataID = (ushort)SpawnModeConfiguration.currentMode.possibleWaveSpawns.RandomElementByWeight(e => e.Value).Key;
            }

            return populationDataID;
        }

        private static ushort GetRandomPopulationForScoutWave(ushort populationDataID)
        {
            if (SpawnModeConfiguration.currentMode.possibleScoutSpawns != null && SpawnModeConfiguration.currentMode.possibleScoutSpawns.Count > 0)
            {
                populationDataID = (ushort)SpawnModeConfiguration.currentMode.possibleScoutSpawns.RandomElementByWeight(e => e.Value).Key;
            }

            return populationDataID;
        }

        private static ushort ApplyPopulationToSettingsMapping(ushort populationDataID, ushort settingsID, Dictionary<int,int> popToSettings)
        {
            if (popToSettings != null && popToSettings.Count > 1)
            {
                if(popToSettings.ContainsKey(populationDataID))
                {
                    settingsID = (ushort)popToSettings[populationDataID];
                }
            }

            return settingsID;
        }
    }
}

