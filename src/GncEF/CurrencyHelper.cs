using GncEF.Models;

namespace GncEF
{
    public static class CurrencyHelper
    {
        public static GncCommodity GetCommodityFromMnenomic(this GncContext context, string commodityName)
        {
            return context.Commodities.SingleOrDefault(c => c.Mnenomic == commodityName);
        }

        public static GncCommodity GetUsdCurrency(this GncContext context)
        {
            return context.GetCommodityFromMnenomic("USD");
        }
     }
}
