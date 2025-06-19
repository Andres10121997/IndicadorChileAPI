namespace IndicadorChileAPI.App.Start
{
    public class Program
    {
        public Program()
            : base()
        {

        }



        ~Program()
        {

        }



        static void Main(string[] args)
        {
            var App = Startup.InitApp(args: args);

            App.Run();
        }
    }
}