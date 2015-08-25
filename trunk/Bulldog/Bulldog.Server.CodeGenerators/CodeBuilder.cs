using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bulldog.Server.CodeGenerators
{
	public class CodeBuilder
	{
		public override string ToString()
		{
			return this.Builder.ToString();
		}

		public readonly StringBuilder Builder = new StringBuilder();

		public int Indent;

		public void WriteIndent()
		{
			for (int i = 0; i < Indent; i++)
			{
				this.Builder.Append("\t");
			}
		}

		public enum Keywords
		{
			@abstract,
			@return,
			@public,
			@interface,
			@class,
			@namespace,
			@protected,
			@static,
			@default,
			@override,
		}

		public void WriteKeywordSpace(Keywords k)
		{
			WriteKeyword(k);
			this.Builder.Append(" ");
		}

		public void WriteKeyword(Keywords k)
		{
			if (k == Keywords.@abstract)
			{
				this.Builder.Append("abstract");
				return;
			}

			if (k == Keywords.@return)
			{
				this.Builder.Append("return");
				return;
			}
			
			if (k == Keywords.@public)
			{
				this.Builder.Append("public");
				return;
			}

			if (k == Keywords.@interface)
			{
				this.Builder.Append("interface");
				return;
			}

			if (k == Keywords.@class)
			{
				this.Builder.Append("class");
				return;
			}

			if (k == Keywords.@namespace)
			{
				this.Builder.Append("namespace");
				return;
			}

			if (k == Keywords.@protected)
			{
				this.Builder.Append("protected");
				return;
			}

			if (k == Keywords.@static)
			{
				this.Builder.Append("static");
				return;
			}

			if (k == Keywords.@default)
			{
				this.Builder.Append("default");
				return;
			}

			if (k == Keywords.@override)
			{
				this.Builder.Append("override");
				return;
			}
		}

		public void Write(string e)
		{
			WriteIndent();
			Builder.Append(e);
		}

		public void WriteLine()
		{
			WriteLine("");
		}

		public void WriteLine(string e)
		{
			WriteIndent();
			Builder.AppendLine(e);
		}

		public IDisposable Block()
		{
			return new BlockBuilder(this);
		}

		public class BlockBuilder : IDisposable
		{
			readonly CodeBuilder Context;
			public BlockBuilder(CodeBuilder Context)
			{
				this.Context = Context;
				this.Context.WriteLine("{");
				this.Context.Indent++;
			}

			#region IDisposable Members

			public void Dispose()
			{
				this.Context.Indent--;
				this.Context.WriteLine("}");
			}

			#endregion
		}


		public IDisposable Parameters()
		{
			return new ParametersBuilder(this);
		}

		public class ParametersBuilder : IDisposable
		{
			readonly CodeBuilder Context;
			public ParametersBuilder(CodeBuilder Context)
			{
				this.Context = Context;
				this.Context.Builder.Append("(");
			}

			#region IDisposable Members

			public void Dispose()
			{
				this.Context.Builder.Append(")");
			}

			#endregion
		}

		public void WriteUsing(string e)
		{
			e = EscapeKeywordsWithinNamespace(e);

			WriteLine("using " + e + ";");
		}

		public string EscapeKeywordsWithinNamespace(string e)
		{
			var segments = e.Split('.');

			for (int i = 0; i < segments.Length; i++)
			{
				if (IsKeyword(segments[i]))
					segments[i] = "@" + segments[i];
			}

			e = segments[0];

			for (int i = 1; i < segments.Length; i++)
			{
				e += "." + segments[i];
			}
			return e;
		}

		public void WriteSummary(string t)
		{
			this.WriteLine("/// <summary>");
			foreach (var s in t.Split('\n'))
			{
				this.WriteLine("/// " + s.Trim());
			}
			this.WriteLine("/// </summary>");
		}

		public bool IsKeyword(string e)
		{
			if (e == "event")
				return true;

			return false;
		}
	}
}
