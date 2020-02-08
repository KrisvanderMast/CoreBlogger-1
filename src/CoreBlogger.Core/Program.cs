using System;

namespace CoreBlogger.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            foreach (var item in args)
            {
                System.Console.WriteLine(item);
            }


            var arguments = new Arguments(args);
        }
    }
}
