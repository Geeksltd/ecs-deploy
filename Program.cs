using System;
using Olive;
using System.Linq;

namespace ECS_Deploy
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.FirstOrDefault() == "?")
                {
                    ShowHelp();
                }
                else
                {
                    Parameters.Load(args);

                    DefaultSettings.LoadSettings();

                    var updatedTaskDefenition = TaskManager.RegisterTaskDefenition().GetAwaiter().GetResult();

                    ServiceManager.CreateOrUpdate(updatedTaskDefenition).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToFullMessage());
                Console.ResetColor();
            }

            Console.ReadKey();
        }

        private static void ShowHelp()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(Parameters.GetParametersInfo());
            Console.ResetColor();
        }
    }
}
