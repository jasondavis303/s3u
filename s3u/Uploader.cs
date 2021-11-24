using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace s3u
{
    public static class Uploader
    {
        public static async Task UploadAsync(Options opts, CancellationToken cancellationToken = default)
        {
            if (opts.Endpoint.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
                opts.Endpoint = opts.Endpoint.Substring(7);

            if (!opts.Endpoint.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                opts.Endpoint = "https://" + opts.Endpoint;

            if (!opts.Source.EndsWith(Path.DirectorySeparatorChar))
                opts.Source += Path.DirectorySeparatorChar;

            if (!opts.Target.EndsWith('/'))
                opts.Target += '/';

            using var client = new AmazonS3Client(opts.AccessKey, opts.AccessSecret, new AmazonS3Config { ServiceURL = opts.Endpoint });
            using var transferUtility = new TransferUtility(client);

            if (opts.Include != null && opts.Include.Count() > 0)
            {
                foreach (string include in opts.Include)
                    foreach (string sourceFile in Directory.EnumerateFiles(opts.Source, include, SearchOption.AllDirectories))
                        await UploadFileAsync(opts, client, transferUtility, sourceFile, cancellationToken).ConfigureAwait(false);
            }
            else if (opts.Exclude != null && opts.Exclude.Count() > 0)
            {
                var excludeLst = new List<string>();
                foreach (string exclude in opts.Exclude)
                    excludeLst.AddRange(Directory.EnumerateFiles(opts.Source, exclude, SearchOption.AllDirectories));
                foreach (string sourceFile in Directory.EnumerateFiles(opts.Source, "*", SearchOption.AllDirectories))
                    if (!excludeLst.Contains(sourceFile))
                        await UploadFileAsync(opts, client, transferUtility, sourceFile, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                foreach (string sourceFile in Directory.EnumerateFiles(opts.Source, "*", SearchOption.AllDirectories))
                    await UploadFileAsync(opts, client, transferUtility, sourceFile, cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task UploadFileAsync(Options opts, AmazonS3Client client, TransferUtility transferUtility, string sourceFile, CancellationToken cancellationToken = default)
        {
            string targetFile = opts.Target + sourceFile.Substring(opts.Source.Length).Replace(Path.DirectorySeparatorChar, '/');
            bool doUpload = true;

            bool targetExists = false;
            try
            {
                if (opts.Verbose)
                    Console.WriteLine("Checking {0}", targetFile);
                using var ret = await client.GetObjectAsync(opts.Bucket, targetFile, cancellationToken).ConfigureAwait(false);
                targetExists = true;
            }
            catch { }


            if (targetExists)
            {
                doUpload = opts.Overwrite;
                if (doUpload)
                {
                    if (opts.Verbose)
                        Console.WriteLine("Deleting {0}", targetFile);
                    await client.DeleteObjectAsync(opts.Bucket, targetFile, cancellationToken).ConfigureAwait(false);
                }
            }
            if (doUpload)
            {
                var req = new TransferUtilityUploadRequest
                {
                    BucketName = opts.Bucket,
                    FilePath = sourceFile,
                    Key = targetFile
                };

                if (opts.Verbose)
                    Console.WriteLine("Uploading {0}", targetFile);
                await transferUtility.UploadAsync(req, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
