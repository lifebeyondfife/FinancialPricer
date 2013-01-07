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
	public class MathsLibraryTest
	{
		[TestMethod]
		public void TestFindQuadraticRoot()
		{
			// Quadratic with roots of x = -7 and x = 4
			Func<double, double> function = x => (2 * x - 8) * (x + 7);

			Assert.AreEqual(-7.0, Mathematics.FindRoot(function, -10, 0), 1E-10, "Finding root where x=-7");
			Assert.AreEqual(4.0, Mathematics.FindRoot(function, 0, 10), 1E-10, "Finding root where x=4");
		}
	}
}
