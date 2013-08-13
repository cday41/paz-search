namespace SEARCH
{
   public abstract class IHomeRangeTrigger
   {
      public abstract int numTimes{get;set;}
      
      public abstract bool timeToLookForHome(Animal inA);
      public abstract void reset(int num);
   }
}