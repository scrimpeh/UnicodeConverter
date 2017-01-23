using System.Collections.Generic;

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
			ConversionType = Util.ConverterType.FrakturBold;
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
			return char.ConvertFromUtf32(ch);
		}

		public override bool CanConvertChar(char ch)
		{
			return Util.Find(conversionRules, ch) != null;
		}
	}
}
