
using System;
using ESRI.ArcGIS.Geometry;

namespace PAZ_Dispersal
{
	
	public class crossOverInfo
	{
      private PointClass mPoint;
      private double mDistance;
      private double mCurrPolyValue;
      private double mNewPolyValue;
       

      public PointClass Point
		{
			get { return mPoint; }
			set { mPoint = value; }
		}

      public double Distance
		{
			get { return mDistance; }
			set { mDistance = value; }
		}

      


		public crossOverInfo()
		{
		   this.mDistance = 0.0;
	      this.mPoint = null;
         this.CurrPolyValue = 0.0;
         this.NewPolyValue = 0.0;
        
		}

      public double CurrPolyValue
		{
			get { return mCurrPolyValue; }
			set { mCurrPolyValue = value; }
		}

      public double NewPolyValue
		{
			get { return mNewPolyValue; }
			set { mNewPolyValue = value; }
		}

	}
}
