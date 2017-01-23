using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace UnicodeConverter
{
	/// <summary>
	/// Contains basic constant Strings to display to the user, along with Dictionaries and Associations
	/// to identify various strings as belonging to various components.
	/// </summary>
	public static class Strings
	{
		public const string WELCOMEMSG = 
			"Welcome to ASCII-Converter v 2.0.\n" + 
			"Type /h for help. Type /q for quit.\n" +
			"Results are stored in the clipboard.";

		/// <summary>
		/// Used for matching a name with a given converter.
		/// </summary>
		public static readonly IDictionary<Util.ConverterType, List<string>> converterAssocs = new Dictionary<Util.ConverterType, List<string>>()
		{
			{ Util.ConverterType.DUMMY, new List<string>() { "dummy", "d" } },
			{ Util.ConverterType.FULLWIDTH, new List<string>() {"fullwidth", "full width", "weaboo", "fw", "f"} },
			{ Util.ConverterType.SCRIPTBOLD, new List<string>() {"scriptbold", "sb" } },
			{ Util.ConverterType.FRAKTURBOLD, new List<string>() {"frakturbold", "fraktur", "fb", "fr" } },
			{ Util.ConverterType.MONOSPACE, new List<string>() {"monospace", "ms", "mono"} }
		};

		private static readonly string[] HELPMSG_HELP =
		{
			"/<help> <command?>",
			"Displays a short help notice for the given topic",
			"If no command is specified, it lists off all possible user commands"
		};

		private static readonly string[] HELPMSG_LIST =
		{
			"/<list>",
			"Lists all available converters and their aliases."
		};

		private static readonly string[] HELPMSG_QUIT =
		{
			"/<quit>",
			"Exits the program"
		};

		public static readonly IDictionary<string, string[]> HELPTOPICS = new Dictionary<string, string[]>()
		{
			{ "help", HELPMSG_HELP },
			{ "list", HELPMSG_LIST },
			{ "quit", HELPMSG_QUIT }
		};

	}

	/// <summary>
	/// Basic Exception class used if a Converter could be found, but converting the 
	/// given string is not possible.
	/// </summary>
	[Serializable()]
	public class CannotConvertException : Exception
	{
		public CannotConvertException() : base() { }

		public CannotConvertException(string input, int index) : 
			base(String.Format("Couldn't convert {0} at index {1}!", input, index)) { }

		public CannotConvertException(string input, int index, Exception inner) :
			base(String.Format("Couldn't convert {0} at index {1}!", input, index), inner) { }

		protected CannotConvertException(SerializationInfo info, StreamingContext context) :
			base(info, context) { }
	}

	/// <summary>
	/// Basic Utility class to contain constant strings,
	/// enum types and what have you.
	/// </summary>
	public static class Util
	{
		public enum ConverterType
		{
			DUMMY,
			FULLWIDTH,
			SCRIPTBOLD,
			FRAKTURBOLD,
			MONOSPACE
		}

		public static string GetName(this ConverterType value)
		{
			switch (value)
			{
				case ConverterType.DUMMY:
					return "Default";
				case ConverterType.FULLWIDTH:
					return "Full Width Encoding";
				case ConverterType.SCRIPTBOLD:
					return "Mathematical Script Bold";
				case ConverterType.FRAKTURBOLD:
					return "Fraktur Bold";
				case ConverterType.MONOSPACE:
					return "Monospace";
				default:
					return "Null";
			}
		}

		/// <summary>
		/// Returns a converter from a matching string.
		/// </summary>
		/// <param name="name">The name of the converter to look for</param>
		/// <returns>The converter type if one is found, null otherwise</returns>
		public static ConverterType GetConverterTypeFromString(string name)
		{
			ConverterType? key = Strings.converterAssocs.FirstOrDefault(entry => entry.Value.Contains(name)).Key;
			return key ?? ConverterType.DUMMY;
		}

		/// <summary>
		/// Returns a string from an IEnumarble of characters.
		/// </summary>
		/// <param name="chars">The characters to turn into a string</param>
		/// <returns></returns>
		public static string Collect(IEnumerable<string> codePoints)
		{
			StringBuilder sb = new StringBuilder(codePoints.Count());
			foreach (string ch in codePoints)
			{
				sb.Append(ch);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Finds the last Conversion Rule appropriate for a given character and returns it.
		/// This allows Conversion rules to be initialized in a "generic first, specific later" way
		/// regarding overlap.
		/// </summary>
		/// <param name="crList">The List of Conversion Rules</param>
		/// <param name="ch">The character to check for</param>
		/// <returns>The Conversion Rule that handles the given character. Null if it does not exist.</returns>
		public static ConversionRule Find(IEnumerable<ConversionRule> crList, char ch)
		{
			return crList.LastOrDefault(cr => cr.CoveredCharacters.Contains(ch));
		}

		/// <summary>
		/// Adds the Conversion Rule to the List of Conversion Rules if it can.
		/// Otherwise, if there is overlap, an exception is thrown.
		/// </summary>
		/// <param name="cr">The conversion rule to add</param>
		/// <param name="crList">The conversion rule to add into</param>
		public static void Add(ConversionRule cr, ICollection<ConversionRule> crList)
		{
			crList.Add(cr);
		}
	}

	/// <summary>
	/// A conversion rule is a basic class that allows to perform calculations
	/// on a single character. Conversion Rules can operate either on ranges
	/// or on single characters and return either an offset or a fixed code point.
	/// </summary>
	public class ConversionRule
	{
		/// <summary>
		/// Is true if the character is converted according to an offset, false otherwise.
		/// </summary>
		public bool Relative { get ; private set ; }
		
#region relative
		/*
		 * Only used in relative conversion. 
		 */

		public int Min { get; private set; }
		public int Max { get; private set; }

		public char MinChar {
			get { return (char)Min; }
			set { }
		}

		public char MaxChar {
			get { return (char)Max; }
			set { }
		}

		public int Offset { get; private set; }
#endregion

#region absolute
		/*
		 * Only used in absolute conversion
		 */
		public char ConvertFrom { get; private set; }
		public char ConvertTo { get; private set; }
#endregion

		/// <summary>
		/// Constructs a ConversionRule using relative offset.
		/// </summary>
		/// <param name="from">The beginning of the range</param>
		/// <param name="to">The end of the range</param>
		/// <param name="offset">The amount of characters to shift by</param>
		public ConversionRule(int from, int to, int offset)
		{
			Relative = true;
			Min = from;
			Max = to;
			Offset = offset;
		}

		/// <summary>
		/// Constructs a ConversionRule using absolute offset.
		/// </summary>
		/// <param name="from">The character to convert from.</param>
		/// <param name="to">The character to convert to.</param>
		public ConversionRule(char from, char to)
		{
			Relative = false;
			ConvertFrom = from;
			ConvertTo = to;

			Offset = ConvertTo - ConvertFrom;
			Min = Max = ConvertFrom;
		}

		/// <summary>
		/// Returns if the character can be converted with this converter or not.
		/// </summary>
		/// <param name="ch">The character to test</param>
		/// <returns>True if a conversion is possible, false otherwise.</returns>
		public Boolean CanConvert(char ch)
		{
			return (Min <= ch && ch <= Max);
		}

		/// <summary>
		/// Converts the Character.
		/// </summary>
		/// <param name="ch">The character to convert.</param>
		/// <returns>The Covnerter character.</returns>
		public string Convert(char ch)
		{
			if (!CanConvert(ch))
			{
				throw new ArgumentException("Char not in range!");
			}

			return Char.ConvertFromUtf32(ch + Offset);
		}

		/// <summary>
		/// Returns the range of Characters this Converter can take.
		/// </summary>
		public char[] CoveredCharacters
		{
			get
			{
				int size = 1 + (Max - Min);
				char[] chars = new char[size];

				for (int i = 0; i < size; i++)
				{
					chars[i] = (char)(Min + i);
				}

				return chars;
			}
			set { }
		}

		/// <summary>
		/// Returns a String representation of this Conversion rule in this form
		/// [CHARS_FROM] -> [CHARS_TO], where CHARS_FROM is the list of characters that are converted
		/// and CHARS_TO is the list of resulting characters.
		/// </summary>
		/// <returns>A string representation of this conversion rule.</returns>
		public override string ToString()
		{
			StringBuilder sbChar = new StringBuilder("[");
			StringBuilder sbConverted = new StringBuilder("[");

			char[] chars = CoveredCharacters;
			string[] convertedChars = chars.Select(ch => Convert(ch)).ToArray();

			sbChar.Append(chars[0]);
			sbConverted.Append(convertedChars[0]);

			for(int i = 1; i < chars.Length; i++)
			{
				sbChar.Append(String.Format(" {0}", chars[i]));
				sbConverted.Append(String.Format(" {0}", convertedChars[i]));
			}

			sbChar.Append(']');
			sbConverted.Append(']');

			return String.Format("{0} -> {1}", sbChar.ToString(), sbConverted.ToString());
		}

		/// <summary>
		/// Convenience method to get two Conversion Rules for the Alphabet, one for lowercase letters,
		/// one for uppercase letters.
		/// </summary>
		/// <param name="codePoint">The Code Point the a starts with.</param>
		/// <param name="offset">If true, this is an offset from an uppercase 'A', else, the code point is absolute.</param>
		/// <returns>A lsit with the new conversion rules</returns>
		public static IEnumerable<ConversionRule> FromAlphabet(int codePoint, bool offset)
		{
			if (!offset) codePoint -= 'A';

			return new List<ConversionRule>()
			{
				new ConversionRule('A', 'Z', codePoint),
				new ConversionRule('a', 'z', codePoint - 6) //Account for the 6 extra code points between 'Z' and 'a' in ASCII
			};
		}
	}
}
