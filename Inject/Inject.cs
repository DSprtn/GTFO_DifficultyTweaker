using AIGraph;
using BepInEx.Logging;
using Enemies;
using GameData;
using GTFO_DIfficulty_Tweaker.Console;
using GTFO_DIfficulty_Tweaker.Core;
using GTFO_DIfficulty_Tweaker.Data;
using GTFO_DIfficulty_Tweaker.Util;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using LevelGeneration;
using Player;
using System;
using UnhollowerBaseLib;
using UnityEngine;


namespace GTFO_Difficulty_Tweaker
{
    
    [HarmonyPatch(typeof(EnemyPopulationDataBlock), nameof(EnemyPopulationDataBlock.SetupEnemyDataLookup))]
    class InjectCustomPopData
    {
        static void Prefix(EnemyPopulationDataBlock __instance)
        {
            EnemyDataBlockGroupInjector.InjectCustomEnemies(__instance);
        }
    }

    [HarmonyPatch(typeof(EGS_PatrolMove), nameof(EGS_PatrolMove.Update))]
    class InjectRemoveAnnoyingStuckDebug
    {
        static void Prefix(EGS_PatrolMove __instance)
        {
            __instance.stuckTimer = Clock.Time;
        }
    }

    [HarmonyPatch(typeof(PUI_GameEventLog), nameof(PUI_GameEventLog.Setup))]
    class InjectChatCommands
    {
        static void Prefix(PUI_GameEventLog __instance)
        {
            LoggerWrapper.AddGameLogReference(__instance);
        }
    }

    [HarmonyPatch(typeof(LG_PopulateArea), nameof(LG_PopulateArea.Build))]
    class InjectPopulationTweak
    {
        static void Prefix(LG_PopulateArea __instance)
        {
            EnemyPopulationTypeTweaker.HandleEnemyPopChanges(__instance);
        }
    }

    [HarmonyPatch(typeof(EnemyPrefabManager), nameof(EnemyPrefabManager.BuildEnemyPrefab))]
    class InjectEnemyDebug
    {
        static void Prefix(EnemyDataBlock data)
        {
            LoggerWrapper.Log("ENEMY_INF - " + data.name + " ID " + data.persistentID, LogLevel.Debug);
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
            EnemyPopulationTypeTweaker.SetEnemyWavePopulationTweak(ref populationDataID, ref settingsID);
        }
    }
    
}