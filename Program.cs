using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSON_Parser
{
    class Program
    {
        static void Main(string[] args)
        {
            // Parser
            StreamReader sr = new StreamReader("");

            char[] rawData = sr.ReadToEnd().ToCharArray();
            sr.Close();

            StreamWriter sw = new StreamWriter("");

            JSONParser p = new JSONParser();
            sw.Write(p.parse(rawData, false));

            sw.Close();
        }
    }
}
