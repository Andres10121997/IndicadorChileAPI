using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace IndicadorChileAPI
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
            DateOnly Date = DateOnly.FromDateTime(Now);
            TimeOnly Time = TimeOnly.FromDateTime(Now);
            #endregion

            Logger.LogInformation(message: "");
            Logger.LogInformation(message: "---");
            Logger.LogInformation(message: OType.ToString());

            // Fecha y Hora
            Logger.LogInformation(message: $"Fecha: {Date}");
            Logger.LogInformation(message: $"Hora: {Time}");

            Logger.LogInformation(message: Message);
            Logger.LogInformation(message: "---");
            Logger.LogInformation(message: "");
        }

        internal static async Task LoogerInformationAsync(ILogger Logger, Type OType, string Message) => await Task.Run(action: () => LoogerInformation(Logger: Logger, Message: Message, OType: OType));

        internal static void LoggerError(ILogger Logger, Type OType, Exception ex)
        {
            #region Variables
            DateTime Now = DateTime.Now;
            DateOnly Date = DateOnly.FromDateTime(Now);
            TimeOnly Time = TimeOnly.FromDateTime(Now);
            #endregion

            Logger.LogError(message: "");
            Logger.LogError(message: "---");
            Logger.LogError(message: OType.ToString());

            // Fecha y Hora
            Logger.LogError(message: $"Fecha: {Date}");
            Logger.LogError(message: $"Hora: {Time}");

            Logger.LogError(message: $"Stack Trace: {ex.StackTrace}");
            Logger.LogError(message: $"Message: {ex.Message}");
            Logger.LogError(message: ex.ToString());
            Logger.LogError(message: "---");
            Logger.LogError(message: "");
        }

        internal static async Task LoggerErrorAsync(ILogger Logger, Type OType, Exception ex) => await Task.Run(action: () => LoggerError(Logger: Logger, OType: OType, ex: ex));
        #endregion
    }
}