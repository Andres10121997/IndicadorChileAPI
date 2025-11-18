using System.Threading.Tasks;

namespace API.App.Start
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var App = await Startup.InitAppAsync(args: args);

            App.Run();
        }
    }
}