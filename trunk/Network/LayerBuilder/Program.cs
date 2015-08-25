using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using ScriptCoreLib.CompilerServices;
using ScriptCoreLib.Reflection.Options;
using System.Windows.Forms;

namespace LayerBuilder
{
	class Program
	{
		static void Main(string[] args)
		{
			var Arguments = new ProgramArguments();

			args.AsParametersTo(Arguments);

			Console.WriteLine("Assembly: " + Arguments.Assembly.FullName);

			if (Arguments.Assembly == null)
				return;

			if (!Arguments.Assembly.Exists)
				return;

			Console.WriteLine(new { Assembly = Arguments.Assembly.FullName, Arguments.TargetType }.ToString());

			#region ensure referenced libraries are provided
			foreach (var reference in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
			{
				var ra = Assembly.Load(reference);

				if (new FileInfo(ra.Location).Directory.FullName == new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName)
					if (new FileInfo(ra.Location).FullName != new FileInfo(new FileInfo(ra.Location).Name).FullName)
					{
						Console.WriteLine("ensure: " + reference);

						// delete old files
						if (new FileInfo(new FileInfo(ra.Location).Name).Exists)
							new FileInfo(new FileInfo(ra.Location).Name).Delete();

						File.Copy(new FileInfo(ra.Location).FullName, new FileInfo(new FileInfo(ra.Location).Name).FullName);

					}
			}
			#endregion

			var server_EntryPoint = default(MethodInfo);
			ConsoleRouterEmitter.BuildServer(Arguments, n => server_EntryPoint = n);

			// business speak:
			// this is the tool to make multiplayer happen
			// it should not be bundled with jsc package until it is mature (2009 summer?)

			Func<byte> BinaryReader_ReadByte = new BinaryReader(new MemoryStream()).ReadByte;
			Func<char> BinaryReader_ReadChar = new BinaryReader(new MemoryStream()).ReadChar;
			Func<int> BinaryReader_ReadInt32 = new BinaryReader(new MemoryStream()).ReadInt32;

			Action<byte> BinaryWriter_WriteByte = new BinaryWriter(new MemoryStream()).Write;
			Action<int> BinaryWriter_WriteInt32 = new BinaryWriter(new MemoryStream()).Write;
			Action<char> BinaryWriter_WriteChar = new BinaryWriter(new MemoryStream()).Write;

			// http://davidhayden.com/blog/dave/archive/2006/02/05/2791.aspx
			// http://groups.google.com/group/microsoft.public.dotnet.languages.csharp/browse_thread/thread/1a591da28ec3f02e/23817db409dfbc84
			// http://stackoverflow.com/questions/193875/assembly-not-saving-correctly

			var TargetAssembly = Assembly.LoadFile(Arguments.Assembly.FullName);
			var TargetType = TargetAssembly.GetType(Arguments.TargetType);



			var PeerAssemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(TargetAssembly.Location) + "Peer");
			var PeerAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(PeerAssemblyName, AssemblyBuilderAccess.RunAndSave);


			var m = PeerAssembly.DefineDynamicModule(
				PeerAssemblyName.Name,
				PeerAssemblyName.Name + ".exe"
				//"Layer1Module1.mod"
			);


			//var t = m.DefineType("``.*`MyType1", TypeAttributes.Public);
			var t = m.DefineType(PeerAssemblyName.Name + ".ServerAndPeer", TypeAttributes.Public);



			t.SetCustomAttribute(
				new CustomAttributeBuilder(typeof(ScriptCoreLib.ScriptAttribute).GetConstructor(new Type[0]), new object[0])
			);

			var Field_Context = t.DefineField("Context", TargetType, FieldAttributes.Public | FieldAttributes.InitOnly);
			var Field_CurrentWriter = t.DefineField("CurrentWriter", typeof(BinaryWriter), FieldAttributes.Public);

			var ctor = t.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[0]);
			var ctor_il = ctor.GetILGenerator();





			// we are going to overload those events
			var Events = TargetType.GetFields().Where(k => k.FieldType.BaseType == typeof(MulticastDelegate)).ToArray();

			Action<string> WriteLine =
				text =>
				{
					ctor_il.EmitWriteLine(text);
					Console.WriteLine(text);
				};



			ctor_il.Emit(OpCodes.Ldarg_0);
			ctor_il.Emit(OpCodes.Newobj, TargetType.GetConstructor(Type.EmptyTypes));
			ctor_il.Emit(OpCodes.Stfld, Field_Context);

			WriteLine("the following events should be overridden:");

			var EmitClassDeserializer = SerializationBuilder.GetEmitClassDeserializer(t);
			var EmitClassSerializer = SerializationBuilder.GetEmitClassSerializer(t);


			var Fields_Event = new List<FieldBuilder>();

			{
				var EventIndex = 0;
				foreach (var k in Events)
				{
					EventIndex++;
					WriteLine("#" + EventIndex + " " + k.Name);

					var Field_Event = t.DefineField("`" + k.Name, k.FieldType, FieldAttributes.InitOnly);
					Fields_Event.Add(Field_Event);

					#region preserve original implementation
					ctor_il.Emit(OpCodes.Ldarg_0);
					ctor_il.Emit(OpCodes.Ldarg_0);
					ctor_il.Emit(OpCodes.Ldfld, Field_Context);
					ctor_il.Emit(OpCodes.Ldfld, k);
					ctor_il.Emit(OpCodes.Stfld, Field_Event);
					#endregion



					var k_Invoke = k.FieldType.GetMethod("Invoke");

					var Method_AtEvent = t.DefineMethod("``" + k.Name, MethodAttributes.Public, CallingConventions.HasThis, k_Invoke.ReturnType, k_Invoke.GetParameters().Select(kk => kk.ParameterType).ToArray());
					var Method_AtEvent_il = Method_AtEvent.GetILGenerator();

					//Method_AtEvent_il.EmitWriteLine(k.Name + " to network");

					#region to network
					Method_AtEvent_il.Emit(OpCodes.Ldarg_0);
					Method_AtEvent_il.Emit(OpCodes.Ldfld, Field_CurrentWriter);
					Method_AtEvent_il.Emit(OpCodes.Ldc_I4, EventIndex);

					Method_AtEvent_il.EmitCall(OpCodes.Callvirt, BinaryWriter_WriteByte.Method, null);

					foreach (var p in k_Invoke.GetParameters().Skip(1))
					{

						if (p.ParameterType == typeof(int))
						{
							Method_AtEvent_il.Emit(OpCodes.Ldarg_0);
							Method_AtEvent_il.Emit(OpCodes.Ldfld, Field_CurrentWriter);
							Method_AtEvent_il.Emit(OpCodes.Ldarg_S, (byte)(p.Position + 1));
							Method_AtEvent_il.EmitCall(BinaryWriter_WriteInt32);
						}
						else if (p.ParameterType == typeof(char))
						{
							Method_AtEvent_il.Emit(OpCodes.Ldarg_0);
							Method_AtEvent_il.Emit(OpCodes.Ldfld, Field_CurrentWriter);
							Method_AtEvent_il.Emit(OpCodes.Ldarg_S, (byte)(p.Position + 1));
							Method_AtEvent_il.EmitCall(BinaryWriter_WriteChar);
						}
						else if (p.ParameterType == typeof(byte))
						{
							Method_AtEvent_il.Emit(OpCodes.Ldarg_0);
							Method_AtEvent_il.Emit(OpCodes.Ldfld, Field_CurrentWriter);
							Method_AtEvent_il.Emit(OpCodes.Ldarg_S, (byte)(p.Position + 1));
							Method_AtEvent_il.EmitCall(BinaryWriter_WriteByte);
						}
						else if (p.ParameterType.IsClass)
						{
							// we are about to serialize this class

							// if null we shall skip to serialize it
							// als we should add stack to prevent circular references

							Method_AtEvent_il.Emit(OpCodes.Ldarg_0);
							Method_AtEvent_il.Emit(OpCodes.Ldarg_0);
							Method_AtEvent_il.Emit(OpCodes.Ldfld, Field_CurrentWriter);
							Method_AtEvent_il.Emit(OpCodes.Ldarg_S, (byte)(p.Position + 1));
							Method_AtEvent_il.EmitCall(OpCodes.Call, EmitClassSerializer(p.ParameterType), null);

						}
						else throw new NotSupportedException(p.ParameterType.FullName);
					}
					#endregion

					//Method_AtEvent_il.EmitWriteLine(k.Name + " to local");

					#region this.AtEvent(arguments[...])
					Method_AtEvent_il.Emit(OpCodes.Ldarg_0);
					Method_AtEvent_il.Emit(OpCodes.Ldfld, Field_Event);

					var pi = 1;
					foreach (var p in k_Invoke.GetParameters())
					{
						Method_AtEvent_il.Emit(OpCodes.Ldarg_S, (byte)(pi++));
					}

					Method_AtEvent_il.Emit(OpCodes.Call, k_Invoke);
					#endregion

					Method_AtEvent_il.Emit(OpCodes.Ret);

					#region this.Context[Event] = this[Event];
					ctor_il.Emit(OpCodes.Ldarg_0);
					ctor_il.Emit(OpCodes.Ldfld, Field_Context);

					ctor_il.Emit(OpCodes.Ldarg_0);
					ctor_il.Emit(OpCodes.Ldftn, Method_AtEvent);
					ctor_il.Emit(OpCodes.Newobj, k.FieldType.GetConstructor(new[] { typeof(object), typeof(IntPtr) }));

					ctor_il.Emit(OpCodes.Stfld, k);
					#endregion

				}
			}

			#region InitializeReader
			var InitializeReader = t.DefineMethod("InitializeReader", MethodAttributes.Public, CallingConventions.HasThis, typeof(void), new[] { typeof(Stream) });
			var InitializeReader_il = InitializeReader.GetILGenerator();

			var InitializeReader_Reader = InitializeReader_il.DeclareLocal(typeof(BinaryReader));
			var InitializeReader_ins = InitializeReader_il.DeclareLocal(typeof(int));

			InitializeReader_il.Emit(OpCodes.Ldarg_1);
			InitializeReader_il.Emit(OpCodes.Newobj, typeof(BinaryReader).GetConstructor(new[] { typeof(Stream) }));
			InitializeReader_il.Emit(OpCodes.Stloc, InitializeReader_Reader);
			InitializeReader_il.EmitWriteLine("InitializeReader");

			var InitializeReader_Continue = InitializeReader_il.DefineLabel();
			InitializeReader_il.MarkLabel(InitializeReader_Continue);


			InitializeReader_il.Emit(OpCodes.Ldloc, InitializeReader_Reader);

			InitializeReader_il.EmitCall(OpCodes.Callvirt, BinaryReader_ReadByte.Method, null);
			InitializeReader_il.Emit(OpCodes.Stloc, InitializeReader_ins);


			{
				var EventIndex = 0;
				foreach (var k in Events)
				{
					EventIndex++;
					var InitializeReader_il_skip = InitializeReader_il.DefineLabel();
					InitializeReader_il.Emit(OpCodes.Ldloc, InitializeReader_ins);
					InitializeReader_il.Emit(OpCodes.Ldc_I4, EventIndex);
					InitializeReader_il.Emit(OpCodes.Ceq);
					InitializeReader_il.Emit(OpCodes.Brfalse, InitializeReader_il_skip);


					InitializeReader_il.Emit(OpCodes.Ldarg_0);
					InitializeReader_il.Emit(OpCodes.Ldfld, Fields_Event[EventIndex - 1]);

					#region sender index to identity

					InitializeReader_il.Emit(OpCodes.Ldnull);
					#endregion

					var k_Invoke = k.FieldType.GetMethod("Invoke");
					foreach (var p in k_Invoke.GetParameters().Skip(1))
					{

						if (p.ParameterType == typeof(int))
						{
							InitializeReader_il.Emit(OpCodes.Ldloc, InitializeReader_Reader);
							InitializeReader_il.EmitCall(BinaryReader_ReadInt32);
						}
						else if (p.ParameterType == typeof(char))
						{
							InitializeReader_il.Emit(OpCodes.Ldloc, InitializeReader_Reader);
							InitializeReader_il.EmitCall(BinaryReader_ReadChar);
						}
						else if (p.ParameterType == typeof(byte))
						{
							InitializeReader_il.Emit(OpCodes.Ldloc, InitializeReader_Reader);
							InitializeReader_il.EmitCall(BinaryReader_ReadByte);
						}
						else if (p.ParameterType.IsClass)
						{
							var ClassDeserializer = EmitClassDeserializer(p.ParameterType);

							InitializeReader_il.Emit(OpCodes.Ldarg_0);
							InitializeReader_il.Emit(OpCodes.Ldloc, InitializeReader_Reader);
							InitializeReader_il.EmitCall(OpCodes.Call, ClassDeserializer, null);

						}
					}

					InitializeReader_il.Emit(OpCodes.Call, k_Invoke);

					InitializeReader_il.MarkLabel(InitializeReader_il_skip);
				}
			}


			InitializeReader_il.Emit(OpCodes.Br, InitializeReader_Continue);
			//InitializeReader_il.Emit(OpCodes.Ret);

			#endregion

			#region InitializeWriter
			var InitializeWriter = t.DefineMethod("InitializeWriter", MethodAttributes.Public, CallingConventions.HasThis, typeof(void), new[] { typeof(Stream) });
			var InitializeWriter_il = InitializeWriter.GetILGenerator();

			InitializeWriter_il.EmitWriteLine("InitializeWriter");

			#region this.CurrentWriter = arguments[0];
			InitializeWriter_il.Emit(OpCodes.Ldarg_0);
			InitializeWriter_il.Emit(OpCodes.Ldarg_1);
			InitializeWriter_il.Emit(OpCodes.Newobj, typeof(BinaryWriter).GetConstructor(new[] { typeof(Stream) }));
			InitializeWriter_il.Emit(OpCodes.Stfld, Field_CurrentWriter);
			#endregion

			InitializeWriter_il.Emit(OpCodes.Ret);

			#endregion

			ctor_il.EmitWriteLine("connecting...");

			#region ConnectToRouter(InitializeWriter, InitializeReader)
			ctor_il.Emit(OpCodes.Ldarg_0);
			ctor_il.Emit(OpCodes.Ldftn, InitializeWriter);
			ctor_il.Emit(OpCodes.Newobj, typeof(Action<Stream>).GetConstructor(new[] { typeof(object), typeof(IntPtr) }));

			ctor_il.Emit(OpCodes.Ldarg_0);
			ctor_il.Emit(OpCodes.Ldftn, InitializeReader);
			ctor_il.Emit(OpCodes.Newobj, typeof(Action<Stream>).GetConstructor(new[] { typeof(object), typeof(IntPtr) }));

			Action<Action<Stream>, Action<Stream>> ConnectToRouter = PrimitiveServerBuilder.ConnectToRouter;
			ctor_il.EmitCall(ConnectToRouter);
			#endregion

			ctor_il.Emit(OpCodes.Ret);


			var tm = t.DefineMethod("MainSTA", MethodAttributes.Static,
				typeof(void), new[] { typeof(string[]) }
			);



			var tmil = tm.GetILGenerator();

			//tmil.EmitWriteLine("");
			//tmil.EmitWriteLine("usage: layer1.exe /java this will convert to java server");
			//tmil.EmitWriteLine("usage: layer1.exe /as this will convert to actionscript3 client");
			//tmil.EmitWriteLine("");

			//ScriptCoreLib.CompilerServices.PrimitiveServerBuilder.EmitVersionInformation(tmil);





			// Field_Context must be Canvas at this point

			if (typeof(Form).IsAssignableFrom(TargetType))
			{
				Action Application_EnableVisualStyles = System.Windows.Forms.Application.EnableVisualStyles;
				Action<Form> Application_Run = System.Windows.Forms.Application.Run;

				tmil.Emit(OpCodes.Call, Application_EnableVisualStyles.Method);
				tmil.Emit(OpCodes.Newobj, ctor);
				tmil.Emit(OpCodes.Ldfld, Field_Context);
				tmil.Emit(OpCodes.Call, Application_Run.Method);
			}

			if (typeof(Canvas).IsAssignableFrom(TargetType))
			{
				Func<Canvas, Window> _ToWindow = ScriptCoreLib.CSharp.Avalon.Extensions.AvalonExtensions.ToWindow;
				MethodInfo _ShowDialog = typeof(Window).GetMethod("ShowDialog");

				tmil.Emit(OpCodes.Newobj, ctor);
				tmil.Emit(OpCodes.Ldfld, Field_Context);
				tmil.EmitCall(OpCodes.Call, _ToWindow.Method, null);
				tmil.EmitCall(OpCodes.Call, _ShowDialog, null);
				tmil.Emit(OpCodes.Pop);
			}



			//tmil.EmitCall(OpCodes.Call, TargetType.GetMethod("GatherUserInput"), null);
			tmil.Emit(OpCodes.Ret);



			var main = t.DefineMethod("Main", MethodAttributes.Static,
				typeof(void), new[] { typeof(string[]) }
			);

			var main_il = main.GetILGenerator();


			main_il.Emit(OpCodes.Ldnull);
			main_il.Emit(OpCodes.Ldftn, tm);
			main_il.Emit(OpCodes.Newobj, typeof(ParameterizedThreadStart).GetConstructors().Single());
			main_il.Emit(OpCodes.Newobj, typeof(Thread).GetConstructor(new[] { typeof(ThreadStart) }));

			Action<ApartmentState> Thread_SetApartmentState = new Thread(delegate() { }).SetApartmentState;
			Action<object> Thread_Start = new Thread(delegate() { }).Start;
			Action Thread_Join = new Thread(delegate() { }).Join;

			main_il.Emit(OpCodes.Dup);
			main_il.Emit(OpCodes.Ldc_I4, (int)ApartmentState.STA);
			main_il.EmitCall(Thread_SetApartmentState);

			main_il.Emit(OpCodes.Dup);
			main_il.Emit(OpCodes.Ldarg_0);
			main_il.EmitCall(Thread_Start);


			//main_il.Emit(OpCodes.Ldarg_0);
			//main_il.EmitCall(OpCodes.Call, server_EntryPoint, null);



			main_il.EmitCall(Thread_Join);
			main_il.Emit(OpCodes.Ret);


			t.CreateType();

			PeerAssembly.SetEntryPoint(main);

			//a.AddResource("name1.name2.ken.txt", "hello world");
			//a.AddResource("Layer1Assembly.assets.Layer1Assembly.about.txt", "kenny");

			//a.DefineResource("log1", "desc1", "f1").AddResource("key1", "value1");


			Environment.CurrentDirectory = Arguments.Assembly.Directory.FullName;

			var PeerFile = new FileInfo(PeerAssemblyName.Name + ".exe");
			Console.WriteLine("peer: " + PeerFile.FullName);
			PeerAssembly.Save(PeerFile.Name);




		}
	}
}
