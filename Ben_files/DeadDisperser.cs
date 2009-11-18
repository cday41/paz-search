using System;

namespace PAZ_Dispersal
{
	/// <summary>
	/// Summary description for DeadDisperser.
	/// </summary>
	public class DeadAnimal : Animal
	{
		#region Public Members (2) 

		#region Constructors (1) 

		public DeadAnimal()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#endregion Constructors 
		#region Methods (1) 

      public override void  doTimeStep(HourlyModifier inHM, DailyModifier inDM,DateTime currTime,bool doTextOutput, ref string status)
      {
         fw.writeLine("dead disperser number " + this.IdNum.ToString() + " is not takeing a timestep");
      }

		#endregion Methods 

		#endregion Public Members 
	}
}
