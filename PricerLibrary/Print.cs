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
using System.Reflection;
using System.Text;

namespace LbF
{
	public class FinancialAttribute : Attribute
	{
		public string Name { get; set; }
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class FinancialInstrumentAttribute : FinancialAttribute { }
	[AttributeUsage(AttributeTargets.Property)]
	public class FinancialInputAttribute : FinancialAttribute { }
	[AttributeUsage(AttributeTargets.Property)]
	public class FinancialOutputAttribute : FinancialAttribute { }

	public static class Print
	{
		public static string FormatProperties(Instrument instrument)
		{
			var stringBuilder = new StringBuilder();

			var instrumentType = instrument.GetType().GetCustomAttributes(typeof(FinancialInstrumentAttribute), true);
			if (instrumentType.Length > 0 && instrumentType[0] is FinancialInstrumentAttribute)
				stringBuilder.AppendFormat("\n##### {0} #####\n", ((FinancialInstrumentAttribute) instrumentType[0]).Name);
		
			var properties = instrument.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
		
			stringBuilder.Append("\n  --- Input ---\n");
			foreach (var propertyDefinition in properties)
			{
				if (propertyDefinition.IsDefined(typeof(FinancialInputAttribute), true))
				{
					var propertyValue = propertyDefinition.GetValue(instrument, null);
					stringBuilder.AppendFormat(("  | " + propertyDefinition.Name + ":").PadRight(Constants.Spacing)
						+ "{0}\n", propertyValue.ToString());
				}
			}

			stringBuilder.Append("\n  --- Output ---\n");
			foreach (var propertyDefinition in properties)
			{
				if (propertyDefinition.IsDefined(typeof(FinancialOutputAttribute), true))
				{
					var propertyValue = propertyDefinition.GetValue(instrument, null);
					stringBuilder.AppendFormat(("  | " + propertyDefinition.Name + ":").PadRight(Constants.Spacing)
						+ "{0}\n", propertyValue.ToString());
				}
			}
			stringBuilder.Append("  --------------\n");

			return stringBuilder.ToString();
		}
	}
}
