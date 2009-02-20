using System;
using ESRI.ArcGIS.Geometry;

namespace PAZ_Dispersal
{
	/// <summary>
	/// Summary description for InitialResidentInfo.
	/// </summary>
	public class InitialResidentInfo
	{
      private IPoint mLocation;
      private string mSex;
		public InitialResidentInfo()
		{
			mLocation = null;
         mSex = null;
		}

     public IPoint Location
		{
			get { return mLocation; }
			set  { mLocation = value; }
		}

      public string Sex
		{
			get { return mSex; }
			set  { mSex = value; }
		}

	}
}
