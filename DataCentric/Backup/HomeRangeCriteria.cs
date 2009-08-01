using System;

namespace PAZ_Dispersal
{
	/// <summary>
	/// contains the criteria for calculating the home range area
	/// </summary>
	public class HomeRangeCriteria : IHomeRangeCriteria
	{
      private double mArea;
      private double mDistanceMean;
      private double mDistanceSD;
      private double mDistanceWeight;
     

		public HomeRangeCriteria()
      {
         mArea=0;
         mDistanceMean=0;
         mDistanceSD=0;
         mDistanceWeight=0;
      }
   
      
      public HomeRangeCriteria (double inArea,double inDM, double inDSD,double inDW)
      {
         mArea=inArea;
         mDistanceMean=inDM;
         mDistanceSD=inDSD;
         mDistanceWeight=inDW;
      }

      
      #region IHomeRangeCriteria Members
      public double Area
		{
			get { return mArea; }
			set  { mArea = value; }
		}

      public double DistanceMean
		{
			get { return mDistanceMean; }
			set  { mDistanceMean = value; }
		}

      public double DistanceSD
		{
			get { return mDistanceSD; }
			set  { mDistanceSD = value; }
		}

      public double DistanceWeight
		{
			get { return mDistanceWeight; }
			set  { mDistanceWeight = value; }
		}

     
      #endregion
   }
}
