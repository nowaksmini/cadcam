using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CADCAM
{
    /// <summary>
    /// Base class for elements drawn with spritebatch.
    /// </summary>
    public class Component
    {
        public Texture2D Texture { get; set; }
        public Color Color { get; set; }
        public Vector2 Position { get; set; }
        public float Angle { get; set; }
        public Vector2 Scale { get; set; }
        private const float AngleScale = MathHelper.Pi * 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="Component"/> class.
        /// </summary>
        public Component() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Component"/> class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="color">The color.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="angle">The angle.</param>
        public Component(Texture2D texture, Color color, Vector2 position, Vector2 scale, float angle)
        {
            Texture = texture;
            Color = color;
            Position = position;
            Scale = scale;
            Angle = angle;
        }

        /// <summary>
        /// Draws the specified sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2((float)Texture.Width / 2, (float)Texture.Height / 2);
            spriteBatch.Draw(Texture, Position, sourceRectangle, Color,
             Angle % AngleScale, origin, Scale, SpriteEffects.None, 0f);
        }
    }
}

