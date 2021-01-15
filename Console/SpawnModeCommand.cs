using BepInEx.Logging;
using GameData;
using GTFO_DIfficulty_Tweaker.Config;
using GTFO_DIfficulty_Tweaker.Data;
using GTFO_DIfficulty_Tweaker.Util;
using System;
using System.Collections.Generic;

namespace GTFO_Difficulty_Tweaker.Console
{
    public class SpawnModeCommand : Command
    {

        public SpawnModeCommand(string commandName, string commandDesc) : base(commandName, commandDesc)
        {
            
        }

        public override void Execute(string payLoad)
        {
            SpawnMode mode = null;

            foreach (KeyValuePair<string, SpawnMode> item in SpawnModeConfiguration.spawnModes)
            {
                if(String.Equals(payLoad, item.Key, StringComparison.OrdinalIgnoreCase))
                {
                    mode = item.Value;
                }
                continue;
            }

            if (SpawnModeConfiguration.currentMode != mode)
            {

                if (mode != null)
                {
                    LoggerWrapper.Log($"Switched to \"{ mode.spawnModeName }\" - \"{mode.description}\" ", LogLevel.Message);
                } else
                {
                    LoggerWrapper.Log("Invalid mode", LogLevel.Message);
                }
                //ToDo fix switching back to normal spawns
                //EnemyPopulationDataBlock.Setup();
                //EnemyPopulationDataBlock.PostSetup();

                SpawnModeConfiguration.currentMode = mode;
            }
        }

        public static string GetAllAvailableModes()
        {
            string result = "";
            int curr = 0;
            foreach (SpawnMode m in SpawnModeConfiguration.spawnModes.Values)
            {
                curr++;
                result += $"{m.spawnModeName};";
                if(curr == 3)
                {
                    result += "\n";
                    curr = 0;
                }
            }
            return result;
        }

        public override string GetDescriptionString()
        {
            return base.GetDescriptionString() + $" Available modes:\n{SpawnModeCommand.GetAllAvailableModes()}";
        }
    }
}
