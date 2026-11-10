using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;

namespace IndicadorChileAPI.App.Start
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            #region Objects
            WebApplication App;
            #endregion

            App = Startup.InitApp(args: args);

            await App.RunAsync();
        }
    }
}