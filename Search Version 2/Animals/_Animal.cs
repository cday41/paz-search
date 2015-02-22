using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Animals
{
	

		
		public partial class Animal
		{
			MoveValues mv = new MoveValues();

			

			public MoveValues Move_Values
			{
				get { return mv; }
				set { mv = value; }
			}
			public void Initialize()
			{
				mv.Angle = 0;
				mv.End = null;
				mv.Heading = 0;
				mv.Id = this.ID;
				mv.StepLength = 1;
				mv.Turt =.78;
				mv.Start = this.CurrLocation;
				mv.PercentTimeStep = 0.0;
			}
		}
	}

