namespace Website.App.Start
{
    public class Program
    {
        static void Main(string[] args)
        {
            var App = Startup.InitApp(args: args);

            App.Run();
        }
    }
}