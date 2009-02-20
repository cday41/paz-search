using ESRI.ArcGIS.Geometry;
using System;

namespace PAZ_Dispersal
{
	/// <summary>
	/// Summary description for InitialAnimalAttributes.
	/// </summary>
	public class InitialAnimalAttributes
	{
      private char mySex;
      private PointClass myLocation;
      private int myNumToMake;

		public InitialAnimalAttributes()
		{
			
         myLocation = new PointClass();
         myNumToMake = 0;
		}
      public void setPointValues(IPoint inP)
      {
         myLocation.X = inP.X;
         myLocation.Y = inP.Y;

      }
      #region getters and setters
      public char Sex
		{
			get { return mySex; }
			set { mySex = value; }
		}

      public PointClass Location
		{
			get { return myLocation; }
			set { myLocation = value; }
		}

      public int NumToMake
		{
			get { return myNumToMake; }
			set { myNumToMake = value; }
		}
      #endregion

     
	}
}
