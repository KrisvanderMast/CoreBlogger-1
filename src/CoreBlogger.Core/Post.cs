using System;
using System.Collections.Generic;
using System.IO;

namespace CoreBlogger.Core
{
    internal class Post
    {
        public Post(PostFrontMatter frontMatter, string markdown, string originalFileName, string postOutputPath)
        {
            FrontMatter = frontMatter;
            Markdown = markdown;
            OriginalFileName = originalFileName;

            Year = originalFileName[0..4];
            Month = originalFileName[5..7];
            Day = originalFileName[8..10];

            string fileName = $"{originalFileName[11..^3]}.html";
            FullySpecifiedFolder = Path.Combine(postOutputPath, Year, Month, Day);
            FullySpecifiedFolderAndFileName = Path.Combine(FullySpecifiedFolder, fileName);
        }

        public PostFrontMatter FrontMatter { get; private set; }
        public string Markdown { get; private set; }
        public string Html { get; set; }
        public string OriginalFileName { get; private set; }
        public string Year { get; internal set; }
        public string Month { get; internal set; }
        public string Day { get; internal set; }
        public string FullySpecifiedFolder { get; internal set; }
        public string FullySpecifiedFolderAndFileName { get; internal set; }
    }

    public class PostFrontMatter
    {
        public bool Search { get; set; }
        public string Title { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string[] Categories { get; set; }
        public string[] Tags { get; set; }
        public string OriginalContent { get; set; }

        public string ExcerptSeparator { get; set; }
        public string Link { get; set; }
        public DateTime Date { get; set; }
        public bool Comments { get; set; }
        public bool ReadTime { get; set; }
        public bool Related { get; set; }
        public bool Share { get; set; }
        public bool Toc { get; set; }
        public string TocLabel { get; set; }
        public string TocIcon { get; set; }
        public bool TocSticky { get; set; }
        public string Excerpt { get; set; }
        public string Author { get; set; }
        public bool AuthorProfile { get; set; }
        public List<Gallery> Gallery { get; set; }
        public List<Gallery> Gallery2 { get; set; }
        public List<Gallery> Gallery3 { get; set; }
        public string Classes { get; set; }
        public List<SideBar> sidebar { get; set; }
        public Header Header { get; set; }
    }

    public class SideBar
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public string ImageAlt { get; set; }
        public string Nav { get; set; }
    }

    public class Header
    {
        public string Image { get; set; }
        public string Teaser { get; set; }
        public string OgImage { get; set; }
        public string OverlayImage { get; set; }
        public string Caption { get; set; }
        public List<Actions> Actions { get; set; }

        public string OverlayColor { get; set; }
        public Video Video { get; set; }
    }

    public class Video
    {
        public string Id { get; set; }
        public string Provider { get; set; }
    }

    public class Gallery
    {
        public string Url { get; set; }
        public string ImagePath { get; set; }
        public string Alt { get; set; }
        public string Title { get; set; }
    }

    public class Actions
    {
        public string Label { get; set; }
        public string Url { get; set; }
    }
}