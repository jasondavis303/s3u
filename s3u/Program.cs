using Amazon.S3;
using Amazon.S3.Transfer;
using CommandLine;
using System;
using System.IO;

namespace s3u
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts =>
                {
                    Run(opts);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                Console.WriteLine();
#if DEBUG
                Console.ReadLine();
#endif
            }
        }

        static void Run(Options opts)
        {
            if (opts.Endpoint.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
                opts.Endpoint = opts.Endpoint.Substring(7);

            if (!opts.Endpoint.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                opts.Endpoint = "https://" + opts.Endpoint;

            if (!opts.Source.EndsWith(Path.DirectorySeparatorChar))
                opts.Source += Path.DirectorySeparatorChar;

            if (!opts.Target.EndsWith('/'))
                opts.Target += '/';

            using var client = new AmazonS3Client(opts.Key, opts.Secret, new AmazonS3Config { ServiceURL = opts.Endpoint });
            using var transferUtility = new TransferUtility(client);

            
            foreach(string sourceFile in Directory.EnumerateFiles(opts.Source, "*", SearchOption.AllDirectories))
            {
                string targetFile = opts.Target + sourceFile.Substring(opts.Source.Length).Replace(Path.DirectorySeparatorChar, '/');
                bool doUpload = true;
                if (TargetExists(opts, client, targetFile))
                {
                    doUpload = opts.Overwrite;
                    if(doUpload)
                        client.DeleteObjectAsync(opts.Bucket, targetFile).Wait();                    
                }
                if(doUpload)
                {
                    var req = new TransferUtilityUploadRequest
                    {
                        BucketName = opts.Bucket,
                        FilePath = sourceFile,
                        Key = targetFile
                    };
                    
                    transferUtility.UploadAsync(req).Wait();
                    Console.WriteLine("Uploaded: {0}", targetFile);
                }
            }
        }

        static bool TargetExists(Options opts, AmazonS3Client client, string key)
        {
            try
            {
                using var ret = client.GetObjectAsync(opts.Bucket, key).Result;                
                return true;
            }
            catch { }

            return false;
        }
    }
}
