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
 * Date:          Saturday, October 13, 2007 11:23:28 AM
 * Description:   Added GetInitialMapData(Animal inA) call.  Should help alleviate
 *                some of the pain with testing.
 * ***************************************************************************/

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesFile;
using System.IO;
using DesignByContract;
using System.Collections;
using System;
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
   public  class MapManager
   {
      //      public  int myMaps;
      private IWorkspace wrkSpace;
      private IWorkspaceFactory wrkSpaceFactory;//  = new ShapefileWorkspaceFactory();
      private IFeatureWorkspace featureWrkSpace; 
      public Maps mySocialMaps = null;
      public Maps myFoodMaps = null;
      public Maps myPredationMaps = null;
      public Maps myMoveMaps = null;
      public Maps myDispersalMaps = null;
      private Map mySocialMap = null;
      private Map myFoodMap = null;
      private Map myPredationMap = null;
      private Map myDispersalMap = null;
      private Map myMoveMap = null;
      private string mOutMapPath;
      private AnimalMap [] myAnimalMaps;
      private Hashtable myHash;
      private FileWriter.FileWriter fw;
      private System.Collections.Specialized.StringCollection errMessages;
      private int errNumber;
      private string errFileName;
      private MapManipulator myMapManipulator;
      private DateTime mCurrTime; 
      private int SocialIndex;

      private static MapManager uniqueInstance;
      private enum ERR
      {
         WRONG_TYPE_OF_SHAPE_FILE,
         CAN_NOT_FIND_REQUIRED_FIELD,
         NO_FILES_FOUND_IN_DIRECTORY,
         DIRECTORY_ALREADY_IN_USE,
         NO_DIRECTORY_FOUND
      }

      #region publicMethods  
      private MapManager()
      {
         //check on whether we are going to log or not
         this.buildLogger();
         myMapManipulator=new MapManipulator();
         // get the error messages ready         
         initializeErrMessages();
         
         //create the shapefile workspace factory
         wrkSpaceFactory = new ShapefileWorkspaceFactoryClass();
         this.myFoodMaps = new Maps("Food");
         this.myMoveMaps = new Maps("Move");
         this.myPredationMaps = new Maps("Predation");
         this.mySocialMaps = new Maps("Social");
         this.myDispersalMaps=new Maps("Dispersal");
         SocialIndex=0;
         
      }

      public static MapManager GetUniqueInstance()
      {

         if(uniqueInstance == null) 
         { 
            uniqueInstance = new MapManager();
         }
         return uniqueInstance;
      }
      
      
      public void AddTimeSteps(int AnimalID, IPolygon inPoly1,IPolygon inPoly2,int timeStep,string sex)
      {
         try
         {
            //add the stuff to step map and end up with an occupied map that has 
            //already made the decision as to available or not available
            fw.writeLine("inside AddTimeSteps for ManManager the animal id is " +AnimalID.ToString());
            fw.writeLine("calling AddTimeStep");
            myMapManipulator.addTimeStep(inPoly1,inPoly2,sex);
            fw.writeLine("back in AddTimeSteps now calling dissolveSteps");
            myMapManipulator.dissolveSteps();
            fw.writeLine("back from AddTimeSteps now calling do Occupied Stuff");
            myMapManipulator.doOccupiedStuff(sex);
            if(timeStep == 0)
            { 
               //if the first time through then we only need to add it to the map
               this.myMapManipulator.copyOccupiedToAnimalMap(this.myAnimalMaps[AnimalID]);
            }
            else
            { 
               //myMapManipulator.unionTimeStep(this.myAnimalMaps[AnimalID]);
               fw.writeLine("Back from doOccupied stuff in AddTimeSteps now calling union time step");
               myMapManipulator.unionTimeStep(this.myAnimalMaps[AnimalID],AnimalID.ToString(), timeStep.ToString());
               fw.writeLine("Back from unionTimeStepin AddTimeSteps now calling copyUnionToAnimalMap");
               myMapManipulator.copyUnionToAnimalMap(this.myAnimalMaps[AnimalID]);
               fw.writeLine("Back from copyUnionToAnimalMap in AddTimeSteps now calling remove uninon maps");
               
            }  
            fw.writeLine("Now calling dissovle maps");
            this.myAnimalMaps[AnimalID].dissolveAvailablePolygons(timeStep.ToString());
            fw.writeLine("Back from dissolveAvailablePolygons in AddTimeSteps now calling myAnimalMaps[AnimalID].explode");
            this.myAnimalMaps[AnimalID].explode(timeStep.ToString());

         }
         catch(System.Exception ex)
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
      public void changeMaps(DateTime now,AnimalManager am)
      {
         fw.writeLine("inside changeMaps (DateTime now,AnimalManager am)");
         fw.writeLine("DateTime = " + now.ToShortDateString() + " " + now.ToShortTimeString());
         mySocialMaps.changeMap(now,am);
         myFoodMaps.changeMap(now);
         myPredationMaps.changeMap(now);
         myMoveMaps.changeMap(now);
         myDispersalMaps.changeMap(now,am);
      }
      public void changeMaps(DateTime now)
      {
         mySocialMaps.changeMap(now);
         myFoodMaps.changeMap(now);
         myPredationMaps.changeMap(now);
         myMoveMaps.changeMap(now);
         myDispersalMaps.changeMap(now);
      }



      public crossOverInfo getCrossOverInfo(IPoint startPoint,IPoint endPoint, int currPolyIndex)
      {
         fw.writeLine("inside map manger getCrossOverInfo");
         return this.myMoveMap.getCrossOverInfo(startPoint,endPoint,currPolyIndex);
      }
      public AnimalMap getAnimalMap(int index)
      {
         AnimalMap am = null;
         try
         {
            if (index >= 0 && index < this.myAnimalMaps.GetLength(0))
            {
               am = this.myAnimalMaps[index];
            }

         }
         catch(System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show (ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return am;
      }
      public double getArea(int index)
      {
         double area = 0;
         try
         {
            area=this.mySocialMap.getAvailableArea(index);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return area;
      }
      public int getCurrPolygon(IPoint inPoint)
      {
         fw.writeLine("inside mapmanger calling map get current poly gon for x = " + inPoint.X.ToString() + " Y " + inPoint.Y.ToString());
         return this.myMoveMap.getCurrentPolygon(inPoint);
      }
      public double getDistance(IPoint start,IPoint end)
      { 
         IPolyline p = new PolylineClass();
         try
         {
            p.FromPoint = start;
            p.ToPoint = end;
         }
         catch(System.Exception ex)
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
            errMessage= "File Name: " + errFileName + System.Environment.NewLine +  errMessages[errNumber];
            errNumber = -1;
         }
         else 
         {
            errMessage="no error";
         }
         return errMessage;

      }
      
      public void GetInitialAnimalAttributes(out InitialAnimalAttributes [] outAttributes)
      {
         outAttributes = null;
         try
         {
            this.myDispersalMap.GetInitialAnimalAttributes(out outAttributes);
         }
         catch(System.Exception ex)
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
         this.myHash = this.myMoveMap.getAllValuesForSinglePolygon(PolyIndex);
         inA.MoveTurtosity = System.Convert.ToDouble( myHash["MVL"]);
         inA.MoveSpeed = System.Convert.ToDouble(myHash["MSL"]);
         inA.EnergyUsed = System.Convert.ToDouble(myHash["ENERGYUSED"]);
         inA.PerceptonModifier = System.Convert.ToDouble(myHash["PR_X"]);
         inA.MoveIndex = PolyIndex;
         

         fw.writeLine("now getting initial food data ");
         PolyIndex = myFoodMap.getCurrentPolygon (inA.Location);
         myHash = myFoodMap.getAllValuesForSinglePolygon(PolyIndex);
         inA.CaptureFood = System.Convert.ToDouble(myHash["PROBCAP"]);
         inA.FoodMeanSize = System.Convert.ToDouble(myHash["X_SIZE"]);
         inA.FoodSD_Size = System.Convert.ToDouble(myHash["SD_SIZE"]);
         inA.FoodIndex = PolyIndex;

         fw.writeLine("now getting initial risk data");
         PolyIndex = myPredationMap.getCurrentPolygon (inA.Location);
         inA.PredationRisk = System.Convert.ToDouble(myPredationMap.getNamedValueForSinglePolygon(PolyIndex,"RISK"));
         inA.RiskIndex = PolyIndex;
      }
      public void GetMoveModifiers(IPoint inPoint, ref int PolyIndex,ref double MVL,ref double MSL, 
         ref double PerceptionModifier,ref double EnergyUsed)
      {
         try
         {
            fw.writeLine("inside mapManager GetMoveModifiers checking to see if we moved out of the current polygon");
            
            if(! this.myMoveMap.pointInPolygon(inPoint,PolyIndex))
            {
               fw.writeLine("we must have moved so get new polyIndex");
               fw.writeLine("old index is " + PolyIndex.ToString());
               fw.writeLine("old values are MSL = " + MSL.ToString() + " MVL + " + MVL.ToString() + " energyused is " + EnergyUsed.ToString() + " PerceptionModifier = " +  PerceptionModifier.ToString());
               fw.writeLine("go to Map Log file to see new values.");
               PolyIndex = this.myMoveMap.getCurrentPolygon(inPoint);
            }
            this.myHash = this.myMoveMap.getAllValuesForSinglePolygon(PolyIndex);
            MVL = System.Convert.ToDouble( myHash["MVL"]);
            MSL = System.Convert.ToDouble(myHash["MSL"]);
            EnergyUsed = System.Convert.ToDouble(myHash["ENERGYUSED"]);
            PerceptionModifier = System.Convert.ToDouble(myHash["PR_X"]);
            
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
        
      }

      public void GetFoodData(IPoint location,ref int PolyIndex,ref double chance,ref double mean, ref double sd)
      {
         try
         { 
            fw.writeLine("inside get food data for poly index " + PolyIndex.ToString());
            //check to see if we have moved out of the food index if so then adjust
            //Author: Bob Cummings moved all logic inside if statement.  No sense 
            //doing it unless the values changed.
            if (! myFoodMap.pointInPolygon(location,PolyIndex))
            {
               PolyIndex = myFoodMap.getCurrentPolygon (location);
            }
            myHash = myFoodMap.getAllValuesForSinglePolygon(PolyIndex);
            chance = System.Convert.ToDouble(myHash["PROBCAP"]);
            mean = System.Convert.ToDouble(myHash["X_SIZE"]);
            sd = System.Convert.ToDouble(myHash["SD_SIZE"]);
            
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }
      public void GetRiskModifier(IPoint location, ref int PolyIndex,ref double risk)
      {
         
         try
         {
            fw.writeLine("incoming risk value is " + risk.ToString());
            fw.writeLine("inside GetRiskModifier checking to see if we moved out of the orginal polygon ");
            fw.writeLine("the poly index is " + PolyIndex.ToString());
            //check to see if we have moved out of the food index if so then adjust
            //Author: Bob Cummings moved all logic inside if statement.  No sense 
            //doing it unless the values changed.
            if (! this.myPredationMap.pointInPolygon(location,PolyIndex))
            {
               fw.writeLine("must have changed now getting the new polygon");
               PolyIndex = myPredationMap.getCurrentPolygon (location);
            }
            fw.writeLine("now getting the risk value");
            risk = System.Convert.ToDouble(myPredationMap.getNamedValueForSinglePolygon(PolyIndex,"RISK"));
            fw.writeLine("out going risk is " + risk.ToString());
            
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
        
        
      }
      public void getSocialIndex(IPoint inPoint,ref int inPolygonIndex)
      {
        
         try
         {
            fw.writeLine("");
            fw.writeLine("inside getSocialIndex for location x = " + inPoint.X.ToString() + " y = " + inPoint.Y.ToString());
            fw.writeLine("mySocialMap name is " + this.mySocialMap.mySelf.AliasName);
            if (! mySocialMap.pointInPolygon(inPoint,inPolygonIndex))
            {
               inPolygonIndex = mySocialMap.getCurrentPolygon(inPoint);
            }
           

         }
         catch(System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show (ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         fw.writeLine("leaving getSocialIndex with an index of " + inPolygonIndex.ToString());
         fw.writeLine("");
         
      }
     
      public IFeatureClass getTimeStepMap(string AnimalID)
      {
         string[] fileNames;
         string path;
         IFeatureClass fc = null;
         try
         {
            fw.writeLine("inside getTimeStepMap for " + AnimalID);
            path = this.mOutMapPath + "\\" + AnimalID;
            fw.writeLine("full path for that animal is " + path);
            fw.writeLine("now get the file names");
            fileNames = Directory.GetFiles(path,"TimeStep*");
            fw.writeLine("we have " + fileNames.Length.ToString() + " files to look for ");
            fw.writeLine("calling Map openFeatureClass");
            fc = Map.openFeatureClass(path,System.IO.Path.GetFileNameWithoutExtension(fileNames[0]));
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return fc;
         
      }
     
      
      

      public bool isOccupied(IPoint inPoint)
      {
         bool occupied = true;
         int polyIndex = this.mySocialMap.getCurrentPolygon(inPoint);
         string s = this.mySocialMap.getNamedValueForSinglePolygon(polyIndex,"OCCUP_MALE").ToString();
         s = s.ToUpper();
         if (s == "NONE")
         {
            s = this.mySocialMap.getNamedValueForSinglePolygon(polyIndex,"OCCUP_FEMA").ToString();
            s = s.ToUpper();
            if (s == "NONE")
            {
               occupied = false;
            }
         }
         return occupied;
      }
      public bool isOccupied(int inPolyIndex,string sex, ref FileWriter.FileWriter inFw)
      {
         bool occupied = true;
         string s;
         inFw.writeLine("inside isOccupied checking for " + sex);
         if (sex == "Male")
         {
            s = this.mySocialMap.getNamedValueForSinglePolygon(inPolyIndex,"OCCUP_MALE").ToString();
        
         }
         else
         { 
            s = this.mySocialMap.getNamedValueForSinglePolygon(inPolyIndex,"OCCUP_FEMA").ToString();
         }
        
         inFw.writeLine("that returns " + s);
         s = s.ToUpper();
         if (s == "NONE")
         {
            occupied = false;
         }
         return occupied;
      }
      public bool makeNewAnimalMaps(int numAnimals)
      {
         bool success = true;
         int i=0;
         try
         {
            IGeometryDef geoDef = this.getSpatialInfo();
            fw.writeLine("inside make new animal map for " + numAnimals.ToString() + " number of animals");
            myAnimalMaps = new AnimalMap[numAnimals];
            for(i=0;i<numAnimals && success ;i++)
            {
               myAnimalMaps[i] = new AnimalMap(i.ToString(),mOutMapPath,geoDef);
               //set reference to social map so we can add those fields on the makeMap call
               myAnimalMaps[i].MySocialMap = this.mySocialMap;
               
            }
            fw.writeLine("leaving make new animal maps");
         }
         catch(System.Exception ex)
         {
            success = false;
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         if (success == false)
         {
            this.errNumber = (int) ERR.DIRECTORY_ALREADY_IN_USE;
            this.errFileName = mOutMapPath + @"\" +i.ToString();
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
            AnimalMap [] temp = new AnimalMap[totalNumMaps];
            fw.writeLine("temp is " + temp.Length + " long");
            fw.writeLine("now copy existing maps to new temp array there are " + currNumMaps.ToString() + " to copy");
            this.myAnimalMaps.CopyTo(temp,0);
            IGeometryDef geoDef = this.getSpatialInfo();
            fw.writeLine("starting loop");
            for (int i = currNumMaps; i<totalNumMaps;i++)
            {
               temp[i] = new AnimalMap(i.ToString(),mOutMapPath,geoDef);
               temp[i].MySocialMap = this.mySocialMap;
            }
            fw.writeLine("done with loop now reinitializing this.myAnimalMaps");
            this.myAnimalMaps = new AnimalMap[temp.Length];
            fw.writeLine("now doing the copy");
            temp.CopyTo(this.myAnimalMaps,0);
            fw.writeLine("now out of here");
         }
         catch(System.Exception ex)
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
            AnimalMap [] temp = new AnimalMap[numResidents+numDispersers];
            // this.myAnimalMaps.CopyTo(temp,0);
            IGeometryDef geoDef = this.getSpatialInfo();
            for(int i=numResidents;i<numResidents+numDispersers;i++)
            {
               temp[i] = new AnimalMap(i.ToString(),mOutMapPath ,geoDef);
               //set reference to social map so we can add those fields on the makeMap call
               temp[i].MySocialMap = this.mySocialMap;
            }
            this.myAnimalMaps = new AnimalMap[numResidents+numDispersers];
            temp.CopyTo(this.myAnimalMaps,0);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      public void makeNextGenerationAnimalMaps(AnimalManager am,string year)
      {
         try
         {
            fw.writeLine("inside makeNextGenerationAnimalMaps for year " + year);
            fw.writeLine("we are going to make " + am.Count.ToString() + "plus one maps");
            AnimalMap [] temp = new AnimalMap[am.Count + 1];
            fw.writeLine("temp is " + temp.Length + " long");
            // this.myAnimalMaps.CopyTo(temp,0);
            IGeometryDef geoDef = this.getSpatialInfo();
            foreach(Animal a in am)
            {
               fw.writeLine("Animal Number " + a.IdNum.ToString() + " is a " + a.GetType().Name );
               if(a.GetType().Name != "Resident" && a.IsDead != true)
               {
                  fw.writeLine("must not be a resident making map for " + a.IdNum.ToString());
                  temp[a.IdNum] = new AnimalMap(a.IdNum.ToString(),mOutMapPath ,geoDef);
                  //set reference to social map so we can add those fields on the makeMap call
                  temp[a.IdNum].MySocialMap = this.mySocialMap;
               }
            }
            this.myAnimalMaps = new AnimalMap[temp.Length];
            temp.CopyTo(this.myAnimalMaps,0);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      public void makeHomeRange(Animal inA)
      {
         try
         {
            fw.writeLine("inside make home range for animial number " + inA.IdNum.ToString());
            //this actually makes the new social map 
            //file name is 
            if(myMapManipulator.makeHomeRange(inA))
            {
               fw.writeLine("ok myMapManipulator.makeHomeRange returned true so create the home range");
               createAnimalHomeRangeMap(inA.IdNum,inA.FileNamePrefix);
               fw.writeLine("now try to change out the social map");
               changeOutSocialMap();
            }
         }
         catch(System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show (ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         

      }
      public bool makeTempMaps(string path)
      {
         bool success;
         IGeometryDef geoDef = this.getSpatialInfo();
         success = this.myMapManipulator.buildTempMaps(path,geoDef);
         return success;
      }
      public bool pointInPolygon(IPoint inPoint,int inPolygonIndex)
      {
         return myMoveMap.pointInPolygon(inPoint,inPolygonIndex);
      }
     
      
      public void setUpNewYearsMaps(DateTime now,AnimalManager am)
      {
         fw.writeLine("inside setUpNewYearsMaps (DateTime now,AnimalManager am)");
         fw.writeLine("DateTime = " + now.ToShortDateString() + " " + now.ToShortTimeString());
         mySocialMaps.changeMap(now,am);
         myFoodMaps.changeMap(now);
         myPredationMaps.changeMap(now);
         myMoveMaps.changeMap(now);
         myDispersalMaps.setUpNewYearDispersalMap(now,am);
      }


      
      public void writeXMLTriggers(ref XmlTextWriter xw)
      {
         mySocialMaps.writeXMLTriggers(ref xw);
         myFoodMaps.writeXMLTriggers(ref xw);
         myPredationMaps.writeXMLTriggers(ref xw);
         myMoveMaps.writeXMLTriggers(ref xw);
         myDispersalMaps.writeXMLTriggers(ref xw);
      }

      public void loadXMLTriggers(string mapType,XPathNodeIterator inIterator)
      {
         try
         {
            switch (mapType)
            {
               case"Social":
                  mySocialMaps.loadXMLTriggers(inIterator);
                  break;
               case"Food":
                  myFoodMaps.loadXMLTriggers(inIterator);
                  break;
               case"Predation":
                  myPredationMaps.loadXMLTriggers(inIterator);
                  break;
               case"Move":
                  myMoveMaps.loadXMLTriggers(inIterator);
                  break;
               case"Dispersal":
                  myDispersalMaps.loadXMLTriggers(inIterator);
                  break;
               default:
                  throw new ArgumentException(mapType + " is an invalid mapType in loadXMLTriggers in MapManagerClass");
                  
            }

         }
         catch (ArgumentException ae)
         {
            System.Windows.Forms.MessageBox.Show(ae.Message);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         
      }
      
      
     
      /********************************************************************************
       *  Function name   : validateMap
       *  Description     : the traffic cop for validating the different types of imput
       *                    maps we need to run the simulation
       *  Return type     : bool 
       *  Argument        : string inMapName  name of map we are validating
       *  Argument        : string inMapDir   directory where we can find the maps
       * *******************************************************************************/
      public bool validateMap(string inmapType,ref string inFilename, string inPath)
      {
         bool success;
         string mapType=inmapType;
         fw.writeLine("inside validate Map for Map Manager ");
         fw.writeLine("the map name is " + inmapType);
         fw.writeLine("the dir is " + inPath);
         success = false;
         if (Directory.Exists(inPath))
         {
            fw.writeLine("ok the directory exsits so now open the workspace");
            this.wrkSpace = this.wrkSpaceFactory.OpenFromFile(inPath,0);
            string[] fileNames;
            string[] fieldNames;
           
            fw.writeLine("the directory exists now get all the file names in the dir");
            fileNames=Directory.GetFiles(inPath,"*.shp");
            fw.writeLine("there were " + fileNames.Length.ToString() + " files found");
            if (fileNames.GetLength(0) > 0)
            {
               switch (inmapType)
               {
                  case "Social":
                     fw.writeLine("inside case under Social");
                     fieldNames = new string [3];
                     fieldNames[0] = "Suitabilit";
                     fieldNames[1] = "OCCUP_MALE";
                     fieldNames[2] = "OCCUP_FEMA";
                     break;
                  case "Food":
                     fw.writeLine("inside case under Food");
                     fieldNames = new string [3];
                     fieldNames[0] = "PROBCAP";
                     fieldNames[1] = "X_SIZE";
                     fieldNames[2] = "SD_SIZE";
                     break;
                  case "Predation":
                     fw.writeLine("inside case under Predation");
                     fieldNames = new string [1];
                     fieldNames[0] = "RISK";
                     break;
                  case "Dispersal":
                     fw.writeLine("inside case under Dispersal");
                     fieldNames = new string [3];
                     fieldNames[0] = "RELEASESIT";
                     fieldNames[1] = "MALES";
                     fieldNames[2] = "FEMS";
                     break;
                  case "Move":
                     fw.writeLine("inside case under Move");
                     fieldNames = new string [5];
                     fieldNames[0] = "MVL";
                     fieldNames[1] = "MSL";
                     fieldNames[2] = "ENERGYUSED";
                     fieldNames[3] = "CROSSING";
                     fieldNames[4] = "PR_X";
                     break;
                  default:
                     fw.writeLine("bombed with invalid name ");
                     System.Windows.Forms.MessageBox.Show ("Not a valid map type" + inmapType);
                     return false;
               }// end switch on map name
               success = validateNFieldPolylMap(fileNames,fieldNames);
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
               this.errNumber = (int) ERR.NO_FILES_FOUND_IN_DIRECTORY;
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
         string mapType="";
         fw.writeLine("inside validate Map for Map Manager ");
         fw.writeLine("the map name is " + inMapName);
         fw.writeLine("the dir is " + inMapDir);
         Check.Require(inMapName.Length > 0,"empty string for map type");
         Check.Require(inMapDir.Length > 0,"empty string for map dir");
         string[] fileNames;
         string[] fieldNames;
         string[] fileInfo;
         success = false;
         if (Directory.Exists(inMapDir))
         {
            fw.writeLine("ok the directory exsits so now open the workspace");
            this.wrkSpace = this.wrkSpaceFactory.OpenFromFile(inMapDir,0);
            
            fw.writeLine("the directory exists now get all the file names in the dir");
            fileNames=Directory.GetFiles(inMapDir,"*.shp");
            fw.writeLine("there were " + fileNames.Length.ToString() + " files found");
            if (fileNames.GetLength(0) > 0)
            {
               switch (inMapName)
               {
                  case "Social":
                     fw.writeLine("inside case under Social");
                     fieldNames = new string [3];
                     fieldNames[0] = "Suitabilit";
                     fieldNames[1] = "OCCUP_MALE";
                     fieldNames[2] = "OCCUP_FEMA";
                     success = validateNFieldPolylMap(fileNames,fieldNames);
                     if (success)
                     {
                        mapType = "Social";
                     }
                     break;
                  case "Food":
                     fw.writeLine("inside case under Food");
                     fieldNames = new string [3];
                     fieldNames[0] = "PROBCAP";
                     fieldNames[1] = "X_SIZE";
                     fieldNames[2] = "SD_SIZE";
                     success = validateNFieldPolylMap(fileNames,fieldNames);
                     if (success)
                     {
                        mapType = "Food";
                     }
                     break;
                  case "Predation":
                     fw.writeLine("inside case under Predation");
                     fieldNames = new string [1];
                     fieldNames[0] = "RISK";
                     success = validateNFieldPolylMap(fileNames,fieldNames);
                     if (success)
                     {
                        mapType = "Predation";
                     }
                     break;
                  case "Dispersal":
                     fw.writeLine("inside case under Dispersal");
                     fieldNames = new string [3];
                     fieldNames[0] = "RELEASESIT";
                     fieldNames[1] = "MALES";
                     fieldNames[2] = "FEMS";
                     success = validateNFieldPointMap(fileNames,fieldNames);
                     if (success)
                     {
                        mapType = "Dispersal";
                     }
                     
                     break;
                  case "Move":
                     fw.writeLine("inside case under Move");
                     fieldNames = new string [5];
                     fieldNames[0] = "MVL";
                     fieldNames[1] = "MSL";
                     fieldNames[2] = "ENERGYUSED";
                     fieldNames[3] = "CROSSING";
                     fieldNames[4] = "PR_X";
                     success = validateNFieldPolylMap(fileNames,fieldNames);
                     if (success)
                     {
                        mapType = "Move";
                     }
                     break;
                  default:
                     fw.writeLine("bombed with invalid name ");
                     System.Windows.Forms.MessageBox.Show ("Not a valid map type" + inMapName);
                     break;
               }// end switch on map name
              
            }
            else // no files in dir
            {
               fw.writeLine("did not find any shape files");
               this.errNumber = (int) ERR.NO_FILES_FOUND_IN_DIRECTORY;
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
      #endregion     
      
      #region privateMethods  
      private void AddMemoryPoly(int AnimalID, IPolygon inPoly1,IPolygon inPoly2,int timeStep,string sex)
      {
         try
         {
            // myMapManipulator.addTimeStep(inPoly1,inPoly2,sex);
         }
         catch(System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }
      private void addNewPoly(Map inMap,IPolygon inPoly)
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
            if (index >=0)
               feature.set_Value(index,1);
            index = fields.FindField("OCCUP_MALE");
            if (index >=0)
               feature.set_Value(index,"Bob");
            feature.Shape = inPoly;
            feature.Store();
         }
         catch(System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
          

      }
      private void AddFirstMemoryPoly(int AnimalID, IPolygon inPoly1,IPolygon inPoly2)
      {
         try
         {
            this.myAnimalMaps[AnimalID].addPolygon(inPoly1);
            this.myAnimalMaps[AnimalID].addPolygon(inPoly2);
           
         }
         catch(System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }
      
      private  void addSuitableTerritory(int inAnimalID)
      {
         try
         {
            IFeatureClass tmpFeature = null; 
            this.myMapManipulator.intersectFeatures(ref tmpFeature,
               myAnimalMaps[inAnimalID].StepTable,
               inAnimalID.ToString() + "suitable");
            myAnimalMaps[inAnimalID].SuitableFeatureClass = tmpFeature;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(tmpFeature);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }
      
      private void changeOutSocialMap()
      {
         string [] fileNames;
         string path = this.mySocialMap.Path;
         string extension;
         string newFileName = "NewSocial" + this.SocialIndex.ToString();
         IFeatureClass fc = null;
         try
         {
            fw.writeLine("inside changeOutSocialMap");
            System.Threading.Thread.Sleep(1000);
            this.mySocialMap = null;
            this.myMapManipulator.SocialMap = null;
            fileNames = Directory.GetFiles(this.mOutMapPath,"HomeRange*");
            for(int i=0;i<fileNames.Length;i++)
            {
               fw.writeLine("The file names are " + fileNames[i]);
               extension = fileNames[i].Substring(fileNames[i].Length-4,4);
               fw.writeLine("the extension is " + extension);
               File.Copy(fileNames[i],this.mOutMapPath+"\\" + newFileName + extension,true);
               //this will delete the files we no longer need
               fw.writeLine("now delete " + fileNames[i]);
               File.Delete(fileNames[i]);
            }
            fc = Map.openFeatureClass(this.mOutMapPath,newFileName);
            this.mySocialMap = new Map(fc);
            this.mySocialMap.Path = path;
            this.myMapManipulator.SocialMap = this.mySocialMap;
            this.SocialIndex++;
         

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         
      }

     

      private DateTime createStartTime(string time,string typeOfTime)
      {
         string []s;
         DateTime dt= DateTime.Today;
         try
         { 
            switch(typeOfTime)
            {  
               case "daily":
                  s = time.Split('/');
                  int day = System.Convert.ToInt32(s[1]);
                  int month= System.Convert.ToInt32(s[0]);
                  int year= System.Convert.ToInt32(s[2]);
                  dt = new DateTime(year,month,day);
                  break;
               case "hourly":
                  dt = DateTime.Today;
                  //the time is in the format of 7:32 PM
                  //so split it up and deal with it you wancker
                  s =time.Split(':');
                  dt=dt.AddHours( System.Convert.ToInt32(s[0]));
                  dt=dt.AddMinutes(System.Convert.ToInt32(s[1].Substring(0,2)));
                  if (time.IndexOf("PM") > 0)
                     dt=dt.AddHours(12);
                  break;
               case "yearly":
                  dt = DateTime.Today;
                  dt=dt.AddYears(System.Convert.ToInt32(time)- dt.Year);
                  break;
               case "static":
                  dt = DateTime.Today;
                  dt=dt.AddYears(System.Convert.ToInt32(time)- dt.Year);
                  break;
               default:
                  throw new SystemException("Bad type of time in Map Manager createStartTime");
               
            }

         }
         catch(System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show (ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
        
         return dt;
      }
      private void createAnimalHomeRangeMap(int inAnimalID,string filePrefix)
      {
         string [] fileNames;
         string extension;
         string newFileName;
         System.Text.StringBuilder sb = new System.Text.StringBuilder();
         try
         {
            
            sb.Append(filePrefix + "_" + "homerange");
         
            // get the path to make the file 
            string path = this.getAnimalMap(inAnimalID).Path +@"\homerange\" ;
            //HACK for remove extra fields below
            string filename = sb.ToString();
            //add that to the new file name
            sb.Insert(0,path);
            //make the new dir and file name
            Directory.CreateDirectory(path);
            newFileName = sb.ToString();
         
            fileNames = Directory.GetFiles(this.mOutMapPath,"HomeRange*");
            for(int i=0;i<fileNames.Length;i++)
            {
               extension = fileNames[i].Substring(fileNames[i].Length-4,4);
               File.Copy(fileNames[i],newFileName + extension,true);
            }
            // this.removeExtraFields(path,filename);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }


      }
     
      
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
               if (s.IndexOf("MapManagerPath") == 0)
               {
                  fw= FileWriter.FileWriter.getMapManagerLogger(s.Substring(s.IndexOf(" ")));
                  
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
       *  Function name   : validateNFieldPolylMap
       *  Description     : Will check to make sure it is a valid polygon shape file and 
       *                    has the required number of fields
       *  Return type     : bool 
       *  Argument        : string [] inMapFileNames all the file names of the map we need 
       *  Argument        : int NumFields the number of field the maps are supposed to have
       * *******************************************************************************/
      
      
     
      
      public IGeometryDef getSpatialInfo()
      {
         IGeometryDef geoDef=null;
         try
         {
            IField f;
            int fieldIndex = 0;
            IFeatureCursor fc;
            fc = this.mySocialMap.mySelf.Search(null,false);
            fieldIndex = fc.FindField("SHAPE");
            f = fc.Fields.get_Field(fieldIndex);
            geoDef = f.GeometryDef;
         }
         catch(System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return geoDef;
      } 

      private string [] getStartTimes(string [] inOutMaps,string mapType)
      {
         
         //         frmMapTimes fmt = new frmMapTimes();
         //         fmt.setLable(mapType);
         //         fmt.fillListView(inOutMaps);
         //         fmt.ShowDialog();
         //         string [] s = fmt.OutFileNamesAndStartTimes;
         //         fmt.Close();
         //         return s;
         return null;
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
      /********************************************************************************
      *   Function name   : loadMap
       *   Description     : loads the maps that need to be loaded for this part of the
       *                     simulation 
       *   Return type     : void 
       *   Argument        : string inMapType is the type of map "Social,Food" etc
       *   Argument        : string fileName is the actual name of the shape file to open
       * ********************************************************************************/
      public void loadOneMap(string inMapType,string fileName,string inPath)
      {
         try
         {
            fw.writeLine("inside load map for " + inMapType);
            //sometimes we are loading from an XML file and so we have to check that the
            //map is actually available
            String fullName = inPath + "\\" + fileName + ".shp";
            if(File.Exists(fullName))
            {
               fw.writeLine("the file we want to load is " + inPath + " " + fileName);
               //the file name is the fully qualifed path and name
               //all we use is the name of the shape file itself
               this.wrkSpace = this.wrkSpaceFactory.OpenFromFile(inPath,0);
               featureWrkSpace = (IFeatureWorkspace)this.wrkSpace;
               fileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
               switch (inMapType)
               {
                  case "Social":
                     fw.writeLine("inside case under Social loading the map");
                     if (mySocialMap == null)
                     {
                    
                        mySocialMap = new Map(this.featureWrkSpace.OpenFeatureClass(fileName));
                     }
                     else
                     {
                        this.wrkSpace = this.wrkSpaceFactory.OpenFromFile(mOutMapPath,0);
                        featureWrkSpace = (IFeatureWorkspace)this.wrkSpace;
                        this.makeMapCopies(inPath,fileName,this.mOutMapPath,"tmpSocial");
                        Map tempMap = new Map(this.featureWrkSpace.OpenFeatureClass("tmpSocial"));
                        this.myMapManipulator.unionSocialMaps(tempMap);
                        this.myMapManipulator.editNewSocialMap("Social");
                        mySocialMap = new Map(this.featureWrkSpace.OpenFeatureClass("Social"));
                   
                     
                     }
                     mySocialMap.TypeOfMap = "Social";
                     mySocialMap.Path = inPath;
                     //add the refernce to the map manipulator
                     this.myMapManipulator.SocialMap = this.mySocialMap;
                     break;
                  case "Food":
                     fw.writeLine("inside case under Food loading the map");
                     if(myFoodMap != null)
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
                     myMoveMap.TypeOfMap="Move";
                     myMoveMap.Path = inPath;
                     break;
                  default:
                     fw.writeLine("bombed with invalid name ");
                     System.Windows.Forms.MessageBox.Show ("Not a valid map type" + inMapType);
                     break;
               }// end switch on map name
            }
            else
            {
               throw new SystemException(fullName + " Not found");
            }

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         
      }

     
      private void makeMapCopies(string orgPath, string orgFileName, string newPath, string newFileName)   
      {
         string [] fileNames;
         string extension;
         try
         {
            fileNames = Directory.GetFiles(orgPath,orgFileName + "*");
            for(int i=0;i<fileNames.Length;i++)
            {
               extension = fileNames[i].Substring(fileNames[i].Length-4,4);
               File.Copy(fileNames[i],newPath + "\\" + newFileName + extension,true);
            }
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      private void removeTimeStepMaps(string AnimalID)
      {
         string []fileNames;
         try
         { 
            //use the animal id because each animal has its own sub folder based
            //on its name.
            fileNames = Directory.GetFiles(mOutMapPath + "\\" + AnimalID,"TimeStep" + "*");
            for(int i=0;i<fileNames.Length;i++)
            {
               File.Delete(fileNames[i]);
            }

         }
         catch(System.Exception ex)
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
      public bool removeExtraFiles(string fileName)
      {
         string []fileNames;
         bool success = true;
         try
         {
            fileNames = Directory.GetFiles(mOutMapPath);
            for(int i=0;i<fileNames.Length;i++)
            {
               if(fileNames[i].IndexOf(fileName) > 0)
               {
                  File.Delete(fileNames[i]);
               }
            }

         }
         catch(System.Exception ex)
         {
            success = false;
            FileWriter.FileWriter.WriteErrorFile(ex);
         }

         
         return success;


      }
      private bool validateNFieldPolylMap(string [] inMapFileNames,string [] inFieldNames)
      {
         int i;
         int found = -1;
         bool result;
         string fileName;
         IFeatureClass myShapefile = null;
         
         
         Check.Require(inFieldNames.Length > 0,"No field names to check in ValidatePolyMap");
         Check.Require(inMapFileNames.Length > 0,"No files to check in validateNFieldPolylMap");

         result = true;//assume we are good to go
         fw.writeLine("inside validateNFieldPolylMap going to look for " + inFieldNames.Length.ToString() + " fields");
         fw.writeLine("set the feature workspace");
         featureWrkSpace = (IFeatureWorkspace)this.wrkSpace;
         for (i=0;i<inMapFileNames.GetLength(0);i++)
         {
            fileName = System.IO.Path.GetFileNameWithoutExtension(inMapFileNames[i]);             
            fw.writeLine("found " + inMapFileNames[i]);
            
            //create the feature_class
            myShapefile = this.featureWrkSpace.OpenFeatureClass(fileName);
            fw.writeLine("now check for correct type of shape file");
            if (myShapefile.ShapeType.ToString() == "esriGeometryPolygon")
            {	
               
               fw.writeLine("must be a polygon now check for the required field names");
               for(int j=0;j<inFieldNames.Length;j++)
               {
                  fw.writeLine("looking for " + inFieldNames[j]);
                  found = myShapefile.Fields.FindField(inFieldNames[j]);
                  fw.writeLine("the result from the find field is " + found.ToString());
                  if(found < 0)
                  {
                     this.errNumber = (int) ERR.CAN_NOT_FIND_REQUIRED_FIELD;
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
               this.errNumber = (int) ERR.WRONG_TYPE_OF_SHAPE_FILE;
               this.errFileName = fileName;
            }//end checking to see if it is a polygon file 
         }//end loop
         fw.writeLine("leaving validateNFieldPolylMap with a value of " + result.ToString());
         return result;
      }// end validating the maps

      /********************************************************************************
       *  Function name   : validateNFieldPointMap
       *  Description     : Will check to make sure it is a valid point shape file and 
       *                    has the required number of fields
       *  Return type     : bool 
       *  Argument        : string [] inMapFileNames the names of the map files to validate
       *  Argument        : int NumFields  the number of fields the maps should have
       * *******************************************************************************/
      
      private bool validateNFieldPointMap(string [] inMapFileNames,string [] inFieldNames)
      {
         int i;
         int found = -1;
         bool result;
         string fileName;
         IFeatureClass myShapefile = null;
         
         
         Check.Require(inFieldNames.Length > 0,"No field names to check in validateNFieldPointMap");
         Check.Require(inMapFileNames.Length > 0,"No files to check in validateNFieldPointMap");

         result = true;//assume we are good to go
         fw.writeLine("inside validateNFieldPointMap going to look for " + inFieldNames.Length.ToString() + " fields");
         fw.writeLine("set the feature workspace");
         featureWrkSpace = (IFeatureWorkspace)this.wrkSpace;
         for (i=0;i<inMapFileNames.GetLength(0);i++)
         {
            fileName = System.IO.Path.GetFileNameWithoutExtension(inMapFileNames[i]);             
            fw.writeLine("found " + inMapFileNames[i]);
            
            //create the feature_class
            myShapefile = this.featureWrkSpace.OpenFeatureClass(fileName);
            fw.writeLine("now check for correct type of shape file");
            if (myShapefile.ShapeType.ToString() == "esriGeometryPoint")
            {	
               
               fw.writeLine("must be a point now check for the required field names");
               for(int j=0;j<inFieldNames.Length;j++)
               {
                  fw.writeLine("looking for " + inFieldNames[j]);
                  found = myShapefile.Fields.FindField(inFieldNames[j]);
                  fw.writeLine("the result from the find field is " + found.ToString());
                  if(found < 0)
                  {
                     this.errNumber = (int) ERR.CAN_NOT_FIND_REQUIRED_FIELD;
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
               this.errNumber = (int) ERR.WRONG_TYPE_OF_SHAPE_FILE;
               this.errFileName = fileName;
            }//end checking to see if it is a point file 
         }//end loop
         fw.writeLine("leaving validateNFieldPolylMap with a value of " + result.ToString());
         return result;
      }// end validating the maps
      
      #endregion

      #region gettersAndSetters
      public string OutMapPath
      {
         get { return mOutMapPath; }
         set 
         {
            mOutMapPath = value;
            this.myMapManipulator.myPath = mOutMapPath;}
      }

      public Map SocialMap
      {
         get { return mySocialMap; }
         set  { mySocialMap = value; }
      }

      public DateTime CurrTime
      {
         get{return mCurrTime;}
         set{ mCurrTime = value;}
      }
      #endregion
    
   }
}
