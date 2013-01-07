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
		BondPricer bondPricerYield;
		BondPricer bondPricerClean;

		public BondPricerTest()
		{
			bondPricerYield = new BondPricer(settlement: DateTime.Parse("1 January 2000"), maturity: DateTime.Parse("15 August 2010"),
				basis: DayCount.ActAct, period: 4, coupon: 0.08, clean: 0.97, principal: 1,
				horizonDate: DateTime.Parse("30 June 2005"), horizonPrice: 1.0, horizonReinvestment: 0.06);

			bondPricerClean = new BondPricer(settlement: DateTime.Parse("1 January 2000"), maturity: DateTime.Parse("15 August 2010"),
				basis: DayCount.ActAct, period: 4, coupon: 0.08, principal: 1, yieldToMaturity: 0.084295197233745636,
				horizonDate: DateTime.Parse("30 June 2005"), horizonPrice: 1.0, horizonReinvestment: 0.06);
		}

		[TestMethod]
		public void TestBondCleanPriceAndPV()
		{
			bondPricerYield.Calculate();
			Assert.AreEqual(0.084295197233745636, bondPricerYield.YieldToMaturity, 1E-10, "Test calculation of Bond Yield to Maturity");

			bondPricerClean.Calculate();
			Assert.AreEqual(0.97, bondPricerClean.Clean, 1E-10, "Test calculation of Bond Clean Price");

			Assert.AreEqual(bondPricerClean.PV, bondPricerYield.PV, 1E-10, "Test bonds' PV values match");
		}

		[TestMethod]
		public void TestBondHorizonYieldAndPV()
		{
			bondPricerYield.Calculate();
			Assert.AreEqual(1.5306128101742258, bondPricerYield.HorizonPV, 1E-10, "Test calculation of PV Yield to Horizon");
			
			bondPricerClean.Calculate();
			Assert.AreEqual(0.084443725360003441, bondPricerClean.HorizonYield, 1E-10, "Test calculation of Yield to Horizon");
		}
	}
}
