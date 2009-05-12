using System;

using System.ComponentModel;
namespace PAZ_Dispersal
{
	/// <summary>
	/// Summary description for HourlyModifier.
	/// </summary>
	public class HourlyModifier:Modifier
	{
      private int mStartTime;

      [CategoryAttribute("Hourly Modifier Settings"), 
      DescriptionAttribute("Sets the time this set of modifers will start each day")]
    
      public int StartTime
		{
			get { return mStartTime; }
			set { mStartTime = value; }
               
		}

		public HourlyModifier()
		{
			
		}
	}
}
