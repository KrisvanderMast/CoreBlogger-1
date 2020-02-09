using System;
using System.Collections.Generic;

namespace CoreBlogger.Core
{
    internal class Generator
    {
        public Generator()
        {
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



            // Write the site content
        }

        private void ExtractPostsData(List<Post> posts)
        {
            throw new NotImplementedException();
        }

        private void ExtractPagesData(List<Page> pages)
        {
            throw new NotImplementedException();
        }

        private void ExtractLayoutsData(Dictionary<string, string> layouts)
        {
            throw new NotImplementedException();
        }

        private void AppendCoreVariablesWithConfigYaml(CoreVariables cv)
        {
            // read out the config.yml, transform it and append the new read in data to the CoreVariables properties
        }

        internal void CreateNewSite(CoreVariables coreVariables)
        {
            throw new NotImplementedException();
        }

        internal void CreateNewBlogPost(CoreVariables coreVariables)
        {
            throw new NotImplementedException();
        }
    }
}