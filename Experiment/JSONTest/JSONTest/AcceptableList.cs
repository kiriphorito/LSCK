using System;
using System.IO;

namespace JSONTest
{
    public class AcceptableList
    {
        public AcceptableList()
        {
        }

        public void modeList(string keyword, string dir, string fileName)
        {
            string[] fileList = Directory.GetFiles(dir, keyword, SearchOption.AllDirectories);
            for (int x = 0; x < fileList.Length; x++)
            {
                fileList[x] = Path.GetFileName(fileList[x].Remove(fileList[x].IndexOf('.'))).Substring(6);;
            }
            File.WriteAllLines(fileName + ".txt", fileList);
            foreach (string line in fileList)
            {
                Console.WriteLine(line);
            }
        }
    }
}
