/******************************************************************************
 * Author:        Bob Cummings
 * Name:          AnimalManager
 * Description:   Manages the animals as they work through the simulation. Everything
 *                from taking a step, to breeding, just about all of it.
 * ****************************************************************************
 * Author:        Bob Cummings
 * Modify Date    Sunday, November 08, 2009
 * Description    2 years into changing thought time to start.  For issue
 *                59 the issue was during the current time step the current
 *                social map reference was not being updated when a new 
 *                resident was added.  Therefore when trying to remove a resident
 *                in the same time step there was not a correct refernce to 
 *                social map.
 * ***************************************************************************/


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
		#region�Constructors�(1)�

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
         this.myAnimals = new List<Animal>();
         currNumAnimals = 0;
   

      }

		#endregion�Constructors�

		#region�Fields�(17)�

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
      private List<Animal> myAnimals;
      private int currNumAnimals;
      

      public int getNumDispersers()
      {
         return getDispersers().Count;
      }

      public int getNumResidents()
      {
         return getResidents().Count;
      }

		#endregion�Fields�

		#region�Properties�(9)�

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
            fw.writeLine("inside animal manager setting the mFemaleModifierMod"); }
      }

      public MaleModifier MaleModifier
      {
         get { return mMaleModifier; }
         set 
         {
            mMaleModifier = value;
            fw.writeLine("inside animal manager setting the mMaleModifierMod"); }
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
            fw.writeLine("inside animal manager setting the mRiskyForageMod"); }
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
            fw.writeLine("inside animal manager setting the mSafeForageMod"); }

      }

      public Modifier SafeSearchMod
      {
         get { return mSafeSearchMod; }
         set 
         {
            mSafeSearchMod = value;
            fw.writeLine("inside animal manager setting the mSafeSearchMod"); }
      }

		#endregion�Properties�

		#region�Methods�(32)�

		#region�Public�Methods�(20)�

      public void addNewDispersers(InitialAnimalAttributes[] inIAA, DateTime currTime)
      {
         try
         {
            fw.writeLine("inside addNewDispersers starting loop");
            for (int i = 0; i <= inIAA.Length - 1; i++)
            {
               fw.writeLine("calling makeNextGenAnimal");
               this.makeNextGenAnimal(inIAA[i],currTime);
            }
            fw.writeLine("done with loop in addNewDispersers calling mHomeRangeTrigger.reset");
            this.mHomeRangeTrigger.reset(this.myAnimals.Count + 1);
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

      public int breedFemales(DateTime currTime)
      {
         fw.writeLine("inside breed females");
         int numMales=0;
         int numFemales=0;
         int totalNewAnimals = 0;
         InitialAnimalAttributes iaa;
         fw.writeLine("starting the loop");
         List<Resident> myBreeders = this.getResidents("female");

         foreach (Resident r in myBreeders)
         {

            r.breed(out numMales, out numFemales);
            totalNewAnimals += numFemales + numMales;
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
         

         this.mHomeRangeTrigger.reset(this.myAnimals.Count + 1);
         setNextGenHomeRange();
         return totalNewAnimals;

      }

      public void changeToDeadAnimal(Animal inA)
      {
         try
         {
            fw.writeLine("inside changeToDeadAnimal for animal num " + inA.IdNum);
            DeadAnimal dd = new DeadAnimal(inA);
            fw.writeLine("removing at position " + dd.IdNum.ToString());
            this.myAnimals.RemoveAt(dd.IdNum);
            this.myAnimals.Insert(dd.IdNum, dd);
                  
            fw.writeLine("new animal type is " + dd.GetType());
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
         
         //List<Animal> liveAnimals = this.getAllLiveAnimals();
         Animal a;
         string status = "";
         try
         {
            fw.writeLine("inside animal manager do time step with modifiers");
           // foreach (Animal a in myAnimals)
            for(int i=0;i<myAnimals.Count; i++)
            {
               a = this.myAnimals[i];
               fw.writeLine("");
               fw.writeLine("animal id = " + a.IdNum.ToString());
               fw.writeLine("animal is type " + a.GetType().ToString());

//               if (a.GetType().ToString().Equals("SEARCH.Resident", StringComparison.CurrentCultureIgnoreCase))
//               {
//                  int bob = 0;
//               }

                a.doTimeStep(inHM, inDM, currTime, DoTextOutPut, ref status);
               //check to see if they died if they did remove them from the list
               if (a.IsDead)
               {
                 this.changeToDeadAnimal(a);
                 this.AdjustMapForDeadAnimal(currSocialMap, a);
               }
               //check to see if they are changing from disperser to resident
               
               if (status == "resident")
               {
                  fw.writeLine("switching " + a.IdNum.ToString() + " to a resident");
                  Resident r = new Resident();
                  r.Sex = a.Sex;
                  r.IdNum = a.IdNum;
                  r.TextFileWriter = a.TextFileWriter;
                  r.HomeRangeCenter = a.HomeRangeCenter;
                  r.HomeRangeCriteria = a.HomeRangeCriteria;
                  r.MyAttributes = this.ResidentAttributes;
                  this.myAnimals.RemoveAt(i);
                  this.myAnimals.Insert(i, r);
                  status = "";
                  //Sunday, November 08, 2009
                  //update reference to current social map.
                  currSocialMap = MapManager.GetUniqueInstance().SocialMap;
               }
              
            }
            }
         
        
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
        
      }

      public void dump()
      {
         foreach (Animal a in myAnimals)
         {
            a.dump();
         }
      }

     

      

      public StringCollection getResidentIDs()
      {  
         System.Collections.Specialized.StringCollection sc = new System.Collections.Specialized.StringCollection();
         try
         {
            foreach (Animal a in this.myAnimals)
            {
               if (a.GetType().Name == "Resident")
               {
                  sc.Add(a.IdNum.ToString());
               }
            }

         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return sc;
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
                     //fw.writeLine("makeing a new male");
                     tmpAnimal = new Male();
                  }
                  else
                  {
                     fw.writeLine("makeing a new female");
                     tmpAnimal = new Female();
                  }

                  tmpAnimal.IdNum = this.currNumAnimals++;
                  fw.writeLine("just made " + tmpAnimal.IdNum.ToString());
                  tmpAnimal.Location = inIAA[i].Location;
                  
                  tmpAnimal.AnimalAtributes = this.AnimalAttributes;
                  fw.writeLine("now setting the mover");
                  tmpAnimal.myMover = this.mMover;
                  tmpAnimal.AnimalManager = this;
                  fw.writeLine("now adding to the list of my animals;");
                  this.myAnimals.Add(tmpAnimal);
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

      public bool makeResidents(InitialAnimalAttributes[]inResAttributes,string currYear)
      {
         bool success = false;
         Resident r;
         fw.writeLine("inside makeResidents going to make " + inResAttributes.Length.ToString());
         fw.writeLine("total animals now is " + this.myAnimals.Count.ToString());
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
               //r.IsDead = false;
               r.IdNum = this.currNumAnimals++;
               r.Location = inResAttributes[i].Location;
               r.HomeRangeCenter = r.Location as PointClass; ;
               r.MyAttributes.OriginalID = inResAttributes[i].OrginalID;
                
               this.myAnimals.Add(r);
            }
            
            fw.writeLine("after adding residents total animals now is " + this.myAnimals.Count.ToString());
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
            fw.writeLine("going to loop through " + this.myAnimals.Count.ToString() + " in the collection.");
            foreach (Animal a in poorSuckers)
            {
               {
                  fw.writeLine("so we are setting isDead to true");
                  if (a.TextFileWriter != null)
                     a.TextFileWriter.addLine("Did not establish a home range so died during winter kill");
                  a.IsDead = true;
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
            List<Animal> a = this.getDispersers();
            foreach (Animal temp in a)
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
               case("Food") : 
                  fw.writeLine("making a foodie");
                  mHomeRangeFinder = BestFoodHomeRangeFinder.getInstance();
                  break;
               case("Risk") : 
                  fw.writeLine("making a risky");
                  mHomeRangeFinder = BestRiskHomeRangeFinder.getInstance();
                  break;
               case("Closest") : 
                  fw.writeLine("making a closest");
                  mHomeRangeFinder = ClosestHomeRangeFinder.getInstance();
                  break;
               case("Combo") : 
                  fw.writeLine("making a combo");
                  mHomeRangeFinder = BestComboHomeRangeFinder.getInstance();
                  break;
               default : 
                  throw new ArgumentException("Unexpected Home Range Criteria sent in type = " + type);
            }
            fw.writeLine("now looping and setting the animals");
            foreach (Animal a in this.myAnimals)
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
               case "SITES" : 
                  mHomeRangeTrigger = new SiteHomeRangeTrigger(num, this.myAnimals.Count);
                  fw.writeLine("making a new site home ranger");
                  break;
               case "STEPS" :
                  mHomeRangeTrigger = new TimeHomeRangeTrigger(num, this.myAnimals.Count);
                  fw.writeLine("making a new step home ranger");
                  break;
               default : 
                  throw new System.Exception("Not a valid home range trigger");
            }
            //now set the trigger to the animals
            fw.writeLine("now set the triggers to the animals");
            
            foreach(Animal a in myAnimals)
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
            foreach (Animal a in myAnimals)
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
            mResidentAttributes = new ResidentAttributes(inTimeStepRisk, inYearlyRisk, inPercentBreed, inPercentFemale, inMeanLitterSize, inSDLitterSize);
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
         fw.writeLine("leaving animal manager set sleep time with a value of " + success.ToString());
         return success;

      }

      public void winterKillResidents(Map currSocialMap, int currentTime)
      {
         try
         {
            
            fw.writeLine("inside winterKillResidents");
            fw.writeLine("going to loop through " + this.myAnimals.Count.ToString() + " in the collection.");

            List<Resident> res = this.getResidents();
            foreach (Resident r in res)
            {
               fw.writeLine("so we are going to call the resident winter kill method");
               r.winterKill(currentTime);
               fw.writeLine("after calling resident winter kill the animal is dead = " + r.IsDead.ToString());
               if (r.IsDead)
               {
                  fw.writeLine("Well he died a glorious death but now he is just a dead animal ");
                  this.changeToDeadAnimal(r);
                  this.AdjustMapForDeadAnimal(currSocialMap, r);

               } 
            }
            
            fw.writeLine("there are " + this.myAnimals.Count.ToString() + " animals left.");
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

		#endregion�Public�Methods�
		#region�Private�Methods�(12)�

      private void AdjustMapForDeadAnimal(Map inSocialMap, Animal a)
      {
         string fieldName;
         try
         {
            if (a.Sex.ToLower() == "male")
               fieldName = "OCCUP_MALE";
            else
               fieldName = "OCCUP_FEMA";
            fw.writeLine("calling resetFields");
            inSocialMap.resetFields(fieldName, a.IdNum.ToString(), "none");
         }
         catch (Exception ex)
         {

            FileWriter.FileWriter.WriteErrorFile(ex);
            FileWriter.FileWriter.AddToErrorFile(inSocialMap.mySelf.AliasName);
            FileWriter.FileWriter.AddToErrorFile(a.IdNum.ToString());

         }
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

      private List<Animal> getAllLiveAnimals()
      {
         var temp = from a in myAnimals where a.IsDead == false select a;
         return temp.ToList<Animal>();
      }
 
      private List<Animal> getDispersers()
      {
         List<Animal> dispersers = new List<Animal>();
         foreach (Animal a in this.myAnimals)
         {
            if (a.GetType().Name != "Resident" && a.IsDead == false)
            {
               dispersers.Add(a);
            }
         }
         return dispersers;

      }
       private List<Animal> getDispersers(string inSex)
      {
         var temp = from a in myAnimals where a.GetType().Name.Equals(inSex, StringComparison.CurrentCultureIgnoreCase)  && !a.IsDead select a;
         return temp.ToList<Animal>();
      }
      private List<Resident> getResidents(string inSex)
      {

         var temp = from a in myAnimals where a.GetType().Name.Equals("resident", StringComparison.CurrentCultureIgnoreCase) && !a.IsDead && a.Sex.Equals(inSex, StringComparison.CurrentCultureIgnoreCase) select a as Resident;
         return temp.ToList<Resident>();
      }
      private List<Resident> getResidents()
      {

         List<Resident> r = new List<Resident>();
         int count = 0;
         try
         {
            foreach (Animal a in this.myAnimals)
            {
               if (a.GetType().Name == "Resident" && a.IsDead == false)
               {
                  r.Add(a as Resident);
               }
            }
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return r;
      }

      private void makeNextGenAnimal(InitialAnimalAttributes inIAA, DateTime currTime)
      {
         Animal tmpAnimal = null;
         fw.writeLine("inside make next gen animal");
         try
         {
            for (int i = 0; i < inIAA.NumToMake; i++)
            {
               if (inIAA.Sex == 'M')
               {
                  fw.writeLine("makeing a new male");
                  tmpAnimal = new Male();
                  tmpAnimal.GenderModifier = this.mMaleModifier;
               }
               else
               {
                  fw.writeLine("makeing a new female");
                  tmpAnimal = new Female();
                  tmpAnimal.GenderModifier = this.mFemaleModifier;
               }
                  
               tmpAnimal.IdNum = this.currNumAnimals++;
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
               this.SetMapValues(tmpAnimal,currTime);
               tmpAnimal.BuildTextWriter(currTime.Year.ToString());
               tmpAnimal.dump();
               this.myAnimals.Add(tmpAnimal);
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
         List<Animal> dispersers = this.getDispersers();

         foreach (Animal d in dispersers)
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
            List<Resident> r = this.getResidents();
            fw.writeLine("inside resetResidentsHomeRange we are going to loop through " + r.Count.ToString() + " residents");
            foreach (Resident a in r)
            {
               fw.writeLine("my animals location is " + a.HomeRangeCenter.X.ToString() + " " + a.HomeRangeCenter.Y.ToString());
               fw.writeLine("now going to try and build a home range with the map manager");
               if (mm.BuildHomeRange(a))
               {
                  fw.writeLine("built it ");
               }
               else
               {
                  fw.writeLine("could not create one");
                  a.TextFileWriter.addLine("Not enough suitable habitat after map switch.");
                  fw.writeLine("calling changeToDeadAnimal");
                  this.changeToDeadAnimal(a);
                  fw.writeLine("new animal type is " + a.GetType());
                  //now reset the social map to not occupied for that resident.
                  AdjustMapForDeadAnimal(newSocialMap, a);
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
         List<Animal> females = this.getDispersers("female");
         foreach (Animal female in females)
         {
            female.HomeRangeCriteria = this.mFemaleHomeRangeCriteria;
         }
         List<Resident> femaleResidents = this.getResidents("female");
         foreach (Animal female in femaleResidents)
         {
            female.HomeRangeCriteria = this.mFemaleHomeRangeCriteria;
         }


      }

      private void setMaleHomeRange()
      {
         List<Animal> males = this.getDispersers("male");
         foreach (Animal male in males)
         {
            male.HomeRangeCriteria = this.mMaleHomeRangeCriteria;
         }
           List<Resident> maleResidents = this.getResidents("male");
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

      public void setResidentsTextOutput(string path, string year)
      {
         List<Resident> res = this.getResidents();
         foreach(Resident r in res)
            r.BuildTextWriter(year,path + "\\Resident");
      }

      private void setResidentAttributes()
      {
         List<Resident> rs = getResidents();
         foreach (Resident r in rs)
            r.MyAttributes = this.ResidentAttributes;
      }

		#endregion�Private�Methods�

		#endregion�Methods�
   }
}