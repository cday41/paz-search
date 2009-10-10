using System;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;
using System.Linq;
namespace SEARCH
{
   /// <summary>
   /// 
   /// </summary>
   public sealed class BestComboHomeRangeFinder : HomeRangeFinder
   {
		#region Public Members (3) 

		#region Constructors (1) 

      public BestComboHomeRangeFinder()
      {
         // 
         // TODO: Add constructor logic here
         //
      }

		#endregion Constructors 
		#region Methods (2) 

      public static BestComboHomeRangeFinder getInstance()
      {
         if (uniqueInstance == null)
         {
            uniqueInstance = new BestComboHomeRangeFinder();
         }
         return uniqueInstance;
      }

      public override bool setHomeRangeCenter(Animal inAnimal, string inFileName)
      {
         bool foundHomeRange = true;
         
         try
         {
            if (base.FindHomeRange(inAnimal, inFileName))
            {
               base.setDistance(inAnimal.Location);
               this.setComboRank(inAnimal.HomeRangeCriteria.DistanceWeight);
               inAnimal.HomeRangeCenter = base.getHomeRangeCenter() as PointClass;
            }
            else
            {
               foundHomeRange = false;
            }
         }
         catch (Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
            foundHomeRange = false;
         }
         return foundHomeRange;
      }

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (2) 

		#region Fields (1) 

      static BestComboHomeRangeFinder uniqueInstance;

		#endregion Fields 
		#region Methods (1) 

      private void setComboRank(double distWeight)
      {
         double d = 0;
         double adjustDistance = 0;
         double maxFood = 0.0;
         double maxRisk = 0.0;
         double foodValue = 0.0;
         double riskValue = 0.0;



         //set up to sort by food first
         EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Food;
         base.siteList.Sort();
         maxFood = base.siteList[0].Food;
         //get the risk data now
         EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Risk;
         base.siteList.Sort();
         maxRisk = base.siteList[0].Risk;
         fw.writeLine("inside setComboRank with a distance factor of " + distWeight.ToString());
         fw.writeLine("max food was " + maxFood.ToString());
         fw.writeLine("max Risk was " + maxRisk.ToString());
         try
         {
            for (int i = 0; i < base.siteList.Count; i++)
            {
               EligibleHomeSite ehs = base.siteList[i];
               adjustDistance = Math.Pow(ehs.DistanceFromCurrLocation, (1 / distWeight));
               fw.writeLine(ehs.X.ToString() + " " + ehs.Y.ToString() + " site is eligible");
               fw.writeLine("the distance from current location is " + ehs.DistanceFromCurrLocation.ToString());
               fw.writeLine("so adjusted distace value is " + adjustDistance.ToString());
               fw.writeLine("this sites food value is " + ehs.Food.ToString());
               fw.writeLine("this sites risk value is " + ehs.Risk.ToString());
               foodValue = ehs.Food / maxFood;
               fw.writeLine("food value is " + foodValue.ToString());
               riskValue = (1 - ehs.Risk / maxRisk);
               fw.writeLine("risk value is " + riskValue.ToString());
               ehs.Rank = (foodValue + riskValue) / adjustDistance;
               fw.writeLine("final rank is " + ehs.Rank.ToString());
               d += ehs.Rank;
            }
            EligibleHomeSites siteManager = new EligibleHomeSites();
            siteManager.MySites = base.siteList;
            siteManager.setRanges(d);
            
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

		#endregion Methods 

		#endregion Non-Public Members 
   }
}
