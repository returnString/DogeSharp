using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Microsoft.CSharp;

// ANTLR marks its own stuff as CLS compliant, gotta avoid warnings
[assembly: CLSCompliant(false)]

namespace DogeSharp
{
	public static class Program
	{
		private delegate string Helper(params string[] strings);

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
			try
			{
				MainAsync(args).Wait();
			}
			catch (AggregateException ex)
			{
				Log("much internal error: {0}", ex.InnerException);
			}

			if (Debugger.IsAttached)
				Console.ReadLine();
		}

		public static async Task MainAsync(string[] args)
		{
			Console.WriteLine(AsciiArt);

			try
			{
				m_bufferWidth = Console.BufferWidth / 2;
			}
			catch (IOException)
			{
				m_bufferWidth = 40;
			}

			Log("many D#");
			Log("very © Ruan Pearce-Authers");
			Log("much programming");
			Log("such language");

			var parsedArgs = args.Select(a => a.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries));
			var files = parsedArgs.Where(t => t.Length != 2 && !t[0].StartsWith("/")).Select(f => f[0]).ToArray();

			if (files.Length == 0)
			{
				Log("very files, much unspecified");
				return;
			}

			Helper getArg = names =>
			{
				foreach (var option in parsedArgs)
				{
					foreach (var name in names)
					{
						if (option[0].ToLower() == name)
							return option.Length > 1 ? option[1] : string.Empty;
					}
				}
				return null;
			};

			var targetType = GetTarget(getArg("/target"));
			var targetName = getArg("/out") ?? Path.GetFileNameWithoutExtension(files[0]) + "." + targetType;

			var timer = Stopwatch.StartNew();
			var tasks = new List<Task<string>>();

			Log("very translating...");

			var preserveTranslated = getArg("/preservetranslated") != null;
			var debug = getArg("/debug") != null;

			foreach (var filename in files)
			{
				// Silly closure semantics
				var actual = filename;

				tasks.Add(Task.Run(async () =>
				{
					using (var file = File.OpenRead(actual))
					{
						var input = new AntlrInputStream(file);
						var lexer = new DogeSharpLexer(input);
						var tokens = new CommonTokenStream(lexer);
						var parser = new DogeSharpParser(tokens);

						Log("much translation: {0}", actual);

						var visitor = new DogeToCSTranslator(actual);
						var text = visitor.Visit(parser.prog());

						if (preserveTranslated)
						{
							using (var translated = File.Open(actual.Replace(".ds", ".cs"), FileMode.Create))
							using (var stream = new StreamWriter(translated))
								await stream.WriteAsync(text);
						}

						return text;
					}
				}));
			}

			var sources = await Task.WhenAll(tasks);

			Helper forwardArgs = names =>
			{
				var temp = new List<string>();
				foreach (var name in names)
				{
					var value = getArg(name);
					if (value != null)
					{
						var full = name;
						if (!string.IsNullOrEmpty(value))
							full += ":" + value;

						temp.Add(full);
					}
				}
				return string.Join(" ", temp);
			};

			var forwardedOptions = forwardArgs("/optimize", "/target");

			if (!string.IsNullOrEmpty(forwardedOptions))
				Log("so forwarding: {0}", forwardedOptions);

			var assemblies = parsedArgs.Where(t => t[0] == "/reference").Select(t => t[1]).ToArray();

			Log("very references: {0}", string.Join(", ", assemblies));

			var provider = new CSharpCodeProvider();
			var options = new CompilerParameters(assemblies, targetName, debug)
			{
				GenerateExecutable = targetType == "exe" || targetType == "winexe",
				CompilerOptions = forwardedOptions
			};

			Log("such translated, wow");
			Log("many compiling...");

			var results = provider.CompileAssemblyFromSource(options, sources.ToArray());

			Log("such compiled, wow");
			Log("many time, such elapsed, so milliseconds: {0}", timer.ElapsedMilliseconds);

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
		}

		private static Random m_random = new Random();
		private static int m_bufferWidth;

		private static void Log(string message, params object[] args)
		{
			var indent = new string(' ', m_random.Next(m_bufferWidth));
			var msg = indent + message;

			if (args.Length == 0)
				Console.WriteLine(msg);
			else
				Console.WriteLine(msg, args);

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
