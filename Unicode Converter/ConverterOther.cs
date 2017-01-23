using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicodeConverter
{
	//Contains mostly miscellaneous converters

	/// <summary>
	/// MonospaceConverter
	/// </summary>
	class MonospaceConverter : Converter
	{
		private ICollection<ConversionRule> conversionRules;

		public MonospaceConverter()
		{
			ConversionType = Util.ConverterType.Monospace;
			InitConversionRules();
		}

		private void InitConversionRules()
		{
			conversionRules = new List<ConversionRule>(ConversionRule.FromAlphabet(0x1D670, false));
		}

		public override string ConvertChar(char ch)
		{
			if (CanConvertChar(ch))
			{
				return Util.Find(conversionRules, ch).Convert(ch);
			}
			return char.ConvertFromUtf32(ch);
		}

		public override bool CanConvertChar(char ch)
		{
			return Util.Find(conversionRules, ch) != null;
		}
	}
}
