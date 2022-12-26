using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankProject
{
    internal class CurrencyConverter
    {
        public float ConvertionRate { get; set; }
        public float CurrencyAmount { get; set; }

        public CurrencyConverter(float currencyAmount)
        {
            this.CurrencyAmount = currencyAmount;
        }

        protected float CurrencyConverterCalculator(float currencyAmount)
        {
            return currencyAmount / ConvertionRate;

        }

        public virtual string PrintCurrencyConverter(float currencyAmount)
        {
            return $"\n\tExchange Rate:{ConvertionRate}\n\texchanged value: {CurrencyConverterCalculator(currencyAmount)}";
        }
    }
}
