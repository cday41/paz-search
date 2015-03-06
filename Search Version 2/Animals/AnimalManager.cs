using Map_Manager;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Threading.Tasks;

namespace Animals
{
	public class AnimalManager
	{
		private List<Animal> myAnimals;
		//private static int change = 0;

		public AnimalManager()
		{
			myAnimals = new List<Animal>();

			//GetNewAnimals();
		}

		#region PublicMethods

		public void Initialize()
		{
			this.DeleteAllAnimals();
			this.GetNewAnimals();
		}

		public void MoveTheAnimals()
		{
			Console.WriteLine("Starting Move the Animals at " + DateTime.Now.ToLongTimeString());
			List<AnimalPath> myPaths = new List<AnimalPath>();
			Mover.Mover mover = new Mover.Mover();
			for (int i = 0; i < 1000; i++)
			{
				//foreach (Animals.Animal a in myAnimals)
				Parallel.ForEach(myAnimals, a =>
				{
					AnimalPath ap = new AnimalPath();
					a.Move_Values.PercentTimeStep = 0;
					ap.AnimalID = a.ID;
					ap.TimeStep = i;
					a.Move_Values.TimeStep = i;
					mover.move(a.Move_Values);
					ap.CurrLocation = a.Move_Values.End;
					a.AnimalPaths.Add(ap);
				}
				);
				this.UpdateAllAnimalsModifiers();
			}
			Console.WriteLine("Done moving at " + DateTime.Now.ToLongTimeString());
			Console.WriteLine("now the data base part");
			UpdateAllAnimalsLocation(myAnimals);
			Console.WriteLine("Finish Move the Animals at " + DateTime.Now.ToLongTimeString());
		}

		

		#endregion PublicMethods

		#region PrivateMethods

		private void AddAnimalsToDB()
		{
			using (AnimalsEntities animalProxy = new AnimalsEntities())
			{
				animalProxy.Animals.AddRange(myAnimals);
				animalProxy.SaveChanges();
			}
		}

		private void BuildAnimals(DbGeometry inLocation, long? numAnimals, string inSex)
		{
			Animals.Animal a = new Animals.Animal();
			for (int i = 0; i < numAnimals; i++)
			{
				a = new Animals.Animal();
				a.Sex = inSex;
				a.CurrLocation = inLocation;
				a.UpdateModifiers();
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

		private void DeleteAllAnimals()
		{
			using (AnimalsEntities ae = new AnimalsEntities())
			{
				ae.Database.ExecuteSqlCommand("Truncate Table [AnimalPath]");
				ae.Database.ExecuteSqlCommand("Delete from Animal");
				ae.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('dbo.Animal', RESEED, -1);");
				ae.SaveChanges();
			}
		}

		private void GetNewAnimals()
		{
			List<long?> numFemales;
			List<long?> numMales;
			List<DbGeometry> locations;
			Release.GetReleaseSiteInfo(out numMales, out numFemales, out locations);

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
			Parallel.ForEach(inA, a =>
				  {
					  using (AnimalsEntities ae = new AnimalsEntities())
					  {
						  Animal CurrAnimal = ae.Animals.Find(a.ID);
						  CurrAnimal.CurrLocation = a.Move_Values.End;
						  CurrAnimal.Move_Values.Start = a.Move_Values.End;
						  AnimalPath path = a.AnimalPaths.LastOrDefault();
						  CurrAnimal.AnimalPaths = a.AnimalPaths;
						  ae.SaveChanges();
					  }// end using
				  }//end foreach scope
				  );//end foreach loop
		}

		private void UpdateAllAnimalsModifiers()
		{

		//	foreach (Animal a in myAnimals)
			Parallel.ForEach(myAnimals, a =>
				{ a.UpdateModifiers(); });
		}

		#endregion PrivateMethods
	}
}//namespace 