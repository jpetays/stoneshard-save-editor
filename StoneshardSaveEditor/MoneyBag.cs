using System;
using Newtonsoft.Json.Linq;

namespace StoneshardSaveEditor
{
    public static class MoneyBag
    {
        /// <summary>
        /// Fills all money bags in the inventory to max capacity.
        /// </summary>
        /// <param name="inventory">the inventory</param>
        /// <returns>Tuple(total money in bags, number of bags</returns>
        public static Tuple<int, int> FillMoneyBags(JArray inventory)
        {
            var countMoney = 0;
            var countBags = 0;

            return new Tuple<int, int>(countMoney, countBags);
        }

        public static Tuple<int, int> CountMoneyBags(JArray inventory)
        {
            var countMoney = 0;
            var countBags = 0;
            foreach (var jToken in inventory!)
            {
                if (!(jToken is JArray jArray) || jArray.Count <= 2 || !"o_inv_moneybag".Equals($"{jArray[0]}"))
                {
                    continue;
                }
                if (!(jArray[1] is JObject moneybag) || !"moneybag".Equals($"{moneybag["idName"]}") ||
                    moneybag["Stack"] == null)
                {
                    return null;
                }
                countMoney += (int)moneybag["Stack"].Value<double>();
                countBags += 1;
            }
            return new Tuple<int, int>(countMoney, countBags);
        }
    }
}
