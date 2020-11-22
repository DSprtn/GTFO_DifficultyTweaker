using GTFO_DIfficulty_Tweaker.Console;
using GTFO_DIfficulty_Tweaker.Core;
using MelonLoader;


namespace GTFO_Difficulty_Tweaker
{
    public class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            CommandParser.Init();

            MelonModLogger.Log("Difficulty tweaker loaded...");


            // Structs require manual hooking, once it's fixed in ML these can be replaced by simple patches
            HackyHooks.HookIntoSpawnData();
            HackyHooks.HookIntoChatMessages();
        }

    }
}
