using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bulldog.Server.CodeGenerators.Java
{
	partial class DefinitionProvider
	{
		public string Server = "services.zproxybuzz.info";

		public string GetString()
		{
			var w = new CodeBuilder();

			w.WriteLine("// This source code was generated for ScriptCoreLib");
			w.WriteLine("// http://" + Server + "/java/" + this.Type.FullName);
			w.WriteLine();

			w.WriteUsing("ScriptCoreLib");

			foreach (var u in this.Type.GetReferencedNamespaces())
			{
				if (u != this.Type.Namespace)
					w.WriteUsing(u);
			}
			w.Builder.AppendLine();

			w.WriteKeyword(CodeBuilder.Keywords.@namespace);
			w.WriteLine(" " + w.EscapeKeywordsWithinNamespace(this.Type.Namespace));
			using (w.Block())
			{
				w.WriteLine("// " + this.Source.ToString());
				w.WriteLine("[Script(IsNative = true)]");

				w.WriteIndent();
				w.WriteKeyword(CodeBuilder.Keywords.@public);
				w.Builder.Append(" ");

				if (this.Type.IsInterface)
					w.WriteKeyword(CodeBuilder.Keywords.@interface);
				else
				{
					if (this.Type.IsAbstract)
						w.WriteKeywordSpace(CodeBuilder.Keywords.@abstract);
					w.WriteKeyword(CodeBuilder.Keywords.@class);
				}

				w.Builder.Append(" ");
				w.Builder.Append(this.Type.Name);

				if (this.Type.BaseType != null)
				{
					w.Builder.Append(" : ");
					w.Builder.Append(this.Type.BaseType.Name);
				};

				w.Builder.AppendLine();
				using (w.Block())
				{
					#region Constructors
					for (int i = 0; i < this.Type.ConstructorsCount; i++)
					{
						var m = this.Type.Constructors[i];

						w.WriteSummary(m.Summary);
						w.WriteIndent();


						if (m.IsStatic)
							w.WriteKeywordSpace(CodeBuilder.Keywords.@static);

						if (m.IsProtected)
							w.WriteKeywordSpace(CodeBuilder.Keywords.@protected);
						else
							w.WriteKeywordSpace(CodeBuilder.Keywords.@public);

						w.Builder.Append(this.Type.Name);


						using (w.Parameters())
						{
							for (int j = 0; j < m.ParametersCount; j++)
							{
								if (j > 0)
									w.Builder.Append(", ");

								var p = m.Parameters[j];

								w.Builder.Append(ToCSharpTypeName(p.Type));
								w.Builder.Append(" ");
								w.Builder.Append("@" + p.Name);
							}
						}


						w.Builder.AppendLine();
						using (w.Block())
						{

						}


						w.Builder.AppendLine();
					}
					#endregion

					#region Methods
					for (int i = 0; i < this.Type.MethodsCount; i++)
					{
						var m = this.Type.Methods[i];

						w.WriteSummary(m.Summary);
						w.WriteIndent();

						if (!this.Type.IsInterface)
						{
							if (m.IsStatic)
								w.WriteKeywordSpace(CodeBuilder.Keywords.@static);

							if (m.IsAbstract)
								w.WriteKeywordSpace(CodeBuilder.Keywords.@abstract);

							if (m.IsProtected)
								w.WriteKeywordSpace(CodeBuilder.Keywords.@protected);
							else
								w.WriteKeywordSpace(CodeBuilder.Keywords.@public);

							if (m.Name == "toString")
								w.WriteKeywordSpace(CodeBuilder.Keywords.@override);
							else if (m.Name == "hashCode")
								w.WriteKeywordSpace(CodeBuilder.Keywords.@override);
							else if (m.Name == "equals")
								w.WriteKeywordSpace(CodeBuilder.Keywords.@override);
						}


						w.Builder.Append(ToCSharpTypeName(m.ReturnParameter.Type));
						w.Builder.Append(" ");

						if (m.Name == "toString")
							w.Builder.Append("ToString");
						else if (m.Name == "hashCode")
							w.Builder.Append("GetHashCode");
						else if (m.Name == "equals")
							w.Builder.Append("Equals");
						else
							w.Builder.Append(m.Name);


						using (w.Parameters())
						{
							for (int j = 0; j < m.ParametersCount; j++)
							{
								if (j > 0)
									w.Builder.Append(", ");

								var p = m.Parameters[j];

								w.Builder.Append(ToCSharpTypeName(p.Type));
								w.Builder.Append(" ");
								w.Builder.Append("@" + p.Name);
							}
						}

						if (this.Type.IsInterface || m.IsAbstract)
						{
							w.Builder.AppendLine(";");
						}
						else
						{
							w.Builder.AppendLine();
							using (w.Block())
							{
								if (m.ReturnParameter.Type.Name == "void")
								{
								}
								else
								{
									w.WriteIndent();
									w.WriteKeywordSpace(CodeBuilder.Keywords.@return);
									w.WriteKeyword(CodeBuilder.Keywords.@default);
									w.Builder.AppendLine("(" + ToCSharpTypeName(m.ReturnParameter.Type) + ");");
								}

							}

						}

						w.Builder.AppendLine();
					}
					#endregion
				}
			}

			return w.ToString();
		}
	}
}
