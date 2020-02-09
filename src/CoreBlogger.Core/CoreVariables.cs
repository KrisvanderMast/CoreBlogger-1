using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreBlogger.Core
{
    public class CoreVariables
    {
        private readonly List<string> _args;
        private readonly string _workingDirectoryPath;
        public CoreVariables(string[] args)
        {
            _args = args.ToList();

            int newPost = _args.FindIndex(fi => fi == "new");
            NewBlogPost = newPost != -1;

            int createNewSite = _args.FindIndex(fi => fi == "createsite");
            CreateNewSite = createNewSite != -1;

            int dashWIndex = _args.FindIndex(fi => fi == "-w");
            _workingDirectoryPath = dashWIndex != -1 ? _args[++dashWIndex] : Environment.CurrentDirectory;

            PostsPath = $"{_workingDirectoryPath}\\_posts";
            PagesPath = $"{_workingDirectoryPath}\\_pages";
            IncludesPath = $"{_workingDirectoryPath}\\_includes";
            LayoutsPath = $"{_workingDirectoryPath}\\_layouts";
            SitePath = $"{_workingDirectoryPath}\\_site";

            PostOutputPath = $"{SitePath}\\post";
        }

        public bool NewBlogPost { get; }
        public bool CreateNewSite { get; }

        public string WorkingDirectoryPath => _workingDirectoryPath;
        public string PostsPath { get; private set; }
        public string PagesPath { get; private set; }
        public string IncludesPath { get; private set; }
        public string LayoutsPath { get; private set; }
        public string SitePath { get; private set; }
        public string PostOutputPath { get; internal set; }
    }
}
