using System;

namespace SEARCH
{
	/// <summary>
	/// Summary description for DeadDisperser.
	/// </summary>
	public class DeadAnimal : Animal
	{
		#region Constructors (1) 

		public DeadAnimal()
		{
         wasRemovedFromMap = false;
         wasResident = false;
		}

      public DeadAnimal(Animal a):base()
      {
         this.AnimalAtributes = a.AnimalAtributes;
         this.IdNum = a.IdNum;
         this.IsDead = true;
         this.Location = a.Location;
         this.sex = a.Sex;
         this.TextFileWriter = a.TextFileWriter;
         
      }

		#endregion Constructors 

		#region Fields (2) 

		#region S to Z (2) 

      bool wasRemovedFromMap;
      bool wasResident;

		#endregion S to Z 

		#endregion Fields 

		#region Properties (2) 

		#region S to Z (2) 

      public bool WasRemovedFromMap
      {
         get { return wasRemovedFromMap; }
         set { wasRemovedFromMap = value; }
      }

      public bool WasResident
      {
         get { return wasResident; }
         set { wasResident = value; }
      }

		#endregion S to Z 

		#endregion Properties 

		#region Methods (1) 

		#region Public Methods (1) 

      public override void  doTimeStep(HourlyModifier inHM, DailyModifier inDM,DateTime currTime,bool doTextOutput, ref string status)
      {
         fw.writeLine("dead disperser number " + this.IdNum.ToString() + " is not takeing a timestep");
      }

		#endregion Public Methods 

		#endregion Methods 
	}
}
