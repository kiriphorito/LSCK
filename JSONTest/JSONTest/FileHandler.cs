using System;
using System.IO;
using System.Collections.Generic;

namespace JSONTest
{
    public class FileHandler
    {

        public String read(String fileDir)
        {
            StreamReader reader = new StreamReader(fileDir);
            return reader.ReadToEnd();
        }

        //Using List<String>
        public void insert(List<String> code , int index , String fileDir , int total)
        {
            string codeString = string.Join("\n", code.ToArray());
            insert(codeString , index , fileDir , total);
        }

        //Using String
        public void insert(String code, int index, String fileDir , int total)
        {
            for (int x = total; x >= index; x--)
            {
                String oldFile = String.Concat(fileDir + x + ".txt");
                String newFile = String.Concat(fileDir + (x + 1) + ".txt");
                System.IO.File.Move(oldFile, newFile);
            }
            String fileName = String.Concat(fileDir, index + ".txt");
            File.WriteAllText(fileName, code);
        }

        public void swap(String fileDir , int first , int second)
        {
            System.IO.File.Move(fileDir + first + ".txt" , fileDir + first + ".txt.bak"); 
            System.IO.File.Move(fileDir + second + ".txt", fileDir + first + ".txt");
            System.IO.File.Move(fileDir + first + ".txt.bak", fileDir + second + ".txt");
        }

        public void delete(int index , int total , String fileDir)
        {
            File.Delete(fileDir + index + ".txt");
            for (int x = index + 1; x <= total + 1 ; x++)
            {
                String oldFile = String.Concat(fileDir + x + ".txt");
                String newFile = String.Concat(fileDir + (x - 1) + ".txt");
                System.IO.File.Move(oldFile, newFile); 
            }
        }
    }
}
