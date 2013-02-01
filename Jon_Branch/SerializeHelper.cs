﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SEARCH
{
  public static class SerializeHelper
   {
     public static void SerializeObjectToFile(string inFileName, object inO)
      {
          StreamWriter sw = new StreamWriter(inFileName);
          try
          {
              XmlSerializer ser = new XmlSerializer(inO.GetType());
              ser.Serialize(sw, inO);
          }
          catch (Exception ex)
          {
              while (ex != null)
              {
                  Console.WriteLine(ex.Message);
                  ex = ex.InnerException;
              }
          }
          finally
          {
              sw.Close();
          }         
      }
     public static object DeserializeFromFile(string inFileName, object inO)
     {
         XmlSerializer ser = new XmlSerializer(inO.GetType());
         StreamReader sr = new StreamReader(inFileName);
         try
         {
             object o = ser.Deserialize(sr);
             sr.Close();
             return o;
         }
         catch (Exception ex)
         {
             sr.Close();
             while (ex != null)
             {
                 Console.WriteLine(ex.Message);
                 ex = ex.InnerException;
             }
             return null;
         }
     }
   }
}