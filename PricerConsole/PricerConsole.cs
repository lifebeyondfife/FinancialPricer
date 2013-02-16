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
	public class PricerConsole
	{
		static void Main()
		{
			var cashflowPricer = new CashflowPricer();			
			cashflowPricer.Calculate();
			Console.WriteLine(cashflowPricer);

			var bondPricer = new BondPricer();
			bondPricer.Calculate();
			Console.WriteLine(bondPricer);

			var yieldCurve = new YieldCurve();
			yieldCurve.BootstrapSpotYields();
			Console.WriteLine(yieldCurve);

			var depositCertificate = new CertificateDeposit();
			depositCertificate.Calculate();
			Console.WriteLine(depositCertificate);

			var discountPaper = new DiscountPaper();
			discountPaper.Calculate();
			Console.WriteLine(discountPaper);

			var fixedDeposit = new FixedDeposit();
			fixedDeposit.Calculate();
			Console.WriteLine(fixedDeposit);

			var fxQuote = new FXQuote();
			Console.WriteLine(fxQuote);
			
			Console.ReadKey();
		}
	}
}
