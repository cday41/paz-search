using Map_Manager;
using System.Collections.Generic;
using System.Data.Entity.Spatial;

namespace Animals
{
	public class AnimalManager
	{

		private List<Animal> myAnimals;

		public AnimalManager()
		{
			myAnimals = new List<Animal>();
	
			GetNewAnimals();
		}
		#region PublicMethods
		public void DeleteAllAnimals()
		{
			using (AnimalsEntities ae = new AnimalsEntities())
			{
				ae.Database.ExecuteSqlCommand("Truncate Table [AnimalPath]");
				ae.Database.ExecuteSqlCommand("Delete from Animal");
				ae.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('dbo.Animal', RESEED, -1);");
				ae.SaveChanges();
			}
		}

		public void MoveTheAnimals()
		{
			Mover.Mover mover = new Mover.Mover();
			for (int i = 0; i < 10; i++)
			{
				foreach (Animals.Animal a in myAnimals)
				{
					a.Move_Values.TimeStep = i;
					mover.move(a.Move_Values);
				}
			}
		
			UpdateAllAnimalsLocation(myAnimals);
		}
		#endregion
		#region PrivateMethods
		private void BuildAnimals(base_release r, long? numAnimals, string inSex)
		{
			Animals.Animal a = new Animals.Animal();
			for (int i = 0; i < numAnimals; i++)
			{
				a = new Animals.Animal();
				a.Sex = inSex;
				a.CurrLocation = r.geom;
				a.Initialize();
				myAnimals.Add(a);
			}
		}

		private void BuildFemaleAnimals(base_release r)
		{
			long? numFemales = r.FEMS;
			if (numFemales > 0)
			{
				BuildAnimals(r, numFemales, "Female");
			}
		}

		private void BuildMaleAnimals(base_release r)
		{
			long? numMales;
			numMales = r.MALES;
			if (numMales > 0)
			{
				BuildAnimals(r, numMales, "Male");
			}
		}

		private void GetNewAnimals()
		{
			List<long?> numFemales;
			List<long?> numMales;
			List<DbGeometry> locations;
			Release.GetReleaseSiteInfor(out numMales, out numFemales, out locations);



			


		}
		private void UpdateAllAnimalsLocation(List<Animal> inA)
		{
			using (AnimalsEntities ae = new AnimalsEntities())
			{
				foreach (Animal a in inA)
				{
					a.CurrLocation = a.Move_Values.End;
					a.Move_Values.Start = a.Move_Values.End;
					AnimalPath ap = new AnimalPath();
					ap.AnimalID = a.ID;
					ap.CurrLocation = a.CurrLocation;
					ap.TimeStep = a.Move_Values.TimeStep;
					ae.AnimalPaths.Add(ap);
					ae.SaveChanges();
				}
			}
		}
		#endregion
	}
}