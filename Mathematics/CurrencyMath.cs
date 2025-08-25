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
                    CurrencyList.Min<CurrencyModel>(selector: x => x.Currency)
                },
                {
                    StatisticsEnum.Maximum,
                    CurrencyList.Max<CurrencyModel>(selector: x => x.Currency)
                },
                {
                    StatisticsEnum.Sum,
                    Math.Round(CurrencyList.Sum<CurrencyModel>(selector: x => x.Currency), 2)
                },
                {
                    StatisticsEnum.SumOfSquares,
                    CurrencyList.Sum<CurrencyModel>(selector: x => Math.Pow(x: x.Currency, y: 2))
                },
                {
                    StatisticsEnum.Average,
                    CurrencyList.Average<CurrencyModel>(selector: x => x.Currency)
                }
            };
        }
    }
}