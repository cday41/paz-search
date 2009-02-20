using System;

namespace PAZ_Dispersal
{
	/// <summary>
	/// Summary description for StraightMover.
	/// </summary>
	public class StraightMover:Mover
	{
		private static StraightMover r;
		private StraightMover(): base()
		{
		}
		private StraightMover(System.Collections.ArrayList inPath,
									double inBaseStepLength,
									double inHeading,
									double inTurnAngleVariability):base (inPath,inBaseStepLength,inHeading,inTurnAngleVariability){}
	      
				
		public static StraightMover getStraightMover()
		{
			if (r==null)
			r = new StraightMover();
			return r;
		}
			public override double getTurnAngle(double rho)
		{
			return 0;
		}//end of getTurnAngle
			public override double getStepLength()
		{
			return this.baseStepLength;
		}//end of getStepLength

	}
}
