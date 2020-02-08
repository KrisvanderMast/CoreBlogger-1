using System.Collections.Generic;
using System.Linq;

namespace CoreBlogger.Core
{
    public class Arguments
    {
        private readonly List<string> _args;
        private readonly string _outputPath;
private readonly bool _newBlogPost;
        public Arguments(string[] args)
        {
            _args = args.ToList();

            int dashOIndex = _args.FindIndex(fi => fi == "-o");
            _outputPath = _args[++dashOIndex];
        }

        public string OutputPath => _outputPath;
    }
}
