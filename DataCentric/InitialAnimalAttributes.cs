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
      private IPoint myLocation;
      private int myNumToMake;
      private string myOrginalID;

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
      public string OrginalID
      {
         get{return myOrginalID;}
         set{myOrginalID = value;}
      }
      public char Sex
		{
			get { return mySex; }
			set { mySex = value; }
		}

      public IPoint Location
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
