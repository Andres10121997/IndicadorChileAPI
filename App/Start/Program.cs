using System.Threading.Tasks;

namespace IndicadorChileAPI.App.Start
{
    public class Program
    {
        public Program()
            : base()
        {

        }



        public static async Task Main(string[] args)
        {
            var App = await Startup.InitAppAsync(args: args);

            App.Run();
        }
    }
}