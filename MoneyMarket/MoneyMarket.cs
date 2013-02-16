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
	[FinancialInstrument(Name = "Money Market Instrument")]
	public abstract class MoneyMarket : Instrument
	{
		[FinancialOutput]
		public double FV { get; set; }	// PV = Settlement Amount, FV = Repayment Amount
		[FinancialInput]
		public DayCount DayCountBasis { get; set; }
	}

	[FinancialInstrument(Name = "Fixed Deposit")]
	public class FixedDeposit : MoneyMarket
	{
		[FinancialOutput]
		public double Yield { get; private set; }

		public FixedDeposit()
		{
			this.Principal = 1000000;
			this.Settlement = DateTime.Parse("17 March 2010");
			this.Maturity = DateTime.Parse("17 July 2010");
			this.Rate = 0.0655d;
			this.DayCountBasis = DayCount.Act360;
		}

		public FixedDeposit(uint principal, double rate, DayCount basis, DateTime settlement, DateTime maturity)
		{
			this.Principal = principal;
			this.Settlement = settlement;
			this.Maturity = maturity;
			this.Rate = rate;
			this.DayCountBasis = basis;
		}

		public void CalculateFV()
		{
			var settlementToMaturity = Basis.AccruedPeriod(this.Maturity, this.Settlement, 1, this.DayCountBasis);
			this.FV = this.Principal * (1 + this.Rate * settlementToMaturity);
		}

		public override void CalculatePV()
		{
			this.PV = this.Principal;
		}

		public override void Calculate()
		{
			var settlementToMaturity = Basis.AccruedPeriod(this.Maturity, this.Settlement, 1, this.DayCountBasis);

			this.FV = this.Principal * (1 + this.Rate * settlementToMaturity);
			this.PV = this.Principal;
			this.Yield = this.Rate * (1 / (1 - this.Rate * settlementToMaturity));
		}
		
		public override double MarkToMarket(DateTime mtmSettlement, double mtmRate, Position position)
		{
			var certificateDeposit = new CertificateDeposit(this.Principal, this.Rate, this.DayCountBasis, mtmSettlement,
				this.Maturity, this.Settlement, mtmRate);

			CalculatePV();
			certificateDeposit.CalculatePV();

			return position == Position.Long ? certificateDeposit.PV - this.PV : this.PV - certificateDeposit.PV;
		}
	}

	[FinancialInstrument(Name = "Certificate of Deposit")]
	public sealed class CertificateDeposit : MoneyMarket
	{
		[FinancialInput]
		public DateTime Issued { get; set; }
		[FinancialInput]
		public double Coupon { get; set; }

		public CertificateDeposit()
		{
			this.PV = 1000;
			this.Coupon = 0.08d;
			this.Principal = 1000;
			this.Settlement = DateTime.Parse("1 January 2010");
			this.Maturity = DateTime.Parse("1 April 2010");
			this.DayCountBasis = DayCount.Thirty360;
			this.Issued = DateTime.Parse("1 January 2010");
			this.Rate = 0.08d;
		}

		public CertificateDeposit(uint principal, double coupon, DayCount basis, DateTime settlement,
			DateTime maturity, DateTime issued, double yield)
		{
			this.Principal = principal;
			this.Coupon = coupon;
			this.DayCountBasis = basis;
			this.Settlement = settlement;
			this.Maturity = maturity;
			this.Issued = issued;
			this.Rate = yield;
		}

		public void CalculateFV()
		{
			var issuedToMaturity = Basis.AccruedPeriod(this.Maturity, this.Issued, 1, this.DayCountBasis);
			this.FV = this.Principal * (1 + this.Coupon * issuedToMaturity);
		}

		public override void CalculatePV()
		{
			if (Math.Abs(this.FV) < Mathematics.Error)
				CalculateFV();

			var settlementToMaturity = Basis.AccruedPeriod(this.Maturity, this.Settlement, 1, this.DayCountBasis);
			this.PV = this.FV / (1 + this.Rate * settlementToMaturity);
		}

		public override void Calculate()
		{
			var issuedToMaturity = Basis.AccruedPeriod(this.Maturity, this.Issued, 1, this.DayCountBasis);
			this.FV = this.Principal * (1 + this.Coupon * issuedToMaturity);

			var settlementToMaturity = Basis.AccruedPeriod(this.Maturity, this.Settlement, 1, this.DayCountBasis);
			this.PV = this.FV / (1 + this.Rate * settlementToMaturity);
		}
	}

	[FinancialInstrument(Name = "Discount Paper")]
	public class DiscountPaper : MoneyMarket
	{
		[FinancialOutput]
		public double Yield { get; private set; }	// this.Rate / (1 - this.Rate);

		public DiscountPaper()
		{
			this.Rate = 0.0712d;
			this.Principal = 500000;
			this.Settlement = DateTime.Parse("12 August 2002");
			this.Maturity = DateTime.Parse("1 October 2002");
			this.DayCountBasis = DayCount.Act365;
		}

		public DiscountPaper(uint principal, double discountRate, DayCount basis, DateTime settlement, DateTime maturity)
		{
			this.Principal = principal;
			this.Rate = discountRate;
			this.DayCountBasis = basis;
			this.Settlement = settlement;
			this.Maturity = maturity;
		}

		public override void CalculatePV()
		{
			var settlementToMaturity = Basis.AccruedPeriod(this.Maturity, this.Settlement, 1, this.DayCountBasis);
			this.PV = this.Principal * (1 - this.Rate * settlementToMaturity);
		}

		public override void Calculate()
		{
			CalculatePV();

			var settlementToMaturityInverse = 1 / Basis.AccruedPeriod(this.Maturity, this.Settlement, 1, this.DayCountBasis);
			this.Yield = (settlementToMaturityInverse * (this.Principal - this.PV)) / this.PV;
		}
	}
}
