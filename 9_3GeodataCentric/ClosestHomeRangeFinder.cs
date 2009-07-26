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
   public sealed class ClosestHomeRangeFinder : HomeRangeFinder
   {
		#region Public Members (2) 

		#region Methods (2) 

      public static ClosestHomeRangeFinder getInstance()
      {
         if (uniqueInstance == null)
         {
            uniqueInstance = new ClosestHomeRangeFinder();
         }
         return uniqueInstance;
      }

      public override bool setHomeRangeCenter(Animal inAnimal, string fileName)
      {
         bool success = false;
         int index;
         PointClass HomeSite =null;
         try
         {
            fw.writeLine("inside setHomeRangeCenter in the ClosestHomeRangeFinder");
            fw.writeLine(" for Animal Number " + inAnimal.IdNum.ToString() + " and the file name is " + fileName);
            string sex = inAnimal.Sex;
            //make sure there are available sites
            fw.writeLine("checking to see if there are any suitable sites");
            if (base.FindHomeRange(inAnimal,fileName))
            {
               fw.writeLine("must have been now set the distances and rank them");
               base.setDistance(inAnimal);
               HomeSite = new PointClass();
               HomeSite.X = base.siteList[base.siteList.Count - 1].X;
               HomeSite.Y = base.siteList[base.siteList.Count - 1].Y;
               inAnimal.HomeRangeCenter = HomeSite;
               success = true;
               
               
            }
            else
            {
               fw.writeLine("must not have been");
            }

           
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         fw.writeLine("leaving setHomeRangeCenter with a value of " + success.ToString());
         return success;
      }

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (2) 

		#region Constructors (1) 

      private ClosestHomeRangeFinder()
      {

      }

		#endregion Constructors 
		#region Methods (1) 

      private double getArea(IPoint inPoint)
      {
         double area = 0;
         IPolygon searchPoly = null;
         IRelationalOperator relOp = null;
         IFeatureCursor searchCurr = null;
         IFeature feat = null;
         fw.writeLine("inside get area of the home range finder class for point X = " + inPoint.X.ToString() + " and Y = " + inPoint.Y.ToString());
         try
         {
            relOp = (IRelationalOperator)inPoint;

            searchCurr = myAvailableAreas.Search(null, true);

            while ((feat = searchCurr.NextFeature()) != null)
            {
               searchPoly = feat.Shape as IPolygon;
               if (relOp.Within(searchPoly))
               {
                  area = this.getArea(searchPoly);
                  break;
               }
            }
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         finally
         {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(searchPoly);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(searchCurr);
         }
         fw.writeLine("leaving with an area of " + area.ToString());
         return area;

      }

		#endregion Methods 

		#endregion Non-Public Members 


      #region private member variables

      static ClosestHomeRangeFinder uniqueInstance;
      #endregion
   }
}
