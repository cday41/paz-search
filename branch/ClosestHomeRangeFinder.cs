using System;
using System.IO;
using System.Collections;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;
using FileWriter;
using DesignByContract;
namespace PAZ_Dispersal
{
   /// <summary>
   /// Summary description for ClosestHomeRangeFinder.
   /// </summary>
   public sealed class ClosestHomeRangeFinder:HomeRangeFinder
   {  
      #region private member variables
      
      static ClosestHomeRangeFinder uniqueInstance;
      #endregion

      public static ClosestHomeRangeFinder getInstance()
      {
         if (uniqueInstance==null)
         {
            uniqueInstance = new ClosestHomeRangeFinder();
         }
         return uniqueInstance;
      }
      private ClosestHomeRangeFinder()
      {
         
      }

      public override bool setHomeRangeCenter(Animal inAnimal, IFeatureClass inAnmialMemoryMap)
      {
         bool success = false;
         IFeatureCursor fc;
         int currStep;
         double polyArea;
         IPolygon currPoly;
         try
         {
            Check.Require(inAnmialMemoryMap != null,"InAnimalMap is null in ClosetHomeRanage finder");
            fw.writeLine("inside the setHomeRangeCenter in the ClosestHomeRangeFinder for george number " + inAnimal.IdNum.ToString());
            fw.writeLine("making the feature class and feature cursor");
            fc = this.getSuitablePolygons(inAnmialMemoryMap);
            fw.writeLine("now calling make array of polygons");
            makeArrayOfPolygons(fc);
            fw.writeLine("now loop through the sites");
            for(currStep = inAnimal.getNumStepsInPath();currStep>=0 && success == false;currStep--)
            {

               IPoint p = inAnimal.getLocation(currStep);
               fw.writeLine("current point is x = " + p.X.ToString() + " y = " + p.Y.ToString() );
               currPoly = this.getPolygon(p);
               if (currPoly != null)//if null was not in a suitable polygon
               {
                  polyArea = this.getArea(currPoly);
                  fw.writeLine("current polygon is " + polyArea.ToString());
                  fw.writeLine("george needs " + inAnimal.HomeRangeArea.ToString());
                  if (polyArea >= inAnimal.HomeRangeArea)
                  {
                     fw.writeLine("ok big enough area");
                     inAnimal.HomeRangeCenter = p as PointClass;
                     success = true;
                  }
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
         fw.writeLine("leaving setHomeRangeCenter with a value of " + success.ToString());
         return success;
      }
   }
}
