using CommandLine;
using System;

namespace s3u
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("S3 Directory Uploader");
                Console.WriteLine();

                Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts =>
                {
                    Uploader.UploadAsync(opts).Wait();
                })
                .WithNotParsed<Options>(opts =>
                {
                    throw new Exception("Invalid arguments");
                });

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Done!");
                Console.ResetColor();
                Console.WriteLine();
                return 0;
            }
            catch (AggregateException aex)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                foreach(var ex in aex.InnerExceptions)
                    Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.WriteLine();
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.WriteLine();
                return -1;
            }
        }
    }
}
