using System;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;





using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.DataManagementTools;
using System.Runtime.InteropServices;
using DesignByContract;
namespace PAZ_Dispersal
{
    /// <summary>
    /// Summary description for MapManipulator.
    /// </summary>
    public class MapManipulator
    {

		#region Constructors (1) 

        public MapManipulator()
        {
           // myDoc = new MxDocumentClass();
           // myMap = myDoc.FocusMap;

            wsName = new WorkspaceNameClass();
            wsDissolveName = new WorkspaceNameClass();
            //ibg = new BasicGeoprocessorClass();
            queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = "CurrTime_1 = 1";
            tolerence = 0.0;
            array_1 = new ESRI.ArcGIS.esriSystem.Array();
            this.buildLogger();
            
            name = 0;
            HomeRangeIndex = 0;

        }

		#endregion Constructors 


        #region privateMemberVariables
        private IArray array_1;



        private IFeatureClass occupiedFeatureClass;
        private IFeatureClass unionFeatureClass;
        private IFeatureClass homeRangeFeatureClass;
        private IFeatureClass dissolveFeatureClass;

        private IFeatureBuffer insertBuff;

        private IFeatureClassName outShapeFileName;
        private IFeatureCursor featCursor;
        private IFields fields;
        private IQueryFilter queryFilter;


        private ITable table_1;
        private ITable table_2;
        private ITable dissolveTable;
        private IWorkspaceName wsName;
        private IWorkspaceName wsDissolveName;
        private IDatasetName dsName;
        private IDatasetName dsDissolveName;
        private Map mCurrStepMap;

        private Map mSocialMap;
        private Map myHomeRangeMap;
        private FileWriter.FileWriter fw;
        private IGeometryDef geoDef;
        private string mPath;
        private string mDissolvePath;
        private int name;
        private int HomeRangeIndex;
        private double tolerence;
        #endregion

        #region publicMethods


//        public bool addTimeStep(IPolygon step1, IPolygon step2, string sex)
//        {
//            bool success = true;
//            try
//            {
//                resetTimeStep();
//                addStepPolygon(step1);
//                addStepPolygon(step2);
//            }
//            catch (System.Exception ex)
//            {
//#if (DEBUG)
//                System.Windows.Forms.MessageBox.Show(ex.Message);
//#endif
//                FileWriter.FileWriter.WriteErrorFile(ex);
//                success = false;
//            }
//            return success;

        //}
        public bool buildTempMaps(string path, IGeometryDef inGeoDef)
        {
            bool success = false;
            this.mPath = path;
            this.geoDef = inGeoDef;

            buildWorkSpaceName();
            //getOutShapeFileName();
            this.myHomeRangeMap = new Map(path, "tempHome", inGeoDef, this.getHomeRangeFields(inGeoDef));
            this.buildFieldCollection(inGeoDef);
            success = true;
            return success;
        }
//        public bool copyOccupiedToAnimalMap(AnimalMap in_outMap)
//        {
//            bool success = false;
//            IFeature f;
//            IFeatureCursor searchCursor = null;
//            IFeatureCursor insertCursor = null;
//            IFeatureBuffer insertBuff = null;
//            try
//            {
//                fw.writeLine("inside copyOccupiedToAnimalMap");
//                fw.writeLine("     map name is " + in_outMap.fullFileName);
//                searchCursor = this.occupiedFeatureClass.Search(null, true);
//                insertCursor = in_outMap.mySelf.Insert(true);
//                insertBuff = in_outMap.mySelf.CreateFeatureBuffer();
//                //            this.searchCursor=this.occupiedFeatureClass.Search(null,true);
//                //            this.insertCursor = in_outMap.mySelf.Insert(true);
//                //            this.insertBuff = in_outMap.mySelf.CreateFeatureBuffer();
//                f = searchCursor.NextFeature();
//                while (f != null)
//                {
//                    insertBuff.Shape = f.Shape;
//                    this.AddFields(insertBuff, f);
//                    insertCursor.InsertFeature(insertBuff);
//                    f = searchCursor.NextFeature();
//                }
//                insertCursor.Flush();
//                this.removeTempMaps("Occupied");
//                success = true;
//            }
//            catch (System.Exception ex)
//            {
//                FileWriter.FileWriter.WriteErrorFile(ex);
//#if (DEBUG)
//                System.Windows.Forms.MessageBox.Show(ex.Message);
//#endif
//            }
//            finally
//            {
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(insertBuff);
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(insertCursor);
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(searchCursor);
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(occupiedFeatureClass);
//            }
//            return success;

//        }
//        public bool copyUnionToAnimalMap(AnimalMap in_outMap)
//        {
//            bool success = false;
//            IFeature feat = null;
//            IFeatureCursor searchCursor = null;
//            IFeatureCursor insertCursor = null;
//            IFeatureBuffer insertBuff = null;
//            //HACK JAN 19 TEST
//            try
//            {
//                in_outMap.removeAllPolygons();
//                searchCursor = this.unionFeatureClass.Search(null, true);
//                insertCursor = in_outMap.mySelf.Insert(true);
//                insertBuff = in_outMap.mySelf.CreateFeatureBuffer();
//                feat = searchCursor.NextFeature();
//                while (feat != null)
//                {
//                    insertBuff.Shape = feat.Shape;
//                    this.AddFields(insertBuff, feat);
//                    insertCursor.InsertFeature(insertBuff);
//                    feat = searchCursor.NextFeature();

//                }
//                insertCursor.Flush();
//                this.removeTempMaps("Union");

//                success = true;
//            }
//            catch (System.Exception ex)
//            {
//                FileWriter.FileWriter.WriteErrorFile(ex);
//#if (DEBUG)
//                System.Windows.Forms.MessageBox.Show(ex.Message);
//#endif
//            }
//            finally
//            {
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(insertBuff);
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(insertCursor);
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(searchCursor);
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(unionFeatureClass);
//            }
//            return success;

//        }
        public IFeatureClass clipMaps(IFeatureClass oldMap, IFeatureClass newMap, ref int SocialIndex)
        {
            IEnvelope firstE = null;
            IEnvelope secondE = null;
            IEnvelope clipE = null;
            IFeatureClass outShapeFile = null;
            IFeatureCursor select = null;
            IFeatureCursor insert = null;
            IFeatureBuffer buffy = null;
            IFeature feat = null;
            ITopologicalOperator toppy = null;
            IField myField = null;
            string name;
            int newIndex;
            try
            {
                try
                {
                    firstE = this.getEnvelope(oldMap);
                    secondE = this.getEnvelope(newMap);
                    clipE = this.CreateClipperEnvelope(firstE, secondE);
                    outShapeFile = this.buildNewFeatureClass(newMap, ref SocialIndex);
                    select = newMap.Search(this.GetSpatialFilter(clipE, newMap), true);
                    insert = outShapeFile.Insert(true);
                    buffy = outShapeFile.CreateFeatureBuffer();
                    feat = select.NextFeature();
                    while (feat != null)
                    {
                        toppy = feat.ShapeCopy as ITopologicalOperator;
                        toppy.Clip(clipE);
                        buffy.Shape = toppy as IGeometry;
                        for (int i = 0; i < feat.Fields.FieldCount; i++)
                        {
                            myField = feat.Fields.get_Field(i);
                            if ((myField.Type != esriFieldType.esriFieldTypeGeometry) &&
                               (myField.Type != esriFieldType.esriFieldTypeOID))
                            {
                                name = myField.Name;
                                newIndex = buffy.Fields.FindField(name);
                                buffy.set_Value(newIndex, feat.get_Value(i));
                            }
                        }
                        insert.InsertFeature(buffy);
                        feat = select.NextFeature();
                    }
                    insert.Flush();

                }
                catch (System.Exception ex)
                {
                    FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
                    System.Windows.Forms.MessageBox.Show(ex.Message);

#endif
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(firstE);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(secondE);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(clipE);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(select);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(insert);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(buffy);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(toppy);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(myField);
                }
            }
            catch (System.Exception ex)
            {
                FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);

#endif
            }
            return outShapeFile;

        }
      //  public void dissolveSteps2()
      //  {
      //      IFeatureClassName myName = new FeatureClassNameClass();
      //      IFeatureClass feat = null ;
            
          
      //      ITable dissolveTable = this.mCurrStepMap.mySelf as ITable;
      //      IDataset dataset = this.mCurrStepMap.mySelf as IDataset;
      //      Geoprocessor geoprocessor = new Geoprocessor();
      //      Dissolve d = new Dissolve();
      //      d.in_features = dissolveTable;
      //      d.out_feature_class = feat;
      //      d.dissolve_field="CurrTime";
      //      geoprocessor.Execute(d, null);

      //  }
      //  public void   dissolveSteps()
      //{
      //   IBasicGeoprocessor ibg = new BasicGeoprocessorClass();
      //   IFeatureLayer featureLayer_1 = new FeatureLayerClass();
      //   ILayer layer_1 = null;
      //   ITable table_1 = null;
      //   try
      //   {
      //      this.buildDissovlePath();
      //      this.buildDissolveWorkSpaceName();
      //      this.getOutShapeFileName();
      //      this.buildDissolveDataSetName();

      //      fw.writeLine("inside dissolve steps getting the feature layer");
      //      featureLayer_1.FeatureClass = this.mCurrStepMap.mySelf; //firstInFeatureClass;
      //      featureLayer_1.Name = this.mCurrStepMap.mySelf.AliasName; //firstInFeatureClass.AliasName;
      //      fw.writeLine("now clearing the old layers from my map");
      //      myMap.ClearLayers();
      //      fw.writeLine("now adding the layers to the map");
      //      myMap.AddLayer(featureLayer_1);
      //      layer_1 = myMap.get_Layer(0);
      //      featureLayer_1 = (IFeatureLayer)layer_1;
      //      fw.writeLine("now getting the table from the layer");
      //      table_1 = (ITable)featureLayer_1;
      //      fw.writeLine("table 1 has " + table_1.RowCount(null).ToString() + " rows");

      //      if (table_1.FindField("CurrTime") > 0)
      //      {
      //         fw.writeLine("found the currtime field so do the dissolve");
      //         dissolveTable = ibg.Dissolve(table_1, false, "CurrTime", "Dissolve.CurrTime,Minimum.CurrTime", this.dsDissolveName);
      //         fw.writeLine("done with the dissolve");
      //         fw.writeLine("dissolve table has " + dissolveTable.RowCount(null).ToString() + " rows");
      //      }
      //      else
      //      {
      //          System.Windows.Forms.MessageBox.Show("no curr time field to dissolve on");
      //      }
      //   }
              
               
      //   catch (System.Exception ex)        
      //   {          FileWriter.FileWriter.WriteErrorFile(ex);  
      //          System.Windows.Forms.MessageBox.Show(ex.Message);


      //      }
      //      finally
      //      {
      //          System.Runtime.InteropServices.Marshal.ReleaseComObject(ibg);
      //          System.Runtime.InteropServices.Marshal.ReleaseComObject(featureLayer_1);
      //          System.Runtime.InteropServices.Marshal.ReleaseComObject(table_1);
      //      }
      //  }
        public void dissolveNewSocial(Map newSocialMap)
        {
            string DissolveMapName;
            ITable T = null;
            try
            {
                T = this.getTable(newSocialMap.mySelf);
                DissolveMapName = this.dissolveFemaleNewSocial(T);
                this.explodeDisolvedMap(DissolveMapName);
            }
            catch (System.Exception ex)
            {
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(T);
            }
        }
        public void dissolveOccupied()
        {
            IBasicGeoprocessor ibg = new BasicGeoprocessorClass();
            IFeatureLayer featureLayer_1 = new FeatureLayerClass();
            try
            {

                this.buildOccupiedDissolveDataSetName();
                fw.writeLine("inside dissolve occupied getting the feature layer");
                featureLayer_1.FeatureClass = this.unionFeatureClass; //firstInFeatureClass;
                featureLayer_1.Name = this.unionFeatureClass.AliasName; //firstInFeatureClass.AliasName;

                fw.writeLine("now clearing the old layers from my map");
                myMap.ClearLayers();
                fw.writeLine("now adding the layers to the map");
                myMap.AddLayer(featureLayer_1);
                layer_1 = myMap.get_Layer(0);
                featureLayer_1 = (IFeatureLayer)layer_1;
                fw.writeLine("now getting the table from the layer");
                table_1 = (ITable)featureLayer_1;
                fw.writeLine("table 1 has " + table_1.RowCount(null).ToString() + " rows");

                if (table_1.FindField("Available") > 0)
                {
                    fw.writeLine("found the Available field so do the dissolve");
                    dissolveTable = ibg.Dissolve(table_1, false, "Available", "Dissolve.Shape,Minimum.Available", this.dsDissolveName);
                    
                    fw.writeLine("done with the dissolve");
                    fw.writeLine("dissolve table has " + dissolveTable.RowCount(null).ToString() + " rows");
                }
                else
                {
                    fw.writeLine("did not find the currtime field");
#if(DEBUG)
                    System.Windows.Forms.MessageBox.Show("can not find the dissolve field");
#endif
                }
                fw.writeLine("leaving dissolveOccupied");
                fw.writeLine("");
            }
            catch (System.Exception ex)
            {
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);

#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ibg);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(featureLayer_1);

            }
        }

        //public void doOccupiedStuff(string sex)
        //{
        //    fw.writeLine("inside doOccupiedStuff");
        //    outShapeFileName = new FeatureClassNameClass();
        //    IFeatureClass tempFC = null;
           
        //    this.addOccupideTerritory();
        //    // this.removeTempMaps("dissolve");
        //    this.makeOccupiedAvailable(sex);
        //}
        public void editNewSocialMap(string fileName)
        {
            IQueryFilter qf = null;
            IFeatureCursor fc = null;
            IFeatureClass ifc = null;
            IFeature f = null;
            string fieldName;
            string sexField;
            int suitableIndex;
            int newSexIndex;
            int oldSexIndex;


            //HACK was an integer
            object idNum;
            try
            {
                qf = new QueryFilterClass();
                ifc = Map.openFeatureClass(this.mPath, fileName);
                fw.writeLine("");
                fw.writeLine("inside editNewSocialMap(string fileName) for map " + fileName);
                fw.writeLine("the path for the file is " + this.mPath);
                //first loop through and move the male values over      
                fieldName = "OCCUP_MA_1";
                sexField = "OCCUP_MALE";

                if (ifc.Fields.FindField(fieldName) >= 0)
                {
                    qf.WhereClause = fieldName + " <> 'none'";
                    fc = ifc.Update(qf, true);
                    f = fc.NextFeature();
                    if (f != null)
                    {
                        suitableIndex = f.Fields.FindField("SUITABILIT");
                        newSexIndex = f.Fields.FindField(sexField);
                        oldSexIndex = f.Fields.FindField(fieldName);

                        while (f != null)
                        {
                            //only add the animal's id number if the new area is suitable.
                            if (f.get_Value(suitableIndex).ToString() == "Suitable")
                            {

                                //idNum = System.Convert.ToInt32(f.get_Value(oldSexIndex));
                                idNum = f.get_Value(oldSexIndex);
                                fw.writeLine("adding male number " + idNum.ToString());
                                f.set_Value(newSexIndex, idNum);
                                f.Store();
                            }
                            f = fc.NextFeature();
                        }
                    }
                }

                //now do the same for the females
                fieldName = "OCCUP_FE_1";
                sexField = "OCCUP_FEMA";
                if (ifc.Fields.FindField(fieldName) >= 0)
                {
                    qf.WhereClause = fieldName + " <> 'none'";
                    fc = ifc.Update(qf, true);
                    f = fc.NextFeature();
                    if (f != null)
                    {
                        suitableIndex = f.Fields.FindField("SUITABILIT");
                        newSexIndex = f.Fields.FindField(sexField);
                        oldSexIndex = f.Fields.FindField(fieldName);
                        while (f != null)
                        {
                            if (f.get_Value(suitableIndex).ToString() == "Suitable")
                            {

                                idNum = f.get_Value(oldSexIndex);
                                fw.writeLine("adding female number " + idNum.ToString());
                                f.set_Value(newSexIndex, idNum);
                                f.Store();
                            }
                            f = fc.NextFeature();
                        }
                    }
                    fw.writeLine("");
                    this.removeSocialMapExtraFields(fileName);
                }
            }
            catch (System.Exception ex)
            {
                FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif

            }
            finally
            {
                if (fc != null)
                {
                    fc.Flush();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(fc);
                }
            }
        }

//        public void intersectFeatures(ref IFeatureClass inFC, ITable inTable, string name)
//        {
//            IBasicGeoprocessor ibg = new BasicGeoprocessorClass();
//            try
//            {
//                table_1 = this.getTable(this.mSocialMap.mySelf);
//                table_2 = inTable;

//                //this works when uncommented
//                 IFeatureClassName outShapeFileName = new FeatureClassNameClass();
//                outShapeFileName.FeatureType = mSocialMap.mySelf.FeatureType;
//                outShapeFileName.ShapeType = mSocialMap.mySelf.ShapeType;
//                outShapeFileName.ShapeFieldName = mSocialMap.mySelf.ShapeFieldName;
//                IWorkspaceName wsName = new WorkspaceNameClass();
                
//                wsName.WorkspaceFactoryProgID = "esriDataSourcesFile.ShapeFileWorkspaceFactory.1";
//               wsName.PathName = mPath;

//                dsName = (IDatasetName)outShapeFileName;
//                dsName.Name = name;
//                dsName.WorkspaceName = wsName;
//                Check.Require(table_1 != null, "table 1 was null");
//                Check.Require(table_2 != null, "table 2 was null");
//                Check.Require(outShapeFileName != null, "outshape file name was null");
//                inFC = ibg.Intersect(table_1, false, table_2, false, tolerence, outShapeFileName);


//            }
//            catch (COMException comEX)
//            {
//                System.Windows.Forms.MessageBox.Show(comEX.Message);
//            }
//            catch (System.Exception ex)
//            {
//#if (DEBUG)
//                System.Windows.Forms.MessageBox.Show(ex.Message);
//#endif
//                FileWriter.FileWriter.WriteErrorFile(ex);
//            }
//            finally
//            {
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(ibg);
//            }


//        }
        public void makeMapCopies(string orgPath, string orgFileName, string newPath, string newFileName)
        {
            string[] fileNames;
            string extension;
            try
            {
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
        public bool makeHomeRange(Animal inA)
        {
            bool success;


            string sex = inA.Sex;
            string id = inA.IdNum.ToString();
            try
            {
                //ths makes the polygon
                success = buildHomeRange(inA);
                if (success)
                {
                    //if we could make a polygon large enough now make the new social map
                    //file name is "HomeRange"
                    addHomeRangeToSocialMap();
                    //move the values from the union
                    this.editNewSocialMap(id, sex);
                    //new social map (HomeRange.shp) has extra fields so remove them.
                    this.removeSocialMapExtraFields("HomeRange");

                }
            }
            catch (System.Exception ex)
            {
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
                success = false;
            }
            return success;
        }
        //public void makeSelectFeatureClass(ITable inTable)
        //{
        //    IBasicGeoprocessor ibg = new BasicGeoprocessorClass();
        //   try
        //   {
        //      if (inTable != null)
        //      {
        //         table_1 = (ITable)this.mCurrStepMap.getTable();
        //         fw.writeLine("inside make select feature class");
        //         this.buildWorkSpaceName();
        //         this.getOutShapeFileName();
        //         this.buildDataSetName("SelectFeature");
        //         fw.writeLine("made all the stuff and named the map step");
        //         int j = table_1.Fields.FindField("CurrTime");
        //         IRow row;
        //         for (int i = 0; i < table_1.RowCount(null); i++)
        //         {
        //            row = table_1.GetRow(i);
        //            fw.writeLine("currtime is = " + row.get_Value(j).ToString());
        //         }
        //         fw.writeLine("now call the intersect method");
        //         selectFeatureClass = ibg.Intersect(table_1, false, inTable, false, tolerence, outShapeFileName);
        //         fw.writeLine("made it passed the intersect method");

        //         fw.writeLine("now calling this.selectFromMap()");
        //         this.selectFromMap();

        //      }
        //   }
        //   catch (System.Exception ex)
        //   {
        //      FileWriter.FileWriter.WriteErrorFile(ex);
        //   }
        //   finally
        //   {
        //       System.Runtime.InteropServices.Marshal.ReleaseComObject(ibg);
        //   }
        //}
        public void makeSinglePart()
        {
            //         IFeatureCursor searchCurr;
            //         IFeatureCursor insertCurr;
            //         IFeatureBuffer insertBuff;
            //         IFeature f;
            //         IGeometryCollection geoColl;
            //
            //         IFeatureClass ifc = this.getDissolveOccupiedShapeFile();
            //
            //         searchCurr = ifc.Search(null,false);
            //         insertCurr = ifc.Insert(true);
            //         insertBuff = ifc.CreateFeatureBuffer();
            //         f = searchCurr.NextFeature();
            //         while(f!=null)
            //         {
            //            geoColl = (IGeometryCollection)f.Shape;
            //            for(int i=0;i<geoColl.GeometryCount; i++)
            //            {
            //               this.InsertFeature(insertCurr,insertBuff,f,geoColl.get_Geometry(i));
            //            }
            //            f.Delete();
            //            f = searchCurr.NextFeature();
            //         }
            //         insertCurr.Flush();
        }
        public void unionSocialMaps(Map newMap)
        {
            IBasicGeoprocessor ibg = new BasicGeoprocessorClass();
            try
            {
                table_1 = this.getTable(newMap.mySelf);
                table_2 = this.getTable(mSocialMap.mySelf);
                this.buildDataSetName("Social");
                ibg.Union(table_1, false, table_2, false, this.tolerence, outShapeFileName);
                fw.writeLine("leaving unionHomeRangeMap");
                fw.writeLine("");
            }
            catch (System.Exception ex)
            {
#if DEBUG
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ibg);
            }

        }
//        public void unionTimeStep(AnimalMap in_outMap, string AnimalID, string timeStep)
//        {
//            IBasicGeoprocessor ibg = new BasicGeoprocessorClass();
//            ITable table_1 = null;
//            ITable table_2 = null;
//            try
//            {
//                this.unionFeatureClass = null;
//                this.makeMapCopies(in_outMap.Path, in_outMap.mySelf.AliasName, in_outMap.Path, "lastMap");
//                fw.writeLine("inside unionTimeStep");
//                fw.writeLine("Animal number = " + AnimalID);
//                fw.writeLine("time step = " + timeStep);
//                string fileName = in_outMap.mySelf.AliasName;
//                table_1 = this.getTable(in_outMap.mySelf);
//                table_2 = this.getOccupiedTable();
//                this.buildDataSetName("Union" + name.ToString());
//                fw.writeLine("calling the union method");
//                fw.writeLine("name is " + name.ToString());


//                Check.Require(table_1 != null, "table 1 was null");
//                Check.Require(table_2 != null, "table 2 was null");
//                Check.Require(outShapeFileName != null, "outshape file name was null");
//                this.unionFeatureClass = ibg.Union(table_1, false, table_2, false, this.tolerence, outShapeFileName);

//                this.removeTempMaps("Occupied");
//                fw.writeLine("done with the union now calling the edit union");
//                editUnionFeatureClass();
//                fw.writeLine("back from editUninon now leaving unionTimeStep");
//                fw.writeLine("");
//            }
//            catch (System.Exception ex)
//            {
//#if DEBUG
//                System.Windows.Forms.MessageBox.Show(ex.Message);
//#endif
//                fw.writeLine("");
//                fw.writeLine("Animal number = " + AnimalID);
//                fw.writeLine("time step = " + timeStep);
//                FileWriter.FileWriter.WriteErrorFile(ex);
//                FileWriter.FileWriter.AddToErrorFile("Animal number = " + AnimalID);
//                FileWriter.FileWriter.AddToErrorFile("time step = " + timeStep);
//            }
//            finally
//            {
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(ibg);
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(table_1);
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(table_2);
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(occupiedFeatureClass);
//            }

//        }
//        public ITable unionTimeStepGetTable()
//        {
//            IBasicGeoprocessor ibg = new BasicGeoprocessorClass();
//            try
//            {
//                table_1 = this.getDissolveOccupiedTable();
//                table_2 = this.getOccupiedTable();
//                this.buildDataSetName("Union" + name.ToString());
//                this.unionFeatureClass = ibg.Union(table_1, false, table_2, false, this.tolerence, outShapeFileName);
//                //this.unionFeatureClass=this.ibg.Union(table_1,true,table_2,false,this.tolerence,this.outShapeFileName);
//                this.removeTempMaps("Occupied");
//                editUnionFeatureClass();
//                fw.writeLine("leaving unionTimeStep");
//                fw.writeLine("");
//            }
//            catch (System.Exception ex)
//            {
//#if DEBUG
//                System.Windows.Forms.MessageBox.Show(ex.Message);
//#endif
//                FileWriter.FileWriter.WriteErrorFile(ex);
//            }
//            finally
//            {
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(ibg);
//            }
//            return this.getOccupiedTable();

//        }

        #endregion

        #region privateMethods
        private void addOccupideTerritory()
        {
            ITable table_1 = null;
            ITable table_2 = null;
            IFeatureClass d = null;
            IDatasetName dsName = null;
            IBasicGeoprocessor ibg = new BasicGeoprocessorClass();
            try
            {


                fw.writeLine("inside add occupied territory");
                table_1 = this.getTable(this.mSocialMap.mySelf);
                fw.writeLine("create dissolve feature class");
                d = this.buildNewDissolveFeatureClass();
                table_2 = this.getTable(d);
                fw.writeLine("after getting table");
                IWorkspaceName wsName = new WorkspaceNameClass();
                wsName.PathName = mPath;
                wsName.WorkspaceFactoryProgID = "esriCore.ShapeFileWorkspaceFactory.1";
                IFeatureClassName outShapeFileName = new FeatureClassNameClass();
                outShapeFileName.FeatureType = mSocialMap.mySelf.FeatureType;
                outShapeFileName.ShapeType = mSocialMap.mySelf.ShapeType;
                outShapeFileName.ShapeFieldName = mSocialMap.mySelf.ShapeFieldName;

                dsName = (IDatasetName)outShapeFileName;
                dsName.Name = "Occupied" + name.ToString();
                fw.writeLine("the occupied file name will be " + dsName.Name);
                name++;
                dsName.WorkspaceName = wsName;
                occupiedFeatureClass = ibg.Intersect(table_1, false, table_2, false, tolerence, outShapeFileName);
                fw.writeLine("leaving addOccupideTerritory");
                fw.writeLine("");
            }
            catch (System.Exception ex)
            {
                FileWriter.FileWriter.AddToErrorFile("the occupied file name was " + dsName.Name);
                FileWriter.FileWriter.WriteErrorFile(ex);

#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif

            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(table_1);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(table_2);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(d);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(dsName);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ibg);

            }



        }
        private void addHomeRangeToSocialMap()
        {

            try
            {
                this.unionHomeRange();
            }
            catch (System.Exception ex)
            {
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
            }

        }
        private void addNewPoly(Map inMap, IPolygon inPoly, string AnimalID, string inAnimalSex)
        {
            IFeature feature = null;
            IFields fields = null;
            int index = 0;


            //now add the one we want
            try
            {
                feature = inMap.mySelf.CreateFeature();
                fields = feature.Fields;

                if (inAnimalSex.ToUpper() == "MALE")
                {
                    index = fields.FindField("OCCUP_MALE");
                }
                else
                {
                    index = fields.FindField("OCCUP_FEMA");
                }

                if (index >= 0)
                    feature.set_Value(index, AnimalID);

                feature.Shape = inPoly;
                feature.Store();
            }
            catch (System.Exception ex)
            {
                FileWriter.FileWriter.WriteErrorFile(ex);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(feature);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(fields);
            }


        }
        private void addStepPolygon(IPolygon inPoly)
        {
            IFeature feature = null;
            IFields fields = null;
            int index = 0;


            //now add the one we want
            try
            {
                fw.writeLine("inside addStepPolygon");
                feature = this.mCurrStepMap.mySelf.CreateFeature();
                fw.writeLine("getting fields from newly created feature");
                fields = feature.Fields;
                index = fields.FindField("CurrTime");
                fw.writeLine("looking for currtime field index is " + index.ToString());
                if (index >= 0)
                {
                    fw.writeLine("setting the value of currtime to 1");
                    feature.set_Value(index, 1);
                }
                feature.Shape = inPoly;
                feature.Store();
                fw.writeLine("leaving addStepPolygon");
                fw.writeLine("");
            }
            catch (System.Exception ex)
            {
                FileWriter.FileWriter.WriteErrorFile(ex);
            }

            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(feature);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(fields);
            }

        }
        //private void addStepPolygon(AnimalMap inMap, IPolygon inPoly)
        //{
        //    IFeature feature = null;
        //    IFields fields;
        //    int index = 0;


        //    //now add the one we want
        //    try
        //    {
        //        feature = inMap.mySelf.CreateFeature();
        //        fields = feature.Fields;
        //        index = fields.FindField("CurrTime");
        //        if (index >= 0)
        //            feature.set_Value(index, 1);
        //        feature.Shape = inPoly;
        //        feature.Store();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        FileWriter.FileWriter.WriteErrorFile(ex);
        //    }
        //}
        private void AddFields(IFeatureBuffer featureBuffer, IFeature feature)
        {
            // Copy the attributes of the orig feature the new feature
            IRowBuffer rowBuffer = (IRowBuffer)featureBuffer;
            IFields fieldsNew = rowBuffer.Fields;

            IFields fields = feature.Fields;
            for (int i = 0; i <= fields.FieldCount - 1; i++)
            {
                IField field = fields.get_Field(i);
                if ((field.Type != esriFieldType.esriFieldTypeGeometry) &&
                   (field.Type != esriFieldType.esriFieldTypeOID))
                {
                    int intFieldIndex = fieldsNew.FindField(field.Name);
                    if (intFieldIndex != -1)
                    {
                        featureBuffer.set_Value(intFieldIndex, feature.get_Value(i));
                    }
                }
            }
        }
        //private void addNewPoly(Map inMap, IPolygon inPoly)
        //{
        //    IFeature feature = null;
        //    IFields fields;
        //    int index = 0;


        //    //now add the one we want
        //    try
        //    {
        //        feature = inMap.mySelf.CreateFeature();
        //        fields = feature.Fields;
        //        index = fields.FindField("CurrTime");
        //        if (index >= 0)
        //            feature.set_Value(index, 1);
        //        feature.Shape = inPoly;
        //        feature.Store();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        FileWriter.FileWriter.WriteErrorFile(ex);
        //    }


        //}
        private PolygonClass buildBoundary(Animal inAnimal, int numberOfPoints, double stretchFactor)
        {
            IPointCollection boundaryPoints = new PolygonClass();
            IPointCollection myPoints = new MultipointClass();
            double[] anglesList = new double[numberOfPoints];
            RandomNumbers rn = RandomNumbers.getInstance();
            IGeometry geom = null;
            double angle = 0;
            double radius = 0;
            IPoint tempPoint;
            object missing = Type.Missing;


            geom = (IGeometry)boundaryPoints;
            geom.SpatialReference = inAnimal.HomeRangeCenter.SpatialReference;


            for (int i = 0; i < numberOfPoints; i++)
            {
                anglesList[i] = (rn.getUniformRandomNum() * Math.PI * 2);
            }
            System.Array.Sort(anglesList);
            //go backwards to get clockwise polygon for external ring
            for (int i = numberOfPoints - 1; i >= 0; i--)
            {
                tempPoint = new PointClass();
                angle = anglesList[i];
                //radius is slightly larger than needed for home range to compensate for not being a circle
                radius = Math.Sqrt(1000000 * inAnimal.HomeRangeArea / Math.PI) * rn.getPositiveNormalRandomNum(1.2, .1) * stretchFactor;
                tempPoint.X = inAnimal.HomeRangeCenter.X + radius * Math.Cos(angle);
                tempPoint.Y = inAnimal.HomeRangeCenter.Y + radius * Math.Sin(angle);
                boundaryPoints.AddPoint(tempPoint, ref missing, ref missing);
            }

            return boundaryPoints as PolygonClass;
        }//end of buildBoundary
        private void buildDataSetName(string inName)
        {
            dsName = null;
            dsName = (IDatasetName)outShapeFileName;
            dsName.Name = inName;
            dsName.WorkspaceName = wsName;
        }
        private void buildDissolveDataSetName()
        {
            dsDissolveName = null;
            dsDissolveName = (IDatasetName)outShapeFileName;
            dsDissolveName.Name = "Dissolve";
            dsDissolveName.WorkspaceName = wsDissolveName;
            name++;
        }
        private bool buildHomeRange(Animal inA)
        {
            double stretch = 1.0;
            double tempArea = 0.0;
            bool success = true;
            PolygonClass homeRange = null;


            string fieldName; //depending on what sex it is we use a different field in many opperations so just do it here once
            string sex = inA.Sex;
            int loopCounter = 0;
            try
            {
                if ("MALE" == inA.Sex.ToUpper())
                {
                    fieldName = "OCCUP_MALE";
                }
                else
                {
                    fieldName = "OCCUP_FEMA";
                }
                fw.writeLine("inside build HomeRange for anmial number " + inA.IdNum.ToString());
                fw.writeLine("my Home Range Center is x = " + inA.Location.X.ToString() + " y= " + inA.Location.Y.ToString());
                fw.writeLine("calling buildBoundary");
                this.myHomeRangeMap.removeAllPolygons();
                clearOutHomeRangeFeatureClass();
                homeRange = this.buildBoundary(inA, 30, stretch);
                homeRange.Close();
                fw.writeLine("adding the polygon to the home range map");
                this.addNewPoly(myHomeRangeMap, homeRange, inA.IdNum.ToString(), inA.Sex);
                //fw.writeLine("now going to intersect the home range with the social map");
                this.intersectHomeRangeWithSocial();
                fw.writeLine("now call edit home range");
                this.editHomeRangeMap(inA.IdNum.ToString(), inA.Sex);
                //fw.writeLine("now dissolve the home range");
                //this.dissolveHomeRange("Available");
                //fw.writeLine("now explode the home range");
                //this.explodeHomeRange();
                fw.writeLine("now call get area");
                tempArea = this.getHomeRangeArea(fieldName, sex);
                fw.writeLine("area from first attempt is " + tempArea.ToString());
                fw.writeLine("the animal wants a home range of " + inA.HomeRangeArea.ToString());
                while (tempArea < inA.HomeRangeArea)
                {
                    fw.writeLine("must not have made it going to try again");
                    loopCounter++;
                    stretch += 0.1;
                    fw.writeLine("removing all the polygons from the home range map");
                    this.myHomeRangeMap.removeAllPolygons();
                    clearOutHomeRangeFeatureClass();
                    homeRange = this.buildBoundary(inA, 30, stretch);
                    homeRange.Close();
                    fw.writeLine("adding the polygon to the home range map");
                    this.addNewPoly(myHomeRangeMap, homeRange, inA.IdNum.ToString(), inA.Sex);
                    fw.writeLine("now going to intersect the home range with the social map");
                    this.intersectHomeRangeWithSocial();
                    fw.writeLine("now call edit home range");
                    this.editHomeRangeMap(inA.IdNum.ToString(), inA.Sex);
                    //               fw.writeLine("now dissolve the home range");
                    //               this.dissolveHomeRange("Available");
                    //fw.writeLine("now explode the home range");
                    //this.explodeHomeRange();
                    fw.writeLine("now call get area");
                    tempArea = this.getHomeRangeArea(fieldName, sex);
                    fw.writeLine("area from first attempt is " + tempArea.ToString());
                    fw.writeLine("the animal wants a home range of " + inA.HomeRangeArea.ToString());
                }
                fw.writeLine("stretch had to get to " + stretch.ToString());
                fw.writeLine("the loop counter is at " + loopCounter.ToString());
                success = true;

            }
            catch (System.Exception ex)
            {
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
                success = false;
            }
            return success;
        }
        private void buildOccupiedDissolveDataSetName()
        {
            dsDissolveName = null;
            dsDissolveName = (IDatasetName)outShapeFileName;
            dsDissolveName.Name = "DissolveOccupied";
            dsDissolveName.WorkspaceName = wsDissolveName;
        }
        private void buildDissovlePath()
        {
            Directory.CreateDirectory(mPath + @"\Dissovle");
            this.mDissolvePath = mPath + @"\Dissovle";
        }
        private void buildFieldCollection(IGeometryDef inGeoDef)
        {
            fields = new FieldsClass();
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
            IField field = new FieldClass();
            IFieldEdit fieldEdit = (IFieldEdit)field;

            fieldEdit = new FieldClass();
            fieldEdit.Name_2 = "OBJECTID";
            fieldEdit.AliasName_2 = "OBJECTID";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsEdit.AddField(fieldEdit);

            fieldEdit = new FieldClass();
            fieldEdit.Name_2 = "SHAPE";
            fieldEdit.IsNullable_2 = true;
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            fieldEdit.GeometryDef_2 = inGeoDef;
            fieldEdit.Required_2 = true;
            fieldsEdit.AddField(fieldEdit);
            fieldEdit = new FieldClass();
            fieldEdit.Name_2 = "ID";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
            fieldsEdit.AddField(fieldEdit);

            fieldEdit = new FieldClass();
            fieldEdit.Name_2 = "CurrTime";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
            fieldsEdit.AddField(fieldEdit);

            //System.Runtime.InteropServices.Marshal.ReleaseComObject(fields);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(field);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(fieldEdit);



        }
        private void buildLogger()
        {
            string s;
            StreamReader sr;
            bool foundPath = false;
            if (File.Exists("logFile.dat"))
            {
                sr = new StreamReader("logFile.dat");
                while (sr.Peek() > -1)
                {
                    s = sr.ReadLine();
                    if (s.IndexOf("MapManipulator") == 0)
                    {
                        fw = new FileWriter.FileWriter(s.Substring(s.IndexOf(" ")));
                        foundPath = true;
                        break;
                    }
                }

            }
            if (!foundPath)
            {
                fw = new FileWriter.EmptyFileWriter();
            }


        }
        private void buildWorkSpaceName()
        {
            wsName.PathName = this.mPath;
            wsName.WorkspaceFactoryProgID = "esriCore.ShapeFileWorkspaceFactory.1";
        }
        private void buildDissolveWorkSpaceName()
        {
            wsDissolveName.PathName = this.mDissolvePath;
            wsDissolveName.WorkspaceFactoryProgID = "esriCore.ShapeFileWorkspaceFactory.1";
        }
        private IFeatureClass buildNewFeatureClass(IFeatureClass inFeatureClass, ref int SocialIndex)
        {
            IFeatureClass outFeatureClass = null;
            IWorkspaceFactory myWrkSpaceFactory = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace myFeatWrkSpace;
            myFeatWrkSpace = (IFeatureWorkspace)myWrkSpaceFactory.OpenFromFile(this.mPath, 0);
            outFeatureClass = myFeatWrkSpace.CreateFeatureClass("NewSocial" + SocialIndex, inFeatureClass.Fields, inFeatureClass.CLSID, inFeatureClass.EXTCLSID, inFeatureClass.FeatureType, inFeatureClass.ShapeFieldName, "");
            SocialIndex++;

            return outFeatureClass;

        }
        private IFeatureClass buildNewDissolveFeatureClass()
        {
            IFeatureClass outFeatureClass = null;
            IWorkspaceFactory myWrkSpaceFactory = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace myFeatWrkSpace;
            myFeatWrkSpace = (IFeatureWorkspace)myWrkSpaceFactory.OpenFromFile(this.mDissolvePath, 0);
            outFeatureClass = myFeatWrkSpace.OpenFeatureClass(this.dsDissolveName.Name);

            return outFeatureClass;

        }
        private IEnvelope getEnvelope(IFeatureClass inFeatureClass)
        {
            IMap myMap = new MapClass();
            IFeatureLayer myFeatureLayer = new FeatureLayerClass();
            myFeatureLayer.FeatureClass = inFeatureClass;
            myFeatureLayer.Name = inFeatureClass.AliasName;
            this.myMap.AddLayer(myFeatureLayer);
            ILayer l = myMap.get_Layer(0);
            IEnvelope e = l.AreaOfInterest;
            return e;
        }
        private double getXMax(IEnvelope firstEnvelope, IEnvelope secondEnvlope)
        {
            fw.writeLine("inside getXMax");
            fw.writeLine("first map value is " + firstEnvelope.XMax.ToString());
            fw.writeLine("second map value is " + secondEnvlope.XMax.ToString());
            if (firstEnvelope.XMax < secondEnvlope.XMax)
                return firstEnvelope.XMax;
            else
                return secondEnvlope.XMax;
        }

        private double getXMin(IEnvelope firstEnvelope, IEnvelope secondEnvlope)
        {
            fw.writeLine("inside getXMin");
            fw.writeLine("first map value is " + firstEnvelope.XMin.ToString());
            fw.writeLine("second map value is " + secondEnvlope.XMin.ToString());
            if (firstEnvelope.XMin > secondEnvlope.XMin)
                return firstEnvelope.XMin;
            else
                return secondEnvlope.XMin;
        }

        private double getYMax(IEnvelope firstEnvelope, IEnvelope secondEnvlope)
        {
            fw.writeLine("inside getYMax");
            fw.writeLine("first map value is " + firstEnvelope.YMax.ToString());
            fw.writeLine("second map value is " + secondEnvlope.YMax.ToString());
            if (firstEnvelope.YMax < secondEnvlope.YMax)
                return firstEnvelope.YMax;
            else
                return secondEnvlope.YMax;
        }

        private double getYMin(IEnvelope firstEnvelope, IEnvelope secondEnvlope)
        {
            fw.writeLine("inside getYMin");
            fw.writeLine("first map value is " + firstEnvelope.YMin.ToString());
            fw.writeLine("second map value is " + secondEnvlope.YMin.ToString());
            if (firstEnvelope.YMin > secondEnvlope.YMin)
                return firstEnvelope.YMin;
            else
                return secondEnvlope.YMin;
        }
        private void getOutShapeFileName()
        {
            try
            {
                outShapeFileName = new FeatureClassNameClass();
                outShapeFileName.FeatureType = mSocialMap.mySelf.FeatureType;
                outShapeFileName.ShapeType = mSocialMap.mySelf.ShapeType;
                outShapeFileName.ShapeFieldName = mSocialMap.mySelf.ShapeFieldName;

            }
            catch (System.Exception ex)
            {
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
            }

        }
        private IFields getFieldCollection(IGeometryDef inGeoDef)
        {

            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
            IField field = new FieldClass();
            IFieldEdit fieldEdit = (IFieldEdit)field;
            try
            {
                fieldEdit = new FieldClass();
                fieldEdit.Name_2 = "OBJECTID";
                fieldEdit.AliasName_2 = "OBJECTID";
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
                fieldsEdit.AddField(fieldEdit);

                fieldEdit = new FieldClass();
                fieldEdit.Name_2 = "SHAPE";
                fieldEdit.IsNullable_2 = true;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
                fieldEdit.GeometryDef_2 = inGeoDef;
                fieldEdit.Required_2 = true;
                fieldsEdit.AddField(fieldEdit);
                fieldEdit = new FieldClass();
                fieldEdit.Name_2 = "ID";
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
                fieldsEdit.AddField(fieldEdit);

                fieldEdit = new FieldClass();
                fieldEdit.Name_2 = "CurrTime";
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
                fieldsEdit.AddField(fieldEdit);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(field);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(fieldEdit);
            }
            return fields;



        }

        private ISpatialFilter GetSpatialFilter(IEnvelope clipper, IFeatureClass newFeature)
        {
            ISpatialFilter myFilter = new SpatialFilterClass();
            IGeometry geom = clipper.Envelope as IGeometry;
            myFilter.Geometry = geom;
            myFilter.GeometryField = newFeature.ShapeFieldName;
            myFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            return myFilter;
        }

        private void clearOutHomeRangeFeatureClass()
        {
            string[] fileNames;
            fileNames = Directory.GetFiles(this.mPath, "tempArea*");
            for (int i = 0; i < fileNames.Length; i++)
            {
                File.Delete(fileNames[i]);
            }
        }
        private string dissolveFemaleNewSocial(ITable inT)
        {
            IBasicGeoprocessor ibg = new BasicGeoprocessorClass();
            try
            {
                ITable femaleDis = null;
                this.buildDataSetName("FemaleDissolve");
                femaleDis = ibg.Dissolve(inT, false, "OCCUP_FEMA", "Dissolve.Shape,Minimum.OCCUP_FEMA", this.dsName);
            }
            catch (System.Exception ex)
            {
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ibg);
            }
            return this.dsName.Name;

        }
//        private void dissolveHomeRange(string fieldName)
//        {
//            ITable tableInput = null;
//            ITable tableOutput = null;
//            IFeatureLayer fl = new FeatureLayerClass();
//            ILayer l;
//            int fieldIndex;
//            IBasicGeoprocessor ibg = new BasicGeoprocessorClass();

//            try
//            {
//                fw.writeLine("inside dissolveHomeRange and we are going to dissolve on " + fieldName);

//                this.getOutShapeFileName();
//                this.buildWorkSpaceName();
//                this.buildDataSetName("DissolveHome");
//                fw.writeLine("ok called the getOutShapeFileName buildWorkSpaceName buildDataSetName");
//                fw.writeLine("now clear out the map and do the layer table thing");
//                //get the table from a layer and not the feature class
//                this.myMap.ClearLayers();
//                fl.FeatureClass = this.homeRangeFeatureClass;
//                fl.Name = this.homeRangeFeatureClass.AliasName;
//                fw.writeLine("the home range we want to dissolve is " + homeRangeFeatureClass.AliasName);
//                myMap.AddLayer(fl);
//                l = myMap.get_Layer(0);
//                tableInput = this.homeRangeFeatureClass as ITable;
//                tableInput = (ITable)l;
//                fw.writeLine("check for the field name " + fieldName);
//                fieldIndex = tableInput.FindField(fieldName);
//                if (fieldIndex < 0)
//                    fw.writeLine("Could not find " + fieldName + " in the dissolve home range method of MapManipulator");
//                fw.writeLine("found " + fieldName + " at position number " + fieldIndex.ToString());
//                fw.writeLine("found it now do the dissovle");
//                tableOutput = ibg.Dissolve(tableInput, false, fieldName, "Dissolve.Shape,Minimum." + fieldName, this.dsName);

//            }
//            catch (System.Exception ex)
//            {
//                FileWriter.FileWriter.WriteErrorFile(ex);
//#if (DEBUG)
//                System.Windows.Forms.MessageBox.Show(ex.Message);
//#endif
//                System.Windows.Forms.Application.Exit();
//            }
//            finally
//            {
//                System.Runtime.InteropServices.Marshal.ReleaseComObject(ibg);
//            }
//        }
        private void editUnionFeatureClass()
        {
            int indexAvailable;
            int indexAvailable_1;
            int val;
            int loop = 0;
            IFeature feat = null;
            IFeatureCursor featCursor = null;
            IQueryFilter qf = new QueryFilterClass();

            try
            {
                fw.writeLine("Inside editUnionFeatureClass");


                featCursor = this.unionFeatureClass.Update(qf, true);
                indexAvailable = featCursor.FindField("Available");
                indexAvailable_1 = featCursor.FindField("Availabl_1");
                feat = featCursor.NextFeature();
                fw.writeLine("going to loop through the features now");
                while (feat != null)
                {
                    val = System.Convert.ToInt32(feat.get_Value(indexAvailable_1));
                    feat.set_Value(indexAvailable, val);
                    feat.Store();
                    feat = featCursor.NextFeature();
                    loop++;
                    if (loop % 100 == 0)
                        featCursor.Flush();
                }

                fw.writeLine("done with loop");
            }
            catch (System.Exception ex)
            {
                FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                System.Windows.Forms.Application.Exit();
            }
            finally
            {
                //System.Runtime.InteropServices.Marshal.ReleaseComObject(featCursor);

            }

        }
        private void editHomeRangeMap(string idNum, string sex)
        {
            string search;
            string suitVal;
            string sexVal;
            int index_1 = 0;
            int index_2 = 0;
            IFeature feat = null;
            IFeatureCursor featCursor = null;

            try
            {
                fw.writeLine("inside editHomeRangeMap id = " + idNum + " and sex is " + sex);
                fw.writeLine("the home range map is " + this.homeRangeFeatureClass.AliasName);
                //not sure how others will enter the data 
                sex = sex.ToUpper();
                if (sex == "MALE")
                    search = "OCCUP_MALE";
                else
                    search = "OCCUP_FEMA";

                fw.writeLine("ok going to search for a field named " + search);

                featCursor = this.homeRangeFeatureClass.Update(null, false);
                feat = featCursor.NextFeature();
                //can be null some times
                if (feat != null)
                {
                    fw.writeLine("looking for the fields now");
                    index_1 = feat.Fields.FindField("SUITABILIT");
                    index_2 = feat.Fields.FindField(search);
                    fw.writeLine("SUITABILIT was found at  " + index_1.ToString());
                    fw.writeLine(search + " was found at " + index_2.ToString());
                    //availableIndex = f.Fields.FindField("Available");
                }
                while (feat != null)
                {
                    suitVal = feat.get_Value(index_1).ToString().ToLower();
                    sexVal = feat.get_Value(index_2).ToString().ToLower();
                    fw.writeLine("suitable value for this polygon is " + suitVal);
                    fw.writeLine("sex value for this polygon is " + sexVal);
                    //if it is good then set the value otherwise blow it away
                    if (suitVal == "suitable" && sexVal == "none")
                    {
                        fw.writeLine("setting value");
                        feat.set_Value(index_2, idNum);
                        feat.Store();
                    }
                    else
                    {
                        fw.writeLine("blowing away the polygon");
                        featCursor.DeleteFeature();
                    }

                    feat = featCursor.NextFeature();
                    featCursor.Flush();
                }
                featCursor.Flush();
            }
            catch (System.Exception ex)
            {
                FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(featCursor);
            }

            fw.writeLine("leaving editHomeRangeMap");
        }
        private void editNewSocialMap(string idNum, string sex)
        {
            IQueryFilter qf = null;
            IFeatureCursor fc = null;
            IFeatureClass ifc = null;
            IFeature f = null;
            string fieldName;
            string sexField;
            int suitableIndex;
            int sexIndex;
            try
            {
                fw.writeLine("inside editNewSocialMap(string idNum, string sex) for animal num " + idNum);
                qf = new QueryFilterClass();
                ifc = Map.openFeatureClass(this.mPath, "HomeRange" + this.HomeRangeIndex.ToString());
                if (sex.Equals("MALE",StringComparison.CurrentCultureIgnoreCase))
                {
                    fieldName = "OCCUP_MA_1";
                    sexField = "OCCUP_MALE";
                }
                else
                {
                    fieldName = "OCCUP_FE_1";
                    sexField = "OCCUP_FEMA";
                }
                qf.WhereClause = fieldName + " = '" + idNum + "'";
                fc = ifc.Update(qf, true);
                f = fc.NextFeature();
                if (f != null)
                {
                    suitableIndex = f.Fields.FindField("SUITABILIT");
                    sexIndex = f.Fields.FindField(sexField);
                    while (f != null)
                    {
                        if (f.get_Value(suitableIndex).ToString() == "Suitable")
                        {
                            f.set_Value(sexIndex, idNum);
                            f.Store();
                        }
                        f = fc.NextFeature();
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
            finally
            {
                fc.Flush();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(fc);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(qf);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ifc);
            }

        }
//        private double getArea(IPolygon inPoly)
//        {
//            double area = 0;
//            IArea areaGetter;

//            try
//            {
//                fw.writeLine("inside get area");
//                areaGetter = (IArea)inPoly;

//                area = areaGetter.Area;
//                fw.writeLine("total area is " + area.ToString());
//                //area is in meters we are measuring in km so divide by 1000
//                area = areaGetter.Area / 1000000;
//                fw.writeLine("total area is " + area.ToString() + " kilometers");
//            }
//            catch (System.Exception ex)
//            {
//#if (DEBUG)
//                System.Windows.Forms.MessageBox.Show(ex.Message);
//#endif
//                FileWriter.FileWriter.WriteErrorFile(ex);
//                area = 0;
//            }
//            return area;
//        }
        private ITable getOccupiedTable()
        {
            IFeatureLayer featureLayer_2 = new FeatureLayerClass();
            ILayer layer = null;
            try
            {
                this.myMap.ClearLayers();
                featureLayer_2.FeatureClass = this.occupiedFeatureClass;
                featureLayer_2.Name = this.occupiedFeatureClass.AliasName;
                this.myMap.AddLayer(featureLayer_2);
                layer = myMap.get_Layer(0);
            }
            catch (Exception ex)
            {
                FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(featureLayer_2);
            }
            return (ITable)layer;

        }
        
        public ITable getTable(IFeatureClass inFeature)
        {
            IFeatureLayer featureLayer_2 = new FeatureLayerClass();
            ILayer layer = null;
            try
            {
                this.myMap.ClearLayers();
                featureLayer_2.FeatureClass = inFeature;
                featureLayer_2.Name = inFeature.AliasName;
                this.myMap.AddLayer(featureLayer_2);
                layer = myMap.get_Layer(0);
            }
            catch (Exception ex)
            {
                FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(featureLayer_2);
            }
            return (ITable)layer;

        }
//        private IFeatureClass getDissolveShapeFile()
//        {
//            IFeatureClass ifc = null;

//            try
//            {
//                IWorkspaceFactory wrkSpaceFactory = new ShapefileWorkspaceFactory();
//                IFeatureWorkspace featureWorkspace = null;
//                featureWorkspace = (IFeatureWorkspace)wrkSpaceFactory.OpenFromFile(this.mDissolvePath, 0);
//                ifc = featureWorkspace.OpenFeatureClass("Dissolve");
//            }


//            catch (System.Exception ex)
//            {
//#if(DEBUG)
//                System.Windows.Forms.MessageBox.Show(ex.Source + " "); //+ ex.InnerException.ToString());
//#endif
//            }

//            return ifc;

//        }
        private IFeatureClass getDissolveOccupiedShapeFile()
        {

            IFeatureClass ifc = null;

            try
            {
                IWorkspaceFactory wrkSpaceFactory = new ShapefileWorkspaceFactory();
                IFeatureWorkspace featureWorkspace = null;
                featureWorkspace = (IFeatureWorkspace)wrkSpaceFactory.OpenFromFile(this.mDissolvePath, 0);
                ifc = featureWorkspace.OpenFeatureClass("DissolveOccupied");
            }


            catch (System.Exception ex)
            {
#if DEBUG
                System.Windows.Forms.MessageBox.Show(ex.Source + " "); //+ ex.InnerException.ToString());
#endif
            }

            return ifc;
        }
        private ITable getDissolveOccupiedTable()
        {
            IFeatureClass ifc = null;
            IFeatureLayer featureLayer_1 = new FeatureLayerClass();

            try
            {
                ifc = this.getDissolveOccupiedShapeFile();
                if (ifc == null)
                    ifc = this.mCurrStepMap.mySelf;
                featureLayer_1.FeatureClass = ifc;
                featureLayer_1.Name = ifc.AliasName;
                this.myMap.ClearLayers();
                this.myMap.AddLayer(featureLayer_1);
                this.layer_1 = myMap.get_Layer(0);
            }
            catch (System.Exception ex)
            {
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(featureLayer_1);
            }
            return (ITable)layer_1;
        }
//        private IFields getFields(IGeometryDef inGeoDef)
//        {
//            fields = new FieldsClass();
//            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
//            IField field = new FieldClass();
//            IFieldEdit fieldEdit = (IFieldEdit)field;
//            try
//            {


//                fieldEdit = new FieldClass();
//                fieldEdit.Name_2 = "OBJECTID";
//                fieldEdit.AliasName_2 = "OBJECTID";
//                fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
//                fieldsEdit.AddField(fieldEdit);

//                fieldEdit = new FieldClass();
//                fieldEdit.Name_2 = "SHAPE";
//                fieldEdit.IsNullable_2 = true;
//                fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
//                fieldEdit.GeometryDef_2 = inGeoDef;
//                fieldEdit.Required_2 = true;
//                fieldsEdit.AddField(fieldEdit);

//                fieldEdit = new FieldClass();
//                fieldEdit.Name_2 = "ID";
//                fieldEdit.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
//                fieldsEdit.AddField(fieldEdit);

//                fieldEdit = new FieldClass();
//                fieldEdit.Name_2 = "CurrTime";
//                fieldEdit.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
//                fieldsEdit.AddField(fieldEdit);

//            }
//            catch (System.Exception ex)
//            {
//#if (DEBUG)
//                System.Windows.Forms.MessageBox.Show(ex.Message);
//#endif
//                FileWriter.FileWriter.WriteErrorFile(ex);
//            }
//            return fieldsEdit as IFields;
//        }
        private IFields getHomeRangeFields(IGeometryDef inGeoDef)
        {
            fields = new FieldsClass();
            IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
            IField field = new FieldClass();
            IFieldEdit fieldEdit = (IFieldEdit)field;
            try
            {


                fieldEdit = new FieldClass();
                fieldEdit.Name_2 = "OBJECTID";
                fieldEdit.AliasName_2 = "OBJECTID";
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
                fieldsEdit.AddField(fieldEdit);

                fieldEdit = new FieldClass();
                fieldEdit.Name_2 = "SHAPE";
                fieldEdit.IsNullable_2 = true;
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
                fieldEdit.GeometryDef_2 = inGeoDef;
                fieldEdit.Required_2 = true;
                fieldsEdit.AddField(fieldEdit);

                fieldEdit = new FieldClass();
                fieldEdit.Name_2 = "SUITABILIT";
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldEdit.Length_2 = 16;
                fieldsEdit.AddField(fieldEdit);

                fieldEdit = new FieldClass();
                fieldEdit.Name_2 = "OCCUP_MALE";
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldEdit.Length_2 = 16;
                fieldsEdit.AddField(fieldEdit);

                fieldEdit = new FieldClass();
                fieldEdit.Name_2 = "OCCUP_FEMA";
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldEdit.Length_2 = 16;
                fieldsEdit.AddField(fieldEdit);

            }
            catch (System.Exception ex)
            {
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
            }
            return fieldsEdit as IFields;
        }
        private double getHomeRangeArea(string fieldName, string id)
        {
            double area = 0;
            IFeatureCursor fc = null;
            IFeature f = null;
            IArea areaGetter = null;
            fw.writeLine("inside get home range area the field name is " + fieldName);
            try
            {
                //no need for the query filter as we have removed all unavailable areas already
                fc = this.homeRangeFeatureClass.Search(null, false);
                fw.writeLine("now starting loop");
                while ((f = fc.NextFeature()) != null)
                {
                    fw.writeLine("area was " + area.ToString());
                    areaGetter = (IArea)f.ShapeCopy;
                    area += areaGetter.Area;
                }
            }
            catch (System.Exception ex)
            {
#if DEBUG
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
            }
            fw.writeLine("now convert to square km from square meters " + area.ToString());
            area = area / 1000000;
            fw.writeLine("leaving getarea with a value of " + area.ToString());
            return area;
        }
        private void InsertFeature(IFeatureCursor inCurr,
           IFeatureBuffer inBuff,
           IFeature origF,
           IGeometry inGeo)
        {
            IField tmpField;
            IFields fieldColl;
            try
            {
                object Missing = Type.Missing;
                fieldColl = origF.Fields;
                for (int i = 0; i < fieldColl.FieldCount; i++)
                {
                    tmpField = fieldColl.get_Field(i);
                    if (tmpField.Type != esriFieldType.esriFieldTypeGeometry &&
                       tmpField.Type != esriFieldType.esriFieldTypeOID &&
                       tmpField.Type != esriFieldType.esriFieldTypeGlobalID &&
                       tmpField.Editable)
                    {
                        inBuff.set_Value(i, origF.get_Value(i));
                    }
                }
                inBuff.Shape = origF.Shape;
                inCurr.InsertFeature(inBuff);
            }
            catch (System.Exception ex)
            {
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
            }

        }
        //      private void addOccupideTerritory()
        //      {
        //        
        //         try
        //         {
        //            fw.writeLine("inside add occupied territory");
        //            table_1 = this.mSocialMap.getTable();
        //            fw.writeLine("table from social map has " + table_1.RowCount(null).ToString() + " rows");
        //            
        //            //table_2 = this.mCurrStepMap.getTable();
        //            //fw.writeLine("table from smCurrStepMap has " + table_2.RowCount(null).ToString() + " rows");
        //            //this works when uncommented
        //            IWorkspaceName wsName = new WorkspaceNameClass();
        //            wsName.PathName= mPath;
        //            wsName.WorkspaceFactoryProgID = "esriCore.ShapeFileWorkspaceFactory.1";
        //            IFeatureClassName outShapeFileName = new FeatureClassNameClass();
        //            outShapeFileName.FeatureType = mSocialMap.mySelf.FeatureType;
        //            outShapeFileName.ShapeType = mSocialMap.mySelf.ShapeType;
        //            outShapeFileName.ShapeFieldName = mSocialMap.mySelf.ShapeFieldName;
        //            
        //            dsName=(IDatasetName)outShapeFileName;
        //            dsName.Name = "Occupied"+name.ToString();
        //            name++;
        //            dsName.WorkspaceName = wsName;      
        //            occupiedFeatureClass = ibg.Intersect(table_1,false,dissolveTable,false,tolerence,outShapeFileName);
        //            //occupiedFeatureClass = ibg.Intersect(table_1,false,table_2,false,tolerence,outShapeFileName);
        //            fw.writeLine("leaving addOccupideTerritory");
        //            fw.writeLine("");
        //         }
        //         catch(System.Exception ex)
        //         {
        //#if (DEBUG)
        //            System.Windows.Forms.MessageBox.Show(ex.Message);
        //#endif
        //            FileWriter.FileWriter.WriteErrorFile(ex);
        //         }
        //
        //
        //      }
        private void intersectHomeRangeWithSocial()
        {
            IBasicGeoprocessor ibg = new BasicGeoprocessorClass();
            try
            {
                homeRangeFeatureClass = null;
                fw.writeLine("inside intersectHomeRangeWithSocial");
                table_1 = this.getTable(this.myHomeRangeMap.mySelf);
                table_2 = this.getTable(this.mSocialMap.mySelf);
                this.getOutShapeFileName();
                this.buildWorkSpaceName();
                this.buildDataSetName("tempArea" + name++);
                homeRangeFeatureClass = ibg.Intersect(table_2, false, table_1, false, this.tolerence, outShapeFileName);
                fw.writeLine("leaving intersectHomeRangeWithSocial");
            }
            catch (System.Exception ex)
            {
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ibg);
            }
        }
        private void explodeDisolvedMap(string fileName)
        {
            try
            {
                IFeatureClass ifc = Map.openFeatureClass(this.mPath, fileName);
                // Copy each feature from the original feature class to the new feature class
                IFeatureCursor loopCursor = ifc.Search(null, false);
                IFeatureCursor insertCurr = ifc.Insert(false);
                IFeatureBuffer insertBuff = ifc.CreateFeatureBuffer();
                IFeature feature;
                IGeometryCollection geometryColl;

                while ((feature = loopCursor.NextFeature()) != null)
                {
                    geometryColl = feature.Shape as IGeometryCollection;
                    IPolygon4 poly4 = feature.Shape as IPolygon4;
                    IGeometryCollection gc = poly4.ConnectedComponentBag as IGeometryCollection;
                    for (int i = 0; i < poly4.ExteriorRingCount; i++)
                    {
                        InsertFeature(insertCurr, insertBuff, feature, gc.get_Geometry(i));
                    }
                    feature.Delete();
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(loopCursor);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(insertCurr);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(insertBuff);

            }
            catch (System.Exception ex)
            {
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
            }

        }
       
           
        public IGeometryDef getSpatialInfo()
        {
            IGeometryDef geoDef = null;
            try
            {
                IField f;
                int fieldIndex = 0;
                IFeatureCursor fc;
                fc = this.mCurrStepMap.mySelf.Search(null, false);
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
//        private void makeNewAnimalFeatureClass(ref AnimalMap in_outMap, IGeometryDef gd)
//        {
//            try
//            {

//                string fileName = in_outMap.mySelf.AliasName;
//                in_outMap.removeAllPolygons();
//                this.removeAnimalMaps(fileName);
//                in_outMap.makeMap(fileName, this.mPath, gd);

//            }
//            catch (System.Exception ex)
//            {
//#if DEBUG
//                System.Windows.Forms.MessageBox.Show(ex.Message);
//#endif
//                FileWriter.FileWriter.WriteErrorFile(ex);
//            }
//        }
        private void makeOccupiedAvailable(string sex)
        {
            string search;
            string suitVal;
            string sexVal;
            int index_1;
            int index_2;
            int currTimeIndex;
            int availableIndex;
            IFeature feat = null;
            IFeatureCursor featCursor = null;
            try
            {


                //not sure how others will enter the data 
                fw.writeLine("inside makeOccupiedAvailable");
                sex = sex.ToUpper();
                if (sex == "MALE")
                    search = "OCCUP_MALE";
                else
                    search = "OCCUP_FEMA";

                fw.writeLine("we are going to be looking for a field named " + search);
                if (occupiedFeatureClass.FindField("Available") < 0)
                {
                    fw.writeLine("could not find an avialble field so adding it");
                    IFieldEdit fieldEdit;
                    fieldEdit = new FieldClass();
                    fieldEdit.Name_2 = "Available";
                    fieldEdit.Type_2 = esriFieldType.esriFieldTypeSmallInteger;
                    occupiedFeatureClass.AddField(fieldEdit);
                }

                featCursor = this.occupiedFeatureClass.Search(null, false);
                feat = featCursor.NextFeature();
                while (feat != null)
                {
                    fw.writeLine("now searching for fields ");
                    index_1 = feat.Fields.FindField("SUITABILIT");
                    index_2 = feat.Fields.FindField(search);
                    currTimeIndex = feat.Fields.FindField("CurrTime");
                    availableIndex = feat.Fields.FindField("Available");
                    suitVal = feat.get_Value(index_1).ToString().ToLower();
                    sexVal = feat.get_Value(index_2).ToString().ToLower();

                    fw.writeLine("index for SUITABILIT is " + index_1.ToString());
                    fw.writeLine("index for " + search + " is " + index_2.ToString());
                    fw.writeLine("index for CurrTime is " + currTimeIndex.ToString());
                    fw.writeLine("index for Available is " + availableIndex.ToString());


                    if (suitVal == "suitable" && sexVal == "none")
                    {
                        feat.set_Value(availableIndex, 1);
                        fw.writeLine("setting the Available index to 1");
                    }
                    else
                    {
                        feat.set_Value(availableIndex, 0);
                        fw.writeLine("setting the Available index to 0");
                    }

                    //for some reason the currtime field is all zeros now?
                    //f.set_Value(currTimeIndex,1);
                    feat.Store();
                    feat = featCursor.NextFeature();
                    featCursor.Flush();


                }
            }
            catch (System.Exception ex)
            {
                FileWriter.FileWriter.WriteErrorFile(ex);
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(featCursor);
            }
        }
       private void removeAnimalMaps(string inName)
       {
          string[] fileNames;
          try
          {
             fileNames = Directory.GetFiles(mPath + "\\" + inName, inName + "*");
             for (int i = 0; i < fileNames.Length; i++)
             {
                File.Delete(fileNames[i]);
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
        private void removeTempMaps(string inName)
        {
            string[] fileNames;
            try
            {
                fileNames = Directory.GetFiles(mPath, inName + "*");
                for (int i = 0; i < fileNames.Length; i++)
                {
                    File.Delete(fileNames[i]);
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
        private void removeSocialMapExtraFields(string fileName)
        {
            IFeatureClass ifc = null;
            IFields allFields = null;
            IField f = null;
            string fieldName;

            try
            {
                //sometimes this is called for adjusting home ranges and sometimes 
                //for swapping out the social map.  When homerange there is an index 
                //value.
                if (fileName.ToLower().Equals("homerange"))
                    fileName = fileName + this.HomeRangeIndex.ToString();
                fw.writeLine("inside removeSocialMapExtraFields for map name " + fileName);
                ifc = Map.openFeatureClass(this.mPath, fileName);
                allFields = ifc.Fields;
                for (int i = allFields.FieldCount - 1; i >= 0; i--)
                {
                    fieldName = allFields.get_Field(i).AliasName;
                    if (fieldName.IndexOf("1") > 0 || fieldName.IndexOf("2") > 0)
                    {
                        f = allFields.get_Field(i);
                        ifc.DeleteField(f);
                    }
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ifc);
            }
            catch (System.Exception ex)
            {
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ifc);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(allFields);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(f);
            }
        }
        private void resetCurrStep()
        {
            IFields myFields = null;
            try
            {
                removeTempMaps("CurrStep");
                myFields = getFieldCollection(this.geoDef);
                mCurrStepMap = new Map(this.mPath, "CurrStep" + name, this.geoDef, myFields);
            }
            catch (System.Exception ex)
            {
#if (DEBUG)
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
                FileWriter.FileWriter.WriteErrorFile(ex);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(myFields);
            }
        }
        
        private void resetTimeStep()
        {
            resetCurrStep();
            if (this.dissolveTable != null)
            {
                this.dissolveTable.DeleteSearchedRows(null);
            }

        }
//        private void resetFeatureClass(IFeatureClass inFC)
//        {
//            try
//            {
//                if (inFC != null)
//                {
//                    IFeatureCursor tmpCur;
//                    IFeature tmpFeature;
//                    tmpCur = inFC.Search(null, false);
//                    tmpFeature = tmpCur.NextFeature();

//                    while (tmpFeature != null)
//                    {
//                        tmpFeature.Delete();
//                        tmpFeature = tmpCur.NextFeature();
//                    }
//                }

//            }
//            catch (System.Exception ex)
//            {
//#if (DEBUG)
//                System.Windows.Forms.MessageBox.Show(ex.Message);
//#endif
//                FileWriter.FileWriter.WriteErrorFile(ex);
//            }

//        }

        //private bool selectFromMap()
        //{


        //   IFeature feature = null;
        //   int featureCount = 0;
        //   bool success = true;
        //   try
        //   { 
        //      fw.writeLine("inside selectFromMap");
        //      fw.writeLine("getting the cursors and bufferes");
        //      this.searchCursor = this.selectFeatureClass.Search(queryFilter, false);
        //      // this.insertCursor = this.mSelectMap.mySelf.Insert(true);
        //      //  this.insertBuff = this.mSelectMap.mySelf.CreateFeatureBuffer();
        //      fw.writeLine("grab the first feature");
        //      feature = searchCursor.NextFeature();
        //      // int i = feature.Fields.FindField("CurrTime");
        //      // s= feature.get_Value(i).ToString();
        //      while (feature != null)
        //      {
        //         fw.writeLine("inside loop");
        //         insertBuff.Shape = feature.Shape;
        //         AddFields(insertBuff, feature);
        //         insertCursor.InsertFeature(insertBuff);

        //         if (++featureCount % 100 == 0)
        //         {
        //            insertCursor.Flush();
        //         }
        //         feature = searchCursor.NextFeature();
        //         // i = feature.Fields.FindField("CurrTime");
        //         //s= feature.get_Value(i).ToString();
        //      }
        //      // this.mergeMaps();
        //      fw.writeLine("leaving selectFromMap");

        //   }
        //   catch (System.Exception ex)
        //   {
        //      FileWriter.FileWriter.WriteErrorFile(ex);
        //      success = false;
        //   }
        //   return success;



        //}

        private IEnvelope CreateClipperEnvelope(IEnvelope fE, IEnvelope sE)
        {


            fw.writeLine("inside set up envelope making new envelope");
            IEnvelope clipper = new EnvelopeClass();
            clipper.XMax = this.getXMax(fE, sE);
            clipper.XMin = this.getXMin(fE, sE);
            clipper.YMax = this.getYMax(fE, sE);
            clipper.YMin = this.getYMin(fE, sE);
            fw.writeLine("so now clip envelope has these values");
            fw.writeLine("XMax = " + clipper.XMax.ToString());
            fw.writeLine("XMin =" + clipper.XMin.ToString());
            fw.writeLine("YMax =" + clipper.YMax.ToString());
            fw.writeLine("YMin =" + clipper.YMin.ToString());
            return clipper;
        }
        private void unionHomeRange()
        {
            IFeatureClass ifc = null;
            IFeatureLayer fl = null;
            ILayer l = null;
            IBasicGeoprocessor ibg = new BasicGeoprocessorClass();
            try
            {
                fl = new FeatureLayerClass();
                fl.FeatureClass = this.homeRangeFeatureClass;
                fl.Name = this.homeRangeFeatureClass.AliasName;
                this.myMap.ClearLayers();
                this.myMap.AddLayer(fl);
                l = this.myMap.get_Layer(0);
                table_2 = (ITable)l;
                table_1 = this.getTable(this.mSocialMap.mySelf);
                this.HomeRangeIndex++;
                this.buildDataSetName("HomeRange" + this.HomeRangeIndex.ToString());
                ifc = ibg.Union(table_1, false, table_2, false, this.tolerence, outShapeFileName);
                fw.writeLine("leaving unionHomeRangeMap");
                fw.writeLine("");
                fw.writeLine("Homerange feature class name is " + fl.Name);
                fw.writeLine("Social map feature class name is " + this.mSocialMap.mySelf.AliasName);
                fw.writeLine("");
            }
            catch (System.Exception ex)
            {
                FileWriter.FileWriter.WriteErrorFile(ex);
                FileWriter.FileWriter.AddToErrorFile("Homerange feature class name is " + fl.Name);
                FileWriter.FileWriter.AddToErrorFile("Social map feature class name is " + this.mSocialMap.mySelf.AliasName);
#if DEBUG
                System.Windows.Forms.MessageBox.Show(ex.Message);
#endif

            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ifc);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(table_2);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(table_1);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(fl);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ibg);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(homeRangeFeatureClass);

            }



        }
       



        #endregion

        #region gettersAndSetters
        public Map SocialMap
        {
            get { return mSocialMap; }
            set { mSocialMap = value; }
        }

        public string myPath
        {
            get { return mPath; }
            set
            {
                mPath = value;
                this.wsName.PathName = mPath;
            }
        }

        #endregion
    }
}
