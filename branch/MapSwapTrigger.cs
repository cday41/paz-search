/******************************************************************************
 * CHANGE LOG
 * DATE:          Sunday, September 09, 2007 12:06:34 PM
 * Author:        Bob Cummings
 * Description:   Added mOriginalStartDate variable.  Will be needed
 *                When the simulation rolls to another year for hourly and 
 *                seasonal type triggers.
 * 
 *                Added mMyTriggerType variable.  Will be needed to determine 
 *                how to set the next start time.  Problem was hourly time swap
 *                was working great for first day.  But did not advance to next
 *                day.
 *****************************************************************************/              
using System;

namespace PAZ_Dispersal
{
	/// <summary>
	/// Summary description for MapSwapTrigger.
	/// </summary>
	public class MapSwapTrigger:ICloneable
	{
		private int mYearGrp;
		private int mSeasonGrp;
		private int mDayGrp;
		private string mStartMonth;
		private string mStartDay;
		private string mStartHour;
		private string mStartMinute;
		private System.DateTime mStartDate;
      private System.DateTime mOriginalStartDate;
		private string mPath;
		private string mFilename;
      private mTriggerType mMyTriggerType;

      //For some reason the enum has to be made public to access
      //through a property.  But you can not see it from outside the 
      //cless?
      public enum mTriggerType
      {
         STATIC,
         YEARLY,
         DAILY,
         HOURLY
      }


		public MapSwapTrigger()
		{
			//
			// TODO: Add constructor logic here
			//
		}

      public override string ToString()
      {
         return mStartDate.ToShortDateString() + " " + mStartDate.ToShortTimeString() + " " + mFilename + " " + this.mMyTriggerType.ToString();
      }


      public void setTriggerType(int numYears,int numDays, int numHours)
      {
         if(numHours > 1)
            this.mMyTriggerType = mTriggerType.HOURLY;
         else if(numDays > 1)
            this.mMyTriggerType = mTriggerType.DAILY;
         else if (numYears > 1)
            this.mMyTriggerType = mTriggerType.YEARLY;
         else 
            this.mMyTriggerType = mTriggerType.STATIC;
      }

		#region getters and setters	

      public mTriggerType MyTriggerType
		{
			get { return mMyTriggerType; }
			set  { mMyTriggerType = value; }
		}

      

		public int YearGrp
		{
			get { return mYearGrp; }
			set { mYearGrp = value; }
		}

		public int SeasonGrp
		{
			get { return mSeasonGrp; }
			set { mSeasonGrp = value; }
		}

		public int DayGrp
		{
			get { return mDayGrp; }
			set { mDayGrp = value; }
		}

		public string StartMonth
		{
			get { return mStartMonth; }
			set { mStartMonth = value; }
		}

		public string StartDay
		{
			get { return mStartDay; }
			set { mStartDay = value; }
		}

		public string StartHour
		{
			get { return mStartHour; }
			set { mStartHour = value; }
		}

		public string StartMinute
		{
			get { return mStartMinute; }
			set { mStartMinute = value; }
		}

		public System.DateTime StartDate
		{
			get { return mStartDate; }
			set { mStartDate = value; }
		}
       public System.DateTime OriginalStartDate
		{
			get { return mOriginalStartDate; }
			set  { mOriginalStartDate = value; }
		}

        public  string Path
		{
            get { return mPath; }
            set { mPath = value; }
		}
		public string Filename
		{
			get { return mFilename; }
			set { mFilename = value; }
		}

		#endregion


        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

    }
}