using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization
{
   [Serializable()]
  public class Animal
   {
		#region Fields (3) 

      int age;
      bool alive;
      string sex;

		#endregion Fields 

		#region Constructors (1) 

      public Animal()
      {
         alive = true;
      }
      public Animal(int inAge, string inSex):this()
      {
         age = inAge;
         sex = inSex;
   

      }

		#endregion Constructors 

      public override string ToString()
      {
         return "My age is " + age.ToString() + " sex  is " + sex + " alive is " + alive.ToString();
      }

		#region Properties (3) 

      public int Age
      {
         get { return age; }
         set { age = value; }
      }

      public bool Alive
      {
         get { return alive; }
         set { alive = value; }
      }

      public string Sex
      {
         get { return sex; }
         set { sex = value; }
      }

		#endregion Properties 
   }
}
