using GameData;
using GTFO_Difficulty_Tweaker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_Difficulty_Tweaker
{
    public static class SpawnTweakSettings
    {
        public static SpawnTweakerMode mode = SpawnTweakerMode.NORMAL;
        public static float spawnPopMult = 1f;
        public static float resourceMult = 1f;
        public static eEnemyRole enemyRole = eEnemyRole.Scout;
        public static float scoutChance = 0.1f;


        public static void SetScoutChance(float newChance)
        {
            scoutChance = newChance;
        }

        public static void SetResourceMult(float resMult)
        {
            resourceMult = resMult;
        }

        public static void SetPopulationMult(float mult)
        {
            spawnPopMult = mult;
        }
    }
}
