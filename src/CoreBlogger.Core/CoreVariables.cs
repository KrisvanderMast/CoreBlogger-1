using System;
using System.Collections.Generic;
using System.IO;
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

            PostsPath = Path.Combine(_workingDirectoryPath, "_posts");
            PagesPath = Path.Combine(_workingDirectoryPath, "_pages");
            IncludesPath = Path.Combine(_workingDirectoryPath, "_includes");
            LayoutsPath = Path.Combine(_workingDirectoryPath, "_layouts");
            SitePath = Path.Combine(_workingDirectoryPath, "_site");
            AssetsImagesPath = Path.Combine(SitePath, "assets", "images");
            AssetsJsPath = Path.Combine(SitePath, "assets", "js");
            AssetsCssPath = Path.Combine(SitePath, "assets", "css");
            AssetsOriginalPath = Path.Combine(_workingDirectoryPath, "assets");
            PostOutputPath = Path.Combine(SitePath, "post");
            AssetsOutputPath = Path.Combine(SitePath, "assets");
            TagsOutputPath = Path.Combine(SitePath, "tags");
            CategoriesOutputPath = Path.Combine(SitePath, "categories");

            // todo: will come from _config.yml later on
            PostsPerIndexPage = 10;
            BaseUrl = "http://www.krisvandermast.com/";
        }

        public bool NewBlogPost { get; }
        public bool CreateNewSite { get; }

        public string WorkingDirectoryPath => _workingDirectoryPath;
        public string PostsPath { get; private set; }
        public string PagesPath { get; private set; }
        public string IncludesPath { get; private set; }
        public string LayoutsPath { get; private set; }
        public string SitePath { get; private set; }
        public string AssetsImagesPath { get; private set; }
        public string AssetsJsPath { get; }
        public string AssetsCssPath { get; }
        public string AssetsOriginalPath { get; }
        public string PostOutputPath { get; internal set; }
        public string AssetsOutputPath { get; }
        public string TagsOutputPath { get; }
        public string CategoriesOutputPath { get; private set; }
        public int PostsPerIndexPage { get; internal set; }
        public string BaseUrl { get; set; }
    }
}
