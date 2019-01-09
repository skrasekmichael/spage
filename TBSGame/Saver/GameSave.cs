using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using MapDriver;

namespace TBSGame.Saver
{
    [Serializable]
    public class GameSave
    {
        public int MapLevel { get; set; } = 0;
        public Dictionary<string, MapInfo> Info { get; set; } = null;
        public List<Unit> Units { get; set; } = null;
        public string Name { get; set; } = "default";
        public int Sources { get; set; } = 0;
        public int Round { get; set; } = 0;
        public DateTime SavedAt { get; private set; }
        public Map Map { get; set; } = null;
        public string ScenarioName { get; private set; }
        public string Level { get; private set; }
        public string Scenario { get; private set; }

        public GameSave(Scenario scenario, Settings settings)
        {
            this.ScenarioName = scenario.Name;
            this.Units = scenario.StarterPack;
            this.Scenario = settings.Scenario;
            NextLevel();
        }

        private string getpath()
        {
            string number = "level" + MapLevel.ToString();
            string level = $@"{Scenario}{ScenarioName}\{number}\{number}.dat";
            return level;
        }

        public void NextLevel()
        {
            MapLevel++;
            Level = getpath();
            if (File.Exists(Level))
                Info = null;
            else
            {
                Error.Log("Loading file failed: " + Level);
                MapLevel--;
            }
        }

        public void Save(string path)
        {
            try
            {
                SavedAt = DateTime.Now;

                if (File.Exists(path))
                    File.Delete(path);

                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, this);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                Error.Log($"Game saving failed! Detail message: {ex.Message}");
            }
        }

        public static GameSave Load(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        GameSave game = (GameSave)formatter.Deserialize(stream);
                        stream.Close();
                        return game;
                    }
                }
                catch (Exception ex)
                {
                    Error.Log("Loading file failed. Detail Message: " + ex.Message);
                    return null;
                }
            }
            else
            {
                Error.Log("File not exist.");
                return null;
            }
        }
    }
}
