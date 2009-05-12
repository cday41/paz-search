using System;

namespace PAZ_Dispersal
{
	/// <summary>
	/// 
	/// </summary>
	public class SleepMover : PAZ_Dispersal.Mover
	{
      private static SleepMover s;
		private SleepMover()
		{
         
		}

      public static SleepMover getSleepMover()
      {
         if (s==null)
            s=new SleepMover();
         return s;

      }
		//Sleeping, so don't move, but set percent time step to 1 to indicate we slept the whole time step
		public override void move(ref double percentTimeStep, Animal inA)
		{
         fw.writeLine("inside move for sleep mover");
			percentTimeStep = 1;
		}
		//this mover doesn't use turn angle, so return 0 to make compiler happy
		public override double getTurnAngle(double rho)
		{
			return 0;
		}//end of getTurnAngle
		//this mover doesn't use step length, so return 0 to make compiler happy
		public override double getStepLength()
		{
			return 0;
		}//end of getStepLength
	}
}
