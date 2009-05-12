using System;
using ESRI.ArcGIS.Geometry;



namespace PAZ_Dispersal

{
   /// <summary>
   /// Summary description for RandomWCMover.
   /// </summary>
   public class RandomWCMover:Mover
   {
      RandomNumbers rn;
      private static RandomWCMover r;
      private RandomWCMover(): base()
      {
         rn = RandomNumbers.getInstance();
      }
      private RandomWCMover(System.Collections.ArrayList inPath,
         double inBaseStepLength,
         double inHeading,
         double inTurnAngleVariability)
         :base (inPath,inBaseStepLength,inHeading,inTurnAngleVariability){}
      
			
      public static RandomWCMover getRandomWCMover()
      {
         if (r==null)
            r = new RandomWCMover();
         return r;


      }
      public override double getTurnAngle(double rho)
      {
         return rn.getWrappedCauchy(rho);
      }//end of getTurnAngle
      public override double getStepLength()
      {
         return this.baseStepLength;
      }//end of getStepLength

   }//end of class
}//end of namespace
