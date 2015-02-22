using System.Data.Entity.Spatial;

namespace Utility
{
	class ReleaseSites
	{
		int numMales;

		public int NumMales
		{
			get { return numMales; }
			set { numMales = value; }
		}
		int numFemales;

		public int NumFemales
		{
			get { return numFemales; }
			set { numFemales = value; }
		}
		DbGeometry location;

		public DbGeometry Location
		{
			get { return location; }
			set { location = value; }
		}
		public ReleaseSites()
		{

		}

		public ReleaseSites(int inMaleNum, int inFemaleNum, DbGeometry inLocation)
		{
			numMales = inMaleNum;
			numFemales = inFemaleNum;
			location = inLocation;

		}
	}
}
