using Animals;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace ModularSearch
{
	internal class Program
	{
		public static List<Animals.Animal> myAnimals = new List<Animals.Animal>();
		private static AnimalManager animalManager;
		private static NameValueCollection settings = ConfigurationManager.AppSettings;

		public static void Main(string[] args)
		{
			//MapManager mm = new MapManager();
			//mm = (MapManager)SerializeHelper.DeserializeFromFile(settings["MapSource"], mm);
			//mm.LoadInitialMaps();

			animalManager = new AnimalManager();
			animalManager.Initialize();
			animalManager.MoveTheAnimals();

			Console.WriteLine("All done press the famous any key to continue");
			Console.ReadKey();
		}
	}
}