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
 *****************************************************************************
 *    Author:        Bob Cummings
 *    Date:          Monday, June 09, 2008 3:20:12 PM
 *    Description:   Commented out the temp time stuff in doTimeStep
 * ****************************************************************************
*    Author:        Bob Cummings
*    Date:          Tuesday, July 15, 2008 6:30:27 AM
*    Description:   Modified use energy to use mEnergy instead of mTemporalEnergy
 * ****************************************************************************
*    Author:        Bob Cummings
*    Date:          Tuesday, November 24, 2009
*    Description:   Changed the way the we are checking for if it is time to
 *                   change activity state.
* ****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using DesignByContract;
using ESRI.ArcGIS.Geometry;
using log4net;

namespace SEARCH
{
   [Serializable()]
   [XmlInclude(typeof(Female))]
   [XmlInclude(typeof(Male))]
   [XmlInclude(typeof(Resident))]
   [XmlInclude(typeof(DeadAnimal))]
   [XmlInclude(typeof(TimeHomeRangeTrigger))]
   [XmlInclude(typeof(SiteHomeRangeTrigger))]
   [XmlInclude(typeof(BestComboHomeRangeFinder))]
   [XmlInclude(typeof(BestFoodHomeRangeFinder))]
   [XmlInclude(typeof(BestRiskHomeRangeFinder))]
   [XmlInclude(typeof(ClosestHomeRangeFinder))]
   [XmlInclude(typeof(HomeRangeFinder))]
   [XmlInclude(typeof(EligibleHomeSites))]
   public class Animal
   {
      #region Constructors (1)

      //end of updateBehavioralModifiers
      public Animal()
      {
         this.MapManager = MapManager.GetUniqueInstance();
         this.rn = RandomNumbers.getInstance();
         this.Heading = rn.getUniformRandomNum() * 2 * Math.PI;
         this.mPath = new IPointList();
         this.mMyVisitedSites = new EligibleHomeSites();
         this.mIsDead = false;
         this.IsAsleep = false;
         this.timeStep = -1;
         this.durationID = 0;
         this.hadCloseCall = false;
         this.goingHome = false;
         this.foundHome = false;
      }

      #endregion Constructors

      #region Fields (48)

      private double mCaptureFood;
      private double mFoodMeanSize;
      private double mFoodSD_Size;
      private double mPredationRisk;
      private double mMoveSpeed;
      private double mMoveTurtosity;
      private double mEnergyUsed;
      private double mPerceptonModifier;
      private double mHeading;
      private double mPerceptionDist;
      private bool hadCloseCall;
      private bool goingHome;
      private bool foundHome;
      private IPoint myLocation;
      private int mMoveIndex;
      private int mRiskIndex;
      private int mFoodIndex;
      private int mSocialIndex;
      private int myIdNum;
      private string myIdNumOrig;
      private double mCurrEnergy;
      private double mTemporalChanceOfEating;
      private double mTemporalRiskValue;
      private double mTemporalMoveSpeed;
      private double mTemporalMoveTurtosity;
      private double mTemporalEnergyUsed;
      private double mTemporalPerceptonDistance;
      private bool mIsDead;

      protected bool MIsDead
      {
         get { return mIsDead; }
         set { mIsDead = value; }
      }
      private bool IsAsleep;
      private DateTime WakeupTime;
      private DateTime SleepTime;
      private int durationID;
      private HomeRangeCriteria homeRangeCriteria;
      private IHomeRangeTrigger mHomeRangeTrigger;
      private PointClass mHomeRangeCenter;
      private IHomeRangeFinder mHomeRangeFinder;
      private AnimalAtributes mAnimalAtributes;
      private Modifier mGenderModifier;
      private Modifier mStateModifer;

      [XmlIgnore]
      private MapManager mMapManager;

      protected string sex;

      [XmlIgnore]
      private AnimalManager mAnimalManager;

      [XmlIgnore]
      private Mover mMover;

      private IPointList mPath;
      private EligibleHomeSites mMyVisitedSites;
      private int timeStep;
      private string fileNamePrefix;

      [XmlIgnore]
      protected TextFileWriter mTextFileWriter;

      [XmlIgnore]
      protected RandomNumbers rn;

      #endregion Fields

      #region Properties (35)

      //Can't serialize the actual mPath so create a list of strings to rebuild mPath off of.
      [XmlArrayItem(ElementName = "Coords")]
      public List<string> myPath { get; set; }

      public HomeRangeCriteria HomeRangeCriteria
      {
         get { return homeRangeCriteria; }
         set { homeRangeCriteria = value; }
      }

      public AnimalAtributes AnimalAtributes
      {
         get { return mAnimalAtributes; }
         set { mAnimalAtributes = value; }
      }

      [XmlIgnore]
      public AnimalManager AnimalManager
      {
         get { return mAnimalManager; }
         set { mAnimalManager = value; }
      }

      public double CaptureFood
      {
         get { return mCaptureFood; }
         set { mCaptureFood = value; }
      }

      public double CurrEnergy
      {
         get { return mCurrEnergy; }
         set { mCurrEnergy = value; }
      }

      public double EnergyUsed
      {
         get { return mEnergyUsed; }
         set { mEnergyUsed = value; }
      }

      public string FileNamePrefix
      {
         get { return fileNamePrefix; }
         set { fileNamePrefix = value; }
      }

      public int FoodIndex
      {
         get { return mFoodIndex; }
         set { mFoodIndex = value; }
      }

      public double FoodMeanSize
      {
         get { return mFoodMeanSize; }
         set { mFoodMeanSize = value; }
      }

      public double FoodSD_Size
      {
         get { return mFoodSD_Size; }
         set { mFoodSD_Size = value; }
      }

      public Modifier GenderModifier
      {
         get { return mGenderModifier; }
         set { mGenderModifier = value; }
      }

      public double Heading
      {
         get { return mHeading; }
         set
         {mHeading = value; }
      }

      [XmlIgnore]
      public PointClass HomeRangeCenter
      {
         get { return mHomeRangeCenter; }
         set { mHomeRangeCenter = value; }
      }

      public IHomeRangeFinder HomeRangeFinder
      {
         get { return mHomeRangeFinder; }
         set { mHomeRangeFinder = value; }
      }

      public IHomeRangeTrigger HomeRangeTrigger
      {
         get { return mHomeRangeTrigger; }
         set { mHomeRangeTrigger = value; }
      }

      public int IdNum
      {
         get { return myIdNum; }
         set { myIdNum = value; }
      }

      public string IdNumOrig
      {
         get { return myIdNumOrig; }
         set { myIdNumOrig = value; }
      }

      public bool IsDead
      {
         get { return mIsDead; }
         set { mIsDead = value; }
      }

      [XmlIgnore]
      public IPoint Location
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

      [XmlIgnore]
      public MapManager MapManager
      {
         get { return mMapManager; }
         set { mMapManager = value; }
      }

      public int MoveIndex
      {
         get { return mMoveIndex; }
         set { mMoveIndex = value; }
      }

      public double MoveSpeed
      {
         get { return this.mMoveSpeed; }
         set { mMoveSpeed = value; }
      }

      public double MoveTurtosity
      {
         get { return this.mMoveTurtosity; }
         set { mMoveTurtosity = value; }
      }

      [XmlIgnore]
      public Mover myMover
      {
         get { return mMover; }
         set { mMover = value; }
      }

      public EligibleHomeSites MyVisitedSites
      {
         get { return mMyVisitedSites; }
         set { mMyVisitedSites = value; }
      }

      public double PerceptonModifier
      {
         get { return mPerceptonModifier; }
         set { mPerceptonModifier = value; }
      }

      public double PredationRisk
      {
         get { return mPredationRisk; }
         set { mPredationRisk = value; }
      }

      public int RiskIndex
      {
         get { return mRiskIndex; }
         set { mRiskIndex = value; }
      }

      public string Sex
      {
         get { return sex; }
         set { sex = value; }
      }

      public int SocialIndex
      {
         get { return mSocialIndex; }
         set { mSocialIndex = value; }
      }

      public Modifier StateModifer
      {
         get { return mStateModifer; }
         set { mStateModifer = value; }
      }

      [XmlIgnore]
      public TextFileWriter TextFileWriter
      {
         get { return mTextFileWriter; }
         set { mTextFileWriter = value; }
      }

      //Save the current location of the animal
      public string myCurrentLocation
      {
         get { if (myLocation != null) { return (myLocation.X.ToString() + "," + myLocation.Y.ToString()); } else { return null; } }
         set
         {
            //If there is an IPoint for myLocation just update the values
            if (myLocation != null) { myLocation.X = Convert.ToDouble(value.Split(',')[0]); myLocation.Y = Convert.ToDouble(value.Split(',')[1]); }
            //Create a new IPoint and load the old values into it
            else
            {
               IPoint p = new PointClass();
               p.X = Convert.ToDouble(value.Split(',')[0]);
               p.Y = Convert.ToDouble(value.Split(',')[1]);
               myLocation = p;
            }
         }
      }

      public string myMoverType
      {
         //Save the current MoverType
         get { if (myMover != null) { return myMover.GetType().ToString(); } return null; }
         set
         { //Create a new Mover from the saved MoverType
            Assembly assembly = Assembly.GetExecutingAssembly();
            myMover = assembly.CreateInstance(value) as Mover;
         }
      }

      public string myHomeRangeCenter
      {
         get { if (HomeRangeCenter != null) { return string.Format("{0},{1}", HomeRangeCenter.X, HomeRangeCenter.Y); } return null; }
         set
         {
            string[] values = value.Split(',');
            PointClass p = new PointClass();
            p.X = Convert.ToDouble(values[0].Trim());
            p.Y = Convert.ToDouble(values[1].Trim());
            mHomeRangeCenter = p;
         }
      }

      public string textFileName
      {
         get { if (fileNamePrefix != null) { return fileNamePrefix; } return null; }
         set { fileNamePrefix = value; }
      }

      public string MyWakeTime
      {
         get { return this.WakeupTime.ToString("o"); }
         set { this.WakeupTime = DateTime.Parse(value); }
      }

      public string MySleepTime
      {
         get { return this.SleepTime.ToString("o"); }
         set { this.SleepTime = DateTime.Parse(value); }
      }

      #endregion Properties

      #region Methods (30)

      #region Public Methods (12)

      //Need to convert mPath from a list of Ipoints to a list of Strings, since IPoints will not xmlSerialize
      //It saves the new list of strings into the myPath for serialization
      public void convertIPointList()
      {
         try
         {
            int j = mPath.getLastIndex();
            List<string> saveList = new List<string>();
            //Foreach IPoint in mPath convert to a string and store it in the list
            for (int i = 0; i < mPath.getLastIndex(); i++)
            {
               IPoint temp = mPath.getPointByIndex(i);
               string myStr = temp.X.ToString() + "," + temp.Y.ToString();
               saveList.Add(myStr);
            }
            myPath = saveList;
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);
         }
      }

      //Reloads mPath from the saved list of strings myPath
      public void rebuildIPointList()
      {
         //Creates a new IPoint reload values and store in mPath
         foreach (string temp in myPath)
         {
            IPoint p = new PointClass();
            double x = Convert.ToDouble((temp.Split(','))[0].Trim());
            double y = Convert.ToDouble((temp.Split(','))[1].Trim());
            p.X = x;
            p.Y = y;
            mPath.add(p);
         }
      }

      public void BuildTextWriter(string currYear)
      {
         BuildTextWriter(currYear, this.AnimalAtributes.OutPutDir);
      }

      public void BuildTextWriter(string CurrYear, string OutPutDir)
      {
         if (this.TextFileWriter == null)
         {
            this.buildFileNamePrefix(CurrYear);
            if (!Directory.Exists(OutPutDir))
               Directory.CreateDirectory(OutPutDir);
            this.TextFileWriter = new TextFileWriter(OutPutDir, this.fileNamePrefix);
         }
      }

      //Rebuilds the TextWriter after a reload
      public void ReBuildTextWriter(string OutPutDir)
      {
         //Update AnimalAtributes to point to the new OutputDirectory
         //this.AnimalAtributes.OutPutDir = OutPutDir;
         // BC 08/08/2013 this was doing shit I removed it
         //if (!Directory.Exists(OutPutDir))
         //    Directory.CreateDirectory(OutPutDir);
         //Builds the new TextFileWriter from the saved fileNamePrefix and the new OutPutDir
         if (fileNamePrefix != null)
         {
            this.TextFileWriter = new TextFileWriter(this.AnimalAtributes.OutPutDir, this.fileNamePrefix);
         }
      }

      public virtual void doTimeStep(HourlyModifier inHM, DailyModifier inDM, DateTime currTime, bool doTextOutput, ref string status)
      {
         double percentTimeStep = 0;
         double tempPercentTimeStep = 0;
         //added Sunday, May 04, 2008
         double previousPercentTimeStep = 0;
         //added temp for holding the old locations for doing text output
         double tempX = 0;
         double tempY = 0;
         int timesStuck = 0;
         bool timeToLookForHome = false;

         try
         {
            //this.timeStepPath++;
            #region
            mLog.Debug("");
            mLog.Debug("inside Animal Do Time Step for " + myIdNum.ToString() +
               " the time is " + currTime.ToShortDateString() + " " + currTime.ToShortTimeString());
            mLog.Debug("percent time step is " + percentTimeStep.ToString());
            mLog.Debug("is dead = " + this.mIsDead.ToString());
            mLog.Debug("do text output is " + doTextOutput.ToString());
            #endregion
            // if(this.IdNum == 14 | this.IdNum == 15) System.Windows.Forms.MessageBox.Show("animal 14");
            this.updateMyLocationValues();
            upDateTemporalModifiers(inHM, inDM);
            this.upDateMyValues();
            while (percentTimeStep < 1 && !this.mIsDead)
            {
               //if(this.Location.X == 0) System.Windows.Forms.MessageBox.Show("No Location");
               mLog.Debug("Start of loop tempPercentTimeStep is " + tempPercentTimeStep.ToString());
               this.timeStep++;

               //store off the original location

               tempX = this.myLocation.X;
               tempY = this.myLocation.Y;

               move(ref percentTimeStep); mLog.Debug("move returned  " + percentTimeStep.ToString());
               mLog.Debug("previousPercentTimeStep is " + previousPercentTimeStep.ToString());
               mLog.Debug("tempPercentTimeStep is " + tempPercentTimeStep.ToString());

               //Saturday, March 15, 2008 added checking to see if his is dead from wandering off the map
               if (!this.IsDead)
               {
                  //Saturday, September 22, 2007 10:03:58 AM Author: Bob Cummings
                  //Added tempPercentTimeStep for accurate percentage on second and subsequent moves in same timestep
                  //so temp should equal 1 - current percent - last times current percent to get this times current percent
                  //Sunday, May 04, 2008 4:14:38 PM Author: Bob Cummings
                  //For some reason I had tempPercentTimeStep = 1 - percentTimeStep - tempPercentTimeStep
                  //But the percent time step is accumlative and the temp% is individual
                  //so added a another temp variable to keep track of accumalting percentTimeSteps

                  //Monday, June 09, 2008
                  tempPercentTimeStep = percentTimeStep - previousPercentTimeStep;
                  mLog.Debug("After doing the math our temp percent is " + tempPercentTimeStep.ToString());
                  //HACK Add issue when two steps are the equal length we get into a loop because
                  //they cancel each other out to Zero so the previous timestep will never accumulate
                  if (tempPercentTimeStep == 0  )
                  {
                     mLog.Debug("evidently the percent returned from move = the accumulated time step so just add the previous to current and call it a day");
                     previousPercentTimeStep += percentTimeStep;
                  }
                  else
                  {
                     previousPercentTimeStep += tempPercentTimeStep;

                  }

                  
                  mLog.Debug("previousPercentTimeStep is " + previousPercentTimeStep.ToString());
                  mLog.Debug("tempPercentTimeStep is " + tempPercentTimeStep.ToString());
                  mLog.Debug("total percent time step is " + percentTimeStep.ToString());

                  //Thursday, February 21, 2008 Moved after move to capture the percent of the step taken
                  //Wednesday, March 05, 2008 moved after calculating the the temp percent to accurately capture the value.
                  if (doTextOutput)
                  {
                     mTextFileWriter.addLine(this.createTextOutput(currTime, tempPercentTimeStep, tempX, tempY));
                  }

                  // do these for every time time step full or partial
                  loseEnergy(tempPercentTimeStep);
                  eat(tempPercentTimeStep);
                  die(tempPercentTimeStep);
                  this.updateBehavioralModifiers();
                  // only do these if going to take another partial step
                  if (percentTimeStep < 1)
                  {
                     this.updateMyLocationValues();
                     this.upDateMyValues();
                  }
               }
            }

            //Saturday, March 15, 2008 added checking to see if his is dead from wandering off the map
            if (!this.IsDead)
            {
               if (this.IsAsleep == false)
               {
                  mLog.Debug("ok now see if it is time to look for a home yet");
                  //Author: Bob Cummings Friday, October 05, 2007
                  //moved this to inside if statement to stop looking for a home
                  //while asleep.
                  timeToLookForHome = this.mHomeRangeTrigger.timeToLookForHome(this);
                  mLog.Debug("Time to look for a home came back with a value of " + timeToLookForHome.ToString());
               }

               if (timeToLookForHome && !this.goingHome && this.MyVisitedSites.SiteCount > 0)
               {
                  mLog.Debug("yes it is time to look for a home calling find home");

                  if (findHome())
                  {
                     this.mTextFileWriter.addLine("I have found a home and I am going to it");
                     mLog.Debug("back in doTimeStep and we found a home to set the mover to directed mover");
                     mLog.Debug("and set the boolean variable goingHome = true");
                     this.mMover = new DirectedMover();
                     this.goingHome = true;
                  }//if we do not find a home we keep wandering around and check again.
               }

               if (this.goingHome)
               {
                  double distToHome;
                  mLog.Debug("ok we are headed home now check the distace between here and there");
                  distToHome = this.MapManager.getDistance(this.Location, this.HomeRangeCenter);
                  mLog.Debug("we still have to go " + distToHome.ToString());
                  mLog.Debug("I can see " + this.mPerceptionDist.ToString());
                  if (distToHome < this.mPerceptionDist)
                  {
                     mLog.Debug("now building the home range");
                     // only change to resident if we were able to build the home range
                     if (this.MapManager.BuildHomeRange(this))
                     {
                        Console.WriteLine("number " + this.IdNum.ToString() + " is home.");
                        mLog.Debug("found a home");
                        mLog.Debug("ok we are home now setting the location = home range center");
                        mLog.Debug("the new X should = " + this.HomeRangeCenter.X.ToString());
                        mLog.Debug("the new Y should = " + this.HomeRangeCenter.Y.ToString());
                        this.Location = this.HomeRangeCenter as PointClass;
                        mLog.Debug("the new X = " + this.Location.X.ToString());
                        mLog.Debug("the new Y = " + this.Location.Y.ToString());
                        this.foundHome = true;
                        status = "resident";
                        if (doTextOutput)
                           this.mTextFileWriter.addLine("Found Home at " + this.myLocation.X.ToString() + " and " + this.myLocation.Y.ToString());
                        // System.Windows.Forms.MessageBox.Show(this.IdNum.ToString() + " found a home kill the application and look at at the input social map ");
                     }
                     else
                     {
                        //evidently can not build a site here.
                        goingHome = false;
                        myMover = RandomWCMover.getRandomWCMover();
                        MyVisitedSites.RemoveSite(this.HomeRangeCenter);
                     }
                  }
               }

               //No sense doing if we are home.
               if (foundHome == false)
               {
                  //Saturday, February 23, 2008 Move to before taking a step
                  //Tuesday, July 15, 2008 move back to end of step
                  if (this.IsAsleep == false)
                  {
                     this.mMyVisitedSites.addSite(new EligibleHomeSite(this.mCaptureFood, this.mPredationRisk, this.myLocation.X, this.myLocation.Y));
                     mLog.Debug("we now have " + MyVisitedSites.MySites.Count.ToString() + " sites");
                  }

                  if (this.goingHome && this.IsAsleep == false)
                  {
                     timesStuck++;
                     if (timesStuck > 20)
                     {
                        this.IsDead = true;
                        mTextFileWriter.addLine("Stuck on the way home for 20 times so died");
                        //get out of the loop
                        percentTimeStep = 1.5;
                     }
                  }
               }
            }
            changeActiveState(currTime);
         }
         catch (System.Exception ex)
         {
            string temp = "Animal " + this.IdNum.ToString() + " had an issue: ";
            eLog.Debug(temp + ex);
         }
      }

      //end of updateMemory
      /********************************************************************************
       *   Function name   : dump
       *   Description     :
       *   Return type     : virtual void
       * ********************************************************************************/

      public virtual void dump()
      {
         try
         {
            mLog.Debug("");
            mLog.Debug("inside animal dump");
            mLog.Debug("My ID number is " + this.IdNum.ToString());
            mLog.Debug("my X Location is " + this.Location.X.ToString() + " my y location is " + this.Location.Y.ToString());
            mLog.Debug("My prob of capturing food is " + this.mCaptureFood.ToString());
            mLog.Debug("My prob of getting killed is " + this.mPredationRisk.ToString());
            mLog.Debug("My movement speed is " + this.mMoveSpeed.ToString());
            mLog.Debug("My move tortuosity is " + this.mMoveTurtosity.ToString());
            mLog.Debug("My current use of food is " + this.mEnergyUsed.ToString());
            mLog.Debug("My perception range is " + this.mPerceptonModifier.ToString());
            mLog.Debug("leaving animal dump");
            mLog.Debug("");
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }
      }

      // end of doTimeStep
      public PointClass GetEligibleStep(int stepNum)
      {
         IPoint p = new PointClass();
         EligibleHomeSite ehs = MyVisitedSites.getSite(stepNum) as EligibleHomeSite;
         p.X = ehs.X;
         p.Y = ehs.Y;
         return p as PointClass;
      }

      public string getLocation_XY()
      {
         return " X = " + myLocation.X.ToString() + " Y = " + myLocation.Y.ToString();
      }

      public int getNumStepsInPath()
      {
         return this.mPath.Count();
      }

      public bool isSiteGood()
      {
         bool isSuitable = false;
         bool isAvailable = false;
         try
         {
            mLog.Debug("inside is site good for animal " + this.IdNum.ToString());
            mLog.Debug("checking against social index number " + this.mSocialIndex.ToString());
            isSuitable = this.MapManager.isSuitable(this.mSocialIndex);
            //if it is occupied it is not available
            if (isSuitable)
            {
               isAvailable = !this.MapManager.isOccupied(this.mSocialIndex, this.sex);
            }
            mLog.Debug("is Suitable returned " + isSuitable.ToString());
            mLog.Debug("is available returned " + isAvailable.ToString());
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
         return isSuitable && isAvailable;
      }
      
      public void setInitialSleepTime(DateTime currTime)
      {
         mLog.Debug("inside set inital sleeptime curr time is " + currTime.ToShortDateString() + "  " + currTime.ToShortTimeString());
         this.WakeupTime = currTime;
         this.calcSleepTime();
      }

      public void setInitialValues(DateTime currTime)
      {
         try
         {
            mLog.Debug("inside setInitialValues for animal " + this.myIdNum.ToString());
            mLog.Debug("the time is " + currTime.ToShortDateString());

            if (this.mMoveIndex < 0)
               setInitialLocaton();
            this.mPerceptionDist = this.AnimalAtributes.PerceptionDistance;

            this.setSocialIndex(this.Location);
            this.mMapManager.GetInitialMapData(this);
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
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
            IPoint startLeft = new PointClass();
            IPoint startRight = new PointClass();
            IPoint endLeft = new PointClass();
            IPoint endRight = new PointClass();
            IGeometry g;
            ITopologicalOperator to;
            CircularArcClass startCurve = new CircularArcClass();
            CircularArcClass endCurve = new CircularArcClass();
            LineClass line1 = new LineClass();
            LineClass line2 = new LineClass();
            LineClass line3 = new LineClass();
            LineClass line4 = new LineClass();
            PolygonClass memoryPolygon = new PolygonClass();
            object Missing = Type.Missing;
            //get angle from start to end
            angleMoved = System.Math.Atan((start.Y - end.Y) / (start.X - end.X));
            //rotate pi/2
            angleLeft = angleMoved + Math.PI / 2;
            angleRight = angleMoved - Math.PI / 2;
            //step perceptionDistance forward and back
            startLeft.X = start.X + this.mPerceptionDist * System.Math.Cos(angleLeft);
            startLeft.Y = start.Y + mPerceptionDist * System.Math.Sin(angleLeft);
            startRight.X = start.X + mPerceptionDist * System.Math.Cos(angleRight);
            startRight.Y = start.Y + mPerceptionDist * System.Math.Sin(angleRight);
            //repeat for end
            endLeft.X = end.X + mPerceptionDist * System.Math.Cos(angleLeft);
            endLeft.Y = end.Y + mPerceptionDist * System.Math.Sin(angleLeft);
            endRight.X = end.X + mPerceptionDist * System.Math.Cos(angleRight);
            endRight.Y = end.Y + mPerceptionDist * System.Math.Sin(angleRight);
            line1.FromPoint = startLeft;
            line1.ToPoint = endLeft;
            line2.FromPoint = endLeft;
            line2.ToPoint = endRight;
            line3.FromPoint = endRight;
            line3.ToPoint = startRight;
            line4.FromPoint = startRight;
            line4.ToPoint = startLeft;
            mLog.Debug("my current location is " + this.getLocation_XY());
            mLog.Debug("StartRight is x,y " + startRight.X.ToString() + " " + startRight.Y.ToString());
            mLog.Debug("startLeft is x,y " + startLeft.X.ToString() + " " + startLeft.Y.ToString());
            mLog.Debug("endRight is x,y " + endRight.X.ToString() + " " + endRight.Y.ToString());
            mLog.Debug("endLeft is x,y " + endLeft.X.ToString() + " " + endLeft.Y.ToString());
            memoryPolygon.SpatialReference = this.Location.SpatialReference;
            memoryPolygon.AddSegment((ISegment)line1, ref Missing, ref Missing);
            memoryPolygon.AddSegment((ISegment)line2, ref Missing, ref Missing);
            memoryPolygon.AddSegment((ISegment)line3, ref Missing, ref Missing);
            memoryPolygon.AddSegment((ISegment)line4, ref Missing, ref Missing);
            if (memoryPolygon.Area < 0.0)
               memoryPolygon.ReverseOrientation();
            to = (ITopologicalOperator)end;
            g = to.Buffer(mPerceptionDist);
            mLog.Debug("adding the step for time step " + this.timeStep.ToString());
            //  this.MapManager.AddMemoryPoly(this.IdNum,memoryPolygon);
            this.MapManager.AddTimeSteps(this.IdNum, memoryPolygon, (IPolygon)g, timeStep, sex);
            if (goingHome)
            {
               this.setSocialIndex(end);
            }
         }
         catch (System.Exception ex)
         {
            //mLog.Debug("error look for error file");
            // System.Windows.Forms.MessageBox.Show(ex.StackTrace);
            eLog.Debug("Problem with animal ID: " + this.IdNum);
            eLog.Debug(ex);
         }
      }

      #endregion Public Methods
      #region Private Methods (17)

      private ILog mLog = LogManager.GetLogger("animalLog");
      private ILog eLog = LogManager.GetLogger("Error");

      private void calcSleepTime()
      {
         Check.Require(this.IsAsleep == false, "Not asleep why get sleep time");
         double numHours = 0;
         double mean = 0;
         double sd = 0;
         mLog.Debug("inside calc sleep getting the number of hours before I go to sleep");
         mLog.Debug("get the mean and sd for this rest time");
         this.AnimalAtributes.getDurationMeanAndSD(ref mean, ref sd, ref durationID);
         numHours = Math.Abs(rn.getNormalRandomNum(mean, sd));
         mLog.Debug("I will be active for " + numHours.ToString());
         this.SleepTime = this.WakeupTime.AddHours(numHours);
         mLog.Debug("so I will go to sleep at " + this.SleepTime.ToShortDateString() + " " + this.SleepTime.ToShortTimeString());
      }

      //end of calcSleepTime
      private void calcWakeTime()
      {
         Check.Require(this.IsAsleep == true, "George is awake why get bedtime?");
         double numHours = 0;
         double mean = 0;
         double sd = 0;
         mLog.Debug("inside calc sleep getting the number of hours before I go to wake up");
         mLog.Debug("get the mean and sd for this rest time");
         this.AnimalAtributes.getDurationMeanAndSD(ref mean, ref sd, ref this.durationID);
         numHours = Math.Abs(rn.getNormalRandomNum(mean, sd));
         mLog.Debug("I will be active for " + numHours.ToString());
         this.WakeupTime = this.SleepTime.AddHours(numHours);
         mLog.Debug("so I will wake up at " + this.WakeupTime.ToShortDateString() + " " + this.WakeupTime.ToShortTimeString());
      }

      //end of calcWakeTime
      private void changeActiveState(DateTime currTime)
      {
         mLog.Debug("inside change active state");
         if (this.IsAsleep == true)
         {
            mLog.Debug("must be asleep");
            mLog.Debug("Waketime is " + this.WakeupTime.Date.ToShortDateString() + " " + this.WakeupTime.TimeOfDay.ToString());

            mLog.Debug("curr time is " + currTime.Date.ToShortDateString() + " " + currTime.TimeOfDay.ToString());
            if ((this.WakeupTime.Date < currTime.Date) || (this.WakeupTime.Date <= currTime.Date && this.WakeupTime.TimeOfDay <= currTime.TimeOfDay))
            {
               this.IsAsleep = false;
               mLog.Debug("time to wake up, I am going home = " + this.goingHome.ToString());
               //Sunday, February 24, 2008
               //had issues when going home, would stop to sleep and then
               //forget he was going home
               if (this.goingHome)
                  this.myMover = new DirectedMover();
               else
                  this.myMover = RandomWCMover.getRandomWCMover();
               mLog.Debug("my new mover is a type of " + this.myMover.GetType().ToString());
               calcSleepTime();
            }
         }
         else
         {
            mLog.Debug("must be awake");
            mLog.Debug("Sleep time is " + this.SleepTime.Date.ToShortDateString() + " " + this.SleepTime.TimeOfDay.ToString());
            mLog.Debug("curr time is " + currTime.Date.ToShortDateString() + " " + currTime.TimeOfDay.ToString());
            if ((this.SleepTime.Date < currTime.Date) || (this.SleepTime.Date <= currTime.Date && this.SleepTime.TimeOfDay <= currTime.TimeOfDay))
            {
               this.IsAsleep = true;
               this.myMover = SleepMover.getSleepMover();
               mLog.Debug("time to go to sleep");
               calcWakeTime();
            }
         }
      }

      //end of changeActiveState
      private string createTextOutput(DateTime currTime, double inPercent, double inX, double inY)
      {
         string outPut = null;

         try
         {
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
               (1 - (Math.Pow((1 - this.mPredationRisk), inPercent))).ToString() + "," +
               (this.mCaptureFood * inPercent).ToString() + "," +
               this.mMoveTurtosity.ToString() + "," +
               (this.mMoveSpeed * inPercent).ToString() + "," +
               this.mPerceptionDist.ToString() + "," +
               inPercent.ToString();
         }
         catch (System.Exception ex)
         {
            eLog.Debug(ex);
         }

         return outPut;
      }

      // end of createTextOutput
      private void die(double percentTimeStep)
      {
         double tempRisk = 0.0;
         double tempCloseCall = 0.0;
         double rollOfTheDice = 0.0;

         rollOfTheDice = rn.getUniformRandomNum();
         mLog.Debug("inside die with roll of " + rollOfTheDice.ToString());
         mLog.Debug("going to adust the chance of getting eaten current chance is " + mPredationRisk.ToString());
         mLog.Debug("and percent time step is " + percentTimeStep.ToString());
         tempRisk = (1 - (Math.Pow((1 - mPredationRisk), percentTimeStep)));
         mLog.Debug("new chance of getting ambushed is " + tempRisk.ToString());
         tempCloseCall = rollOfTheDice - tempRisk;
         mLog.Debug("Close call value is " + tempCloseCall.ToString());
         mLog.Debug("SafeRiskyTrigger is " + this.AnimalAtributes.SafeRiskyTrigger.ToString());
         mLog.Debug("RiskySafeTrigger is " + this.AnimalAtributes.RiskySafeTrigger.ToString());

         if (tempRisk > rollOfTheDice)
         {
            mLog.Debug("george dies from predation");
            this.mIsDead = true;
            mTextFileWriter.addLine("Animal number " + this.IdNum.ToString() + " died from predation");
         }
         else if (mCurrEnergy < this.AnimalAtributes.MinEnergy_Survive)
         {
            mLog.Debug("george dies from starvation");
            this.mIsDead = true;
            mTextFileWriter.addLine("Animal number " + this.IdNum.ToString() + " died from starvation");
         }

         if (tempCloseCall < this.AnimalAtributes.RiskySafeTrigger) //george nearly died, so he feels like he had a close call
         {
            this.hadCloseCall = true;
         }
         else if (tempCloseCall < (this.AnimalAtributes.SafeRiskyTrigger + this.AnimalAtributes.RiskySafeTrigger)) //predator wasn't even close, so george no longer feels like he had a close call
         {
            this.hadCloseCall = false;
         }
      }

      //end of die
      private void eat(double percentTimeStep)
      {
         double chanceOfEating = 0;
         double foodAmt = 0;
         double rollOfTheDice = rn.getUniformRandomNum();
         mLog.Debug("inside eat for animail number " + this.IdNum.ToString() + " current energy level  is " + mCurrEnergy.ToString());
         mLog.Debug("the roll of the dice is " + rollOfTheDice.ToString());

         mLog.Debug("my food index is " + mFoodIndex.ToString());
         mLog.Debug("chance of eating from the map is " + this.mCaptureFood.ToString());
         chanceOfEating = this.mCaptureFood * percentTimeStep;
         mLog.Debug("I had this good a chance to get food " + chanceOfEating.ToString() + " this timestep");
         if (rollOfTheDice < chanceOfEating)
         {
            foodAmt = System.Math.Abs(rn.getNormalRandomNum(this.mFoodMeanSize, this.mFoodSD_Size));
            mLog.Debug("so I get this much energy " + foodAmt.ToString());
            this.mCurrEnergy = this.mCurrEnergy + foodAmt;
            if (this.mCurrEnergy > this.AnimalAtributes.MaxEnergy)
               this.mCurrEnergy = this.AnimalAtributes.MaxEnergy;
         }
         mLog.Debug("so current energy level is " + this.mCurrEnergy.ToString());
      }

      //end of eat
      private bool findHome()
      {
         string myMapName;
         bool success = false;
         try
         {
            mLog.Debug("inside find home calling to get the name of my memory map");
            myMapName = this.MapManager.getAnimalMapName(this.IdNum);
            if (myMapName != null)
            {
               mLog.Debug("now setting the home range center");
               success = this.HomeRangeFinder.setHomeRangeCenter(this, myMapName);

               mLog.Debug("that returned " + success.ToString());
            }
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }

         return success;
      }

      private void loseEnergy(double percentTimeStep)
      {
         double lostEnergy = 0;
         //changed calc to use EnergyUsed instead of mTemporalEnergyUsed
         lostEnergy = percentTimeStep * mEnergyUsed; mLog.Debug("I used " + mEnergyUsed.ToString() + " this timestep");
         mLog.Debug("inside lose energy current energy level  is " + mCurrEnergy.ToString());
         this.mCurrEnergy = this.mCurrEnergy - lostEnergy; mLog.Debug("so current energy level is " + this.mCurrEnergy.ToString());
      }
      

      private void move(ref double percentTimeStep)
      {
         mLog.Debug("inside move calling myMover Move passing myself as well");
         myMover.move(ref percentTimeStep, this);
      }

      private void setInitialLocaton()
      {
         mLog.Debug("inside set initial location x = " + myLocation.X.ToString() + " y " + myLocation.Y.ToString());
      }

      private void setSocialIndex()
      {
         // fw.writeLine("inside setting social index old one was " +SocialIndex.ToString());
         //fw.writeLine("my location is x = " + this.myLocation.X.ToString() + " Y = " + this.myLocation.Y.ToString());
         this.mMapManager.getSocialIndex(this.Location, ref mSocialIndex);
         // fw.writeLine("new social index is " + this.SocialIndex.ToString());
      }

      private void setSocialIndex(IPoint currLoc)
      {
         this.mMapManager.getSocialIndex(currLoc, ref mSocialIndex);
      }

      private void updateBehavioralModifiers()
      {
         if (this.hadCloseCall) //animal feels threatened, so will act more safely
         {
            mLog.Debug("inside updateBehavioralModifiers, hadCloseCall = " + hadCloseCall.ToString());
            mLog.Debug("Energy level is: " + CurrEnergy.ToString() + " ForageSearchTrigger is: " + this.AnimalAtributes.ForageSearchTrigger.ToString());
            if (this.CurrEnergy < this.AnimalAtributes.ForageSearchTrigger)//animal is hungry, needs to forage
            {
               mLog.Debug("setting stateModifier to a new SafeForageModifier");
               this.StateModifer = this.AnimalManager.SafeForageMod;
            }
            else //animal isn't hungry, so is looking for a home
            {
               mLog.Debug("setting stateModifier to a new SafeSearchModifier");
               this.StateModifer = this.mAnimalManager.SafeSearchMod;
            }
         }
         else  //animal feels safe, so will act more riskily
         {
            mLog.Debug("inside updateBehavioralModifiers, hadCloseCall = " + hadCloseCall.ToString());
            if (this.CurrEnergy < this.AnimalAtributes.ForageSearchTrigger)//animal is hungry, needs to forage
            {
               mLog.Debug("setting stateModifier to a new RiskyForageModifier");
               this.StateModifer = this.mAnimalManager.RiskyForageMod;
            }
            else  //animal isn't hungry, so is looking for a home
            {
               mLog.Debug("setting stateModifier to a new RiskySearchModifier");
               this.StateModifer = this.mAnimalManager.RiskySearchMod;
            }
         }
      }

      private void updateMyLocationValues()
      {
         mLog.Debug("inside update updateMyLocationValues where we pull the data direct from the map");
         this.setSocialIndex();
         this.mMapManager.GetMoveModifiers(this.myLocation, ref mMoveIndex, ref mMoveTurtosity, ref mMoveSpeed, ref mPerceptonModifier, ref mEnergyUsed);
         mLog.Debug("the map says my turosity should be " + mMoveTurtosity.ToString() + " before modifying");
         this.mMapManager.GetRiskModifier(this.myLocation, ref this.mRiskIndex, ref mPredationRisk);
         //Author: Bob Cummings moved down here from eat() made more sense to do it all in one place
         // fw.writeLine("my food index is " + this.FoodIndex.ToString());
        this.mMapManager.GetFoodData(this.myLocation, ref this.mFoodIndex, ref this.mCaptureFood, ref this.mFoodMeanSize, ref this.mFoodSD_Size);
      }

      private void upDateMyValues()
      {
         mLog.Debug("now inside update my values which combines the time adjustment, the state of being adjustment, and location adjustment");
         mLog.Debug("so the value for turtosity from the map is " + mMoveTurtosity.ToString());
         mLog.Debug("the adjustment base on time is " + mTemporalMoveTurtosity.ToString());
         mLog.Debug("the adjustment base on state of being is " + StateModifer.MoveTurtosity.ToString());
         this.mMoveSpeed = this.mMoveSpeed * this.mTemporalMoveSpeed * this.StateModifer.MoveSpeed;
         this.mMoveTurtosity = this.mMoveTurtosity * this.mTemporalMoveTurtosity * this.StateModifer.MoveTurtosity;
         this.mCaptureFood = this.mCaptureFood * this.mTemporalChanceOfEating * this.StateModifer.CaptureFood;
         this.mPredationRisk = this.mPredationRisk * this.mTemporalRiskValue * this.StateModifer.PredationRisk;
         this.mEnergyUsed = this.mEnergyUsed * this.mTemporalEnergyUsed * this.StateModifer.EnergyUsed;
         this.mPerceptionDist = this.mAnimalAtributes.PerceptionDistance * this.mTemporalPerceptonDistance * mPerceptonModifier * this.StateModifer.PerceptonModifier;
         //Now/make sure the risk modifier is >1
         //BC 08/08/2013

         if (mPredationRisk > 1)
         {
            mPredationRisk = 1;
         }
         mLog.Debug("so my new turtosity is " + mMoveTurtosity.ToString());
      }

      private void upDateTemporalModifiers(HourlyModifier inHM, DailyModifier inDM)
      {
         try
         {
            //Author: Bob Cummings added the temporal values.  we were modify too much.
            mLog.Debug("inside setting the temporal adjustmenst");
            mLog.Debug("current temporal move turosity is " + mTemporalMoveTurtosity.ToString());
            mLog.Debug("gender modifier is " + this.GenderModifier.MoveTurtosity.ToString());
            mLog.Debug("current hourly modifier is " + inHM.MoveTurtosity.ToString());
            mLog.Debug("current daily modifier is " + inDM.MoveTurtosity);
            mTemporalChanceOfEating = this.GenderModifier.CaptureFood * inHM.CaptureFood * inDM.CaptureFood;
            mTemporalRiskValue = this.GenderModifier.PredationRisk * inHM.PredationRisk * inDM.PredationRisk;
            mTemporalMoveSpeed = this.GenderModifier.MoveSpeed * inHM.MoveSpeed * inDM.MoveSpeed;
            mTemporalMoveTurtosity = this.GenderModifier.MoveTurtosity * inHM.MoveTurtosity * inDM.MoveTurtosity;
            mTemporalEnergyUsed = this.GenderModifier.EnergyUsed * inHM.EnergyUsed * inDM.EnergyUsed;
            mTemporalPerceptonDistance = this.GenderModifier.PerceptonModifier * inHM.PerceptonModifier * inDM.PerceptonModifier;
            mLog.Debug("so now the current temporal move turosity is " + mTemporalMoveTurtosity.ToString());
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      #endregion Private Methods
      #region Protected Methods (1)

      //end of buildLogger
      protected void buildFileNamePrefix(string year)
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();
         try
         {
            Check.Require(sex != null, "No sex for the animal to build the file name prefix");
            sb.Append(this.IdNum.ToString());
            //pad out to 5 long
            while (sb.Length < 6)
               sb.Insert(0, "0");

            sb.Insert(0, sex);
            sb.Insert(0, year);
            this.fileNamePrefix = sb.ToString();
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            eLog.Debug(ex);
         }
      }

      #endregion Protected Methods

      #endregion Methods
   }
}