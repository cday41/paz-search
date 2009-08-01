using System;

namespace PAZ_Dispersal
{
	/// <summary>
	/// Summary description for DeadDisperser.
	/// </summary>
	public class DeadAnimal : Animal
	{
		public DeadAnimal()
		{
			//
			// TODO: Add constructor logic here
			//
		}
      public override void  doTimeStep(HourlyModifier inHM, DailyModifier inDM,DateTime currTime,bool doTextOutput, ref string status)
      {
         fw.writeLine("dead disperser number " + this.IdNum.ToString() + " is not takeing a timestep");
      }
	}
}
