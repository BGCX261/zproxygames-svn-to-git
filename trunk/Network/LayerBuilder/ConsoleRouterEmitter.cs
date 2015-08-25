using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;

using jsc.meta.Library;
using System.ComponentModel;
using LayerBuilder.Server;

#pragma warning disable 1720
// http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=482159

namespace LayerBuilder
{
	public class ConsoleRouterEmitter
	{
		// todo: ScriptCoreLib.Net shall not contain il emitter functionality
		// jsc.meta shall contaion it instead

		static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			//var bin = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
			var lib = new DirectoryInfo(Environment.CurrentDirectory);

			if (!lib.Exists)
				lib = new DirectoryInfo(@"c:\util\jsc\lib");


			if (lib.Exists)
			{
				var r = new AssemblyName(args.Name);
		
				var lib_dll = Path.Combine(lib.FullName, r.Name + ".dll");
				if (File.Exists(lib_dll))
				{
					return Assembly.LoadFile(lib_dll);
				}

				var lib_exe = Path.Combine(lib.FullName, r.Name + ".exe");
				if (File.Exists(lib_exe))
				{
					return Assembly.LoadFile(lib_exe);
				}
			}
			return null;
		}

		public static void BuildServer(ProgramArguments args, Action<MethodInfo> set_EntryPoint)
		{
			var TargetAssembly = Assembly.LoadFile(args.Assembly.FullName);

			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

			var TargetType = TargetAssembly.GetType(args.TargetType);

			if (TargetType == null)
				throw new InvalidOperationException();

			var name = new AssemblyName(Path.GetFileNameWithoutExtension(TargetAssembly.Location) + "Server");

			var a = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);

			var ServerPath = name.Name + ".exe";


			var m = a.DefineDynamicModule(name.Name, ServerPath);

			var Events = TargetType.GetFields().Where(k => k.FieldType.BaseType == typeof(MulticastDelegate)).ToArray();



			var t = m.DefineType(name.Name + ".ConsoleRouterImplementation", TypeAttributes.Public | TypeAttributes.Sealed, typeof(LayerBuilder.Server.ConsoleRouter));
			var t_ctor = t.DefineDefaultConstructor(MethodAttributes.Public);


			var DefineDescriptionAttribute = default(Func<string, DescriptionAttribute>).DefineAttributeAt(t);

			DefineDescriptionAttribute("This type is unaware of the actual types used in the messages but is aware of replicas of them. This type is designed to run on .net and jvm.");

			var EventTypes = new List<Type>();

			#region AddTypeToMessages
			var AddTypeToMessages = default(Action<Type>);

			AddTypeToMessages =
				k =>
				{
					if (EventTypes.Contains(k))
						return;

					if (!k.IsClass)
						return;

					if (k.IsArray)
					{
						AddTypeToMessages(k.GetElementType());
						return;
					}

					if (k.Equals(typeof(object)))
						return;

					if (k.Equals(typeof(string)))
						return;

					EventTypes.Add(k);

					foreach (var f in k.GetFields())
					{
						AddTypeToMessages(f.FieldType);
					}
				};
			#endregion


			#region KnownEvents
			var KnownEvents_2 = Events.Select(
				(k, index) =>
				{
					var LocalType = m.DefineType(name.Name + ".Tuples." + k.Name, TypeAttributes.Public);
					var LocalTypeDispatcher = LocalType.DefineNestedType("Dispatcher", TypeAttributes.NestedPublic);
					var LocalTypeDispatcherConstructor =
						LocalTypeDispatcher.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
						new[] { typeof(ConsoleRouter.DispatcherArguments), t }
					);
					var LocalTypeDispatcherBroadcast = LocalTypeDispatcher.DefineMethod("Broadcast", MethodAttributes.Public);

					var SerializePrimitive = t.DefineMethod("SerializePrimitive" + LocalType.Name, MethodAttributes.Public, CallingConventions.Standard,
							   typeof(void), new[] { typeof(BinaryWriter), LocalType }
						   );
					var DeserializePrimitive = t.DefineMethod("DeserializePrimitive" + LocalType.Name, MethodAttributes.Public, CallingConventions.Standard,
							LocalType, new[] { typeof(BinaryReader) }
						);

					{
						var il = SerializePrimitive.GetILGenerator();

						il.Emit(OpCodes.Ldarg_1);
						il.Emit(OpCodes.Ldc_I4, index + 1);
						il.Emit(OpCodes.Call, typeof(BinaryWriter).GetMethod("Write", new [] { typeof(byte)}));
					}

					return new
					{
						Operation = (byte)(index + 1),
						LocalType,
						LocalConstructor = LocalType.DefineDefaultConstructor(MethodAttributes.Public),
						Parameters = k.FieldType.GetMethod("Invoke").GetParameters().Skip(1).Select(
							p =>
							{
								AddTypeToMessages(p.ParameterType);

								//return LocalType.DefineField(p.Name, p.ParameterType, FieldAttributes.Public);
								return p;
							}
						).ToArray(),
						SerializePrimitive,
						DeserializePrimitive,
						LocalTypeDispatcher,
						LocalTypeDispatcherConstructor,
						LocalTypeDispatcherBroadcast

					};
				}
			).ToArray();
			#endregion


			#region define serializers...
			var DefinedEventTypes_2 = EventTypes.ToDictionary(
				k => k,
				k =>
				{
					var LocalType = m.DefineType(name.Name + ".Tuples." + k.Name, TypeAttributes.Public);
					var Serialize = t.DefineMethod("Serialize" + LocalType.Name, MethodAttributes.Private, CallingConventions.Standard,
							typeof(void), new[] { typeof(BinaryWriter), LocalType }
						);

					// document the parameter as optional
					Serialize.DefineParameter(2, ParameterAttributes.Optional, "value").SetConstant(null);

					return new
					{
						LocalType,
						LocalConstructor = LocalType.DefineDefaultConstructor(MethodAttributes.Public),
						Serialize,
						Deserialize = t.DefineMethod("Deserialize" + LocalType.Name, MethodAttributes.Private, CallingConventions.Standard,
							LocalType, new[] { typeof(BinaryReader) }
						)
					};


				}

			);

			var DefinedEventTypes = DefinedEventTypes_2.ToDictionary(
				k => k.Key,
				k =>
				{
					return new
					{
						k.Value.Deserialize,
						k.Value.LocalType,
						k.Value.Serialize,
						k.Value.LocalConstructor,

						Fields = k.Key.GetFields().Select(
							f =>
								DefinedEventTypes_2[k.Key].LocalType.DefineField(f.Name,
								DefinedEventTypes_2.ContainsKey(f.FieldType) ?
								DefinedEventTypes_2[f.FieldType].LocalType : f.FieldType, FieldAttributes.Public)


						).ToArray()
					};
				}
			);

			foreach (var k in DefinedEventTypes)
			{


				ImplementSerialize(k.Value.Serialize, k.Value.Fields, kk => DefinedEventTypes.Single(kkk => kkk.Value.LocalType == kk).Value.Serialize, true);
				ImplementDeserialize(
					k.Value.LocalConstructor,
					k.Value.Deserialize,
					k.Value.Fields,
					kk => DefinedEventTypes.Single(kkk => kkk.Value.LocalType == kk).Value.Deserialize,
					true
				);

				k.Value.LocalType.CreateType();
			}
			#endregion

			#region define non optional 'primitive' serializers
			foreach (var k in KnownEvents_2)
			{
				var LocalType = k.LocalType;

				var Fields = k.Parameters.Select(
					p =>
					{
						return LocalType.DefineField(p.Name,

							DefinedEventTypes.ContainsKey(p.ParameterType) ?
							DefinedEventTypes[p.ParameterType].LocalType :
							p.ParameterType

							, FieldAttributes.Public);

					}
				).ToArray();


				ImplementSerialize(k.SerializePrimitive, Fields, kk => DefinedEventTypes.Single(kkk => kkk.Value.LocalType == kk).Value.Serialize, false);


				ImplementDeserialize(
					k.LocalConstructor,
					k.DeserializePrimitive,
					Fields,
					kk => DefinedEventTypes.Single(kkk => kkk.Value.LocalType == kk).Value.Deserialize,
					false
				);




				LocalType.CreateType();

			}
			#endregion


			var main = t.DefineMethod("Main", MethodAttributes.Static | MethodAttributes.Public,
				typeof(void), new[] { typeof(string[]) }
			);

			var main_il = main.GetILGenerator();

			//main_il.EmitWriteLine("hello world");
			//main_il.EmitWriteLine("hello world2");


			//var _ConsoleRouter = new LayerBuilder.Server.ConsoleRouter();
			var _ConsoleRouter_ctor = default(Func<LayerBuilder.Server.ConsoleRouter>).ToConstructorInfo();

			MethodInfo _Invoke = typeof(LayerBuilder.Server.ConsoleRouter).GetMethod("Invoke");
			FieldInfo _Arguments = typeof(LayerBuilder.Server.ConsoleRouter).GetField("Arguments");

			var main_loc_router = main_il.DeclareLocal(typeof(LayerBuilder.Server.ConsoleRouter));

			main_il.Emit(OpCodes.Newobj, t_ctor);
			main_il.Emit(OpCodes.Stloc_S, (byte)main_loc_router.LocalIndex);

			main_il.Emit(OpCodes.Ldloc_S, (byte)main_loc_router.LocalIndex);
			main_il.Emit(OpCodes.Ldarg_0);
			main_il.Emit(OpCodes.Stfld, _Arguments);

			main_il.Emit(OpCodes.Ldloc_S, (byte)main_loc_router.LocalIndex);
			main_il.EmitCall(OpCodes.Call, _Invoke, null);

			main_il.Emit(OpCodes.Ret);

			#region override Dispatcher
			var _DispatcherAbstract = typeof(ConsoleRouter).GetMethod("Dispatcher", BindingFlags.NonPublic | BindingFlags.Instance);
			var _Dispatcher = t.DefineMethod("Dispatcher", MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.Family, typeof(void), new[] { typeof(ConsoleRouter.DispatcherArguments) });
			t.DefineMethodOverride(_Dispatcher, _DispatcherAbstract);

			{
				var il = _Dispatcher.GetILGenerator();

				var Operation = il.DeclareLocal(typeof(byte));
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldfld, typeof(ConsoleRouter.DispatcherArguments).GetField("Operation"));
				il.Emit(OpCodes.Stloc_S, (byte)Operation.LocalIndex);

				foreach (var k in KnownEvents_2)
				{
					var next = il.DefineLabel();
					il.Emit(OpCodes.Ldloc_S, (byte)Operation.LocalIndex);
					il.Emit(OpCodes.Ldc_I4, (int)k.Operation);
					il.Emit(OpCodes.Ceq);
					il.Emit(OpCodes.Brfalse, next);

					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Newobj, k.LocalTypeDispatcherConstructor);

					il.EmitCall(OpCodes.Call, k.LocalTypeDispatcherBroadcast, null);

					il.MarkLabel(next);
				}

				il.Emit(OpCodes.Ret);
			}
			#endregion

			foreach (var k in KnownEvents_2)
			{
				var _value = default(FieldInfo);
				var _args = default(FieldInfo);
				var _context = default(FieldInfo);

				{
					var il = k.LocalTypeDispatcherConstructor.GetILGenerator();

					_value = k.LocalTypeDispatcher.DefineField("value", k.LocalType, FieldAttributes.InitOnly);
					_args = k.LocalTypeDispatcher.DefineField("args", typeof(ConsoleRouter.DispatcherArguments), FieldAttributes.InitOnly);
					_context = k.LocalTypeDispatcher.DefineField("context", t, FieldAttributes.InitOnly);

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Stfld, _args);

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Stfld, _context);

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldfld, typeof(ConsoleRouter.DispatcherArguments).GetField("Reader"));
					il.EmitCall(OpCodes.Call, k.DeserializePrimitive, null);
					il.Emit(OpCodes.Stfld, _value);

					il.Emit(OpCodes.Ret);
				}

				var TransmitTo = k.LocalTypeDispatcher.DefineMethod("TransmitTo", MethodAttributes.Public, CallingConventions.HasThis,
					typeof(void), new[] { typeof(BinaryWriter) }
				);

				{
					var il = TransmitTo.GetILGenerator();

					// here we should write to/from information

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, _context);


					il.Emit(OpCodes.Ldarg_1);

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, _value);

					il.Emit(OpCodes.Call, k.SerializePrimitive);

					il.Emit(OpCodes.Ret);
				}

				{
					var il = k.LocalTypeDispatcherBroadcast.GetILGenerator();

					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldfld, _args);
					il.Emit(OpCodes.Ldfld, typeof(ConsoleRouter.DispatcherArguments).GetField("Broadcast"));



					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldftn, TransmitTo);
					il.Emit(OpCodes.Newobj, typeof(ConsoleRouter.WriterDelegate).GetConstructors().Single());

					il.EmitCall(OpCodes.Call, typeof(ConsoleRouter.BroadcastDelegate).GetMethod("Invoke"), null);



					il.Emit(OpCodes.Ret);
				}


				k.LocalTypeDispatcher.CreateType();

			}


			t.CreateType();

			a.SetEntryPoint(main);


			a.Save(ServerPath);

			if (args.javapath != null)
				if (args.javapath.Exists)
					new jsc.meta.Commands.Extend.ExtendToJavaConsole
					{
						javapath = args.javapath,
						assembly = new FileInfo(ServerPath),
						//staging = new DirectoryInfo("staging")
					}.Invoke();


			set_EntryPoint(main);
		}

		private static void ImplementDeserialize(ConstructorInfo LocalConstructor, MethodBuilder _Deserialize, FieldInfo[] fields, Func<Type, MethodInfo> get_Deserialize, bool IsOptional)
		{

			Func<byte> BinaryReader_ReadByte = new BinaryReader(new MemoryStream()).ReadByte;
			Func<string> BinaryReader_ReadString = new BinaryReader(new MemoryStream()).ReadString;
			Func<int> BinaryReader_ReadInt32 = new BinaryReader(new MemoryStream()).ReadInt32;


			var il = _Deserialize.GetILGenerator();

			if (IsOptional)
			{
				#region if (read == 0) return null;
				var notnull = il.DefineLabel();

				// jsc needs a temporal variable it seems...
				var isnull = il.DeclareLocal(typeof(bool));

				#region read 0/1
				il.Emit(OpCodes.Ldarg_1);
				il.EmitCall(OpCodes.Call, BinaryReader_ReadByte.Method, null);
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Ceq);
				il.Emit(OpCodes.Stloc_S, (byte)isnull.LocalIndex);

				il.Emit(OpCodes.Ldloc_S, (byte)isnull.LocalIndex);
				il.Emit(OpCodes.Brfalse, notnull);
				#endregion

				il.Emit(OpCodes.Ldnull);
				il.Emit(OpCodes.Ret);

				il.MarkLabel(notnull);
				#endregion

			}

			var r = il.DeclareLocal(LocalConstructor.DeclaringType);

			il.Emit(OpCodes.Newobj, LocalConstructor);
			il.Emit(OpCodes.Stloc_S, (byte)r.LocalIndex);

			foreach (var f in fields)
			{
				il.Emit(OpCodes.Ldloc_S, (byte)r.LocalIndex);

				if (f.FieldType.IsClass && !(f.FieldType == typeof(string)))
				{
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.EmitCall(OpCodes.Call, get_Deserialize(f.FieldType), null);
				}
				else
				{
					if (f.FieldType == typeof(int))
					{
						il.Emit(OpCodes.Ldarg_1);
						il.EmitCall(OpCodes.Call, BinaryReader_ReadInt32.Method, null);
					}
					else if (f.FieldType == typeof(string))
					{
						il.Emit(OpCodes.Ldarg_1);
						il.EmitCall(OpCodes.Call, BinaryReader_ReadString.Method, null);
					}
					else if (f.FieldType == typeof(byte))
					{
						il.Emit(OpCodes.Ldarg_1);
						il.EmitCall(OpCodes.Call, BinaryReader_ReadByte.Method, null);
					}
					else throw new NotSupportedException(f.FieldType.FullName);
				}

				il.Emit(OpCodes.Stfld, f);
			}


			il.Emit(OpCodes.Ldloc_S, (byte)r.LocalIndex);
			il.Emit(OpCodes.Ret);
		}

		private static void ImplementSerialize(MethodBuilder _Serialize, FieldInfo[] fields, Func<Type, MethodInfo> get_Serialize, bool IsOptional)
		{

			Action<byte> BinaryWriter_WriteByte = new BinaryWriter(new MemoryStream()).Write;
			Action<int> BinaryWriter_WriteInt32 = new BinaryWriter(new MemoryStream()).Write;
			Action<string> BinaryWriter_WriteString = new BinaryWriter(new MemoryStream()).Write;
			//Action<char> BinaryWriter_WriteChar = new BinaryWriter(new MemoryStream()).Write;


			var il = _Serialize.GetILGenerator();

			if (IsOptional)
			{
				#region if (k == null) { write(0); return; } write(1);
				var notnull = il.DefineLabel();

				il.Emit(OpCodes.Ldarg_2);

				il.Emit(OpCodes.Brtrue, notnull);

				#region return w.WriteByte(0)
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldc_I4_0);
				il.EmitCall(OpCodes.Call, BinaryWriter_WriteByte.Method, null);
				il.Emit(OpCodes.Ret);
				#endregion

				il.MarkLabel(notnull);

				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Ldc_I4_1);
				il.EmitCall(OpCodes.Call, BinaryWriter_WriteByte.Method, null);
				#endregion
			}

			foreach (var f in fields)
			{
				if (f.FieldType.IsClass && !(f.FieldType == typeof(string)))
				{
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Ldfld, f);
					il.EmitCall(OpCodes.Call, get_Serialize(f.FieldType), null);
				}
				else
				{
					il.Emit(OpCodes.Ldarg_1);
					il.Emit(OpCodes.Ldarg_2);
					il.Emit(OpCodes.Ldfld, f);

					if (f.FieldType == typeof(int))
					{
						il.EmitCall(OpCodes.Call, BinaryWriter_WriteInt32.Method, null);
					}
					else if (f.FieldType == typeof(string))
					{
						il.EmitCall(OpCodes.Call, BinaryWriter_WriteString.Method, null);
					}
					else if (f.FieldType == typeof(byte))
					{
						il.EmitCall(OpCodes.Call, BinaryWriter_WriteByte.Method, null);
					}
					else throw new NotSupportedException(f.FieldType.FullName);
				}

			}
			il.Emit(OpCodes.Ret);
		}
	}
}
