using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using Microsoft.CSharp;

// ANTLR marks its own stuff as CLS compliant, gotta avoid warnings
[assembly: CLSCompliant(false)]

namespace DogeSharp
{
	public static class Program
	{
		const string AsciiArt =
@"
         ▄              ▄
        ▌▒█           ▄▀▒▌
        ▌▒▒█        ▄▀▒▒▒▐
       ▐▄▀▒▒▀▀▀▀▄▄▄▀▒▒▒▒▒▐
     ▄▄▀▒░▒▒▒▒▒▒▒▒▒█▒▒▄█▒▐
   ▄▀▒▒▒░░░▒▒▒░░░▒▒▒▀██▀▒▌
  ▐▒▒▒▄▄▒▒▒▒░░░▒▒▒▒▒▒▒▀▄▒▒▌
  ▌░░▌█▀▒▒▒▒▒▄▀█▄▒▒▒▒▒▒▒█▒▐
 ▐░░░▒▒▒▒▒▒▒▒▌██▀▒▒░░░▒▒▒▀▄▌
 ▌░▒▄██▄▒▒▒▒▒▒▒▒▒░░░░░░▒▒▒▒▌
▀▒▀▐▄█▄█▌▄░▀▒▒░░░░░░░░░░▒▒▒▐
▐▒▒▐▀▐▀▒░▄▄▒▄▒▒▒▒▒▒░▒░▒░▒▒▒▒▌
▐▒▒▒▀▀▄▄▒▒▒▄▒▒▒▒▒▒▒▒░▒░▒░▒▒▐
 ▌▒▒▒▒▒▒▀▀▀▒▒▒▒▒▒░▒░▒░▒░▒▒▒▌
 ▐▒▒▒▒▒▒▒▒▒▒▒▒▒▒░▒░▒░▒▒▄▒▒▐
  ▀▄▒▒▒▒▒▒▒▒▒▒▒░▒░▒░▒▄▒▒▒▒▌
    ▀▄▒▒▒▒▒▒▒▒▒▒▄▄▄▀▒▒▒▒▄▀
      ▀▄▄▄▄▄▄▀▀▀▒▒▒▒▒▄▄▀
         ▒▒▒▒▒▒▒▒▒▒▀▀

";

		public static void Main(string[] args)
		{
			Console.WriteLine(AsciiArt);

			Log("many D#");
			Log("very © Ruan Pearce-Authers");
			Log("much programming");
			Log("such language");

			var parsedArgs = args.Select(a => a.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries));
			var files = parsedArgs.Where(t => t.Length != 2).Select(f => f[0]).ToArray();

			if (files.Length == 0)
				return;

			Func<string, string> getArg = name =>
			{
				foreach (var option in parsedArgs.Where(t => t.Length == 2))
				{
					if (option[0].ToLower() == name)
						return option[1];
				}
				return null;
			};

			var targetType = GetTarget(getArg("/target"));
			var targetName = getArg("/out") ?? Path.GetFileNameWithoutExtension(files[0]) + "." + targetType;

			var timer = Stopwatch.StartNew();
			var visitor = new DogeToCSTranslator();
			var sources = new List<string>();

			Log("very translating...");

			var preserveTranslated = getArg("/preservetranslated") != null;

			foreach (var filename in files)
			{
				using (var file = File.OpenRead(filename))
				{
					var input = new AntlrInputStream(file);
					var lexer = new DogeSharpLexer(input);
					var tokens = new CommonTokenStream(lexer);
					var parser = new DogeSharpParser(tokens);

					Log("much translation: {0}", filename);

					var text = visitor.Visit(parser.prog());

					if (preserveTranslated)
						File.WriteAllText(filename.Replace(".ds", ".cs"), text);

					sources.Add(text);
				}
			}

			var provider = new CSharpCodeProvider();
			var options = new CompilerParameters(new string[0], targetName, true)
			{
				GenerateExecutable = targetType == "exe" || targetType == "winexe"
			};

			Log("such translated, wow");
			Log("many compiling...");

			var results = provider.CompileAssemblyFromSource(options, sources.ToArray());

			Log("such compiled, wow: {0}", timer.Elapsed);

			if (results.Errors.HasErrors)
			{
				Log("so errors:");
				foreach (var error in results.Errors)
				{
					Log(error.ToString());
				}
			}
			else
			{
				Log("many success: {0}", results.PathToAssembly);
			}

			if (Debugger.IsAttached)
				Console.ReadLine();
		}

		private static Random m_random = new Random();

		private static void Log(string message, params object[] args)
		{
			var indent = new string(' ', m_random.Next(Console.BufferWidth / 2));
			Console.WriteLine(indent + message, args);
			Console.WriteLine();
		}

		private static string GetTarget(string option)
		{
			switch (option)
			{
				case "library":
					return "dll";

				case "winexe":
				case "exe":
				case null:
					return "exe";

				default:
					throw new CompilerException("many target, much invalid");
			}
		}
	}
}
