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
	public class Mathematics
	{
		public static double FindRoot(Func<double, double> function, double lowerBound, double upperBound)
		{
			var x0 = lowerBound;
			var x1 = upperBound;

			var xNext = 0.0;
			var function_x0 = function(x0);
			var function_x1 = function(x1);

			//	Secant Root Finding Method.
			while (Math.Abs(function_x1) > 1E-12)
			{
				xNext = x1 - (function_x1 * (x1 - x0) / (function_x1 - function_x0));
				x0 = x1;
				x1 = xNext;
				function_x0 = function_x1;
				function_x1 = function(x1);
			}

			if (Double.IsNaN(xNext))
				throw new FinancialException(string.Format("Could not find solve rate for bounds [{0}, {1}]", lowerBound, upperBound));

			return xNext;
		}
	}
}
