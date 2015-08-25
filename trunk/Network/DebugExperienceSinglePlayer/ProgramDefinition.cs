using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using DebugExperienceSinglePlayer.Library;
using System.Net;
using System.IO;
using DebugExperienceSinglePlayer.Window;

namespace DebugExperienceSinglePlayer
{
	public class ProgramDefinition : ProgramInput
	{
		// the idea is to have a working 
		// single player definition like this one
		// which can also be debugged
		// using a tool this application should be
		// enhanced for different networking providers

		// read more: http://www.gamasutra.com/view/feature/4052/networks_with_letters.php?page=2
		public object Tag;

		public EndPointIdentity Identity = new EndPointIdentity();

		public const int TokenAtUserInput = 1;
		// this is the first synchronized method, should be frame synced?
		public Action<EndPointIdentity, char> AtUserInput;

		public const int TokenAtMove = 2;
		// this is the first synchronized method, should be frame synced?
		public Action<EndPointIdentity, int, int> AtMove;

		public Action<EndPointIdentity, int> AtMessage;

		public class Note
		{
			public int Index;

			public static implicit operator Note(int i)
			{
				return new Note { Index = i };
			}
		}

		public class AtArgumentsTuple
		{
			public int X;
			public int Y;
			public int Z;

			// can we to nested arguments?
			public AtArgumentsTuple Next;
			public Note Note;

			public void ApplyToWorld(EndPointIdentity Sender, object World)
			{
				var p = this;

				while (p != null)
				{
					if (p.Note != null)
					{
						Console.WriteLine("Home with note index " + p.Note.Index);
					}

					Console.WriteLine("Home know as " + new { p.X, p.Y, p.Z });
					p = p.Next;
				}
			}
		}

		public Action<EndPointIdentity, AtArgumentsTuple> AtArguments;


		// this should be implementation detail not visible to here...
		public class EndPointIdentity
		{
			// the next step would to introduce virtual players
			// for spilt screen gameplay and alike


			public int Index;

			public int Frame;
		}

		public void ToWindow()
		{
			TheCanvasExtensions.OpenWindow();
		}

		public ProgramDefinition()
		{
			//new Thread(ToWindow) { ApartmentState = ApartmentState.STA, IsBackground = true }.Start();


			#region act upon events
			this.AtUserInput =
				(sender, c) =>
				{
					// lets mutate our world

					if (c == 'w')
						Console.ForegroundColor = ConsoleColor.White;
					else if (c == 'y')
						Console.ForegroundColor = ConsoleColor.Yellow;

					Console.WriteLine("#" + this.Identity.Frame + " key: " + c);
				};

			this.AtMove =
				(sender, x, y) =>
				{
					Console.WriteLine("#" + this.Identity.Frame + " move x: " + x + ", y: " + y);
				};

			this.AtMessage =
				(Sender, MessageIndex) =>
				{
					if (MessageIndex == 1)
					{
						Console.WriteLine("hello world!");
						return;
					}
					Console.WriteLine("how is network programming going?");
				};

			this.AtArguments =
				(Sender, Arguments) => Arguments.ApplyToWorld(Sender, this);

			
			#endregion


			#region translate user input to events
			this.ConsoleKeyPressed +=
				c =>
				{
					if (c.Key == ConsoleKey.LeftArrow)
					{
						this.AtMove(null, -1, 0);
						return;
					}

					if (c.Key == ConsoleKey.RightArrow)
					{
						this.AtMove(null, 1, 0);
						return;
					}

					if (c.Key == ConsoleKey.UpArrow)
					{
						this.AtMove(null, 0, -1);
						return;
					}

					if (c.Key == ConsoleKey.DownArrow)
					{
						this.AtMove(null, 0, 1);
						return;
					}

					if (c.Key == ConsoleKey.PageUp)
					{
						this.AtMessage(null, 1);
						return;
					}

					if (c.Key == ConsoleKey.PageDown)
					{
						this.AtMessage(null, 2);
						return;
					}

					if (c.Key == ConsoleKey.Home)
					{
						this.AtArguments(null, new AtArgumentsTuple { X = 45, Z = 678, Y = 45435, Next = null });
						return;
					}

					if (c.Key == ConsoleKey.End)
					{
						this.AtArguments(null, new AtArgumentsTuple { X = 45, Z = 678, Y = 45435, Note = 666, Next = new AtArgumentsTuple { X = 3, Y = 4, Z = 5 } });
						return;
					}

					// we are calling this delegate
					// to allow frame sync as the implementation
					// will not be called before it's time in the future
					this.AtUserInput(null, c.KeyChar);
				};
			#endregion


		}



	}



}
