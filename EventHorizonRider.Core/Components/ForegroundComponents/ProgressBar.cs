using EventHorizonRider.Core.Graphics;
using EventHorizonRider.Core.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EventHorizonRider.Core.Components.ForegroundComponents
{
    internal class ProgressBar : ComponentBase
    {
        private float progress;
        private Texture2D progressBar;
        private Vector2 position;
        private Vector2 scale;

        public void Initialize(Vector2 initPosition, Vector2 size)
        {
            position = initPosition;
            scale = size;
        }

        public void SetProgress(float newProgress)
        {
            progress = newProgress;
        }

        protected override void LoadContentCore(ContentManager content, GraphicsDevice graphics)
        {
            progressBar = new Texture2D(graphics, 1, 1);
            progressBar.SetData(new[] { Color.White });
        }

        protected override void DrawCore(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                progressBar,
                position: new Vector2(position.X + 2, position.Y + 2),
                color: Color.Black,
                scale: scale,
                depth: Depth);

            spriteBatch.Draw(
                progressBar,
                position: position,
                color: Color.DarkGray.AdjustLight(0.5f),
                scale: scale,
                depth: Depth + 0.00001f);

            spriteBatch.Draw(
                progressBar,
                position: position,
                color: Color.Green,
                scale: new Vector2(MathHelper.Lerp(0f, scale.X, progress), scale.Y),
                depth: Depth + 0.00002f);
        }
    }
}
