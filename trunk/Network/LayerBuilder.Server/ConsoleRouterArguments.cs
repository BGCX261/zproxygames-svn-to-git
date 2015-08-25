using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LayerBuilder.Server
{
	public class ConsoleRouterArguments
	{
		public int ServerPort = 2000;


		public Trunk[] Trunk;
	}

	public class Trunk
	{
		public int TrunkPort;
	}
}
