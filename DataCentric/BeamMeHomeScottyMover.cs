using System;
using ESRI.ArcGIS.Geometry;
namespace PAZ_Dispersal
{
	/// <summary>
	/// Used to move an animal to its home range
	/// </summary>
	public class BeamMeHomeScottyMover : PAZ_Dispersal.Mover
	{
		RandomNumbers rn;
		public BeamMeHomeScottyMover(): base()
		{
			rn = RandomNumbers.getInstance();
		}
		public override void move(ref double percentTimeStep, Animal inA)
		{
         //inA.Location = inA.HomeRangeCenter;

		}
		public override double getTurnAngle(double rho)
		{
			return rn.getWrappedCauchy(rho);
		}//end of getTurnAngle
		public override double getStepLength()
		{
			return this.baseStepLength;
		}//end of getStepLength

	}
}
