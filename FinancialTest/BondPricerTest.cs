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
	public class BondPricerTest
	{
		private readonly BondPricer bondPricerYield;
		private readonly BondPricer bondPricerClean;

		public BondPricerTest()
		{
			bondPricerYield = new BondPricer(settlement: DateTime.Parse("1 January 2000"), maturity: DateTime.Parse("15 August 2010"),
				basis: DayCount.ActAct, period: 4, coupon: 0.08d, clean: 0.97d, principal: 1,
				horizonDate: DateTime.Parse("30 June 2005"), horizonPrice: 1d, horizonReinvestment: 0.06d);

			bondPricerClean = new BondPricer(settlement: DateTime.Parse("1 January 2000"), maturity: DateTime.Parse("15 August 2010"),
				basis: DayCount.ActAct, period: 4, coupon: 0.08d, principal: 1, yieldToMaturity: 0.084295197233745636d,
				horizonDate: DateTime.Parse("30 June 2005"), horizonPrice: 1d, horizonReinvestment: 0.06d);
		}

		[TestMethod]
		public void TestBondCleanPriceAndPV()
		{
			bondPricerYield.Calculate();
			Assert.AreEqual(0.084295197233745636d, bondPricerYield.YieldToMaturity, Mathematics.Error, "Test calculation of Bond Yield to Maturity");

			bondPricerClean.Calculate();
			Assert.AreEqual(0.97d, bondPricerClean.Clean, Mathematics.Error, "Test calculation of Bond Clean Price");

			Assert.AreEqual(bondPricerClean.PV, bondPricerYield.PV, Mathematics.Error, "Test bonds' PV values match");
		}

		[TestMethod]
		public void TestBondHorizonYieldAndPV()
		{
			bondPricerYield.Calculate();
			Assert.AreEqual(1.5306128101742258d, bondPricerYield.HorizonPV, Mathematics.Error, "Test calculation of PV Yield to Horizon");
			
			bondPricerClean.Calculate();
			Assert.AreEqual(0.084443725360003441d, bondPricerClean.HorizonYield, Mathematics.Error, "Test calculation of Yield to Horizon");
		}
	}
}
