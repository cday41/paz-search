using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
namespace Utility
{
   
        public static class SerializeHelper
        {
            public static void SerializeObjectToFile(string inFileName, object inO)
            {
                GetDirectory(inFileName);
                StreamWriter sw = new StreamWriter(inFileName);
                XmlSerializer ser = new XmlSerializer(inO.GetType());
                ser.Serialize(sw, inO);
                sw.Close();
            }

            private static void GetDirectory(string inFileName)
            {
                string backupDir = Path.GetDirectoryName(inFileName);
                if (String.IsNullOrEmpty(backupDir))
                {
                    backupDir = Directory.GetCurrentDirectory();

                }
                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }
                
            }
            public static object DeserializeFromFile(string inFileName, object inO)
            {
                XmlSerializer ser = new XmlSerializer(inO.GetType());
                StreamReader sr = new StreamReader(inFileName);
                object o = ser.Deserialize(sr);
                sr.Close();
                return o;
            }

            public static void SerializeDataContractObjectToFile(string inFileName, object inO)
            {
                GetDirectory(inFileName);
                FileStream writer = new FileStream(inFileName, FileMode.OpenOrCreate);
                var ser = new DataContractSerializer(inO.GetType());
                ser.WriteObject(writer, inO);
            }
        }
    
}
