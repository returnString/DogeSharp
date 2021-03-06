﻿using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

namespace DogeSharp
{
	public class DogeToCSTranslator : DogeSharpBaseVisitor<string>
	{
		const string QuoteStr = "\"";

		private readonly string m_filename;

		public DogeToCSTranslator(string filename)
		{
			m_filename = filename;
		}

		public override string VisitCall(DogeSharpParser.CallContext context)
		{
			var name = context.ID.Text;
			var expr = context.expr();

			// Skip the left side for the arg list
			if (context.Expr != null)
				expr = expr.Skip(1).ToList();

			var args = expr.Select(e => Visit(e));

			var call = string.Format("{0}({1})", name, string.Join(",", args));
			var prefix = context.Expr != null ? Visit(context.Expr) + "." : "";

			switch (context.Pre.Text)
			{
				// Call
				case "plz":
					return prefix + call;

				// Ctor
				case "gimme":
					return string.Format("new {0}{1}", prefix, call);

				default:
					throw new CompilerException(context.Pre, "very unexpect, much token");
			}
		}

		public override string VisitDeclare(DogeSharpParser.DeclareContext context)
		{
			return string.Format("var {0} = {1}", context.ID.Text, Visit(context.Expr));
		}

		public override string VisitNumber(DogeSharpParser.NumberContext context)
		{
			return context.Value.Text;
		}

		public override string VisitString(DogeSharpParser.StringContext context)
		{
			// It works, and that's what important... right? Need to learn ANTLR properly.
			return QuoteStr + string.Join(" ", context.children.Select(c => c.GetText()).Skip(1).TakeWhile(s => s != QuoteStr)) + QuoteStr;
		}

		public override string VisitIdent(DogeSharpParser.IdentContext context)
		{
			return context.Value.Text;
		}

		public override string VisitPrint(DogeSharpParser.PrintContext context)
		{
			// Need a full reference in case System isn't in scope
			return "System.Console.WriteLine(" + Visit(context.Expr) + ")";
		}

		// TODO: Automatic ctor generation, structs
		public override string VisitDeclareClass(DogeSharpParser.DeclareClassContext context)
		{
			var props = context.classProperty().Select(p => Visit(p));
			var methods = context.declareFunction().Select(m => Visit(m));
			var inherits = context.Ident().Skip(1).Select(i => i.ToString());
			var inheritStr = string.Join(", ", inherits);
			if (!string.IsNullOrEmpty(inheritStr))
				inheritStr = ": " + inheritStr;

			var attributes = string.Join(" ", context.attribute().Select(a => Visit(a)));
			var modifier = context.Hidden != null ? "internal" : "public";

			return string.Format("{0} {1} class {2} {3} {{ {4} {5} }}", attributes, modifier, context.ID.Text, inheritStr,
				string.Join(Environment.NewLine, props), string.Join(Environment.NewLine, methods));
		}

		public override string VisitDeclareFunction(DogeSharpParser.DeclareFunctionContext context)
		{
			var skip = 1;
			var name = context.ID.Text;
			
			var attributes = string.Join(" ", context.attribute().Select(a => Visit(a)));

			var returnType = "";
			if (context.ReturnType != null)
			{
				returnType = context.ReturnType.Text;
				skip += 1;
			}

			var modifiers = string.Join(" ", context.Modifier().Select(m => m.Symbol.Text));

			var prmString = "";
			var idents = context.Ident();

			for (var i = skip; i < idents.Count; i += 2)
			{
				if (i != skip)
					prmString += ",";

				prmString += string.Format("{0} {1} ", idents[i], idents[i + 1]);
			}

			var block = Visit(context.scopeBlock());

			return string.Format("{0} {1} {2} {3}({4}) {5}",
				attributes, modifiers, returnType, name, prmString, block);
		}

		public override string VisitGetField(DogeSharpParser.GetFieldContext context)
		{
			return context.ID.Text;
		}

		public override string VisitOperation(DogeSharpParser.OperationContext context)
		{
			var ret = Visit(context.Left);
			var operators = context.Operator();

			for (var i = 0; i < operators.Count; i++)
			{
				var op = operators[i];
				var expr = context.expr(i + 1);

				ret += op.GetText() + Visit(expr);
			}

			return ret;
		}

		public override string VisitUseNamespace(DogeSharpParser.UseNamespaceContext context)
		{
			return "using " + context.ID.Text + ";";
		}

		public override string VisitClassProperty(DogeSharpParser.ClassPropertyContext context)
		{
			var access = "private";
			var getAccess = "";
			var setAccess = "";

			foreach (var modifier in context.Modifier())
			{
				switch (modifier.GetText())
				{
					case "static":
						access += " static";
						break;

					case "readonly":
						access = "public";
						setAccess = "private";
						break;
				}
			}

			var attributes = string.Join(" ", context.attribute().Select(a => Visit(a)));

			return string.Format("{0} {1} {2} {3} {{ {4} get; {5} set; }}",
				attributes, access, context.Type.Text, context.Name.Text, getAccess, setAccess);
		}

		public override string VisitReturn(DogeSharpParser.ReturnContext context)
		{
			if (context.Expr == null)
				return "return";

			return "return " + Visit(context.Expr);
		}

		public override string VisitProg(DogeSharpParser.ProgContext context)
		{
			var namespaces = context.useNamespace().Select(n => Visit(n));
			var classes = context.declareClass().Select(c => Visit(c));

			return string.Join("", namespaces) + string.Join("", classes);
		}

		public override string VisitAssign(DogeSharpParser.AssignContext context)
		{
			return Visit(context.Left) + '=' + Visit(context.Right);
		}

		public override string VisitHandleEvent(DogeSharpParser.HandleEventContext context)
		{
			return Visit(context.Left) + "+=" + Visit(context.Right);
		}

		public override string VisitAttribute(DogeSharpParser.AttributeContext context)
		{
			return string.Format("[{0}({1})]", context.ID.Text, string.Join(",", context.expr().Select(e => Visit(e))));
		}

		public override string VisitAwait(DogeSharpParser.AwaitContext context)
		{
			return "await " + Visit(context.Expr);
		}

		public override string VisitUsing(DogeSharpParser.UsingContext context)
		{
			return string.Format("using ({0}) {1}", Visit(context.Expr), Visit(context.scopeBlock()));
		}

		public override string VisitScopeBlock(DogeSharpParser.ScopeBlockContext context)
		{
			return string.Format("{{ {0} }}", CreateBlock(context.stmt(), context.block(), context.conditional()));
		}

		public override string VisitStmt(DogeSharpParser.StmtContext context)
		{
			var exprs = context.expr().Select(c => new { Line = c.Start.Line, Text = Visit(c) }).Where(e => !string.IsNullOrWhiteSpace(e.Text)).ToArray();
			var statements = exprs.Select(e => string.Format("{0}#line {1} \"{2}\" {0} {3};", Environment.NewLine, e.Line, m_filename, e.Text));
			return string.Join(" ", statements);
		}

		public override string VisitLock(DogeSharpParser.LockContext context)
		{
			var block = Visit(context.scopeBlock());
			return string.Format("lock ({0}) {{ {1} }}", context.ID.Text, block);
		}

		public override string VisitConditional(DogeSharpParser.ConditionalContext context)
		{
			var extras = string.Join(" ", context.conditionalElse().Select(c => Visit(c)));
			return string.Format("if ({0}) {1} {2}", Visit(context.expr()), Visit(context.scopeBlock()), extras);
		}

		public override string VisitConditionalElse(DogeSharpParser.ConditionalElseContext context)
		{
			var start = "else";
			if (context.Pre.Text == "but rly")
			{
				start += string.Format(" if ({0})", Visit(context.expr()));
			}

			return string.Format("{0} {1}", start, Visit(context.scopeBlock()));
		}

		private string CreateBlock(params IEnumerable<ParserRuleContext>[] contexts)
		{
			var block = string.Empty;

			foreach (var entry in Group(contexts))
			{
				block += Visit(entry);
			}

			return block;
		}

		private IEnumerable<ParserRuleContext> Group(params IEnumerable<ParserRuleContext>[] contexts)
		{
			return contexts.SelectMany(c => c).OrderBy(c => c.Start.Line);
		}
	}
}
