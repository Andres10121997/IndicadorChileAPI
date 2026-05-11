using Microsoft.Extensions.Logging;
using System;

namespace API
{
    internal static class Log<T>
    {
        static Log()
        {
            
        }



        internal static void Information(ILogger Logger,
                                         Type OType,
                                         T Value)
        {
            Logger.LogInformation(
                $"" +
                $"---" +
                $"{OType}" +
                $"Fecha y hora: {DateTime.Now}" +
                Value +
                $"---" +
                $""
            );
        }
    }
}