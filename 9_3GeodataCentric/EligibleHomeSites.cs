/******************************************************************************
 * Change Date:   02/10/2008
 * Change By:     Bob Cummings
 * Description:   Added logging capabilites
 * 
 *                Changed logic in setting rank.  Specs said to calculate 
 *                the Nth root of the distance value.  Old code was
 *                Math.Pow(ehs.DistanceFromCurrLocation,distanceFactor)
 *                Changed to some article on the web.
 *                Math.Pow(ehs.DistanceFromCurrLocation,(1/distanceFactor)
 * 
 *                Added call to sort the sites before choosing one.
 * ***************************************************************************
 * Change Date:   02/11/2008
 * Change By:     Bob Cummings
 * Description:   Added logic to reset the rank based on a range of values.
 * ****************************************************************************
 * Change Date:   Saturday, February 16, 2008
 * Change By:     Bob Cummings
 * Description:   Added logic calculating rank via distance factor.
 * ***************************************************************************/

using System;
using System.IO;
using System.Collections.Generic;



namespace PAZ_Dispersal
{
   /// <summary>
   /// Summary description for EligibleHomeSites.
   /// </summary>
   public class EligibleHomeSites:System.Collections.ArrayList
   {

		#region Fields (1) 


      private FileWriter.FileWriter fw;

		#endregion Fields 

		#region Constructors (1) 

      public EligibleHomeSites()
      {
         this.buildLogger();
      }

		#endregion Constructors 

		#region Private Methods (5) 

      private void resetRank()
      {
         int i = 0;
         int j = 1;

        for(i=0,j=1; j< this.Count; i++,j++)
         {
            if(this.getSite(j).SuitableSite)
            {
               this.getSite(j).Rank = this.getSite(i).Rank + this.getSite(j).Rank;
            }
        }
         //sometimes it will only be .9999999999999987 and the random number could
         //conviebly be .9999999999989 so eliminate any chance.
         this.getSite(this.Count-1).Rank = 1.0;

      }

      public void setRanges(double inDouble)
      {
         double d = 0;
         try
         {
            fw.writeLine("now setting the ranges based on the total rankings = " + inDouble.ToString());
            fw.writeLine("starting the loop");
            foreach(EligibleHomeSite ehs in this)
            {
               if(ehs.SuitableSite)
               {  
                  fw.writeLine(ehs.X.ToString() + ehs.Y.ToString() + " is eligble site raw rank is " + ehs.Rank.ToString());
                  ehs.Rank = ehs.Rank / inDouble;
                  fw.writeLine("after adjusting rank is " + ehs.Rank.ToString());
                  d+=ehs.Rank;
               }
            }
            
            
            fw.writeLine("total rank is " + d.ToString());
            this.sortByRank();
            this.resetRank();
            

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      private void sortByFood()
      {
         EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Food;
         this.Sort();
      }

      private void sortByRank()
      {
          EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Rank;
         this.Sort();

      }

      private void sortByRisk()
      {
         EligibleHomeSite.SortOrder = EligibleHomeSite.SortMethod.Risk;
         this.Sort();
      }

		#endregion Private Methods 

		#region Public Methods (7) 

      public void addSite(EligibleHomeSite inSite)
      {
         this.Add(inSite);
      }

      public void addSite(EligibleHomeSite inSite, ref FileWriter.FileWriter fw)
      {
         fw.writeLine("adding an eligible home site");
         fw.writeLine("X = " + inSite.X.ToString() + " Y= " +inSite.Y.ToString());
         fw.writeLine("Food = " + inSite.Food.ToString() + " Risk = " + inSite.Risk.ToString());
         this.Add(inSite);
         fw.writeLine("now there are " + this.Count.ToString() + " sites to choose from");
      }

      public EligibleHomeSite getFirstSuitableSite()
      {
         EligibleHomeSite ehs = null;
         try
         {
            for(int i=0;i<this.Count;i++)
            {
               ehs = this[i] as EligibleHomeSite;
               if (ehs.SuitableSite)
               {
                  break;
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
         return ehs;
      }

      public EligibleHomeSite getSite (int index)
      {
         EligibleHomeSite ehs =null;
         try
         {
            if (this.Count>0)
            {
               ehs = this[index]as EligibleHomeSite;
            }
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return ehs;
      }

      public List<EligibleHomeSite> getQualifiedSites()
      {
         RandomNumbers rn = RandomNumbers.getInstance();
         double luckyNumber = rn.getUniformRandomNum();
         List<EligibleHomeSite> qualifyingSites = new List<EligibleHomeSite>();
         foreach (EligibleHomeSite ehs in this)
         {
            if (luckyNumber >= ehs.Rank)
            {
               qualifyingSites.Add(ehs);
            }
         }
         return qualifyingSites;

      }

      public List<Point> getPoints()
      {
         RandomNumbers rn = RandomNumbers.getInstance();
         double luckyNumber = rn.getUniformRandomNum();
         List<Point> qualifyingSites = new List<Point>();
         foreach (EligibleHomeSite ehs in this)
         {
            if (luckyNumber >= ehs.Rank)
            {
               Point p = new Point(ehs.X, ehs.Y);
               qualifyingSites.Add(p);
            }
         }
         return qualifyingSites;
      }

      

      public void setFoodRank(double distanceFactor)
      {
         double d=0;
         double adjustDistance = 0;
         fw.writeLine("inside setFoodRank with a distance factor of " + distanceFactor.ToString());
         try
         {
            fw.writeLine("starting the loop through " + this.Count.ToString() + " sites");
            foreach(EligibleHomeSite ehs in this)
            {
               if(ehs.SuitableSite)
               {
                  adjustDistance = Math.Pow(ehs.DistanceFromCurrLocation,(1/distanceFactor));
                  fw.writeLine(ehs.X.ToString() + " " + ehs.Y.ToString() + " site is eligible");
                  fw.writeLine("the distance from current location is " + ehs.DistanceFromCurrLocation.ToString() );
                  fw.writeLine("so adjusted distace value is " + adjustDistance.ToString());
                  fw.writeLine("the food value is " + ehs.Food.ToString());
                  //BC Saturday, February 16, 2008 made chanage from (ehs.Rank = ehs.Food * adjustDistance)
                  ehs.Rank = ehs.Food / adjustDistance;
                  fw.writeLine("so its adjusted rank is " + ehs.Rank.ToString());
                  d+= ehs.Rank;
               }
            }
            setRanges(d);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

      public void setRiskRank(double distanceFactor)
      {
         double d=0;
         double adjustDistance = 0;
         fw.writeLine("inside setRiskRank with a distance factor of " + distanceFactor.ToString());

         try
         {
            foreach(EligibleHomeSite ehs in this)
            {
               if(ehs.SuitableSite)
               {
                  adjustDistance = Math.Pow(ehs.DistanceFromCurrLocation,(1/distanceFactor));
                  fw.writeLine(ehs.X.ToString() + " " + ehs.Y.ToString() + " site is eligible");
                  fw.writeLine("the distance from current location is " + ehs.DistanceFromCurrLocation.ToString() );
                  fw.writeLine("so adjusted distace value is " + adjustDistance.ToString());
                  fw.writeLine("the risk value is " + ehs.Risk.ToString());

                  ehs.Rank = (1-ehs.Risk) / adjustDistance;
                  d+= ehs.Rank;
               }
            }
            setRanges(d);
         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

		#endregion Public Methods 

		#region Protected Methods (1) 

      protected void buildLogger()
      {
         string s;
         StreamReader sr; 
         bool foundPath = false;
         
         string st = System.Windows.Forms.Application.StartupPath;
         if(File.Exists( st + @"\logFile.dat"))
         {
            sr= new StreamReader( st + @"\logFile.dat");
            while(sr.Peek() > -1)
            {
               s = sr.ReadLine();
               if (s.IndexOf("HomeSiteLogPath") == 0)
               {
                  fw= FileWriter.FileWriter.getHomeSiteLogger(s.Substring(s.IndexOf(" ")));
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

		#endregion Protected Methods 

//end of buildLogger

   }
}
