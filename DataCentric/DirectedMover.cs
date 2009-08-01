using System;

namespace PAZ_Dispersal
{
	/// <summary>
	/// 
	/// </summary>
	public class DirectedMover : PAZ_Dispersal.Mover
	{
		#region Public Members (4) 

		#region Constructors (1) 

		public DirectedMover()
		{
			myMover = RandomWCMover.getRandomWCMover();
		}

		#endregion Constructors 
		#region Methods (3) 

		public override double getStepLength()
		{
			return myMover.getStepLength();
		}

		public override double getTurnAngle(double turtosity)
		{
			return myMover.getTurnAngle(turtosity);
		}

		public override void move(ref double percentTimeStep, Animal inA)
		{
			inA.Heading = System.Math.Atan((inA.Location.Y - inA.HomeRangeCenter.Y)/(inA.Location.X-inA.HomeRangeCenter.X));
         if (inA.Location.X > inA.HomeRangeCenter.X)
            inA.Heading = inA.Heading + Math.PI;
			myMover.move (ref percentTimeStep, inA);
			
		}

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (1) 

		#region Fields (1) 

		RandomWCMover myMover;

		#endregion Fields 

		#endregion Non-Public Members 
	}
}
