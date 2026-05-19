// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.IO;
// using Avalonia.Input;
// using Newtonsoft.Json;

// namespace IRis.Models;

// [Serializable]
// public class KeyGestureConfig
// {
//     public Dictionary<string, KeyGesture> Entries { get; set; } = new();

//     private static readonly string ConfigPath = Path.Combine(
//         Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
//         "IRis",
//         "keygestures.json");

//     public KeyGestureConfig()
//     {
//         Entries["SaveFile"] = new KeyGesture(Key.S, KeyModifiers.Control);
//         Entries["OpenFile"] = new KeyGesture(Key.O, KeyModifiers.Control);
//         Entries["NewFile"] = new KeyGesture(Key.N, KeyModifiers.Control);

//         Entries["Copy"] = new KeyGesture(Key.C, KeyModifiers.Control);
//         Entries["Cut"] = new KeyGesture(Key.X, KeyModifiers.Control);
//         Entries["Paste"] = new KeyGesture(Key.V, KeyModifiers.Control);

//         Entries["Undo"] = new KeyGesture(Key.Z, KeyModifiers.Control);
//         Entries["Redo"] = new KeyGesture(Key.Y, KeyModifiers.Control);
        

//         Entries["Delete"] = new KeyGesture(Key.Delete);
//         Entries["Unselect"] = new KeyGesture(Key.Escape);
        
//         Entries["RotateClockwise"] = new KeyGesture(Key.A);
//         Entries["RotateCounterClockwise"] = new KeyGesture(Key.D);

//         Entries["OtherComponents"] = new KeyGesture(Key.LeftAlt);
//     }

//     // Static methods to save and load keybinds
//     public static void SaveKeyGestureConfig(KeyGestureConfig config)
//     {
//         try
//         {
//             var directory = Path.GetDirectoryName(ConfigPath);
//             if (!Directory.Exists(directory))
//             {
//                 Directory.CreateDirectory(directory);
//             }
//             // SERIALIZATION
//             string json = JsonConvert.SerializeObject (config, Formatting.Indented);
//             json = json.Replace("KeyModifiers", "Modifiers"); // I hate this but it has to be this way
//             File.WriteAllText(ConfigPath, json);
            
//             Console.WriteLine("Saved KeyConfig to: " + ConfigPath);
//         }
//         catch (Exception ex)
//         {
//             // Log error
//             Console.WriteLine($"Failed to save keybinds: {ex.Message}");
//         }
//     }

//     public static KeyGestureConfig LoadKeyGestureConfig()
//     {
//         try
//         {
//             if (File.Exists(ConfigPath))
//             {
//                 // DESERIALIZATION
//                 string json = File.ReadAllText(ConfigPath);
//                 return JsonConvert.DeserializeObject<KeyGestureConfig>(json);
//             }
//         }
//         catch (Exception ex)
//         {
//             // Log error
//             Console.WriteLine($"Couldn't find keygestures.json: {ex.Message}");
//         }

//         // Use defaults if loading fails
//         return new KeyGestureConfig();
//     }
// }