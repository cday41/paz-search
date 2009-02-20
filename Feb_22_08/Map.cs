using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using System;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using System.Collections;
namespace PAZ_Dispersal
{
   public class Map
   {
      #region privateData
      private DateTime mBeginTime;
      protected FileWriter.FileWriter fw;
      protected IFeatureClass mMySelf;
      private IFeature myFeature;
      protected IFeatureLayer myFeatureLayer;
      private IField myField;
      private IMap myMap;
      private object myObject;
      protected IWorkspaceName wsName;
      protected IDatasetName dsName;
      protected IFeatureClassName outShapeFileName;
      protected string myPath;
      protected string myFileName;
      protected string mTypeOfMap;
      private string mChangeType;
      private Hashtable myHash;
      #endregion
      #region gettersAndSetters

      public string ChangeType
		{
			get { return mChangeType; }
			set { mChangeType = value; }
		}

      public DateTime BeginTime
		{
			get { return mBeginTime; }
			set { mBeginTime = value; }
		}

      public string fullFileName
      {
         get {return myPath + "\\" + myFileName;}
      }
      public string TypeOfMap
      {
         get { return mTypeOfMap; }
         set  { mTypeOfMap = value; }
      }

      public IFeatureClass mySelf
      {
         get { return mMySelf; }
         set { mMySelf = value; }
      }
      public string Path
      {
         get { return this.myPath; }
         set {this.myPath = value;}
      }
      
      #endregion

      public Map()
      {
         buildLogger();
         outShapeFileName = new FeatureClassNameClass();
         wsName = new WorkspaceNameClass();
         myHash = new Hashtable();
         this.myFeatureLayer = new FeatureLayerClass();
         this.myMap = new MapClass();
         this.myFeature = null;
        
      }
    
      public Map(string path,string fileName,IGeometryDef inGeoDef,IFields inFieldsCollection):this()
      {
        
         try
         {
            myPath = path;
            myFileName = fileName;
            IWorkspaceFactory shpWkspFactory = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace shapeWksp = null;
            IPropertySet connectionProperty = new PropertySetClass();
            IGeometryDefEdit geoDef = new GeometryDefClass();
            IFieldsEdit fieldsEdit = (IFieldsEdit)inFieldsCollection;
            
            connectionProperty.SetProperty ("DATABASE",path);
            shapeWksp = (IFeatureWorkspace) shpWkspFactory.Open(connectionProperty,0);
            this.mySelf = shapeWksp.CreateFeatureClass(fileName,fieldsEdit,null,null,esriFeatureType.esriFTSimple,"Shape","");


         }
         catch(System.Exception ex)
         {
           
            if (ex.Source == "ESRI.ArcGIS.Geodatabase")
            {
               //mErrMessage = "That Directory is full with maps already";
               throw new System.Exception("That Directory is full with maps already");
            }
#if DEBUG
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
           
         }
        
      }

     
      public Map(IFeatureClass inSelf):this()
      {
         mMySelf = inSelf;
      }
      
      #region publicMethods
      public void getMap()
      {
      }
      /********************************************************************************
       *   Function name   : getCrossOverInfo
       *   Description     : If the movement of an animal is going to cross from one
       *                     polygon to another we need to know how far he would
       *                     go in one polygon, where the animal would cross.
       * 
       * // how much do I like the polygon i am in and how much do I like the other polygon
       *   Return type     : crossOverInfo data structre for returning the data cleanly
       *   Argument        : IPoint startPoint  movement starting point
       *   Argument        : IPoint endPoint    movement endiing point
       *  
       * ********************************************************************************/
      public void addDay()
      {
         fw.writeLine("inside add day for " + this.mySelf.AliasName + " adding a day to the start time");
         this.BeginTime = this.BeginTime.AddDays(1);
      }
      public void addYear()
      {
         fw.writeLine("inside add year for " + this.mySelf.AliasName + " adding a year to the start time");
         fw.writeLine("before adding year = " + this.BeginTime.ToShortDateString());
         this.BeginTime = this.BeginTime.AddYears(1);
         fw.writeLine("after adding year = " + this.BeginTime.ToShortDateString());
      }
      public void addField(string name,esriFieldType type)
      {
         IFieldEdit fieldEdit;
         fieldEdit = new FieldClass();
         fieldEdit.Name_2 = name;
         fieldEdit.Type_2 = type;//esriFieldType.esriFieldTypeSmallInteger;
         this.mySelf.AddField(fieldEdit);


      }
      public crossOverInfo getCrossOverInfo(IPoint startPoint,IPoint endPoint, int newPolyIndex)
      {
         crossOverInfo coInfo = new crossOverInfo();
         IPolyline tempLine = new PolylineClass();
         PointClass tempPoint = null;
         IPolygon tempPolygon = null;
         IRelationalOperator relOp = null;
         ITopologicalOperator topo = null;
         IMultipoint tempGeom = null;
         IPointCollection pointCollection = null;
         IFeature currFeaturePolygon = null;
         IFeature newFeaturePolygon = null;
         IRowBuffer rowBuf = null;
        
         IField field = null;
         int tempPolyIndex = 0;

         try
         {
           
            fw.writeLine("inside map get cross over info making the line");
            fw.writeLine("the start point is " + startPoint.X.ToString() + " " + startPoint.Y.ToString());
            fw.writeLine("the end point is " + endPoint.X.ToString() + " " + endPoint.Y.ToString());
            fw.writeLine("the poly index is " + newPolyIndex.ToString());
            //make the polyline between startPoint and endPoint and create the relational operator
            tempLine.FromPoint = startPoint;
            tempLine.ToPoint = endPoint;
            fw.writeLine("the distance between the two points is " + tempLine.Length.ToString() + " should be as long as a step length");
            relOp = (IRelationalOperator)tempLine;
           
            //get the current polygon from the map
            //get feature returns an IFeature object so we need to cast it
            //to an IGeometry type to use the IRelationalOperator in this
            //case we are going to cast it to a polygon
           
            
            currFeaturePolygon = this.mySelf.GetFeature(newPolyIndex-1);
           
            tempPolygon = (IPolygon) currFeaturePolygon.ShapeCopy;
            fw.writeLine("the polygon we want to cross is " + newPolyIndex);
            fw.writeLine("now check to make sure we really cross for some reason");
            //get tempPoint where tempLine crosses the tempPoly
            //but sometimes the turosity of new landscape makes him want to turn around
            if (relOp.Crosses(tempPolygon))
            {
               topo = (ITopologicalOperator)tempLine;
               //this is stupid the intersect will return a multipoint even if it only has one point
               tempGeom = (IMultipoint)topo.Intersect(tempPolygon,esriGeometryDimension.esriGeometry0Dimension);
               pointCollection = (IPointCollection) tempGeom;
               fw.writeLine("there are " + pointCollection.PointCount.ToString() + " points on the intersections");
               fw.writeLine("the entry in the collection has x = " + pointCollection.get_Point(0).X.ToString() + " y = " + pointCollection.get_Point(0).Y .ToString());
               //TODO ERROR ON CROSS OVER
               //tempPoint =(PointClass) pointCollection.get_Point(0);
               tempPoint = new PointClass();
               tempPoint.X = pointCollection.get_Point(0).X;
               tempPoint.Y = pointCollection.get_Point(0).Y;
               fw.writeLine("TEST 1");
               tempLine.ToPoint = tempPoint;
               fw.writeLine("TEST 2");
               coInfo.Point = tempPoint;
               fw.writeLine("TEST 3");
            }
            else
            {
               coInfo.Point = (PointClass)endPoint;
            }
         
            //make new line for getting the distace from where we start to the border
            //tempLine.ToPoint = tempPoint;
            fw.writeLine("the distance George is going to travel in this polygon is " + tempLine.Length.ToString());


            //get the distance from the start to the border
            coInfo.Distance = tempLine.Length;

            //current polygon value
            //tempPolyIndex = getCurrentPolygon(startPoint);
            //currFeaturePolygon = this.mySelf.GetFeature(tempPolyIndex);
            rowBuf = (IRowBuffer)currFeaturePolygon;

            for(int i=0;i<rowBuf.Fields.FieldCount;i++)
            {
               field = rowBuf.Fields.get_Field(i);
               if(field.Name == "CROSSING")
               {
                  coInfo.CurrPolyValue = System.Convert.ToDouble(rowBuf.get_Value(i));
                  break;
               }
            }

            //new polygon value
            tempPolyIndex = getCurrentPolygon(endPoint);
            newFeaturePolygon = this.mySelf.GetFeature(tempPolyIndex);
            rowBuf = (IRowBuffer)newFeaturePolygon;

            for(int i=0;i<rowBuf.Fields.FieldCount;i++)
            {
               field = rowBuf.Fields.get_Field(i);
               if(field.Name == "CROSSING")
               {
                  coInfo.NewPolyValue = System.Convert.ToDouble(rowBuf.get_Value(i));
                  break;
               }
            }
         }
         catch(System.Exception ex)
         {
            //TODO REMOVE BEFORE RELEASE
            System.Windows.Forms.MessageBox.Show ("error we are looking for has occured, grab files and send to bob");
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return coInfo;
      }

      public void disovleFeatures(string fieldName)
      {
         ITable myTable = null;
         IDatasetName dsName = null;
         IBasicGeoprocessor bgp = new BasicGeoprocessorClass();
         IFeatureClassName fcn = new FeatureClassNameClass();
         try
         { 
            fcn.FeatureType=this.mySelf.FeatureType;
            fcn.ShapeType = this.mySelf.ShapeType;
            fcn.ShapeFieldName = this.mySelf.ShapeFieldName;

            dsName=(IDatasetName)fcn;
            dsName.Name = "testDisolove";
            dsName.WorkspaceName=this.getWorkspaceName();
           
            myTable = this.getTable();
            if (myTable.FindField(fieldName) >= 0)
            {
               ITable t = bgp.Dissolve(myTable,false,fieldName,"Dissolve.Shape,Minimum."+fieldName,dsName);
            }
            else
            {
               System.Windows.Forms.MessageBox.Show(fieldName + " can not be found for dissolve");
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
      public bool hasFeatures()
      {
         return this.mySelf.FeatureCount(null) > 0;
      }
      public double getArea(IPoint inPoint)
      {
         double d = -1;
         IFeature tempPoly = null;
         IArea tempArea;
         int polyIndex = -1;
         try
         {
            polyIndex = this.getCurrentPolygon(inPoint);
            tempPoly = this.mySelf.GetFeature(polyIndex);
            tempArea = tempPoly.Shape as IArea;
            d = tempArea.Area;  

         }
         catch(System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show (ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return d;
      }
      //      public void GetMoveModifiers(int PolyIndex,ref double MVL,ref double MSL, ref double PerceptionModifier, ref double EnergyUsed)
      //      {
      //         MVL = 1;
      //         MSL = 1;
      //         EnergyUsed = 1;
      //         // PerceptionModifier = 1;
      //         IFeature currPoly = null;
      //         currPoly = this.mySelf.GetFeature(PolyIndex);
      //         IRowBuffer rowBuf =(IRowBuffer)currPoly;
      //         IField field = null;
      //         
      //         // fw.writeLine("");
      //         for(int i=0;i<rowBuf.Fields.FieldCount;i++)
      //         {
      //            field = rowBuf.Fields.get_Field(i);
      //            switch (field.Name)
      //            {
      //               case "MVL":
      //                  MVL = MVL * System.Convert.ToDouble(rowBuf.get_Value(i));
      //                  break;
      //               case "MSL":
      //                  MSL = MSL * System.Convert.ToDouble(rowBuf.get_Value(i));
      //                  break;
      //               case "ENERGYUSED":
      //                  EnergyUsed = EnergyUsed * System.Convert.ToDouble(rowBuf.get_Value(i));
      //                  break;
      //               default:
      //                  break;
      //            }//end switch
      //         }//end looping through the fields
      //      }
      //
      //      public void GetFoodData(IPoint location,ref double chance,ref double mean, ref double sd)
      //      {
      //         IFeature currPoly = null;
      //         IField field = null;
      //         int currPolyIndex = 0;
      //
      //     
      //         try
      //         {
      //            currPolyIndex = this.getCurrentPolygon(location);
      //            fw.writeLine("inside get food data on map looking for the polygon at index " + currPolyIndex.ToString());
      //            //currPolyIndex--;
      //            currPoly = this.mySelf.GetFeature(currPolyIndex);
      //            IRowBuffer rowBuf =(IRowBuffer)currPoly;
      //         
      //            // fw.writeLine("");
      //            for(int i=0;i<rowBuf.Fields.FieldCount;i++)
      //            {
      //               field = rowBuf.Fields.get_Field(i);
      //               switch (field.Name)
      //               {
      //                  case "PROBCAP":
      //                     chance = System.Convert.ToDouble(rowBuf.get_Value(i));
      //                     break;
      //                  case "X_SIZE":
      //                     mean = System.Convert.ToDouble(rowBuf.get_Value(i));
      //                     break;
      //                  case "SD_SIZE":
      //                     sd = System.Convert.ToDouble(rowBuf.get_Value(i));
      //                     break;
      //                  default:
      //                     break;
      //               }//end switch
      //            }//end looping through the fields
      //        
      //            fw.writeLine("leaving getfood with the following values");
      //            fw.writeLine("chance =  " + chance.ToString());
      //            fw.writeLine("x size = " + mean.ToString());
      //            fw.writeLine("sd size = " + sd.ToString());
      //         }
      //         catch(System.Exception ex)
      //         {
      //            FileWriter.FileWriter.WriteErrorFile(ex);
      //         }
      //         
      //      }
      //      public void GetRiskModifier(int PolyIndex,ref double risk)
      //      {
      //         IFeature currPoly = null;
      //         IField field = null;
      //         IRowBuffer rowBuf = null;
      //
      //         try
      //         {
      //            currPoly = this.mySelf.GetFeature(PolyIndex);
      //            rowBuf =(IRowBuffer)currPoly;
      //            for(int i=0;i<rowBuf.Fields.FieldCount;i++)
      //            {
      //               field = rowBuf.Fields.get_Field(i);
      //               if (field.Name == "RISK")
      //               {
      //                  risk = risk * System.Convert.ToDouble(rowBuf.get_Value(i));
      //                  break;
      //               }
      //            }
      //         }
      //         catch(System.Exception ex)
      //         {
      //            FileWriter.FileWriter.WriteErrorFile(ex);
      //         }
      //      }


     
      /********************************************************************************
       *   Function name   : makeNewMap
       *   Description     : 
       *   Return type     : void 
       * ********************************************************************************/
     
      
      public static IFeatureClass getMap(string path, string fileName)
      {
         IFeatureClass ifc = null;
         string newPath = path + "\\" + fileName;
         try
         {
            IWorkspaceFactory wrkSpaceFactory = new ShapefileWorkspaceFactory();
            IFeatureWorkspace featureWorkspace=null;
            featureWorkspace = (IFeatureWorkspace)wrkSpaceFactory.OpenFromFile(newPath,0);
            ifc = featureWorkspace.OpenFeatureClass(fileName);
         }
         catch(System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return ifc;
      }
      public static IFeatureClass openFeatureClass(string path,string fileName)
      {
         IName name=null;
         try
         {
            // Create the workspace name object
            IWorkspaceName workspaceName = new WorkspaceNameClass();
            workspaceName.PathName = path;
            workspaceName.WorkspaceFactoryProgID = "esriDataSourcesFile.ShapefileWorkspaceFactory";

            // Create the feature class name object
            IDatasetName datasetName = new FeatureClassNameClass();
            datasetName.Name          = fileName;
            datasetName.WorkspaceName = workspaceName;

            // Open the feature class
            name = (IName) datasetName;
		
            
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         // Return FeaureClass
         return (IFeatureClass) name.Open();

      }

      /********************************************************************************
       *   Function name   : pointInPolygon
       *   Description     : checks to see if the point is in the specific polygon
       *   Return type     : bool   true if it is and false otherwise
       *   Argument        : IPoint inPoint  the point we are concerned about
       *   Argument        : int inPolygonIndex index of the polygon to look in
       * ********************************************************************************/
      public bool pointInPolygon(IPoint inPoint, int inPolygonIndex)
      {
         bool success = false;
         IFeature tempPoly = null;
         IPolygon searchPoly = null;
         IRelationalOperator relOp = null;
         fw.writeLine("");
         fw.writeLine("inside pointInPolygon checking to see if the point is in the polygon");
         fw.writeLine("point is at x = " + inPoint.X.ToString() + " y = " + inPoint.Y.ToString() );
         fw.writeLine("the polygon checking is " + inPolygonIndex);
         fw.writeLine("the map is " + this.mySelf.AliasName);
         try
         {
            //cast the point object into the relational operator 
            relOp = (IRelationalOperator)inPoint;
            tempPoly = this.mySelf.GetFeature(inPolygonIndex);
            //get feature returns an IFeature object so we need to cast it
            //to an IGeometry type to use the IRelationalOperator in this
            //case we are going to cast it to a polygon
            searchPoly = (IPolygon)tempPoly.ShapeCopy;
            if (searchPoly != null)
            {
               if(relOp.Within(searchPoly))
               {
                  success = true;
               }
            }
         }
         catch(System.Exception ex)
         {
           
            FileWriter.FileWriter.WriteErrorFile(ex);
   
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
         }
         
         fw.writeLine("leaving with a value of " + success.ToString());
         fw.writeLine("");
            return success;
        

      }
      
      /********************************************************************************
       *  Function name   : getAllValuesForSinglePolygon
       *  Description     : finds the specific polygon requested.  Then loops throug
       *                    all the fields in the database row for that polygon.
       *                    and adds them to the hash table with the name as the key
       *                    and the value as an objec.  The calling methos must know 
       *                    how to convert the object based on the names.
       *  Return type     : Hashtable 
       *  Argument        : int inPolyIndex
       * *******************************************************************************/
      

      public Hashtable getAllValuesForSinglePolygon(int inPolyIndex)
      {
         fw.writeLine("inside getAllValuesForSinglePolygon for poly index" + inPolyIndex.ToString());
         try
         {
            //clear out the hashtable
            this.myHash.Clear();
            //get the feature
            myFeature = this.mySelf.GetFeature(inPolyIndex);
            //loop through all the fields in the database
            for(int i=0;i<myFeature.Fields.FieldCount;i++)
            {
               myField = myFeature.Fields.get_Field(i);
               //do not want the two fields that esri adds
               if ((myField.Type != esriFieldType.esriFieldTypeGeometry) &&
                  (myField.Type != esriFieldType.esriFieldTypeOID))
               {
                  fw.writeLine("getting the value for " + myField.Name);
                  this.myHash.Add(myField.Name,myFeature.get_Value(i));
               }
            }
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         fw.writeLine("leaving getAllValuesForSinglePolygon");
         return this.myHash;
	
      }
      
      public double getAvailableArea(int inPolyIndex)
      {
         double area = 0;
         try
         {
            IFeature f = this.mySelf.GetFeature(inPolyIndex);
            IArea a = (IArea)f.ShapeCopy;
            //TODO divide by 1000 power of 2 for moving square meters to square kilos
            area = a.Area/(1000^2);
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
      public double getAllAvailableArea(int inAnimalID, string sex)
      {
         IArea a;
         double area = 0;double d = 0;
         string PolyID;
         IFeatureCursor search = null;
         try
         {
            IFeature f = null;
            
            IQueryFilter qf = new QueryFilterClass();
            if (sex.ToUpper() == "MALE")
               qf.WhereClause = "OCCUP_MALE = '" + inAnimalID.ToString() + "'";
            else
               qf.WhereClause = "OCCUP_FEMA = '" + inAnimalID.ToString() + "'";

            search = this.mySelf.Search(qf,false);
            f = search.NextFeature();
            while (f != null)
            {
               a = (IArea)f.ShapeCopy;
               //TODO divide by 1000 power of 2 for moving square meters to square kilos
               d = a.Area /(1000^2);
               area += d;
               fw.writeLine("f.OID") ;
               f = search.NextFeature();
            }


         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         finally
         {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(search);
         }
         return area;
      }
      public void GetInitialAnimalAttributes(out InitialAnimalAttributes [] outAttributes)
      {
         IPoint tmpPoint = null;
         IFeature tmpFeature = null;
         IFeatureClass tmpFeatureClass = null;
         IFeatureCursor tmpCur = null;
         IRowBuffer rowBuff;
         
         ArrayList tmpArrayList = new ArrayList();
         int numToMake=0;
         int fieldIndex;

         InitialAnimalAttributes tempIAA;

         tmpFeatureClass = mySelf;

         tmpCur = tmpFeatureClass.Search(null,false);
         tmpFeature = tmpCur.NextFeature();

         if (tmpFeature.Shape.GeometryType.ToString() == "esriGeometryPoint")
         {
            while (tmpFeature != null)
            {
               //make an attribute from the first row in the data base

               //Author: Bob CummingsFriday, October 13, 2006 9:55:59 AM
               //commented out during refactor may have to put back in
              // tempIAA = new InitialAnimalAttributes();

               //have to have a row buffer to get the value out of the database
               rowBuff = (IRowBuffer)tmpFeature;

               tmpPoint = (IPoint)tmpFeature.ShapeCopy;
               fieldIndex = tmpFeature.Fields.FindField("MALES");
               if (fieldIndex >= 0)
               { 
                  numToMake = System.Convert.ToInt32( rowBuff.get_Value(fieldIndex));
                  if(numToMake > 0)
                  {
                     //Author: Bob CummingsFriday, October 13, 2006 9:55:59 AM
                     //Moved logic inside if statement.  May have to move back out
                     tempIAA = new InitialAnimalAttributes();
                     tempIAA.setPointValues(tmpPoint);
                     tempIAA.Sex = 'M';
                     tempIAA.NumToMake = numToMake;
                     tempIAA.NumToMake=System.Convert.ToInt32( rowBuff.get_Value(fieldIndex));
                     tmpArrayList.Add(tempIAA);
                  }
               }

               //Author: Bob CummingsFriday, October 13, 2006 9:55:59 AM
               //commented out during refactor may have to put back in
//               else
//               {
//                  numToMake = 0;
//               }
//               tempIAA.setPointValues(tmpPoint);
//               tempIAA.Sex = 'M';
//               tempIAA.NumToMake = numToMake;
//               tmpArrayList.Add(tempIAA);

              
               fieldIndex = tmpFeature.Fields.FindField("FEMS");
               if (fieldIndex >= 0)
               { 
                  numToMake = System.Convert.ToInt32(rowBuff.get_Value(fieldIndex));
                  if(numToMake>0)
                  {
                     tempIAA = new InitialAnimalAttributes();
                     tempIAA.setPointValues(tmpPoint);
                     tempIAA.Sex = 'F';
                     tempIAA.NumToMake = numToMake;
                     tempIAA.NumToMake = System.Convert.ToInt32(rowBuff.get_Value(fieldIndex));
                     tmpArrayList.Add(tempIAA);
                  }
               }
//               else
//               {
//                  numToMake = 0;
//               }
//               tempIAA.setPointValues(tmpPoint);
//               tempIAA.Sex = 'F';
//               tempIAA.NumToMake = numToMake;
//               tmpArrayList.Add(tempIAA);
               tmpFeature = tmpCur.NextFeature();

            }
         }
         outAttributes = new InitialAnimalAttributes[tmpArrayList.Count];
         tmpArrayList.CopyTo(outAttributes);

        
      }
      
      public IFeatureLayer getLayer()
      {
         myMap.ClearLayers();
         this.myFeatureLayer.FeatureClass = mMySelf;
         this.myFeatureLayer.Name = this.mySelf.AliasName;
         this.myMap.AddLayer(this.myFeatureLayer);
         ILayer l = myMap.get_Layer(0);
         return (IFeatureLayer)l;

      }
      public object getNamedValueForSinglePolygon(int inPolyIndex,string inName)
      {
         this.myObject=null;
         try
         {
            int index;
            //get the polygon we want
            fw.writeLine("inside getNamedValueForSinglePolygon polyindex is " + inPolyIndex.ToString() + " value we want is " + inName);
            myFeature = this.mySelf.GetFeature(inPolyIndex);
            index=myFeature.Fields.FindField(inName);
            myObject = myFeature.get_Value(index);

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         

         return myObject;

      }

      public ITable getTable()
      {
         ITable t;
         t = (ITable)getLayer();
         return t;
      }

      public IWorkspaceName getWorkspaceName()
      {
         IWorkspaceName wsName=null;
         try
         {
            wsName = new WorkspaceNameClass();
            wsName.PathName=this.myPath;
            wsName.WorkspaceFactoryProgID = "esriCore.ShapeFileWorkspaceFactory.1";
         }
         catch(System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return wsName;
      }
      public void dissolvePolygons(string inFieldName)
      {
         try
         {
            IBasicGeoprocessor bgp = new BasicGeoprocessorClass();
            fw.writeLine("inside map mergepolygons clear out any exsiting layers");
            this.myMap.ClearLayers();
            fw.writeLine("make the feature class layer and add to map");
            this.myFeatureLayer = (IFeatureLayer)getLayer();
            ITable t = (ITable) this.myFeatureLayer;
            fw.writeLine("now work on the workspace thing");
            IWorkspaceName wsName = getWorkspaceName();
            IFeatureClassName outShapeFileName = new FeatureClassNameClass();
            outShapeFileName.FeatureType = mMySelf.FeatureType;
            outShapeFileName.ShapeType = mMySelf.ShapeType;
            outShapeFileName.ShapeFieldName = mMySelf.ShapeFieldName;
            IDatasetName dsName=(IDatasetName)outShapeFileName;
            dsName.Name = this.myFileName;
            dsName.WorkspaceName = wsName;

            t = bgp.Dissolve(t,false,inFieldName,"Dissolve.Shape",dsName);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }
      public void removeAllPolygons()
      {
         try
         {
            IFeatureCursor tmpCur;
            IFeature tmpFeature;
            tmpCur = this.mySelf.Update(null,false);
            tmpFeature = tmpCur.NextFeature();

            while(tmpFeature != null)
            {
               tmpCur.DeleteFeature();
               tmpFeature = tmpCur.NextFeature();
            }
            tmpCur.Flush();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(tmpCur);
            
         }
         catch(System.Exception ex)
         {
#if DEBUG
            System.Windows.Forms.MessageBox.Show (ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         
      }
      public void removeAllPolygons(ref IFeatureClass inFeatureClass)
      {
         IFeatureCursor tmpCur;
         IFeature tmpFeature;
         tmpCur = inFeatureClass.Update(null,false);
         tmpFeature = tmpCur.NextFeature();

         while(tmpFeature != null)
         {
            tmpFeature.Delete();
            tmpFeature = tmpCur.NextFeature();
         }
         tmpCur.Flush();
         int j = inFeatureClass.FeatureCount(null);




      }
      public void removeCertainPolygons(string fieldName,string fieldValue)
      {
         IQueryFilter qf = new QueryFilterClass();
         qf.WhereClause = fieldName + "=" + fieldValue;
         IFeatureCursor tmpCur;
         IFeature tmpFeature;
         tmpCur = mySelf.Search(qf,false);
         tmpFeature = tmpCur.NextFeature();

         while(tmpFeature != null)
         {
            tmpFeature.Delete();
            tmpFeature = tmpCur.NextFeature();
         }
      }

      public void resetFields(string fieldName, string oldValue, string newValue)
      {  
         IQueryFilter qf = null;
         IFeatureCursor tmpCur = null;
         IFeature tmpFeature = null;
         int fieldIndex;
         try
         {
            fw.writeLine("inside reset fields we are going to change " + fieldName + "from " + oldValue + " to " + newValue);
            qf = new QueryFilterClass();
            qf.WhereClause = fieldName + "='" + oldValue +"'";
            tmpCur = mySelf.Update(qf,false);
            tmpFeature = tmpCur.NextFeature();
            
            while(tmpFeature != null)
            {
               fieldIndex = tmpFeature.Fields.FindField(fieldName);
               fw.writeLine("before update value was " + tmpFeature.get_Value(fieldIndex).ToString());
               tmpFeature.set_Value(fieldIndex,newValue);
               tmpCur.UpdateFeature(tmpFeature);
               fw.writeLine("after update value is " + tmpFeature.get_Value(fieldIndex).ToString());
               tmpFeature = tmpCur.NextFeature();
            }

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         fw.writeLine("leaving resetFields");
      }

  
      #endregion



      #region privateMethods


      protected void buildDataSetName(string inName)
      {
         dsName=(IDatasetName)outShapeFileName;
         dsName.Name = inName;
         dsName.WorkspaceName = wsName;
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
               if (s.IndexOf("MapLogPath") == 0)
               {
                  fw = FileWriter.FileWriter.getMapLogger(s.Substring(s.IndexOf(" ")));
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
      
      protected void buildWorkSpaceName()
      {
         wsName.PathName = myPath;
         wsName.WorkspaceFactoryProgID = "esriCore.ShapeFileWorkspaceFactory.1";

      }
      protected void buildOutShapeFileName()
      {
         try
         {
            outShapeFileName.FeatureType = esriFeatureType.esriFTSimple;
            outShapeFileName.ShapeType = esriGeometryType.esriGeometryPolygon;
            outShapeFileName.ShapeFieldName = "Shape";

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
       *   Function name   : getCurrentPolygon
       *   Description     : takes the point in and tells us which polygon it is in.
       *   Return type     : int    the index for the ploygon
       *   Argument        : IPoint inPoint  the location we are wondering about
       * ********************************************************************************/

      public int getCurrentPolygon (IPoint inPoint)
      {
         int polyIndex = 0;
        
         try
         {
            fw.writeLine("inside Map getCurrentPolygon for X = " + inPoint.X.ToString() + " Y " + inPoint.Y.ToString());
            //each individual polygon in the shapefile
            IPolygon searchPoly = null;
            IRelationalOperator relOp = null;
            IFeature tempPoly = null;
            //used to enumerate through the all the polygons
            IFeatureCursor polyCursor = null;

            relOp = (IRelationalOperator)inPoint;
            //fill the cursor with all polygons
            polyCursor = this.mySelf.Search(null,false);
            
            fw.writeLine("the cursor has " + polyCursor.Fields.FieldCount.ToString() + " number of fields");
            //get the feature class 
            tempPoly = polyCursor.NextFeature();
            fw.writeLine("have the first poly gon");
            while(tempPoly != null)
            {
             
               // then cast it to a polygon
               searchPoly = (IPolygon)tempPoly.ShapeCopy;
               if(relOp.Within(searchPoly))
               {
                  fw.writeLine("found it time to leave");
                  //found it so time to leave
                  break;
               }
               //keep going until we find what we are looking for
               tempPoly = polyCursor.NextFeature();
               polyIndex++;
            }

         }
         catch(System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return polyIndex;

      }

      private double getPolyValue(int polyIndex, string fieldName)
      {
         double returnValue = 0.0;
         IFeature currPoly = null;
         IField field = null;
         try
         {
            //get the polygon from the map and a database reader for the polygon values
            currPoly = this.mySelf.GetFeature(polyIndex);
            IRowBuffer rowBuf =(IRowBuffer)currPoly;

            //loop through and find the matching name
            for(int i=0;i<rowBuf.Fields.FieldCount;i++)
            {
               field = rowBuf.Fields.get_Field(i);
               if (field.Name == fieldName)
               {
                  returnValue = System.Convert.ToDouble(rowBuf.get_Value(i));
               }
            }
         }
         catch(System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return returnValue;
      }
   

      private void getTable(ref ITable inOutTable)
      {
         IMap myMap = new MapClass();
         IFeatureLayer tempFLayer = new FeatureLayerClass();
         ILayer tempLayer;

         tempFLayer.FeatureClass = mySelf;
         tempFLayer.Name = mySelf.AliasName;
         myMap.AddLayer(tempFLayer);
         tempLayer = myMap.get_Layer(0);
         inOutTable = (ITable)tempLayer;
      }
      protected void getTable(out ITable inOutTable, IFeatureClass inFeatureClass)
      {
         IMap myMap = new MapClass();
         IFeatureLayer tempFLayer = new FeatureLayerClass();
         ILayer tempLayer;

         tempFLayer.FeatureClass = inFeatureClass;
         tempFLayer.Name = inFeatureClass.AliasName;
         myMap.AddLayer(tempFLayer);
         tempLayer = myMap.get_Layer(0);
         inOutTable = (ITable)tempLayer;
      }
      protected void removePolygon(IFeature inFeature)
      {
         IFeatureCursor tmpCur;
         IFeature tmpFeature;
         try
         {
               fw.writeLine("inside remove polgon on map class");
            tmpCur = mySelf.Update(null,false);
            tmpFeature = tmpCur.NextFeature();
            fw.writeLine("going to enter loop to look for feature");
            while(tmpFeature != null)
            {
               if (tmpFeature.OID == inFeature.OID)
               {
                  fw.writeLine("found the match so delete the feature");
                  tmpCur.DeleteFeature();
                  break;
               }
               tmpFeature = tmpCur.NextFeature();

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
      
      #endregion
   }
}
