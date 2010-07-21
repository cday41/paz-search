/******************************************************************************
 * Change Date:   02/10/2008
 * Change By:     Bob Cummings
 * Description:   Added homeSiteTriggerLogger
 * ***************************************************************************/
using System;
using System.IO;
using Microsoft.Win32;

namespace FileWriter
{
   /// <summary>
   /// The class that will write the logging out to a text file. -The Singleton defines an Instance operation that lets clients access its unique instance. 
   /// -It may be responsible for creating its own unique instance.
   /// 
   /// 
   /// </summary>
   public class FileWriter
   {
      private StreamWriter sw;
      public FileWriter()
      {
         //not implemented only needed for empty file writer class
      }
      
      private static FileWriter uniqueInstance;
      private static FileWriter homeRangeLogger;
      private static FileWriter animalLogger;
      private static FileWriter animalManagerLogger;
      private static FileWriter animalMapLogger;
      private static FileWriter moveLogger;
      private static FileWriter mapLogger;
      private static FileWriter mapManagerLogger;
      private static FileWriter mapsLogger;
      private static FileWriter formLogger;
      private static FileWriter homeRangeTriggerLogger;
      private static FileWriter homeSiteTriggerLogger;
      private static FileWriter dataLogger;

      public FileWriter(string inFileName)
      {
          sw = new StreamWriter(inFileName);
      }

      public virtual void writeLine(string data)
      {
            sw.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff") + ":  " + data);
            sw.Flush();
      }
      public virtual void close()
      {
            sw.Close();
      }
      
     
      public static FileWriter GetUniqueInstance(string filePath)
      {

         if(uniqueInstance == null) 
         {  
            //get rid of any old log files
            if (File.Exists(filePath))
               File.Delete(filePath);
            uniqueInstance = new FileWriter(filePath);
         }
         return uniqueInstance;
      }

      public static FileWriter getAnimalManagerLogger(string filePath)
      {
         if(animalManagerLogger == null) 
         {
            //get rid of any old log files
            if (File.Exists(filePath))
               File.Delete(filePath);
            animalManagerLogger = new FileWriter(filePath);
         }
         return animalManagerLogger;
      }

      public static FileWriter getAnimalMapLogger(string filePath)
      {
         if(animalMapLogger == null) 
         {
            //get rid of any old log files
            if (File.Exists(filePath))
               File.Delete(filePath);
            animalMapLogger = new FileWriter(filePath);
         }
         return animalMapLogger;
      }

      public static FileWriter getAnimalLogger(string filePath)
      {
         if(animalLogger == null) 
         {
            //get rid of any old log files
            if (File.Exists(filePath))
               File.Delete(filePath);
            animalLogger = new FileWriter(filePath);
         }
         return animalLogger;
      }

      public static FileWriter getMoverLogger(string filePath)
      {
         if(moveLogger == null) 
         { 
            //get rid of any old log files
            if (File.Exists(filePath))
               File.Delete(filePath);
            moveLogger = new FileWriter(filePath);
         }
         return moveLogger;
      }

      public static FileWriter getMapLogger(string filePath)
      {
         if(mapLogger == null) 
         { 
            //get rid of any old log files
            if (File.Exists(filePath))
               File.Delete(filePath);
            mapLogger = new FileWriter(filePath);
         }
         return mapLogger;
      }
      public static FileWriter getMapManagerLogger(string filePath)
      {
         if(mapManagerLogger == null) 
         { 
            //get rid of any old log files
            if (File.Exists(filePath))
               File.Delete(filePath);
            mapManagerLogger = new FileWriter(filePath);
         }
         return mapManagerLogger;
      }
      public static FileWriter getMapsLogger(string filePath)
      {
         if(mapsLogger == null) 
         { 
            //get rid of any old log files
            if (File.Exists(filePath))
               File.Delete(filePath);
            mapsLogger = new FileWriter(filePath);
         }
         return mapsLogger;
      }
      public static FileWriter getFormLogger(string filePath)
      {
         if(formLogger == null) 
         { 
            //get rid of any old log files
            if (File.Exists(filePath))
               File.Delete(filePath);
            formLogger = new FileWriter(filePath);
         }
         return formLogger;
      }

      public static FileWriter getHomeRangeLogger(string filePath)
      {
         if(homeRangeLogger == null) 
         { 
            //get rid of any old log files
            if (File.Exists(filePath))
               File.Delete(filePath);
            homeRangeLogger = new FileWriter(filePath);
         }
         return homeRangeLogger;
      }

      public static FileWriter getHomeRangeTriggerLogger(string filePath)
      {
         if(homeRangeTriggerLogger == null) 
         { 
            //get rid of any old log files
            if (File.Exists(filePath))
               File.Delete(filePath);
            homeRangeTriggerLogger = new FileWriter(filePath);
         }
         return homeRangeTriggerLogger;
      }

      public static FileWriter getHomeSiteLogger(string filePath)
      {
         if(homeSiteTriggerLogger == null) 
         { 
            //get rid of any old log files
            if (File.Exists(filePath))
               File.Delete(filePath);
            homeSiteTriggerLogger = new FileWriter(filePath);
         }
         return homeSiteTriggerLogger;
      }


      public static FileWriter getDataLogger(string filePath)
      {
         if (dataLogger == null)
         {
            //get rid of any old log files
            if (File.Exists(filePath))
               File.Delete(filePath);
            dataLogger = new FileWriter(filePath);
         }
         return dataLogger;
      }

      public static void WriteErrorFile(System.Exception ex)
      {
         StreamWriter swE = new StreamWriter(@"C:\SEARCH\logs\DispersalError.log",true);
         swE.WriteLine();
         swE.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff") + ":  " + ex.Message);
         swE.WriteLine(ex.StackTrace);
         swE.Close();
         swE = null;
      }
      public static void AddToErrorFile(string data)
      {
         StreamWriter swE = new StreamWriter(@"C:\SEARCH\logs\DispersalError.log",true);
         swE.WriteLine();
         swE.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss:fff") + ":  " + data);
         swE.Close();
         swE = null;
      }
   }
}
