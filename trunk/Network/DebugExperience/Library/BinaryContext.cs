using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DebugExperience.Library
{
	public static class BinaryContext
	{
		public sealed class BinaryWriter_AsBuilder
		{
			public BinaryWriter Context;
		}

		public static BinaryWriter_AsBuilder AsBuilder(this BinaryWriter Context)
		{
			return new BinaryWriter_AsBuilder { Context = Context };
		}

		public sealed class BinaryWriter_AsBuilder_ConstantInt32
		{
			public BinaryWriter_AsBuilder Context;
			public BinaryWriter_AsBuilder_ConstantInt32 Context_ConstantInt32;

			public int Value;
		}


		public static BinaryWriter_AsBuilder_ConstantInt32 Int32(this BinaryWriter_AsBuilder Context, int Value)
		{
			return new BinaryWriter_AsBuilder_ConstantInt32 { Context = Context, Value = Value };
		}

		public static BinaryWriter_AsBuilder_ConstantInt32 Int32(this BinaryWriter_AsBuilder_ConstantInt32 Context, int Value)
		{
			return new BinaryWriter_AsBuilder_ConstantInt32 { Context_ConstantInt32 = Context, Value = Value };
		}

		public class BinaryWriter_AsBuilder_ParameterInt32
		{
			public BinaryWriter_AsBuilder_ConstantInt32 Context;
		}


		public static BinaryWriter_AsBuilder_ParameterInt32 Int32(this BinaryWriter_AsBuilder_ConstantInt32 Context)
		{
			return new BinaryWriter_AsBuilder_ParameterInt32 { Context = Context };
		}

		public static Action<int> ToAction(this BinaryWriter_AsBuilder_ParameterInt32 Context)
		{
			return Value1 => Context.Write(Value1, k => {});
		}

		public static void Write(this BinaryWriter_AsBuilder_ParameterInt32 Context, int Value, Action<BinaryWriter> set_Writer)
		{
			var Writer = default(BinaryWriter);

			Context.Context.Write(
				k =>
				{
					Writer = k;
					set_Writer(Writer);
				}
			);

			Writer.Write(Value);
		}

		public static void Write(this BinaryWriter_AsBuilder_ConstantInt32 Context, Action<BinaryWriter> set_Writer)
		{
			var Writer = default(BinaryWriter);

			
			if (Context.Context_ConstantInt32 != null)
				Context.Context_ConstantInt32.Write(
					k =>
					{
						Writer = k;
						set_Writer(Writer);
					}
				);

			if (Context.Context != null)
			{
				Writer = Context.Context.Context;
				set_Writer(Writer);
			}

			Writer.Write(Context.Value);
		}


		public class BinaryWriter_AsBuilder_ParameterInt32Int32
		{
			public BinaryWriter_AsBuilder_ParameterInt32 Context;
		}


		public static BinaryWriter_AsBuilder_ParameterInt32Int32 Int32(this BinaryWriter_AsBuilder_ParameterInt32 Context)
		{
			return new BinaryWriter_AsBuilder_ParameterInt32Int32 { Context = Context };
		}

		public static Action<int, int> ToAction(this BinaryWriter_AsBuilder_ParameterInt32Int32 Context)
		{
			return Context.Write;
		}

		public static void Write(this BinaryWriter_AsBuilder_ParameterInt32Int32 Context, int Value1, int Value2)
		{
			var Writer = default(BinaryWriter);

			Context.Context.Write(Value1,
				k =>
				{
					Writer = k;
					//set_Writer(Writer);
				}
			);

			Writer.Write(Value2);
		}

		public static void Write(this BinaryWriter_AsBuilder_ParameterInt32Int32 Context, int Value1, int Value2, Action<BinaryWriter> set_Writer)
		{
			var Writer = default(BinaryWriter);

			Context.Context.Write(Value1,
				k =>
				{
					Writer = k;
					set_Writer(Writer);
				}
			);

			Writer.Write(Value2);
		}




		public class BinaryWriter_AsBuilder_ParameterInt32Int32Int32
		{
			public BinaryWriter_AsBuilder_ParameterInt32Int32 Context;
		}


		public static BinaryWriter_AsBuilder_ParameterInt32Int32Int32 Int32(this BinaryWriter_AsBuilder_ParameterInt32Int32 Context)
		{
			return new BinaryWriter_AsBuilder_ParameterInt32Int32Int32 { Context = Context };
		}

		public static Action<int, int, int> ToAction(this BinaryWriter_AsBuilder_ParameterInt32Int32Int32 Context)
		{
			return Context.Write;
		}

		public static void Write(this BinaryWriter_AsBuilder_ParameterInt32Int32Int32 Context, int Value1, int Value2, int Value3)
		{
			var Writer = default(BinaryWriter);

			Context.Context.Write(Value1, Value2,
				k =>
				{
					Writer = k;
					//set_Writer(Writer);
				}
			);

			Writer.Write(Value3);
		}
	}
}
