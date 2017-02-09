//Class to controll JSON and files

using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LSCK
{
    public class FJController
    {
        private SettingsJSON settingsJSON = new SettingsJSON();
        private List<JSON> json = new List<JSON>();
        private FileHandler fileHandler = new FileHandler();
        private readonly String fileDir;
        private int currentPage = 1;

        String settingsFileName = @"/settings.json";
            
         private class SettingsJSON
        {
            public String Title { get; set; }
            public List<String> PageHeadings { get; set; }
        }

        public FJController(String fileDir)
        {
            this.fileDir = fileDir;
            if (!Directory.Exists(String.Concat(fileDir, @"/data")))
            {
                Directory.CreateDirectory(String.Concat(fileDir, @"/data/"));
            }
            if (!File.Exists(String.Concat(fileDir, settingsFileName)))
            {
                List<String> newPageHeadings = new List<String>();
                newPageHeadings.Add("LSCK"); //NEED TO CHANGE TO NAME OF SOLUTION!!!!!!
                settingsJSON.Title = newPageHeadings[0];
                settingsJSON.PageHeadings = newPageHeadings;
                string jsonString = JsonConvert.SerializeObject(settingsJSON, Formatting.Indented);
                File.WriteAllText(String.Concat(fileDir , settingsFileName), jsonString);
                json.Add(new JSON(fileDir + @"/data/", 1));
                if (!Directory.Exists(String.Concat(fileDir, @"/" + 1)))
                {
                    Directory.CreateDirectory(String.Concat(fileDir, @"/data/" + 1));
                }
            }
            else
            {
                StreamReader reader = new StreamReader(String.Concat(fileDir, settingsFileName));
                string fileJson = reader.ReadToEnd();
                reader.Close();
                settingsJSON = JsonConvert.DeserializeObject<SettingsJSON>(fileJson);
                for (int x = 1 ; x <= settingsJSON.PageHeadings.Count ; x++)
                {
                    json.Add(new JSON(fileDir + @"/data/" , x));
                }
            }
        }

        public void addPage(String newPageHeader)
        {
            settingsJSON.PageHeadings.Add(newPageHeader);
            string jsonString = JsonConvert.SerializeObject(settingsJSON, Formatting.Indented);
            File.WriteAllText(String.Concat(fileDir, settingsFileName), jsonString);
            if (!Directory.Exists(String.Concat(fileDir, @"/" + settingsJSON.PageHeadings.Count)))
            {
                Directory.CreateDirectory(String.Concat(fileDir, @"/data/" + settingsJSON.PageHeadings.Count));
            }
            json.Add(new JSON(fileDir + @"/data/", settingsJSON.PageHeadings.Count));
        }

        public void setCurrentPage(int newPage)
        {
            currentPage = newPage;
        }

        public int getNoOfPages()
        {
            return settingsJSON.PageHeadings.Count;
        }

        public String getTitle()
        {
            return settingsJSON.Title;
        }

        //Get all elements of a snippet
        public Snippet readElement(int index)
        {
            Snippet element = new Snippet();
            element.position = json[currentPage - 1].getPosition(index);
            element.header = json[currentPage - 1].getHeader(index);
            element.language = json[currentPage - 1].getLanguage(index);
            element.code = fileHandler.read(fileDir + @"/data/" + currentPage + @"/" + index + @".txt");
            element.comment = json[currentPage - 1].getComment(index);
            return element;
        }

        public List<Snippet> readAll()
        {
            List<Snippet> list = new List<Snippet>();
            for (int x = 1; x <= json[currentPage - 1].numberOfEntries(); x++)
            {
                list.Add(readElement(x));
            }
            return list;
        }

        //Insert into specific position in list
        public void insert(int index , String header , String language , String comment , List<String> code)
        {
            fileHandler.insert(code , index , fileDir + @"/data/" + currentPage + @"/" , json[currentPage - 1].numberOfEntries());
            json[currentPage].insert(header, language, comment, index);
        }

        public void insert(int index, String header, String language, String comment, String code)
        {
            fileHandler.insert(code, index, fileDir + @"/data/" + currentPage + @"/" , json[currentPage - 1].numberOfEntries());
            json[currentPage - 1].insert(header, language, comment, index);
        }

        //Add to the end of the list
        public void insert(String header, String language, String comment, List<String> code)
        {
            fileHandler.insert(code, json[currentPage - 1].numberOfEntries() + 1, fileDir + @"/data/" + currentPage + @"/" , json[currentPage - 1].numberOfEntries());
            json[currentPage].insert(header, language, comment, json[currentPage - 1].numberOfEntries() + 1);
        }

        public void insert(String header, String language, String comment, String code)
        {
            fileHandler.insert(code, json[currentPage - 1].numberOfEntries() + 1, fileDir + @"/data/" + currentPage + @"/" , json[currentPage - 1].numberOfEntries());
            json[currentPage - 1].insert(header, language, comment, json[currentPage - 1].numberOfEntries() + 1);
        }

        //Swaps two different entries
        public void swap(int first , int second)
        {
            fileHandler.swap(fileDir + @"/data/" + currentPage + @"/", first, second);
            json[currentPage - 1].swap(first , second);
        }

        //Delete an entry at a specific position
        public void delete(int index)
        {
            fileHandler.delete(index, json[currentPage - 1].numberOfEntries(), fileDir + @"/data/" + currentPage + @"/");
            json[currentPage - 1].delete(index);
        }
    }
}
