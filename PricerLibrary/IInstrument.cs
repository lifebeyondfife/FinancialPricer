using System;

namespace LbF
{
	public interface IInstrument
	{
		uint Principal { get; set; }
		DateTime Settlement { get; set; }
		DateTime Maturity { get; set; }

		double Rate { get; }
		double PV { get; }

		void Calculate();
		double MarkToMarket(DateTime mtmSettlement, double mtmRate, Position position);
	}
}
