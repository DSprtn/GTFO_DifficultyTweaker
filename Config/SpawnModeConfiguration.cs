using BepInEx;
using BepInEx.Configuration;
using GTFO_DIfficulty_Tweaker.Data;
using GTFO_DIfficulty_Tweaker.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_DIfficulty_Tweaker.Config
{
    public static class SpawnModeConfiguration
    {
        public static Dictionary<string, SpawnMode> spawnModes;
        public static SpawnMode currentMode;

        static string path_to_modes = Path.Combine(Paths.ConfigPath, "DifficultyTweaker");
        internal static void Load()
        {
            LoggerWrapper.Log("Loading modes...", BepInEx.Logging.LogLevel.Debug);
            foreach (string file in Directory.GetFiles(path_to_modes))
            {
                if (file.EndsWith(".difficultyTweakerMode"))
                {
                    SpawnMode mode = DeserializeMode(file);
                    if (mode != null)
                    {
                        if(spawnModes.ContainsKey(mode.spawnModeName))
                        {
                            LoggerWrapper.Log($"Found duplicate spawn mode { mode.spawnModeName } - Overwriting the current one! ", BepInEx.Logging.LogLevel.Warning);
                        }
                        spawnModes[mode.spawnModeName] = mode;
                    }
                }
            }
        }

        internal static void Init()
        {
            spawnModes = new Dictionary<string, SpawnMode>();
            if(!Directory.Exists(path_to_modes))
            {
                Directory.CreateDirectory(path_to_modes);
            }
            CreateDefaultModes();
            CreateHelpFile();
            Load();
        }

        private static void CreateHelpFile()
        {
            string help_file = "General help file to help you create modes. \nAll values have an id to them as first value and a weight as the second. The weight is the chance that the given SpawnGroup will be used.\n" +
                "\nAvailable enemies:\n";
            foreach(EnemyID id in Enum.GetValues(typeof(EnemyID))) {
                help_file += $"    Enemy: {Enum.GetName(typeof(EnemyID), id) } - ID: { (int)id} \n";
            }
            help_file += "\n\nAvailable survival wave populations:\n\n";
            foreach(SurvivalSpawnWavePopulation wave in Enum.GetValues(typeof(SurvivalSpawnWavePopulation)))
            {
                help_file += $"    Wave: {Enum.GetName(typeof(SurvivalSpawnWavePopulation), wave)} - ID: {(int)wave} \n";
            }


            File.WriteAllText(Path.Combine(path_to_modes, "help.txt"), help_file, Encoding.UTF8);
        }

        

        static void SerializeMode(SpawnMode mode)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(Path.Combine(path_to_modes, mode.spawnModeName + ".difficultyTweakerMode")))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, mode);
            }

        }

        static SpawnMode DeserializeMode(string filePath)
        {
            SpawnMode m = null;
            File.ReadAllText(filePath);
            try
            {
                m = JsonConvert.DeserializeObject<SpawnMode>(File.ReadAllText(filePath));
            } catch (JsonException)
            {
                LoggerWrapper.Log($"Failed to load  {filePath} , is your JSON correct?", BepInEx.Logging.LogLevel.Warning);
            }
            
            return m;
        }



        private static void CreateDefaultModes()
        {
            CreateScoutMadness();
            CreateRandom();
            CreateBalancedRandom();
            CreateShadows();
            CreateChargers();
            CreateShooters();
        }

        private static void CreateShooters()
        {
            SpawnMode shooters = new SpawnMode();
            shooters.spawnModeName = "Shooters";
            shooters.description = "Made only to annoy Dragon";
            shooters.possibleEnemyTypes = new Dictionary<int, float>()
            {
              { (int)EnemyID.Shooter_Hibernate, 10f},
              { (int)EnemyID.Shooter_Big, 5f},
              { (int)EnemyID.Shooter_Big_RapidFire, 1f}
            };
            shooters.possibleScoutSpawns = new Dictionary<int, float>()
            {
              { (int)SurvivalSpawnWavePopulation.Shooters_Only, 8f},
              { (int)SurvivalSpawnWavePopulation.Baseline_Rapid_Shooter, 1f}
            };
            shooters.possibleWaveSpawns = shooters.possibleScoutSpawns;
            shooters.baseEnemyPopulationMultiplier = 2f;
            SerializeMode(shooters);
        }

        private static void CreateChargers()
        {
            SpawnMode chargers = new SpawnMode();
            chargers.baseEnemyPopulationMultiplier = 1f;
            chargers.spawnModeName = "Chargers";
            chargers.description = "Hope you like spikes";
            chargers.possibleEnemyTypes = new Dictionary<int, float>()
            {
              { (int)EnemyID.Striker_Big_Bullrush, 8f},
              { (int)EnemyID.Striker_Bullrush, 20f},
              { (int)EnemyID.Tank, 1f}
            };
            chargers.possibleScoutSpawns = new Dictionary<int, float>()
            {
              { (int)SurvivalSpawnWavePopulation.Bullrush_Only, 8f},
              { (int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Boss, 1f},
                { (int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Mix, 3f}
            };
            chargers.possibleWaveSpawns = chargers.possibleScoutSpawns;
            SerializeMode(chargers);
        }

        private static void CreateShadows()
        {
            SpawnMode shadows = new SpawnMode();
            shadows.baseEnemyPopulationMultiplier = 1f;
            shadows.spawnModeName = "Shadows";
            shadows.description = "Nothing to see here";
            shadows.possibleEnemyTypes = new Dictionary<int, float>()
            {
              { (int)EnemyID.Scout_Shadow, 3f},
              { (int)EnemyID.Shadow, 5f},
              { (int)EnemyID.Striker_Big_Shadow, 2f}
            };
            shadows.possibleScoutSpawns = new Dictionary<int, float>()
            {
              { (int)SurvivalSpawnWavePopulation.Shadows_Only, 1f}
            };
            shadows.possibleWaveSpawns = new Dictionary<int, float>()
            {
              { (int)SurvivalSpawnWavePopulation.Shadows_Only, 1f}
            };
            SerializeMode(shadows);
        }

        private static void CreateBalancedRandom()
        {
            SpawnMode balancedRandom = new SpawnMode();
            balancedRandom.baseEnemyPopulationMultiplier = 1f;
            balancedRandom.spawnModeName = "Balanced_Random";
            balancedRandom.description = "You have a chance, sometimes";
            balancedRandom.possibleScoutSpawns = new Dictionary<int, float>();
            balancedRandom.possibleScoutSpawns = new Dictionary<int, float>();
            balancedRandom.possibleEnemyTypes = new Dictionary<int, float>();

            foreach (EnemyID enemy in Enum.GetValues(typeof(EnemyID)))
            {
                balancedRandom.possibleEnemyTypes.Add((int)enemy, 50);
            }
            balancedRandom.possibleEnemyTypes[(int)EnemyID.Scout] = 25;
            balancedRandom.possibleEnemyTypes[(int)EnemyID.Scout_Shadow] = 15;
            balancedRandom.possibleEnemyTypes[(int)EnemyID.Striker_Bullrush] = 60;
            balancedRandom.possibleEnemyTypes[(int)EnemyID.Striker_Big_Bullrush] = 50;
            balancedRandom.possibleEnemyTypes[(int)EnemyID.Striker_Hibernate] = 60;
            balancedRandom.possibleEnemyTypes[(int)EnemyID.Shooter_Hibernate] = 60;
            balancedRandom.possibleEnemyTypes[(int)EnemyID.Shooter_Big] = 35;
            balancedRandom.possibleEnemyTypes[(int)EnemyID.Striker_Big_Shadow] = 20;
            balancedRandom.possibleEnemyTypes[(int)EnemyID.Shadow] = 35;
            balancedRandom.possibleEnemyTypes[(int)EnemyID.Striker_Big_Hibernate] = 25;
            balancedRandom.possibleEnemyTypes[(int)EnemyID.Tank] = 1;
            balancedRandom.possibleEnemyTypes[(int)EnemyID.Birther] = 1;
            balancedRandom.possibleEnemyTypes[(int)EnemyID.Birther_Boss] = 1;

            balancedRandom.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Shadows_Only, 3);
            balancedRandom.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Bullrush_Only, 2);
            balancedRandom.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Baseline_Big, 2);
            balancedRandom.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Mix, 3);
            balancedRandom.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Boss, 1);
            balancedRandom.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Baseline, 6);
            balancedRandom.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Shooters_Only, 4);
            balancedRandom.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Baseline_Bullrush, 2);
            balancedRandom.possibleWaveSpawns = balancedRandom.possibleScoutSpawns;

            balancedRandom.scoutWavePopulationToWaveSettingsMapping = new Dictionary<int, int>()
            {
                {(int)SurvivalSpawnWavePopulation.Tank, (int)SurvivalSpawnWaveSettings.Single},
                {(int)SurvivalSpawnWavePopulation.Baseline_Birther, (int)SurvivalSpawnWaveSettings.Single},
                {(int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Boss, (int)SurvivalSpawnWaveSettings.Three},
                {(int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Mix, (int)SurvivalSpawnWaveSettings.Six},
                {(int)SurvivalSpawnWavePopulation.Baseline_Big, (int)SurvivalSpawnWaveSettings.Six},
                {(int)SurvivalSpawnWavePopulation.Baseline, (int)SurvivalSpawnWaveSettings.TwentyFour},
            };

            balancedRandom.anyWavePopulationToWaveSettingsMapping = new Dictionary<int, int>()
            {
                {(int)SurvivalSpawnWavePopulation.Tank, (int)SurvivalSpawnWaveSettings.Single},
                {(int)SurvivalSpawnWavePopulation.Baseline_Birther, (int)SurvivalSpawnWaveSettings.Single},
                {(int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Boss, (int)SurvivalSpawnWaveSettings.Six},
                {(int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Mix, (int)SurvivalSpawnWaveSettings.Eight},
                {(int)SurvivalSpawnWavePopulation.Baseline_Big, (int)SurvivalSpawnWaveSettings.Twelve},
                {(int)SurvivalSpawnWavePopulation.Baseline, (int)SurvivalSpawnWaveSettings.TwentyFour},
            };

            SerializeMode(balancedRandom);
        }

        private static void CreateRandom()
        {
            SpawnMode random = new SpawnMode();
            random.baseEnemyPopulationMultiplier = 1.1f;
            random.spawnModeName = "True_Random";
            random.description = "Not for the faint of heart";
            random.possibleScoutSpawns = new Dictionary<int, float>();
            random.possibleWaveSpawns = new Dictionary<int, float>();
            random.possibleEnemyTypes = new Dictionary<int, float>();
            foreach (SurvivalSpawnWavePopulation wave in Enum.GetValues(typeof(SurvivalSpawnWavePopulation)))
            {
                random.possibleWaveSpawns.Add((int)wave, 1);
                random.possibleScoutSpawns.Add((int)wave, 1);
            }

            foreach (EnemyID enemy in Enum.GetValues(typeof(EnemyID)))
            {
                random.possibleEnemyTypes.Add((int)enemy, 1);
            }

            random.scoutWavePopulationToWaveSettingsMapping = new Dictionary<int, int>()
            {
                {(int)SurvivalSpawnWavePopulation.Tank, (int)SurvivalSpawnWaveSettings.Single},
                {(int)SurvivalSpawnWavePopulation.Baseline_Birther, (int)SurvivalSpawnWaveSettings.Single},
                {(int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Boss, (int)SurvivalSpawnWaveSettings.Three},
                {(int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Mix, (int)SurvivalSpawnWaveSettings.Six},
                {(int)SurvivalSpawnWavePopulation.Baseline_Big, (int)SurvivalSpawnWaveSettings.Six},
                {(int)SurvivalSpawnWavePopulation.Baseline, (int)SurvivalSpawnWaveSettings.TwentyFour},
            };

            random.anyWavePopulationToWaveSettingsMapping = new Dictionary<int, int>()
            {
                {(int)SurvivalSpawnWavePopulation.Tank, (int)SurvivalSpawnWaveSettings.Single},
                {(int)SurvivalSpawnWavePopulation.Baseline_Birther, (int)SurvivalSpawnWaveSettings.Single},
                {(int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Boss, (int)SurvivalSpawnWaveSettings.Six},
                {(int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Mix, (int)SurvivalSpawnWaveSettings.Eight},
                {(int)SurvivalSpawnWavePopulation.Baseline_Big, (int)SurvivalSpawnWaveSettings.Twelve},
                {(int)SurvivalSpawnWavePopulation.Baseline, (int)SurvivalSpawnWaveSettings.TwentyFour},
            };
            SerializeMode(random);
        }

    

        private static void CreateScoutMadness()
        {
            SpawnMode scout_madness = new SpawnMode();
            scout_madness.spawnModeName = "Scout_Madness";
            scout_madness.description = "Don't touch the spaget";
            scout_madness.possibleWaveSpawns = new Dictionary<int, float>();
            scout_madness.possibleScoutSpawns = new Dictionary<int, float>();
            scout_madness.possibleWaveSpawns = new Dictionary<int, float>();
            scout_madness.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Shadows_Only, 5);
            scout_madness.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Bullrush_Only, 5);
            scout_madness.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Tank, 2);
            scout_madness.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Baseline_Birther, 1);
            scout_madness.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Baseline_Big, 5);
            scout_madness.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Mix, 5);
            scout_madness.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Boss, 3);
            scout_madness.possibleScoutSpawns.Add((int)SurvivalSpawnWavePopulation.Baseline, 5);
            scout_madness.baseEnemyPopulationMultiplier = 3f;
            scout_madness.possibleEnemyTypes = new Dictionary<int, float>();
            scout_madness.possibleEnemyTypes.Add((int)EnemyID.Scout, 5);
            scout_madness.possibleEnemyTypes.Add((int)EnemyID.Scout_Shadow, 2);
            scout_madness.scoutWavePopulationToWaveSettingsMapping = new Dictionary<int, int>()
            {
                {(int)SurvivalSpawnWavePopulation.Tank, (int)SurvivalSpawnWaveSettings.Single},
                {(int)SurvivalSpawnWavePopulation.Baseline_Birther, (int)SurvivalSpawnWaveSettings.Single},
                {(int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Boss, (int)SurvivalSpawnWaveSettings.Three},
                {(int)SurvivalSpawnWavePopulation.Baseline_Bullrush_Mix, (int)SurvivalSpawnWaveSettings.Six},
                {(int)SurvivalSpawnWavePopulation.Baseline_Big, (int)SurvivalSpawnWaveSettings.Six},
                {(int)SurvivalSpawnWavePopulation.Baseline, (int)SurvivalSpawnWaveSettings.TwentyFour},
            };

            SerializeMode(scout_madness);
        }
    }
}
