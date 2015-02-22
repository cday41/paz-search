using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Map_Manager
{
    public static class Release
	 {
		static MapEntities me = new MapEntities();

		 
		public static bool  GetReleaseSiteInfor(out List<long?> numMales, out List<long?> numFemales, out List<DbGeometry> location)
		 {
			
			 bool success = true;
			 numFemales = new List<long?>();
			 numMales = new List<long?>();
			 location = new List<DbGeometry>();
			 var tempRelease = me.base_release;

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
