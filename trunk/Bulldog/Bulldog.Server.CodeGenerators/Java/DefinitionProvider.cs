using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bulldog.Server.CodeGenerators.Java
{
	public partial class DefinitionProvider
	{
		// to fix:
		// 1. namespace ".event"
		// 2. abstract method
		// 3. inheritance
		// 4. array parameters

		public readonly Uri Source;
		public readonly string DataString;

		public readonly TypeInfo Type;

		public delegate string ToWebStringDelegate(Uri e);

		public DefinitionProvider(TypeInfo Type, ToWebStringDelegate ToWebString)
		{
			// http://java.sun.com/j2se/1.4.2/docs/api/org/w3c/dom/Document.html

			this.Type = Type;

			// http://robocode.sourceforge.net/docs/robocode/robocode/BattleRules.html

			if (this.Type.FullName.StartsWith("javax.servlet"))
			{
				Source = new Uri("http://java.sun.com/webservices/docs/1.6/api/" + this.Type.FullName.Replace(".", "/") + ".html");
			}
			else if (this.Type.FullName.StartsWith("robocode."))
			{
				Source = new Uri("http://robocode.sourceforge.net/docs/robocode/" + this.Type.FullName.Replace(".", "/") + ".html");
			}
			else if (this.Type.FullName.StartsWith("javax.jdo"))
			{
				Source = new Uri("http://download.oracle.com/docs/cd/E13189_01/kodo/docs341/jdo-javadoc/" + this.Type.FullName.Replace(".", "/") + ".html");
			}
			else
			{
				Source = new Uri("http://java.sun.com/j2se/1.4.2/docs/api/" + this.Type.FullName.Replace(".", "/") + ".html");
			}

			DataString = ToWebString.Invoke(Source);

			var BaseTypesHint1 = "<B>" + Type.Name + "</B>";
			var BaseTypesHint2 = "<DT>";

			{
				var i = DataString.IndexOf(BaseTypesHint1);
				var j = DataString.Substring(0, i).LastIndexOf(BaseTypesHint2);
				var Modifiers = DataString.Substring(j + BaseTypesHint2.Length, i - (j + BaseTypesHint2.Length));

				this.Type.IsAbstract = Modifiers.Contains("abstract");
				this.Type.IsInterface = Modifiers.Contains("interface");
			}

			var BaseTypesHint3 = BaseTypesHint1 + "<DT>extends";
			var BaseTypesHint4 = "</A>";
			var BaseTypesHint5 = "<DT>implements";
			var BaseTypesHint6 = "</DL>";
			{
				var i = DataString.IndexOf(BaseTypesHint3);
				if (i >= 0)
				{
					i += BaseTypesHint3.Length;

					var j = DataString.IndexOf(BaseTypesHint4, i);

					if (j >= 0)
					{
						var BaseTypeString = DataString.Substring(i, j + BaseTypesHint4.Length - i);

						var jj = BaseTypeString.IndexOf(BaseTypesHint5);
						if (jj > 0)
						{
							BaseTypeString = BaseTypeString.Substring(0, jj);
						}

						jj = BaseTypeString.IndexOf(BaseTypesHint6);
						if (jj > 0)
						{
							BaseTypeString = BaseTypeString.Substring(0, jj);
						}
						var BaseType = new ParameterInfo();

						ParseParameter(BaseTypeString, BaseType, null, null, null);

						// dismiss object
						if (BaseType.Type.FullName == "java.lang.Object")
							BaseType.Type = null;

						Type.BaseType = BaseType.Type;

					}
				}
			}

			#region Constructors
			var ConstructorSummaryHint = "<A NAME=\"constructor_summary\">";
			var ConstructorSummaryStart = "<TABLE";
			var ConstructorSummaryEnd = "</TABLE>";

			var ConstructorSummary = default(string);

			{
				var ii = DataString.IndexOf(ConstructorSummaryHint);
				if (ii >= 0)
				{
					// robocode might not show constructors?

					var i = DataString.IndexOf(ConstructorSummaryStart, ii);
					var ic = DataString.IndexOf(ConstructorSummaryEnd, i + ConstructorSummaryStart.Length);

					ConstructorSummary = DataString.Substring(i, ic - i + ConstructorSummaryEnd.Length);
					ParseConstructorSummary(Type, ConstructorSummary);
				}
			}

			#endregion

			#region Methods
			var MethodSummaryHint = "<A NAME=\"method_summary\">";
			var MethodSummaryStart = "<TABLE";
			var MethodSummaryEnd = "</TABLE>";

			var MethodSummary = default(string);

			{
				var ii = DataString.IndexOf(MethodSummaryHint);
				if (ii >= 0)
				{
					var i = DataString.IndexOf(MethodSummaryStart, ii);
					var ic = DataString.IndexOf(MethodSummaryEnd, i + MethodSummaryStart.Length);

					MethodSummary = DataString.Substring(i, ic - i + MethodSummaryEnd.Length);
					ParseMethodSummary(Type, MethodSummary);
				}
			}

			#endregion

			//var x = this.GetString();
		}

		private static void ParseConstructorSummary(TypeInfo context, string ConstructorSummary)
		{
			var MethodDefinitionStart = "<TR BGCOLOR=\"white\" CLASS=\"TableRowColor\">";
			var MethodDefinitionEnd = "</TR>";

			{
				var i = ConstructorSummary.IndexOf(MethodDefinitionStart);


				while (i > -1)
				{
					var ic = ConstructorSummary.IndexOf(MethodDefinitionEnd, i + MethodDefinitionStart.Length);

					var ConstructorDefinition = ConstructorSummary.Substring(i, ic - i + MethodDefinitionEnd.Length);

					ParseConstructorDefinition(context, ConstructorDefinition);



					i = ConstructorSummary.IndexOf(MethodDefinitionStart, ic);
				}
			}
		}

		private static void ParseConstructorDefinition(TypeInfo context, string ConstructorDefinition)
		{
			var Signature = default(string);

			var TDStart = "<TD";
			var TDEnd = "</TD>";


			{

				var i = ConstructorDefinition.IndexOf(TDStart);


				while (i > -1)
				{
					var ic = ConstructorDefinition.IndexOf(TDEnd, i + TDStart.Length);

					var Text = ConstructorDefinition.Substring(i, ic - i + TDEnd.Length);

					var j = Text.IndexOf(">") + 1;
					Text = Text.Substring(j, Text.Length - TDEnd.Length - j);

					Signature = Text;


					i = ConstructorDefinition.IndexOf(TDStart, ic);
				}
			}

			var SummaryHint = "<BR>";

			var Summary = Signature.Substring(Signature.IndexOf(SummaryHint) + SummaryHint.Length);

			Signature = Signature.Substring(0, Signature.IndexOf(SummaryHint));

			ParseConstructorDefinition(context, Signature, Summary);
		}

		private static void ParseConstructorDefinition(TypeInfo context, string Signature, string Summary)
		{
			Summary = Summary.Replace("&nbsp;", " ").Trim();

			// <CODE><B><A HREF="../../../org/w3c/dom/Document.html#createAttributeNS(java.lang.String, java.lang.String)">createAttributeNS</A></B>(<A HREF="../../../java/lang/String.html" title="class in java.lang">String</A>&nbsp;namespaceURI,
			// <A HREF="../../../java/lang/String.html" title="class in java.lang">String</A>&nbsp;qualifiedName)</CODE>

			var ParametersHint = "</B>";
			var ParametersStart = "(";
			var ParametersEnd = ")";

			var i = Signature.IndexOf(ParametersStart, Signature.IndexOf(ParametersHint));
			var ic = Signature.IndexOf(ParametersEnd, i);

			var value = new MethodDefinitionInfo();
			value.Summary = Summary;

			Action IsProtected = () => value.IsProtected = true;
			Action IsStatic = () => value.IsStatic = true;
			// <FONT SIZE=\"-1\">\n<CODE>&nbsp;boolean</CODE></FONT>
			//ParseParameter(ReturnType, value.ReturnParameter, IsProtected, IsStatic);

			var Parameters = Signature.Substring(i + ParametersStart.Length, ic - i - ParametersStart.Length).Trim();

			if (Parameters.Length > 0)
				foreach (var Parameter in Parameters.Split(','))
				{
					// "<A HREF=\"../../../java/lang/String.html\" title=\"class in java.lang\">String</A>&nbsp;namespaceURI"

					var p = new ParameterInfo();

					ParseParameter(Parameter, p, null, null, null);

					value.Add(p);
				}


			context.AddConstructor(value);
		}


		private static void ParseMethodSummary(TypeInfo context, string MethodSummary)
		{
			var MethodDefinitionStart = "<TR BGCOLOR=\"white\" CLASS=\"TableRowColor\">";
			var MethodDefinitionEnd = "</TR>";

			{
				var i = MethodSummary.IndexOf(MethodDefinitionStart);


				while (i > -1)
				{
					var ic = MethodSummary.IndexOf(MethodDefinitionEnd, i + MethodDefinitionStart.Length);

					var MethodDefinition = MethodSummary.Substring(i, ic - i + MethodDefinitionEnd.Length);

					ParseMethodDefinition(context, MethodDefinition);



					i = MethodSummary.IndexOf(MethodDefinitionStart, ic);
				}
			}
		}


		private static void ParseMethodDefinition(TypeInfo context, string MethodDefinition)
		{
			var ReturnType = default(string);
			var Signature = default(string);

			var TDStart = "<TD";
			var TDEnd = "</TD>";


			{

				var i = MethodDefinition.IndexOf(TDStart);


				while (i > -1)
				{
					var ic = MethodDefinition.IndexOf(TDEnd, i + TDStart.Length);

					var Text = MethodDefinition.Substring(i, ic - i + TDEnd.Length);

					var j = Text.IndexOf(">") + 1;
					Text = Text.Substring(j, Text.Length - TDEnd.Length - j);

					if (string.IsNullOrEmpty(ReturnType))
					{
						ReturnType = Text;
					}
					else
					{
						Signature = Text;
					}


					i = MethodDefinition.IndexOf(TDStart, ic);
				}
			}

			var SummaryHint = "<BR>";

			var Summary = Signature.Substring(Signature.IndexOf(SummaryHint) + SummaryHint.Length);

			Signature = Signature.Substring(0, Signature.IndexOf(SummaryHint));

			ParseMethodDefinition(context, ReturnType, Signature, Summary);
		}

		private static void ParseMethodDefinition(TypeInfo context, string ReturnType, string Signature, string Summary)
		{
			Summary = Summary.Replace("&nbsp;", " ").Trim();

			// <CODE><B><A HREF="../../../org/w3c/dom/Document.html#createAttributeNS(java.lang.String, java.lang.String)">createAttributeNS</A></B>(<A HREF="../../../java/lang/String.html" title="class in java.lang">String</A>&nbsp;namespaceURI,
			// <A HREF="../../../java/lang/String.html" title="class in java.lang">String</A>&nbsp;qualifiedName)</CODE>

			var ParametersHint = "</B>";
			var ParametersStart = "(";
			var ParametersEnd = ")";

			var i = Signature.IndexOf(ParametersStart, Signature.IndexOf(ParametersHint));
			var ic = Signature.IndexOf(ParametersEnd, i);

			var value = new MethodDefinitionInfo();
			value.Name = Signature.Substring(0, i).ToElementText();
			value.ReturnParameter = new ParameterInfo { Type = new TypeInfo() };
			value.Summary = Summary;

			Action IsAbstract = () => { value.IsAbstract = true; context.IsAbstract = true; };
			Action IsProtected = () => value.IsProtected = true;
			Action IsStatic = () => value.IsStatic = true;
			// <FONT SIZE=\"-1\">\n<CODE>&nbsp;boolean</CODE></FONT>
			ParseParameter(ReturnType, value.ReturnParameter, IsProtected, IsStatic, IsAbstract);

			var Parameters = Signature.Substring(i + ParametersStart.Length, ic - i - ParametersStart.Length).Trim();

			if (Parameters.Length > 0)
				foreach (var Parameter in Parameters.Split(','))
				{
					// "<A HREF=\"../../../java/lang/String.html\" title=\"class in java.lang\">String</A>&nbsp;namespaceURI"

					var p = new ParameterInfo();

					ParseParameter(Parameter, p, null, null, null);

					value.Add(p);
				}


			context.AddMethod(value);
		}

		private static void ParseParameter(string Parameter, ParameterInfo p, Action IsProtected, Action IsStatic, Action IsAbstract)
		{
			p.Type = new TypeInfo();

			var NamespaceHint1 = "title=";
			var NamespaceHint2 = "in ";
			var NamespaceHint3 = "\"";


			if (Parameter.IndexOf(NamespaceHint1) < 0)
			{

				var CleanedParameter = Parameter.ToElementText().Replace("&nbsp;", " ").Trim();

				if (CleanedParameter.StartsWith("protected"))
				{
					CleanedParameter = CleanedParameter.Substring("protected".Length).Trim();
					IsProtected();
				}

				if (CleanedParameter.StartsWith("static"))
				{
					CleanedParameter = CleanedParameter.Substring("static".Length).Trim();
					IsStatic();
				}

				if (CleanedParameter.StartsWith("abstract"))
				{
					CleanedParameter = CleanedParameter.Substring("abstract".Length).Trim();
					IsAbstract();
				}

				p.Type = new TypeInfo();

				var j = CleanedParameter.IndexOf(" ");

				if (j < 0)
				{ p.Type.Name = CleanedParameter; }
				else
				{
					p.Type.Name = CleanedParameter.Substring(0, j);
					p.Name = CleanedParameter.Substring(j + 1);
				}

			}
			else
			{
				var CleanedParameter = Parameter.ToElementText().Replace("&nbsp;", " ").Trim();

				if (CleanedParameter.StartsWith("protected"))
				{
					CleanedParameter = CleanedParameter.Substring("protected".Length).Trim();
					IsProtected();
				}

				if (CleanedParameter.StartsWith("static"))
				{
					CleanedParameter = CleanedParameter.Substring("static".Length).Trim();
					IsStatic();
				}

				if (CleanedParameter.StartsWith("abstract"))
				{
					CleanedParameter = CleanedParameter.Substring("abstract".Length).Trim();
					IsAbstract();
				}

				var j = Parameter.IndexOf(NamespaceHint2, Parameter.IndexOf(NamespaceHint1) + NamespaceHint1.Length) + NamespaceHint2.Length;
				var jc = Parameter.IndexOf(NamespaceHint3, j);



				p.Type.Namespace = Parameter.Substring(j, jc - j);

				var NameHint1 = ">";
				var NameHint2 = "</A>";

				j = Parameter.IndexOf(NameHint1, jc) + NameHint1.Length;
				jc = Parameter.IndexOf(NameHint2, j);

				p.Type.Name = Parameter.Substring(j, jc - j);
				p.Name = Parameter.Substring(jc + NameHint2.Length).ToElementText().Replace("&nbsp;", " ").Trim(); ;


			}

			if (p.Name != null)
				while (p.Name.StartsWith("[]"))
				{
					p.Name = p.Name.Substring(2).Trim();

					p.Type = new TypeInfo
					{
						FullName = p.Type.FullName + "[]",
						ElementType = p.Type
					};
				}

			if (p.Type.FullName.EndsWith("[]"))
			{
				p.Type.ElementType = new TypeInfo
				{
					FullName = p.Type.FullName.Substring(0, p.Type.FullName.Length - 2)
				};
			}
		}

		public class TypeInfo
		{
			public TypeInfo BaseType;

			public bool IsAbstract;
			public bool IsInterface;

			public bool IsGlobal
			{
				get
				{
					return string.IsNullOrEmpty(Namespace);
				}
			}

			public string Namespace;
			public string Name;

			public readonly MethodDefinitionInfo[] Methods = new MethodDefinitionInfo[0xFF];
			public int MethodsCount;

			public void AddMethod(MethodDefinitionInfo Method)
			{
				Methods[MethodsCount] = Method;
				MethodsCount++;
			}

			public readonly MethodDefinitionInfo[] Constructors = new MethodDefinitionInfo[0xFF];
			public int ConstructorsCount;

			public void AddConstructor(MethodDefinitionInfo Constructor)
			{
				Constructors[ConstructorsCount] = Constructor;
				ConstructorsCount++;
			}

			public string FullName
			{
				get
				{
					if (string.IsNullOrEmpty(Namespace))
						return Name;

					return Namespace + "." + Name;
				}

				set
				{
					var i = value.LastIndexOf(".");

					if (i < 0)
					{
						Name = value;
						return;
					}

					Name = value.Substring(i + 1);
					Namespace = value.Substring(0, i);
				}
			}

			public TypeInfo ElementType;

			public static implicit operator TypeInfo(string e)
			{
				return new TypeInfo { FullName = e };
			}

			public delegate void TypeInfoDelegate(TypeInfo t);

			public void VisitTypes(TypeInfoDelegate h)
			{
				if (this.BaseType != null)
				{
					h(this.BaseType);
				}

				#region Constructors
				for (int i = 0; i < this.ConstructorsCount; i++)
				{
					var m = this.Constructors[i];

					for (int j = 0; j < m.ParametersCount; j++)
					{
						var p = m.Parameters[j];
						h(p.Type);
					}
				}
				#endregion

				#region Methods
				for (int i = 0; i < this.MethodsCount; i++)
				{
					var m = this.Methods[i];
					h(m.ReturnParameter.Type);

					for (int j = 0; j < m.ParametersCount; j++)
					{
						var p = m.Parameters[j];
						h(p.Type);
					}
				}
				#endregion
			}

			public string[] GetReferencedTypes()
			{
				var a = new string[0x7FFF];
				var c = 0;

				VisitTypes(
					t =>
					{
						a[c] = t.FullName;
						c++;
					}
				);

			


				var b = new string[c];
				Array.Copy(a, b, c);

				Array.Sort((Array)b);

				// distinct
				var k = 0;
				var n = 0;
				if (c > 0)
				{
					n = 1;
					for (int i = 1; i < c; i++)
					{
						if (b[i - k - 1] == b[i])
						{
							k++;
						}
						else
						{
							b[i - k] = b[i];
							n++;
						}
					}
				}
				a = new string[n];
				Array.Copy(b, a, n);

				return a;
			}


			public string[] GetReferencedNamespaces()
			{
				var a = new string[0x7FFF];
				var c = 0;

				VisitTypes(
					t =>
					{
						if (t.IsGlobal)
							return;

						a[c] = t.Namespace;
						c++;
					}
				);



				var b = new string[c];
				Array.Copy(a, b, c);

				Array.Sort((Array)b);

				// distinct
				var k = 0;
				var n = 0;
				if (c > 0)
				{
					n = 1;
					for (int i = 1; i < c; i++)
					{
						if (b[i - k - 1] == b[i])
						{
							k++;
						}
						else
						{
							b[i - k] = b[i];
							n++;
						}
					}
				}
				a = new string[n];
				Array.Copy(b, a, n);

				return a;
			}

			public override string ToString()
			{
				return FullName;
			}
		}

		public class ParameterInfo
		{
			public TypeInfo Type;
			public string Name;
		}

		public class MethodDefinitionInfo
		{
			public string Name;

			public string Summary;

			public readonly ParameterInfo[] Parameters = new ParameterInfo[0xFF];
			public int ParametersCount;

			public ParameterInfo ReturnParameter;

			public bool IsAbstract;
			public bool IsProtected;
			public bool IsStatic;

			public void Add(ParameterInfo Parameter)
			{
				Parameters[ParametersCount] = Parameter;
				ParametersCount++;

			}

			public override string ToString()
			{
				var w = new StringBuilder();


				w.Append(ReturnParameter.Type.FullName);
				w.Append(" ");
				w.Append(this.Name);
				w.Append("(...)");

				return base.ToString();
			}
		}

		public static bool IsPrimitiveType(string e)
		{
			if (e == "byte")
				return true;

			if (e == "boolean")
				return true;

			if (e == "java.lang.String")
				return true;

			return false;
		}

		public static string ToCSharpTypeName(TypeInfo e)
		{
			if (e.ElementType != null)
			{
				return ToCSharpTypeName(e.ElementType) + "[]";
			}

			if (e.FullName == "java.lang.Object")
				return "object";

			if (e.FullName == "byte")
				return "sbyte";


			if (e.FullName == "boolean")
				return "bool";

			if (e.FullName == "java.lang.String")
				return "string";

			return e.Name;
		}


	}
}
