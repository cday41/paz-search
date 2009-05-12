
using System;
using ESRI.ArcGIS.Geometry;

namespace PAZ_Dispersal
{
	
	public class crossOverInfo
	{
      private PointClass mPoint;
      private PointClass mNewPolyPoint;
      private double mDistance;
      private double mCurrPolyValue;
      private double mNewPolyValue;
      private bool onTheMap;
      private bool changedPolys;
      
      public bool ChangedPolys
      {
         get {return changedPolys;}
         set {changedPolys = value;}
      }
      public PointClass NewPolyPoint
      {
         get { return mNewPolyPoint; }
         set { mNewPolyPoint = value; }
      }


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
      public bool OnTheMap
      {
         get { return onTheMap; }
         set { onTheMap = value; }
      }

      


		public crossOverInfo()
		{
		   this.mDistance = 0.0;
	      this.mPoint = new PointClass();
         this.mPoint.X = 0.0;
         this.mPoint.Y = 0.0;
         this.CurrPolyValue = 0.0;
         this.NewPolyValue = 0.0;
         
         onTheMap = true;
         changedPolys = false;
        
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
