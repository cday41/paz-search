using System;
using System.Data.Entity.Spatial;

namespace Utility
{
	public class MoveValues
	{
		#region PrivateMembers

		private Double angle;
		private DbGeometry end;
		private Double heading;
		private int id;
		private double location;
		private Double percentTimeStep;
		private DbGeometry start;
		private Double stepLength;
		private Double turt;
		private int timeStep;

		public int TimeStep
		{
			get { return timeStep; }
			set { timeStep = value; }
		}

		#endregion PrivateMembers

		#region Properties

		public Double Angle
		{
			get { return angle; }
			set { angle = value; }
		}

		public DbGeometry End
		{
			get { return end; }
			set { end = value; }
		}
		public Double Heading
		{
			get { return heading; }
			set { heading = value; }
		}
		public double Location
		{
			get { return location; }
			set { location = value; }
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public Double PercentTimeStep
		{
			get { return percentTimeStep; }
			set { percentTimeStep = value; }
		}

		public DbGeometry Start
		{
			get { return start; }
			set { start = value; }
		}

		public Double StepLength
		{
			get { return stepLength; }
			set { stepLength = value; }
		}

		public Double Turt
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