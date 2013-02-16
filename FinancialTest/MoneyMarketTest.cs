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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LbF
{
	[TestClass]
	public class MoneyMarketTest
	{
		private readonly MoneyMarket certificateDepositPV;
		private readonly MoneyMarket certificateDepositFV;
		private readonly MoneyMarket certificateDepositIssued;
		private readonly MoneyMarket certificateDepositMtM;
		private readonly MoneyMarket commercialPaper;
		private readonly MoneyMarket fixedDepositMtM;

		public MoneyMarketTest()
		{
			this.certificateDepositPV = new CertificateDeposit(principal: 1000, coupon: 0.08d, basis: DayCount.Thirty360,
				settlement: DateTime.Parse("1 January 2010"), maturity: DateTime.Parse("1 April 2010"),
				issued: DateTime.Parse("1 January 2010"), yield: 0.08d);

			this.certificateDepositFV = new CertificateDeposit(principal: 1000, coupon: 0.08d, basis: DayCount.Thirty360,
				settlement: DateTime.Parse("1 January 2010"), maturity: DateTime.Parse("1 April 2010"),
				issued: DateTime.Parse("1 January 2010"), yield: 0.08d);

			this.certificateDepositIssued = new CertificateDeposit(principal: 1000000, coupon: 0.0565d, basis: DayCount.Act360,
				settlement: DateTime.Parse("11 March 2002"), maturity: DateTime.Parse("11 June 2002"),
				issued: DateTime.Parse("11 December 2001"), yield: 0.0475d);
			
			this.certificateDepositMtM = new CertificateDeposit(principal: 500000, coupon: 0.0625d, basis: DayCount.Act360,
				settlement: DateTime.Parse("30 July 2002"), maturity: DateTime.Parse("14 March 2003"),
				issued: DateTime.Parse("14 May 2002"), yield: 0.0576d);

			this.commercialPaper = new DiscountPaper(principal: 1000000, discountRate: 0.059d, basis: DayCount.Act360,
				settlement: DateTime.Parse("30 July 2002"), maturity: DateTime.Parse("10 February 2003"));

			this.fixedDepositMtM = new FixedDeposit(principal: 460000, rate: 0.055d, basis: DayCount.Act360,
				settlement: DateTime.Parse("30 July 2002"), maturity: DateTime.Parse("30 January 2003"));
		}

		[TestMethod]
		public void TestDepositCertificatePV()
		{
			this.certificateDepositPV.FV = 1000d;
			this.certificateDepositPV.CalculatePV();

			Assert.AreEqual(980.39215686274508d, this.certificateDepositPV.PV, Mathematics.Error, "Test calculation of Certificate of Deposit PV");
		}

		[TestMethod]
		public void TestDepositCertificateFV()
		{
			this.certificateDepositFV.PV = 1000d;
			this.certificateDepositFV.Calculate();

			Assert.AreEqual(1020d, this.certificateDepositFV.FV, Mathematics.Error, "Test calculation of Certificate of Deposit FV");
		}

		[TestMethod]
		public void TestDepositCertificateIssued()
		{
			this.certificateDepositIssued.Calculate();

			Assert.AreEqual(1028563.8888888889d, this.certificateDepositIssued.FV, Mathematics.Error,
				"Test calculation of Certificate of Deposit FV from Issued date");
			Assert.AreEqual(1016228.0099898455d, this.certificateDepositIssued.PV, Mathematics.Error,
				"Test calculation of Certificate of Deposit PV from Issued date");
		}

		[TestMethod]
		public void TestCommercialPaper()
		{
			this.commercialPaper.Calculate();

			Assert.AreEqual(968041.66666666674d, this.commercialPaper.PV, Mathematics.Error, "Test calculation of Commercial Paper PV");
		}

		[TestMethod]
		public void TestCommercialPaperMarkToMarket()
		{
			Assert.AreEqual(7161.11111111101d, this.commercialPaper.MarkToMarket(DateTime.Parse("5 September 2002"), 0.0565d,
				Position.Long), Mathematics.Error, "Test calculation of Commerical Paper Mark to Market");
		}

		[TestMethod]
		public void TestCertificateDepositMarkToMarket()
		{
			Assert.AreEqual(-3914.55885083048d, this.certificateDepositMtM.MarkToMarket(DateTime.Parse("5 September 2002"), 0.0538d,
				Position.Short), Mathematics.Error, "Test calculation of Certificate Deposit Mark to Market");
		}

		[TestMethod]
		public void TestFixedDepositMarkToMarket()
		{
			Assert.AreEqual(-3236.9136167767574d, this.fixedDepositMtM.MarkToMarket(DateTime.Parse("5 September 2002"), 0.05125d,
				Position.Short), Mathematics.Error, "Test calculation of Certificate Deposit Mark to Market");
		}
	}
}
