using System.Threading.Tasks;

namespace IndicadorChileAPI.App.Start
{
    public class Program
    {
        #region Constructor Method
        public Program()
            : base()
        {

        }
        #endregion



        public static async Task Main(string[] args)
        {
            var App = await Startup.InitAppAsync(args: args);

            App.Run();
        }
    }
}