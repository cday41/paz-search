using System;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Collections.Generic;
namespace PAZ_Dispersal
{
   /// <summary>
   /// 
   /// </summary>
   public sealed class BestComboHomeRangeFinder : HomeRangeFinder
   {

		#region Static Fields (1) 

      static BestComboHomeRangeFinder uniqueInstance;

		#endregion Static Fields 

		#region Constructors (1) 

      public BestComboHomeRangeFinder()
      {
         // 
         // TODO: Add constructor logic here
         //
      }

		#endregion Constructors 

		#region Static Methods (1) 

      public static BestComboHomeRangeFinder getInstance()
      {
         if (uniqueInstance == null)
         {
            uniqueInstance = new BestComboHomeRangeFinder();
         }
         return uniqueInstance;
      }

		#endregion Static Methods 

		#region Overridden Methods (1) 



      public override bool setHomeRangeCenter(Animal inAnimal, string inFileName)
      {
         bool foundHomeRange = false;
         // this will tell us if we have any suitable sites to work with if so there will be
         // a point map made in the Animal's home directory with the name HomeSteps.shp

         if(this.setSuitableSites(inAnimal,inFileName))
         {
            //now see if any of them have enough area  
            String path = System.IO.Path.GetDirectoryName(inFileName);
            if(this.setSuitablePolygons(inAnimal.HomeRangeArea,path + "\\HomeSteps.shp",inFileName))
            {
               int i = 0;
            }
         }
         base.setDistance(inAnimal);
         this.setComboRank(inAnimal);
         
         return foundHomeRange;
         
      } 

		#endregion Overridden Methods 

       public void setComboRank(Animal inA)
       {
           double d = 0;
           double adjustDistance = 0;
           double maxFood = 0.0;
           double maxRisk = 0.0;
           double foodValue = 0.0;
           double riskValue = 0.0;

          
           
           //set up to sort by food first
           EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Food;
           inA.MySites.Sort();
           maxFood = inA.MySites.getSite(0).Food;
           //get the risk data now
           EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Risk;
           maxRisk = inA.MySites.getSite(0).Risk;
           fw.writeLine("inside setComboRank with a distance factor of " + inA.DistanceWeight.ToString());
           fw.writeLine("max food was " + maxFood.ToString());
           fw.writeLine("max Risk was " + maxRisk.ToString());
           try
           {
               foreach (EligibleHomeSite ehs in inA.MySites)
               {
                       adjustDistance = Math.Pow(ehs.DistanceFromCurrLocation, (1 / inA.DistanceWeight));
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
               inA.MySites.setRanges(d);
           }
           catch (System.Exception ex)
           {
#if (DEBUG)
               System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
               FileWriter.FileWriter.WriteErrorFile(ex);
           }
       }

   }
}
