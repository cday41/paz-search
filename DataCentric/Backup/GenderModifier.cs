using System.ComponentModel;
namespace PAZ_Dispersal
{
   public class GenderModifiers

   {
       private double mProbCaptureFood;
      private double mProgKilledByPredator;
      private double mMoveSpeed;
      private double mMoveTortusoity;
      private double mEnergyUsedPerTimeStep;
      private double mVisionRange;

      [CategoryAttribute("Gender Modifiers"), 
      DescriptionAttribute("Adjusts the probability of capturing food")]
      public double ProbCaptureFood
		{
			get { return mProbCaptureFood; }
			set { mProbCaptureFood = value; }
		}
      [CategoryAttribute("Gender Modifiers"), 
      DescriptionAttribute("Adjusts the probability of killed by a predator or other risk")]
      public double ProgKilledByPredator
		{
			get { return mProgKilledByPredator; }
			set { mProgKilledByPredator = value; }
		}


      [CategoryAttribute("Gender Modifiers"), 
      DescriptionAttribute("Adjusts the amount of area covered in a single time step")]
      public double MoveSpeed
		{
			get { return mMoveSpeed; }
			set { mMoveSpeed = value; }
		}



      [CategoryAttribute("Gender Modifiers"), 
      DescriptionAttribute("Adjusts the probability of going straight or turning during a time step")]
      public double MoveTortusoity
		{
			get { return mMoveTortusoity; }
			set { mMoveTortusoity = value; }
		}




      [CategoryAttribute("Gender Modifiers"), 
      DescriptionAttribute("Adjusts the amount of energy used during a time step")]
      public double EnergyUsedPerTimeStep
		{
			get { return mEnergyUsedPerTimeStep; }
			set { mEnergyUsedPerTimeStep = value; }
		}


      [CategoryAttribute("Gender Modifiers"), 
      DescriptionAttribute("Adjusts the distance the animal can 'see' as it moves.")]
      public double VisionRange
		{
			get { return mVisionRange; }
			set { mVisionRange = value; }
		}




   }
}
