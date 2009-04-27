using System;

namespace PAZ_Dispersal
{
	public class Point
	{
		private double xCoordinate;
		private double yCoordinate;
		private double angle; //in radians
		private double radius;

		public double getX(){return xCoordinate;}
		public double getY(){return yCoordinate;}
		public double getAngle(){return angle;}
		public double getRadius(){return radius;}

		public Point (double x, double y)
		{
			xCoordinate = x;
			yCoordinate = y;
			radius = Math.Sqrt(x * x + y * y);
			angle = Math.Atan2(y,x);
		}//end of constructor

	}//end of class
}//end of namespace
