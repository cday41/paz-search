/******************************************************************************
 * Change Date:   02/10/2008
 * Change By:     Bob Cummings
 * Description:   In the sort routine we were using Risk value when sorting 
 *                by Rank.  Stupid cut and paste.
 * ***************************************************************************/
using System;
using System.Collections;
namespace PAZ_Dispersal
{
   /// <summary>
   /// //holds the information about areas we have visited
   /// </summary>
   public class EligibleHomeSite : IComparable

   {
      public enum SortMethod
      {Food = 0,Risk = 1,Rank = 2};
      #region private members
     
      private double mDistanceFromCurrLocation;
      private double mRank;
      private double mFood;
      private double mRisk;
      private double mX;
      private double mY;
      private static SortMethod _sortOrder;
      #endregion

      public EligibleHomeSite()
      {
         mDistanceFromCurrLocation = 0;
         mFood=0;
         mRisk=0;
         mX=0;
         mY=0;
      }

      public EligibleHomeSite(double inX, double inY)
      {
         mDistanceFromCurrLocation = 0;
         mFood=0;
         mRisk=0;
         mX=inX;
         mY=inY;
      }
      public EligibleHomeSite(double inFood, double inRisk, double inX, double inY)
      {
         mDistanceFromCurrLocation = 0;
         mFood=inFood;
         mRisk=inRisk;
         mX=inX;
         mY=inY;
      }

      


      #region getters and setters
      public double DistanceFromCurrLocation
		{
			get { return mDistanceFromCurrLocation; }
			set  { mDistanceFromCurrLocation = value; }
		}

     

      public double Rank
      {
         get { return mRank; }
         set { mRank = value; }
      }
      public double Food
		{
			get { return mFood; }
			set  { mFood = value; }
		}

      public double Risk
		{
			get { return mRisk; }
			set  { mRisk = value; }
		}


      public double X
		{
			get { return mX; }
			set  { mX = value; }
		}

      public double Y
		{
			get { return mY; }
			set  { mY = value; }
		}

      public static SortMethod SortOrder
      {
         get { return _sortOrder; }
         set { _sortOrder = value; }
      }

      #endregion

      #region IComparable Members

      public int CompareTo(object obj)
      {
         int result=0;
         try
         {
            if (obj is EligibleHomeSite)
            {
               EligibleHomeSite other = (EligibleHomeSite) obj;
               switch (_sortOrder)
               { 
                     //because the default is asscending order we do it backwards to get descending order
                  case SortMethod.Food:
                     result = other.Food.CompareTo(mFood);
                     break;
                  case SortMethod.Risk:
                     result = other.Risk.CompareTo(mRisk);
                     break;
                  case SortMethod.Rank:
                     result = other.Rank.CompareTo(mRank);
                     break;
                  default:
                     throw new System.Exception( _sortOrder.ToString()+ " is bad Sort Method for EligibleHomeSite");
               }
            }
            else
            {
               throw new ArgumentException("Object is not an EligibleHomeSite");
            }

         }
         catch(System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         return result;
      }

      #endregion
   }
}
