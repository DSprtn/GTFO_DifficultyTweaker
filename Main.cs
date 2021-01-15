using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using GTFO_DIfficulty_Tweaker;
using GTFO_DIfficulty_Tweaker.Config;
using GTFO_DIfficulty_Tweaker.Console;
using GTFO_DIfficulty_Tweaker.Core;
using GTFO_DIfficulty_Tweaker.Util;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace GTFO_Difficulty_Tweaker
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main : BasePlugin
    {
        public const string
            MODNAME = "GTFO-Difficulty_Tweaker",
            AUTHOR = "Spartan",
            GUID = "com." + AUTHOR + "." + MODNAME,
            VERSION = "0.1.0";

        public override void Load()
        {
            var harmony = new Harmony(GUID);
            harmony.PatchAll();
            LoggerWrapper.logSource = Logger.CreateLogSource(MODNAME);
            SpawnModeConfiguration.Init();
            CommandParser.Init();


            // Structs require manual hooking, once it's fixed in unhollower these can be replaced by simple patches
            HackyHooks.HookIntoSpawnData();
            HackyHooks.HookIntoChatMessages();
        }
    }
}
