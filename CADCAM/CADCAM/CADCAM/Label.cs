using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CADCAM
{
    /// <summary>
    /// Simple label without texture, only string drawing enabled.
    /// </summary>
    public class Label : Component
    {
        public String Text { get; set; }
        private readonly SpriteFont _spriteFont;

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="spriteFont">The sprite font.</param>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="angle">The angle.</param>
        public Label(SpriteFont spriteFont, String text, Color color, Vector2 position, Vector2 scale,
            float angle)
        {
            Text = text;
            _spriteFont = spriteFont;
            Color = color;
            Position = position;
            Scale = scale;
            Angle = angle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        /// <param name="spriteFont">The sprite font.</param>
        /// <param name="text">The text.</param>
        /// <param name="color">The color.</param>
        public Label(SpriteFont spriteFont, String text, Color color)
        {
            _spriteFont = spriteFont;
            Text = text;
            Color = color;
            Position = new Vector2(0, 0);
            Scale = new Vector2(1, 1);
            Angle = 0;
        }

        /// <summary>
        /// Draws the specified sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(0, 0);
            spriteBatch.DrawString(_spriteFont, Text, Position, Color, Angle, origin, Scale, 
                SpriteEffects.None, 0);
        }
    }
}