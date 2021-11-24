using CommandLine;
using System.Collections.Generic;

namespace s3u
{
    public class Options
    {
        /// <summary>
        /// S3 Endpoint (https://s3.us-west-1.amazonaws.com)
        /// </summary>
        [Option("endpoint", Required = true)]
        public string Endpoint { get; set; }

        [Option("access-key", Required = true)]
        public string AccessKey { get; set; }

        [Option("access-secret", Required = true)]
        public string AccessSecret { get; set; }

        /// <summary>
        /// Source directory
        /// </summary>
        [Option("source", Required = true, HelpText = "Source directory")]
        public string Source { get; set; }

        [Option("include", HelpText = "Filter to only include the specified files", SetName = "Filter")]
        public IEnumerable<string> Include { get; set; } 

        [Option("exclude", HelpText = "Filter to exclude specified files", SetName = "Filter")]
        public IEnumerable<string> Exclude { get; set; }


        [Option("bucket", Required = true)]
        public string Bucket { get; set; }

        /// <summary>
        /// Target directory
        /// </summary>
        [Option("target", Required = true, HelpText = "Target directory")]
        public string Target { get; set; }

        [Option("overwrite")]
        public bool Overwrite { get; set; }

        [Option("verbose")]
        public bool Verbose { get; set; }
    }
}
