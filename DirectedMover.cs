using System;

namespace PAZ_Dispersal
{
	/// <summary>
	/// 
	/// </summary>
	public class DirectedMover : PAZ_Dispersal.Mover
	{
		RandomWCMover myMover;
		public DirectedMover()
		{
			myMover = RandomWCMover.getRandomWCMover();
		}
		public override void move(ref double percentTimeStep, Animal inA)
		{
			inA.Heading = System.Math.Atan((inA.Location.Y - inA.HomeRangeCenter.Y)/(inA.Location.X-inA.HomeRangeCenter.X));
         if (inA.Location.X > inA.HomeRangeCenter.X)
            inA.Heading = inA.Heading + Math.PI;
			myMover.move (ref percentTimeStep, inA);
			
		}
		public override double getStepLength()
		{
			return myMover.getStepLength();
		}

		public override double getTurnAngle(double turtosity)
		{
			return myMover.getTurnAngle(turtosity);
		}


	}
}
