using System;

namespace PAZ_Dispersal
{
   /// <summary>
   /// Summary description for IHomeRangeCriteria.
   /// </summary>
   public interface IHomeRangeCriteria
   {
      double Area
      {
         get;
         set;
      }
      double DistanceMean
      {
         get;
         set;
      }
      double DistanceSD
      {
         get;
         set;
      }
  

      double DistanceWeight
      {
         get;
         set;
      }
  	
   }
}
