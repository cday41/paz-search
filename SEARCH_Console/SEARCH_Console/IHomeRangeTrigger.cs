namespace SEARCH_Console
{
   public interface  IHomeRangeTrigger
   {
      int numTimes{get;set;}
      
      bool timeToLookForHome(Animal inA);
      void reset(int num);
   }
}