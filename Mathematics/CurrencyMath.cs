using IndicadorChileAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static IndicadorChileAPI.App.Interfaces.IStatistics;

namespace IndicadorChileAPI.Mathematics
{
    public static class CurrencyMath
    {
        static CurrencyMath()
        {
            
        }



        public static Dictionary<StatisticsEnum, double> MathematicalOperations(CurrencyModel[] CurrencyList)
        {
            return new Dictionary<StatisticsEnum, double>
            {
                {
                    StatisticsEnum.Count,
                    CurrencyList.Length
                },
                {
                    StatisticsEnum.Minimum,
                    Math.Round(value: CurrencyList.Min<CurrencyModel>(selector: x => x.Currency), digits: 2)
                },
                {
                    StatisticsEnum.Maximum,
                    Math.Round(value: CurrencyList.Max<CurrencyModel>(selector: x => x.Currency), digits: 2)
                },
                {
                    StatisticsEnum.Sum,
                    Math.Round(value: CurrencyList.Sum<CurrencyModel>(selector: x => x.Currency), digits: 2)
                },
                {
                    StatisticsEnum.SumOfSquares,
                    CurrencyList.Sum<CurrencyModel>(selector: x => Math.Pow(x: x.Currency, y: 2))
                },
                {
                    StatisticsEnum.Average,
                    Math.Round(value: CurrencyList.Average<CurrencyModel>(selector: x => x.Currency), digits: 2)
                }
            };
        }
    }
}