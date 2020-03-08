using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
            ParsePostsWithLayoutAndWriteToDisk(posts, layouts);
            ParsePagesWithLayout(pages, layouts);

            Console.WriteLine($"It took {sw.ElapsedMilliseconds}ms to parse the data");
            sw.Restart();

            // Write the site content
            // WritePostsToDisk(posts);
            WritePagesToDisk(pages);
            CopyAssets();

            CreateIndexPages(posts, layouts["index"]);
            CreateTagPages(posts);
            CreateCategoriesPages(posts);

            sw.Stop();
            Console.WriteLine($"It took {sw.ElapsedMilliseconds}ms to physically create the site.");
        }

        private void CreateIndexPages(List<Post> posts, string indexLayout)
        {
            var orderedPosts = OrderAndDetermineNextPreviousPost(posts);

            // Calculate how many index pages
            int amountOfIndexPages = posts.Count() / _cv.PostsPerIndexPage;
            int extraIndexPage = posts.Count() % _cv.PostsPerIndexPage == 0 ? 0 : 1;
            int totalAmountOfIndexPages = amountOfIndexPages + extraIndexPage;

            var indexPages = new Dictionary<int, List<Post>>(totalAmountOfIndexPages);

            // Split up the posts per page
            for (int i = 0; i < totalAmountOfIndexPages; i++)
            {
                indexPages.Add(i, orderedPosts.Skip(i * _cv.PostsPerIndexPage).Take(_cv.PostsPerIndexPage).ToList());
                var sb = new StringBuilder();

                for (int j = 0; j < totalAmountOfIndexPages; j++)
                {
                    string cssClass = j < i ? "previousIndexPage" : j == i ? "currentIndexPage" : "nextIndexPage";
                    string pageNumber = j == 0 ? string.Empty : $"page{j+1}/";
                    sb.Append($"<span class'{cssClass}'><a href='{_cv.BaseUrl}{pageNumber}index.html'>{j+1}</a></span>");
                }

                string prevNext = sb.ToString();
                sb = new StringBuilder();

                for (int j = 0; j < indexPages[i].Count(); j++)
                {
                    sb.Append($"<a href='{indexPages[i][j].Url}'>{indexPages[i][j].FrontMatter.Title}</a><br/>");
                }

                string html = indexLayout.Replace("{{ content }}", sb.ToString()).Replace("{{ page.PreviousNext }}", prevNext);

                string path = i == 0 ? _cv.SitePath : Path.Combine(_cv.SitePath, $"page{i + 1}");
                IOHelper.MakeSureSubfoldersExist(path);
                IOHelper.WriteFile(html, Path.Combine(path, "index.html"));
            }
        }

        private void CreateTagPages(List<Post> posts)
        {

        }

        private void CreateCategoriesPages(List<Post> posts)
        {

        }

        private void CopyAssets()
        {
            IOHelper.DirectoryCopy(_cv.AssetsOriginalPath, _cv.AssetsOutputPath, true);
        }

        private void ParsePostsWithLayoutAndWriteToDisk(List<Post> posts, Dictionary<string, string> layouts)
        {
            // For the MVP (Minimal Viable Product) we're assuming that single will be the main layout for posts.
            // This will become dynamic from _config.yml later on where defaults for posts will be added

            foreach (Post post in OrderAndDetermineNextPreviousPost(posts))
            {
                post.Html = layouts["single"].Replace("{{ content }}", post.Html);
                post.Html = post.Html
                    .Replace("{{ post.Previous }}", post.Previous)
                    .Replace("{{ post.Next }}", post.Next);

                IOHelper.MakeSureSubfoldersExist(post.FullySpecifiedFolder);
                IOHelper.WriteFile(post.Html, post.FullySpecifiedFolderAndFileName);
            }
        }

        private void ParsePagesWithLayout(List<Page> pages, Dictionary<string, string> layouts)
        {

        }

        // private void WritePostsToDisk(List<Post> posts)
        // {
        //     foreach (Post post in posts)
        //     {
        //         IOHelper.MakeSureSubfoldersExist(post.FullySpecifiedFolder);
        //         IOHelper.WriteFile(post.Html, post.FullySpecifiedFolderAndFileName);
        //     }
        // }

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

            posts.Add(new Post(frontMatter, originalBody, fileInfo.Name, _cv.PostOutputPath, _cv.BaseUrl));
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
            // Later on this will become a more dynamic algorithnm to determine all layouts, 
            // likely via a recursive function where default page is the beginning
            layouts["single"] = layouts["default"].Replace("{{ content }}", layouts["single"]);
            layouts["index"] = layouts["default"].Replace("{{ content }}", layouts["index"]);
            layouts["tags"] = layouts["default"].Replace("{{ content }}", layouts["tags"]);
            layouts["categories"] = layouts["default"].Replace("{{ conent }}", layouts["categories"]);
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

        private IEnumerable<Post> OrderAndDetermineNextPreviousPost(List<Post> posts)
        {
            List<Post> orderedEnumerable = posts.OrderByDescending(o => o.Year).ThenByDescending(t => t.Month).ThenByDescending(t => t.Day).ToList();

            int postsCount = orderedEnumerable.Count();
            for (int i = 0; i < postsCount; i++)
            {
                orderedEnumerable[i].Previous = i == 0 ? "" : orderedEnumerable[i - 1].Url;
                orderedEnumerable[i].Next = i == postsCount - 1 ? "" : orderedEnumerable[i + 1].Url;
            }

            return orderedEnumerable;
        }
    }
}