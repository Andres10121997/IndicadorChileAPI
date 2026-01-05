using System;
using System.Threading.Tasks;

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
            #region Variables
            DateTime Now = DateTime.Now;
            DateOnly Date = DateOnly.FromDateTime(dateTime: Now);
            TimeOnly Time = TimeOnly.FromDateTime(dateTime: Now);
            #endregion

            Console.Error.WriteLine(value: "");
            Console.Error.WriteLine(value: "---");
            Console.Error.WriteLine(value: OType);

            // Fecha y Hora
            Console.Error.WriteLine(value: Date);
            Console.Error.WriteLine(value: Time);

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
            #region Variables
            DateTime Now = DateTime.Now;
            DateOnly Date = DateOnly.FromDateTime(dateTime: Now);
            TimeOnly Time = TimeOnly.FromDateTime(dateTime: Now);
            #endregion

            Console.Out.WriteLine(value: "");
            Console.Out.WriteLine(value: "---");
            Console.Out.WriteLine(value: OType);

            // Fecha y Hora
            Console.Out.WriteLine(value: Date);
            Console.Out.WriteLine(value: Time);

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
    }
}