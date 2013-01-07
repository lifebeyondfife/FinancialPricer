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
	public class YieldCurveTest
	{
		private YieldCurve yieldCurveSpot;
		private YieldCurve yieldCurvePar;

		public YieldCurveTest()
		{
			yieldCurveSpot = new YieldCurve();
			yieldCurveSpot.ParYields = new[] { 0.0, 0.08, 0.09, 0.1, 0.11 };

			yieldCurvePar = new YieldCurve();
			yieldCurvePar.SpotYields = new[] { 0.0, 0.08, 0.09045445071809799, 0.10139519558712839, 0.11301161161266693 };
		}

		[TestMethod]
		public void TestYieldCurveSpotYields()
		{
			yieldCurveSpot.BootstrapSpotYields();

			var expectedValues = new[] { 0.0, 0.08, 0.09045445071809799, 0.10139519558712839, 0.11301161161266693 };
			for (int i = 0; i < expectedValues.Length; ++i)
				Assert.AreEqual(expectedValues[i], yieldCurveSpot.SpotYields[i], 1E-10, "Test calculation of Bootstrapped Spot Yields");
		}

		[TestMethod]
		public void TestYieldCurveParYields()
		{
			yieldCurvePar.BootstrapParYields();

			var expectedValues = new[] { 0.0, 0.08, 0.09, 0.1, 0.11 };
			for (int i = 0; i < expectedValues.Length; ++i)
				Assert.AreEqual(expectedValues[i], yieldCurvePar.ParYields[i], 1E-10, "Test calculation of Bootstrapped Par Yields");
		}

		[TestMethod]
		public void TestDiscountFactors()
		{
			yieldCurveSpot.BootstrapSpotYields();
			yieldCurvePar.BootstrapParYields();

			for (int i=0; i < yieldCurvePar.ParYields.Count; i++)
				Assert.AreEqual(yieldCurvePar.DiscountFactors[i], yieldCurveSpot.DiscountFactors[i], 1E-10,
					"Test calculation of Bootstrapped Discount Factors");
		}
	}
}
