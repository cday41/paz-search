using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Geometry;

namespace PAZ_Dispersal
{
   struct MapValue
   {
		#region Data Members (22) 

      private double mCaptureFood;
      private IPoint mCurrLocation;
      private double mEnergyUsed;
      private double mFoodMeanSize;
      private double mFoodSD_Size;
      private double mHeading;
      private double mMoveSpeed;
      private double mMoveTurtosity;
      private double mPerceptionDist;
      private double mPerceptonModifier;
      private double mPredationRisk;

      private int mMoveIndex;    //index of the polygon on the move map

      public int MoveIndex
      {
         get { return mMoveIndex; }
         set { mMoveIndex = value; }
      }
      private int mRiskIndex;       //index of the polygon on the risk index

      public int RiskIndex
      {
         get { return mRiskIndex; }
         set { mRiskIndex = value; }
      }
      private int mFoodIndex;       //index of the polygon on the food index

      public int FoodIndex
      {
         get { return mFoodIndex; }
         set { mFoodIndex = value; }
      }
      private int mSocialIndex;    // index of the polygon on the social index

      public int SocialIndex
      {
         get { return mSocialIndex; }
         set { mSocialIndex = value; }
      }

#endregion
      #region getters and setters

      public double CaptureFood
      {
         get { return mCaptureFood; }
         set { mCaptureFood = value; }
      }

      public IPoint CurrLocation
      {
         get { return mCurrLocation; }
         set { mCurrLocation = value; }
      }

      public double EnergyUsed
      {
         get { return mEnergyUsed; }
         set { mEnergyUsed = value; }
      }

      public double FoodMeanSize
      {
         get { return mFoodMeanSize; }
         set { mFoodMeanSize = value; }
      }

      public double FoodSD_Size
      {
         get { return mFoodSD_Size; }
         set { mFoodSD_Size = value; }
      }

      public double Heading
      {
         get { return mHeading; }
         set { mHeading = value; }
      }

      public double MoveSpeed
      {
         get { return mMoveSpeed; }
         set { mMoveSpeed = value; }
      }

      public double MoveTurtosity
      {
         get { return mMoveTurtosity; }
         set { mMoveTurtosity = value; }
      }

      public double PerceptionDist
      {
         get { return mPerceptionDist; }
         set { mPerceptionDist = value; }
      }

      public double PerceptonModifier
      {
         get { return mPerceptonModifier; }
         set { mPerceptonModifier = value; }
      }

      public double PredationRisk
      {
         get { return mPredationRisk; }
         set { mPredationRisk = value; }
      }

		#endregion Data Members 
   }
}
