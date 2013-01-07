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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LbF
{
	[TestClass]
	public class CashflowPricerTest
	{
		private CashflowPricer cashflowPricerPV;
		private CashflowPricer cashflowPricerRate;
		
		public CashflowPricerTest()
		{
			cashflowPricerRate = new CashflowPricer(years: 3, period: 1, coupon: 0.4021148036253774, principal:100, pv: 100.0);
			cashflowPricerPV = new CashflowPricer(years: 3, period: 1, coupon: 0.4021148036253774, rate: 0.1, principal: 100);
		}

		[TestMethod]
		public void TestCashflowRate()
		{
			cashflowPricerRate.SolveInternalRateOfReturn();

			Assert.AreEqual(0.1, cashflowPricerRate.Rate, 1E-10, "Test calculation of Cashflow Rate");
		}
	
		[TestMethod]
		public void TestCashflowPV()
		{
			cashflowPricerPV.CalculatePV();

			Assert.AreEqual(100, cashflowPricerPV.PV, 1E-10, "Test calculation of Cashflow Rate");
		}
	}
}
