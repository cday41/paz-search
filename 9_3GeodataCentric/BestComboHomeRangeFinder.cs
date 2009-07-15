using System;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;
using System.Linq;
namespace PAZ_Dispersal
{
   /// <summary>
   /// 
   /// </summary>
   public sealed class BestComboHomeRangeFinder : HomeRangeFinder
   {
      #region Public Members (4)

      #region Constructors (1)

      public BestComboHomeRangeFinder()
      {
         // 
         // TODO: Add constructor logic here
         //
      }

      #endregion Constructors
      #region Methods (3)

      public static BestComboHomeRangeFinder getInstance()
      {
         if (uniqueInstance == null)
         {
            uniqueInstance = new BestComboHomeRangeFinder();
         }
         return uniqueInstance;
      }

      public void setComboRank(List<EligibleHomeSite> inGoodSites, double distWeight)
      {
         double d = 0;
         double adjustDistance = 0;
         double maxFood = 0.0;
         double maxRisk = 0.0;
         double foodValue = 0.0;
         double riskValue = 0.0;



         //set up to sort by food first
         EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Food;
         inGoodSites.Sort();
         maxFood = inGoodSites[0].Food;
         //get the risk data now
         EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Risk;
         inGoodSites.Sort();
         maxRisk = inGoodSites[0].Risk;
         fw.writeLine("inside setComboRank with a distance factor of " + distWeight.ToString());
         fw.writeLine("max food was " + maxFood.ToString());
         fw.writeLine("max Risk was " + maxRisk.ToString());
         try
         {
            for (int i = 0; i < inGoodSites.Count; i++)
            {
               EligibleHomeSite ehs = inGoodSites[i];
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
            siteManager.MySites = inGoodSites;
            siteManager.setRanges(d);
            //siteManager.SetRanges((d);
            //setRanges(d);
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      public override bool setHomeRangeCenter(Animal inAnimal, string inFileName)
      {
         bool foundHomeRange = true;
         List<EligibleHomeSite> goodSites = new List<EligibleHomeSite>();
         try
         {
            // this will create a point map of all the steps in the animals memory.
            // It will be made in the Animal's home directory with the name Step.shp
            if (this.setSuitableSites(inAnimal, inFileName))
            {
               //now see if any of the areas visited are eligible and large enough
               //this will create a polygon map in the Animal's home directory with the name SuitablePolygons.shp
               if (this.setSuitablePolygons(inAnimal.HomeRangeArea, inAnimal.Sex, inFileName))
               {
                  // Now see if any of 
                  List<EligibleHomeSite> suitableSites = this.getSuitableSteps(inFileName);
                  var q = inAnimal.MyVisitedSites.MySites.Intersect(suitableSites, new SiteComparer());
                  //make sure there were some points
                  if (q.Count() > 0)
                  {

                     foreach (EligibleHomeSite i in q)
                     {
                        goodSites.Add(i);
                     }
                  }
                  else
                  {
                     foundHomeRange = false;
                  }
               }
            }
            if (foundHomeRange)
            {
               base.setDistance(inAnimal.Location, goodSites);
               this.setComboRank(goodSites, inAnimal.DistanceWeight);
               inAnimal.HomeRangeCenter = base.getHomeRangeCenter(goodSites) as PointClass;
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

      #region Non-Public Members (1)

      #region Fields (1)

      static BestComboHomeRangeFinder uniqueInstance;

      #endregion Fields

      #endregion Non-Public Members
   }
}
