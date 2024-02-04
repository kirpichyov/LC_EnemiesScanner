using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EnemiesScannerMod.Models;
using Newtonsoft.Json;

namespace EnemiesScannerMod
{
    internal static class AliasesConfig
    {
        private const string FileName = "aliases.json";
        private static string _filePath; 
        
        public static AliasesConfigObject ConfigObject { get; private set; }
        
        public static void Init()
        {
            var locationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (locationPath != null)
            {
                _filePath = Path.Combine(locationPath, FileName);
            }

            if (!File.Exists(_filePath))
            {
                ModLogger.Instance.LogInfo($"Generating the default aliases config in {_filePath}");
                
                try
                {
                    var jsonContent = JsonConvert.SerializeObject(GetDefaultContentObject(), Formatting.Indented);
                    File.AppendAllText(_filePath, jsonContent);
                }
                catch (Exception exception)
                {
                    ModLogger.Instance.LogError("Failed to initialize alias config file");
                    ConfigObject = GetFallbackContentObject();
                    throw;
                }
            }
            else
            {
                ModLogger.Instance.LogInfo("Default aliases config already exists");
            }

            try
            {
                var content = File.ReadAllText(_filePath);
                ConfigObject = JsonConvert.DeserializeObject<AliasesConfigObject>(content);
            }
            catch (Exception exception)
            {
                ModLogger.Instance.LogError("Failed to read alias config file");
                ConfigObject = GetFallbackContentObject();
                throw;
            }
            
            // Normalize dictionary
            foreach (var entry in ConfigObject.Config.ToArray())
            {
                ConfigObject.Config.Remove(entry.Key);
                ConfigObject.Config.Add(entry.Key.ToLowerInvariant(), entry.Value);
            }
        }

        public static string GetAliasOrDefault(string originalName)
        {
            var exists = ConfigObject.Config.TryGetValue(originalName.ToLowerInvariant(), out var alias);
            return exists ? alias : null;
        }

        private static AliasesConfigObject GetFallbackContentObject()
        {
            return new AliasesConfigObject() { Config = new Dictionary<string, string>(0) };
        }

        private static AliasesConfigObject GetDefaultContentObject()
        {
            return new AliasesConfigObject
            {
                Config = new Dictionary<string, string>()
                {
                    {"BaboonBird", "Baboon Hawk"},
                    {"DocileLocustBees", "Roaming Locust"},
                    {"Doublewing", "Manticoil"},
                    {"Jester", "Jester"},
                    {"MaskedPlayer", "Masked"},
                    {"Nutcracker", "Nutcracker"},
                    {"Puffer", "Spore Lizard"},
                    {"SandSpider", "Bunker Spider"},
                    {"SandWorm", "Earth Leviathan"},
                    {"Blob", "Hygrodere"},
                    {"Centipede", "Snare Flea"},
                    {"Crawler", "Thumper"},
                    {"RedLocustBees", "Circuit Bee"},
                    {"DressGirl", "Ghost Girl"},
                    {"Flowerman", "Bracken"},
                    {"ForestGiant", "Forest Keeper"},
                    {"HoarderBug", "Hoarding Bug"},
                    {"MouthDog", "Eyeless Dog"},
                    {"SpringMan", "Coil-Head"},
                },
            };
        }
    }
}