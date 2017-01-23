using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicodeConverter.Converters
{
	//This file contains mostly conveters for script.

	/// <summary>
	/// A class for bold script. Completely featured in the Unicode code chart.
	/// Starts at U+1D4Dx
	/// </summary>
	class ScriptBoldConverter : Converter
	{
		private ICollection<ConversionRule> conversionRules;

		public ScriptBoldConverter()
		{
			ConversionType = Util.ConverterType.SCRIPTBOLD;
			InitConversionRules();
		}

		private void InitConversionRules()
		{
			conversionRules = new List<ConversionRule>(ConversionRule.FromAlphabet(0x1D4D0, false));
		}

		public override string ConvertChar(char ch)
		{
			if (CanConvertChar(ch))
			{
				return Util.Find(conversionRules, ch).Convert(ch);
			}
			return Char.ConvertFromUtf32(ch);
		}

		public override bool CanConvertChar(char ch)
		{
			return Util.Find(conversionRules, ch) != null;
		}
	}
}
