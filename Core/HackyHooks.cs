using Agents;
using Enemies;
using GTFO_Difficulty_Tweaker;
using GTFO_DIfficulty_Tweaker.Console;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnhollowerBaseLib;
using UnityEngine;

namespace GTFO_DIfficulty_Tweaker.Core
{
    public static class HackyHooks
    {
        public static void HookIntoChatMessages()
        {
            unsafe
            {
                var originalMethodPointer = *(IntPtr*)(IntPtr)UnhollowerUtils
                    .GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(PlayerChatManager).GetMethod(nameof(PlayerChatManager.DoSendChatMessage)))
                    .GetValue(null);

                Imports.Hook((IntPtr)(&originalMethodPointer), typeof(HackyHooks).GetMethod(nameof(HackyHooks.ChatMsgPatch), BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());

                ourChatDelegate = Marshal.GetDelegateForFunctionPointer<ChatDelegate>(originalMethodPointer);
            }
        }

        public static void HookIntoSpawnData()
        {
            unsafe
            {
                var originalMethodPointer = *(IntPtr*)(IntPtr)UnhollowerUtils
                    .GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(EnemyGroup).GetMethod(nameof(EnemyGroup.GetSpawnData)))
                    .GetValue(null);

                Imports.Hook((IntPtr)(&originalMethodPointer), typeof(HackyHooks).GetMethod(nameof(HackyHooks.GetEnemySpawnDataPatch), BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());

                ourGetEnemySpawnData = Marshal.GetDelegateForFunctionPointer<EnemySpawnDataDelegate>(originalMethodPointer);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate IntPtr EnemySpawnDataDelegate(IntPtr thisPtr, Vector3* position, IntPtr courseNode,
            EnemyGroupType groupType, eEnemyGroupSpawnType spawnType, uint persistentGameDataID,
            float populationScore, IntPtr targetReplicator, IntPtr survivalWave,
            uint debugEnemyID, AgentMode debugEnemyMode);

        private unsafe static IntPtr GetEnemySpawnDataPatch(IntPtr thisPtr, Vector3* position, IntPtr courseNode,
    EnemyGroupType groupType, eEnemyGroupSpawnType spawnType, uint persistentGameDataID,
    float populationScore, IntPtr targetReplicator, IntPtr survivalWave,
    uint debugEnemyID, AgentMode debugEnemyMode)
        {
            return ourGetEnemySpawnData(thisPtr, position, courseNode, groupType, spawnType, persistentGameDataID, populationScore * SpawnTweakSettings.spawnPopMult, targetReplicator, survivalWave, debugEnemyID, debugEnemyMode);
        }

        private static EnemySpawnDataDelegate ourGetEnemySpawnData;


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private unsafe delegate void ChatDelegate(IntPtr thisPtr, MarshChatMsg msg);

        private unsafe static void ChatMsgPatch(IntPtr thisPtr, MarshChatMsg msg)
        {
            string chatStr = IL2CPP.Il2CppStringToManaged(msg.msg.m_data);
            CommandParser.HandleMessage(chatStr);
            ourChatDelegate(thisPtr, msg);
        }

        private static ChatDelegate ourChatDelegate;

        public struct MarshChatMsg
        {
            public MarshPlayer toPlayer;
            public MarshPlayer fromPlayer;
            public MarshMsg msg;

            public MarshChatMsg(MarshPlayer toPlayer, MarshPlayer fromPlayer, MarshMsg msg)
            {
                this.toPlayer = toPlayer;
                this.fromPlayer = fromPlayer;
                this.msg = msg;
            }
        }

        public struct MarshPlayer
        {
            ulong lookup;

            public MarshPlayer(ulong lkup)
            {
                this.lookup = lkup;
            }
        }

        public struct MarshMsg
        {
            public IntPtr m_data;

            public MarshMsg(IntPtr data)
            {
                m_data = data;
            }

        }
    }
}

