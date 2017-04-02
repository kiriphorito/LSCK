using System.IO;
using System.Collections.Generic;
namespace LSCK
{
    public class FileHandler
    {

        public string Read(string fileDir)
        {
            string result = "";
            using (StreamReader reader = new StreamReader(fileDir))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        public List<string> GetFileNames(string sectionName, string fileDir)
        {
            var result = new List<string>();
            string[] files = Directory.GetFiles(fileDir + "/", string.Concat(sectionName.ToLower().Replace(" ", "") + "*"));
            foreach (string path in files)
            {
                result.Add(Read(path));
            }
            return result;
        }

        //Using List<String>
        public void InsertSnippet(List<string> code, int index, string section, string fileDir)
        {
            string codeString = string.Join("\n", code.ToArray());
            InsertSnippet(codeString, index, section, fileDir);
        }

        //Using String
        public void InsertSnippet(string code, int index, string section, string fileDir)
        {
            string sectionFD = fileDir + "/" + section.ToLower().Replace(" ", "") + "-";
            string[] files = Directory.GetFiles(fileDir + "/", string.Concat(section.ToLower().Replace(" ", "") + "*"));
            for (int x = files.Length; x >= index; x--)
            {
                string oldFile = sectionFD + x + ".txt";
                string newFile = sectionFD + (x + 1) + ".txt";
                File.Move(oldFile, newFile);
            }
            string fileName = sectionFD + index + ".txt";
            File.WriteAllText(fileName, code);
        }

        public void InsertFile(string userFileDir, int index, string section, string fileDir)
        {
            string userFile = Path.GetFileName(userFileDir);
            if (!Directory.Exists(string.Concat(fileDir, @"userfiles/")))
            {
                Directory.CreateDirectory(string.Concat(fileDir, @"userfiles/"));
            }
            File.Copy(userFileDir, fileDir + @"userfiles/" + userFile);
            InsertSnippet(userFile, index, section, fileDir);
        }

        public void Swap(int first, int second, string section, string fileDir)
        {
            string sectionFD = fileDir + "/" + section.ToLower().Replace(" ", "") + "-";
            File.Move(sectionFD + first + ".txt", sectionFD + first + ".txt.bak");
            File.Move(sectionFD + second + ".txt", sectionFD + first + ".txt");
            File.Move(sectionFD + first + ".txt.bak", sectionFD + second + ".txt");
        }

        public void Delete(int index, string section, string fileDir)
        {
            string sectionFD = fileDir + "/" + section.ToLower().Replace(" ", "") + "-";
            //If the snippet has a file associated with it such as an image
            if (File.Exists(fileDir + @"/userfiles/" + Read(sectionFD + index + ".txt").Split('\n')[0]))
            {
                File.Delete(fileDir + @"/userfiles/" + Read(sectionFD + index + ".txt"));
            }
            File.Delete(sectionFD + index + ".txt");
            string[] files = Directory.GetFiles(fileDir + "/", string.Concat(section.ToLower().Replace(" ", "") + "*"));
            for (int x = index + 1; x <= files.Length + 1; x++)
            {
                string oldFile = string.Concat(sectionFD + x + ".txt");
                string newFile = string.Concat(sectionFD + (x - 1) + ".txt");
                File.Move(oldFile, newFile);
            }
        }
    }
}
