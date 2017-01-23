using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnicodeConverter
{
	//Contains converters for gothic typing.

	/// <summary>
	/// Converter into Frakturschrift
	/// </summary>
	class FrakturConverter : Converter
	{
		private ICollection<ConversionRule> conversionRules;

		public FrakturConverter()
		{
			ConversionType = Util.ConverterType.FRAKTURBOLD;
			InitConversionRules();
		}

		private void InitConversionRules()
		{
			conversionRules = new List<ConversionRule>(ConversionRule.FromAlphabet(0x1D56C, false));
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
