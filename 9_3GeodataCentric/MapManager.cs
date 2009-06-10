//# define pat
/******************************************************************************
 * CHANGE LOG
 * ****************************************************************************
 * Author:        Bob Cummings
 * Date:          Thursday, October 11, 2007 6:59:50 PM
 * Description:   Changed the methods GetMoveModifiers,GetRiskModifier, and 
 *                GetFoodData to only update the values passed in if the animal
 *                had changed polygons on the relevant maps.  
 ******************************************************************************
 * Author:        Bob Cummings
 * Date:          Saturday, October 13, 20078 11:23:28 AM
 * Description:   Added GetInitialMapData(Animal inA) call.  Should help alleviate
 *                some of the pain with testing.
 * ****************************************************************************
 * Author:        Bob Cummings
 * Date:          Saturday, February 23, 2008
 * Description:   Added logic to GetMoveModifiers if see if George was sitting 
 *                on the fence between 2 polygons when looking for new move parameters. 
 *                If he is then he will just hold onto his previous values.
 * ****************************************************************************
 * Author:        Bob Cummings
 * Date:          Saturday, May 02, 2009
 * Descripton:    Added the DataManipulator class to simplify some data processeing
 *                using the new features in ArcGIS 9.3.
 * ***************************************************************************/

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesFile;
using System.IO;
using DesignByContract;
using System.Collections;
using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;


namespace PAZ_Dispersal
{

   /// <summary>
   /// -The Singleton defines an Instance operation that lets clients access its unique instance. 
   /// -It may be responsible for creating its own unique instance.
   /// 
   /// 
   /// </summary>
   public class MapManager
   {

		#region Enums (1) 

      private enum ERR
      {
         WRONG_TYPE_OF_SHAPE_FILE,
         CAN_NOT_FIND_REQUIRED_FIELD,
         NO_FILES_FOUND_IN_DIRECTORY,
         DIRECTORY_ALREADY_IN_USE,
         NO_DIRECTORY_FOUND
      }

		#endregion Enums 

		#region Static Fields (1) 

private static MapManager uniqueInstance;

		#endregion Static Fields 

		#region Fields (25) 


      private AnimalMap[] myAnimalMaps;

      private string _currStepPath;
      private string errFileName;
      private string mOutMapPath;

      private int errNumber;
      private int numHomeRanges;
      private int SocialIndex;

      private DataManipulator myDataManipulator;
      private DateTime mCurrTime;
      private FileWriter.FileWriter fw;
      private Hashtable myHash;
      //  = new ShapefileWorkspaceFactory();
      private IFeatureWorkspace featureWrkSpace;
      //      public  int myMaps;
      private IWorkspace wrkSpace;
      private IWorkspaceFactory wrkSpaceFactory;
      private Map myDispersalMap = null;
      private Map myFoodMap = null;
      private Map myMoveMap = null;
      private Map myPredationMap = null;
      private Map mySocialMap = null;
      public Maps myDispersalMaps = null;
      public Maps myFoodMaps = null;
      public Maps myMoveMaps = null;
      public Maps myPredationMaps = null;
      public Maps mySocialMaps = null;
      private System.Collections.Specialized.StringCollection errMessages;

		#endregion Fields 

		#region Constructors (1) 

private MapManager()
      {
         //check on whether we are going to log or not
         this.buildLogger();
         // get the error messages ready         
         initializeErrMessages();
         myDataManipulator = new DataManipulator();
         wrkSpaceFactory = new ShapefileWorkspaceFactoryClass();
         this.myFoodMaps = new Maps("Food");
         this.myMoveMaps = new Maps("Move");
         this.myPredationMaps = new Maps("Predation");
         this.mySocialMaps = new Maps("Social");
         this.myDispersalMaps = new Maps("Dispersal");
         SocialIndex = 0;
         numHomeRanges = 0;

      }

		#endregion Constructors 

		#region Properties (4) 


      public string CurrStepPath
      {
         get { return _currStepPath; }
         set { _currStepPath = value; }
      }

      public string OutMapPath
      {
         get { return mOutMapPath; }
         set
         {
            mOutMapPath = value;
            
         }
      }


      public DateTime CurrTime
      {
         get { return mCurrTime; }
         set { mCurrTime = value; }
      }

      public Map SocialMap
      {
         get { return mySocialMap; }
         set { mySocialMap = value; }
      }


		#endregion Properties 

		#region Static Methods (2) 

      public static MapManager GetUniqueInstance()
      {

         if (uniqueInstance == null)
         {
            uniqueInstance = new MapManager();
         }
         return uniqueInstance;
      }

      public static void RemoveFiles(string FullFilePath)
      {

         string[] fileNames;
         string currDir = System.IO.Path.GetDirectoryName(FullFilePath);
         string fileName = System.IO.Path.GetFileNameWithoutExtension(FullFilePath);
         string compareFileName;
         bool success = true;
         try
         {
            fileNames = Directory.GetFiles(currDir);
            for (int i = 0; i < fileNames.Length; i++)
            {
               compareFileName =  System.IO.Path.GetFileNameWithoutExtension(fileNames[i]);
               if (compareFileName.Equals(fileName,StringComparison.CurrentCultureIgnoreCase))
               {
                  File.Delete(fileNames[i]);
               }
            }
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

		#endregion Static Methods 

		#region Private Methods (12) 

      private void AddFirstMemoryPoly(int AnimalID, IPolygon inPoly1, IPolygon inPoly2)
      {
         try
         {
            this.myAnimalMaps[AnimalID].addPolygon(inPoly1);
            this.myAnimalMaps[AnimalID].addPolygon(inPoly2);

         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      private void addNewPoly(Map inMap, IPolygon inPoly)
      {
         IFeature feature = null;
         IFields fields;
         int index = 0;


         //now add the one we want
         try
         {
            feature = inMap.mySelf.CreateFeature();
            fields = feature.Fields;
            index = fields.FindField("CurrTime");
            if (index >= 0)
               feature.set_Value(index, 1);
            index = fields.FindField("OCCUP_MALE");
            
            feature.Shape = inPoly;
            feature.Store();
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
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
               if (s.IndexOf("MapManagerPath") == 0)
               {
                  fw = FileWriter.FileWriter.getMapManagerLogger(s.Substring(s.IndexOf(" ")));

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

      private DateTime createStartTime(string time, string typeOfTime)
      {
         string[] s;
         DateTime dt = DateTime.Today;
         try
         {
            switch (typeOfTime)
            {
               case "daily":
                  s = time.Split('/');
                  int day = System.Convert.ToInt32(s[1]);
                  int month = System.Convert.ToInt32(s[0]);
                  int year = System.Convert.ToInt32(s[2]);
                  dt = new DateTime(year, month, day);
                  break;
               case "hourly":
                  dt = DateTime.Today;
                  //the time is in the format of 7:32 PM
                  //so split it up and deal with it you wancker
                  s = time.Split(':');
                  dt = dt.AddHours(System.Convert.ToInt32(s[0]));
                  dt = dt.AddMinutes(System.Convert.ToInt32(s[1].Substring(0, 2)));
                  if (time.IndexOf("PM") > 0)
                     dt = dt.AddHours(12);
                  break;
               case "yearly":
                  dt = DateTime.Today;
                  dt = dt.AddYears(System.Convert.ToInt32(time) - dt.Year);
                  break;
               case "static":
                  dt = DateTime.Today;
                  dt = dt.AddYears(System.Convert.ToInt32(time) - dt.Year);
                  break;
               default:
                  throw new SystemException("Bad type of time in Map Manager createStartTime");

            }

         }
         catch (System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }

         return dt;
      }

      private void EditNewHomeRangeUnion(string inUnionSocialMapPath, string sex, string inAnimalID)
      {
         int sexIndex;
         IFeatureCursor updateCurr;
         IFeature feat;
         IQueryFilter qf = new QueryFilterClass();
         qf.WhereClause = "FID_availa >= 0";

         IFeatureClass fc = myDataManipulator.GetFeatureClass(inUnionSocialMapPath);
         if (sex.Equals("male", StringComparison.CurrentCultureIgnoreCase))
            sexIndex = fc.FindField("OCCUP_MALE");
         else
            sexIndex = fc.FindField("OCCUP_FEMA");

         updateCurr = fc.Update(qf, true);
         while((feat = updateCurr.NextFeature()) != null)
         {
            feat.set_Value(sexIndex,inAnimalID);
            feat.Store();
         }
         updateCurr.Flush();
         System.Runtime.InteropServices.Marshal.ReleaseComObject(qf);
         
      }

      private void initializeErrMessages()
      {
         errMessages = new System.Collections.Specialized.StringCollection();
         this.errMessages.Add("Wrong type of shape file");
         this.errMessages.Add("Required field not found for this shape file.");
         this.errMessages.Add("No files found in directory");
         this.errMessages.Add("That directory is all ready in use");
         this.errMessages.Add("That directory is not available");
         errNumber = -1;

      }

      private void makeMapCopies(string orgPath, string orgFileName, string newPath, string newFileName)
      {
         string[] fileNames;
         string extension;
         try
         {
            if (!Directory.Exists(newPath))
               Directory.CreateDirectory(newPath);
            fileNames = Directory.GetFiles(orgPath, orgFileName + "*");
            for (int i = 0; i < fileNames.Length; i++)
            {
               extension = fileNames[i].Substring(fileNames[i].Length - 4, 4);
               File.Copy(fileNames[i], newPath + "\\" + newFileName + extension, true);
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

      private string makeNewMapPath(string oldMapPath, string timeStep, string inAnimalID)
      {
         StringBuilder sb = new StringBuilder();
         sb.Append(System.IO.Path.GetDirectoryName(oldMapPath) + "\\");
         sb.Append(inAnimalID + timeStep + ".shp");
         return sb.ToString();

      }

      private void removeAnimalMap(string oldMapPath)
      {
        
         try
         { 
            fw.writeLine("");
            fw.writeLine("Inside removeAnimalMap " + oldMapPath);
            string currDir = System.IO.Path.GetDirectoryName(oldMapPath);
            string oldFileName = System.IO.Path.GetFileNameWithoutExtension(oldMapPath);
            string pattern = oldFileName + "*";
            string [] files = Directory.GetFiles(currDir,pattern);
            for (int i=0; i< files.Length; i++)
            {
               if(oldFileName.Equals(System.IO.Path.GetFileNameWithoutExtension(files[i])))
               {
                  File.Delete(files[i]);
               }
            }
         }

         catch (Exception)
         {
            fw.writeLine("Exectional Fail");
            throw;
         }

      }

      private void removeTimeStepMaps(string AnimalID)
      {
         string[] fileNames;
         try
         {
            //use the animal id because each animal has its own sub folder based
            //on its name.
            fileNames = Directory.GetFiles(mOutMapPath + "\\" + AnimalID, "TimeStep" + "*");
            for (int i = 0; i < fileNames.Length; i++)
            {
#if ! pat
               File.Delete(fileNames[i]);
#endif
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

      /********************************************************************************
       *  Function name   : validateNFieldPointMap
       *  Description     : Will check to make sure it is a valid point shape file and 
       *                    has the required number of fields
       *  Return type     : bool 
       *  Argument        : string [] inMapFileNames the names of the map files to validate
       *  Argument        : int NumFields  the number of fields the maps should have
       * *******************************************************************************/
      private bool validateNFieldPointMap(string[] inMapFileNames, string[] inFieldNames)
      {
         int i;
         int found = -1;
         bool result;
         string fileName;
         IFeatureClass myShapefile = null;


         Check.Require(inFieldNames.Length > 0, "No field names to check in validateNFieldPointMap");
         Check.Require(inMapFileNames.Length > 0, "No files to check in validateNFieldPointMap");

         result = true;//assume we are good to go
         fw.writeLine("inside validateNFieldPointMap going to look for " + inFieldNames.Length.ToString() + " fields");
         fw.writeLine("set the feature workspace");
         featureWrkSpace = (IFeatureWorkspace)this.wrkSpace;
         for (i = 0; i < inMapFileNames.GetLength(0); i++)
         {
            fileName = System.IO.Path.GetFileNameWithoutExtension(inMapFileNames[i]);
            fw.writeLine("found " + inMapFileNames[i]);

            //create the feature_class
            myShapefile = this.featureWrkSpace.OpenFeatureClass(fileName);
            fw.writeLine("now check for correct type of shape file");
            if (myShapefile.ShapeType.ToString() == "esriGeometryPoint")
            {

               fw.writeLine("must be a point now check for the required field names");
               for (int j = 0; j < inFieldNames.Length; j++)
               {
                  fw.writeLine("looking for " + inFieldNames[j]);
                  found = myShapefile.Fields.FindField(inFieldNames[j]);
                  fw.writeLine("the result from the find field is " + found.ToString());
                  if (found < 0)
                  {
                     this.errNumber = (int)ERR.CAN_NOT_FIND_REQUIRED_FIELD;
                     fw.writeLine("did not find " + inFieldNames[j] + "  so set result = false and bail from loop");
                     this.errFileName = fileName;
                     result = false;
                     break;
                  }
               }//end checking for correct field names
            }
            else
            {
               fw.writeLine("wrong type of shape file so set result to false and bail out of here");
               result = false;
               this.errNumber = (int)ERR.WRONG_TYPE_OF_SHAPE_FILE;
               this.errFileName = fileName;
            }//end checking to see if it is a point file 
         }//end loop
         fw.writeLine("leaving validateNFieldPolylMap with a value of " + result.ToString());
         return result;
      }

      private bool validateNFieldPolylMap(string[] inMapFileNames, string[] inFieldNames)
      {
         int i;
         int found = -1;
         bool result;
         string fileName;
         IFeatureClass myShapefile = null;


         Check.Require(inFieldNames.Length > 0, "No field names to check in ValidatePolyMap");
         Check.Require(inMapFileNames.Length > 0, "No files to check in validateNFieldPolylMap");

         result = true;//assume we are good to go
         fw.writeLine("inside validateNFieldPolylMap going to look for " + inFieldNames.Length.ToString() + " fields");
         fw.writeLine("set the feature workspace");
         featureWrkSpace = (IFeatureWorkspace)this.wrkSpace;
         for (i = 0; i < inMapFileNames.GetLength(0); i++)
         {
            fileName = System.IO.Path.GetFileNameWithoutExtension(inMapFileNames[i]);
            fw.writeLine("found " + inMapFileNames[i]);

            //create the feature_class
            myShapefile = this.featureWrkSpace.OpenFeatureClass(fileName);
            fw.writeLine("now check for correct type of shape file");
            if (myShapefile.ShapeType.ToString() == "esriGeometryPolygon")
            {

               fw.writeLine("must be a polygon now check for the required field names");
               for (int j = 0; j < inFieldNames.Length; j++)
               {
                  fw.writeLine("looking for " + inFieldNames[j]);
                  found = myShapefile.Fields.FindField(inFieldNames[j]);
                  fw.writeLine("the result from the find field is " + found.ToString());
                  if (found < 0)
                  {
                     this.errNumber = (int)ERR.CAN_NOT_FIND_REQUIRED_FIELD;
                     fw.writeLine("did not find " + inFieldNames[j] + "  so set result = false and bail from loop");
                     this.errFileName = fileName;
                     result = false;
                     break;

                  }//end checking for correct field names
               }
            }
            else
            {
               fw.writeLine("wrong type of shape file so set result to false and bail out of here");
               result = false;
               this.errNumber = (int)ERR.WRONG_TYPE_OF_SHAPE_FILE;
               this.errFileName = fileName;
            }//end checking to see if it is a polygon file 
         }//end loop
         fw.writeLine("leaving validateNFieldPolylMap with a value of " + result.ToString());
         return result;
      }

		#endregion Private Methods 

		#region Public Methods (39) 

      public void AddTimeSteps(int AnimalID, IPolygon inPoly1, IPolygon inPoly2, int timeStep, string sex)
      {
         try
         {
            
            
            string mapPath = this.myAnimalMaps[AnimalID].FullFileName;
            string newMapPath = this.makeNewMapPath(mapPath, timeStep.ToString(),AnimalID.ToString());
            string clipPath = this.OutMapPath + "\\Clippy_" + AnimalID.ToString() + timeStep.ToString() + ".shp";
            string unionPath = this.OutMapPath + "\\Union_" + AnimalID.ToString() + timeStep.ToString() + ".shp";
            string timeStepPath = this.OutMapPath + "\\TimeStepPath_" + AnimalID.ToString() + timeStep.ToString() + ".shp";
            string dissolvePath = this.OutMapPath + "\\DissolvePath_" + AnimalID.ToString() + timeStep.ToString() + ".shp";


            fw.writeLine("the  current animal map is " + mapPath);
            fw.writeLine("the  current clip map is " + clipPath);
            fw.writeLine("the  current union map is " + unionPath);
            fw.writeLine("the  current temp timeStep map is " + timeStepPath);
            fw.writeLine("the  current dissolve map is " + dissolvePath);
      
            fw.writeLine("inside AddTimeSteps for ManManager the animal id is " + AnimalID.ToString());
            fw.writeLine("time step is " + timeStep.ToString());
            fw.writeLine("calling MakeDissolvedTimeStep");
            this.myDataManipulator.MakeDissolvedTimeStep(this._currStepPath, timeStepPath, inPoly1, inPoly2);
           
            fw.writeLine("back in AddTimeSteps now going to clip against the social map");
            this.myDataManipulator.Clip(this.mySocialMap.FullFileName, timeStepPath, clipPath );
            
            fw.writeLine("back from Clipping now update the Current Animal Map");
             //this is to get the current occupied or not at this time because
             // it could change over time.
            if (timeStep == 0)
            {
               //if the first time through then we only need to add it to the map
               fw.writeLine("Calling Copy to Animal Map since it is the first time step");
               this.myDataManipulator.CopyToAnotherlMap(mapPath, clipPath);
               this.myDataManipulator.Dissolve(mapPath, dissolvePath, "SUITABILIT;OCCUP_MALE;OCCUP_FEMA");
            }
            else
            {
               fw.writeLine("Calling update the animal map");
               this.myDataManipulator.UnionAnimalClipData(mapPath,clipPath,unionPath);
               this.myDataManipulator.Dissolve(unionPath, dissolvePath, "SUITABILIT;OCCUP_MALE;OCCUP_FEMA"); 
               this.myAnimalMaps[AnimalID].removeAllPolygons();
               
            }

            fw.writeLine("now make the new animal map");

            this.makeMapCopies(System.IO.Path.GetDirectoryName(dissolvePath), System.IO.Path.GetFileNameWithoutExtension(dissolvePath), System.IO.Path.GetDirectoryName(mapPath), System.IO.Path.GetFileNameWithoutExtension(newMapPath));
            fw.writeLine("now we need to move the dissovled back to the orginal map");
            fw.writeLine("now going to copy " + dissolvePath + " to " + mapPath);
            fw.writeLine("time to remove those extra files");
            this.removeExtraFiles(clipPath);
            this.removeExtraFiles(unionPath);
            this.removeExtraFiles(timeStepPath);
            this.removeExtraFiles(dissolvePath);


         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }

      }

      public void addYearToMaps()
      {
         mySocialMaps.addYearToMaps();
         myFoodMaps.addYearToMaps();
         myPredationMaps.addYearToMaps();
         myMoveMaps.addYearToMaps();
         myDispersalMaps.addYearToMaps();

      }

      public void BuildHomeRange(Animal inAnimal)
      {
         try
         {
            fw.writeLine("inside BuildHomeRange for George number " + inAnimal.IdNum.ToString());
            fw.writeLine("Building the new paths");
            string currSocialMapPath = this.SocialMap.FullFileName;
            string newUnionSocialMapPath = this.OutMapPath + "\\tempUnionSocial" + inAnimal.IdNum.ToString() + ".shp";
            string newTempPolyGonPath = this.OutMapPath + "\\tempPolyGon" + inAnimal.IdNum.ToString() + ".shp";
            string newTempSocialMapPath = this.OutMapPath + "\\tempSocial" + inAnimal.IdNum.ToString() + ".shp";
            string newSocialMapPath = this.OutMapPath  + "\\NewSocialMap" + inAnimal.IdNum.ToString() + ".shp";

            fw.writeLine("Now making the home range builder");
            HomeRangeBuilder hrb = new HomeRangeBuilder();
            string NewHomeRangeFileName = hrb.BuildHomeRange(inAnimal, currSocialMapPath);
            fw.writeLine("new home range name is " + NewHomeRangeFileName);
            fw.writeLine("going to union it up and create " + newUnionSocialMapPath);
            this.myDataManipulator.UnionHomeRange(currSocialMapPath, NewHomeRangeFileName, newUnionSocialMapPath);
            fw.writeLine("now edit that map");
            this.EditNewHomeRangeUnion(newUnionSocialMapPath, inAnimal.Sex, inAnimal.IdNum.ToString());
            fw.writeLine("now call  myDataManipulator.CopyToAnotherlMap new map name is " + newTempSocialMapPath);
            this.myDataManipulator.CopyToAnotherlMap(newTempSocialMapPath, newUnionSocialMapPath);
            fw.writeLine("now call myDataManipulator.RemoveExtraFields");
            this.myDataManipulator.RemoveExtraFields(newTempSocialMapPath, "FID_availa; SUITABIL_1; OCCUP_MA_1; OCCUP_FE_1");
            fw.writeLine("now calling myDataManipulator.DissolveAndReturn to make " + newSocialMapPath);
            IFeatureClass newFC = this.myDataManipulator.DissolveAndReturn(newTempSocialMapPath, newSocialMapPath, "SUITABILIT;OCCUP_MALE;OCCUP_FEMA");
            fw.writeLine("Remove the old social map and assign the new one to me");
            this.SocialMap = null;
            this.SocialMap = new Map(newFC, newSocialMapPath);
            fw.writeLine("now blow away the temp maps for next time");
            //this.removeExtraFiles(newUnionSocialMapPath);
            //this.removeExtraFiles(newTempPolyGonPath);
            //this.removeExtraFiles(newTempSocialMapPath);
            //if (numHomeRanges > 2)//do not want to blow away the orginal data just the temp maps
               //this.removeExtraFiles(currSocialMapPath);
            numHomeRanges++;
            
         }
         catch(System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
   }
      }

      public void changeMaps(DateTime now, AnimalManager am)
      {
         fw.writeLine("inside changeMaps (DateTime now,AnimalManager am)");
         fw.writeLine("DateTime = " + now.ToShortDateString() + " " + now.ToShortTimeString());
         
         myFoodMaps.changeMap(now);
         myPredationMaps.changeMap(now);
         myMoveMaps.changeMap(now);
         myDispersalMaps.changeMap(now, am);
      }

      public void changeMaps(DateTime now)
      {
         mySocialMaps.changeMap(now);
         myFoodMaps.changeMap(now);
         myPredationMaps.changeMap(now);
         myMoveMaps.changeMap(now);
         myDispersalMaps.changeMap(now);
      }

      public IFeatureClass getAnimalMap(int index)
      {
         AnimalMap am = null;
         try
         {
            if (index >= 0 && index < this.myAnimalMaps.GetLength(0))
            {
               am = this.myAnimalMaps[index];
            }

         }
         catch (System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return am.mySelf;
      }

      public string getAnimalMapName(int index)
      {
         string fileName = "";
         try
         {
            if (index >= 0 && index < this.myAnimalMaps.GetLength(0))
            {
               fileName = this.myAnimalMaps[index].FullFileName;
            }

         }
         catch (System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
            fileName = "Not Found";
         }
         return fileName;

      }

      public double getArea(int index)
      {
         double area = 0;
         try
         {
            area = this.mySocialMap.getAvailableArea(index);
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return area;
      }

      //public void changeSocialMap(DateTime now, AnimalManager am)
      //{
      //   IFeatureClass oldSocialFeatureClass;
      //    IFeatureClass newSocialFeatureClass;
      //   try
      //   {
      //      fw.writeLine("inside change social map");
      //      oldSocialFeatureClass = this.SocialMap.mySelf;
      //      fw.writeLine("social map name is " + oldSocialFeatureClass.AliasName);
      //      mySocialMaps.changeMap(now, am);
      //      fw.writeLine("after change social map name is " + this.SocialMap.mySelf.AliasName);
      //      if (!oldSocialFeatureClass.Equals(this.SocialMap))
      //      {
      //        // newSocialFeatureClass = this.myMapManipulator.clipMaps(oldSocialFeatureClass, mySocialMap.mySelf,ref this.SocialIndex);
      //         this.changeOutSocialMapAfterClip(newSocialFeatureClass);
      //      }
      //   }
      //   catch (Exception ex)
      //   {

      //      FileWriter.FileWriter.WriteErrorFile(ex);
      //   }
      //}
      public crossOverInfo getCrossOverInfo(IPoint startPoint, IPoint endPoint)
      {
         return this.myMoveMap.getCrossOverInfo(startPoint, endPoint);
      }

      public crossOverInfo getCrossOverInfo(IPoint startPoint, IPoint endPoint, ref int currPolyIndex, ref int newPolyIndex)
      {
         fw.writeLine("inside map manger getCrossOverInfo");
         return this.myMoveMap.getCrossOverInfo(startPoint, endPoint, ref currPolyIndex, ref newPolyIndex);
      }

      public int getCurrMovePolygon(IPoint inPoint)
      {
         fw.writeLine("inside mapmanger calling map get current poly gon for x = " + inPoint.X.ToString() + " Y " + inPoint.Y.ToString());
         fw.writeLine("move map name is " + this.myMoveMap.FullFileName);
         return this.myMoveMap.getCurrentPolygon(inPoint);
      }

      public double getDistance(IPoint start, IPoint end)
      {
         IPolyline p = new PolylineClass();
         try
         {
            p.FromPoint = start;
            p.ToPoint = end;
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }

         return p.Length;
      }

      public string getErrMessage()
      {
         string errMessage;
         if (errNumber >= 0)
         {
            errMessage = "File Name: " + errFileName + System.Environment.NewLine + errMessages[errNumber];
            errNumber = -1;
         }
         else
         {
            errMessage = "no error";
         }
         return errMessage;

      }

      public void GetFoodData(IPoint location, ref int PolyIndex, ref double chance, ref double mean, ref double sd)
      {

         try
         {
            fw.writeLine("inside get food data for poly index " + PolyIndex.ToString());
            //check to see if we have moved out of the food index if so then adjust
            //Author: Bob Cummings moved all logic inside if statement.  No sense 
            //doing it unless the values changed.
            if (!myFoodMap.pointInPolygon(location, PolyIndex))
            {
               PolyIndex = myFoodMap.getCurrentPolygon(location);
            }
            //if the Poly Index is  less then 0 then george is sitting on the fence.
            //so use the old values and do not try to update.
            if (PolyIndex >= 0)
            {
               myHash = myFoodMap.getAllValuesForSinglePolygon(PolyIndex);
               chance = System.Convert.ToDouble(myHash["PROBCAP"]);
               mean = System.Convert.ToDouble(myHash["X_SIZE"]);
               sd = System.Convert.ToDouble(myHash["SD_SIZE"]);
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

      public void GetInitalResidentAttributes(out InitialAnimalAttributes[] outAttributes)
      {
         outAttributes = null;
         try
         {
            this.mySocialMap.GetInitialResidentInformation(out outAttributes);
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
      }

      public void GetInitialAnimalAttributes(out InitialAnimalAttributes[] outAttributes)
      {
         outAttributes = null;
         try
         {
            this.myDispersalMap.GetInitialAnimalAttributes(out outAttributes);
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }

      }

      public void GetInitialMapData(Animal inA)
      {
         int PolyIndex = 0;
         fw.writeLine("inside get inital map data");
         fw.writeLine("getting move modifiers now");

         PolyIndex = this.myMoveMap.getCurrentPolygon(inA.Location);
         if (PolyIndex >= 0)
         {
            this.myHash = this.myMoveMap.getAllValuesForSinglePolygon(PolyIndex);
            inA.MoveTurtosity = System.Convert.ToDouble(myHash["MVL"]);
            inA.MoveSpeed = System.Convert.ToDouble(myHash["MSL"]);
            inA.EnergyUsed = System.Convert.ToDouble(myHash["ENERGYUSED"]);
            inA.PerceptonModifier = System.Convert.ToDouble(myHash["PR_X"]);


         }
         else
         {
            inA.MoveTurtosity = 1;
            inA.MoveSpeed = 1;
            inA.EnergyUsed = 0;
            inA.PerceptonModifier = 1;
            PolyIndex = this.myMoveMap.getCurrentPolygon(inA.myMover.stepBack(inA));

         }

         inA.MoveIndex = PolyIndex;


         fw.writeLine("now getting initial food data ");
         PolyIndex = myFoodMap.getCurrentPolygon(inA.Location);
         myHash = myFoodMap.getAllValuesForSinglePolygon(PolyIndex);
         inA.CaptureFood = System.Convert.ToDouble(myHash["PROBCAP"]);
         inA.FoodMeanSize = System.Convert.ToDouble(myHash["X_SIZE"]);
         inA.FoodSD_Size = System.Convert.ToDouble(myHash["SD_SIZE"]);
         inA.FoodIndex = PolyIndex;

         fw.writeLine("now getting initial risk data");
         PolyIndex = myPredationMap.getCurrentPolygon(inA.Location);
         inA.PredationRisk = System.Convert.ToDouble(myPredationMap.getNamedValueForSinglePolygon(PolyIndex, "RISK"));
         inA.RiskIndex = PolyIndex;
      }

      public void GetMoveModifiers(IPoint inPoint, ref int PolyIndex, ref double MVL, ref double MSL,
         ref double PerceptionModifier, ref double EnergyUsed)
      {
         try
         {
            fw.writeLine("inside mapManager GetMoveModifiers checking to see if we moved out of the current polygon");
            fw.writeLine("move map name is " + this.myMoveMap.FullFileName + "\\" + this.myMoveMap.mySelf.AliasName);
            fw.writeLine("the PolyIndex is " + PolyIndex.ToString());
            fw.writeLine("my location is x=" + inPoint.X.ToString() + " and y=" + inPoint.Y.ToString());

            if (!this.myMoveMap.pointInPolygon(inPoint, PolyIndex))
            {
               fw.writeLine("we must have moved so get new polyIndex");
               fw.writeLine("old index is " + PolyIndex.ToString());
               fw.writeLine("old values are MSL = " + MSL.ToString() + " MVL + " + MVL.ToString() + " energyused is " + EnergyUsed.ToString() + " PerceptionModifier = " + PerceptionModifier.ToString());
               fw.writeLine("go to Map Log file to see new values.");
               PolyIndex = this.myMoveMap.getCurrentPolygon(inPoint);
               fw.writeLine("new move index is " + PolyIndex.ToString());

            }
            // Bob Cummings Saturday, February 23, 2008
            // If the PolyIndex is a negative value then George is on the fence between
            // two polygons so keep the orginal values.  Otherwise the program bombs.
            if (PolyIndex >= 0)
            {
               this.myHash = this.myMoveMap.getAllValuesForSinglePolygon(PolyIndex);
               MVL = System.Convert.ToDouble(myHash["MVL"]);
               MSL = System.Convert.ToDouble(myHash["MSL"]);
               EnergyUsed = System.Convert.ToDouble(myHash["ENERGYUSED"]);
               PerceptionModifier = System.Convert.ToDouble(myHash["PR_X"]);
            }
            else
            {
               MVL = 1;
               MSL = 1;
               EnergyUsed = 1;
               PerceptionModifier = 1;
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

//      public IFeatureClass getTimeStepMap(string AnimalID)
//      {
//         string[] fileNames;
//         string path;
//         IFeatureClass fc = null;
//         try
//         {
//            fw.writeLine("inside getTimeStepMap for " + AnimalID);
//            path = this.mOutMapPath + "\\" + AnimalID;
//            fw.writeLine("full path for that animal is " + path);
//            fw.writeLine("now get the file names");
//            fileNames = Directory.GetFiles(path, "TimeStep*");
//            fw.writeLine("we have " + fileNames.Length.ToString() + " files to look for ");
//            fw.writeLine("calling Map openFeatureClass");
//            fc = Map.openFeatureClass(path, System.IO.Path.GetFileNameWithoutExtension(fileNames[0]));
//         }
//         catch (System.Exception ex)
//         {
//#if (DEBUG)
//            System.Windows.Forms.MessageBox.Show(ex.Message);
//#endif
//            FileWriter.FileWriter.WriteErrorFile(ex);
//         }
//         return fc;

//      }
      public void getNumResidents(out int numMales, out int numFemales)
      {
         numMales = 0;
         numFemales = 0;
         this.mySocialMap.getNumResidents(out numMales, out numFemales);
      }

      public void GetRiskModifier(IPoint location, ref int PolyIndex, ref double risk)
      {

         try
         {
            fw.writeLine("incoming risk value is " + risk.ToString());
            fw.writeLine("inside GetRiskModifier checking to see if we moved out of the orginal polygon ");
            fw.writeLine("the poly index is " + PolyIndex.ToString());
            //check to see if we have moved out of the risk index if so then adjust
            //Author: Bob Cummings moved all logic inside if statement.  No sense 
            //doing it unless the values changed.
            if (!this.myPredationMap.pointInPolygon(location, PolyIndex))
            {
               fw.writeLine("must have changed now getting the new polygon");
               PolyIndex = myPredationMap.getCurrentPolygon(location);
            }
            //if the value is less then zero George is sitting on a fence between
            //two polygons.  So use old values.
            if (PolyIndex >= 0)
            {
               fw.writeLine("now getting the risk value");
               risk = System.Convert.ToDouble(myPredationMap.getNamedValueForSinglePolygon(PolyIndex, "RISK"));
               fw.writeLine("out going risk is " + risk.ToString());
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

      public void getSocialIndex(IPoint inPoint, ref int inPolygonIndex)
      {

         try
         {
            fw.writeLine("");
            fw.writeLine("inside getSocialIndex for location x = " + inPoint.X.ToString() + " y = " + inPoint.Y.ToString());
            fw.writeLine("mySocialMap name is " + this.mySocialMap.mySelf.AliasName);
            if (!mySocialMap.pointInPolygon(inPoint, inPolygonIndex))
            {
               int tempPolyIndex = inPolygonIndex;
               inPolygonIndex = mySocialMap.getCurrentPolygon(inPoint);
               //sometimes the point is on a border between polygons in that case the 
               //return value is -1, so just use the last value
               if (inPolygonIndex < 0)
                  inPolygonIndex = tempPolyIndex;
            }


         }
         catch (System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         fw.writeLine("leaving getSocialIndex with an index of " + inPolygonIndex.ToString());
         fw.writeLine("");

      }

      /********************************************************************************
       *  Function name   : validateNFieldPolylMap
       *  Description     : Will check to make sure it is a valid polygon shape file and 
       *                    has the required number of fields
       *  Return type     : bool 
       *  Argument        : string [] inMapFileNames all the file names of the map we need 
       *  Argument        : int NumFields the number of field the maps are supposed to have
       * *******************************************************************************/
      public IGeometryDef getSpatialInfo()
      {
         IGeometryDef geoDef = null;
         try
         {
            IField f;
            int fieldIndex = 0;
            IFeatureCursor fc;
            fc = this.mySocialMap.mySelf.Search(null, false);
            fieldIndex = fc.FindField("SHAPE");
            f = fc.Fields.get_Field(fieldIndex);
            geoDef = f.GeometryDef;
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return geoDef;
      }

      public bool isOccupied(IPoint inPoint)
      {
         bool occupied = true;
         int polyIndex = this.mySocialMap.getCurrentPolygon(inPoint);
         string s = this.mySocialMap.getNamedValueForSinglePolygon(polyIndex, "OCCUP_MALE").ToString();
        
         if (s.Equals("NONE",StringComparison.CurrentCultureIgnoreCase))
         {
            s = this.mySocialMap.getNamedValueForSinglePolygon(polyIndex, "OCCUP_FEMA").ToString();

            if (s.Equals("NONE", StringComparison.CurrentCultureIgnoreCase))
            {
               occupied = false;
            }
         }
         return occupied;
      }

      public bool isOccupied(int inPolyIndex, string sex, ref FileWriter.FileWriter inFw)
      {
         bool occupied = true;
         string s;
         inFw.writeLine("inside isOccupied checking for " + sex);
         if (sex == "Male")
         {
            s = this.mySocialMap.getNamedValueForSinglePolygon(inPolyIndex, "OCCUP_MALE").ToString();

         }
         else
         {
            s = this.mySocialMap.getNamedValueForSinglePolygon(inPolyIndex, "OCCUP_FEMA").ToString();
         }

         inFw.writeLine("that returns " + s);
         s = s.ToUpper();
         if (s == "NONE")
         {
            occupied = false;
         }
         return occupied;
      }

      public bool isPointOnMoveMap(IPoint inPoint)
      {
         return this.myMoveMap.isPointOnMap(inPoint);
      }

      //Bob Cummings Sunday, February 24, 2008
      //added for checking if site is suitable for site home range trigger.
      public bool isSuitable(int inPolyIndex, ref FileWriter.FileWriter inFw)
      {
         bool isSuitable = false;
         string s;
         inFw.writeLine("inside isSuitable checking for suitablility for index" + inPolyIndex.ToString());
         try
         {
            s = this.mySocialMap.getNamedValueForSinglePolygon(inPolyIndex, "SUITABILIT").ToString();
            if (s == "Suitable")
               isSuitable = true;
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
         return isSuitable;
      }

      /********************************************************************************
      *   Function name   : loadMap
       *   Description     : loads the maps that need to be loaded for this part of the
       *                     simulation 
       *   Return type     : void 
       *   Argument        : string inMapType is the type of map "Social,Food" etc
       *   Argument        : string fileName is the actual name of the shape file to open
       * ********************************************************************************/
      public void loadOneMap(string inMapType, string fileName, string inPath)
      {
         try
         {
            fw.writeLine("inside load map for " + inMapType);
            //sometimes we are loading from an XML file and so we have to check that the
            //map is actually available
            String fullName = inPath + "\\" + fileName + ".shp";
            if (File.Exists(fullName))
            {
               fw.writeLine("the file we want to load is " + inPath + " " + fileName);
               //the file name is the fully qualifed path and name
               //all we use is the name of the shape file itself
               this.wrkSpace = this.wrkSpaceFactory.OpenFromFile(inPath, 0);
               featureWrkSpace = (IFeatureWorkspace)this.wrkSpace;
               fileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
               switch (inMapType)
               {
                  case "Social":
                     
                      
                      fw.writeLine("inside case under Social loading the map");
                     if (mySocialMap == null)
                     {

                         mySocialMap = new Map(this.featureWrkSpace.OpenFeatureClass(fileName), fullName);
                     }
                     else
                     {
                        //System.Runtime.InteropServices.Marshal.ReleaseComObject(this.wrkSpace);
                        //if (!Directory.Exists(mOutMapPath))
                        //   Directory.CreateDirectory(mOutMapPath);
                        //this.wrkSpace = this.wrkSpaceFactory.OpenFromFile(mOutMapPath, 0);
                        //featureWrkSpace = (IFeatureWorkspace)this.wrkSpace;
                        //this.makeMapCopies(inPath, fileName, this.mOutMapPath, "tmpSocial");
                        //Map tempMap = new Map(this.featureWrkSpace.OpenFeatureClass("tmpSocial"));
                        //this.myMapManipulator.unionSocialMaps(tempMap);
                        //this.myMapManipulator.editNewSocialMap("Social");
                        //mySocialMap = new Map(this.featureWrkSpace.OpenFeatureClass("Social"));


                     }
                     mySocialMap.TypeOfMap = "Social";
                     mySocialMap.Path = mOutMapPath;
                     //add the refernce to the map manipulator
                    // this.myMapManipulator.SocialMap = this.mySocialMap;
                     break;
                  case "Food":
                     fw.writeLine("inside case under Food loading the map");
                     if (myFoodMap != null)
                        fw.writeLine("old map name is " + myFoodMap.mySelf.AliasName);
                     myFoodMap = new Map(this.featureWrkSpace.OpenFeatureClass(fileName));
                     fw.writeLine("new map name is " + myFoodMap.mySelf.AliasName);
                     myFoodMap.TypeOfMap = "Food";
                     myFoodMap.Path = inPath;
                     break;
                  case "Predation":
                     fw.writeLine("inside case under Predation loading the map");

                     myPredationMap = new Map(this.featureWrkSpace.OpenFeatureClass(fileName));
                     myPredationMap.TypeOfMap = "Risk";
                     myPredationMap.Path = inPath;
                     break;
                  case "Dispersal":
                     fw.writeLine("inside case under Dispersal loading the map");
                     myDispersalMap = new Map(this.featureWrkSpace.OpenFeatureClass(fileName));
                     myDispersalMap.TypeOfMap = "Dispersal";
                     myDispersalMap.Path = inPath;
                     break;
                  case "Move":
                     fw.writeLine("inside case under Move loading the map");
                     myMoveMap = new Map(this.featureWrkSpace.OpenFeatureClass(fileName));
                     myMoveMap.TypeOfMap = "Move";
                     myMoveMap.Path = inPath;
                     break;
                  default:
                     fw.writeLine("bombed with invalid name ");
                     System.Windows.Forms.MessageBox.Show("Not a valid map type" + inMapType);
                     break;
               }// end switch on map name
            }
            else
            {
               throw new SystemException(fullName + " Not found");
            }

         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif


         }

      }

      public void loadXMLTriggers(string mapType, XPathNodeIterator inIterator)
      {
         try
         {
            switch (mapType)
            {
               case "Social":
                  mySocialMaps.loadXMLTriggers(inIterator);
                  break;
               case "Food":
                  myFoodMaps.loadXMLTriggers(inIterator);
                  break;
               case "Predation":
                  myPredationMaps.loadXMLTriggers(inIterator);
                  break;
               case "Move":
                  myMoveMaps.loadXMLTriggers(inIterator);
                  break;
               case "Dispersal":
                  myDispersalMaps.loadXMLTriggers(inIterator);
                  break;
               default:
                  throw new ArgumentException(mapType + " is an invalid mapType in loadXMLTriggers in MapManagerClass");

            }

         }
         catch (ArgumentException ae)
         {
            // System.Windows.Forms.MessageBox.Show(ae.Message);
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }

      }

      public bool MakeCurrStepMap(string path)
      {
         this._currStepPath = path + "\\currStep.shp";
         this.myDataManipulator.CreateEmptyFeatureClass(this._currStepPath, "polygon");
         return true;
      }

      public void makeHomeRange(Animal inA)
      {
         IFeatureClass oldSocialFeatureClass;
         IFeatureClass newSocialFeatureClass;
         IPolygon homeRange;
         try
         {
            fw.writeLine("inside make home range for animial number " + inA.IdNum.ToString());
            fw.writeLine("creating the home range polygon");
            homeRange = Map.BuildHomeRangePolygon(inA, 1.0);
            {
               //oldSocialFeatureClass = this.SocialMap.mySelf;
               //fw.writeLine("ok myMapManipulator.makeHomeRange returned true so create the home range");
               //createAnimalHomeRangeMap(inA.IdNum, inA.FileNamePrefix);
               //fw.writeLine("now try to change out the social map");
               //changeOutSocialMapWithHomeRange();
               //newSocialFeatureClass = this.myMapManipulator.clipMaps(oldSocialFeatureClass, mySocialMap.mySelf, ref this.SocialIndex);
               //this.changeOutSocialMapAfterClip(newSocialFeatureClass);
            }
         }
         catch (System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      public bool makeNewAnimalMaps(int numAnimals)
      {
         bool success = true;
         int i = 0;
         try
         {
            IGeometryDef geoDef = this.getSpatialInfo();
            fw.writeLine("inside make new animal map for " + numAnimals.ToString() + " number of animals");
            myAnimalMaps = new AnimalMap[numAnimals];
            for (i = 0; i < numAnimals && success; i++)
            {
               myAnimalMaps[i] = new AnimalMap(i.ToString(), mOutMapPath, geoDef);
               //set reference to social map so we can add those fields on the makeMap call
               myAnimalMaps[i].MySocialMap = this.mySocialMap;

            }
            fw.writeLine("leaving make new animal maps");
         }
         catch (System.Exception ex)
         {
            success = false;
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         if (success == false)
         {
            this.errNumber = (int)ERR.DIRECTORY_ALREADY_IN_USE;
            this.errFileName = mOutMapPath + @"\" + i.ToString();
         }
         return success;
      }

      public void makeNewDisperserAnimalMaps(AnimalManager am)
      {
         int totalNumMaps = am.Count + 1;
         int currNumMaps = this.myAnimalMaps.Length;


         try
         {
            fw.writeLine("inside makeNewDisperserAnimalMaps making temp array to hold all maps");
            AnimalMap[] temp = new AnimalMap[totalNumMaps];
            fw.writeLine("temp is " + temp.Length + " long");
            fw.writeLine("now copy existing maps to new temp array there are " + currNumMaps.ToString() + " to copy");
            this.myAnimalMaps.CopyTo(temp, 0);
            IGeometryDef geoDef = this.getSpatialInfo();
            fw.writeLine("starting loop");
            for (int i = currNumMaps; i < totalNumMaps; i++)
            {
               temp[i] = new AnimalMap(i.ToString(), mOutMapPath, geoDef);
               temp[i].MySocialMap = this.mySocialMap;
            }
            fw.writeLine("done with loop now reinitializing this.myAnimalMaps");
            this.myAnimalMaps = new AnimalMap[temp.Length];
            fw.writeLine("now doing the copy");
            temp.CopyTo(this.myAnimalMaps, 0);
            fw.writeLine("now out of here");
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
      }

      public void makeNextGenerationAnimalMaps(int numResidents, int numDispersers)
      {
         try
         {
            AnimalMap[] temp = new AnimalMap[numResidents + numDispersers];
            // this.myAnimalMaps.CopyTo(temp,0);
            IGeometryDef geoDef = this.getSpatialInfo();
            for (int i = numResidents; i < numResidents + numDispersers; i++)
            {
               temp[i] = new AnimalMap(i.ToString(), mOutMapPath, geoDef);
               //set reference to social map so we can add those fields on the makeMap call
               temp[i].MySocialMap = this.mySocialMap;
            }
            this.myAnimalMaps = new AnimalMap[numResidents + numDispersers];
            temp.CopyTo(this.myAnimalMaps, 0);
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      public void makeNextGenerationAnimalMaps(AnimalManager am, string year)
      {
         try
         {
            fw.writeLine("inside makeNextGenerationAnimalMaps for year " + year);
            fw.writeLine("we are going to make " + am.Count.ToString() + "plus one maps");
            AnimalMap[] temp = new AnimalMap[am.Count + 1];
            fw.writeLine("temp is " + temp.Length + " long");
            // this.myAnimalMaps.CopyTo(temp,0);
            IGeometryDef geoDef = this.getSpatialInfo();
            foreach (Animal a in am)
            {
               fw.writeLine("Animal Number " + a.IdNum.ToString() + " is a " + a.GetType().Name);
               if (a.GetType().Name != "Resident" && a.IsDead != true)
               {
                  fw.writeLine("must not be a resident making map for " + a.IdNum.ToString());
                  temp[a.IdNum] = new AnimalMap(a.IdNum.ToString(), mOutMapPath, geoDef);
                  //set reference to social map so we can add those fields on the makeMap call
                  temp[a.IdNum].MySocialMap = this.mySocialMap;
               }
            }
            this.myAnimalMaps = new AnimalMap[temp.Length];
            temp.CopyTo(this.myAnimalMaps, 0);
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      //      private void removeUnionMaps()
      //      {
      //         string []fileNames;
      //         try
      //         {
      //            fileNames = Directory.GetFiles(mOutMapPath , "Union*");
      //            for(int i=0;i<fileNames.Length;i++)
      //            {
      //               File.Delete(fileNames[i]);
      //            }
      //
      //
      //         }
      //         catch(System.Exception ex)
      //         {
      //#if (DEBUG)
      //            System.Windows.Forms.MessageBox.Show(ex.Message);
      //#endif
      //            FileWriter.FileWriter.WriteErrorFile(ex);
      //         }
      //      }
      public bool removeExtraFiles(string FullFilePath)
      {
         string[] fileNames;
         string currDir = System.IO.Path.GetDirectoryName(FullFilePath);
         string fileName = System.IO.Path.GetFileNameWithoutExtension(FullFilePath);
         bool success = true;
         try
         {
            fileNames = Directory.GetFiles(currDir);
            for (int i = 0; i < fileNames.Length; i++)
            {
               if (fileNames[i].IndexOf(fileName) > 0)
               {
#if ! pat
                  File.Delete(fileNames[i]);
#endif
               }
            }

         }
         catch (System.Exception ex)
         {
            success = false;
            FileWriter.FileWriter.WriteErrorFile(ex);
         }


         return success;


      }

      public void setUpNewYearsMaps(DateTime now, AnimalManager am)
      {
         fw.writeLine("inside setUpNewYearsMaps (DateTime now,AnimalManager am)");
         fw.writeLine("DateTime = " + now.ToShortDateString() + " " + now.ToShortTimeString());
         mySocialMaps.changeMap(now, am);
         myFoodMaps.changeMap(now);
         myPredationMaps.changeMap(now);
         myMoveMaps.changeMap(now);
         myDispersalMaps.setUpNewYearDispersalMap(now, am);
      }

      /********************************************************************************
       *  Function name   : validateMap
       *  Description     : the traffic cop for validating the different types of imput
       *                    maps we need to run the simulation
       *  Return type     : bool 
       *  Argument        : string inMapName  name of map we are validating
       *  Argument        : string inMapDir   directory where we can find the maps
       * *******************************************************************************/
      public bool validateMap(string inmapType, ref string inFilename, string inPath)
      {
         bool success;
         string mapType = inmapType;
         fw.writeLine("inside validate Map for Map Manager ");
         fw.writeLine("the map name is " + inmapType);
         fw.writeLine("the dir is " + inPath);
         success = false;
         if (Directory.Exists(inPath))
         {
            fw.writeLine("ok the directory exsits so now open the workspace");
            this.wrkSpace = this.wrkSpaceFactory.OpenFromFile(inPath, 0);
            string[] fileNames;
            string[] fieldNames;

            fw.writeLine("the directory exists now get all the file names in the dir");
            fileNames = Directory.GetFiles(inPath, "*.shp");
            fw.writeLine("there were " + fileNames.Length.ToString() + " files found");
            if (fileNames.GetLength(0) > 0)
            {
               switch (inmapType)
               {
                  case "Social":
                     fw.writeLine("inside case under Social");
                     fieldNames = new string[3];
                     fieldNames[0] = "Suitabilit";
                     fieldNames[1] = "OCCUP_MALE";
                     fieldNames[2] = "OCCUP_FEMA";
                     break;
                  case "Food":
                     fw.writeLine("inside case under Food");
                     fieldNames = new string[3];
                     fieldNames[0] = "PROBCAP";
                     fieldNames[1] = "X_SIZE";
                     fieldNames[2] = "SD_SIZE";
                     break;
                  case "Predation":
                     fw.writeLine("inside case under Predation");
                     fieldNames = new string[1];
                     fieldNames[0] = "RISK";
                     break;
                  case "Dispersal":
                     fw.writeLine("inside case under Dispersal");
                     fieldNames = new string[3];
                     fieldNames[0] = "RELEASESIT";
                     fieldNames[1] = "MALES";
                     fieldNames[2] = "FEMS";
                     break;
                  case "Move":
                     fw.writeLine("inside case under Move");
                     fieldNames = new string[5];
                     fieldNames[0] = "MVL";
                     fieldNames[1] = "MSL";
                     fieldNames[2] = "ENERGYUSED";
                     fieldNames[3] = "CROSSING";
                     fieldNames[4] = "PR_X";
                     break;
                  default:
                     fw.writeLine("bombed with invalid name ");
                     System.Windows.Forms.MessageBox.Show("Not a valid map type" + inmapType);
                     return false;
               }// end switch on map name
               success = validateNFieldPolylMap(fileNames, fieldNames);
               if (success)
               {
                  inFilename = fileNames[0];
               }
               if (fileNames.Length > 1)
               {
                  fw.writeLine("More than one map in directory.  Used first (alphabetically) one.");
               }

            }
            else // no files in dir
            {
               fw.writeLine("did not find any shape files");
               this.errNumber = (int)ERR.NO_FILES_FOUND_IN_DIRECTORY;
               this.errFileName = inPath;
            }
         }
         else//directory does not exsit
         {
            this.errNumber = (int)ERR.NO_DIRECTORY_FOUND;
            this.errFileName = inPath;
            fw.writeLine("directory does not exist");
            success = false;
         }

         fw.writeLine("leaving validateMap with a value of " + success);
         return success;
      }

      public bool validateMap(string inMapName, string inMapDir)
      {
         bool success;
         fw.writeLine("inside validate Map for Map Manager ");
         fw.writeLine("the map name is " + inMapName);
         fw.writeLine("the dir is " + inMapDir);
         Check.Require(inMapName.Length > 0, "empty string for map type");
         Check.Require(inMapDir.Length > 0, "empty string for map dir");
         string[] fileNames;
         string[] fieldNames;
         string[] fileInfo;
         string mapType;
         success = false;
         if (Directory.Exists(inMapDir))
         {
            fw.writeLine("ok the directory exsits so now open the workspace");
            this.wrkSpace = this.wrkSpaceFactory.OpenFromFile(inMapDir, 0);

            fw.writeLine("the directory exists now get all the file names in the dir");
            fileNames = Directory.GetFiles(inMapDir, "*.shp");
            fw.writeLine("there were " + fileNames.Length.ToString() + " files found");
            if (fileNames.GetLength(0) > 0)
            {
               switch (inMapName)
               {
                  case "Social":
                     fw.writeLine("inside case under Social");
                     fieldNames = new string[3];
                     fieldNames[0] = "Suitabilit";
                     fieldNames[1] = "OCCUP_MALE";
                     fieldNames[2] = "OCCUP_FEMA";
                     success = validateNFieldPolylMap(fileNames, fieldNames);
                     if (success)
                     {
                        mapType = "Social";
                     }
                     break;
                  case "Food":
                     fw.writeLine("inside case under Food");
                     fieldNames = new string[3];
                     fieldNames[0] = "PROBCAP";
                     fieldNames[1] = "X_SIZE";
                     fieldNames[2] = "SD_SIZE";
                     success = validateNFieldPolylMap(fileNames, fieldNames);
                     if (success)
                     {
                        mapType = "Food";
                     }
                     break;
                  case "Predation":
                     fw.writeLine("inside case under Predation");
                     fieldNames = new string[1];
                     fieldNames[0] = "RISK";
                     success = validateNFieldPolylMap(fileNames, fieldNames);
                     if (success)
                     {
                        mapType = "Predation";
                     }
                     break;
                  case "Dispersal":
                     fw.writeLine("inside case under Dispersal");
                     fieldNames = new string[3];
                     fieldNames[0] = "RELEASESIT";
                     fieldNames[1] = "MALES";
                     fieldNames[2] = "FEMS";
                     success = validateNFieldPointMap(fileNames, fieldNames);
                     if (success)
                     {
                        mapType = "Dispersal";
                     }

                     break;
                  case "Move":
                     fw.writeLine("inside case under Move");
                     fieldNames = new string[5];
                     fieldNames[0] = "MVL";
                     fieldNames[1] = "MSL";
                     fieldNames[2] = "ENERGYUSED";
                     fieldNames[3] = "CROSSING";
                     fieldNames[4] = "PR_X";
                     success = validateNFieldPolylMap(fileNames, fieldNames);
                     if (success)
                     {
                        mapType = "Move";
                     }
                     break;
                  default:
                     fw.writeLine("bombed with invalid name ");
                     System.Windows.Forms.MessageBox.Show("Not a valid map type" + inMapName);
                     break;
               }// end switch on map name

            }
            else // no files in dir
            {
               fw.writeLine("did not find any shape files");
               this.errNumber = (int)ERR.NO_FILES_FOUND_IN_DIRECTORY;
               this.errFileName = inMapDir;
            }
         }
         else//directory does not exsit
         {
            this.errNumber = (int)ERR.NO_DIRECTORY_FOUND;
            this.errFileName = inMapDir;
            fw.writeLine("directory does not exist");
            success = false;
         }

         fw.writeLine("leaving validateMap with a value of " + success);
         return success;
      }

      public void writeXMLTriggers(ref XmlTextWriter xw)
      {
         mySocialMaps.writeXMLTriggers(ref xw);
         myFoodMaps.writeXMLTriggers(ref xw);
         myPredationMaps.writeXMLTriggers(ref xw);
         myMoveMaps.writeXMLTriggers(ref xw);
         myDispersalMaps.writeXMLTriggers(ref xw);
      }

		#endregion Public Methods 

   }
}
