using System.Collections.Generic;
namespace SEARCH
{
   public interface  IHomeRangeTrigger
   {
      bool timeToLookForHome(Animal inA);
      void reset(List<Animal> inList);
      void addNewDispersers(List<Animal> inList);
     
   }
}