/******************************************************************************
 * CHANGE LOG
 *    
 *    Author:        Author: Bob Cummings
 *    Date:          Friday, October 05, 2007 6:34:57 AM
 *    Description:   Moved checking for time to look for home call in 
 *                   public virtual void doTimeStep to only occur while 
 *                   the animal is active.
 *    Side Effect:   Will change the number of time steps from N hours from
 *                   start of season to N hours individual is active.
 * 
 * ****************************************************************************
 *    Author:        Bob Cummings
 *    Date:          Thursday, October 11, 2007 7:04:26 PM
 *    Description:   Added several temporal variables.  We were modify the wrong
 *                   variables.  So now we have variables that are from the map, and 
 *                   modifiers that are temporial based.  Then we make a new temp variable
 *                   from both values.  This allowed us to not have to update values
 *                   from the maps unless George moved out his current polygon. 
 ******************************************************************************
 *    Author:        Bob Cummings
 *    Date:          Saturday, February 16, 2008 11:35:14 AM
 *    Description:   Changed createText output to show current values.
 *****************************************************************************
 *    Author:        Bob Cummings
 *    Date:          Thursday, February 21, 2008 3:28:53 PM
 *    Description:   Moved the do text output after the move event.
 *****************************************************************************/

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using System.IO;
using System;
using DesignByContract;
namespace PAZ_Dispersal
{
   public  class Animal
   {
      #region privateMemberVariables
      
      //Map based values that adjust how George acts
      private double mCaptureFood;
      private double mFoodMeanSize;
      private double mFoodSD_Size;
      private double mPredationRisk;
      private double mMoveSpeed;
      private double mMoveTurtosity;
      private double mEnergyUsed;
      private double mPerceptonModifier;
      private double mHeading;
     
      //information about George at this current timestep
      private bool hadCloseCall;
      private bool goingHome; 
      private PointClass myLocation;
      private int mMoveIndex;    //index of the polygon on the move map
      private int mRiskIndex;       //index of the polygon on the risk index
      private int mFoodIndex;       //index of the polygon on the food index
      private int mSocialIndex;    // index of the polygon on the social index
      private int myIdNum;
      private double mCurrEnergy;

      //Added Thursday, October 11, 2007
      private double mTemporalChanceOfEating;
      private double mTemporalRiskValue;
      private double mTemporalMoveSpeed;
      private double mTemporalMoveTurtosity;
      private double mTemporalEnergyUsed;
      private double mTemporalPerceptonDistance;
      // end addition on Thursday, October 11, 2007

      protected bool mIsDead;
      private bool IsAsleep;
      private DateTime WakeupTime;
      private DateTime SleepTime;
      private int durationID;       // because all the animals will sleep or be active according
      // to their own internal time clock we need to keep track of
      // where they are in the duration cycle
      

      //used in calculating georges home area if he finds one
      private double mHomeRangeArea;
      private double mDistanceMean; //used to calculate the home range
      private double mDistanceSE;   //used to calculate the home range
      private double mDistanceWeight; //used when calculating where to set up home range
      private IHomeRangeTrigger mHomeRangeTrigger;
      private PointClass mHomeRangeCenter;
      private IHomeRangeFinder mHomeRangeFinder;

      //used to specific modifiers depending on sex,behavior mode and other stuff
      private AnimalAtributes mAnimalAtributes;
      private Modifier mGenderModifier;
      private Modifier mStateModifer;
      private MapManager mMapManager; 
      protected string sex;
      private AnimalManager mAnimalManager;

      //moves george around the board
      private Mover mMover; 
      private IPointList mPath;
      private EligibleHomeSites mMySites;

      //logs out debugging information
      protected FileWriter.FileWriter fw;
      private int timeStep;
      private string fileNamePrefix;
      
      protected TextFileWriter mTextFileWriter;
      
      //helps george roll the dice
      protected RandomNumbers rn;
           
      #endregion

      #region privateMethods

      private void buildLogger()
      {
         string s;
         StreamReader sr; 
         bool foundPath = false;
         
         string st = System.Windows.Forms.Application.StartupPath;
         if(File.Exists( st + @"\logFile.dat"))
         {
            sr= new StreamReader( st + @"\logFile.dat");
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
      }//end of buildLogger
      private void buildFileNamePrefix(string year)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();
         try
         {
            Check.Require(sex != null,"No sex for the animal to build the file name prefix");
            sb.Append(this.IdNum.ToString());
            //pad out to 5 long
            while (sb.Length < 6)
               sb.Insert(0,"0");
        
            sb.Insert(0,sex);
            sb.Insert(0,year);
            this.fileNamePrefix = sb.ToString();

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         
         
      }
      private void calcSleepTime()
      {
         Check.Require(this.IsAsleep == false,"Not asleep why get sleep time");
         double numHours = 0;
         double mean=0;
         double sd=0;
         fw.writeLine("inside calc sleep getting the number of hours before I go to sleep");
         fw.writeLine("get the mean and sd for this rest time");
         this.AnimalAtributes.getDurationMeanAndSD(ref mean,ref sd,ref durationID);
         numHours =Math.Abs(rn.getNormalRandomNum(mean,sd));
         fw.writeLine("I will be active for " +  numHours.ToString());
         this.SleepTime = this.WakeupTime.AddHours(numHours);
         fw.writeLine("so I will go to sleep at " + this.SleepTime.ToShortDateString() + " " + this.SleepTime.ToShortTimeString());
      }//end of calcSleepTime
      
      private void calcWakeTime()
      {
         Check.Require(this.IsAsleep == true,"George is awake why get bedtime?");
         double numHours = 0;
         double mean=0;
         double sd=0;
         fw.writeLine("inside calc sleep getting the number of hours before I go to wake up");
         fw.writeLine("get the mean and sd for this rest time");
         this.AnimalAtributes.getDurationMeanAndSD(ref mean,ref sd,ref this.durationID);
         numHours = Math.Abs(rn.getNormalRandomNum(mean,sd));
         fw.writeLine("I will be active for " +  numHours.ToString());
         this.WakeupTime = this.SleepTime.AddHours(numHours);
         fw.writeLine("so I will wake up at " + this.WakeupTime.ToShortDateString() + " " + this.WakeupTime.ToShortTimeString());

      }//end of calcWakeTime
      private void changeActiveState(DateTime currTime)
      {
         fw.writeLine("inside change active state");
         if (this.IsAsleep == true)
         {
            fw.writeLine("must be asleep");
            fw.writeLine("Waketime is " + this.WakeupTime.Date.ToShortDateString() + " "  + this.WakeupTime.TimeOfDay.ToString());

            fw.writeLine("curr time is " + currTime.Date.ToShortDateString() + " " + currTime.TimeOfDay.ToString());
            if (this.WakeupTime.Date <= currTime.Date && this.WakeupTime.TimeOfDay <= currTime.TimeOfDay)
            {  
               this.IsAsleep = false;
               fw.writeLine("time to wake up");
               this.myMover = RandomWCMover.getRandomWCMover();
               calcSleepTime();
            }
         }
         else
         {
            fw.writeLine("must be awake");
            fw.writeLine("Sleep time is " + this.SleepTime.Date + " " +this.SleepTime.TimeOfDay.ToString());
            fw.writeLine("curr time is " + currTime.Date.ToShortDateString() + " " + currTime.TimeOfDay.ToString());
            if (this.SleepTime.Date <= currTime.Date && this.SleepTime.TimeOfDay <= currTime.TimeOfDay)
            {
               this.IsAsleep = true;
               this.myMover = SleepMover.getSleepMover();
               fw.writeLine("time to go to sleep");
               calcWakeTime();
            }
         }
      }//end of changeActiveState
      private string createTextOutput(DateTime currTime,double inPercent, double inX,double inY)
      {
         string outPut;

         outPut = currTime.Year.ToString() + "," +
            currTime.ToShortDateString() + "," +
            currTime.ToShortTimeString() + "," +
            this.IdNum.ToString() + "," +
            inX.ToString() + "," +
            inY.ToString() + "," +
            this.IsAsleep.ToString() + "," +
            this.StateModifer.Name + "," +
            this.mCurrEnergy.ToString() + "," +
            //BC Saturday, February 16, 2008
            this.mTemporalRiskValue.ToString() + "," +
            (this.mTemporalChanceOfEating * this.mCaptureFood * inPercent).ToString() + "," +
            this.mTemporalMoveTurtosity.ToString() + "," +
            this.mTemporalMoveSpeed.ToString()+ "," +
            inPercent.ToString();
//         this.mCurrEnergy.ToString() + "," +
//            this.mPredationRisk.ToString() + "," +
//            this.mCaptureFood.ToString() + "," +
//            this.mMoveTurtosity.ToString() + "," +
//            this.mMoveSpeed.ToString();



         return outPut;
      }// end of createTextOutput
      private void die(double percentTimeStep)
      {
         double tempRisk = 0.0;
         double rollOfTheDice = 0.0;

         rollOfTheDice = rn.getUniformRandomNum();
         fw.writeLine("inside die with roll of " + rollOfTheDice.ToString());
         fw.writeLine("going to adust the chance of getting eaten current chance is " + mPredationRisk.ToString() );
         tempRisk = this.GenderModifier.PredationRisk *  percentTimeStep * mPredationRisk;
         fw.writeLine("new chance of getting ambushed is " + tempRisk.ToString());
         if (tempRisk > rollOfTheDice)
         {
            fw.writeLine("george dies");
            this.mIsDead = true;
            mTextFileWriter.addLine("George number " + this.IdNum.ToString() + " died from predation");
         }
         else if( mCurrEnergy < this.AnimalAtributes.MinEnergy_Survive)
         {
            fw.writeLine("george dies");
            this.mIsDead = true;
            mTextFileWriter.addLine("George number " + this.IdNum.ToString() + " died from starvation");
         }
         if ((tempRisk - rollOfTheDice) < this.AnimalAtributes.SafeRiskyTrigger) //george nearly died, so he feels like he had a close call
         {
            this.hadCloseCall = true;
         }
         else if ((tempRisk - rollOfTheDice) > this.AnimalAtributes.RiskySafeTrigger) //predator wasn't even close, so george no longer feels like he had a close call
         {
            this.hadCloseCall = false;
         }
      }//end of die
	
     
      private void eat(double percentTimeStep)
      {
         double chanceOfEating = 0;
         double foodAmt = 0;
         double rollOfTheDice = rn.getUniformRandomNum();
         fw.writeLine("inside eat for animail number " + this.IdNum.ToString() + " current energy level  is " + mCurrEnergy.ToString());
         fw.writeLine("the roll of the dice is " + rollOfTheDice.ToString());
         
         fw.writeLine("my food index is " + mFoodIndex.ToString());
         fw.writeLine("chance of eating from the map is " + this.mCaptureFood.ToString());
         chanceOfEating = this.mCaptureFood * percentTimeStep * mTemporalChanceOfEating;
         fw.writeLine("I had this good a chance to get food " + chanceOfEating.ToString() + " this timestep");
         if (rollOfTheDice < chanceOfEating)
         {
            foodAmt = System.Math.Abs(rn.getNormalRandomNum(this.mFoodMeanSize,this.mFoodSD_Size));
            fw.writeLine("so I get this much energy " + foodAmt.ToString());
            this.mCurrEnergy = this.mCurrEnergy + foodAmt;
            if (this.mCurrEnergy > this.AnimalAtributes.MaxEnergy)
               this.mCurrEnergy = this.AnimalAtributes.MaxEnergy;
         }
         fw.writeLine("so current energy level is " + this.mCurrEnergy.ToString());
      }//end of eat
	
      private bool findHome()
      {
         IFeatureClass fc = null;
         bool success = false;
         try
         {
            fw.writeLine("inside find home calling to get the time step map");
            fc = this.MapManager.getTimeStepMap(this.IdNum.ToString());
            if (fc != null)
            {
               fw.writeLine("now setting the home range center");
               success=this.HomeRangeFinder.setHomeRangeCenter(this,fc);
               fw.writeLine("that returned " + success.ToString());
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
      
      private bool isSuitable(int polyRef)
      {  
         bool suitable = false;
         return suitable;


      }
      private void move(ref double percentTimeStep)
      {
         fw.writeLine("inside move calling myMover Move passing myself as well");
         myMover.move(ref percentTimeStep,this);
      }
      private void loseEnergy(double percentTimeStep)
      {
         double lostEnergy = 0;
         lostEnergy = percentTimeStep * mTemporalEnergyUsed;  fw.writeLine("I used " + mEnergyUsed.ToString() + " this timestep");
         fw.writeLine("inside lose energy current energy level  is " + mCurrEnergy.ToString());
         this.mCurrEnergy = this.mCurrEnergy - lostEnergy;  fw.writeLine("so current energy level is " + this.mCurrEnergy.ToString());
      }
      private void setInitialLocaton()
      {
         fw.writeLine("inside set initial location x = " + myLocation.X.ToString() + " y " + myLocation.Y.ToString());

      }
      private void setInitialSleepTime(DateTime currTime)
      {
         if (this.IdNum == 0 )
            fw.writeLine("inside set inital sleeptime curr time is " + currTime.ToShortDateString() + "  " + currTime.ToShortTimeString());
         this.WakeupTime = currTime;
         this.calcSleepTime();
         if (this.IdNum == 0 )
         {
            //fw.writeLine("leaving set inital sleeptime waketime is " + this.WakeupTime.ToShortDateString() + "  " + this.WakeupTime.ToShortTimeString());
            //fw.writeLine("leaving set inital sleeptime sleeptime is " + this.SleepTime.ToShortDateString() + "  " + this.SleepTime.ToShortTimeString());
         }
      }
      private void setSocialIndex(IPoint currLoc)
      {
         this.mMapManager.getSocialIndex(currLoc,ref mSocialIndex);
      }
      private void setSocialIndex()
      {
         fw.writeLine("inside setting social index old one was " +mSocialIndex.ToString());
         fw.writeLine("my location is x = " + this.myLocation.X.ToString() + " Y = " + this.myLocation.Y.ToString());
         this.mMapManager.getSocialIndex(this.Location,ref mSocialIndex);
         fw.writeLine("new social index is " + this.mSocialIndex.ToString());
      }
      private void upDateTemporalModifiers(HourlyModifier inHM, DailyModifier inDM)
      {
         try
         {
            //Author: Bob Cummings added the temporal values.  we were modify too much.
            mTemporalChanceOfEating = this.GenderModifier.CaptureFood * inHM.CaptureFood * inDM.CaptureFood * this.StateModifer.CaptureFood;      
            mTemporalRiskValue =  this.mPredationRisk * this.GenderModifier.PredationRisk * inHM.PredationRisk * inDM.PredationRisk * this.StateModifer.PredationRisk;    
            mTemporalMoveSpeed = this.mMoveSpeed * this.GenderModifier.MoveSpeed * inHM.MoveSpeed * inDM.MoveSpeed * this.StateModifer.MoveSpeed;        
            mTemporalMoveTurtosity = mMoveTurtosity * this.GenderModifier.MoveTurtosity * inHM.MoveTurtosity * inDM.MoveTurtosity * this.StateModifer.MoveTurtosity;    
            mTemporalEnergyUsed = mEnergyUsed * this.GenderModifier.EnergyUsed * inHM.EnergyUsed * inDM.EnergyUsed * this.StateModifer.EnergyUsed;       
            mTemporalPerceptonDistance = mPerceptonModifier * this.GenderModifier.PerceptonModifier * inHM.PerceptonModifier * inDM.PerceptonModifier * this.StateModifer.PerceptonModifier * this.AnimalAtributes.PerceptionDistance;
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
        
      }
     

      private void updateMyLocationModifiers()
      {
         fw.writeLine("inside updateMyLocationModifiers");
         fw.writeLine("calling set social index");
         this.setSocialIndex();
         this.mMapManager.GetMoveModifiers(this.myLocation,ref mMoveIndex,ref mMoveTurtosity,ref mMoveSpeed,ref mPerceptonModifier,ref mEnergyUsed);
         this.mMapManager.GetRiskModifier(this.myLocation,ref this.mRiskIndex, ref mPredationRisk);
         //Author: Bob Cummings moved down here from eat() made more sense to do it all in one place
         this.mMapManager.GetFoodData(this.myLocation,ref this.mFoodIndex,ref this.mCaptureFood, ref this.mFoodMeanSize, ref this.mFoodSD_Size);
      }

      private void updateBehavioralModifiers()
      {
         if (this.hadCloseCall) //animal feels threatened, so will act more safely
         {
            fw.writeLine("inside updateBehavioralModifiers, hadCloseCall = " + hadCloseCall.ToString());
            fw.writeLine("Energy level is: " + CurrEnergy.ToString() + " ForageSearchTrigger is: " +this.AnimalAtributes.ForageSearchTrigger.ToString() );
            if (this.CurrEnergy < this.AnimalAtributes.ForageSearchTrigger)//animal is hungry, needs to forage
            {
               fw.writeLine("setting stateModifier to a new SafeForageModifier");
               this.StateModifer = this.AnimalManager.SafeForageMod;
            }
            else //animal isn't hungry, so is looking for a home
            {
               fw.writeLine("setting stateModifier to a new SafeSearchModifier");
               this.StateModifer = this.mAnimalManager.SafeSearchMod;
            }
         }
         else  //animal feels safe, so will act more riskily
         {
            fw.writeLine("inside updateBehavioralModifiers, hadCloseCall = " + hadCloseCall.ToString());
            if (this.CurrEnergy < this.AnimalAtributes.ForageSearchTrigger)//animal is hungry, needs to forage
            {
               fw.writeLine("setting stateModifier to a new RiskyForageModifier");
               this.StateModifer = this.mAnimalManager.RiskyForageMod;
            }
            else  //animal isn't hungry, so is looking for a home
            {
               fw.writeLine("setting stateModifier to a new RiskySearchModifier");
               this.StateModifer = this.mAnimalManager.RiskySearchMod;
            }
         }
      }//end of updateBehavioralModifiers
      #endregion

      #region publicMethods
	
      public Animal()
      {
         this.buildLogger();
         this.MapManager = MapManager.GetUniqueInstance();
         this.rn = RandomNumbers.getInstance();
         this.Heading = rn.getUniformRandomNum() * 2 * Math.PI;
         this.mPath = new IPointList();
         this.mMySites = new EligibleHomeSites();
         this.mIsDead = false;
         this.IsAsleep = false;
         this.timeStep = -1;
         this.durationID=0;
         this.hadCloseCall = false;
         this.goingHome = false;
        
      }
     


      public virtual void doTimeStep(HourlyModifier inHM, DailyModifier inDM,DateTime currTime,bool doTextOutput,ref string status)
      {
         double percentTimeStep = 0;
         double tempPercentTimeStep = 0;
         //added temp for olding the old locations for doing text output
		   double tempX = 0;
		   double tempY = 0;
         bool timeToLookForHome = false;
        
        
         try
         {
           
            this.timeStep++;
            #region 
            fw.writeLine("inside Animal Do Time Step for " + myIdNum.ToString() +
               " the time is " + currTime.ToShortDateString() + " " + currTime.ToShortTimeString());
            fw.writeLine("percent time step is " + percentTimeStep.ToString());
            fw.writeLine("is dead = " + this.mIsDead.ToString()); 
            fw.writeLine("do text output is " + doTextOutput.ToString() );
            #endregion
            while (percentTimeStep < 1 && !this.mIsDead)
            {
               //store off the original location
				   tempX = this.myLocation.X;
               tempY = this.myLocation.Y;
               fw.writeLine("calling update behavioral modifier");
               updateMyLocationModifiers(); fw.writeLine("back from update location modifier now update behavioral modifiers");
               updateBehavioralModifiers(); fw.writeLine("back from update behavioral modifier now update my temporal modifiers");
               upDateTemporalModifiers(inHM, inDM); fw.writeLine("done updating temporal modifier now start loop for moving");
               fw.writeLine("mPredationRisk is " + mPredationRisk.ToString());
               
               move(ref percentTimeStep);  fw.writeLine("move returned  " + percentTimeStep.ToString());
               
               //Thursday, February 21, 2008 Moved after move to capture the percent of the step taken
               if (doTextOutput)
                  mTextFileWriter.addLine(this.createTextOutput(currTime,percentTimeStep,tempX,tempY));

               //Saturday, September 22, 2007 10:03:58 AM Author: Bob Cummings
               //Added tempPercentTimeStep for accurate percentage on second and subsequent moves in same timestep
               //so temp should equal 1 - current percent - last times current percent to get this times current percent
               if(percentTimeStep < 1)
                  tempPercentTimeStep = 1 - percentTimeStep - tempPercentTimeStep;
               else
                  tempPercentTimeStep = 1;
               
               
               eat(tempPercentTimeStep);
               loseEnergy(tempPercentTimeStep);
               changeActiveState(currTime);
               
               die(tempPercentTimeStep);
               changeActiveState(currTime);
              
            }
            if(this.IsAsleep == false)
            {
               fw.writeLine("adding a new eligible home site to the collection mPredationRisk is " + mPredationRisk.ToString());
               this.mMySites.addSite(new EligibleHomeSite(this.mCaptureFood,this.mPredationRisk,this.myLocation.X,this.myLocation.Y),ref fw);
               fw.writeLine("ok now see if it is time to look for a home yet");
               //Author: Bob Cummings Friday, October 05, 2007
               //moved this to inside if statement to stop looking for a home 
               //while asleep.  The will also change the number of time steps to only include
               //active time steps.
               timeToLookForHome = this.mHomeRangeTrigger.timeToLookForHome(this);
            }
          
            
            if (timeToLookForHome && !this.goingHome)
            {
               // System.Windows.Forms.MessageBox.Show("Time to look for home " + this.IdNum.ToString());
               
               fw.writeLine("yes it is time to look for a home calling find home");
               if( findHome())
               {
                  this.mTextFileWriter.addLine("I have found a home and I am going to it");
                  fw.writeLine("back in doTimeStep and we found a home to set the mover to directed mover");
                  fw.writeLine("and set the boolean variable goingHome = true");
                  this.mMover = new DirectedMover();
                  this.goingHome = true;
               }//if we do not find a home we keep wandering around and check again.
            }
            if (this.goingHome)
            {
               double distToHome;
               fw.writeLine("ok we are headed home now check the distace between here and there");
               distToHome = this.MapManager.getDistance(this.Location,this.HomeRangeCenter);
               fw.writeLine("we still have to go " + distToHome.ToString());
               //TODO take out hard coded value of 100 meters and change to base perception distance
               if(distToHome < 100)
               {
                  fw.writeLine("ok we are home now setting the location = home range center");
                  this.Location = this.HomeRangeCenter;
                  fw.writeLine("now building the home range");
                  this.MapManager.makeHomeRange(this);
                  status = "resident";
                  if (doTextOutput)
                     this.mTextFileWriter.addLine("Found Home at " + this.myLocation.X.ToString() + " and " + this.myLocation.Y.ToString());
                  // System.Windows.Forms.MessageBox.Show(this.IdNum.ToString() + " found a home kill the application and look at at the input social map ");
               }
            }
         }
         
         catch(System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }// end of doTimeStep
      public IPoint getLocation(int stepNum)
      {
         return this.mPath.getPointByIndex(stepNum);
      }
      public int getNumStepsInPath()
      {
         return this.mPath.Count();
      }
      public bool isSiteGood(ref FileWriter.FileWriter inFw)
      {
         
         bool largeEnough = false;
         bool isAvailable = false;
         
         try
         {
            inFw.writeLine("inside is site good for animal " + this.IdNum.ToString());
            inFw.writeLine("checking against social index number " + this.mSocialIndex.ToString());
            largeEnough = this.MapManager.getArea(this.mSocialIndex) > this.HomeRangeArea;
            //if it is occupied it is not available
            isAvailable = !this.MapManager.isOccupied(this.mSocialIndex,this.sex, ref inFw);
            inFw.writeLine("large enough returned " + largeEnough.ToString());
            inFw.writeLine("is available returned " + isAvailable.ToString());

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return largeEnough && isAvailable;
      }
      public void setInitialValues(DateTime currTime)
      {
         try
         { 
            setInitialSleepTime(currTime);
            if (this.mMoveIndex < 0)
               setInitialLocaton();
            this.mPerceptonModifier = this.AnimalAtributes.PerceptionDistance;
            this.buildFileNamePrefix(currTime.Year.ToString());
            this.TextFileWriter = new TextFileWriter(this.AnimalAtributes.OutPutDir,this.fileNamePrefix);
            this.setSocialIndex(this.Location);
            this.mMapManager.GetInitialMapData(this);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
        
      }

      public void updateMemory()
      {
         try
         {
            //get values
            int n = this.mPath.getLastIndex();
            IPoint start = this.mPath[n - 1];
            IPoint end = this.mPath[n];
            double angleMoved = 0;
            double angleLeft = 0;
            double angleRight = 0;
            IPoint startLeft= new PointClass();
            IPoint startRight= new PointClass();
            IPoint endLeft= new PointClass();
            IPoint endRight= new PointClass();
            IGeometry g;
            ITopologicalOperator to;
            CircularArcClass startCurve = new CircularArcClass();
            CircularArcClass endCurve = new CircularArcClass();
            LineClass leftLine = new LineClass();
            LineClass rightLine = new LineClass();
            LineClass endLine = new LineClass();
            LineClass startLine = new LineClass();
            PolygonClass memoryPolygon = new PolygonClass();
            object Missing = Type.Missing;

            //get angle from start to end
            angleMoved = System.Math.Atan((start.Y - end.Y)/(start.X-end.X));
            //rotate pi/2
            angleLeft = angleMoved + Math.PI/2;
            angleRight = angleMoved - Math.PI/2;
			
            //step perceptionDistance forward and back
            startLeft.X = start.X+ mTemporalPerceptonDistance * System.Math.Cos(angleLeft);
            startLeft.Y = start.Y+ mTemporalPerceptonDistance * System.Math.Sin(angleMoved + angleLeft);

            startRight.X = start.X+ mTemporalPerceptonDistance * System.Math.Cos(angleRight);
            startRight.Y = start.Y+ mTemporalPerceptonDistance * System.Math.Sin(angleRight);
			
            //repeat for end
            endLeft.X = end.X+ mTemporalPerceptonDistance * System.Math.Cos(angleLeft);
            endLeft.Y = end.Y+ mTemporalPerceptonDistance * System.Math.Sin(angleLeft);

            endRight.X = end.X+ mTemporalPerceptonDistance * System.Math.Cos(angleRight);
            endRight.Y = end.Y+ mTemporalPerceptonDistance * System.Math.Sin(angleRight);
            leftLine.FromPoint = startLeft;
            leftLine.ToPoint = endLeft;

            rightLine.FromPoint = startRight;
            rightLine.ToPoint = endRight;

            endLine.FromPoint = endRight;
            endLine.ToPoint = startLeft;

            startLine.FromPoint = endLeft;
            startLine.ToPoint = startLeft;

            memoryPolygon.SpatialReference = this.Location.SpatialReference;
            memoryPolygon.AddSegment((ISegment)leftLine, ref Missing,  ref Missing);
            memoryPolygon.AddSegment((ISegment)rightLine, ref Missing,  ref Missing);
            memoryPolygon.AddSegment((ISegment)endLine, ref Missing,  ref Missing);
            memoryPolygon.AddSegment((ISegment)startLine, ref Missing,  ref Missing);
           
            to = (ITopologicalOperator)end;
            g = to.Buffer(mTemporalPerceptonDistance);
            fw.writeLine("adding the step for time step " + this.timeStep.ToString());
            //  this.MapManager.AddMemoryPoly(this.IdNum,memoryPolygon);
            this.MapManager.AddTimeSteps(this.IdNum,memoryPolygon,(IPolygon)g,timeStep,sex);
            if(goingHome)
            {
               this.setSocialIndex(end);
            }
         }

        
         catch(System.Exception ex)
         {
            fw.writeLine("error look for error file");
            System.Windows.Forms.MessageBox.Show(ex.StackTrace);
            
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
        
      }//end of updateMemory
      /********************************************************************************
       *   Function name   : dump
       *   Description     : 
       *   Return type     : virtual void 
       * ********************************************************************************/
         
      public virtual void dump()
      {
         try
         {
            fw.writeLine("");
            fw.writeLine("inside animal dump");
            fw.writeLine("My ID number is " + this.IdNum.ToString());
            fw.writeLine("my X Location is " + this.Location.X.ToString() + " my y location is " + this.Location.Y.ToString());
            fw.writeLine("My prob of capturing food is " + this.mCaptureFood.ToString());
            fw.writeLine("My prob of getting killed is " + this.mPredationRisk.ToString());
            fw.writeLine("My movement speed is " + this.mMoveSpeed.ToString());
            fw.writeLine("My move tortuosity is " + this.mMoveTurtosity.ToString());
            fw.writeLine("My current use of food is " + this.mEnergyUsed.ToString());
            fw.writeLine("My perception range is " + this.mPerceptonModifier.ToString());
            fw.writeLine("leaving animal dump");
            fw.writeLine("");
         }
         catch(System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }
      

      
      #endregion

      #region gettersAndSetters
      public AnimalManager AnimalManager
      {
         get { return mAnimalManager; }
         set  { mAnimalManager = value; }
      }

      public bool IsDead
      {
         get { return mIsDead; }
         set { mIsDead = value; }
      }

      public  TextFileWriter TextFileWriter
      {
         get { return mTextFileWriter; }
         set { mTextFileWriter = value; }
      }

      public double CurrEnergy
      {
         get { return mCurrEnergy; }
         set { mCurrEnergy = value; }
      }

      public double Heading
      {
         get { return mHeading; }
         set  { mHeading = value; }
      }

      public MapManager MapManager
      {
         get { return mMapManager; }
         set { mMapManager = value; }
      }

      public AnimalAtributes AnimalAtributes
      {
         get { return mAnimalAtributes; }
         set { mAnimalAtributes = value; }
               
      }

     
      public double HomeRangeArea
      {
         get { return mHomeRangeArea; }
         set { mHomeRangeArea = value; }
      }

      public double DistanceMean
      {
         get { return mDistanceMean; }
         set { mDistanceMean = value; }
      }

      public double DistanceSE
      {
         get { return mDistanceSE; }
         set { mDistanceSE = value; }
      }

      public double DistanceWeight
      {
         get { return mDistanceWeight; }
         set  { mDistanceWeight = value; }
      }

      public Modifier StateModifer
      {
         get { return mStateModifer; }
         set { mStateModifer = value; }
      }



      public int MoveIndex
		{
			get { return mMoveIndex; }
			set  { mMoveIndex = value; }
		}
   
      public int RiskIndex
		{
			get { return mRiskIndex; }
			set  { mRiskIndex = value; }
		}
       
      public int FoodIndex
		{
			get { return mFoodIndex; }
			set  { mFoodIndex = value; }
		}
      
      public int SocialIndex
		{
			get { return mSocialIndex; }
			set  { mSocialIndex = value; }
		}
    

      public Modifier GenderModifier
      {
         get { return mGenderModifier; }
         set { mGenderModifier = value; }
      }

      public Mover myMover
      {
         get{ return mMover;}
         set{mMover = value;}         
      }//end of myMover property

      public int IdNum
      {
         get { return myIdNum; }
         set { myIdNum = value; }
      }

      public PointClass Location
      {
         get { return myLocation; }
         set 
         {
            myLocation = value;
            this.mPath.add(myLocation);
            // fw.writeLine("current polygon is " + this.mPolygonIndex.ToString());
            // this.mPolygonIndex = this.MapManager.getCurrPolygon(myLocation);   
            // fw.writeLine("new polygon is " + this.mPolygonIndex.ToString());
         }
      }
     
      public double CaptureFood
      {
         get { return mCaptureFood; }
         set { mCaptureFood = value; }
      }

      public double FoodMeanSize
		{
			get { return mFoodMeanSize; }
			set  { mFoodMeanSize = value; }
		}

      public double FoodSD_Size
		{
			get { return mFoodSD_Size; }
			set  { mFoodSD_Size = value; }
		}

      public double PredationRisk
      {
         get { return mPredationRisk; }
         set { mPredationRisk = value; }
      }

      public double MoveSpeed
      {
         get { return mTemporalMoveSpeed; }
         set { mTemporalMoveSpeed = value; }
      }

      public double MoveTurtosity
      {
         get { return mTemporalMoveTurtosity; }
         set { mTemporalMoveTurtosity = value; }
      }

      public double EnergyUsed
      {
         get { return mEnergyUsed; }
         set { mEnergyUsed = value; }
      }

      public double PerceptonModifier
      {
         get { return mPerceptonModifier; }
         set { mPerceptonModifier = value; }
      }
      public PointClass HomeRangeCenter
      {
         get { return mHomeRangeCenter; }
         set { mHomeRangeCenter = value; }
      }
      public IHomeRangeFinder HomeRangeFinder
      {
         get { return mHomeRangeFinder; }
         set  { mHomeRangeFinder = value; }
      }

      public IHomeRangeTrigger HomeRangeTrigger
      {
         get { return mHomeRangeTrigger; }
         set { mHomeRangeTrigger = value; }
      }

      public string Sex
      {
         get{return sex;}
         set{sex = value;}
      }
      public string FileNamePrefix
      {
         get {return fileNamePrefix;}
      }
      public EligibleHomeSites MySites
      {
         get { return mMySites; }
         set  { mMySites = value; }
      }


      #endregion
	  
   }
}
