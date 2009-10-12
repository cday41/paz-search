using System;
using System.Collections.Generic;
using System.IO;

namespace SEARCH
{
   /// <summary>
   /// Summary description for SiteHomeRangeTrigger.
   /// </summary>
   public sealed class SiteHomeRangeTrigger : IHomeRangeTrigger
   {
		#region Constructors (1) 

      public SiteHomeRangeTrigger(int numTimes, List<Animal> inDispersers)
      {
         buildLogger();
         this.mNumTimesNeeded = numTimes;
         myTriggers = new Dictionary<int, int>();
         this.reset(inDispersers);
      }

		#endregion Constructors 

		#region Fields (3) 

      private Dictionary<int, int> myTriggers;
      private FileWriter.FileWriter fw;
      private int mNumTimesNeeded;

		#endregion Fields 

		#region Methods (4) 

		#region Public Methods (3) 

      public void addNewDispersers(List<Animal> inList)
      {
         foreach (Animal a in inList)
         {
            myTriggers.Add(a.IdNum, 0);
         }
      }

      public void reset(List<Animal> inList)
      {
         myTriggers.Clear();
         addNewDispersers(inList);
      }

      public bool timeToLookForHome(Animal inA)
      {
         bool time = false;
         fw.writeLine("inside timeToLookForHome for animal number " + inA.IdNum.ToString());
         fw.writeLine("checking to see if the site is good or not");

         if (inA.isSiteGood(ref fw))
         {
            int totalSites = myTriggers[inA.IdNum];
            myTriggers[inA.IdNum] = ++totalSites;
            fw.writeLine("it was good so we have " + totalSites + " sites");
            fw.writeLine("we need " + mNumTimesNeeded.ToString());
            if (totalSites >= mNumTimesNeeded)
            {
               time = true;
               fw.writeLine("so return true");
            }
         }
         return time;
      }

		#endregion Public Methods 
		#region Private Methods (1) 

      private void buildLogger()
      {
         string s;
         StreamReader sr;
         bool foundPath = false;
         string path = System.Windows.Forms.Application.StartupPath;
         if (File.Exists(path + "\\logFile.dat"))
         {
            sr = new StreamReader(path + "\\logFile.dat");
            while (sr.Peek() > -1)
            {
               s = sr.ReadLine();
               if (s.IndexOf("HomeRangeTriggerPath") == 0)
               {
                  fw = FileWriter.FileWriter.getHomeRangeTriggerLogger(s.Substring(s.IndexOf(" ")));
                  foundPath = true;
                  break;
               }
            }
            sr.Close();

         }
         if (!foundPath)
         {
            fw = new FileWriter.EmptyFileWriter();
         }

      }

		#endregion Private Methods 

		#endregion Methods 
   }
}
