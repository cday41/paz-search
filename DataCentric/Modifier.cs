 #define DEVELOP
using System;
using System.ComponentModel;
namespace PAZ_Dispersal
{
   
   public  class Modifier
   {
      public Modifier()
      {
#if (DEVELOP)
         mCaptureFood = 1;
         mPredationRisk = 1;
         mMoveSpeed = 1;
         mMoveTurtosity = 1;
         mEnergyUsed = 1;
         mPerceptonModifier = 1;

#else
         mCaptureFood = 1;
         mPredationRisk = 1;
         mMoveSpeed = 1;
         mMoveTurtosity = 1;
         mEnergyUsed = 1;
         mPerceptonModifier = 1;

#endif

      }
      private double mCaptureFood;
      private double mPredationRisk;
      private double mMoveSpeed;
      private double mMoveTurtosity;
      private double mEnergyUsed;
      private double mPerceptonModifier;
      private string mName;
      
      #region getters and setters

      public string Name
		{
			get { return mName; }
			set { mName = value; }
		}



      [CategoryAttribute("Basic Modifier Settings"), 
      DescriptionAttribute("Adjusts the probability of successfull foraging for one time step")]
      public double CaptureFood
      {
         get { return mCaptureFood; }
         set { mCaptureFood = value; }
      }
      [CategoryAttribute("Basic Modifier Settings"), 
      DescriptionAttribute("Adjusts the probability of dieing from predation or other risks during one time step")]
      public double PredationRisk
      {
         get { return mPredationRisk; }
         set { mPredationRisk = value; }
      }
      [CategoryAttribute("Basic Modifier Settings"), 
      DescriptionAttribute("Adjusts the amount of area traveled in one time step")]
     
      public double MoveSpeed
      {
         get { return mMoveSpeed; }
         set { mMoveSpeed = value; }
      }
      [CategoryAttribute("Basic Modifier Settings"), 
      DescriptionAttribute("Adjusts the proability of turning/going straight in one time step")]
      public double MoveTurtosity
      {
         get { return mMoveTurtosity; }
         set { mMoveTurtosity = value; }
      }
      [CategoryAttribute("Basic Modifier Settings"), 
      DescriptionAttribute("Adjusts the amount of energy used in one time step")]
    
      public double EnergyUsed
      {
         get { return mEnergyUsed; }
         set { mEnergyUsed = value; }
      }
      [CategoryAttribute("Basic Modifier Settings"), 
      DescriptionAttribute("Adjusts the perception distance for one time step")]
    
      public double PerceptonModifier
      {
         get { return mPerceptonModifier; }
         set { mPerceptonModifier = value; }
      }
      #endregion
   }
}
