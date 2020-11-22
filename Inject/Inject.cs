using AIGraph;
using Enemies;
using GameData;
using GTFO_Difficulty_Tweaker.Data;
using GTFO_DIfficulty_Tweaker;
using GTFO_DIfficulty_Tweaker.Core;
using GTFO_DIfficulty_Tweaker.Data;
using GTFO_DIfficulty_Tweaker.Util;
using Harmony;
using LevelGeneration;
using MelonLoader;
using SNetwork;
using System;
using System.Collections.Generic;
using UnhollowerBaseLib;
using UnityEngine;
using static GTFO_Difficulty_Tweaker.SpawnTweakSettings;

namespace GTFO_Difficulty_Tweaker
{

    [HarmonyPatch(typeof(EnemyPopulationDataBlock), "SetupEnemyDataLookup")]
    class InjectCustomPopData
    {
        static bool injected = false;
        static void Prefix(EnemyPopulationDataBlock __instance)
        {
            EnemyDataBlockGroupInjector.InjectCustomEnemies(__instance);
        }
    }
    
    [HarmonyPatch(typeof(EnemyPopulationDataBlock), "SetupEnemyDataLookup")]
    class InjectDebugPossibleEnemies
    {
        static void Postfix(EnemyPopulationDataBlock __instance)
        {
            EnemyGroupData.SetupData(__instance);
        }
    }


    [HarmonyPatch(typeof(EnemyPrefabManager), "BuildEnemyPrefab")]
    class InjectEnemyDebug
    {
        static void Prefix(EnemyDataBlock data)
        {
            LoggerWrapper.Log("ENEMY_INF - " + data.name + " ID " + data.persistentID, LogLevel.Info);
        }
    }

    [HarmonyPatch(typeof(SurvivalWave), nameof(SurvivalWave.Spawn), new[] {
         typeof(float),
        typeof(SurvivalWaveSpawnType),
        typeof(AIG_CourseNode),
        typeof(Vector3),
        typeof(ushort),
        typeof(ushort),
        typeof(ushort),
        typeof(bool),
        typeof(bool),
        typeof(Vector3) })]
    class InjectRandomSurvivalWaves
    {
        static void Prefix(ref ushort populationDataID, ref ushort settingsID)
        {
            populationDataID = EnemyPopulationTypeTweaker.GetEnemyWavePopulationTweak(populationDataID);

        }
    }

    [HarmonyPatch(typeof(LG_PopulateArea), "Build")]
    class InjectSettingsType
    {
        static void Prefix(LG_PopulateArea __instance)
        {
            EnemyPopulationTypeTweaker.HandleEnemyPopChanges(__instance);
        }
    }
}