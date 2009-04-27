using ESRI.ArcGIS.Geometry;
using System.Collections;
using System.Collections.Specialized;
using System;
using System.IO;
using System.Diagnostics;
using DesignByContract;

namespace PAZ_Dispersal
{
   public class AnimalManager : System.Collections.ArrayList
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
       //  myMapManager = MapManager.GetUniqueInstance();
      //   mResidentAttributes = new ResidentAttributes();

      }
      
		#endregion Constructors 

      
      #region member variables
      private string myErrMessage;
      private FileWriter.FileWriter fw;
      private MaleModifier mMaleModifier;
      private FemaleModifier mFemaleModifier;
      private AnimalAtributes mAnimalAttributes;
      private Modifier mRiskySearchMod;
      private Modifier mRiskyForageMod;
      private Modifier mSafeForageMod;
      private Modifier mSafeSearchMod;
      private Mover mMover;
      private IEnumerator currAnimal;
      //private MapManager myMapManager;
      private ResidentAttributes mResidentAttributes;
      private HomeRangeCriteria mMaleHomeRangeCriteria;
      private HomeRangeCriteria mFemaleHomeRangeCriteria;
      private IHomeRangeTrigger mHomeRangeTrigger;
      private IHomeRangeFinder mHomeRangeFinder;
      #endregion

      #region public methods
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
            this.mHomeRangeTrigger.reset(this.Count + 1);
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
         try
         {
            Resident[] r = this.getResidents();
            string fieldName;
            fw.writeLine("inside adjustNewSocialMap we are going to loop through " + r.Length.ToString() + " residents");
            foreach (Animal a in r)
            {
               fw.writeLine("my type of animal is a " + a.GetType());
               fw.writeLine("my animals location is " + a.getLocation_XY());
               
               fw.writeLine(a.IdNum.ToString() + " needs " + a.HomeRangeArea.ToString() + " and the new map has " + newSocialMap.getAllAvailableArea(a.IdNum, a.Sex).ToString());
               if (a.HomeRangeArea > newSocialMap.getAllAvailableArea(a.IdNum, a.Sex))
               {
                  fw.writeLine("not enough so kill the bugger off");
                  
                  a.TextFileWriter.addLine("Not enough suitable habitat after map switch.");
                  //Saturday, February 23, 2008
                  //change them to a dead animal by removing the resident and then replacing with
                  //a dead animal.
                  fw.writeLine("calling changeToDeadAnimal");
                  //Saturday, March 15, 2008 refactored into a method to be called from 
                  //other places.
                  this.changeToDeadAnimal(a);
//                  DeadAnimal dd = new DeadAnimal();
//                  dd.IdNum = a.IdNum;
//                  dd.IsDead = false;
//                  fw.writeLine("removing at position " + dd.IdNum.ToString());
//                  this.RemoveAt(dd.IdNum);
//                  this.Insert(dd.IdNum,dd);
                  
                  fw.writeLine("new animal type is " + a.GetType());

                  //now reset the social map to not occupied for that resident.
                  if (a.Sex.ToLower() == "male")
                     fieldName = "OCCUP_MALE";
                  else
                     fieldName = "OCCUP_FEMA";
                  fw.writeLine("calling resetFields");
                  newSocialMap.resetFields(fieldName, a.IdNum.ToString(),"none");
                  fw.writeLine("back in adjustNewSocialMap");
               }
            }
            fw.writeLine("leaving adjustNewSocialMap");
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         
      }
      public void breedFemales(DateTime currTime)
      {
         fw.writeLine("inside breed females");
         int numMales;
         int numFemales;
         InitialAnimalAttributes iaa;
         Animal a;
         fw.writeLine("starting the loop");
         for (int i = 0; i < this.Count; i++)
         {  
            
            a = this[i] as Animal;
            fw.writeLine(a.IdNum.ToString() + " is dead = " + a.IsDead.ToString());
            if (!a.IsDead)
            {
               fw.writeLine(a.IdNum.ToString() + " is  a " + a.GetType().Name + " with a sex of " + a.Sex);
           
               if (a.GetType().Name == "Resident" && a.Sex == "Female")
               {
                  fw.writeLine("so recast to Resident and breed");
                  Resident r = a as Resident;
                  
                  r.breed(out numMales, out numFemales);
                  if (numMales > 0)
                  {
                     iaa = new InitialAnimalAttributes();
                     iaa.Location = a.HomeRangeCenter;
                     iaa.Sex = 'M';
                     iaa.NumToMake = numMales;
                     this.makeNextGenAnimal(iaa, currTime);
                  }
                  if (numFemales > 0)
                  {
                     iaa = new InitialAnimalAttributes();
                     iaa.Location = a.HomeRangeCenter;
                     iaa.Sex = 'F';
                     iaa.NumToMake = numFemales;
                     this.makeNextGenAnimal(iaa, currTime);
                  }
               }
            }
         }
         this.mHomeRangeTrigger.reset(this.Count + 1);
         setNextGenHomeRange();
        
      }

      public void changeToDeadAnimal(Animal inA)
      {
         try
         {
            fw.writeLine("inside changeToDeadAnimal for animal num " + inA.IdNum);
            DeadAnimal dd = new DeadAnimal();
            dd.IdNum = inA.IdNum;
            dd.IsDead = false;
            fw.writeLine("removing at position " + dd.IdNum.ToString());
            this.RemoveAt(dd.IdNum);
            this.Insert(dd.IdNum, dd);
                  
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
      public void doTimeStep(HourlyModifier inHM, DailyModifier inDM, DateTime currTime, bool DoTextOutPut)
      {
         Animal a;
         //IEnumerator e;
         
         string status = "";
         try
         {
            fw.writeLine("inside animal manager do time step with modifiers");
            for (int i = 0; i < this.Count; i++)
            {
               a = (Animal) this[i];
#if (DEBUG)
               //if (a.IdNum == 0)
#endif
               a.doTimeStep(inHM, inDM, currTime, DoTextOutPut, ref status);
               //check to see if they died if they did remove them from the list
               if (a.IsDead)
               {
                 this.changeToDeadAnimal(a);
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
                  r.HomeRangeArea = a.HomeRangeArea;
                  r.MyAttributes = this.ResidentAttributes;
                  r.MyAttributes.OriginalID = a.IdNum.ToString();
                  this.RemoveAt(i);
                  this.Insert(i, r);
                  status = "";
               }
            }
         }
        
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
        
      }
      
      public int getNumDispersers()
      {
         int num = 0;
         try
         {
           num = this.Count - this.getNumResidents();

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
      public int getNumResidents()
      {
         int num = 0;
         try
         {
            foreach (Animal a in this)
            {
               if (a.GetType().Name == "Resident")
                  num++;
            }

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

      public StringCollection getResidentIDs()
      {  
         System.Collections.Specialized.StringCollection sc = new System.Collections.Specialized.StringCollection();
         try
         {
            foreach (Animal a in this)
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
         int id = 0;
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
                  
                  tmpAnimal.IdNum = id++;
                  fw.writeLine("just made " + tmpAnimal.IdNum.ToString());
                  tmpAnimal.Location = inIAA[i].Location;
                  
                  tmpAnimal.AnimalAtributes = this.AnimalAttributes;
                  fw.writeLine("now setting the mover");
                  tmpAnimal.myMover = this.mMover;
                  tmpAnimal.StateModifer = this.SafeSearchMod;
                  tmpAnimal.AnimalManager = this;
                  fw.writeLine("now adding to the list of my animals;");
                  this.Add(tmpAnimal);
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
      public bool makeResidents(InitialAnimalAttributes[]inResAttributes)
      {
         bool success = false;
         Resident r;
         fw.writeLine("inside makeResidents going to make " + inResAttributes.Length.ToString());
         fw.writeLine("total animals now is " + this.Count.ToString());
         try
         {
            for (int i = 0; i < inResAttributes.Length; i++)
            {
               r = new Resident();
               r.MyAttributes = new ResidentAttributes();
               if (inResAttributes[i].Sex.ToString().ToUpper() == "M")
                  r.Sex = "Male";
               else
                  r.Sex = "Female";

               r.IdNum = this.Count + 1;
               r.Location = inResAttributes[i].Location;
               r.HomeRangeCenter = r.Location;
               r.MyAttributes.OriginalID = inResAttributes[i].OrginalID;
               this.Add(r);
            }
            
            fw.writeLine("after adding residents total animals now is " + this.Count.ToString());
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
            Animal a;
            fw.writeLine("inside remove remaining dispersers");
            fw.writeLine("going to loop through " + this.Count.ToString() + " in the collection.");
            for (int i = this.Count - 1; i >= 0; i--)
            {
               a = (Animal) this[i];
               fw.writeLine("Animal number " + i.ToString() + " is a " + a.GetType().Name);
               if (a.GetType().Name != "Resident" && a.IsDead == false)
               {
                  fw.writeLine("so we are setting isDead to true");
                  if (a.TextFileWriter != null)
                     a.TextFileWriter.addLine("Died during winter kill");
                  a.IsDead = true;
               }
            }
            fw.writeLine("there are " + this.Count.ToString() + " animals left.");
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
         Animal a;
         try
         {
            currAnimal = this.GetEnumerator();
            if (currAnimal != null)
            {
               while (currAnimal.MoveNext())
               {
                  a = (Animal) currAnimal.Current;
                  if (a.GetType().Name != "Resident")
                  {
                     a.AnimalAtributes = this.AnimalAttributes;
                     a.CurrEnergy = this.AnimalAttributes.InitialEnergy;
                  }
               }
            }
            else
            {
               success = false;
               this.ErrMessage = "No animals to set values to";
            }
         }
         catch (System.Exception ex)
         {
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
     
      public bool setGenderModifiers()
      {
         Animal a;
         IEnumerator e;
         bool success = false;
         try
         {
            fw.writeLine("inside set gender modifier in Animal Modifier");
            e = this.GetEnumerator();
            while (e.MoveNext())
            {
               a = (Animal) e.Current;
               
               if (a.GetType().Name == "Male")
                  a.GenderModifier = this.MaleModifier;
               else if (a.GetType().Name == "Female")
                  a.GenderModifier = this.FemaleModifier;
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
            foreach (Animal a in this)
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
            Animal a;
            
            fw.writeLine("inside setHomeRangeTrigger want to create " + type + " for " + num.ToString());
            switch (type)
            {
               case "SITES" : 
                  mHomeRangeTrigger = new SiteHomeRangeTrigger(num, this.Count);
                  fw.writeLine("making a new site home ranger");
                  break;
               case "STEPS" : 
                  mHomeRangeTrigger = new TimeHomeRangeTrigger(num, this.Count);
                  fw.writeLine("making a new step home ranger");
                  break;
               default : 
                  throw new System.Exception("Not a valid home range trigger");
            }
            //now set the trigger to the animals
            fw.writeLine("now set the triggers to the animals");
            currAnimal = this.GetEnumerator();
            Check.Require(currAnimal != null, "No animals to assign a trigger to");
            
            while (currAnimal.MoveNext())
            {
               a = (Animal) currAnimal.Current;
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
      
      public void setInitialValues()
      {
         try
         {

         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
      }
      public bool setSleepTime(DateTime currTime)
      {
         Animal a;
         Animal b = new Animal();
         
         bool success = true;
         fw.writeLine("inside animal manager calling set sleep time");
         try
         {
            currAnimal = this.GetEnumerator();
            if (currAnimal != null)
            {
               while (currAnimal.MoveNext())
               {
                  a = (Animal) currAnimal.Current;
                  if (a.GetType().Name != "Resident" && !a.IsDead)
                  {
                     if (a.Location == b.Location)
                     {
                        a.MoveIndex = b.MoveIndex;
                     }
                     a.setInitialValues(currTime);
                     b = a;
                  }
               }
            }
            else
            {
               success = false;
               this.ErrMessage = "No animals to set values to";
            }
         }
         catch (System.Exception ex)
         {
         //   System.Windows.Forms.MessageBox.Show("Error look for error file");
            FileWriter.FileWriter.WriteErrorFile(ex);
            Process.GetCurrentProcess().Kill();
         }
         fw.writeLine("leaving animal manager set sleep time with a value of " + success.ToString());
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


      public void winterKillResidents()
      {
         try
         {
            Animal a;
            Resident r;
            fw.writeLine("inside winterKillResidents");
            fw.writeLine("going to loop through " + this.Count.ToString() + " in the collection.");
            for (int i = 0; i < this.Count; i++)
            {
               a = (Animal) this[i];
               fw.writeLine("Animal number " + i.ToString() + " is a " + a.GetType().Name);
               if (a.GetType().Name == "Resident")
               {  
                  r = (Resident)this[i];
                  fw.writeLine("so we are going to call the resident winter kill method");
                  r.winterKill();
                  fw.writeLine("after calling resident winter kill the animal is dead = " + r.IsDead.ToString());
                  if (r.IsDead)
                  {
                     fw.writeLine("Well he died a glorious death but now he is just a dead animal ");
                     DeadAnimal da = new DeadAnimal();
                     da.IdNum = r.IdNum;
                     //HACK WHY IS NOT ALIVE
                     //da.IsDead = false;
                     this.RemoveAt(i);
                     this.Insert(i, da);
                  }
               }
            }
            fw.writeLine("there are " + this.Count.ToString() + " animals left.");
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }
      public void dump()
      {
        
         Animal a = null;
         for (int i = 0; i < this.Count; i++)
         {
            
            if (this[i].GetType().ToString() == "PAZ_Dispersal.Male")
            {
               a = (Male)this[i];
            }
            else
            {
               a = (Female)this[i];
            }
            a.dump();
         }
         fw.close();
      }
     
      #endregion

      #region private methods

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

      private Resident[] getResidents()
      {
         Resident[] r = new Resident[getNumResidents()];
         int count = 0;
         try
         {
            foreach (Animal a in this)
            {
               if (a.GetType().Name == "Resident" && a.IsDead == false)
               {
                  r[count++] = a as Resident;
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
                  
               tmpAnimal.IdNum = this.Count;
               fw.writeLine("just made " + tmpAnimal.IdNum.ToString());
               tmpAnimal.Location = inIAA.Location;
               // this sets the index for the animals move polygon
               tmpAnimal.MoveIndex = MapManager.GetUniqueInstance().getCurrMovePolygon(inIAA.Location);
               tmpAnimal.AnimalAtributes = this.AnimalAttributes;
               tmpAnimal.CurrEnergy = this.AnimalAttributes.InitialEnergy;
               fw.writeLine("now setting the mover");
               tmpAnimal.myMover = this.mMover;
               tmpAnimal.StateModifer = this.SafeSearchMod;
               tmpAnimal.AnimalManager = this;
               fw.writeLine("now setting attrbutes;");
               setAttributes();
               fw.writeLine("now setting the gender modifiers");
               setGenderModifiers();

               tmpAnimal.HomeRangeTrigger = this.mHomeRangeTrigger;
               tmpAnimal.HomeRangeFinder = this.mHomeRangeFinder;
               tmpAnimal.setInitialValues(currTime);
               tmpAnimal.dump();
               this.Add(tmpAnimal);
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
      private void setMaleHomeRange()
      {
         int numAnimals = this.Count;
         Animal tmpAnimal = null;
         

         for (int i = 0; i < numAnimals; i++)
         {
            tmpAnimal = (Animal)this[i];
            if (!tmpAnimal.IsDead)
            {
               //fw.writeLine("inside the loop checking to see if it is a male");
               //fw.writeLine("the animal id is " + tmpAnimal.IdNum.ToString());
               //fw.writeLine("the animal sex is " + tmpAnimal.GetType().ToString());

               //we know it is not a resident or the GetType would return Resident so it has to be a disperser
               if (tmpAnimal.GetType().ToString().ToUpper().IndexOf(".MALE") >= 0)
               {
                  //fw.writeLine("must have been a Male setting data");
                  tmpAnimal.HomeRangeArea = mMaleHomeRangeCriteria.Area;
                  tmpAnimal.DistanceMean = mMaleHomeRangeCriteria.DistanceMean;
                  tmpAnimal.DistanceSE = mMaleHomeRangeCriteria.DistanceSD;
                  tmpAnimal.DistanceWeight = mMaleHomeRangeCriteria.DistanceWeight;
               }
            }
         }
         //fw.writeLine("done with AnimalManager.setMaleHomeRange ");
      }

      private void setNextGenHomeRange()
      {
         setMaleHomeRange();
         setFemaleHomeRange();

      }
      private void setFemaleHomeRange()
      {
         int numAnimals = this.Count;
         Animal tmpAnimal = null;
         //fw.writeLine("inside AnimalManager.setFemaleHomeRange going to start the loop");
         for (int i = 0; i < numAnimals; i++)
         {
            tmpAnimal = (Animal)this[i];
            if (!tmpAnimal.IsDead)
            {
               //fw.writeLine("inside the loop checking to see if it is a male");
               //fw.writeLine("the animal id is " + tmpAnimal.IdNum.ToString());
               if (tmpAnimal.GetType().ToString().ToUpper().IndexOf(".FEMALE") >= 0)
               {
                  //fw.writeLine("must have been a Female setting data");
                  tmpAnimal.HomeRangeArea = this.mFemaleHomeRangeCriteria.Area;
                  tmpAnimal.DistanceMean = this.mFemaleHomeRangeCriteria.DistanceMean;
                  tmpAnimal.DistanceSE = this.mFemaleHomeRangeCriteria.DistanceSD;
                  tmpAnimal.DistanceWeight = this.mFemaleHomeRangeCriteria.DistanceWeight;
               }
            }
         }
         //fw.writeLine("done with AnimalManager.setFemaleHomeRange ");
      }
      
      private void setResidentAttributes()
      {
         Resident[] rs = getResidents();
         foreach (Resident r in rs)
            r.MyAttributes = this.ResidentAttributes;
      }
      #endregion

      #region gettersAndSetters

      public ResidentAttributes ResidentAttributes
      {
         get { return mResidentAttributes; }
         set { mResidentAttributes = value; }
      }

      public Modifier RiskySearchMod
      {
         get { return mRiskySearchMod; }
         set { mRiskySearchMod = value; }
      }

      public Modifier RiskyForageMod
      {
         get { return mRiskyForageMod; }
         set 
         {
            mRiskyForageMod = value;
            fw.writeLine("inside animal manager setting the mRiskyForageMod"); }
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



      public MaleModifier MaleModifier
      {
         get { return mMaleModifier; }
         set 
         {
            mMaleModifier = value;
            fw.writeLine("inside animal manager setting the mMaleModifierMod"); }
      }

      public FemaleModifier FemaleModifier
      {
         get { return mFemaleModifier; }
         set 
         {
            mFemaleModifier = value;
            fw.writeLine("inside animal manager setting the mFemaleModifierMod"); }
      }

      public string ErrMessage
      {
         get { return myErrMessage; }
                                 
         set { myErrMessage = value; }
      }

      public AnimalAtributes AnimalAttributes
      {
         get { return mAnimalAttributes; }
         set { mAnimalAttributes = value; }
      }

      #endregion
   }
}
