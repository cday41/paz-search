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
      #region private member variables

      static ClosestHomeRangeFinder uniqueInstance;
      #endregion

      public static ClosestHomeRangeFinder getInstance()
      {
         if (uniqueInstance == null)
         {
            uniqueInstance = new ClosestHomeRangeFinder();
         }
         return uniqueInstance;
      }
      private ClosestHomeRangeFinder()
      {

      }

      public override bool setHomeRangeCenter(Animal inAnimal, string fileName)
      {
         bool success = false;
         int index;
         double polyArea;
         PointClass currEligibleSite=null;
         try
         {
            fw.writeLine("inside setHomeRangeCenter in the ClosestHomeRangeFinder");
            fw.writeLine(" for Animal Number " + inAnimal.IdNum.ToString() + " and the file name is " + fileName);
            string sex = inAnimal.Sex;
            //make sure there are available sites
            fw.writeLine("checking to see if there are any suitable sites");
            if (setSuitableSites(inAnimal, fileName))
            {
               fw.writeLine("there are " + inAnimal.MySites.Count.ToString() + " sites to work with ");
               double requiredArea = inAnimal.HomeRangeArea;
               fw.writeLine("we need " + requiredArea.ToString() );
               for (index = inAnimal.MySites.Count - 1; index >= 0; index--)
               {
                  fw.writeLine("looking at site number " + index.ToString());
                  currEligibleSite = inAnimal.GetEligibleStep(index);
                  if (this.getArea(currEligibleSite) >= requiredArea)
                  {
                     inAnimal.HomeRangeCenter = currEligibleSite;
                     success = true;
                     break;
                  }
               }
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
   }
}
