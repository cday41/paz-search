using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using FileWriter;
using ESRI.ArcGIS.DataSourcesFile;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geoprocessing;

namespace PAZ_Dispersal
{
   class DataManipulator
   {

		#region Fields (4) 


      string tempLayer1;
      string tempLayer2;

      FileWriter.FileWriter fw;
      Geoprocessor myProcessor;

		#endregion Fields 

		#region Constructors (2) 

      public DataManipulator(string fileName)
      {
         myProcessor = new Geoprocessor();
         fw = new FileWriter.FileWriter(fileName);
      }

      public DataManipulator()
      {
         myProcessor = new Geoprocessor();
         fw = FileWriter.FileWriter.getDataLogger(@"C:\DataLogger.log");
         tempLayer1 = "\\layer1";
         tempLayer2 = "\\layer2";
      }

		#endregion Constructors 

		#region Private Methods (12) 

      private void AddPolyGon(IFeatureClass inFC, IPolygon inPoly)
      {
         IFields fields;
         IFeature feature;
         int index;
         feature = inFC.CreateFeature();
         fields = feature.Fields;
         index = fields.FindField("CurrTime");
         if (index >= 0)
            feature.set_Value(index, 1);
         index = fields.FindField("Available");
         if (index >= 0)
            feature.set_Value(index, 0);
         feature.Shape = inPoly;
         feature.Store();

      }

      public void CleanUnionResults(string UnionPath)
      {
         IFeatureClass fc;
         IQueryFilter qf;
         int SUITABILIT;
         int OCCUP_MALE;
         int OCCUP_FEMA;
         int SUITABIL_1;
         int OCCUP_MA_1;
         int OCCUP_FE_1;
         string suitValue;
         string occMale;
         string occFemale;
         
         GetFeatureClassFromFileName(UnionPath, out fc, out qf);
         IField field = fc.Fields.get_Field(2);
         qf.WhereClause = field.AliasName + " = -1";
         IFeatureCursor curr = fc.Update(qf, false);
         SUITABILIT = curr.FindField("SUITABILIT");
         OCCUP_MALE = curr.FindField("OCCUP_MALE");
         OCCUP_FEMA = curr.FindField("OCCUP_FEMA");
         SUITABIL_1 = curr.FindField("SUITABIL_1");
         OCCUP_MA_1 = curr.FindField("OCCUP_MA_1");
         OCCUP_FE_1 = curr.FindField("OCCUP_FE_1");

         IFeature feat = curr.NextFeature();
         while (feat != null)
         {
            suitValue = feat.get_Value(SUITABIL_1).ToString();
            occMale = feat.get_Value(OCCUP_MA_1).ToString();
            occFemale = feat.get_Value(OCCUP_FE_1).ToString();

            feat.set_Value(SUITABILIT, suitValue);
            feat.set_Value(OCCUP_MALE, occMale);
            feat.set_Value(OCCUP_FEMA, occFemale);

            feat.Store();
            feat = curr.NextFeature();
         }
         curr.Flush();
         System.Runtime.InteropServices.Marshal.ReleaseComObject(fc);
         System.Runtime.InteropServices.Marshal.ReleaseComObject(qf);
         System.Runtime.InteropServices.Marshal.ReleaseComObject(curr);
      }

      private void ClipFeatures(string clipFromLayer, string clipFeatureLayer, string outFeatureClassName)
      {
         ESRI.ArcGIS.AnalysisTools.Clip c = new ESRI.ArcGIS.AnalysisTools.Clip();
         c.in_features = clipFromLayer;
         c.clip_features = clipFeatureLayer;
         c.out_feature_class = outFeatureClassName;
         RunProcess(c, null);
      }

      private void CopyFeaturesToFeatureClass(string inLayer, string RecievingFeatureClass)
      {
         CopyFeatures cf = new CopyFeatures();
         cf.in_features = inLayer;
         cf.out_feature_class = RecievingFeatureClass;
         RunProcess(cf,null);
      }

      private void DissolveFeatures(string layerName, string outName, string fieldName)
      {
         Dissolve d = new Dissolve();
         d.in_features = layerName;
         d.out_feature_class = outName;
         d.dissolve_field = fieldName as object;
         d.multi_part = "SINGLE_PART";
         RunProcess(d, null);
      }

      private IFeatureClass GetFeatureClass(string path, string fileName)
      {
         IFeatureClass ifc = null;
         try
         {
            
            IWorkspaceFactory wrkSpaceFactory = new ShapefileWorkspaceFactory();
            IFeatureWorkspace featureWorkspace = null;
            featureWorkspace = (IFeatureWorkspace)wrkSpaceFactory.OpenFromFile(path, 0);
            ifc = featureWorkspace.OpenFeatureClass(fileName);
         }
         catch (COMException COMEx)
         {
            FileWriter.FileWriter.WriteErrorFile(COMEx);
                System.Windows.Forms.MessageBox.Show(COMEx.GetBaseException().ToString(),"COM Error: " + COMEx.ErrorCode.ToString()); 
         }

         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
              System.Windows.Forms.MessageBox.Show (ex.Source + " " );//+ ex.InnerException.ToString());
         }

         return ifc;
      }

      private void GetPathAndFileName(string inFullFilePath, out string path, out string fileName)
      {
         path = System.IO.Path.GetDirectoryName(inFullFilePath);
         fileName = System.IO.Path.GetFileName(inFullFilePath);
        
      }

      private void MakeLayer(string inFileName, string outLayerName)
      {
         MakeFeatureLayer makefeaturelayer = new MakeFeatureLayer();
         makefeaturelayer.in_features = inFileName;
         makefeaturelayer.out_layer = outLayerName;
         this.RunProcess(makefeaturelayer, null);
      }

      private void removeAllPolygons(ref IFeatureClass inFeatureClass)
      {
         IFeatureCursor tmpCur;
         IFeature tmpFeature;
         tmpCur = inFeatureClass.Update(null, false);
         tmpFeature = tmpCur.NextFeature();

         while (tmpFeature != null)
         {
            tmpFeature.Delete();
            tmpFeature = tmpCur.NextFeature();
         }
         tmpCur.Flush();
         int j = inFeatureClass.FeatureCount(null);




      }

      // Function for returning the tool messages.
      private void ReturnMessages(Geoprocessor gp)
      {
         try
         {
            if (gp.MessageCount > 0)
            {
               for (int Count = 0; Count <= gp.MessageCount - 1; Count++)
               {
                  string s = gp.GetMessage(Count);
                  if (s.Contains("ERROR") || s.Contains("WARNING"))
                  {
                     this.fw.writeLine(s);
                     System.Windows.Forms.MessageBox.Show("Error in DataManipulator");
                  }
                  this.fw.writeLine(s);
                  
               }
            }
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }

      }

      private void RunProcess(IGPProcess inProcess, ITrackCancel inCancel)
      {
         try
         {
            string toolbox = inProcess.ToolboxName;
            fw.writeLine("inside run process");
            fw.writeLine("the process I want to run is " + inProcess.ToolName);
            fw.writeLine("the tool box is " + toolbox);
            myProcessor.OverwriteOutput = true;
            myProcessor.Execute(inProcess, null);
            ReturnMessages(myProcessor);
            myProcessor.RemoveToolbox(toolbox);
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex.Message);
            ReturnMessages(myProcessor);
         }
      }

      private void SelectByValue(string inLayerName, string whereClause)
      {
         SelectLayerByAttribute selectByValue = new SelectLayerByAttribute();
         selectByValue.in_layer_or_view = inLayerName;
         selectByValue.selection_type = "NEW_SELECTION";
         selectByValue.where_clause = whereClause;
         this.RunProcess(selectByValue, null);
      }

      
      private void UnionFeatures(string LayerList, string fc)
      {
         fw.writeLine("inside UnionFeatures going to make " + fc);
         fw.writeLine("my layer list is " + LayerList);
         Union u = new Union();
         fw.writeLine("made new union tool");
         u.in_features = LayerList;
         fw.writeLine("just set the layers");
         u.out_feature_class = fc;
         fw.writeLine("set the feature class");
         fw.writeLine("Calling Run Process");
         this.RunProcess(u, null);
         fw.writeLine("back from runprocess");
      }

		#endregion Private Methods 

		#region Public Methods (7) 

      public void CheckLock(string inFile)
      {
         fw.writeLine("");
         fw.writeLine("inside Check Lock for file " + inFile);
         string s = myProcessor.TestSchemaLock(inFile);
         fw.writeLine("test returns " + s);
         if (s.Equals("false", StringComparison.CurrentCultureIgnoreCase))
         {
            fw.writeLine("Ok now check the attributes");
            if ((File.GetAttributes(inFile) & FileAttributes.Archive) == FileAttributes.Archive)
            {
               //DirectoryInfo di =new DirectoryInfo(System.IO.Path.GetDirectoryName(inFile));
               //di.Attributes = di.Attributes ^ FileAttributes.Normal;


               s = myProcessor.TestSchemaLock(inFile);
            }
         }

      }

      public void Clip(string inFileNameClipFrom, string inFileNameClipFeature, string outFileName)
      {
         if(! System.IO.File.Exists(inFileNameClipFeature))
         {
         string path;
         string fileName;
         this.GetPathAndFileName(inFileNameClipFeature, out path, out fileName);
         this.CreateEmptyFeatureClass(path, fileName);
         }

         this.MakeLayer(inFileNameClipFrom, "clipFrom");
         this.MakeLayer(inFileNameClipFeature, "clipFeature");
         this.ClipFeatures("clipFrom", "clipFeature", outFileName);
      }

      public void CopyToAnimalMap(string AnimalMapPath, string ClippedMapPath)
      {
         int num = 0;
         this.MakeLayer(ClippedMapPath, this.tempLayer1);
         this.CopyFeaturesToFeatureClass(this.tempLayer1, AnimalMapPath);
      }

      public void CreateEmptyFeatureClass(string dirName, string fileName)
      {
         try
         {
            CreateFeatureclass cf = new CreateFeatureclass();
            cf.out_path = dirName;
            cf.out_name = fileName;
            cf.geometry_type = "POLYGON";
            this.RunProcess(cf, null);
         }
         catch (System.Runtime.InteropServices.COMException ex)
         {
            System.Windows.Forms.MessageBox.Show(ex.Message);
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
   

      }

      public void DeleteAllFeatures(string inFileName)
      {
         IFeatureClass fc;
         IQueryFilter qf;
         GetFeatureClassFromFileName(inFileName, out fc, out qf);
         IFeatureCursor curr = fc.Update(null, false);
         IFeature feat = curr.NextFeature();
         while (feat != null)
         {
            feat.Delete();
            feat = curr.NextFeature();
         }
         curr.Flush();

         System.Runtime.InteropServices.Marshal.ReleaseComObject(curr);
         System.Runtime.InteropServices.Marshal.ReleaseComObject(fc);
         System.Runtime.InteropServices.Marshal.ReleaseComObject(qf);

      }

      private void GetFeatureClassFromFileName(string inFileName, out IFeatureClass fc, out IQueryFilter qf)
      {
         MakeFeatureLayer makefeaturelayer = new MakeFeatureLayer();
         makefeaturelayer.in_features = inFileName;
         makefeaturelayer.out_layer = "tempLayer";
         IGeoProcessorResult result = (IGeoProcessorResult)myProcessor.Execute(makefeaturelayer, null);


         IGPUtilities util = new GPUtilitiesClass();
         util.DecodeFeatureLayer(result.GetOutput(0), out fc, out qf);
      }

      public void Dissolve(string inFile, string outFile, string FieldNames)
      {
         this.MakeLayer(inFile, this.tempLayer1);
         this.DissolveFeatures(this.tempLayer1, outFile, FieldNames);
      }

      public bool MakeDissolvedTimeStep(string inFullFilePath, string dissovlePath, IPolygon inPoly1, IPolygon inPoly2)
      {
         IFeatureClass fc = null;
         string path;
         string fileName;
         bool result = true;
         try
         {
            fw.writeLine("inside MakeDissolvedTimeStep");
            fw.writeLine("file name is " + inFullFilePath);
            fw.writeLine("dissolvePath is " + dissovlePath);
            GetPathAndFileName(inFullFilePath,out path,out fileName);
            fc = this.GetFeatureClass(path,fileName);
            this.AddPolyGon(fc, inPoly1);
            this.AddPolyGon(fc, inPoly2);
            this.MakeLayer(inFullFilePath, this.tempLayer1);
            fw.writeLine("Calling check lock");
            this.CheckLock(dissovlePath);
            this.DissolveFeatures(this.tempLayer1, dissovlePath, "Id");
            this.removeAllPolygons(ref fc);
            
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
            result = false;
         }
         finally
         {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(fc);
         }
         return result;
      }

      public bool UnionAnimalClipData(string inAnimalPath, string inClipPath, string outPutFileName)
      {
         bool result = true;

         try
         {
            fw.writeLine("");
            fw.writeLine("inside UnionAnimalClipData for " + inAnimalPath);
            this.MakeLayer(inAnimalPath, this.tempLayer1);
            this.MakeLayer(inClipPath, this.tempLayer2);
            this.UnionFeatures(this.tempLayer1 + "; " + this.tempLayer2, outPutFileName);
            this.CleanUnionResults(outPutFileName);
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
            result = false;
         }
         return result;
      }

		#endregion Public Methods 

   }

  

}
        



