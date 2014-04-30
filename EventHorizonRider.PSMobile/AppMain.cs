using System;
using System.Collections.Generic;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using EventHorizonRider.Core;

namespace EventHorizonRider.PSMobile
{
	public class AppMain
	{
		private static MainGame game;
		
		public static void Main (string[] args)
		{
			game = new MainGame();
			game.DetailLevel = DetailLevel.CollisionDetectionFull | DetailLevel.PixelShaderEffectsNone;
			
			while (true) {
				game.Run();
			}
		}
	}
}
