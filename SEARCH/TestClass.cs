using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SEARCH
{
   class TestClass
   {
      public TestClass ()
	{
       Maps m = new Maps();
         m.MyPath = @"C:\logs";
         m.AddTrigger(new MapSwapTrigger());
       StreamWriter sw = new StreamWriter("Map.xml");
       XmlSerializer ser = new XmlSerializer(m.GetType());
       ser.Serialize(sw, m);
       sw.Close();
	}
     
   }
   
}
