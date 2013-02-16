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
	public class FXQuoteTest
	{
		private readonly FXQuote fxAUD_USD;
		private readonly FXQuote fxGBP_USD;
		private readonly FXQuote fxUSD_CHF;
		private readonly FXQuote fxUSD_EUR;

		public FXQuoteTest()
		{
			this.fxAUD_USD = new FXQuote(baseCurrency: "AUD", counterCurrency: "USD", bidRate: 1.00275d, offerPlusPip: 5);
			this.fxGBP_USD = new FXQuote(baseCurrency: "GBP", counterCurrency: "USD", bidRate: 1.56788d, offerPlusPip: 5);
			this.fxUSD_CHF = new FXQuote(baseCurrency: "USD", counterCurrency: "CHF", bidRate: 0.92866d, offerPlusPip: 5);
			this.fxUSD_EUR = new FXQuote(baseCurrency: "USD", counterCurrency: "EUR", bidRate: 0.76257d, offerPlusPip: 5);
		}

		[TestMethod]
		public void TestCalculateSameQuote()
		{
			var fxCHF_EUR = FXQuote.CalculateCrossCurrencyQuote(this.fxUSD_CHF, this.fxUSD_EUR);
			Assert.AreEqual(0.82070902750871755d, fxCHF_EUR.BidRate, Mathematics.Error, "Bid rate wrong for A/B, A/C => B/C");
			Assert.AreEqual(0.82168931578834d, fxCHF_EUR.OfferRate, Mathematics.Error, "Offer rate wrong for A/B, A/C => B/C");

			var fxAUD_GBP = FXQuote.CalculateCrossCurrencyQuote(this.fxAUD_USD, this.fxGBP_USD);
			Assert.AreEqual(0.63935398309083269d, fxAUD_GBP.BidRate, Mathematics.Error, "Bid rate wrong for A/B, C/B => A/C");
			Assert.AreEqual(0.63987677628389927d, fxAUD_GBP.OfferRate, Mathematics.Error, "Offer rate wrong for A/B, C/B => A/C");
		}
	
		[TestMethod]
		public void TestCalculateDifferentQuote()
		{
			var fxGBP_EUR = FXQuote.CalculateCrossCurrencyQuote(this.fxGBP_USD, this.fxUSD_EUR);
			Assert.AreEqual(1.1956182516d, fxGBP_EUR.BidRate, Mathematics.Error, "Bid rate wrong for A/B, B/C => A/C");
			Assert.AreEqual(1.1967837266d, fxGBP_EUR.OfferRate, Mathematics.Error, "Offer rate wrong for A/B, B/C => A/C");

			var fxEUR_AUD = FXQuote.CalculateCrossCurrencyQuote(this.fxUSD_EUR, this.fxAUD_USD);
			Assert.AreEqual(1.3062504465947817d, fxEUR_AUD.BidRate, Mathematics.Error, "Bid rate wrong for A/B, C/A => B/C");
			Assert.AreEqual(1.3077586867568349d, fxEUR_AUD.OfferRate, Mathematics.Error, "Offer rate wrong for A/B, C/A => B/C");
		}
	}
}
