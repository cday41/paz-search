using System;
using System.IO;

namespace PAZ_Dispersal
{
	/// <summary>
	/// Summary description for SiteHomeRangeTrigger.
	/// </summary>
	public sealed class SiteHomeRangeTrigger:IHomeRangeTrigger
	{
      private FileWriter.FileWriter fw;
      private int mNumTimesNeeded;
      //this will hold the number of good sites each animal has visited
      //we are indexing by the animals id so goodSites[0] will hold the number of
      //good sites animal 0 has witnessed
      private int[] goodSites;

		public SiteHomeRangeTrigger(int numTimes,int inNumAnimals)
		{
         this.buildLogger();
         fw.writeLine("inside SiteHomeRangeTrigger constructor we need " + numTimes.ToString());
         this.mNumTimesNeeded = numTimes;
         goodSites = new int[inNumAnimals];
      }

      private void buildLogger()
      {
         string s;
         StreamReader sr; 
         bool foundPath = false;
         string path = System.Windows.Forms.Application.StartupPath;
         if(File.Exists(path +"\\logFile.dat"))         
         {
            sr= new StreamReader(path +"\\logFile.dat");
            while(sr.Peek() > -1)
            {
               s = sr.ReadLine();
               if (s.IndexOf("HomeRangeTriggerPath") == 0)
               {
                  fw= FileWriter.FileWriter.getHomeRangeTriggerLogger(s.Substring(s.IndexOf(" ")));
                  foundPath = true;
                  break;
               }
            }
            sr.Close();

         }
         if (! foundPath)
         {
            fw = new FileWriter.EmptyFileWriter();
         }
        
      }
      
      #region HomeRangeTrigger Members

      public int numTimes
      {
         get
         {
            return mNumTimesNeeded;
         }
         set
         {
            mNumTimesNeeded = value;
         }
      }

      public void reset(int inNumAnimals)
      {
         
         goodSites = new int[inNumAnimals];
      }
      public bool timeToLookForHome(Animal inA)
      {
         bool time = false;
         fw.writeLine("inside timeToLookForHome for animal number " + inA.IdNum.ToString());
         fw.writeLine("checking to see if the site is good or not");
         bool isSiteGood = inA.isSiteGood(ref fw);
         if (isSiteGood)
         {
            fw.writeLine("it was good so we have " + System.Convert.ToString(goodSites[inA.IdNum] + 1) + " sites");
            fw.writeLine("we need " + mNumTimesNeeded.ToString());
            if (++goodSites[inA.IdNum]>=mNumTimesNeeded)
            {
               time = true;
               fw.writeLine("so return true");
            }
         }
         return time;
      }

      #endregion
   }
}
