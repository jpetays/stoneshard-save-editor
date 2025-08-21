using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StoneshardSaveEditor
{
    public static class MoneyBag
    {
        private const int MoneyBagArraySize = 10;
        private const int NameIndex = 0;
        private const int BagIndex = 1;
        private const int MaxGoldInBag = 2000;
        private const int LootListArraySize = 20;
        private const int GoldArrayArraySize = 10;
        private const int GoldValueIndex = 6;
        private const double MaxGoldValue = 100.0;

        /// <summary>
        /// Fills all money bags in the inventory to max capacity.
        /// </summary>
        /// <param name="inventory">the inventory</param>
        /// <returns>Tuple(number of bags, total money in bags</returns>
        public static Tuple<int, int> FillMoneyBags(JArray inventory)
        {
            if (inventory == null)
            {
                return new Tuple<int, int>(-1, -1);
            }
            var countBags = 0;
            var countMoney = 0;
            foreach (var jToken in inventory)
            {
                if (!(jToken is JArray { Count: MoneyBagArraySize } jArray)
                    || !"o_inv_moneybag".Equals($"{jArray[NameIndex]}"))
                {
                    continue;
                }
                if (!(jArray[BagIndex] is JObject moneybag)
                    || !"moneybag".Equals($"{moneybag["idName"]}") || moneybag["Stack"] == null)
                {
                    return new Tuple<int, int>(-1, -1);
                }
                countBags += 1;
                var stack = (int)moneybag["Stack"].Value<double>();
                if (stack == MaxGoldInBag)
                {
                    countMoney += MaxGoldInBag;
                    continue;
                }
                if (!(moneybag["lootList"] is JArray lootList) || lootList.Count == 0)
                {
                    return new Tuple<int, int>(-1, -1);
                }
                if (lootList.Count == LootListArraySize)
                {
                    if (!TopUpMoneyBag(moneybag))
                    {
                        return new Tuple<int, int>(-1, -1);
                    }
                    continue;
                }
                if (!SetFullMoneyBag(moneybag))
                {
                    return new Tuple<int, int>(-1, -1);
                }
            }
            return new Tuple<int, int>(countBags, countMoney);

            bool TopUpMoneyBag(JObject moneybag)
            {
                if (moneybag["Stack"] == null ||
                    !(moneybag["lootList"] is JArray lootList))
                {
                    return false;
                }
                var bagMoney = 0;
                foreach (var goldToken in lootList)
                {
                    if (!(goldToken is JArray { Count: GoldArrayArraySize } goldArray))
                    {
                        return false;
                    }
                    var goldTextValue = goldArray[GoldValueIndex].ToString();
                    if (!double.TryParse(goldTextValue, out var goldValue) ||
                        goldValue < 0 || goldValue > MaxGoldValue)
                    {
                        return false;
                    }
                    if (goldValue < MaxGoldValue)
                    {
                        goldArray[GoldValueIndex] = MaxGoldValue;
                    }
                    bagMoney += (int)goldArray[GoldValueIndex].Value<double>();
                }
                moneybag["Stack"] = bagMoney;
                countMoney += (int)moneybag["Stack"].Value<double>();
                return true;
            }

            bool SetFullMoneyBag(JObject moneybag)
            {
                var fullBag = MoneyBagJson.FullMoneyBag;
                if (moneybag["Stack"] == null ||
                    fullBag["Stack"] == null ||
                    !(moneybag["lootList"] is JArray lootList) ||
                    !(fullBag["lootList"] is JArray fullList))
                {
                    return false;
                }
                lootList.Clear();
                foreach (var goldItem in fullList)
                {
                    lootList.Add(goldItem);
                }
                moneybag["Stack"] = fullBag["Stack"];
                countMoney += (int)moneybag["Stack"].Value<double>();
                return true;
            }
        }

        public static Tuple<int, int> CountMoneyBags(JArray inventory)
        {
            if (inventory == null)
            {
                return new Tuple<int, int>(-1, -1);
            }
            var countBags = 0;
            var countMoney = 0;
            foreach (var jToken in inventory)
            {
                if (!(jToken is JArray { Count: MoneyBagArraySize } jArray)
                    || !"o_inv_moneybag".Equals($"{jArray[NameIndex]}"))
                {
                    continue;
                }
                if (!(jArray[BagIndex] is JObject moneybag) || !"moneybag".Equals($"{moneybag["idName"]}") ||
                    moneybag["Stack"] == null)
                {
                    return new Tuple<int, int>(-1, -1);
                }
                countBags += 1;
                countMoney += (int)moneybag["Stack"].Value<double>();
            }
            return new Tuple<int, int>(countBags, countMoney);
        }

        private static class MoneyBagJson
        {
            public static JObject FullMoneyBag
            {
                get
                {
                    if (!(JsonConvert.DeserializeObject(JsonText) is JArray { Count: MoneyBagArraySize } jArray))
                    {
                        return null;
                    }
                    if (!"o_inv_moneybag".Equals($"{jArray[NameIndex]}"))
                    {
                        return null;
                    }
                    return jArray[BagIndex] as JObject;
                }
            }

            private const string JsonText =
                @"[
  ""o_inv_moneybag"",
  {
    ""Material"": ""leather"",
    ""max_charge"": 1.0,
    ""idName"": ""moneybag"",
    ""Duration"": 0.0,
    ""is_cursed"": 0.0,
    ""MaxDuration"": 0.0,
    ""i_index"": 0.0,
    ""Stack"": 2000.0,
    ""Main"": [],
    ""identified"": 1.0,
    ""charge"": 1.0,
    ""Effects_Duration"": 0.0,
    ""lootList"": [
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        5.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        0.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        1.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        2.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        3.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        4.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        6.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        7.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        10.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        11.0,
        4.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        12.0,
        4.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        8.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        9.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        13.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        14.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        15.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        16.0,
        4.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        17.0,
        4.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        18.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ],
      [
        ""o_inv_gold"",
        {
          ""Material"": ""gold"",
          ""max_charge"": 1.0,
          ""idName"": ""gold"",
          ""Duration"": 0.0,
          ""is_cursed"": 0.0,
          ""MaxDuration"": 0.0,
          ""i_index"": 0.0,
          ""Main"": [],
          ""identified"": 1.0,
          ""charge"": 1.0,
          ""Effects_Duration"": 0.0,
          ""is_execute"": 1.0
        },
        0.0,
        19.0,
        5.0,
        1.0,
        100.0,
        0.0,
        0.0,
        ""N/A""
      ]
    ],
    ""is_execute"": 1.0
  },
  0.0,
  32.0,
  0.0,
  1.0,
  0.0,
  0.0,
  0.0,
  ""N/A""
]";
        }
    }
}
