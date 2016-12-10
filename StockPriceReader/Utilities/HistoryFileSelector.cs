using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockPrice
{
    public class HistoryFileSelector
    {

        private DirectoryInfo folder;

        public HistoryFileSelector(string folderToLook)
        {
            folder = new DirectoryInfo(folderToLook);
        }

        public string GetMostRecentFileFullPath()
        {
            List<FileInfo> matchedFiles = new List<FileInfo>();

            IEnumerable<FileInfo> allFiles = folder.EnumerateFiles();

            //get all files that match the conditions
            foreach (FileInfo file in allFiles)
            {
                if((file.Extension == ".TXT" || file.Extension == ".txt") && file.Name.Contains("COTAHIST"))
                {
                    matchedFiles.Add(file);
                }
            }

            //get most recent one
            string fullName = matchedFiles.OrderBy(f => f.LastWriteTime).Last().FullName;

            return fullName;
        }

    }
}
