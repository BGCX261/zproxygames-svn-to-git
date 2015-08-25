using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using ScriptCoreLib.Shared.Lambda;
using ScriptCoreLib.Shared.Avalon.Extensions;

namespace AvalonMinesweeper.Core.Library
{
	public static class TimerBuilderExtensions
	{
		// context based programming
		public class TimerBuilder
		{
			public int Interval;

			public abstract class ContextTrace
			{
				public TimerBuilder ContextOfTimerBuilder;
				public WithBuilder ContextOfWithBuilder;
			}

			public sealed class WithBuilder : ContextTrace
			{
				public Action Tick;
			}

			public sealed class WithTimerBuilder : ContextTrace
			{
				public Action<DispatcherTimer> Tick;
			}
		}



		public static int GetInterval(this TimerBuilder.ContextTrace context)
		{
			if (context.ContextOfTimerBuilder != null)
				return context.ContextOfTimerBuilder.Interval;

			if (context.ContextOfWithBuilder != null)
				return context.ContextOfWithBuilder.GetInterval();

			return -1;
		}

		public static void Invoke(this TimerBuilder.WithBuilder e)
		{
			if (e.Tick != null)
				e.Tick();

			if (e.ContextOfWithBuilder != null)
				e.ContextOfWithBuilder.Invoke();
		}

		public static void Invoke(this TimerBuilder.WithTimerBuilder e, DispatcherTimer t)
		{
			if (e.Tick != null)
				e.Tick(t);

			if (e.ContextOfWithBuilder != null)
				e.ContextOfWithBuilder.Invoke();
		}

		public static DispatcherTimer Start(this TimerBuilder.WithTimerBuilder context)
		{
			return context.GetInterval().AtIntervalWithTimer(context.Invoke);
		}

		public static TimerBuilder.WithTimerBuilder With(this TimerBuilder.WithBuilder context, Action<DispatcherTimer> Tick)
		{
			return new TimerBuilder.WithTimerBuilder { ContextOfWithBuilder = context, Tick = Tick };
		}

		public static TimerBuilder.WithBuilder With(this TimerBuilder.WithBuilder context, Action Tick)
		{
			return new TimerBuilder.WithBuilder { ContextOfWithBuilder = context, Tick = Tick };
		}

		public static TimerBuilder.WithBuilder With(this TimerBuilder context, Action Tick)
		{
			return new TimerBuilder.WithBuilder { ContextOfTimerBuilder = context, Tick = Tick };
		}

		public static TimerBuilder ToTimerBuilder(this int interval)
		{
			return new TimerBuilder { Interval = interval };
		}
	}
}
