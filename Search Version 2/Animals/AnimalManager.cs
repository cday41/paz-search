using Map_Manager;
using System.Collections.Generic;
using System.Data.Entity.Spatial;

namespace Animals
{
	public class AnimalManager
	{
		private AnimalsEntities animalProxy;
		private List<Animal> myAnimals;

		public AnimalManager()
		{
			myAnimals = new List<Animal>();

			//GetNewAnimals();
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

		public void Initialize()
		{
			animalProxy = new AnimalsEntities();
			this.GetNewAnimals();
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

		#endregion PublicMethods

		#region PrivateMethods

		private void AddAnimalsToDB()
		{
			animalProxy.Animals.AddRange(myAnimals);
			animalProxy.SaveChanges();
		}

		private void BuildAnimals(DbGeometry inLocation, long? numAnimals, string inSex)
		{
			Animals.Animal a = new Animals.Animal();
			for (int i = 0; i < numAnimals; i++)
			{
				a = new Animals.Animal();
				a.Sex = inSex;
				a.CurrLocation = inLocation;
				a.Initialize();
				myAnimals.Add(a);
			}
		}

		private void BuildFemaleAnimals(DbGeometry inLocation, long? numAnimals)
		{
			if (numAnimals > 0)
			{
				BuildAnimals(inLocation, numAnimals, "Female");
			}
		}

		private void BuildMaleAnimals(DbGeometry inLocation, long? numAnimals)
		{
			if (numAnimals > 0)
			{
				BuildAnimals(inLocation, numAnimals, "Male");
			}
		}

		private void GetNewAnimals()
		{
			List<long?> numFemales;
			List<long?> numMales;
			List<DbGeometry> locations;
			Release.GetReleaseSiteInfor(out numMales, out numFemales, out locations);

			int numAnimals = numMales.Count;
			for (int i = 0; i < numAnimals; i++)
			{
				BuildMaleAnimals(locations[i], numMales[i]);
				BuildFemaleAnimals(locations[i], numFemales[i]);
			}
			this.AddAnimalsToDB();
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

		#endregion PrivateMethods
	}
}