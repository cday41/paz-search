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
using System.Xml.XPath;
namespace SEARCH_Console
{
   /// <summary>
   /// -The Client asks the Singleton to get its unique instance .  It accesses a Singleton instance solely through Singleton's Instance operation.
   /// </summary>
   public class SimulatonManager
   {
		#region Public Members (1) 

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

		#endregion Public Members 



      #region private variables

      private bool mDoTextOutPut;
      private int  mNumDaysSeason;
      private int  mNumSeasons;
      private double mNumTotalTimeSteps;
      private double currTimeStep;
      private double mElapsedTimeBetweenTimeStep;

      //output variables
      private string mTextOutPutFileName;
      private string mMapOutPutPath;
      
      //keep track of the time
      private DateTime mStartSeasonDate;
      private DateTime mEndSeasonDate;
      private DateTime mEndSimulatonDate;
      private DateTime mStartSimulationDate;
      private DateTime currTime;

      //starting times for the temporal modifiers
      private DateTime nextDayStart;
      private int nextStartHour;
      

      //temporal modifiers and their managers
      private HourlyModifier currHourMod;
      private DailyModifier currDailyMod;
      private HourlyModifierCollection myHourlyModifiers;
      private DailyModiferCollection myDailyModifiers;
      
      //animal and map managers
      private AnimalManager mAnimalManager;
      private MapManager mMapManager;
      
      private FileWriter.FileWriter fw;
      private string mErrMessage;
      #endregion

      #region publicMethods
      
      /********************************************************************************
       *  Function name   : doSimulation
       *  Description     : 
       *  Return type     : void 
       * *******************************************************************************/
      
      public void doSimulation()
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
               Console.WriteLine("doing timestep");
               this.doTimeStep();
               this.currTimeStep++;
               this.currTime = this.currTime.AddMinutes(this.mElapsedTimeBetweenTimeStep);
               if (currTime.Hour == 0)
               {
                  initializeDailySimulation();
                  
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
         this.mAnimalManager.makeResidents(iAA);
         return true;
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
      public void loadXMLOutput(string inFileName)
      {
         XPathDocument doc = new System.Xml.XPath.XPathDocument(inFileName);
         XPathNavigator nav = doc.CreateNavigator();
         XPathNodeIterator result = nav.Select("//Tab[@name=\"Simulation\"]");
         XPathNodeIterator tempNIT = result.Current.Select(("//Output/*"));
         tempNIT.MoveNext();
         Console.WriteLine("setting the output paths");
         this.mMapManager.OutMapPath = tempNIT.Current.Value;
         this.mMapManager.makeNewAnimalMaps(this.mAnimalManager.Count);
         this.mMapManager.MakeCurrStepMap();
         this.DoTextOutPut = true;
         this.AnimalManager.AnimalAttributes.OutPutDir = tempNIT.Current.Value;
         Console.WriteLine("Done setting output ");


      }
      public bool makeInitialAnimalMaps()
      {
         bool success = true;
         int numInitialAnimals = 0;
         try
         {
            fw.writeLine("inside make inital animal maps for the sim manager");
            numInitialAnimals = this.AnimalManager.Count;
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

      public bool ReadInXmlFile(string inFileName)
      {
         bool didReadFile = true;
         System.Xml.XPath.XPathDocument doc = null;
         System.Xml.XPath.XPathNavigator nav = null;
         System.Xml.XPath.XPathNodeIterator result = null;

         Console.WriteLine("Checking to see if " + inFileName + " is a valid file");
         if(System.IO.File.Exists(inFileName))
         {
            doc = new System.Xml.XPath.XPathDocument(inFileName);
            nav = doc.CreateNavigator();
            
            Console.WriteLine("Must be now going to set the Time Parmeters");
            result = nav.Select("//Tab[@name=\"Time\"]");
            didReadFile=this.LoadTimeParameters(result);
            if (didReadFile)
            {
               result = nav.Select("//Tab[@name=\"Movement\"]");
               if (this.LoadModifiers(result))
               {
                  Console.WriteLine("Time parameters processed");
                  Console.WriteLine("Now process the maps");
                  this.mMapManager = MapManager.GetUniqueInstance();
                  this.mMapManager.ReadXML(nav);
                  this.mMapManager.changeMaps(this.StartSimulationDate);
                  Console.WriteLine("Made the maps now lets make some animals");
                  this.buildAnimals();
                  Console.WriteLine("Ok now lets build some residents");
                  this.buildResidents();
                  Console.WriteLine("Ok now lets set their attributes");
                  result = nav.Select("//Tab[@name=\"Simulation\"]");
                  this.LoadResidentAttributes(result);
                  Console.WriteLine("done setting resident attributes");

                  
               }
            }
            else
            { //Error Handle TODO
            }
         }
         else
         {
            didReadFile = false;
            mErrMessage = inFileName + " does not exist";
         }
         return didReadFile;
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
      #endregion

      #region private Methods

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
       *  Function name   : initializeDailySimulation
       *  Description     : 
       *  Return type     : void 
       * *******************************************************************************/
      
      private void initializeDailySimulation()
      {
         //this.currHourMod = (HourlyModifier)this.myHourlyModifiers.GetByIndex(0);
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
         this.mAnimalManager.winterKillResidents();
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
         
         this.mAnimalManager.breedFemales(this.currTime);
         //this.mAnimalManager.setSleepTime(this.currTime);
         this.mMapManager.makeNextGenerationAnimalMaps(this.mAnimalManager,this.currTime.Year.ToString());
         fw.writeLine("done initializing yearly simulation");
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
         this.AnimalManager.doTimeStep(this.currHourMod,this.currDailyMod, this.currTime, this.mDoTextOutPut);
         
      }


      private bool LoadTimeParameters(XPathNodeIterator inIterator)
      {
         bool DidLoadTimeParmeters = true;

         try
         {
            XPathNodeIterator temp = inIterator.Current.Select("//StartDate");
            temp.MoveNext();
            this.StartSeasonDate = System.Convert.ToDateTime(temp.Current.Value);
            temp = inIterator.Current.Select("//EndDaySeason");
            temp.MoveNext();
            this.EndSeasonDate = System.Convert.ToDateTime(temp.Current.Value);
            temp = inIterator.Current.Select("//NumSeasonDays");
            temp.MoveNext();
            this.NumDaysSeason = System.Convert.ToInt32(temp.Current.Value);
            temp = inIterator.Current.Select("//NumYears");
            temp.MoveNext();
            this.NumSeasons = System.Convert.ToInt32(temp.Current.Value);
            temp = inIterator.Current.Select("//TimeBetweenDailyTimeStep");
            temp.MoveNext();
            this.ElapsedTimeBetweenTimeStep = System.Convert.ToDouble(temp.Current.Value);
            temp = inIterator.Current.Select("//StartTime");
            temp.MoveNext();
            this.StartSimulationDate = this.StartSeasonDate.AddHours(System.Convert.ToDouble(temp.Current.Value));
            this.EndSeasonDate = this.StartSimulationDate.AddDays(this.mNumDaysSeason);
            this.EndSimulatonDate = this.EndSeasonDate.AddYears(this.NumSeasons);
         }
         catch (Exception ex)
         {

            Console.WriteLine(ex.ToString());
            DidLoadTimeParmeters = false;

         }


         return DidLoadTimeParmeters;
      }

      private bool LoadModifiers(XPathNodeIterator inIterator)
      {
         bool didLoadModifiers = true;
         try
         {
            XPathNodeIterator temp = inIterator.Current.Select("//DailyModifiers/*");
            while (temp.MoveNext())
            {
               DailyModifier dm = new DailyModifier();
               temp.Current.MoveToFirstChild();
               dm.StartDate = System.DateTime.Parse(temp.Current.Value);
               temp.Current.MoveToNext();
               dm.CaptureFood = System.Convert.ToDouble(temp.Current.Value);
               temp.Current.MoveToNext();
               dm.EnergyUsed = System.Convert.ToDouble(temp.Current.Value);
               temp.Current.MoveToNext();
               dm.MoveSpeed = System.Convert.ToDouble(temp.Current.Value);
               temp.Current.MoveToNext();
               dm.MoveTurtosity = System.Convert.ToDouble(temp.Current.Value);
               temp.Current.MoveToNext();
               dm.Name = temp.Current.Value;
               temp.Current.MoveToNext();
               dm.PerceptonModifier = System.Convert.ToDouble(temp.Current.Value);
               temp.Current.MoveToNext();
               dm.PredationRisk = System.Convert.ToDouble(temp.Current.Value);
               this.addDailyModifier(dm);


            }//end of while
            temp = inIterator.Current.Select("//HourlyModifiers/*");
            while (temp.MoveNext())
            {
               HourlyModifier hm = new HourlyModifier();
               temp.Current.MoveToFirstChild();
               hm.StartTime = System.Convert.ToInt32(temp.Current.Value);
               temp.Current.MoveToNext();
               hm.CaptureFood = System.Convert.ToDouble(temp.Current.Value);
               temp.Current.MoveToNext();
               hm.EnergyUsed = System.Convert.ToDouble(temp.Current.Value);
               temp.Current.MoveToNext();
               hm.MoveSpeed = System.Convert.ToDouble(temp.Current.Value);
               temp.Current.MoveToNext();
               hm.MoveTurtosity = System.Convert.ToDouble(temp.Current.Value);
               temp.Current.MoveToNext();
               hm.Name = temp.Current.Value;
               temp.Current.MoveToNext();
               hm.PerceptonModifier = System.Convert.ToDouble(temp.Current.Value);
               temp.Current.MoveToNext();
               hm.PredationRisk = System.Convert.ToDouble(temp.Current.Value);
               this.addHourlyModifier(hm);
            }//end of while
         }
         catch (Exception ex)
         {

            didLoadModifiers = false;
            this.ErrMessage = ex.ToString();
         }
         return didLoadModifiers;
      }

      private bool LoadResidentAttributes(XPathNodeIterator inIterator)
      {
         bool didSetAttributes = true;
         ResidentAttributes tempRA = new ResidentAttributes();
         try
         {

            XPathNodeIterator temp = inIterator.Current.Select("//txtResDieBetweenSeason");
            temp.MoveNext();
            tempRA.ResidentYearlyRisk = System.Convert.ToDouble(System.Convert.ToDouble(temp.Current.Value));
            temp = inIterator.Current.Select("//txtResDieTimeStep");
            temp.MoveNext();
            tempRA.ResidentTimeStepRisk = System.Convert.ToDouble(temp.Current.Value);
            temp = inIterator.Current.Select("//txtResBreedPercent");
            temp.MoveNext();
            tempRA.PercentBreed = System.Convert.ToDouble(temp.Current.Value);
            temp = inIterator.Current.Select("//txtResFemalePercent");
            temp.MoveNext();
            tempRA.PercentFemale = System.Convert.ToDouble(temp.Current.Value);
            temp = inIterator.Current.Select("//txtResOffspringMean");
            temp.MoveNext();
            tempRA.NumChildernMean = System.Convert.ToDouble(temp.Current.Value);
            temp = inIterator.Current.Select("//txtResOffspringSD");
            temp.MoveNext();
            tempRA.NumChildernSD = System.Convert.ToDouble(temp.Current.Value);

            this.AnimalManager.setResidentModifierValues(tempRA);
         }
         catch (Exception ex)
         {
            didSetAttributes = false;
          
         }
         return didSetAttributes;
      }

      #endregion

      #region getters and setters

      internal HourlyModifierCollection GetHourlyModifierCollection()
      {
         fw.writeLine("inside sim manager getting a collecton of Hourly modifiers");
         HourlyModifierCollection myHourlyModifiers = HourlyModifierCollection.GetUniqueInstance();
         fw.writeLine("done getting the collection current Count is " + myHourlyModifiers.Count.ToString());
         return myHourlyModifiers;
      }
   
      internal DailyModiferCollection GetDailyModiferCollection()
      {
         DailyModiferCollection mySingleton = DailyModiferCollection.GetUniqueInstance();
         return mySingleton;
      }
      public MapManager MapManager
      {
         get { return mMapManager; }
         set { mMapManager = value; }
      }
      public string ErrMessage
      {
         get { return mErrMessage;}
         set  { mErrMessage = value; }
      }

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

      public string TextOutPutFileName
      {
         get { return mTextOutPutFileName; }
         set { mTextOutPutFileName = value; }
      }

      

      public string MapOutPutPath
      {
         get { return mMapOutPutPath; }
         set { mMapOutPutPath = value; }
      }

      public DateTime EndSimulatonDate
		{
			get { return mEndSimulatonDate; }
			set { mEndSimulatonDate = value; }
		}

      public DateTime StartSimulationDate
		{
			get { return mStartSimulationDate; }
			set  { mStartSimulationDate = value; }
		}

      public DateTime EndSeasonDate
      {
         get { return mEndSeasonDate; }
         set { mEndSeasonDate = value; }
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

      public double ElapsedTimeBetweenTimeStep
      {
         get { return mElapsedTimeBetweenTimeStep; }
         set  { mElapsedTimeBetweenTimeStep = value; }
      }


      public DateTime StartSeasonDate
      {
         get { return mStartSeasonDate; }
         set { mStartSeasonDate = value; }
      }


      #endregion
   }
}
