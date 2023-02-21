using System.Globalization;

namespace BankProject
{
    internal class ConvertToEuro : CurrencyConverter
    {
        protected double ConvertionRateEuro = 11.17f;
        public ConvertToEuro(double currencyAmount) : base(currencyAmount)
        {
            this.ConvertionRate = ConvertionRateEuro;
        }


        public override string PrintCurrencyConverter(double currencyAmount)
        {
            return $"\n\tExchange Rate: {ConvertionRateEuro}\n\tExchange Value in EUR:{CurrencyConverterCalculator(currencyAmount).ToString("c2", CultureInfo.CreateSpecificCulture("fr-FR"))}";
        }
    }
}
