using CommandLine;

namespace s3u
{
    class Options
    {
        [Option("endpoint", Required = true)]
        public string Endpoint { get; set; }

        [Option("key", Required = true)]
        public string Key { get; set; }

        [Option("secret", Required = true)]
        public string Secret { get; set; }

        [Option("source", Required = true)]
        public string Source { get; set; }

        [Option("bucket", Required = true)]
        public string Bucket { get; set; }

        [Option("target", Required = true)]
        public string Target { get; set; }

        [Option("overwrite")]
        public bool Overwrite { get; set; }
    }
}
