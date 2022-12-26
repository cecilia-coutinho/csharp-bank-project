using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankProject
{
    internal class ConvertToEuro : CurrencyConverter
    {
        protected float ConvertionRateEuro = 11.17f;
        public ConvertToEuro(float currencyAmount) : base(currencyAmount)
        {
            this.ConvertionRate = ConvertionRateEuro;
        }


        public override string PrintCurrencyConverter(float currencyAmount)
        {
            return $"\n\tExchange Rate: {ConvertionRateEuro}\n\tExchange Value EUR:{CurrencyConverterCalculator(currencyAmount).ToString("c2", CultureInfo.CreateSpecificCulture("fr-FR"))}";
        }
    }
}
