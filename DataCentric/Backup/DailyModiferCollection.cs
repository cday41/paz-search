/******************************************************************************
 *    Changed By:    Bob Cummings
 *    Changed On:    Saturday, February 23, 2008
 *    Description:   Added method public DailyModifier getFirst()
 *                   needed to start the simulation.
 * ***************************************************************************/
using System;
namespace PAZ_Dispersal
{
   
   public sealed class DailyModiferCollection:System.Collections.SortedList
   {
      int currIndex;
      private DateTime mNextStartDate;
      private static DailyModiferCollection uniqueInstance;
      private DailyModiferCollection()
      {
         currIndex = 0;
      }
   
      public void reset()
      {
         currIndex = 0;
      }

      public void advanceOneYear()
      {  
         DailyModifier dm;
         
         for(int i=0;i<this.Count;i++)
         {
            dm = (DailyModifier)this.GetByIndex(i);
            dm.advanceOneYear();
         }
      }

      public static DailyModiferCollection GetUniqueInstance()
      {
         if(uniqueInstance == null) 
         { 
            uniqueInstance = new DailyModiferCollection();
         }
         return uniqueInstance;
      }
      
      public DailyModifier getFirst()
      {
         DailyModifier nextDM;
         if (currIndex == this.Count-1)
         {
            currIndex = 0;
         }
         else
         {
            currIndex++;
         }
         nextDM = (DailyModifier)this.GetByIndex(currIndex);
         this.mNextStartDate = nextDM.StartDate;
         return (DailyModifier)this.GetByIndex(0);
         
      }
      public DailyModifier getNext()
      {
         DailyModifier dm;
         DailyModifier nextDM;
         dm = (DailyModifier)this.GetByIndex(currIndex);
         if (currIndex == this.Count-1)
         {
            currIndex = 0;
         }
         else
         {
            currIndex++;
         }
         nextDM = (DailyModifier)this.GetByIndex(currIndex);
         this.mNextStartDate = nextDM.StartDate;
         return dm;
      }
     public DateTime NextStartDate
		{
			get { return mNextStartDate; }
			set { mNextStartDate = value; }
		}

   }
}
