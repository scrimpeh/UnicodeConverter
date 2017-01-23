using System;
using System.Collections.Generic;
using System.Linq;
using UnicodeConverter.Converters;

using ConverterType = UnicodeConverter.Util.ConverterType;

namespace UnicodeConverter
{
	/// <summary>
	/// Basic Prototype which handles all other conversion operations.
	/// Subclasses will be instantiated with each typeface.
	/// </summary>
	public abstract class Converter
	{
		/// <summary>
		/// Converts an individual character from ASCII into this converter's type.
		/// type.
		/// </summary>
		/// <param name="ascii">The character to convert</param>
		/// <returns>The converterd character if conversion is possible. Otherwise, it returns the original character.</returns>
		public abstract string ConvertChar(char ascii);

		/// <summary>
		/// Tests if a character can be converted to this type.
		/// </summary>
		/// <param name="ascii">The character to test</param>
		/// <returns>True, if the character can be converted in this given converter. False otherwise</returns>
		public abstract bool CanConvertChar(char ascii);

		/// <summary>
		/// Converts an ASCII string into this converter's type.
		/// </summary>
		/// <param name="ascii">The string to return</param>
		/// <returns>The converted string</returns>
		public string Convert(string ascii)
		{
			return Util.Collect(ascii.Select(ch => ConvertChar(ch)));
		}

		/// <summary>
		/// Allows the program to send further parameters and options contained within
		/// a string to the Converter. Converters are not required to take any options.
		/// </summary>
		/// <param name="args">The options to implement</param>
		public virtual void SetOption(string[] args)
		{
			//Do nothing
		}

		/// <summary>
		/// Returns a given UnicodeConverter given the Type. Throws an Exception if the reference is invalid.
		/// </summary>
		/// <param name="type">The Type of Converter desired</param>
		/// <returns>An instance of an appropriate converter</returns>
		public static Converter GetConverter(ConverterType type)
		{
			switch (type)
			{
				case ConverterType.Dummy:
					return new DummyConverter();
				case ConverterType.Fullwidth:
					return new FullwidthConverter();
				case ConverterType.ScriptBold:
					return new ScriptBoldConverter();
				case ConverterType.FrakturBold:
					return new FrakturConverter();
				case ConverterType.Monospace:
					return new MonospaceConverter();
				default:
					throw new Exception("Can't find appropriate converter!");
			}
		}

		public string GetName()
		{
			return Util.GetName(ConversionType);
		}

		/// <summary>
		/// The Type of Converter
		/// </summary>
		public ConverterType ConversionType { get; protected set; }
	}

	/// <summary>
	/// Basic implementation for a converter, performs the identity function.
	/// </summary>
	public class DummyConverter : Converter
	{
		public DummyConverter()
		{
			ConversionType = ConverterType.Dummy;
		}

		public override string ConvertChar(char ascii)
		{
			return char.ConvertFromUtf32(ascii);
		}

		public override bool CanConvertChar(char ascii)
		{
			return true;
		}
	}

	/// <summary>
	/// Basic implementation for a Converter, allowing it to print fullwidth characters.
	/// </summary>
	public class FullwidthConverter : Converter
	{
		private ICollection<ConversionRule> conversionRules;

		public FullwidthConverter()
		{
			ConversionType = ConverterType.Fullwidth;
			InitConversionRules();
		}

		private void InitConversionRules()
		{
			conversionRules = new List<ConversionRule>();
			conversionRules.Add(new ConversionRule('\u0021', '\u007e', 0xFEE0)); //ASCII Characters
			conversionRules.Add(new ConversionRule(' ', '\u3000')); //Space.
		}

		public override string ConvertChar(char ch)
		{
			if (CanConvertChar(ch))
			{
				return Util.Find(conversionRules, ch).Convert(ch);
			}
			else if (Program.allowWrongInput)
			{
				return char.ConvertFromUtf32(ch);
			}
			else throw new CannotConvertException();
		}

		public override bool CanConvertChar(char ch)
		{
			return Util.Find(conversionRules, ch) != null;
		}

	}

}
