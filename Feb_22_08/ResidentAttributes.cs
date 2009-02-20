using System;

namespace PAZ_Dispersal
{
   /// <summary>
   /// Summary description for ResidentAttributes.
   /// </summary>
   public class ResidentAttributes
   {
      private double mResidentTimeStepRisk;
      private double mResidentYearlyRisk;
      private double mPercentBreed;
      private int mNumChildernMean;
      private int mNumChildernSD;
      
      private double mPercentFemale;

      public ResidentAttributes()
      {
         mResidentTimeStepRisk=0;
         mResidentYearlyRisk=0;
         mPercentBreed=0;
         mPercentFemale=0;
      }
      public ResidentAttributes(double inTimeRisk,
         double inYearRisk,
         double inPercentBreed,
         double inPercentFemale,
         int inMeanLitterSize,
         int inSDLitterSize)
      {
         mResidentTimeStepRisk=inTimeRisk;
         mResidentYearlyRisk=inYearRisk;
         mPercentBreed=inPercentBreed;
         mPercentFemale=inPercentFemale;
         mNumChildernMean = inMeanLitterSize;
         mNumChildernSD=mNumChildernSD;
      }
      #region gettersAndSetters

       public double PercentBreed
		{
			get { return mPercentBreed; }
			set  { mPercentBreed = value; }
		}


      public double PercentFemale
		{
			get { return mPercentFemale; }
			set  { mPercentFemale = value; }
		}



      public int NumChildernMean
      {
         get { return mNumChildernMean; }
         set { mNumChildernMean = value; }
      }

      public int NumChildernSD
      {
         get { return mNumChildernSD; }
         set { mNumChildernSD = value; }
      }

      public double ResidentTimeStepRisk
      {
         get { return mResidentTimeStepRisk; }
         set  { mResidentTimeStepRisk = value;} 
         
      }

      public double ResidentYearlyRisk
      {
         get { return mResidentYearlyRisk; }
         set  { mResidentYearlyRisk = value; }
      }
      #endregion

   }
}
