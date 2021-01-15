using GameData;
using GTFO_Difficulty_Tweaker.Data;
using GTFO_DIfficulty_Tweaker.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_Difficulty_Tweaker
{
    public static class SpawnTweakSettings
    {
        static float spawnMult = 1;
        public static float spawnPopMult
        {
            get
            {
                return spawnMult * (SpawnModeConfiguration.currentMode == null ? 1 : SpawnModeConfiguration.currentMode.baseEnemyPopulationMultiplier);
            }
            set 
            {
                spawnMult = value;
            }
        }

        public static void SetPopulationMult(float mult)
        {
            spawnPopMult = mult;
        }
    }
}
