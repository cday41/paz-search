using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization
{
   class Program
   {
      #region Methods (2)

      // Private Methods (2) 

      static void Main(string[] args)
      {
         string fileName = "animal.xml";
         List<Animal> myAnimals = null;
         Console.Write("Num args " + args.Count().ToString());
         Console.ReadKey();
         if (args.Count() == 0)
            myAnimals = MakeBrandNewAnimals();
         else
            myAnimals = SerializeHelper.DeserializeFromFile(fileName, new List<Animal>()) as List<Animal>;


         foreach (Animal a in myAnimals)
         {

            Console.WriteLine(a.ToString());
         }

         Console.WriteLine("Press a key to continue");
         Console.ReadKey();

         int i = 0;
         foreach (Animal a in myAnimals)
         {
            if (i % 2 == 0)
            {
               a.Sex = "female";

            }
            if (i % 5 == 0)
            {
               a.Alive = false;
            }
            Console.WriteLine(a.ToString());
            i++;

         }
         Console.WriteLine("Press a key to continue");
         Console.ReadKey();

         SerializeHelper.SerializeObjectToFile(fileName, myAnimals);
      }

      static List<Animal> MakeBrandNewAnimals()
      {
         List<Animal> returnValue = new List<Animal>();
         for (int i = 0; i < 10; i++)
         {
            Animal a = new Animal(i, "male");
            returnValue.Add(a);
         }
         return returnValue;
      }

      #endregion Methods
   }
}

