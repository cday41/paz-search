/******************************************************************************
 * CHANGE LOG
 * DATE:       Saturday, September 09, 2006 4:17:04 PM
 * Author:     Bob Cummings
 * Descrition: Modified the initializeYearlySimulation method.
 *             Moved winter killing of dispersers to before switching out maps.
 *             This solved the issue of swaping out release maps, adding dispersers
 *             Then killing them off before they even had a chance
 * 
 *             Added call to move maps start date ahead one year.
 *****************************************************************************/

using System;
using System.IO;
namespace SEARCH
{
   /// <summary>
   /// -The Client asks the Singleton to get its unique instance .  It accesses a Singleton instance solely through Singleton's Instance operation.
   /// </summary>
   public class SimulatonManager
   {
		#region Constructors (1) 

      public SimulatonManager()
      {
         this.buildLogger();
         fw.writeLine("making new animal manager ");
         mAnimalManager = new AnimalManager();
         fw.writeLine("getting the hourly manager collection");
         myHourlyModifiers = HourlyModifierCollection.GetUniqueInstance();
         myDailyModifiers = DailyModiferCollection.GetUniqueInstance();
         mElapsedTimeBetweenTimeStep = 0;
     //    this.makeTempMap(@"C:\MapTest");
         fw.writeLine("back in sim manager with a modifier Count of " + myHourlyModifiers.Count.ToString());
      }

		#endregion Constructors 

		#region Fields (23) 

      private DailyModifier currDailyMod;
      //temporal modifiers and their managers
      private HourlyModifier currHourMod;
      private DateTime currTime;
      private double currTimeStep;
      private FileWriter.FileWriter fw;
      //animal and map managers
      private AnimalManager mAnimalManager;
      private bool mDoTextOutPut;
      private double mElapsedTimeBetweenTimeStep;
      private DateTime mEndSeasonDate;
      private DateTime mEndSimulatonDate;
      private string mErrMessage;
      private MapManager mMapManager;
      private string mMapOutPutPath;
      private int  mNumDaysSeason;
      private int  mNumSeasons;
      private double mNumTotalTimeSteps;
      //keep track of the time
      private DateTime mStartSeasonDate;
      private DateTime mStartSimulationDate;
      //output variables
      private string mTextOutPutFileName;
      private DailyModiferCollection myDailyModifiers;
      private HourlyModifierCollection myHourlyModifiers;
      //starting times for the temporal modifiers
      private DateTime nextDayStart;
      private int nextStartHour;

		#endregion Fields 

		#region Properties (13) 

      public AnimalManager AnimalManager
      {
         get { return mAnimalManager; }
         set { mAnimalManager = value; }
      }

      public bool DoTextOutPut
      {
         get { return mDoTextOutPut; }
         set { mDoTextOutPut = value; 
         fw.writeLine("inside sim manager setting mDoTextOutPut to " + value.ToString());}
      }

      public double ElapsedTimeBetweenTimeStep
      {
         get { return mElapsedTimeBetweenTimeStep; }
         set  { mElapsedTimeBetweenTimeStep = value; }
      }

      public DateTime EndSeasonDate
      {
         get { return mEndSeasonDate; }
         set { mEndSeasonDate = value; }
      }

      public DateTime EndSimulatonDate
		{
			get { return mEndSimulatonDate; }
			set { mEndSimulatonDate = value; }
		}

      public string ErrMessage
      {
         get { return mErrMessage;}
         set  { mErrMessage = value; }
      }

      public MapManager MapManager
      {
         get { return mMapManager; }
         set { mMapManager = value; }
      }

      public string MapOutPutPath
      {
         get { return mMapOutPutPath; }
         set { mMapOutPutPath = value; }
      }

      public int NumDaysSeason
      {
         get { return mNumDaysSeason; }
         set { mNumDaysSeason = value; }
      }

      public int NumSeasons
      {
         get { return mNumSeasons; }
         set { mNumSeasons = value; }
      }

      public DateTime StartSeasonDate
      {
         get { return mStartSeasonDate; }
         set { mStartSeasonDate = value; }
      }

      public DateTime StartSimulationDate
		{
			get { return mStartSimulationDate; }
			set  { mStartSimulationDate = value; }
		}

      public string TextOutPutFileName
      {
         get { return mTextOutPutFileName; }
         set { mTextOutPutFileName = value; }
      }

		#endregion Properties 

		#region Methods (18) 

		#region Public Methods (8) 

      /********************************************************************************
       *   Function name   : addDailyModifier
       *   Description     : 
       *   Return type     : void 
       *   Argument        : DailyModifier inDM
       * ********************************************************************************/
      public void addDailyModifier(DailyModifier inDM)
      {
         fw.writeLine("inside the sim manager adding a daily modifier the current Count is" + 
            myDailyModifiers.Count.ToString());
         myDailyModifiers.Add(inDM.StartDate, inDM);
         fw.writeLine("now the Count is " + myDailyModifiers.Count.ToString());
      }

      /********************************************************************************
       *  Function name   : addHourlyModifier
       *  Description     : 
       *  Return type     : void 
       *  Argument        : HourlyModifier inHm
       * *******************************************************************************/
      public void addHourlyModifier(HourlyModifier inHm)
      {
         try
         {
            fw.writeLine("inside sim manager adding an hourly modifier current Count is " + 
               myHourlyModifiers.Count.ToString());
            myHourlyModifiers.Add(inHm.StartTime, inHm);
            fw.writeLine("now the Count is " + myHourlyModifiers.Count.ToString());
         
         }
         catch(System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         
      }

      /********************************************************************************
       *  Function name   : buildAnimals
       *  Description     : makes the intial set of animals and their corresponding maps
       *  Return type     : bool 
       * *******************************************************************************/
      public bool buildAnimals()
      {
         bool success = false;
         InitialAnimalAttributes[] iAA = null;
         this.mMapManager.GetInitialAnimalAttributes(out iAA);
         if (iAA != null)
         {
            success = this.mAnimalManager.makeInitialAnimals(iAA);
            
         }
         else
         { 
            this.mErrMessage = "Could Not get animal attributes";
         }
         return success;

      }

      public bool buildResidents()
      {
         InitialAnimalAttributes[] iAA = null;
         this.mMapManager.GetInitalResidentAttributes(out iAA);
         this.mAnimalManager.makeResidents(iAA,currTime.Year.ToString());
         return true;
      }

      /********************************************************************************
       *  Function name   : doSimulation
       *  Description     : 
       *  Return type     : void 
       * *******************************************************************************/
      public void doSimulation(frmInput inForm)
      {
         fw.writeLine("inside sim manager do simulation calling initialize daily sim");
         initializeSimulation();
         
         this.currTime = this.mStartSeasonDate;
         this.currTime = this.currTime.AddHours( this.AnimalManager.AnimalAttributes.WakeUpTime);
         this.MapManager.CurrTime = this.currTime;
         fw.writeLine("now start the big loop for the sim");
         for (int currSeason = 0; currSeason < this.mNumSeasons; currSeason++)
         {
            fw.writeLine("starting a season");
            fw.writeLine("currDate is " + this.currTime.ToShortDateString());
            fw.writeLine("end season date is " + this.EndSeasonDate.ToShortDateString());
            while (currTime < this.EndSeasonDate)
            {
               this.doTimeStep();
               this.currTimeStep++;
               this.currTime = this.currTime.AddMinutes(this.mElapsedTimeBetweenTimeStep);
               if (currTime.Hour == 0)
               {
                  initializeDailySimulation();
                  this.upDateForm(ref inForm);
               }
            }

            //now reset the year and advance by a year
            fw.writeLine("done with one season now addvance throuout the year");
            fw.writeLine("currDate is " + this.currTime.ToShortDateString());
            this.currTime = this.mStartSeasonDate.AddYears(currSeason+1);
            this.currTime = this.currTime.AddHours( this.AnimalManager.AnimalAttributes.WakeUpTime);
            this.EndSeasonDate = this.EndSeasonDate.AddYears(1);
            fw.writeLine("now the currDate is " + this.currTime.ToShortDateString());
            fw.writeLine("calling initialize yearly sim");
            initializeYearlySimulation();
            fw.writeLine("done with initializing yearly sim");
            fw.writeLine("");
            fw.writeLine("");
         }
        // this.mMapManager.removeExtraFiles();

         System.Windows.Forms.MessageBox.Show("done");

      }

      public bool makeInitialAnimalMaps()
      {
         bool success = true;
         int numInitialAnimals = 0;
         try
         {
            fw.writeLine("inside make inital animal maps for the sim manager");
            numInitialAnimals = this.AnimalManager.getNumDispersers();
            if (numInitialAnimals > 0)
            {
               success=this.mMapManager.makeNewAnimalMaps(numInitialAnimals);
               if (success == false)
               {
                  mErrMessage = mMapManager.getErrMessage();
               }
            }
            else
            {
               success = false;
               mErrMessage = "No animals to create maps for";
            }
            
         }
         catch(System.Exception ex)
         {
            success = false;
            FileWriter.FileWriter.WriteErrorFile(ex);
           
         }
         fw.writeLine("Done making intital maps");
         return success;

      }

      public bool makeResidentMaps()
      {
         bool success = true;
         int numResidents = 0;
         try
         {
            fw.writeLine("inside make makeResidentMaps for the sim manager");
            numResidents = this.AnimalManager.getNumResidents();
            if (numResidents > 0)
            {
               success = this.mMapManager.makeNewResidentAnimalMaps(numResidents);
               if (success == false)
               {
                  mErrMessage = mMapManager.getErrMessage();
               }
            }
           

         }
         catch (Exception ex)
         {
             success = false;
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return success;
      }

      public bool makeTempMap(string path)
      { bool success = true;
      try
      {

         this.MapManager.MakeCurrStepMap(path);


      }
      
      catch (System.Exception ex)
      {
         System.Windows.Forms.MessageBox.Show(ex.Message);
         FileWriter.FileWriter.WriteErrorFile(ex);
         success = false;
         mErrMessage = "unable to make temp map look for error file";
      }
         return success;
        
      }

      public void setResidentsTextOutPut(string path)
      {
         this.mAnimalManager.setResidentsTextOutput(path, currTime.Year.ToString());
      }

      public bool setResidentAttributes(double inTimeStepRisk,double inYearlyRisk,
         double inPercentBreed, double inPercentFemale,int inMeanLitterSize,int inSDLitterSize)
      {
         bool success=true;
         try
         {
            fw.writeLine("inside sim manager setting the residents attributes");
            this.AnimalManager.setResidentModifierValues(inTimeStepRisk,inYearlyRisk,
               inPercentBreed,inPercentFemale,inMeanLitterSize, inSDLitterSize);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
            success = false;
         }
         return success;

      }



		#endregion Public Methods 
		#region Private Methods (8) 

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
               if (s.IndexOf("SimulationLogPath") == 0)
               {
                  fw= FileWriter.FileWriter.GetUniqueInstance(s.Substring(s.IndexOf(" ")));
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

      /********************************************************************************
       *  Function name   : checkTemporalModifiers
       *  Description     : 
       *  Return type     : void 
       * *******************************************************************************/
      private void checkTemporalModifiers()
      {
         fw.writeLine("inside checkTemporalModifiers ");
         fw.writeLine("nextStartHour is " + nextStartHour.ToString() );
         fw.writeLine("currTime.Hour is " + currTime.Hour.ToString());
         
         if (nextStartHour == this.currTime.Hour && this.currTime.Minute == 0)
         {
            fw.writeLine("must be time to change hourly modifier ");
            currHourMod = this.myHourlyModifiers.getNext();
            this.nextStartHour = this.myHourlyModifiers.NextStartHour;
            fw.writeLine("so now the modifier start time is " + currHourMod.StartTime.ToString());
            fw.writeLine("and the next start time is " + this.nextStartHour.ToString());
         }
         if (this.nextDayStart.CompareTo(this.currTime) == 0)
         {
            currDailyMod = this.myDailyModifiers.getNext();
            this.nextDayStart = this.myDailyModifiers.NextStartDate;
         }
         this.MapManager.changeMaps(this.currTime,this.mAnimalManager);
         
      }

      private void doOutput()
      {
      }

      /********************************************************************************
       *  Function name   : doTimeStep
       *  Description     : 
       *  Return type     : void 
       * *******************************************************************************/
      private void doTimeStep()
      {
         checkTemporalModifiers();
         fw.writeLine("doing time step for day " + this.currTime.ToShortDateString() + "  " + this.currTime.ToShortTimeString());
         fw.writeLine("my hourly modifier starttime is " + this.currHourMod.StartTime.ToString());
         fw.writeLine("my daily modifier start date is " + this.currDailyMod.StartDate.ToShortDateString());
         fw.writeLine("do text out put = " + this.DoTextOutPut.ToString());
         this.AnimalManager.doTimeStep(this.currHourMod,this.currDailyMod, this.currTime, this.mDoTextOutPut, this.mMapManager.SocialMap);
         
      }

      /********************************************************************************
       *  Function name   : initializeDailySimulation
       *  Description     : 
       *  Return type     : void 
       * *******************************************************************************/
      private void initializeDailySimulation()
      {
         //this.currHourMod = (HourlyModifier)this.myHourlyModifiers.GetByIndex(0);
      }

      /********************************************************************************
       *  Function name   : initializeSimulation
       *  Description     : 
       *  Return type     : void 
       * *******************************************************************************/
      private void initializeSimulation()
      {
         this.currHourMod = (HourlyModifier)this.myHourlyModifiers.GetByIndex(0);
         this.myHourlyModifiers.reset();
         this.currHourMod = this.myHourlyModifiers.getNext();
         this.nextStartHour = this.myHourlyModifiers.NextStartHour;
         this.myDailyModifiers.reset();
         this.currDailyMod = this.myDailyModifiers.getFirst();
         this.nextDayStart = this.myDailyModifiers.NextStartDate;
         // this should get us the number of steps per hour * 24 = numSteps per day * days per season = steps/year * numSeasons = steps/simulation
         this.mNumTotalTimeSteps = (this.mElapsedTimeBetweenTimeStep/60) * 24 * this.NumDaysSeason * this.NumSeasons;
         this.currTimeStep = 0;
      }

      /********************************************************************************
       *  Function name   : initializeYearlySimulation
       *  Description     : 
       *  Return type     : void 
       * *******************************************************************************/
      private void initializeYearlySimulation()
      {
        // int numOrginalDispersers = 0;
         fw.writeLine("inside initialize yearly simulation");
         this.myHourlyModifiers.reset();
         this.currHourMod = this.myHourlyModifiers.getNext();
         this.nextStartHour = this.myHourlyModifiers.NextStartHour;
         this.myDailyModifiers.reset();
         this.myDailyModifiers.advanceOneYear();
         this.currDailyMod = this.myDailyModifiers.getNext();
         this.nextDayStart = this.myDailyModifiers.NextStartDate;
         //09/09/2007 moved winter kill before changing maps
         //09/12/2009 changed to passing the social map so any residents that die, their area will be avaialable again.
         this.mAnimalManager.winterKillResidents(this.MapManager.SocialMap, this.currTime.Year);
         //the old mMapManager out put path include last years YEAR so we need to strip it off and replace it with this years YEAR
         this.mMapManager.OutMapPath = this.mMapManager.OutMapPath.Remove(this.mMapManager.OutMapPath.LastIndexOf("\\"),5);
         this.mMapManager.OutMapPath = this.mMapManager.OutMapPath + "\\" + this.currTime.Year.ToString();
         
         //reset the maps start dates for this year
         this.MapManager.addYearToMaps();

         //now deal with the animals.
         //Author: Bob Cummings Saturday, October 13, 2007
         this.mAnimalManager.removeRemaingDispersers();

         //had an issue with breeding before switchout the maps so moved it to happen after
         this.MapManager.setUpNewYearsMaps(this.currTime,this.mAnimalManager);
         
         int numNewAnimals = this.mAnimalManager.breedFemales(this.currTime);
         //this.mAnimalManager.setSleepTime(this.currTime);
         this.mMapManager.makeNewDisperserAnimalMaps(numNewAnimals);
         fw.writeLine("done initializing yearly simulation");
      }

      private void upDateForm(ref frmInput inForm)
      {

         inForm.updateProgressBar(this.currTimeStep/this.mNumTotalTimeSteps);
      }

		#endregion Private Methods 
		#region Internal Methods (2) 

      internal DailyModiferCollection GetDailyModiferCollection()
      {
         DailyModiferCollection mySingleton = DailyModiferCollection.GetUniqueInstance();
         return mySingleton;
      }

      internal HourlyModifierCollection GetHourlyModifierCollection()
      {
         fw.writeLine("inside sim manager getting a collecton of Hourly modifiers");
         HourlyModifierCollection myHourlyModifiers = HourlyModifierCollection.GetUniqueInstance();
         fw.writeLine("done getting the collection current Count is " + myHourlyModifiers.Count.ToString());
         return myHourlyModifiers;
      }

		#endregion Internal Methods 

		#endregion Methods 
   }
}
