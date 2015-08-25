using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace OrcasSimpleWindowsFormsApplication1
{
	using Sender = Object;

	public partial class Form1 : Form
	{
		public class MessageArguments
		{
			// using integers to specify messages enables less bandwidth
			public int Index;

			public string Text;
		}

		public Action<Sender, MessageArguments> AtMessage1;
		public Action<Sender, MessageArguments> AtMessage2;
		public Action<Sender, MessageArguments> AtMessage;

		public Form1()
		{
			InitializeComponent();

			this.AtMessage1 =
				(sender, i) =>
				{
					button3.Text = "Message: 1";
				};

			this.AtMessage2 =
				(sender, i) =>
				{

					button3.Text = "Message: 2 + " + i.Text;
				};

			this.AtMessage =
				(sender, i) =>
				{
					button3.Text = "Message: " + i.Index;
				};
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.AtMessage1(null, null);
			//MessageBox.Show("hello world");
		}

		private void button2_Click(object sender, EventArgs e)
		{
			this.AtMessage2(null, new MessageArguments { Index = 7,

				Text = "hi there! 7 here!" 

				// will this crash? yes
				//Text = null
			
			});

			//MessageBox.Show("hi");

			// http://www.codeproject.com/KB/edit/InputBox.aspx
			// InputBox

		}

		private void button3_Click(object sender, EventArgs e)
		{
			new Form2().Show();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			var context = new[]
			{
				panel1,
				panel2,
				panel3,
				panel4,
				panel5,
				panel6,
			};

			var _BackColor = context[0].BackColor;

			for (int i = 0; i < context.Length - 1; i++)
			{
				context[i].BackColor = context[i + 1].BackColor;
			}

			context[context.Length - 1].BackColor = _BackColor;
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			MessageBox.Show(
@"Contact jsc developers and ask for it!

Development blog:
http://zproxy.wordpress.com");
		}

		private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			MessageBox.Show(
@"We could provide more controls just for you! Ask for more features!

Development blog:
http://zproxy.wordpress.com");
		}

		private void button4_Click(object sender, EventArgs e)
		{
			this.AtMessage(null, new MessageArguments { Index = int.MaxValue });

		}


	}
}
