using System.IO;

namespace CoreBlogger.Core
{
    internal static class IOHelper
    {
        internal static void MakeSureSubfoldersExist(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            
            if (!di.Exists)
            {
                di.Create();
            }
        }

        internal static string ReadContentAsString(FileInfo fileInfo)
        {
            using (StreamReader sr = fileInfo.OpenText())
            {
                string s = string.Empty;
                s = sr.ReadToEnd();
                return s;
            }
        }
    }
}
