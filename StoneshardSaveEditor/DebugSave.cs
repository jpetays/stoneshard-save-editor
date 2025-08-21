using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StoneshardSaveEditor
{
    public static class DebugSave
    {
        private static readonly Encoding Encoding = new UTF8Encoding(false);

        private const string SavePath =
            @"Q:\Work\github\jpetays\Stoneshard\jpetays\stoneshard-save-editor\StoneshardSaveEditor\jsonData";

        public static void SaveJson(string currentPath, JObject jObject, bool isLoading, bool both = false)
        {
            // C:\Users\petays\AppData\Local\StoneShard\characters_v1
            // Characters are numbered from 1, save files are numbered from 1
            // C:\Users\petays\AppData\Local\StoneShard\characters_v1\character_NNN\save_NNN\data.sav

            var tokens = currentPath.Split('\\');
            var prefix = $"{tokens[tokens.Length - 3]}.{tokens[tokens.Length - 2]}"
                .Replace("character", "char")
                .Replace("exitsave", "exit")
                .Replace("autosave", "auto");
            var verb = isLoading ? "load" : "save";

            var path = Path.Combine(SavePath, $"{prefix}_{verb}_inventory.json");
            File.WriteAllText(path, jObject["inventoryDataList"].ToString(Formatting.Indented));
            if (!both)
            {
                return;
            }
            path = Path.Combine(SavePath, $"{prefix}_{verb}_savegame.json");
            File.WriteAllText(path, jObject.ToString(Formatting.Indented));
        }

        public static bool MoneyBagTest()
        {
#if false
    [
      "o_inv_moneybag",
      {
        "Material": "leather",
        "max_charge": 1.0,
        "idName": "moneybag",
        "Duration": 0.0,
        "is_cursed": 0.0,
        "MaxDuration": 0.0,
        "i_index": 0.0,
        "Stack": 250.0,
        "Main": [],
        "identified": 1.0,
        "charge": 1.0,
        "Effects_Duration": 0.0,
        "lootList": [...],
        "is_execute": 1.0
      },
      0.0,
      0.0,
      1.0,
      1.0,
      0.0,
      false,
      0.0,
      "N/A"
    ],
    // --- max 20 items per bag --
    [
      "o_inv_gold",
      {
        "Material": "gold",
        "max_charge": 1.0,
        "idName": "gold",
        "Duration": 0.0,
        "is_cursed": 0.0,
        "MaxDuration": 0.0,
        "i_index": 0.0,
        "Main": [],
        "identified": 1.0,
        "charge": 1.0,
        "Effects_Duration": 0.0,
        "is_execute": 1.0
      },
      a,
      b,
      c,
      d,
      10.0,
      f,
      g,
      "N/A"
    ],

#endif
            const string testBag =
                @"Q:\Work\github\jpetays\Stoneshard\jpetays\stoneshard-save-editor\StoneshardSaveEditor\jsonData\o_inv_moneybag.json";

            var jsonString = File.ReadAllText(testBag, Encoding);
            var jArray = JsonConvert.DeserializeObject(jsonString) as JArray;
            Debug.Assert(jArray is { Count: 10 });

            Debug.Assert("o_inv_moneybag".Equals($"{jArray[0]}"));
            if (!(jArray[1] is JObject moneybag))
            {
                return false;
            }
            Debug.Assert(moneybag["Stack"] != null);
            Debug.Assert(moneybag["lootList"] != null);

            var stackValue = (int)moneybag["Stack"].Value<double>();
            var goldValue = 0;
            foreach (var goldToken in moneybag["lootList"])
            {
                if (!(goldToken is JArray goldArray))
                {
                    continue;
                }
                Debug.Assert(goldArray is { Count: 10 });
                Debug.Assert("o_inv_gold".Equals($"{goldArray[0]}"));
                var goldAmount = (int)goldArray[6].Value<double>();
                goldValue += goldAmount;
            }
            if (stackValue == goldValue && goldValue == 2000)
            {
                // Full bag
                return false;
            }
            stackValue = 0;
            foreach (var goldToken in moneybag["lootList"])
            {
                if (goldToken is JArray goldArray && goldArray[6].Value<double>() < 100.0)
                {
                    goldArray[6] = 100.0;
                    stackValue += (int)goldArray[6].Value<double>();
                }
            }
            moneybag["Stack"] = (double)stackValue;
            var jsonFormatted = jArray.ToString(Formatting.Indented);
            File.WriteAllText(testBag, jsonFormatted, Encoding);
            return false;
        }
    }
}
