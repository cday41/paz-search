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
      static void Main(string[] args)
      {
         StringCollection sc = new StringCollection();
        
         if (args.Length == 1)
         {
            Console.WriteLine("Correct number of arguments now parse them out and create the objects");
            MakeSimulationManager(args[0]);
         }
         Console.WriteLine("Done press any key to exit");
         Console.ReadKey();
      }

      static void MakeSimulationManager(string inFileName)
      {
         SimulatonManager sm = new SimulatonManager();
         sm.ReadInXmlFile(inFileName);

      }

     
      
   }
}
