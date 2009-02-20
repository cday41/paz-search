
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
