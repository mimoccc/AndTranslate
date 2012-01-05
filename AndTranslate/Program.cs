using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Threading;
using System.Text;
using System.Xml;
using AndTranslate.net.bing.api;

namespace AndTranslate
{
    //  
    class Program
    {

        private static string infile = "";
        private static string outfile = "";
        private static string fromLang = "";
        private static string toLang = "";

        static XmlDocument xdoc = new XmlDocument();
        NameValueCollection nv = new NameValueCollection();
        private Thread th;

        private static void Main(string[] args)
        {
            try
            {
                infile = args[0];
                fromLang = args[1];
                toLang = args[2];
                outfile = "translatedto-" + toLang + "-" + infile;
            }
            catch (Exception)
            {
                Console.WriteLine("Error reading arguments");
               return;
            }
                
            Program p = new Program();
    //     string result =    Translator.Translate("en", "sv", "In the Settings you can define which lap times you want to time. To prepare for a new race, select distance and press Start. Instructions will be given under the race on where to press the Time Mark button.");
        }

        public Program()
        {
            
            // read infile
            xdoc.Load(infile);
            XmlNode root = xdoc.ChildNodes.Item(1);

          
            foreach (XmlElement node in root.ChildNodes)
            {
                nv.Add(node.GetAttribute("name"), node.InnerText);
            }

            doTranslation();
            
        }


        private void doTranslation()
        {

          th = new Thread(new ThreadStart(sendRequest));
            th.Start();
        }

        private void sendRequest()
        {

            foreach (string key in nv.AllKeys)
            {
                string totrans = nv.Get(key);
                string transl = Translator.Translate(fromLang, toLang, totrans);
                nv.Set(key, transl);
                Thread.Sleep(300);
            }
            WriteOutput((fromLang), nv);
        }

        private static void WriteOutput(string lang, NameValueCollection values)
        {
            XmlDocument doc = new XmlDocument();
            //Xml Declaration
            XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
            //Attach declaration to the document
            doc.AppendChild(declaration);

            //Create root element
            XmlElement root = doc.CreateElement("resources");

            //Attach the root node to the document
            doc.AppendChild(root);

            foreach (string key in values.AllKeys)
            {
                XmlElement str = doc.CreateElement("string");
                //Add an attribute name with value John Smith
                str.SetAttribute("name", key);
                str.InnerText = values.Get(key);
                root.AppendChild(str);
            }
            doc.Save(outfile);
        }
 
    }
}
