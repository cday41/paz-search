using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Spatial;
using System.Diagnostics;


namespace ModularSearch
{
	 public class GisHelper
	 {

		public void loadShapeFile()
		{
			string Program = "shape2sql.exe" ;


			string args =  @"shp=F:\SearchInputAndBackup\SearchMaps\Ridenour\Food\early_food.shp" +
				@"-connstr=""DataSource=localhost;Initial Catalog=SEARCH;Integrated Security=SSPI;"" -table= EarlyFood";

			Process process = new Process();
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.FileName = Program;
			process.StartInfo.Arguments = args;
			process.Start();
			process.WaitForExit();
		}
	 }
}
