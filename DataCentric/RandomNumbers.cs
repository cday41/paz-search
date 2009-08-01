using MathNet.Numerics.Generators;
using System;
namespace PAZ_Dispersal
{
   public class RandomNumbers
   {
		#region Public Members (11) 

		#region Fields (2) 

//end of method
      public int myCurNum;
      public int myNumbers;

		#endregion Fields 
		#region Properties (2) 

//end of getWrappedCauchy
      public double Mean
		{
			get { return mMean; }
			set { mMean = value; }
		}

      public double SD
		{
			get { return mSD; }
			set { mSD = value; }
		}

		#endregion Properties 
		#region Methods (7) 

	   public static RandomNumbers getInstance()
	   {
		   if (theInstance == null)
		   {
			   theInstance = new RandomNumbers();
		   }
		   return theInstance;
	   }

	   /// <summary>
	   /// 
	   /// </summary>
	   /// <param name="inMean"></param>
	   /// <param name="inSD"></param>
	   /// <returns>A double from a normal distribution with specified mean and standard deviation</returns>
      public double getNormalRandomNum(double inMean, double inSD)
      {
         
		  numGen.Mean = inMean;
		  numGen.Sigma = inSD;
         return numGen.Next();
      }

      public int getPositiveNormalRandomInt(double inMean, double inSD)
      {
         numGen.Mean = inMean;
         numGen.Sigma = inSD;
         return System.Convert.ToInt32(Math.Abs(numGen.Next()));
      }

	   public double getPositiveNormalRandomNum(double inMean, double inSD)
	   {
		   mMean = inMean;
		   mSD = inSD;
		   numGen.Mean = inMean;
		   numGen.Sigma = inSD;
		   return Math.Abs(numGen.Next());
	   }

      public int getRandomInt(int topValue)
      {
         return rand.Next(topValue);
      }

	   public double getUniformRandomNum()
      {
         return rand.NextDouble();
      }

	   public double getWrappedCauchy(double variance)
	   {
		   //convert to radian [0,2pi)
		   double angleUniform = (rand.NextDouble() * 2 * System.Math.PI);
		   //convert to Wrapped Cauchy [-pi,pi]
			//per SODA source angleDelta = 2 * atan((1-rho)/((1+rho)*tan(rndAngle * deg2rad)))
		   double angleWC = 2 * System.Math.Atan((1 - variance) / ((1 + variance) * System.Math.Tan(angleUniform)));
		   //System.Console.WriteLine("New angle: " + angleWC);
		   return angleWC;
	   }

		#endregion Methods 

		#endregion Public Members 

		#region Non-Public Members (6) 

		#region Fields (5) 

      private double mMean;
      private double mSD;
      private static MathNet.Numerics.Generators.NormalGenerator numGen;
	   private static System.Random rand;
	   private static RandomNumbers theInstance;

		#endregion Fields 
		#region Constructors (1) 

	   private RandomNumbers()
	   {
              
         
		   rand = new System.Random();
         numGen = new MathNet.Numerics.Generators.NormalGenerator();
         mMean = 0;
         mSD = 0;
         
	   }

		#endregion Constructors 

		#endregion Non-Public Members 
   }
}
