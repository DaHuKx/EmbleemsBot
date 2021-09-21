using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EmbleemsBot
{
    static class Get
    {
        static private StreamReader GetReader(string PathFromCurrentDirectory)
        {
            string path = Directory.GetCurrentDirectory();
            StreamReader reader = new StreamReader(path + "/" + PathFromCurrentDirectory, Encoding.UTF8);

            return reader;
        }

        static public string FileText(string PathFromCurrentDirectory)
        {
            StreamReader reader = GetReader(PathFromCurrentDirectory);

            string Text = reader.ReadToEnd();
            reader.Close();

            return Text;
        }

        static public List<string> ListOfLines(string PathFromCurrentDirectory)
        {
            StreamReader reader = GetReader(PathFromCurrentDirectory);
            List<string> list = new List<string>();

            list.AddRange(reader.ReadToEnd().Split('\n'));

            return list;
        }
    }
}
