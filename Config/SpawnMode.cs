using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_DIfficulty_Tweaker.Config
{
    public class SpawnMode
    {
        [JsonProperty("ModeName")]
        public string spawnModeName = "DEFAULT";
        [JsonProperty("Description")]
        public string description = "NONE";
        [JsonProperty("AllowedEnemyIDs")]
        public Dictionary<int, float> possibleEnemyTypes;
        [JsonProperty("AllowedScoutWaveSpawns")]
        public Dictionary<int, float> possibleScoutSpawns;
        [JsonProperty("AllowedOtherSurvivalWaveSpawns")]
        public Dictionary<int, float> possibleWaveSpawns;
        [JsonProperty("PopulationMultiplier")]
        public float baseEnemyPopulationMultiplier;
        [JsonProperty("ScoutWavePopulationToSettingsBinding")]
        public Dictionary<int, int> scoutWavePopulationToWaveSettingsMapping;
        [JsonProperty("AnyWaveButScoutPopulationToSettingsBinding")]
        public Dictionary<int, int> anyWavePopulationToWaveSettingsMapping;
    }

}
