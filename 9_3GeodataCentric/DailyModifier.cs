using System;
using System.ComponentModel;
namespace PAZ_Dispersal
{
   /// <summary>
   /// A modifier that hangs out for a certain number of days
   /// </summary>
   public class DailyModifier : Modifier
   {
      private DateTime mStartDate;
      [CategoryAttribute("Daily Modifer Start Setting"), 
      DescriptionAttribute("Sets the day of the year this modifier should take effect")]
      public DateTime StartDate
		{
			get { return mStartDate; }
			set { mStartDate = value; }
		}

      public void advanceOneYear()
      {  
         mStartDate = mStartDate.AddYears(1);

      }


   }
}
