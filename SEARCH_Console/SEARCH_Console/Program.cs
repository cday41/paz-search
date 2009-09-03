using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Soap;


namespace SEARCH_Console
{
   class Program
   {
     static SimulatonManager sm;
     static void Main(string[] args)
      {
         StringCollection sc = new StringCollection();
         sm = new SimulatonManager();
         if (args.Length == 1)
         {
            Console.WriteLine("Correct number of arguments now parse them out and create the objects");
            if(sm.ReadInXmlFile(args[0]))
            {
               sm.AnimalManager.ReadXMLFile(args[0]);
            }
            else
            {
               Console.WriteLine(sm.ErrMessage);
               
               }
         }
         Console.WriteLine("Done press any key to exit");
         Console.ReadKey();
      }

     

     
      
   }
}
