using System;
using System.IO;
using GSUISerializer;

namespace GS
{
    public static partial class GS
    {
        private static bool CheckJsonFileExists(string path)
        {
            Log($"Checking if json file {path} exists...");
            if (File.Exists(path))
            {
                Log("File exists.");
                return true;
            }

            Log("Json file does not exist at " + path);
            return false;
        }


        public static bool WriteToDisk(Settings Settings)
        {
            Warn("WriteToDisk");
            var serializer = new fsSerializer();
            var fsResult = serializer.TrySerialize(Settings, out var data);
            if (fsResult.Failed)
            {
                Error(fsResult.FormattedMessages);
                return false;
            }

            var json = fsJsonPrinter.PrettyJson(data);
            if (!Directory.Exists(DataDir)) Directory.CreateDirectory(DataDir);
            if (!Directory.Exists(DataDir)) return false;
            try
            {
                File.WriteAllText(Path.Combine(DataDir, "Preferences.json"), json);
            }
            catch (Exception e)
            {
                Error(e.Message);
                return false;
            }

            return true;
        }

        public static Settings ReadFromDisk()
        {
            Log("ReadFromDisk");
            var path = Path.Combine(DataDir, "Preferences.json");
            if (!CheckJsonFileExists(path))
            {
                Warn("Cannot find Preferences.json. Creating");

                var newPreferences = new Settings();
                WriteToDisk(newPreferences);
                return newPreferences;
            }

            Log("Loading Preferences from " + path);
            var serializer = new fsSerializer();
            var json = File.ReadAllText(path);
            var preferences = new Settings();
            var parsedJson = fsJsonParser.Parse(json);
            var fsResult = serializer.TryDeserialize(parsedJson, ref preferences);
            if (fsResult.Failed)
            {
                Error("Failed to Deserialize Settings Json");
                Warn(fsResult.FormattedMessages);
                return new Settings();
            }

            return preferences;
        }
    }
}