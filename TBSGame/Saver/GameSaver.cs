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
    public class GameSaver
    {
        public int MapLevel { get; set; }
        public Dictionary<string, MapInfo> Info { get; set; }
        public List<Unit> Units { get; set; } = null;
        public string Name { get; set; } = "default";
        public int Sources { get; set; } = 0;
        public string Map = null;

        public void Save(string path)
        {
            try
            {
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

        public static GameSaver Load(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        GameSaver game = (GameSaver)formatter.Deserialize(stream);
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
