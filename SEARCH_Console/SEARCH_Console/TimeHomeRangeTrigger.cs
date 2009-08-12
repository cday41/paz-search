using System;
using System.IO;
using System.Collections;


namespace SEARCH_Console
{
   
   public sealed class TimeHomeRangeTrigger:IHomeRangeTrigger
   {
		#region Public Members (1) 

		#region Constructors (1) 

      public TimeHomeRangeTrigger(int numTimes,int numAnimals)
      {
         buildLogger();
         this.numTimesCalled = new int[numAnimals];
         this.mNumTimesNeeded = numTimes;
      }

		#endregion Constructors 

		#endregion Public Members 

		#region Non-Public Members (4) 

		#region Fields (3) 

      private FileWriter.FileWriter fw;
      // private int numTimesCalled;
      private int mNumTimesNeeded;
      private int[] numTimesCalled;

		#endregion Fields 
		#region Methods (1) 

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

		#endregion Methods 

		#endregion Non-Public Members 


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

      public void reset (int numAnimals)
      {
          this.numTimesCalled = new int[numAnimals];
      }
      public bool timeToLookForHome(Animal inA)
      {
         bool success = false;
         try
         {
            fw.writeLine("inside timeToLookForHome for animal number " + inA.IdNum.ToString());
            numTimesCalled[inA.IdNum]++;
            fw.writeLine("we now have " + numTimesCalled[inA.IdNum].ToString() + " active timesteps added");
            fw.writeLine("we need " + mNumTimesNeeded.ToString());
         
            if (this.mNumTimesNeeded <= this.numTimesCalled[inA.IdNum])
            {
               fw.writeLine("so it is time to look for a home");
               success = true;
            }
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return success;
      }

      #endregion
   }
}
