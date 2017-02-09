//Class for reading and writing to JSON files.

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace JSONTest
{
    public class JSON
    {
        private RootJSONObject JSONFile;
        private String fileDir;

        //For each snippet of code that the user wishes to add
        private class JSONCode
        {
            public int position { get; set; }
            public string header { get; set; }
            public string language { get; set; }
            public string comment { get; set; }
        }

        //To hold the main objct for JSON
        private class RootJSONObject
        {
            public List<JSONCode> Codes { get; set; }
        }

        //Constructor
        public JSON(String fileDir , int page)
        {
            this.JSONFile = new RootJSONObject();
            this.fileDir = fileDir + @"/" + page + @".json";
            readJSON();
        }

        //Read from JSON file or create one if it doesn't exit
        private void readJSON()
        {
            if (!File.Exists(fileDir))
            {
                JSONFile.Codes = new List<JSONCode>();
                writeJSON();
            }
            else 
            {
                StreamReader reader = new StreamReader(fileDir);
                string fileJson = reader.ReadToEnd();
                reader.Close();
                JSONFile = JsonConvert.DeserializeObject<RootJSONObject>(fileJson);
            }
        }

        //Writes to/Updates the JSON file
        private void writeJSON()
        {
            string jsonString = JsonConvert.SerializeObject(JSONFile, Formatting.Indented);
            File.WriteAllText(fileDir, jsonString);
        }

        //Insert an entry at any position value
        public void insert(string header, string language, string comment, int index)
        {
            //New entry for the List<>
            JSONCode newEntry = new JSONCode();
            newEntry.position = index;
            newEntry.header = header;
            newEntry.language = language;
            newEntry.comment = comment;

            JSONFile.Codes.Insert(index - 1 , newEntry);
            for (int x = index ; x < JSONFile.Codes.Count; x++)
            {
                JSONFile.Codes[x].position++;
            }

            writeJSON();
        }

        //Swaps the position value of two different elements
        public void swap(int firstElement , int secondElement)
        {
            JSONFile.Codes[firstElement - 1].position = secondElement;
            JSONFile.Codes[secondElement - 1].position = firstElement;
            JSONFile.Codes.Insert(firstElement - 1, JSONFile.Codes[secondElement - 1]);
            JSONFile.Codes.Insert(secondElement, JSONFile.Codes[firstElement]);
            JSONFile.Codes.RemoveAt(firstElement);
            JSONFile.Codes.RemoveAt(secondElement);
            writeJSON();
        }

        //Deletes an entry
        public void delete(int index)
        {
            JSONFile.Codes.RemoveAt(index - 1);
            for (int x = index - 1 ; x < JSONFile.Codes.Count; x++)
            {
                JSONFile.Codes[x].position--;
            }
            writeJSON();
        }

        //Number of items in the list
        public int numberOfEntries()
        {
            return JSONFile.Codes.Count;
        }

        //List of headers for all entries
        public List<String> listOfHeaders()
        {
            List<String> headers = new List<string>();
            foreach (var item in JSONFile.Codes)
            {
                headers.Add(item.header);
            }
            return headers;
        }

        //List of comments for all entries
        public List<String> listOfComments()
        {
            List<String> comments = new List<string>();
            foreach (var item in JSONFile.Codes)
            {
                comments.Add(item.header);
            }
            return comments;
        }

        public int getPosition(int index)
        {
            return JSONFile.Codes[index - 1].position;
        }

        public String getHeader(int index)
        {
            return JSONFile.Codes[index - 1].header;
        }

        public String getLanguage(int index)
        {
            return JSONFile.Codes[index - 1].language;
        }

        public String getComment(int index)
        {
            return JSONFile.Codes[index - 1].comment;
        }
    }
}
