using GameData;
using GTFO_Difficulty_Tweaker.Data;
using GTFO_DIfficulty_Tweaker.Util;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_DIfficulty_Tweaker.Data
{
    public static class EnemyGroupData
    {

        public static List<pAvailableEnemyTypes> enemyTypes;

        public static void SetupData(EnemyPopulationDataBlock __instance)
        {
            if (enemyTypes == null)
            {
                enemyTypes = new List<pAvailableEnemyTypes>();
            }

            foreach (Il2CppSystem.Collections.Generic.List<EnemyRoleData> data in __instance.m_enemyDataPerRoleAndDiff.Values)
            {
                foreach (EnemyRoleData data1 in data)
                {
                    pAvailableEnemyTypes enemyData = new pAvailableEnemyTypes(
                        data1.Enemy, data1.Cost, data1.Difficulty, data1.Role, data1.Weight);
                    enemyTypes.Add(enemyData);

                    LoggerWrapper.Log($"Enemy: {data1.Enemy} Cost: {data1.Cost} Difficulty: {data1.Difficulty} Role: {data1.Role}", LogLevel.Debug);
                }
            }
        }
    }
}
