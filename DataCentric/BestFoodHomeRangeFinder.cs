using System;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;


namespace PAZ_Dispersal
{
	/// <summary>
	/// Summary description for BestFoodHomeRangeFinder.
	/// </summary>
	public class BestFoodHomeRangeFinder:HomeRangeFinder     
	{
      static BestFoodHomeRangeFinder uniqueInstance;
		public BestFoodHomeRangeFinder()
		{
      }
      public static BestFoodHomeRangeFinder getInstance()
      {
         if (uniqueInstance==null)
         {
            uniqueInstance = new BestFoodHomeRangeFinder();
         }
         return uniqueInstance;
      }
      #region IHomeRangeFinder Members



      public override bool setHomeRangeCenter(Animal inAnimal, string inFileName)
      {
         bool foundHomeRange = true;
         base.setDistance(inAnimal);
         this.setFoodRank(inAnimal.DistanceWeight,inAnimal.MySites);
         List<EligibleHomeSite> qs = inAnimal.MySites.getQualifiedSites();
         inAnimal.HomeRangeCenter = base.chooseHomeRangeCenter(qs, inAnimal.HomeRangeArea) as PointClass;
         if (inAnimal.HomeRangeCenter == null)
            foundHomeRange = false;

         return foundHomeRange;

      }

      private void setFoodRank(double distanceFactor,EligibleHomeSites inAnimalSites)
      {
         double d = 0;
         double adjustDistance = 0;
         fw.writeLine("inside setFoodRank with a distance factor of " + distanceFactor.ToString());
         try
         {
            fw.writeLine("starting the loop through " + inAnimalSites.Count.ToString() + " sites");
            foreach (EligibleHomeSite ehs in inAnimalSites)
            {
               adjustDistance = Math.Pow(ehs.DistanceFromCurrLocation, (1 / distanceFactor));
               fw.writeLine(ehs.X.ToString() + " " + ehs.Y.ToString() + " site is eligible");
               fw.writeLine("the distance from current location is " + ehs.DistanceFromCurrLocation.ToString());
               fw.writeLine("so adjusted distace value is " + adjustDistance.ToString());
               fw.writeLine("the food value is " + ehs.Food.ToString());
               //BC Saturday, February 16, 2008 made chanage from (ehs.Rank = ehs.Food * adjustDistance)
               ehs.Rank = ehs.Food / adjustDistance;
               fw.writeLine("so its adjusted rank is " + ehs.Rank.ToString());
               d += ehs.Rank;
            }
            setRanges(d,inAnimalSites);
         }
         catch (System.Exception ex)
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
