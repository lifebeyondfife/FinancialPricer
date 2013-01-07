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
	public enum Position
	{
		Long,
		Short
	}

	[FinancialInstrument]
	public abstract class Instrument
	{
		[FinancialInput]
		public uint Principal { get; set; }
		[FinancialInput]
		public DateTime Settlement { get; set; }
		[FinancialInput]
		public DateTime Maturity { get; set; }

		//	Sometimes this is an interest rate, a yield or a coupon etc.
		//	This field has been commandeered to be overloaded in this way
		//	such that a generic MarkToMarket function could be written.
		[FinancialInput]
		public double Rate { get; set; }

		[FinancialOutput]
		public double PV { get; set; }

		public override string ToString()
		{
			return Print.FormatProperties(this);
		}

		//	Calculating a PV is something that all instruments should be able to do
		public abstract void CalculatePV();

		//	This should be overriden if pricing an instrument computes more FinancialOutput property values than just this.PV
		public virtual void Calculate()
		{
			CalculatePV();
		}

		public virtual double MarkToMarket(DateTime mtmSettlement, double mtmRate, Position position)
		{
			var instrumentCopy = (Instrument) this.MemberwiseClone();

			instrumentCopy.Settlement = mtmSettlement;
			instrumentCopy.Rate = mtmRate;
			instrumentCopy.CalculatePV();

			CalculatePV();

			return position == Position.Long ? instrumentCopy.PV - this.PV : this.PV - instrumentCopy.PV;
		}
	}
}
