/*
  Copyright © Iain McDonald 2012
  
  This file is part of FinancialPricer.

	FinancialPricer is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	FinancialPricer is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with FinancialPricer.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace LbF
{
	public class YieldCurve
	{
		public IList<double> ParYields { get; set; }
		public IList<double> SpotYields { get; set; }
		public IList<double> DiscountFactors { get; set; }

		public double this[double i]
		{
			get { return 1 / Math.Pow(1 + InterpolatedSpotYield(i), i); }
		}

		public YieldCurve()
		{
			this.ParYields = new List<double>(new[] { 0.0, 0.06, 0.067, 0.0685 });
		}

		public virtual void BootstrapSpotYields()
		{
			this.SpotYields = new List<double>(this.ParYields.Count);
			this.SpotYields.Insert(0, this.ParYields[0]);
			this.DiscountFactors = new List<double>(this.ParYields.Count);
			this.DiscountFactors.Insert(0, 1 / (1 + this.ParYields[0]));

			var accumulatedSpotYields = 0.0;
			for (int i = 1; i < this.ParYields.Count; ++i)
			{
				if (i > 1)
					accumulatedSpotYields += 1 / Math.Pow(1 + this.SpotYields[i - 1], i - 1);

				var totalRate = (1 + this.ParYields[i]) / (1 - this.ParYields[i] * accumulatedSpotYields);
				this.SpotYields.Insert(i, Math.Pow(totalRate, 1.0 / i) - 1);
				this.DiscountFactors.Insert(i, 1 / totalRate);
			}
		}

		public virtual void BootstrapParYields()
		{
			this.ParYields = new List<double>(this.SpotYields.Count);
			this.ParYields.Insert(0, this.SpotYields[0]);
			this.DiscountFactors = new List<double>(this.SpotYields.Count);
			this.DiscountFactors.Insert(0, 1 / (1 + this.SpotYields[0]));

			var accumulatedParYields = 0.0;
			for (int i = 1; i < this.SpotYields.Count; ++i)
			{
				this.DiscountFactors.Insert(i, 1 / Math.Pow(1 + this.SpotYields[i], i));
				accumulatedParYields += this.DiscountFactors[i];

				this.ParYields.Insert(i, (1 - this.DiscountFactors[i]) / accumulatedParYields);
			}
		}

		private double InterpolatedSpotYield(double maturity)
		{
			var previousMaturity = (int) maturity;
			var nextMaturity = previousMaturity + 1;

			if (previousMaturity == maturity)
				return this.SpotYields[previousMaturity];

			var fromPrevious = maturity - previousMaturity;

			return this.SpotYields[previousMaturity] + fromPrevious * (this.SpotYields[nextMaturity] - this.SpotYields[previousMaturity]);
		}

		public override string ToString()
		{
			var stringBuilder = new StringBuilder();

			stringBuilder.Append("\n##### Yield Curve #####\n\n");
			stringBuilder.Append("  --------------\n");
			for (int i = 1; i < this.SpotYields.Count; ++i)
				stringBuilder.AppendFormat(("  | " + i + "Y Spot Yield:").PadRight(Constants.Spacing) + "{0}\n", this.SpotYields[i]);
			stringBuilder.Append("  | \n");

			for (int i = 1; i < this.ParYields.Count; ++i)
				stringBuilder.AppendFormat(("  | " + i + "Y Par Yield:").PadRight(Constants.Spacing) + "{0}\n", this.ParYields[i]);
			stringBuilder.Append("  | \n");

			for (int i = 1; i < this.DiscountFactors.Count; ++i)
				stringBuilder.AppendFormat(("  | " + i + "Y Discount Factor:").PadRight(Constants.Spacing) +
					"{0}\n", this.DiscountFactors[i]);
			stringBuilder.Append("  --------------\n");

			return stringBuilder.ToString();
		}
	}

	public class YieldCurveCreditSpread : YieldCurve
	{
		public IList<double> CreditSpread { get; set; }

		public override void BootstrapSpotYields()
		{
			for (int i = 1; i < this.ParYields.Count; ++i)
				this.ParYields[i] += this.CreditSpread[i];

			base.BootstrapSpotYields();
		}

		public override void BootstrapParYields()
		{
			for (int i = 1; i < this.SpotYields.Count; ++i)
				this.SpotYields[i] += this.CreditSpread[i];

			base.BootstrapParYields();
		}
	}

	public class YieldCurveZSpread : YieldCurve
	{
		public IList<double> ZSpread { get; set; }

		public override void BootstrapSpotYields()
		{
			base.BootstrapSpotYields();
			ApplyZSpread();
		}

		public override void BootstrapParYields()
		{
			base.BootstrapParYields();
			ApplyZSpread();
		}

		private void ApplyZSpread()
		{
			for (int i = 1; i < this.SpotYields.Count; ++i)
				this.SpotYields[i] += this.ZSpread[i];
			for (int i = 1; i < this.ParYields.Count; ++i)
				this.ParYields[i] += this.ZSpread[i];

			this.DiscountFactors = new List<double>(this.SpotYields.Count);
			for (int i = 0; i < this.SpotYields.Count; ++i)
				this.DiscountFactors.Insert(i, 1 / Math.Pow(1 + this.SpotYields[i], i));
		}
	}
}
