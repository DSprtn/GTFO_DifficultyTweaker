using BepInEx.Logging;
using GameData;
using GTFO_Difficulty_Tweaker.Data;
using GTFO_DIfficulty_Tweaker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_DIfficulty_Tweaker.Data
{
    public static class EnemyPopData
    {

        static List<pAvailableEnemyTypes> enemyTypes;

        static List<eEnemyRole> validRoles = new List<eEnemyRole> { eEnemyRole.Lurker, eEnemyRole.Melee, eEnemyRole.Ranged, eEnemyRole.Scout, eEnemyRole.MiniBoss, eEnemyRole.PureSneak };

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
                    if(!validRoles.Contains(data1.Role))
                    {
                        continue;
                    }
                    pAvailableEnemyTypes enemyData = new pAvailableEnemyTypes(
                        data1.Enemy, data1.Cost, data1.Difficulty, data1.Role, data1.Weight);
                    enemyTypes.Add(enemyData);

                    LoggerWrapper.Log(enemyData.ToString());
                }
            }
        }

        public static List<pAvailableEnemyTypes> GetAvailableSpawnData()
        {
            if(enemyTypes == null)
            {
                SetupData(GameDataBlockBase<EnemyPopulationDataBlock>.GetBlock(RundownManager.ActiveExpedition.Expedition.EnemyPopulation));
            }
            return enemyTypes;
        }
    }
}
