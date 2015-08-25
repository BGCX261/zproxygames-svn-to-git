using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DebugExperienceSinglePlayer.Library;

namespace DebugExperienceSinglePlayer
{
	public abstract class ProgramInput
	{
		public event Action<ConsoleKeyInfo> ConsoleKeyPressed;

		#region input simulation
		public  void GatherUserInput()
		{
			// this method simulates various user interactions
			// during a game session

			Console.WriteLine("press 'escape' to close this client");

			ConsoleKey.Escape.ReadKeysUntil(
				c =>
				{
					this.ConsoleKeyPressed(c);

				}
			);
		}
		#endregion

	}
}
