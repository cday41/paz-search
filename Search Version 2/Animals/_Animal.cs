using DataHelper;
using Utility;

namespace Animals
{
	public partial class Animal
	{
		#region fields

		private MoveValues mv = new MoveValues();

		#endregion fields

		#region Public Methods


		public void UpdateModifiers()
		{
			DbHelper db = new DbHelper();
			GetMoveModifiers(db);
			GetFoodModifiers(db);

			// it is a one liner so leave it here
			var site = db.GetRiskSite(this.CurrLocation) as base_risk;
			this.mv.Risk = site.RISK;
		}
		#endregion Public Methods


		#region Private Methods
		private void GetFoodModifiers(DbHelper db)
		{
			var foodSite = db.GetFoodSite(this.CurrLocation) as base_food;
			this.mv.ChanceToEat = foodSite.PROBCAP;
			this.mv.MeanChanceToEat = foodSite.X_SIZE;
			this.mv.StdDeviationToEat = foodSite.SD_SIZE;
		}

		private void GetMoveModifiers(DbHelper db)
		{
			var site = db.GetMoveSite(this.CurrLocation);
			this.mv.EnergyUsed = site.ENERGYUSED;
			this.mv.StepLength = site.MSL;
			this.mv.Turt = site.MVL;
			this.mv.PerceptionModifier = site.PR_X;
		}

		#endregion
		#region Properties

		public MoveValues Move_Values
		{
			get { return mv; }
			set { mv = value; }
		}

		#endregion Properties
	}
}