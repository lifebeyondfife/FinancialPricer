using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LbF
{
	public class FXQuote
	{
		public string BaseCurrency { get; private set; }
		public string CounterCurrency { get; private set; }
		public double BidRate { get; private set; }
		public double OfferRate { get; private set; }

		public FXQuote()
		{
			this.BaseCurrency = "GBP";
			this.CounterCurrency = "EUR";
			this.BidRate = 1.19237d;
			this.OfferRate = 1.19287d;
		}

		public FXQuote(string baseCurrency, string counterCurrency, double bidRate, ushort offerPlusPip)
		{
			this.BaseCurrency = baseCurrency;
			this.CounterCurrency = counterCurrency;
			this.BidRate = bidRate;
			this.OfferRate = bidRate + (offerPlusPip / 10000d);
		}

		public override string ToString()
		{
			var result = new StringBuilder("\n##### FX Quote #####\n\n");
			result.Append("  --------------\n");
			result.AppendFormat("  | {0}/{1}:".PadRight(Constants.Spacing), this.BaseCurrency, this.CounterCurrency);

			var bidString = this.BidRate.ToString(CultureInfo.InvariantCulture).Take(7).ToArray();
			var offerString = this.OfferRate.ToString(CultureInfo.InvariantCulture).Take(7).ToArray();

			var offerDiff = new StringBuilder();
			var bidAndOfferDiffer = false;
			for (var i = 0; i < Math.Max(bidString.Length, offerString.Length); ++i)
			{
				if (bidAndOfferDiffer)
				{
					result.Append(i < bidString.Length ? bidString[i] : '0');
					offerDiff.Append(i < offerString.Length ? offerString[i] : '0');
				}
				else
				{
					result.Append(i < bidString.Length ? bidString[i] : '0');

					if (i == bidString.Length || (i < offerString.Length && bidString[i] != offerString[i]))
					{
						bidAndOfferDiffer = true;
						offerDiff.Append(offerString[i]);
					}
				}
			}

			if (offerDiff.Length > 0)
				result.AppendFormat("/{0}", offerDiff.ToString());

			result.Append("\n  --------------\n");
			return result.ToString();
		}

		public static FXQuote CalculateCrossCurrencyQuote(FXQuote firstQuote, FXQuote secondQuote)
		{
			var commonCurrency = FindCommonCurrency(firstQuote, secondQuote);

			if (firstQuote.BaseCurrency == commonCurrency)
			{
				return firstQuote.BaseCurrency == secondQuote.BaseCurrency ?
					CalculateSameQuote(commonCurrency, firstQuote, secondQuote) :
					CalculateDifferentQuote(commonCurrency, firstQuote, secondQuote);
			}

			return firstQuote.CounterCurrency == secondQuote.CounterCurrency ?
				CalculateSameQuote(commonCurrency, firstQuote, secondQuote) :
				CalculateDifferentQuote(commonCurrency, firstQuote, secondQuote);
		}

		private static FXQuote CalculateDifferentQuote(string commonCurrency, FXQuote firstQuote, FXQuote secondQuote)
		{
			var fxQuote = new FXQuote();

			if (firstQuote.BaseCurrency == commonCurrency)
			{
				fxQuote.BaseCurrency = firstQuote.CounterCurrency;
				fxQuote.CounterCurrency = secondQuote.BaseCurrency;
				fxQuote.BidRate = 1 / (firstQuote.OfferRate * secondQuote.OfferRate);
				fxQuote.OfferRate = 1 / (firstQuote.BidRate * secondQuote.BidRate);
			}
			else
			{
				fxQuote.BaseCurrency = firstQuote.BaseCurrency;
				fxQuote.CounterCurrency = secondQuote.CounterCurrency;
				fxQuote.BidRate = firstQuote.BidRate * secondQuote.BidRate;
				fxQuote.OfferRate = firstQuote.OfferRate * secondQuote.OfferRate;
			}

			return fxQuote;
		}

		private static FXQuote CalculateSameQuote(string commonCurrency, FXQuote firstQuote, FXQuote secondQuote)
		{
			var fxQuote = new FXQuote();

			if (firstQuote.BaseCurrency == commonCurrency)
			{
				fxQuote.BaseCurrency = firstQuote.CounterCurrency;
				fxQuote.CounterCurrency = secondQuote.CounterCurrency;
				fxQuote.BidRate = secondQuote.BidRate / firstQuote.OfferRate;
				fxQuote.OfferRate = secondQuote.OfferRate / firstQuote.BidRate;
			}
			else
			{
				fxQuote.BaseCurrency = firstQuote.BaseCurrency;
				fxQuote.CounterCurrency = secondQuote.BaseCurrency;
				fxQuote.BidRate = firstQuote.BidRate / secondQuote.OfferRate;
				fxQuote.OfferRate = firstQuote.OfferRate / secondQuote.BidRate;
			}

			return fxQuote;
		}

		private static string FindCommonCurrency(FXQuote firstQuote, FXQuote secondQuote)
		{
			if (firstQuote.BaseCurrency == secondQuote.BaseCurrency)
				return firstQuote.BaseCurrency;

			if (firstQuote.BaseCurrency == secondQuote.CounterCurrency)
				return firstQuote.BaseCurrency;

			if (firstQuote.CounterCurrency == secondQuote.BaseCurrency)
				return firstQuote.CounterCurrency;

			if (firstQuote.CounterCurrency == secondQuote.CounterCurrency)
				return firstQuote.CounterCurrency;

			throw new FinancialException("No common currency between FXQuote objects");
		}
	}
}
