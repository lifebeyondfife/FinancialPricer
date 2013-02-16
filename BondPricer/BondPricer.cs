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
	[FinancialInstrument(Name = "Bond")]
	public class BondPricer : Instrument
	{
		[FinancialInput]
		public DayCount DayCountBasis { get; set; }
		[FinancialInput]
		public int Period { get; set; }
		[FinancialOutput]
		public double Clean { get; set; }
		[FinancialOutput]
		public double Accrued { get; private set; }
		[FinancialOutput]
		public double YieldToMaturity { get; set; }

		[FinancialInput]
		public DateTime HorizonDate { get; set; }
		[FinancialInput]
		public double HorizonPrice { get; set; }
		[FinancialInput]
		public double HorizonReinvestment { get; set; }
		[FinancialOutput]
		public double HorizonPV { get; private set; }
		[FinancialOutput]
		public double HorizonYield { get; private set; }
		[FinancialOutput]
		public double HorizonGross { get; private set; }

		public BondPricer()
		{
			this.Settlement = DateTime.Parse("1 January 2000");
			this.Maturity = DateTime.Parse("1 January 2007");
			this.DayCountBasis = DayCount.Thirty360;
			this.Period = 2;
			this.Rate = 0.08d;
			this.Clean = 0.97d;
			this.YieldToMaturity = 0d;
			this.Principal = 1000;
			this.PV = 0d;

			this.HorizonDate = DateTime.Parse("1 January 2007");
			this.HorizonPrice = 1;
			this.HorizonReinvestment = 0.05d;
		}

		protected BondPricer(DateTime settlement, DateTime maturity, DayCount basis, int period, double coupon, double clean,
			uint principal, double yieldToMaturity, DateTime horizonDate, double horizonPrice, double horizonReinvestment)
		{
			this.Settlement = settlement;
			this.Maturity = maturity;
			this.DayCountBasis = basis;
			this.Period = period;
			this.Rate = coupon;
			this.Clean = clean;
			this.YieldToMaturity = yieldToMaturity;
			this.Principal = principal;

			this.HorizonDate = horizonDate;
			this.HorizonPrice = horizonPrice;
			this.HorizonReinvestment = horizonReinvestment;
		}

		public BondPricer(DateTime settlement, DateTime maturity, DayCount basis, int period, double coupon, double clean,
			uint principal, DateTime horizonDate, double horizonPrice, double horizonReinvestment)
			: this(settlement, maturity, basis, period, coupon, clean, principal, 0d,
			horizonDate, horizonPrice, horizonReinvestment)
		{
		}

		public BondPricer(DateTime settlement, DateTime maturity, DayCount basis, int period, double coupon, uint principal,
			double yieldToMaturity, DateTime horizonDate, double horizonPrice, double horizonReinvestment)
			: this(settlement, maturity, basis, period, coupon, 0d, principal, yieldToMaturity,
			horizonDate, horizonPrice, horizonReinvestment)
		{
		}

		public override void Calculate()
		{
			CalculatePV();
			CalculateYieldToHorizon();
		}

		public double GetCurrentYield()
		{
			if (Math.Abs(this.Clean) < Mathematics.Error)
				CalculateClean();

			return this.Rate / this.Clean * 100d;
		}

		public double GetAdjustedCurrentYield()
		{
			if (Math.Abs(this.Clean) < Mathematics.Error)
				CalculateClean();

			DateTime previousCoupon;
			DateTime nextCoupon;
			Basis.GetPreviousNextCoupons(this.Settlement, this.Maturity, this.Period, out previousCoupon, out nextCoupon);

			var accruedPeriod = Basis.AccruedPeriod(nextCoupon, this.Settlement, this.Period, this.DayCountBasis) / this.Period;
			var payments = Basis.PaymentCount(this.Maturity, nextCoupon, this.Period) / this.Period;

			return (this.Rate + ((1 - this.Clean) / (payments + accruedPeriod))) / this.Clean;
		}

		public void CalculateYieldToHorizon()
		{
			DateTime previousCoupon;
			DateTime nextCoupon;
			Basis.GetPreviousNextCoupons(this.HorizonDate, this.Maturity, this.Period, out previousCoupon, out nextCoupon);
			var accruedPeriod = Basis.AccruedPeriod(this.HorizonDate, previousCoupon, this.Period, this.DayCountBasis);

			var lastCoupon = previousCoupon;
			Basis.GetPreviousNextCoupons(this.Settlement, lastCoupon, this.Period, out previousCoupon, out nextCoupon);
			var payments = Basis.PaymentCount(lastCoupon, nextCoupon, this.Period);

			Basis.GetPreviousNextCoupons(this.Settlement, this.Maturity, this.Period, out previousCoupon, out nextCoupon);
			var accruedToPay = Basis.AccruedPeriod(this.Settlement, previousCoupon, this.Period, this.DayCountBasis);

			var couponsToHorizon = 0d;
			var payment = (this.Rate / this.Period) * this.Principal;
			for (var i = 0; i < payments; ++i)
				couponsToHorizon += payment * Math.Pow(1 + this.HorizonReinvestment / this.Period, i + accruedPeriod);

			this.HorizonPV = this.HorizonPrice * this.Principal + (accruedPeriod * this.Principal * this.Rate / this.Period) + couponsToHorizon;
			this.HorizonYield = Math.Pow(Math.Pow(this.HorizonPV / this.PV, 1 / (payments + accruedPeriod - accruedToPay)), this.Period) - 1;
			this.HorizonGross = this.HorizonPV - this.PV;
		}

		public override void CalculatePV()
		{
			if (Math.Abs(this.YieldToMaturity) < Mathematics.Error)
				SolveBondYield();
			else
				CalculateClean();
		}

		private void CalculateClean()
		{
			DateTime previousCoupon;
			DateTime nextCoupon;
			Basis.GetPreviousNextCoupons(this.Settlement, this.Maturity, this.Period, out previousCoupon, out nextCoupon);

			var accruedPeriod = Basis.AccruedPeriod(this.Settlement, previousCoupon, this.Period, this.DayCountBasis);
			var payments = Basis.PaymentCount(this.Maturity, nextCoupon, this.Period);

			this.Accrued = accruedPeriod * (this.Rate / this.Period) * this.Principal;
			this.PV = 0d;

			for (var i = 0; i < payments; ++i)
			{
				var payment = (this.Rate / this.Period) * this.Principal;
				var discountFactor = Math.Pow(1 + this.YieldToMaturity / this.Period, -(1 - accruedPeriod + i));
				this.PV += (i == payments - 1) ? (payment + this.Principal) * discountFactor : payment * discountFactor;
			}

			this.Clean = this.PV - this.Accrued;
		}

		private void SolveBondYield()
		{
			DateTime previousCoupon;
			DateTime nextCoupon;
			Basis.GetPreviousNextCoupons(this.Settlement, this.Maturity, this.Period, out previousCoupon, out nextCoupon);

			var accruedPeriod = Basis.AccruedPeriod(this.Settlement, previousCoupon, this.Period, this.DayCountBasis);
			var payments = Basis.PaymentCount(this.Maturity, nextCoupon, this.Period);

			this.Accrued = accruedPeriod * (this.Rate / this.Period) * this.Principal;

			Func<double, double> bondPrice = (calcYield =>
			{
				var pv = 0d;
				for (var i = 0; i < payments; ++i)
				{
					var payment = (this.Rate / this.Period);
					var discountFactor = Math.Pow(1 + calcYield / this.Period, -(1 - accruedPeriod + i));
					pv += (i == payments - 1) ? (payment + 1) * discountFactor : payment * discountFactor;
				}
				return pv - accruedPeriod * (this.Rate / this.Period) - this.Clean;
			});

			this.YieldToMaturity = Mathematics.FindRoot(bondPrice, 0d, 0.5d);
			this.PV = this.Clean * this.Principal + this.Accrued;
		}
	}
}
