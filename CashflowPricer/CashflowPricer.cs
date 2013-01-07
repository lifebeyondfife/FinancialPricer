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

namespace LbF
{
	[FinancialInstrument(Name = "Cashflow")]
	public class CashflowPricer : Instrument
	{
		[FinancialInput]
		public int Years { get; set; }
		[FinancialInput]
		public int Period { get; set; }
		[FinancialInput]
		public double Coupon { get; set; }
		[FinancialOutput]
		public double FV { get; private set; }

		public CashflowPricer()
		{
			this.Settlement = DateTime.Parse("1 January 2000");
			this.Years = 3;
			this.Period = 1;
			this.Coupon = 0.4021148036253774;
			this.Rate = 0.0;
			this.PV = 100.0;
			this.Principal = 100;
		}

		protected CashflowPricer(int years, int period, double coupon, double rate, uint principal, double pv)
		{
			this.Years = years;
			this.Period = period;
			this.Coupon = coupon;
			this.Rate = rate;
			this.Principal = principal;
			this.PV = pv;
		}

		public CashflowPricer(int years, int period, double coupon, double rate, uint principal)
			: this(years, period, coupon, rate, principal, 0.0)
		{
		}

		public CashflowPricer(int years, int period, double coupon, uint principal, double pv)
			: this(years, period, coupon, 0.0, principal, pv)
		{
		}

		public override void Calculate()
		{
			if (this.Rate == 0.0)
				SolveInternalRateOfReturn();
			else
				CalculatePV();

			this.FV = this.Years * this.Coupon * this.Principal;
		}

		public override void CalculatePV()
		{
			var d1 = 1.0 / (1 + this.Rate / this.Period);
			var dT_1 = 1.0 / Math.Pow(1 + this.Rate / this.Period, this.Years * this.Period + 1);

			this.PV = this.Principal * (this.Coupon / this.Period) * (d1 - dT_1) / (1 - d1);
		}

		public void SolveInternalRateOfReturn()
		{
			Func<double, double> cashflowPrice = (calcRate =>
			{
				var pv = 0.0;
				for (int i = 1; i <= this.Years * this.Period; ++i)
				{
					pv += this.Principal * (this.Coupon / this.Period) * Math.Pow(1 + calcRate / this.Period, -i);
				}
				return pv - this.PV;
			});

			//	Assume yield is between 0% and 50%
			this.Rate = Mathematics.FindRoot(cashflowPrice, 0.0, 0.5);
		}
	}
}
