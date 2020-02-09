using System;

namespace CoreBlogger.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                WriteHowToMakeUseOfTheToolMessage();
                Environment.Exit(0);
            }

            var arguments = new CoreVariables(args);
        }

        private static void WriteHowToMakeUseOfTheToolMessage()
        {
            Console.WriteLine("How to make use of CoreBlogger:");
            Console.WriteLine(@"CoreBlogger [new] [createsite] -w c:\code\myblog");
            System.Console.WriteLine(string.Empty);
            System.Console.WriteLine("new: indicates you want to create a new blog post");
            System.Console.WriteLine("createsite: indicates you want to create a fully new site");
            System.Console.WriteLine("-w option: the working directory of the site");
        }
    }
}
