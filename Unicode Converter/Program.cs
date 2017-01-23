using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnicodeConverter
{
	/*
	 * Basic Console-Input based Command-Line utility for converting a text string into
	 * various funky Unicode letterings such as Widespace and the Mathematical Alphanumeric Symbols Block.
	 * Results are stored in the clipboard, commands that begin with '/' are meta-commands
	 * and change the program, such as setting the conversion tool and/or performing some function
	 * in the program.
	 */
	class Program
	{
		private static bool quit = false;
		internal static bool allowWrongInput = true;
		private static Converter currentConverter = null;

		[STAThread]
		static void Main(string[] args)
		{
			string input;
			currentConverter = Converter.GetConverter(Util.ConverterType.Dummy);

			Console.WriteLine("\n{0}\n", Strings.WelcomeMsg);

			while (!quit)
			{
				input = Console.ReadLine();

				if (string.IsNullOrWhiteSpace(input)) continue;

				//Commands
				if (input[0] == '/' || input[0] == '\\')
				{
					
					string[] commandArgs = input.Split(' ');
					string command = commandArgs[0].Substring(1).ToLower();

					string qualifiedCommand = commandAssocs.FirstOrDefault(entry => entry.Value.Contains(command)).Key;

					if (qualifiedCommand == null)
					{
						//Couldn't recognize any command, trying to set the converter.
						Util.ConverterType type = Util.GetConverterTypeFromString(command);
						if (type == Util.ConverterType.Dummy)
						{
							Console.WriteLine("Invalid comamnd! Type /h for help!");
						}
						else
						{
							if (commandArgs.Length == 1)
							{
								SetConverter(new string[] { "set", command });
							}
							else
							{
								//Convert one String just once.
								var old = currentConverter;
								currentConverter = Converter.GetConverter(type);

								//build input now.
								StringBuilder toConvert = new StringBuilder(commandArgs[1]);
								for (int i = 2; i < commandArgs.Length; i++)
								{
									toConvert.Append(" " + commandArgs[i]);
								}

								ConvertString(toConvert.ToString());

								currentConverter = old;
							}
						}
					}
					else
					{
						commands[qualifiedCommand](commandArgs.Select(str => str.ToLower()).ToArray());
					}
				}
				else
				{				
					//Read User String
					//This should really be refactored to give the user more info on what went wrong.
					ConvertString(input);
				}
			}
		}

		/// <summary>
		/// Associates an input string to its command
		/// </summary>
		static IDictionary<string, IEnumerable<string>> commandAssocs = new Dictionary<string, IEnumerable<string>> ()
		{
			{ "quit", new List<string>() { "quit", "q", "exit" } },
			{ "set", new List<string>() { "set", "converter" } },
			{ "help", new List<string>() { "help", "h" } },
			{ "list" , new List<string>() { "list", "l", "all" } }
		};

		/// <summary>
		/// Associates a console command with an appropriate action.
		/// </summary>
		static IDictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>()
		{
			{ "quit", ExitProgram },
			{ "set", SetConverter },
			{ "help", DisplayHelp },
			{ "list", ListConverters }
		};

		//Actions

		private static void ExitProgram(string[] args)
		{
			quit = true;
		}

		private static void SetConverter(string[] args)
		{
			if (args.Length <= 1)
			{
				Console.WriteLine("Currently using: {0}", currentConverter.GetName());
				return;
			}

			Util.ConverterType type = Util.GetConverterTypeFromString(args[1]);
			string outputLine = type == Util.ConverterType.Dummy ?
				"Could not find specified converter. Using default instead." :
				$"Using {type.GetName()}...";
			Console.WriteLine(outputLine);
			currentConverter = Converter.GetConverter(type);
		}

		private static void DisplayHelp(string[] args)
		{
			//Displays basic help. If args doesn't contain anything except help,
			//then generic help. Else help specific to the entered command.
			Console.WriteLine();
			if (args.Length == 1)
			{
				ListCommands();
			}
			else try
			{
				string queriedCommand = commandAssocs.FirstOrDefault(entry => entry.Value.Contains(args[1])).Key ?? "";
				string[] helpMessage = Strings.HelpTopics[queriedCommand];
				var aliases = commandAssocs[queriedCommand].ToArray();

				Console.WriteLine("{0}:", queriedCommand.ToUpper());
				Console.WriteLine("{0}\n", helpMessage[0]);

				Console.Write("Aliases: ");

				Console.Write(aliases[0]);
				for (int i = 1; i < aliases.Length; i++)
				{
					Console.Write(", {0}", aliases[i]);
				}
				Console.WriteLine();

				for (var i = 1; i < helpMessage.Length; i++)
				{
					Console.WriteLine(helpMessage[i]);
				}
				Console.WriteLine();
			}
			catch (KeyNotFoundException)
			{
				Console.WriteLine("Couldn't find specified command!");
			}
		}

		private static void ListCommands()
		{
			Console.WriteLine("Help Commands:\n");
			foreach (var assoc in Strings.HelpTopics)
			{
				Console.WriteLine("{0}: {1}", assoc.Key, assoc.Value[1]);
			}
			Console.WriteLine();
		}

		private static void ConvertString(string input)
		{
			try
			{
				string result = currentConverter.Convert(input);
				Console.WriteLine(">>> {0}", result);
				Clipboard.SetText(result);
			}
			catch (CannotConvertException)
			{
				Console.WriteLine("Couldn't convert this line. Certain characters not supported.");
			}
		}

		private static void ListConverters(string[] args)
		{
			Console.WriteLine("Available Converters:\n");
			foreach (var ctype in Strings.converterAssocs)
			{
				Console.Write(ctype.Key.GetName() + ": ");
				Console.Write(ctype.Value[0]);
				for (int i = 1; i < ctype.Value.Count; i++)
				{
					Console.Write(", {0}", ctype.Value[i]);
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}
	}
}
