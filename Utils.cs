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
                                               string Message,
                                               Type OType)
        {
            Logger.LogInformation(message: "");
            Logger.LogInformation(message: "---");
            Logger.LogInformation(message: OType.ToString());
            Logger.LogInformation(message: DateTime.Now.ToString());
            Logger.LogInformation(message: Message);
            Logger.LogInformation(message: "---");
            Logger.LogInformation(message: "");
        }

        internal static async Task LoogerInformationAsync(ILogger Logger, string Message, Type OType) => await Task.Run(action: () => LoogerInformation(Logger: Logger, Message: Message, OType: OType));

        internal static void LoggerError(ILogger Logger, Exception ex, Type OType)
        {
            Logger.LogError(message: "");
            Logger.LogError(message: "---");
            Logger.LogError(message: OType.ToString());
            Logger.LogError(message: DateTime.Now.ToString());
            Logger.LogError(message: $"Stack Trace: {ex.StackTrace}");
            Logger.LogError(message: $"Message: {ex.Message}");
            Logger.LogError(message: ex.ToString());
            Logger.LogError(message: "---");
            Logger.LogError(message: "");
        }

        internal static async Task LoggerErrorAsync(ILogger Logger, Exception ex, Type OType) => await Task.Run(action: () => LoggerError(Logger: Logger, ex: ex, OType: OType));
        #endregion
    }
}