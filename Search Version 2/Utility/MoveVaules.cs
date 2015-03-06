using System;
using System.Data.Entity.Spatial;

namespace Utility
{
	public class MoveValues
	{
		#region PrivateMembers

		private Double angle;
		private Double? chanceToEat;
		private DbGeometry end;
		private Double? energyUsed;
		private Double heading;
		private int id;
		private double location;
		private Double? meanChanceToEat;
		private Double percentTimeStep;
		private Double? perceptionModifier;
		private Double? risk;

		private Double? stdDeviationToEat;
		private DbGeometry start;
		private Double? stepLength;
		private int timeStep;
		private Double? turt;

		#endregion PrivateMembers

		#region Properties

		public Double Angle
		{
			get { return angle; }
			set { angle = value; }
		}

		public Double? ChanceToEat
		{
			get { return chanceToEat; }
			set { chanceToEat = value; }
		}

		public DbGeometry End
		{
			get { return end; }
			set { end = value; }
		}

		public Double? EnergyUsed
		{
			get { return energyUsed; }
			set { energyUsed = value; }
		}

		public Double Heading
		{
			get { return heading; }
			set { heading = value; }
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public double Location
		{
			get { return location; }
			set { location = value; }
		}

		public Double? MeanChanceToEat
		{
			get { return meanChanceToEat; }
			set { meanChanceToEat = value; }
		}

		public Double PercentTimeStep
		{
			get { return percentTimeStep; }
			set { percentTimeStep = value; }
		}

		public Double? PerceptionModifier
		{
			get { return perceptionModifier; }
			set { perceptionModifier = value; }
		}

		public Double? Risk
		{
			get { return risk; }
			set { risk = value; }
		}

		public DbGeometry Start
		{
			get { return start; }
			set { start = value; }
		}

		public Double? StdDeviationToEat
		{
			get { return stdDeviationToEat; }
			set { stdDeviationToEat = value; }
		}

		public Double? StepLength
		{
			get { return stepLength; }
			set { stepLength = value; }
		}

		public int TimeStep
		{
			get { return timeStep; }
			set { timeStep = value; }
		}

		public Double? Turt
		{
			get { return turt; }
			set { turt = value; }
		}

		#endregion Properties

		public string EndValues()
		{
			return "End X " + End.XCoordinate.Value.ToString() + " Y " + End.YCoordinate.Value.ToString();
		}

		public string StartValues()
		{
			return "End X " + Start.XCoordinate.Value.ToString() + " Y " + Start.YCoordinate.Value.ToString();
		}
	}
}