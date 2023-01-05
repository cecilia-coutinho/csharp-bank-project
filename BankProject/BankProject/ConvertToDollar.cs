using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankProject
{
    internal class ConvertToDollar : CurrencyConverter
    {
        protected double ConvertionRateDollar = 10.51f;
        public ConvertToDollar(double currencyAmount) : base(currencyAmount)
        {
            this.ConvertionRate = ConvertionRateDollar;
        }


        public override string PrintCurrencyConverter(double currencyAmount)
        {
            return $"\n\tExchange Rate: {ConvertionRateDollar}\n\tExchange Value USS:{CurrencyConverterCalculator(currencyAmount).ToString("c2", CultureInfo.CreateSpecificCulture("en-us"))}";
        }
    }
}
