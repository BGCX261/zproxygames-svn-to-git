using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bulldog.Promotion;
using System.Net;
using ScriptCoreLib.Shared.Lambda;
using ScriptCoreLib.Shared.Avalon.Extensions;
using System.Windows.Media.Imaging;

namespace Bulldog.Client.Promotion.Controls
{
	public class GameMenuWithGames : GameMenu
	{
		public event Action DownloadStarting;
		public event Action DownloadComplete;

		public GameMenuWithGames(int Width, int Height, int ShadowHeight)
			: base(Width, Height, ShadowHeight)
		{

		}

		/// <summary>
		/// The promotion server knows about the latest games. We want to know it too. This method will
		/// start the download and eventually add loaded images into the caraousel. If the application is
		/// offline a partial dataset embedded into current build shall be used.
		/// </summary>
		public void Download()
		{
			var wc = new WebClient();

			wc.DownloadStringCompleted +=
				(sender, args) =>
				{

					this.Container.Dispatcher.Invoke(
						(Action)
						delegate
						{
							if (DownloadStarting != null)
								DownloadStarting();
						}
					);

					var a = GameReferenceExtensions.Partial;

					a.ForEach(k => k.Source = k.Image.Substring(GameReferenceExtensions.Host.Length + 1));

					if (args.Error == null)
						a = a.FromYAML(args.Result);

					// we should only take random 10 ?

					var DownloadComplete = this.DownloadComplete.WhereCounter(a.Where(k => k.Source != null).Count() - 1);

					this.Container.Dispatcher.Invoke(
						(Action)
						delegate
						{

							a.ForEach(
								k =>
								{
									if (k.Source != null)
									{
										this.Add(
											new Option
											{
												Text = "Play " + k.Title + "!",
												Source = (k.Source).ToSource(),
												Hyperlink = new Uri(k.Link),
												MarginAfter = Math.PI / 3
											}
										);
									}
									else
									{
										var s = new BitmapImage();

										s.BeginInit();
										s.UriSource = new Uri(k.Image);
										s.EndInit();

										s.DownloadCompleted +=
											delegate
											{
												this.Add(
													new Option
													{
														Text = "Play " + k.Title + "!",
														Source = s,
														Hyperlink = new Uri(k.Link),
														MarginAfter = Math.PI / 3
													}
												);

												DownloadComplete();
											};
									}
								}
							);
						}
					);
				};

			wc.DownloadStringAsync(new Uri(GameReferenceExtensions.Host + GameReferenceExtensions.Path));
		}
	}
}
