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
	public enum DayCount
	{
		Act360,
		Act365,
		ActAct,
		Thirty360
	}

	public class Basis
	{
		public static double AccruedPeriod(DateTime endDate, DateTime startDate, int period, DayCount dayCount)
		{
			int _;
			return AccruedPeriod(endDate, startDate, period, dayCount, out _);
		}

		public static double AccruedPeriod(DateTime endDate, DateTime startDate, int period, DayCount dayCount, out int totalDays)
		{
			switch (dayCount)
			{
				case DayCount.ActAct:
					totalDays = endDate.Subtract(startDate).Days;
					return (double) totalDays / startDate.AddMonths(12 / period).Subtract(startDate).Days;

				case DayCount.Act360:
					totalDays = endDate.Subtract(startDate).Days;
					return totalDays / ((double) 360 / period);

				case DayCount.Act365:
					totalDays = endDate.Subtract(startDate).Days;
					return totalDays / ((double) 365 / period);

				case DayCount.Thirty360:
					var years = (endDate.Year - startDate.Year);
					var months = (endDate.Month - startDate.Month);
					var days = Math.Min(endDate.Day - startDate.Day, 30);
					totalDays = (years * 12 * 30) + (months * 30) + days;
					return totalDays / ((double) 360 / period);

				default:
					throw new FinancialException("Invalid Basis: " + dayCount);
			}
		}

		public static int PaymentCount(DateTime maturity, DateTime nextCoupon, int period)
		{
			if (nextCoupon > maturity)
				throw new FinancialException("Next Coupon must be before Maturity");

			var payments = 0;
			var currentPayment = nextCoupon;
			while (currentPayment <= maturity)
			{
				currentPayment = currentPayment.AddMonths(12 / period);
				++payments;
			}

			return payments;
		}

		public static void GetPreviousNextCoupons(DateTime settlement, DateTime maturity, int period, out DateTime previousCoupon, out DateTime nextCoupon)
		{
			if (settlement > maturity)
				throw new FinancialException("Settlement must be before Maturity");

			previousCoupon = maturity;

			do
			{
				nextCoupon = previousCoupon;
				previousCoupon = nextCoupon.AddMonths(-(12 / period));
			}
			while (previousCoupon > settlement);
		}

		public static double BondToMoneyMarketYield(double bondEquivalentYield)
		{
			return (360d / 365d) * bondEquivalentYield;
		}

		public static double MoneyMarketToBondYield(double moneyMarketYield)
		{
			return (365d / 360d) * moneyMarketYield;
		}
	}
}
