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
            return new Dictionary<StatisticsEnum, double>()
            {
                {
                    StatisticsEnum.Count,
                    CurrencyList.Length
                },
                {
                    StatisticsEnum.Minimum,
                    CurrencyList.Min<CurrencyModel>(selector: item => item.Currency)
                },
                {
                    StatisticsEnum.Maximum,
                    CurrencyList.Max<CurrencyModel>(selector: item => item.Currency)
                },
                {
                    StatisticsEnum.Sum,
                    CurrencyList.Sum<CurrencyModel>(selector: item => item.Currency)
                },
                {
                    StatisticsEnum.SumOfSquares,
                    CurrencyList.Sum<CurrencyModel>(selector: item => Math.Pow(x: item.Currency, y: 2))
                },
                {
                    StatisticsEnum.Average,
                    CurrencyList.Average<CurrencyModel>(selector: item => item.Currency)
                }
            };
        }
    }
}