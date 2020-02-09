using System;
using System.Collections.Generic;
using System.IO;
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

            // Prepare the data
            ExtractPostsData(posts);
            ExtractPagesData(pages);
            ExtractLayoutsData(layouts);

            // Manipulate
            TransformPostMarkdownToHtml(posts);
            TransformPagesMarkdownToHtml(pages);

            // Write the site content
            WritePostsToDisk(posts);
            WritePagesToDisk(pages);
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
}