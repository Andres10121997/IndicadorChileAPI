using IndicadorChileAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static IndicadorChileAPI.App.Interfaces.IStatistics;

namespace IndicadorChileAPI.Mathematics
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
                    checked(Convert.ToSingle(value: CurrencyList.Sum<CurrencyModel>(selector: item => Math.Pow(x: item.Currency, y: 2))))
                },
                {
                    StatisticsEnum.Average,
                    CurrencyList.Average<CurrencyModel>(selector: item => item.Currency)
                }
            };
        }
    }
}