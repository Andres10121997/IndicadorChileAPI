using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace IndicadorChileAPI
{
    internal static class Utils
    {
        #region ConstructorMethod
        static Utils()
        {

        }
        #endregion



        #region Message
        public static void ErrorMessage(Exception ex,
                                        Type OType)
        {
            Console.Error.WriteLine("");
            Console.Error.WriteLine("---");
            Console.Error.WriteLine(OType);
            Console.Error.WriteLine(DateTime.Now);
            Console.Error.WriteLine($"Stack Trace: {ex.StackTrace}");
            Console.Error.WriteLine($"Message: {ex.Message}");
            Console.Error.WriteLine(ex);
            Console.Error.WriteLine("---");
            Console.Error.WriteLine("");
        }

        public static async Task ErrorMessageAsync(Exception ex,
                                                   Type OType)
        {
            await Task.Run(action: () => ErrorMessage(ex: ex, OType: OType));
        }

        public static void OutMessage(string Message,
                                      Type? OType)
        {
            Console.Out.WriteLine("");
            Console.Out.WriteLine("---");
            Console.Out.WriteLine(OType);
            Console.Out.WriteLine(DateTime.Now);
            Console.Out.WriteLine($"Message: {Message}");
            Console.Out.WriteLine("---");
            Console.Out.WriteLine("");
        }

        public static async Task OutMessageAsync(string Message,
                                                 Type? OType)
        {
            await Task.Run(action: () => OutMessage(Message: Message, OType: OType));
        }
        #endregion



        #region Logger
        internal static void LoogerInformation(ILogger Logger, string Message, Type OType)
        {
            Logger.LogInformation("");
            Logger.LogInformation("---");
            Logger.LogInformation($"{OType}");
            Logger.LogInformation($"{DateTime.Now}");
            Logger.LogInformation(message: Message);
            Logger.LogInformation("---");
            Logger.LogInformation("");
        }

        internal static async Task LoogerInformationAsync(ILogger Logger, string Message, Type OType)
        {
            await Task.Run(action: () => LoogerInformation(Logger: Logger, Message: Message, OType: OType));
        }

        internal static void LoggerError(ILogger Logger, Exception ex, Type OType)
        {
            Logger.LogError("");
            Logger.LogError("---");
            Logger.LogError($"{OType}");
            Logger.LogError($"{DateTime.Now}");
            Logger.LogError($"Stack Trace: {ex.StackTrace}");
            Logger.LogError($"Message: {ex.Message}");
            Logger.LogError($"{ex}");
            Logger.LogError("---");
            Logger.LogError("");
        }

        internal static async Task LoggerErrorAsync(ILogger Logger, Exception ex, Type OType)
        {
            await Task.Run(action: () => LoggerError(Logger: Logger, ex: ex, OType: OType));
        }
        #endregion
    }
}