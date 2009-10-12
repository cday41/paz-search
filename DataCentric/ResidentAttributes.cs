using System;

namespace SEARCH
{
   /// <summary>
   /// Summary description for ResidentAttributes.
   /// </summary>
   public class ResidentAttributes
   {
		#region Constructors (2) 

      public ResidentAttributes(double inTimeRisk,
         double inYearRisk,
         double inPercentBreed,
         double inPercentFemale,
         double inMeanLitterSize,
         double inSDLitterSize,
         string outPath)
      {
         mResidentTimeStepRisk=inTimeRisk;
         mResidentYearlyRisk=inYearRisk;
         mPercentBreed=inPercentBreed;
         mPercentFemale=inPercentFemale;
         mNumChildernMean = inMeanLitterSize;
         mNumChildernSD=inSDLitterSize;
         out_Path = outPath;
      }

      public ResidentAttributes()
      {
         mResidentTimeStepRisk=0;
         mResidentYearlyRisk=0;
         mPercentBreed=0;
         mPercentFemale=0;
      }

		#endregion Constructors 

		#region Fields (8) 

      private double mNumChildernMean;
      private double mNumChildernSD;
      private string mOrginalID;
      private double mPercentBreed;
      private double mPercentFemale;
      private double mResidentTimeStepRisk;
      private double mResidentYearlyRisk;
      string out_Path;

		#endregion Fields 

		#region Properties (8) 

      public string Out_Path
      {
         get { return out_Path; }
         set { out_Path = value; }
      }

      public string OriginalID
      {
         get {return mOrginalID;}
         set {mOrginalID = value;}
      }

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

      public double NumChildernMean
      {
         get { return mNumChildernMean; }
         set { mNumChildernMean = value; }
      }

      public double NumChildernSD
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

		#endregion Properties 
   }
}
