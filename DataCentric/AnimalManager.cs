using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Diagnostics;
using System.IO;
using DesignByContract;
using ESRI.ArcGIS.Geometry;

namespace SEARCH
{
   public class AnimalManager
   {
		#region Constructors (1) 

      public AnimalManager()
      {
         buildLogger();
         fw.writeLine("getting the modifiers");
         mMaleModifier = MaleModifier.GetUniqueInstance();
         mFemaleModifier = FemaleModifier.GetUniqueInstance();
         mAnimalAttributes = new AnimalAtributes();
         mRiskySearchMod = new RiskySearchModifier();
         mRiskyForageMod = new RiskyForageModifier();
         mSafeForageMod = new SafeForageModifier();
         mSafeSearchMod = new SafeSearchModifier();
         fw.writeLine("now making the mover object");
         mMover = RandomWCMover.getRandomWCMover();
         this.myDispersers = new List<Animal>();
         this.myResidents = new List<Resident>();
   


      }

		#endregion Constructors 

		#region Fields (21) 

      private IEnumerator currAnimal;
      private FileWriter.FileWriter fw;
      private AnimalAtributes mAnimalAttributes;
      private HomeRangeCriteria mFemaleHomeRangeCriteria;
      private FemaleModifier mFemaleModifier;
      private IHomeRangeFinder mHomeRangeFinder;
      private IHomeRangeTrigger mHomeRangeTrigger;
      private HomeRangeCriteria mMaleHomeRangeCriteria;
      private MaleModifier mMaleModifier;
      private Dictionary<IPoint, MapValue> mMapValues;
      private Mover mMover;
      private ResidentAttributes mResidentAttributes;
      private Modifier mRiskyForageMod;
      private Modifier mRiskySearchMod;
      private Modifier mSafeForageMod;
      private Modifier mSafeSearchMod;
      private string myErrMessage;
      List<Animal> myDispersers;

      public List<Animal> MyDispersers
      {
         get { return myDispersers; }
         
      }
      List<Resident> myResidents;

      public List<Resident> MyResidents
      {
         get { return myResidents; }
         
      }


		#endregion Fields 

		#region Properties (10) 

      public int NumAnimals
      {
         get { return myDispersers.Count; }

      }

      public AnimalAtributes AnimalAttributes
      {
         get { return mAnimalAttributes; }
         set { mAnimalAttributes = value; }
      }

      public string ErrMessage
      {
         get { return myErrMessage; }

         set { myErrMessage = value; }
      }

      public FemaleModifier FemaleModifier
      {
         get { return mFemaleModifier; }
         set
         {
            mFemaleModifier = value;
            fw.writeLine("inside animal manager setting the mFemaleModifierMod");
         }
      }

      public MaleModifier MaleModifier
      {
         get { return mMaleModifier; }
         set
         {
            mMaleModifier = value;
            fw.writeLine("inside animal manager setting the mMaleModifierMod");
         }
      }

      public ResidentAttributes ResidentAttributes
      {
         get { return mResidentAttributes; }
         set { mResidentAttributes = value; }
      }

      public Modifier RiskyForageMod
      {
         get { return mRiskyForageMod; }
         set
         {
            mRiskyForageMod = value;
            fw.writeLine("inside animal manager setting the mRiskyForageMod");
         }
      }

      public Modifier RiskySearchMod
      {
         get { return mRiskySearchMod; }
         set { mRiskySearchMod = value; }
      }

      public Modifier SafeForageMod
      {
         get { return mSafeForageMod; }
         set
         {
            mSafeForageMod = value;
            fw.writeLine("inside animal manager setting the mSafeForageMod");
         }

      }

      public Modifier SafeSearchMod
      {
         get { return mSafeSearchMod; }
         set
         {
            mSafeSearchMod = value;
            fw.writeLine("inside animal manager setting the mSafeSearchMod");
         }
      }

		#endregion Properties 

		#region Methods (34) 

		#region Public Methods (19) 

      public void addNewDispersers(InitialAnimalAttributes[] inIAA, DateTime currTime)
      {
         try
         {
            fw.writeLine("inside addNewDispersers starting loop");
            for (int i = 0; i <= inIAA.Length - 1; i++)
            {
               fw.writeLine("calling makeNextGenAnimal");
               this.makeNextGenAnimal(inIAA[i], currTime);
            }
            fw.writeLine("done with loop in addNewDispersers calling mHomeRangeTrigger.reset");
            this.mHomeRangeTrigger.addNewDispersers(this.myDispersers);
            fw.writeLine("back in addNewDispersers calling setNextGenHomeRange");
            setNextGenHomeRange();
            fw.writeLine("done with adding new dispersers leaving addNewDispersers");

         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      public void adjustNewSocialMap(Map newSocialMap)
      {
         this.resetResidentsHomeRange(newSocialMap);
         this.resetDisperserSocialIndex(newSocialMap);

      }

      public void breedFemales(DateTime currTime)
      {
         fw.writeLine("inside breed females");
         int numMales = 0;
         int numFemales = 0;
         InitialAnimalAttributes iaa;
         fw.writeLine("starting the loop");
         List<Resident> myBreeders = this.getResidentsBySex("female");

         foreach (Resident r in myBreeders)
         {

            r.breed(out numMales, out numFemales);
            if (numMales > 0)
            {
               iaa = new InitialAnimalAttributes();
               iaa.Location = r.HomeRangeCenter;
               iaa.Sex = 'M';
               iaa.NumToMake = numMales;
               this.makeNextGenAnimal(iaa, currTime);
            }
            if (numFemales > 0)
            {
               iaa = new InitialAnimalAttributes();
               iaa.Location = r.HomeRangeCenter;
               iaa.Sex = 'F';
               iaa.NumToMake = numFemales;
               this.makeNextGenAnimal(iaa, currTime);
            }
         }
        
         this.mHomeRangeTrigger.reset(this.myDispersers);
         setNextGenHomeRange(); 

      }

      public void changeToDeadAnimal(Animal inA)
      {
         try
         {
            fw.writeLine("inside changeToDeadAnimal for animal num " + inA.IdNum);
            DeadAnimal dd = new DeadAnimal(inA);
            fw.writeLine("removing at position " + dd.IdNum.ToString());
            this.myDispersers.RemoveAt(dd.IdNum);
            this.myDispersers.Insert(dd.IdNum, dd);

            fw.writeLine("new animal type is " + inA.GetType());
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
      }

      public void doTimeStep(HourlyModifier inHM, DailyModifier inDM, DateTime currTime, bool DoTextOutPut, Map currSocialMap)
      {
         try
         {
            fw.writeLine("inside animal manager do time step with modifiers");
            fw.writeLine("calling do disperser time step");
            doDisperserTimeStep(inHM, inDM, ref currTime, DoTextOutPut, currSocialMap);
            fw.writeLine("now calling do Resident time step");
            doResidentTimeStep();
            fw.writeLine("done with do time step");
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      public void dump()
      {
         foreach (Animal a in myDispersers)
         {
            a.dump();
         }
      }

      public int getNumDispersers()
      {
         int num = 0;
         try
         {
            num = this.getDispersers().Count;

         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return num;
      }

      public bool makeInitialAnimals(InitialAnimalAttributes[] inIAA)
      {
         bool success = true;

         Animal tmpAnimal;

         //fw.writeLine("inside make initial animals");
         try
         {
            //this is an array of attributes one of which is how many to make of this type
            for (int i = 0; i <= inIAA.Length - 1; i++)
            {
               //so for each entity in the array we need to make j amount of animals
               for (int j = 0; j < inIAA[i].NumToMake; j++)
               {
                  if (inIAA[i].Sex == 'M')
                  {
                     //fw.writeLine("makeing r new male");
                     tmpAnimal = new Male();
                  }
                  else
                  {
                     fw.writeLine("makeing r new female");
                     tmpAnimal = new Female();
                  }

                 
                  fw.writeLine("just made " + tmpAnimal.IdNum.ToString());
                  tmpAnimal.Location = inIAA[i].Location;

                  tmpAnimal.AnimalAtributes = this.AnimalAttributes;
                  fw.writeLine("now setting the mover");
                  tmpAnimal.myMover = this.mMover;
                  tmpAnimal.AnimalManager = this;
                  fw.writeLine("now adding to the list of my animals;");
                  this.myDispersers.Add(tmpAnimal);
                  
               }
            }
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
            fw.writeLine("");
            success = false;
         }
         return success;
      }

      public bool makeResidents(string currYear, InitialAnimalAttributes[] inResAttributes)
      {
         bool success = false;
         Resident r;
         fw.writeLine("inside makeResidents going to make " + inResAttributes.Length.ToString());
         fw.writeLine("total animals now is " + this.myDispersers.Count.ToString());
         try
         {
            for (int i = 0; i < inResAttributes.Length; i++)
            {
               r = new Resident();

               r.MyAttributes = new ResidentAttributes();
               if (inResAttributes[i].Sex.ToString().ToUpper() == "M")
               {
                  r.Sex = "Male";

               }
               else
               {
                  r.Sex = "Female";

               }
               r.Location = inResAttributes[i].Location;
               r.HomeRangeCenter = r.Location as PointClass; ;
               r.MyAttributes.OriginalID = inResAttributes[i].OrginalID;
               this.myResidents.Add(r);
            }

            fw.writeLine("after adding residents total animals now is " + this.myDispersers.Count.ToString());
            success = true;

         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }

         return success;

      }

      public void removeRemaingDispersers()
      {
         try
         {
            List<Animal> poorSuckers = this.getDispersers();
            fw.writeLine("inside remove remaining dispersers");
            fw.writeLine("going to loop through " + this.myDispersers.Count.ToString() + " in the collection.");
            foreach (Animal a in poorSuckers)
            {
               {
                  fw.writeLine("so we are setting isDead to true");
                  if (a.TextFileWriter != null)
                     a.TextFileWriter.addLine("Died during winter kill");
                  myDispersers.Remove(a);
               }
            }
            fw.writeLine("there are " + this.getNumDispersers().ToString() + " animals left.");
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }

      }

      public bool setAttributes()
      {
         bool success = true;
         try
         {

            foreach (Animal temp in myDispersers)
            {
               temp.AnimalAtributes = this.AnimalAttributes;
               temp.CurrEnergy = this.AnimalAttributes.InitialEnergy;
            }
         }
         catch (System.Exception ex)
         {
            success = false;
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return success;
      }

      /********************************************************************************
       *   Function name   : setHomeRange
       *   Description     : 
       *   Return type     : void 
       *   Argument        : string sex
       *   Argument        : double area
       *   Argument        : double distanceMean
       *   Argument        : double distanceSD
       * ********************************************************************************/
      public void setHomeRange(string sex, double area, double distanceMean, double distanceSD, double distanceWeight)
      {
         if (sex.ToUpper() == "MALE")
         {
            this.mMaleHomeRangeCriteria = new HomeRangeCriteria(area, distanceMean, distanceSD, distanceWeight);
            setMaleHomeRange();
         }
         else
         {
            this.mFemaleHomeRangeCriteria = new HomeRangeCriteria(area, distanceMean, distanceSD, distanceWeight);
            setFemaleHomeRange();
         }
      }

      public bool setHomeRangeCriteria(string type)
      {
         bool success = false;

         try
         {
            fw.writeLine("inside AnimalManger setHomeRangeCriteria " + "for " + type);
            switch (type)
            {
               case ("Food"):
                  fw.writeLine("making r foodie");
                  mHomeRangeFinder = BestFoodHomeRangeFinder.getInstance();
                  break;
               case ("Risk"):
                  fw.writeLine("making r risky");
                  mHomeRangeFinder = BestRiskHomeRangeFinder.getInstance();
                  break;
               case ("Closest"):
                  fw.writeLine("making r closest");
                  mHomeRangeFinder = ClosestHomeRangeFinder.getInstance();
                  break;
               case ("Combo"):
                  fw.writeLine("making r combo");
                  mHomeRangeFinder = BestComboHomeRangeFinder.getInstance();
                  break;
               default:
                  throw new ArgumentException("Unexpected Home Range Criteria sent in type = " + type);
            }
            fw.writeLine("now looping and setting the animals");
            foreach (Animal a in this.myDispersers)
            {

               a.HomeRangeFinder = mHomeRangeFinder;
            }
            //if we get this far everything worked out fine
            success = true;

         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return success;
      }

      public void setHomeRangeTrigger(string type, int num)
      {
         try
         {


            fw.writeLine("inside setHomeRangeTrigger want to create " + type + " for " + num.ToString());
            switch (type)
            {
               case "SITES":
                  mHomeRangeTrigger = new SiteHomeRangeTrigger(num, this.myDispersers);
                  fw.writeLine("making r new site home ranger");
                  break;
               case "STEPS":
                  mHomeRangeTrigger = new TimeHomeRangeTrigger(num, this.myDispersers);
                  fw.writeLine("making r new step home ranger");
                  break;
               default:
                  throw new System.Exception("Not r valid home range trigger");
            }
            //now set the trigger to the animals
            fw.writeLine("now set the triggers to the animals");

            foreach (Animal a in myDispersers)
            {
               a.HomeRangeTrigger = this.mHomeRangeTrigger;
            }
            fw.writeLine("done leaving setHomeRangeTrigger");
         }
         catch (System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      public bool setModifiers()
      {
         bool success = false;
         try
         {
            fw.writeLine("inside set gender modifier in Animal Modifier");
            foreach (Animal a in myDispersers)
            {
               if (a.GetType().Name == "Male")
                  a.GenderModifier = this.MaleModifier;
               else if (a.GetType().Name == "Female")
                  a.GenderModifier = this.FemaleModifier;

               a.StateModifer = this.SafeSearchMod;
            }
            fw.writeLine("done setting all the modifiers");
            success = true;
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return success;
      }

      public void setResidentModifierValues(double inTimeStepRisk, double inYearlyRisk,
         double inPercentBreed, double inPercentFemale, double inMeanLitterSize, double inSDLitterSize)
      {
         try
         {
            fw.writeLine("inside animal Manager setResidentModifierValues inSDLitterSize is" + inSDLitterSize.ToString());
            Check.Require(inTimeStepRisk >= 0, "Resident Time Step Risk less then zero");
            Check.Require(inYearlyRisk >= 0, "Resident Yearly Risk less then zero");
            Check.Require(inPercentBreed >= 0, "Resident chance of breeding less then zero");
            Check.Require(inPercentFemale >= 0, "Resident chance of having female offspring less then zero");
            Check.Require(inMeanLitterSize >= 0, "Resident mean litter size less then zero");
            Check.Require(inSDLitterSize >= 0, "Resident sd litter size less then zero");
            mResidentAttributes = new ResidentAttributes(inTimeStepRisk, inYearlyRisk, inPercentBreed, inPercentFemale, inMeanLitterSize, inSDLitterSize, this.mAnimalAttributes.OutPutDir);
            this.setResidentAttributes();


         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      public void setResidentTextWriters(string currYear)
      {
         foreach (Resident r in myResidents)
            r.BuildTextWriter(currYear);
      }

      public bool setSleepTime(DateTime currTime)
      {
         string CurrYear = currTime.Year.ToString();
         bool success = true;
         try
         {
            List<Animal> myDispersers = this.getDispersers();
            foreach (Animal a in myDispersers)
            {
               a.setInitialSleepTime(currTime);
               a.BuildTextWriter(CurrYear, this.AnimalAttributes.OutPutDir);
               SetMapValues(a, currTime);
            }

         }
         catch (System.Exception ex)
         {
            success = false;
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
            Process.GetCurrentProcess().Kill();
         }
         fw.writeLine("leaving animal manager set sleep time with r value of " + success.ToString());
         return success;

      }

      public void winterKillResidents(Map currSocialMap)
      {
         try
         {
            fw.writeLine("inside winterKillResidents");
            fw.writeLine("going to loop through " + this.myResidents.Count.ToString() + " in the collection.");
            for (int i = 0; i < myResidents.Count; i++)
            {
               fw.writeLine("so we are going to call the resident winter kill method");
               myResidents[i].winterKill();
               fw.writeLine("after calling resident winter kill the animal is dead = " + myResidents[i].IsDead.ToString());
               if (myResidents[i].IsDead)
               {
                  fw.writeLine("Well he died r glorious death but now he is just r dead animal ");
                  this.AdjustMapForDeadResident(currSocialMap, myResidents[i]);
                  myResidents.RemoveAt(i);
               }
            }

            fw.writeLine("there are " + this.myResidents.Count.ToString() + " animals left.");
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

		#endregion Public Methods 
		#region Private Methods (15) 

      private void AdjustMapForDeadDisperser(Map inSocialMap, Animal a)
      {
         
         string fieldName;
         if (a.Sex.ToLower() == "male")
            fieldName = "OCCUP_MALE";
         else
            fieldName = "OCCUP_FEMA";
         fw.writeLine("calling resetFields");
         inSocialMap.resetFields(fieldName, a.IdNum.ToString(), "none");
      }

      private void AdjustMapForDeadResident(Map inSocialMap, Resident r)
      {

         string fieldName;
         if (r.Sex.ToLower() == "male")
            fieldName = "OCCUP_MALE";
         else
            fieldName = "OCCUP_FEMA";
         fw.writeLine("calling resetFields");
         inSocialMap.resetFields(fieldName, r.OriginalID.ToString(), "none");
      }

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
               if (s.IndexOf("AnimalManagerLogPath") == 0)
               {
                  fw = FileWriter.FileWriter.getAnimalManagerLogger(s.Substring(s.IndexOf(" ")));
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

      private void doDisperserTimeStep(HourlyModifier inHM, DailyModifier inDM, ref DateTime currTime, bool DoTextOutPut, Map currSocialMap)
      {
         fw.writeLine("in inside doDisperserTimeStep");
         
         for(int i=0; i<myDispersers.Count; i++)
         {
            string status = "";
            fw.writeLine("calling " + myDispersers[i].IdNum.ToString() + " to do the time step");
            myDispersers[i].doTimeStep(inHM, inDM, currTime, DoTextOutPut, ref status);
            //check to see if they died if they did remove them from the list
            if (myDispersers[i].IsDead)
            {
               fw.writeLine("poor guy died for some reason so remove him");
               myDispersers.RemoveAt(i);
            }
            //check to see if they are changing from disperser to resident

            if (status == "resident")
            {
               fw.writeLine("switching " + myDispersers[i].IdNum.ToString() + " to r resident");
               Resident r = new Resident();
               r.Sex = myDispersers[i].Sex;
               r.OriginalID = myDispersers[i].IdNum.ToString();
               r.TextFileWriter = myDispersers[i].TextFileWriter;
               r.HomeRangeCenter = myDispersers[i].HomeRangeCenter;
               r.HomeRangeCriteria = myDispersers[i].HomeRangeCriteria;
               r.MyAttributes = this.ResidentAttributes;
               r.MyAttributes.OriginalID = myDispersers[i].IdNum.ToString();
               this.myDispersers.RemoveAt(i);
               this.myResidents.Add(r);
               status = "";

            }
            
         }
      }

      private void doResidentTimeStep()
      {
         foreach (Resident r in myResidents)
         {
            string status = String.Empty;
            r.doTimeStep(ref status);
            if (status == "dead FROM ROLL OF DICE")
            {
               myResidents.Remove(r);
               MapManager mm = MapManager.GetUniqueInstance();
               this.AdjustMapForDeadResident(mm.SocialMap, r);
            }
         }
      }

      private List<Animal> getDisperserBySex(string inSex)
      {

         var sexBased = from a in myDispersers where a.Sex.Equals(inSex, StringComparison.CurrentCultureIgnoreCase) select a;
         return sexBased.ToList<Animal>();

      }

      //private List<Animal> getAllLiveAnimals()
      //{
      //   List<Animal> temp = this.getDispersers();
      //   var aliveResidents = from r in myResidents where !r.IsDead select r;
      //   List<Resident> tempR = aliveResidents.ToList<Resident>();
      //   temp.AddRange(tempR);
      //   return temp;

      //}
      private List<Animal> getDispersers()
      {
         var aliveDispersers = from a in myDispersers where a.IsDead == false select a;
         return aliveDispersers.ToList<Animal>();
      }

      private List<Resident> getResidentsBySex(string inSex)
      {
         var sexBased = from r in myResidents where r.Sex.Equals(inSex, StringComparison.CurrentCultureIgnoreCase) select r;
         return sexBased.ToList<Resident>();
      }

      private void makeNextGenAnimal(InitialAnimalAttributes inIAA, DateTime currTime)
      {
         Animal tmpAnimal = null;
         string mapPath = AnimalAttributes.MapPath + "\\" + currTime.Year.ToString();
         fw.writeLine("inside make next gen animal");
         try
         {
            for (int i = 0; i < inIAA.NumToMake; i++)
            {
               if (inIAA.Sex == 'M')
               {
                  fw.writeLine("makeing r new male");
                  tmpAnimal = new Male();
                  tmpAnimal.GenderModifier = this.mMaleModifier;
               }
               else
               {
                  fw.writeLine("makeing r new female");
                  tmpAnimal = new Female();
                  tmpAnimal.GenderModifier = this.mFemaleModifier;
               }

               fw.writeLine("just made " + tmpAnimal.IdNum.ToString());
               tmpAnimal.Location = inIAA.Location;
               tmpAnimal.AnimalAtributes = this.AnimalAttributes;
               tmpAnimal.CurrEnergy = this.AnimalAttributes.InitialEnergy;
               fw.writeLine("now setting the mover");
               tmpAnimal.myMover = this.mMover;
               tmpAnimal.StateModifer = this.SafeSearchMod;
               tmpAnimal.AnimalManager = this;
               tmpAnimal.HomeRangeTrigger = this.mHomeRangeTrigger;
               tmpAnimal.HomeRangeFinder = this.mHomeRangeFinder;
               tmpAnimal.setInitialSleepTime(currTime);
               this.SetMapValues(tmpAnimal, currTime);
               tmpAnimal.BuildTextWriter(currTime.Year.ToString());
               tmpAnimal.dump();
               this.myDispersers.Add(tmpAnimal);
               MapManager.GetUniqueInstance().makeOneNewAnimalMap(tmpAnimal.IdNum,mapPath);
            }

         }


         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      /// <summary>
      /// After changing the social map, the animals location will stay the same, 
      /// but the social index will need to be reset.
      /// </summary>
      /// <param name="inSocialMap">The new social map to pull the new index from</param>
      private void resetDisperserSocialIndex(Map inSocialMap)
      {

         foreach (Animal d in myDispersers)
         {
            d.SocialIndex = inSocialMap.getCurrentPolygon(d.Location);
         }

      }

      /// <summary>
      /// after changing out the social map we need to see if the current residents
      /// still have enough area to survive.
      /// </summary>
      /// <param name="newSocialMap">The new map to check area against</param>
      private void resetResidentsHomeRange(Map newSocialMap)
      {
         try
         {
            MapManager mm = MapManager.GetUniqueInstance();
            fw.writeLine("inside resetResidentsHomeRange we are going to loop through " + myResidents.Count.ToString() + " residents");
            foreach (Resident resident in myResidents)
            {
               fw.writeLine("my animals location is " + resident.HomeRangeCenter.X.ToString() + " " + resident.HomeRangeCenter.Y.ToString());
               fw.writeLine("now going to try and build r home range with the map manager");
               if (mm.BuildHomeRange(resident))
               {
                  fw.writeLine("built it ");
               }
               else
               {
                  fw.writeLine("could not create one");
                  resident.TextFileWriter.addLine("Not enough suitable habitat after map switch.");
                  fw.writeLine("calling changeToDeadAnimal");
                  resident.IsDead = true;
                  AdjustMapForDeadResident(newSocialMap, resident);
               }
            }
            fw.writeLine("leaving resetResidentsHomeRange");
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      private void setFemaleHomeRange()
      {
         List<Animal> females = this.getDisperserBySex("female");
         foreach (Animal female in females)
         {
            female.HomeRangeCriteria = this.mFemaleHomeRangeCriteria;
         }
         List<Resident> femaleResidents = this.getResidentsBySex("female");
         foreach (Animal female in femaleResidents)
         {
            female.HomeRangeCriteria = this.mFemaleHomeRangeCriteria;
         }


      }

      private void setMaleHomeRange()
      {
         List<Animal> males = this.getDisperserBySex("male");
         foreach (Animal male in males)
         {
            male.HomeRangeCriteria = this.mMaleHomeRangeCriteria;
         }
         List<Resident> maleResidents = this.getResidentsBySex("male");
         foreach (Resident male in maleResidents)
         {
            male.HomeRangeCriteria = this.mMaleHomeRangeCriteria;
         }
      }

      private void SetMapValues(Animal a, DateTime currTime)
      {
         fw.writeLine("inside SetMapValues for " + a.IdNum.ToString());
         this.mMapValues = new Dictionary<IPoint, MapValue>();
         fw.writeLine("Now check to see if we have the value for this location or not");
         if (this.mMapValues.ContainsKey(a.Location))
         {
            fw.writeLine("had it so set the values");

            a.CaptureFood = this.mMapValues[a.Location].CaptureFood;
            a.FoodMeanSize = this.mMapValues[a.Location].FoodMeanSize;
            a.MoveSpeed = this.mMapValues[a.Location].MoveSpeed;
            a.MoveTurtosity = this.mMapValues[a.Location].MoveTurtosity;
            a.PerceptonModifier = this.mMapValues[a.Location].PerceptonModifier;
            a.PredationRisk = this.mMapValues[a.Location].PredationRisk;
            a.FoodIndex = this.mMapValues[a.Location].FoodIndex;
            a.MoveIndex = this.mMapValues[a.Location].MoveIndex;
            a.RiskIndex = this.mMapValues[a.Location].RiskIndex;
            a.SocialIndex = this.mMapValues[a.Location].SocialIndex;
         }
         else
         {
            fw.writeLine("did not have it, so go get them from the animal");
            a.setInitialValues(currTime);
            fw.writeLine("now store them off");
            MapValue mv = new MapValue();
            mv.CaptureFood = a.CaptureFood;
            mv.CurrLocation = a.Location;
            mv.FoodMeanSize = a.FoodMeanSize;
            mv.FoodSD_Size = a.FoodSD_Size;
            mv.MoveSpeed = a.MoveSpeed;
            mv.MoveTurtosity = a.MoveTurtosity;
            mv.PerceptonModifier = a.PerceptonModifier;
            mv.PredationRisk = a.PredationRisk;
            mv.FoodIndex = a.FoodIndex;
            mv.MoveIndex = a.MoveIndex;
            mv.RiskIndex = a.RiskIndex;
            mv.SocialIndex = a.SocialIndex;
            fw.writeLine("add to the list");
            this.mMapValues.Add(a.Location, mv);
         }
         return;
      }

      private void setNextGenHomeRange()
      {
         setMaleHomeRange();
         setFemaleHomeRange();

      }

      private void setResidentAttributes()
      {
         foreach (Resident r in myResidents)
            r.MyAttributes = this.ResidentAttributes;
      }

		#endregion Private Methods 

		#endregion Methods 
   }
}
