using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataHelper;

namespace Map_Manager
{
	 public static class Release
	 {
	

		 
		public static bool  GetReleaseSiteInfo(out List<long?> numMales, out List<long?> numFemales, out List<DbGeometry> location)
		 {
			 DbHelper db = new DbHelper();
			
			 bool success = true;
			 numFemales = new List<long?>();
			 numMales = new List<long?>();
			 location = new List<DbGeometry>();
			 var tempRelease = db.GetReleaseSites();

			foreach(var rs in tempRelease)
			{
				numFemales.Add(rs.FEMS);
				numMales.Add(rs.MALES);
				location.Add(rs.geom);
			}

			 return success;
		 }

	 }
}
