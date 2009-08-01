using System;
using System.IO;
using DesignByContract;
namespace PAZ_Dispersal
{
   /// <summary>
   /// Attributes for a gender/species combination animal
   /// </summary>
   public class AnimalAtributes
   {
		#region Public Members (3) 

		#region Constructors (1) 

      public AnimalAtributes()
      {
         this.buildLogger();
         fw.writeLine("inside the animal attribtures constructor");
      }

		#endregion Constructors 
		#region Methods (2) 

      public double addDuration(Duration inD)
      {
         
         double numHoursLeft = hasMoreHours(ref inD);
         try
         {
            if (this.myActivityDuration != null)
            {
               Duration [] tmpD = new Duration[this.myActivityDuration.Length + 1];
               this.myActivityDuration.CopyTo(tmpD,0);
               tmpD[tmpD.Length-1] = inD;
               this.myActivityDuration = new Duration[tmpD.Length];
               tmpD.CopyTo(this.myActivityDuration,0);
            }
            else
            {
               this.myActivityDuration = new Duration[1];
               this.myActivityDuration[0]=inD;
            }
         }
         catch(System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return numHoursLeft;
      }

      public void getDurationMeanAndSD(ref double mean,ref double sd,ref int durationID)
      {
         try
         {
            fw.writeLine("inside getDurationMeanAndSD for  " + durationID.ToString());
            //TODO remove before release
            Check.Require(durationID <= this.myActivityDuration.GetUpperBound(0),"Duration ID is more then upper bound of the duration array in animal attributes");
            //get the mean and sd for the timed duration
            mean = this.myActivityDuration[durationID].MeanAmt;
            sd = this.myActivityDuration[durationID].StandardDeviation;
            fw.writeLine("the mean is " + mean.ToString() + " sd = " + sd.ToString());
            //advance the duration 
            durationID++;
            //if reached the end of the cycle then reset.
            if (durationID > this.myActivityDuration.GetUpperBound(0))
               durationID = 0;
            fw.writeLine("new duration id is " + durationID.ToString());

         }
         catch(System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show (ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }

      }

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (2) 

		#region Fields (1) 

      private FileWriter.FileWriter fw;

		#endregion Fields 
		#region Methods (1) 

      private double hasMoreHours(ref Duration inD)
      {
         double d = 0;
         //make sure we are not in the first one created
         if (myActivityDuration != null)
         {
            //add up all the allocated so far
            foreach(Duration dur in myActivityDuration)
            {
               d += dur.MeanAmt;
            }
         }
         // add in the new amount
         d+=inD.MeanAmt;
         //if over 24 hours then reset back to only allow 24 hours
         if (d > 24)
         {
            d-=inD.MeanAmt;
            inD.MeanAmt = numHoursInADay - d;
            d+=inD.MeanAmt;
         }
         return numHoursInADay - d;
      }

		#endregion Methods 

		#endregion Non-Public Members 


      #region private Variables

      private double mInitialEnergy;         //initial energy level for start of program
      private double mMaxEnergy;             //most energy this species can store
      private double mMinEnergy_Survive;     //less amount before starvation
  
      private Duration [] myActivityDuration;//an array of times for george to be awake or asleep
      private double mWakeUpTime;            //start of the day for george
      
      private double mPerceptionDistance;    //how far can george see around him(base line)

      private double mForageSearchTrigger;   //level of energy that will force george to switch from 
                                             //searching for a home to foraging for food and back
      private double mRiskySafeTrigger;      //percentage to make george feel safe (Roll of Dice – Observed Risk) < mRiskySafeTrigger
      private double mSafeRiskyTrigger;      //percentage to make george feel scared (Roll of Dice – Observed Risk) > mSafeRiskyTrigger
		private double mSafeProbCaptureFoodMod;  //modifier used to adjust george's probability of capturing food wwhen he feels safe
		private double mSafeProbGetKilledMod;  //modifier used to adjust george's probability of getting killed when he feels safe
		private double mRiskyProbCaptureFoodMod;  //modifier used to adjust george's probability of capturing food when he feels threatened
		private double mRiskyProbGetKilledMod;  //modifier used to adjust george's probability of getting killed when he feels threatened


    
      private string mErrMessage;
      private string mOutPutDir;
      private const double numHoursInADay = 24;
     
      #endregion

      #region getters and setters
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
               if (s.IndexOf("AnimalLogPath") == 0)
               {
                  fw= FileWriter.FileWriter.getAnimalLogger(s.Substring(s.IndexOf(" ")));
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

		
		public Duration [] ActivityDurations
		{
			get{ return myActivityDuration;}
		}
		public string OutPutDir
      {
         get { return mOutPutDir; }
         set  { mOutPutDir = value; 
         fw.writeLine("inside setting the output dir for animal attributes to " + value);}
      }




      public double InitialEnergy
      {
         get { return mInitialEnergy; }
         set 
         {
            mInitialEnergy = value; 
            fw.writeLine("AnimalAtributes.mInitialEnergy = " + mInitialEnergy.ToString());}
      }
         
      public double MaxEnergy
      {
         get { return mMaxEnergy; }
         set 
         {
            mMaxEnergy = value; 
            fw.writeLine("AnimalAtributes.mMaxEnergy = " + mMaxEnergy.ToString());}

      }
             
      public double MinEnergy_Survive
      {
         get { return mMinEnergy_Survive; }
         set 
         {
            mMinEnergy_Survive = value; 
            fw.writeLine("AnimalAtributes.mMinEnergy_Survive = " + mMinEnergy_Survive.ToString());}

      }
    
  
      public double WakeUpTime
      {
         get { return mWakeUpTime; }
         set 
         {
            mWakeUpTime = value; 
            fw.writeLine("AnimalAtributes.mWakeUpTime = " + mWakeUpTime.ToString());}

      }
            
           

      public double PerceptionDistance
      {
         get { return mPerceptionDistance; }
         set 
         {
            mPerceptionDistance = value; 
            fw.writeLine("AnimalAtributes.mPerceptionDistance = " + mPerceptionDistance.ToString());}

      }
    

      public double ForageSearchTrigger
      {
         get { return mForageSearchTrigger; }
         set 
         {
            mForageSearchTrigger = value; 
            fw.writeLine("AnimalAtributes.mForageSearchTrigger = " + mForageSearchTrigger.ToString());}

      }
   
      public double RiskySafeTrigger
      {
         get { return mRiskySafeTrigger; }
         set 
         {
            mRiskySafeTrigger = value; 
            fw.writeLine("AnimalAtributes.mRiskySafeTrigger = " + mRiskySafeTrigger.ToString());}

      }
 
      public double SafeRiskyTrigger
      {
         get { return mSafeRiskyTrigger; }
         set 
         {
            mSafeRiskyTrigger = value; 
            fw.writeLine("AnimalAtributes.mSafeRiskyTrigger = " + mSafeRiskyTrigger.ToString());}

      }
    
      public string ErrMessage
      {
         get { return mErrMessage; }
         set 
         {
            mErrMessage = value; 
            fw.writeLine("AnimalAtributes.mErrMessage = " + mErrMessage.ToString());}

		}
		public double SafeProbCaptureFoodMod
		{
			get { return mSafeProbCaptureFoodMod; }
			set { mSafeProbCaptureFoodMod = value; }
		}

		public double SafeProbGetKilledMod
		{
			get { return mSafeProbGetKilledMod; }
			set { mSafeProbGetKilledMod = value; }
		}

		public double RiskyProbCaptureFoodMod
		{
			get { return mRiskyProbCaptureFoodMod; }
			set { mRiskyProbCaptureFoodMod = value; }
		}

		public double RiskyProbGetKilledMod
		{
			get { return mRiskyProbGetKilledMod; }
			set { mRiskyProbGetKilledMod = value; }
		}
     #endregion
   }
}
