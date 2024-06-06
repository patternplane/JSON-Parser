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
        [STAThreadAttribute]
        static void Main(string[] args)
        {
            // Parser
            System.Windows.Forms.OpenFileDialog fd = new System.Windows.Forms.OpenFileDialog();
            fd.Multiselect = true;
            fd.ShowDialog(); 
            System.Windows.Forms.FolderBrowserDialog fb = new System.Windows.Forms.FolderBrowserDialog();
            fb.ShowDialog();


            foreach (string file in fd.FileNames) {
                StreamReader sr = new StreamReader(file);

                char[] rawData = sr.ReadToEnd().ToCharArray();
                sr.Close();


                StreamWriter sw = new StreamWriter(fb.SelectedPath + "\\" + Path.GetFileNameWithoutExtension(file) + ".txt");

                JSONParser p = new JSONParser();
                sw.Write(p.parse(rawData, false));

                sw.Close();
            }
        }
    }
}
