using System.IO;
using System.Collections.Generic;

namespace LSCK
{
    public class FileHandler
    {

        public string read(string fileDir)
        {
            var reader = new StreamReader(fileDir);
            return reader.ReadToEnd();
        }

        //Using List<String>
        public void insertSnippet(List<string> code, int index, string section, string fileDir)
        {
            string codeString = string.Join("\n", code.ToArray());
            insertSnippet(codeString , index , section , fileDir);
        }

        //Using String
        public void insertSnippet(string code, int index, string section, string fileDir)
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

        public void insertFile(string userFileDir, int index, string section, string fileDir)
        {
            string userFile = Path.GetFileName(userFileDir);
            if (!Directory.Exists(string.Concat(fileDir, @"userfiles/")))
            {
                Directory.CreateDirectory(string.Concat(fileDir, @"userfiles/"));
            }
            File.Copy(userFileDir, fileDir + @"userfiles/" + userFile);
            insertSnippet(userFile, index, section, fileDir);
        }

        public void swap(int first , int second , string section , string fileDir)
        {
            string sectionFD = fileDir + "/" + section.ToLower().Replace(" ", "") + "-";
            File.Move(sectionFD + first + ".txt" , sectionFD + first + ".txt.bak"); 
            File.Move(sectionFD + second + ".txt", sectionFD + first + ".txt");
            File.Move(sectionFD + first + ".txt.bak", sectionFD + second + ".txt");
        }

        public void delete(int index , string section , string fileDir)
        {
            string sectionFD = fileDir + "/" + section.ToLower().Replace(" ", "") + "-";
            if (File.Exists(fileDir + @"/userfiles/" + read(sectionFD + index + ".txt").Split('\n')[0]))
            {
                File.Delete(fileDir + @"/userfiles/" + read(sectionFD + index + ".txt"));
            }
            File.Delete(sectionFD + index + ".txt");
            string[] files = Directory.GetFiles(fileDir + "/", string.Concat(section.ToLower().Replace(" ", "") + "*"));
            for (int x = index + 1; x <= files.Length + 1 ; x++)
            {
                string oldFile = string.Concat(sectionFD + x + ".txt");
                string newFile = string.Concat(sectionFD + (x - 1) + ".txt");
                File.Move(oldFile, newFile); 
            }
        }
    }
}
