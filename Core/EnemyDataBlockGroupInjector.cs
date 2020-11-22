using GameData;
using GTFO_DIfficulty_Tweaker.Util;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GTFO_DIfficulty_Tweaker.Core
{
    public static class EnemyDataBlockGroupInjector
    {
        static bool injected = false;
        public static void InjectCustomEnemies(EnemyPopulationDataBlock __instance)
        {
            if (!injected)
            {
                Il2CppSystem.Collections.Generic.List<EnemyRoleData> customRoles = new Il2CppSystem.Collections.Generic.List<EnemyRoleData>();

                customRoles.Add(CreateCustomData(eEnemyRoleDifficulty.MiniBoss, eEnemyRole.Lurker, 6, 35, 2));
                customRoles.Add(CreateCustomData(eEnemyRoleDifficulty.MiniBoss, eEnemyRole.Melee, 10, 39, 1));
                foreach (EnemyRoleData data in __instance.RoleDatas)
                {
                    customRoles.Add(data);
                    
                }
                __instance.RoleDatas = customRoles;
                injected = true;
                LoggerWrapper.Log("Adding invisible giants...", LogLevel.UserInfo, ConsoleColor.Red);
            }
        }

        static EnemyRoleData CreateCustomData(eEnemyRoleDifficulty difficulty, eEnemyRole role, float cost, uint enemyId, float weightMult = 1)
        {
            EnemyRoleData data = new EnemyRoleData();
            data.Difficulty = difficulty;
            data.Enemy = enemyId;
            data.Cost = cost;
            data.Role = role;
            data.Weight = Mathf.Min(2f, Mathf.Max(0.2f, UnityEngine.Random.value * weightMult));
            return data;
        }
    }
}
