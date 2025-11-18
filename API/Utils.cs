using API.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static API.App.Interfaces.IStatistics;

namespace API
{
    internal static class Utils
    {
        #region Constructor Method
        static Utils()
        {

        }
        #endregion



        #region Message
        public static void MessageError(Exception ex,
                                        Type OType)
        {
            Console.Error.WriteLine(value: "");
            Console.Error.WriteLine(value: "---");
            Console.Error.WriteLine(value: OType);
            Console.Error.WriteLine(value: DateTime.Now);
            Console.Error.WriteLine(value: $"Stack Trace: {ex.StackTrace}");
            Console.Error.WriteLine(value: $"Message: {ex.Message}");
            Console.Error.WriteLine(value: ex);
            Console.Error.WriteLine(value: "---");
            Console.Error.WriteLine(value: "");
        }

        public static async Task MessageErrorAsync(Exception ex,
                                                   Type OType)
        {
            await Task.Run(action: () => MessageError(ex: ex, OType: OType));
        }

        public static void MessageOut(string Message,
                                      Type? OType)
        {
            Console.Out.WriteLine(value: "");
            Console.Out.WriteLine(value: "---");
            Console.Out.WriteLine(value: OType);
            Console.Out.WriteLine(value: DateTime.Now);
            Console.Out.WriteLine(value: $"Message: {Message}");
            Console.Out.WriteLine(value: "---");
            Console.Out.WriteLine(value: "");
        }

        public static async Task MessageOutAsync(string Message,
                                                 Type? OType)
        {
            await Task.Run(action: () => MessageOut(Message: Message, OType: OType));
        }
        #endregion



        #region Logger
        internal static void LoogerInformation(ILogger Logger,
                                               Type OType,
                                               string Message)
        {
            #region Variables
            DateTime Now = DateTime.Now;
            DateOnly Date = DateOnly.FromDateTime(dateTime: Now);
            TimeOnly Time = TimeOnly.FromDateTime(dateTime: Now);
            #endregion

            Logger.LogInformation(message: "---");
            Logger.LogInformation(message: OType.ToString());

            // Fecha y Hora
            Logger.LogInformation(message: $"Fecha: {Date}");
            Logger.LogInformation(message: $"Hora: {Time}");

            Logger.LogInformation(message: Message);
            Logger.LogInformation(message: "---");
        }

        internal static void LoggerError(ILogger Logger,
                                         Type OType,
                                         Exception ex)
        {
            #region Variables
            DateTime Now = DateTime.Now;
            DateOnly Date = DateOnly.FromDateTime(dateTime: Now);
            TimeOnly Time = TimeOnly.FromDateTime(dateTime: Now);
            #endregion

            Logger.LogError(message: "---");
            Logger.LogError(message: OType.ToString());

            // Fecha y Hora
            Logger.LogError(message: $"Fecha: {Date}");
            Logger.LogError(message: $"Hora: {Time}");

            // Excepción
            Logger.LogError(message: $"Stack Trace: {ex.StackTrace}");
            Logger.LogError(message: $"Message: {ex.Message}");
            Logger.LogError(message: ex.ToString());
            Logger.LogError(message: "---");
        }
        #endregion



        #region Math
        internal static Dictionary<StatisticsEnum, float> MathematicalOperations(CurrencyModel[] CurrencyList)
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
        #endregion
    }
}