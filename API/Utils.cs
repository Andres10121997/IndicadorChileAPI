using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace API
{
    internal static class Utils
    {
        #region Variables
        private static ParallelOptions parallelForEachOptions;
        #endregion



        #region Constructor Method
        static Utils()
        {
            parallelForEachOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                TaskScheduler = TaskScheduler.Current
            };
        }
        #endregion



        #region Field
        internal static ParallelOptions ParallelForEachOptions
        {
            get => parallelForEachOptions;
            set => parallelForEachOptions = value;
        }
        #endregion



        internal static void LoogerInformation(ILogger Logger,
                                               Type OType,
                                               string Message)
        {
            Logger.LogInformation(
                $"" +
                $"---" +
                $"{OType}" +
                $"Fecha y hora: {DateTime.Now}" +
                Message +
                $"---" +
                $""
            );
        }
    }
}