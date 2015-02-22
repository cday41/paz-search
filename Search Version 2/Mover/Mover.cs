using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Spatial;
using Utility;
using MathNet.Numerics.Differentiation;

namespace Mover
{
	 public class Mover
	 {
		 ILog mLog = LogManager.GetLogger("Mover");

		 public virtual void move(MoveValues inMoveValues)
		 {
			 DbGeometry there = null;
			 DbGeometry here = null;
			 
			
			// crossOverInfo co;
			 try
			 {
				 mLog.Debug(" inside move method with animal passed in " + inMoveValues.Id.ToString());
				 //get specs  
				 //get terrain modifiers from polygon
				 double stepLen = inMoveValues.StepLength * (1 - inMoveValues.PercentTimeStep);//determine step length
				 double turt = inMoveValues.Turt;//determine turtosity
				 inMoveValues.Angle = this.GetTurnAngle(turt);//determine turn angle

				 mLog.Debug("step length is " + stepLen.ToString());
				 mLog.Debug("turt is " + turt.ToString());
				 mLog.Debug("angle is " + inMoveValues.Angle.ToString());
				 mLog.Debug("Heading is " + inMoveValues.Heading.ToString());
				 mLog.Debug("start location is X = " + inMoveValues.StartValues());
				 mLog.Debug("now calling step");
				 here = inMoveValues.Start;//get current location
				 if (inMoveValues.PercentTimeStep > 0)
				 {
					 inMoveValues.Angle = 0; //keep originMoveValuesl heading if partial time step
				 }

				 //move
				 step( inMoveValues);//get new location
				 mLog.Debug("after step heading is " + inMoveValues.Heading.ToString());

				 inMoveValues.Heading = (inMoveValues.Heading + inMoveValues.Angle);//set new heading
				 mLog.Debug("new heading is " + inMoveValues.Heading.ToString());
			//	 mLog.Debug("the current location is x = " + inMoveValues.Location.X.ToString() + " y = " + inMoveValues.Location.Y.ToString());
				 mLog.Debug("the new  location is  " + inMoveValues.EndValues());
				 mLog.Debug("now make sure the end point is on the map");

				 #region Comments
				 //if (myMapManager.isPointOnMoveMap(there))
				 //{
				 //	#region
				 //	co = this.myMapManager.getCrossOverInfo(here, there);//get crossover info

				 //	mLog.Debug("cross over info");
				 //	mLog.Debug("did we cross over any polyons = " + co.ChangedPolys.ToString());
				 //	mLog.Debug("CurrPolyValue = " + co.CurrPolyValue.ToString());
				 //	mLog.Debug("NewPolyValue = " + co.NewPolyValue.ToString());
				 //	mLog.Debug("Distance = " + co.Distance.ToString());
				 //	mLog.Debug(" X = " + co.Point.X.ToString() + " Y = " + co.Point.Y.ToString());
				 //	mLog.Debug("George is on the map = " + co.OnTheMap.ToString());

				 //	//check if this pushed us into a new polygon
				 //	//Wednesday, August 23, 2006 changed from length = length OR polyValue = polyValue
				 //	//length = length AND polyValue = polyValue
				 //	if (co.ChangedPolys == false)
				 //	//went the full distance stayed inside the same polygon the whole time
				 //	{
				 #endregion
				
				 inMoveValues.PercentTimeStep = 1;
				 #region Commented Out
				 //	}
				 //	else //new poly, move to border,decide if you want to go or not, set percentTimeStep
				 //	{
				 //		double likeHere = 0.0;
				 //		double likeThere = 0.0;
				 //		double distToBorder = 0.0;
				 //		double crossoverRatio = 0.0;
				 //		PointClass borderCrossing = null;


				 //		mLog.Debug("different polygon so now lets get to it");
				 //		mLog.Debug("calling mapmanager get cross over inFo");


				 //		//move to border
				 //		borderCrossing = co.Point;//get point on border
				 //		inMoveValues.Location = borderCrossing;//move to border

				 //		//decide if you want to go or not
				 //		likeHere = co.CurrPolyValue;//get the field that represents an animal's affinity for the current polygon
				 //		likeThere = co.NewPolyValue;//get the field that represents an animal's affinity for the new polygon
				 //		crossoverRatio = likeThere / likeHere; //calculate the ratio
				 //		//compare the ratio to a uniform random [0-1]
				 //		RandomNumbers rn = RandomNumbers.getInstance();

				 //		if (crossoverRatio > rn.getUniformRandomNum())//crossover
				 //		{
				 //			mLog.Debug("I am going to cross over");
				 //			//change poly to new poly
				 //			//Wednesday, July 02, 2008
				 //			//inMoveValues.MoveIndex = this.myMapManager.getCurrMovePolygon(co.NewPolyPoint);
				 //			//if we are going to cross over then give him a nudge into the new polygon
				 //			inMoveValues.Location.PutCoords(co.NewPolyPoint.X, co.NewPolyPoint.Y);
				 //		}
				 //		else //go back
				 //		{
				 //			mLog.Debug("I am not going to cross over");
				 //			inMoveValues.Heading = inMoveValues.Heading + Math.PI; //turn around 
				 //			//Now move back into the orginMoveValuesl polygon
				 //			Mover.stepForward(ref inMoveValues);
				 //		}

				 //		//set percentTimeStep
				 //		distToBorder = co.Distance;//get dist from start to border crossing
				 //		percentTimeStep += (1 - percentTimeStep) * (distToBorder / stepLen);//Update percentTimeStep
				 //		percentTimeStep = Math.Round(percentTimeStep, 2);//round to avoid taking steps < .01 time step
				 //	}
				 //}
				 //	#endregion
				 //else
				 //{
				 //	//Wandered off the rest of the world
				 //	inMoveValues.IsDead = true;
				 //	inMoveValues.TextFileWriter.addLine("Animal has left the map");
				 //	percentTimeStep = 1;
				 //	return;
				 //}
				 //mLog.Debug("percent time step is " + percentTimeStep.ToString());
				 //mLog.Debug("now the heading value is " + inMoveValues.Heading.ToString());
				 //inMoveValues.updateMemory();
				 #endregion
			 }//end of try
			 catch (System.Exception ex)
			 {
				
			 }//end of catch
		 }//end of move

		 public double GetTurnAngle(double variance)
		 {
			
			 System.Random rand = new Random();
			 //convert to radian [0,2pi)
			 double angleUniform = (rand.NextDouble() * 2 * System.Math.PI);
			 //convert to Wrapped Cauchy [-pi,pi]
			 //per SODA source angleDelta = 2 * atan((1-rho)/((1+rho)*tan(rndAngle * deg2rad)))
			 double angleWC = 2 * System.Math.Atan((1 - variance) / ((1 + variance) * System.Math.Tan(angleUniform)));
			 //System.Console.WriteLine("New angle: " + angleWC);
			 return angleWC;
		 }

		 private void step(MoveValues inOutMV)
		 {
			 Double? X = inOutMV.Start.StartPoint.XCoordinate.Value;
			 Double? Y = inOutMV.Start.StartPoint.YCoordinate.Value;			
			 mLog.Debug("inside step with the following values");
			 mLog.Debug("stepLength is " + inOutMV.StepLength.ToString());
			 mLog.Debug("turnAngle is " + inOutMV.Angle.ToString());
			 mLog.Debug("heading is " + inOutMV.Heading.ToString());
			 mLog.Debug("start x is " +  X.ToString()+ " start y is " +  Y.ToString());

			 try
			 {
				 double? newX = X + inOutMV.StepLength * System.Math.Cos(inOutMV.Heading + inOutMV.Angle);
				 double? newY = Y + inOutMV.StepLength * System.Math.Sin(inOutMV.Heading + inOutMV.Angle);
				 DbGeometry EndPoint = DbGeometry.FromText("POINT(" + newX.ToString() + " " + newY.ToString() + ")", 0);
				 inOutMV.End = EndPoint;
				

			 }
			 catch (System.Exception ex)
			 {
				 mLog.Debug(ex);
			 }
	
		 }//end of step
	 }
}
