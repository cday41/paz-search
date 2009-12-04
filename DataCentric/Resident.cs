using System;
using DesignByContract;

namespace SEARCH
{
   /// <summary>
   /// Summary description for Resident.
   /// </summary>
   public class Resident : Animal
   {
		#region Constructors (1) 

      public Resident()
      {
         mMyAttributes = null;
         this.IdNum = -1;
      }

		#endregion Constructors 

		#region Fields (2) 

      private ResidentAttributes mMyAttributes;
      

		#endregion Fields 

		#region Properties (2) 

     

      public ResidentAttributes MyAttributes
      {
         get { return mMyAttributes; }
         set 
         {
            mMyAttributes = value;
            fw.writeLine("inside resident having attributes set"); }
      }

		#endregion Properties 

		#region Methods (4) 

		#region Public Methods (3) 

      public void breed(out int numMales, out int numFemales)
      {
         int numChildren = 0;
         numMales = 0;
         numFemales = 0;
         //TODO REMOVE VARIABLE USED ONLY FOR LOGGING
         double rollDice;
         try
         {
            fw.writeLine("inside breed for resident " + this.IdNum);
            //roll the dice to see if we breed or not.
            rollDice = rn.getUniformRandomNum();
            fw.writeLine("rolling the dice returned " + rollDice.ToString());
            fw.writeLine("percent breed is " + this.mMyAttributes.PercentBreed.ToString());
            if (rollDice <= this.mMyAttributes.PercentBreed)
            {
               //pretend we win now roll to see how many
               //TODO figure out how to get correct amount of kids
               fw.writeLine("my mean is " + this.mMyAttributes.NumChildernMean.ToString());
               fw.writeLine("my SD is " + this.MyAttributes.NumChildernSD.ToString());
               numChildren = rn.getNormalRandomInt(this.mMyAttributes.NumChildernMean, this.mMyAttributes.NumChildernSD);
               fw.writeLine("we are going to have a litter with " + numChildren.ToString());
               fw.writeLine("percent chance of having a female is " + this.mMyAttributes.PercentFemale.ToString());
               for (int i = 0; i < numChildren; i++)
               {
                  rollDice = rn.getUniformRandomNum();
                  fw.writeLine("rolling the dice returned " + rollDice.ToString());
                  if (rollDice <= this.mMyAttributes.PercentFemale)
                     numFemales++;
                  else
                     numMales++;
               }
               fw.writeLine("we have " + numMales + " males");
               fw.writeLine("we have " + numFemales + " females");
            }
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
         
         
      }

      public override void doTimeStep(HourlyModifier inHM, DailyModifier inDM, DateTime currTime, bool doTextOutput, ref string status)
      {
         Check.Require(mMyAttributes != null, "Resident Attributes have not been set");
         Check.Require(this.IdNum >= 0, "Resident ID was not set");
         fw.writeLine("inside time step for resident number " + this.IdNum.ToString());
         die(ref status, currTime);
      }

      public void winterKill(int currentTime)
      {
         try
         {
            fw.writeLine("inside winter kill for resident number " + this.IdNum.ToString());
            double rollOfTheDice = 0.0;
            rollOfTheDice = rn.getUniformRandomNum();
            fw.writeLine("inside winter kill with roll of " + rollOfTheDice.ToString());
            fw.writeLine("my chance of winter kill is " + this.MyAttributes.ResidentYearlyRisk.ToString());
            if (mMyAttributes.ResidentYearlyRisk > rollOfTheDice)
            {
               fw.writeLine("did not make it through the winter so setting mDead to true");
               this.mTextFileWriter.addLine("died as winter kill in " + currentTime.ToString());
               this.mIsDead = true;
            }
         }
         catch (System.Exception ex)
         {
#if (DEBUG)
            System.Windows.Forms.MessageBox.Show(ex.Message);
#endif
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

		#endregion Public Methods 
		#region Private Methods (1) 

      private void die(ref string status, DateTime currentTime)
      {
         try
         {
            double rollOfTheDice = 0.0;
            rollOfTheDice = rn.getUniformRandomNum();
            fw.writeLine("inside resident " + this.IdNum.ToString() + " time step die with roll of " + rollOfTheDice.ToString());
            fw.writeLine("my chance of dieing during a time is " + this.MyAttributes.ResidentTimeStepRisk.ToString());
            if (mMyAttributes.ResidentTimeStepRisk > rollOfTheDice)
            {
               status = "dead FROM ROLL OF DICE";
               fw.writeLine("george dies");
               this.mTextFileWriter.addLine("died durning timestep at " + currentTime.ToString());
               this.mIsDead = true;
            }
         }
         catch (System.Exception ex)
         {
            FileWriter.FileWriter.WriteErrorFile(ex);
         }
      }

		#endregion Private Methods 

		#endregion Methods 
   }
}
