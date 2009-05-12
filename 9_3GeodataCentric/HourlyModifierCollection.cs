using System;

namespace PAZ_Dispersal
{
   
   public sealed class HourlyModifierCollection : System.Collections.SortedList
   {
      private int currIndex;
      private int mNextStartHour;
      private static HourlyModifierCollection uniqueInstance;

      private HourlyModifierCollection()
      {
         currIndex = 0;
         
      }
      
      public static HourlyModifierCollection GetUniqueInstance()
      {
         if (uniqueInstance == null) 
         { 
            uniqueInstance = new HourlyModifierCollection();
         }
         return uniqueInstance;
      }
      
      public void reset()
      {
         this.currIndex = 0;
      }
      public HourlyModifier getNext()
      {
         HourlyModifier hm;
         HourlyModifier nextHM;
         hm = (HourlyModifier)this.GetByIndex(currIndex);
         if (currIndex == this.Count - 1)
         {
            currIndex = 0;
         }
         else
         {
            currIndex++;
         }
         //since we moved the modifer up we need to move up the next start hour
         nextHM = (HourlyModifier)this.GetByIndex(currIndex);
         mNextStartHour = nextHM.StartTime;
         return hm;
      }
      public int NextStartHour
      {
         get 
         {
            HourlyModifier hm;
            hm = (HourlyModifier)this.GetByIndex(currIndex);
            mNextStartHour = hm.StartTime;
            return mNextStartHour; }

         set { mNextStartHour = value; }
      }

   }
}
