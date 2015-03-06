using DotSpatial.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Spatial;

namespace DataHelper
{
	public class GisHelper : System.IDisposable
	{
		private MapEntites me;

		#region Public Methods

		public void loadFoodFiles(string inFileName)
		{
			List<base_food> myFoodSites = new List<base_food>();
			IFeatureSet fs = FeatureSet.Open(inFileName);
			DataTable dt = fs.DataTable;

			for (int i = 0; i < dt.Rows.Count; i++)
			{
				base_food food = new base_food();
				IFeature f = fs.Features[i];
				food.geom = DbGeometry.FromText(f.BasicGeometry.ToString());
				food.PROBCAP = GetNullableDouble(dt.Rows[i], "PROBCAP");
				food.SD_SIZE = GetLong(dt.Rows[i], "SD_SIZE");
				food.X_SIZE = GetLong(dt.Rows[i], "X_SIZE");
				myFoodSites.Add(food);
			}
			using (me = new MapEntites())
			{
				me.Database.ExecuteSqlCommand("Truncate Table [base_food]");
				me.base_food.AddRange(myFoodSites);
				me.SaveChanges();
			}
		}

		public void loadMoveFiles(string inFileName)
		{
			List<base_move> myMoveSites = new List<base_move>();
			IFeatureSet fs = FeatureSet.Open(inFileName);
			DataTable dt = fs.DataTable;

			for (int i = 0; i < dt.Rows.Count; i++)
			{
				base_move move = new base_move();
				IFeature f = fs.Features[i];
				move.geom = DbGeometry.FromText(f.BasicGeometry.ToString());
				move.CROSSING = GetNullableDouble(dt.Rows[i], "CROSSING");
				move.ENERGYUSED = GetNullableDouble(dt.Rows[i], "ENERGYUSED");
				move.MSL = GetLong(dt.Rows[i], "MSL");
				move.MVL = GetNullableDouble(dt.Rows[i], "MVL");
				move.PR_X = GetNullableDouble(dt.Rows[i], "PR_X");
				myMoveSites.Add(move);
			}
			using (me = new MapEntites())
			{
				me.Database.ExecuteSqlCommand("Truncate Table [base_move]");
				me.base_move.AddRange(myMoveSites);
				me.SaveChanges();
			}
		}

		public void loadReleaseFile(string inFileName)
		{
			List<base_release> myReleaseSites = new List<base_release>();
			IFeatureSet fs = FeatureSet.Open(inFileName);

			DataTable dt = fs.DataTable;
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				base_release br = new base_release();
				IFeature f = fs.Features[i];
				br.geom = DbGeometry.FromBinary(f.BasicGeometry.ToBinary(), 0);

				br.MALES = GetLong(dt.Rows[i], "MALES");
				br.FEMS = GetLong(dt.Rows[i], "FEMS");
				br.RELEASESIT = GetLong(dt.Rows[i], "RELEASESIT");
				myReleaseSites.Add(br);
			}
			using (me = new MapEntites())
			{
				me.Database.ExecuteSqlCommand("Truncate Table [base_release]");
				me.base_release.AddRange(myReleaseSites);
				me.SaveChanges();
			}
		}

		public void loadRiskFiles(string inFileName)
		{
			List<base_risk> myRiskSites = new List<base_risk>();
			IFeatureSet fs = FeatureSet.Open(inFileName);
			DataTable dt = fs.DataTable;

			for (int i = 0; i < dt.Rows.Count; i++)
			{
				base_risk risk = new base_risk();
				IFeature f = fs.Features[i];
				risk.geom = DbGeometry.FromText(f.BasicGeometry.ToString());
				risk.RISK = GetNullableDouble(dt.Rows[i], "RISK");
				//	risk.LABEL = dt.Rows[i]["LABEL"];
				myRiskSites.Add(risk);
			}
			using (me = new MapEntites())
			{
				me.Database.ExecuteSqlCommand("Truncate Table [base_risk]");
				me.base_risk.AddRange(myRiskSites);
				me.SaveChanges();
			}
		}

		public void loadSocialFiles(string inFileName)
		{
			List<base_social> mySocialSites = new List<base_social>();
			IFeatureSet fs = FeatureSet.Open(inFileName);
			DataTable dt = fs.DataTable;

			for (int i = 0; i < dt.Rows.Count; i++)
			{
				base_social social = new base_social();
				IFeature f = fs.Features[i];
				social.geom = DbGeometry.FromText(f.BasicGeometry.ToString());
				social.OCCUP_FEMA = dt.Rows[i]["OCCUP_FEMA"].ToString();
				social.OCCUP_MALE = dt.Rows[i]["OCCUP_MALE"].ToString();
				social.SUITABILIT = dt.Rows[i]["SUITABILIT"].ToString();

				mySocialSites.Add(social);
			}
			using (me = new MapEntites())
			{
				me.Database.ExecuteSqlCommand("Truncate Table [base_social]");
				me.base_social.AddRange(mySocialSites);
				me.SaveChanges();
			}
		}

		#endregion Public Methods

		#region Private Methods

		private double? GetNullableDouble(DataRow dr, string inFieldName)
		{
			return (double?)System.Convert.ToDouble(dr[inFieldName]);
		}

		private long? GetLong(DataRow dr, string inFieldName)
		{
			return (long?)System.Convert.ToInt32(dr[inFieldName]);
		}

		private int GetInt(DataRow dr, string inFieldName)
		{
			return System.Convert.ToInt32(dr[inFieldName]);
		}

		#endregion Private Methods

		public void Dispose()
		{
		}
	}
}