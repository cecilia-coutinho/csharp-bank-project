namespace BankProject
{
    internal class CurrencyConverter
    {
        public double ConvertionRate { get; set; }
        public double CurrencyAmount { get; set; }

        public CurrencyConverter(double currencyAmount)
        {
            this.CurrencyAmount = currencyAmount;
        }

        protected double CurrencyConverterCalculator(double currencyAmount)
        {
            return currencyAmount / ConvertionRate;

        }

        public virtual string PrintCurrencyConverter(double currencyAmount)
        {
            return $"\n\tExchange Rate:{ConvertionRate}\n\texchanged value: {CurrencyConverterCalculator(currencyAmount)}";
        }
    }
}
