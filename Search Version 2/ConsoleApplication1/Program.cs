using Animals;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;

namespace ModularSearch
{
	internal class Program
	{
		public static List<Animals.Animal> myAnimals = new List<Animals.Animal>();
		private static AnimalManager animalManager;
		private static base_release release = null;

		public static void Main(string[] args)
		{
			animalManager = new AnimalManager();
			//animalManager.DeleteAllAnimals();
			

	

		
		}



		
	}
}