using IndicadorChileAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static IndicadorChileAPI.App.Interfaces.IStatistics;

namespace IndicadorChileAPI.App.Mathematics
{
    public static class CurrencyMath
    {
        #region Constructor Method
        static CurrencyMath()
        {
            
        }
        #endregion



        public static Dictionary<StatisticsEnum, float> MathematicalOperations(CurrencyModel[] CurrencyList)
        {
            return new Dictionary<StatisticsEnum, float>()
            {
                {
                    StatisticsEnum.Count,
                    CurrencyList.Length
                },
                {
                    StatisticsEnum.Minimum,
                    CurrencyList.Min(selector: item => item.Currency)
                },
                {
                    StatisticsEnum.Maximum,
                    CurrencyList.Max(selector: item => item.Currency)
                },
                {
                    StatisticsEnum.Sum,
                    CurrencyList.Sum(selector: item => item.Currency)
                },
                {
                    StatisticsEnum.SumOfSquares,
                    checked(Convert.ToSingle(value: CurrencyList.Sum(selector: item => Math.Pow(x: item.Currency, y: 2))))
                },
                {
                    StatisticsEnum.Average,
                    CurrencyList.Average(selector: item => item.Currency)
                }
            };
        }
    }
}