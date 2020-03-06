using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Markdig;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CoreBlogger.Core
{
    internal class Generator
    {
        private readonly CoreVariables _cv;

        public Generator(CoreVariables cv)
        {
            _cv = cv;
        }

        internal void GenerateSite(CoreVariables cv)
        {
            AppendCoreVariablesWithConfigYaml(cv);

            var posts = new List<Post>();
            var pages = new List<Page>();
            var layouts = new Dictionary<string, string>();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // Prepare the data
            ExtractLayoutsData(layouts);
            ExtractPostsData(posts);
            ExtractPagesData(pages);

            Console.WriteLine($"It took {sw.ElapsedMilliseconds}ms to extract the data");
            sw.Restart();

            // Manipulate
            TransformPostMarkdownToHtml(posts);
            TransformPagesMarkdownToHtml(pages);
            ParsePostsWithLayout(posts, layouts);
            ParsePagesWithLayout(pages, layouts);

            Console.WriteLine($"It took {sw.ElapsedMilliseconds}ms to parse the data");
            sw.Restart();

            // Write the site content
            WritePostsToDisk(posts);
            WritePagesToDisk(pages);
            CopyAssets();

            CreateIndexPages(posts);
            CreateTagPages(posts);
            CreateCategoriesPages(posts);

            sw.Stop();
            Console.WriteLine($"It took {sw.ElapsedMilliseconds}ms to physically create the site.");
        }

        private IEnumerable<Post> OrderAndDetermineNextPreviousPost(List<Post> posts)
        {
            List<Post> orderedEnumerable = posts.OrderByDescending(o => o.Year).ThenByDescending(t => t.Month).ThenByDescending(t => t.Day).ToList();

            for (int i = 0; i < orderedEnumerable.Count(); i++)
            {
                orderedEnumerable[i].Previous = i == 0 ? "" : orderedEnumerable[i - 1].FullySpecifiedFolderAndFileName;
                orderedEnumerable[i].Next = i == orderedEnumerable.Count() -1 ? "" : orderedEnumerable[i + 1].FullySpecifiedFolderAndFileName;
            }

            return orderedEnumerable;
        }

        private void CreateCategoriesPages(List<Post> posts)
        {

        }

        private void CreateTagPages(List<Post> posts)
        {

        }

        private void CreateIndexPages(List<Post> posts)
        {
            var p = OrderAndDetermineNextPreviousPost(posts);
        }

        private void CopyAssets()
        {
            IOHelper.DirectoryCopy(_cv.AssetsOriginalPath, _cv.AssetsOutputPath, true);
        }

        private void ParsePostsWithLayout(List<Post> posts, Dictionary<string, string> layouts)
        {
            // For the MVP (Minimal Viable Product) we're assuming that single will be the main layout for posts.
            // This will become dynamic from _config.yml later on where defaults for posts will be added

            foreach (Post post in OrderAndDetermineNextPreviousPost(posts))
            {
                post.Html = layouts["single"].Replace("{{ content }}", post.Html);

                post.Html = post.Html.Replace("{{ post.Previous }}", post.Previous).Replace("{{ post.Next }}", post.Next);
            }
        }

        private void ParsePagesWithLayout(List<Page> pages, Dictionary<string, string> layouts)
        {

        }

        private void WritePostsToDisk(List<Post> posts)
        {
            foreach (Post post in posts)
            {
                IOHelper.MakeSureSubfoldersExist(post.FullySpecifiedFolder);
                IOHelper.WriteFile(post.Html, post.FullySpecifiedFolderAndFileName);
            }
        }

        private void WritePagesToDisk(List<Page> pages)
        {

        }

        private void TransformPagesMarkdownToHtml(List<Page> pages)
        {
        }

        private void TransformPostMarkdownToHtml(List<Post> posts)
        {
            var pipeline = new MarkdownPipelineBuilder()
                            .UseAdvancedExtensions()
                            .Build();

            foreach (var post in posts)
            {
                post.Html = Markdown.ToHtml(post.Markdown, pipeline);
            }
        }

        private void ExtractPostsData(List<Post> posts)
        {
            var deserializer = new DeserializerBuilder()
                                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                        .WithNamingConvention(UnderscoredNamingConvention.Instance)
                                        .Build();

            foreach (FileInfo post in new DirectoryInfo(_cv.PostsPath).GetFiles())
            {
                ReadFileAndInitializeFrontMatter(posts, deserializer, post);
            }
        }

        private void ReadFileAndInitializeFrontMatter(List<Post> posts, IDeserializer deserializer, FileInfo fileInfo)
        {
            var originalContent = IOHelper.ReadContentAsString(fileInfo);
            string[] parts = originalContent.Split("---", 2, StringSplitOptions.RemoveEmptyEntries);

            var frontMatter = deserializer.Deserialize<PostFrontMatter>(parts[0]);
            var originalBody = parts[1];

            posts.Add(new Post(frontMatter, originalBody, fileInfo.Name, _cv.PostOutputPath));
        }

        private void ExtractPagesData(List<Page> pages)
        {

        }

        private void ExtractLayoutsData(Dictionary<string, string> layouts)
        {
            var deserializer = new DeserializerBuilder()
                                                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                                                    .Build();

            var l = new List<(string name, string masterLayout)>();

            foreach (FileInfo fileInfo in new DirectoryInfo(_cv.LayoutsPath).GetFiles())
            {
                var originalContent = IOHelper.ReadContentAsString(fileInfo);
                string[] parts = originalContent.Split("---", 2, StringSplitOptions.RemoveEmptyEntries);

                var d = deserializer.Deserialize<LayoutFrontMatter>(parts[0]);
                string name = fileInfo.Name[0..^5];
                l.Add((name, d?.Layout ?? string.Empty));

                if (!layouts.Keys.Contains(name))
                {
                    layouts.Add(name, parts[1]);
                }
            }

            // This is good enough for the MVP as there are only 2 pages at the moment in our testsite. 
            // Later on this will become a more dynamic algorithnm to determine all layouts, likely via a recursive function where default page is the beginning
            layouts["single"] = layouts["default"].Replace("{{ content }}", layouts["single"]);
        }

        private void AppendCoreVariablesWithConfigYaml(CoreVariables cv)
        {
            // read out the config.yml, transform it and append the new read in data to the CoreVariables properties

        }

        internal void CreateNewSite(CoreVariables coreVariables)
        {

        }

        internal void CreateNewBlogPost(CoreVariables coreVariables)
        {

        }
    }

    internal class LayoutFrontMatter
    {
        public string Layout { get; set; }
    }
}