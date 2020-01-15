using System;
using System.IO;
using System.Text.Json;

namespace BirthdayBot
{
    public static class JsonHelper
    {
        public static T DeserializeFile<T>(string file, T ifFileNotFound)
        {
            try
            {
                var json = File.ReadAllText(file);
                var obj = JsonSerializer.Deserialize<T>(json, GetSerializerOptions());
                return obj;
            }
            catch (JsonException e)
            {
                Console.WriteLine($"Could not parse {file}: {e}");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Could not find {file}, creating default {file} file.");
                var json = JsonSerializer.Serialize(ifFileNotFound, GetSerializerOptions());
                File.WriteAllText(file, json);
            }
            return ifFileNotFound;
        }

        public static void SerializeToFile<T>(string file, T value)
        {
            var json = JsonSerializer.Serialize(value, GetSerializerOptions());
            File.WriteAllText(file, json);
        }

        private static JsonSerializerOptions GetSerializerOptions()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            options.Converters.Add(new DateTimeConverter());
            return options;
        }
    }
}