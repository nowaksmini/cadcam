using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CADCAM
{
    public class Button : Component
    {
        private BState _state;
        private double _timer;
        private readonly Color _pressedColor = Color.DarkSlateBlue;
        private readonly Color _hoverColor = Color.LightBlue;
        private Color _normalColor = Color.White;
        private bool _mpressed, _prevMpressed;
        private int _mx, _my;
        private double _frameTime;
        /// <summary>
        /// The click function.
        /// </summary>
        public Action Click;
        private readonly Label _textLabel;

        public String Text
        {
            get { return _textLabel.Text; }
            set { _textLabel.Text = value; }
        }

        public Color NormalColor { set { _normalColor = value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="texture">The texture.</param>
        /// <param name="color">The color.</param>
        /// <param name="position">The position.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="timer">The timer.</param>
        /// <param name="textColor">Color of the text.</param>
        /// <param name="spriteFont">The sprite font.</param>
        /// <param name="text">The text.</param>
        public Button(BState state, Texture2D texture, Color color, Vector2 position, Vector2 scale, float angle,
            double timer, Color textColor, SpriteFont spriteFont = null, String text = "")
            : base(texture, color, position, scale, angle)
        {
            _textLabel = new Label(spriteFont, text, textColor, position, scale, angle);
            _state = state;
            _timer = timer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        /// <param name="color">The color.</param>
        /// <param name="spriteFont">The sprite font.</param>
        /// <param name="text">The text.</param>
        /// <param name="textColor">Color of the text.</param>
        public Button(Texture2D texture, Color color, SpriteFont spriteFont, String text, Color textColor)
            : base(texture, color, new Vector2(0, 0), new Vector2(1, 1), 0f)
        {
            _textLabel = new Label(spriteFont, text, textColor, new Vector2(0, 0), new Vector2(1, 1), 0);
            _state = BState.Up;
            _timer = 2f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Button"/> class.
        /// </summary>
        public Button()
        {
            Color = _normalColor;
            _state = BState.Up;
            _timer = 0.0;
        }

        /// <summary>
        /// Updates the state of button depends of mouse position on screen.
        /// </summary>
        /// <param name="mx">The mx of mouse position.</param>
        /// <param name="my">My of mouse position.</param>
        /// <param name="frameTime">The frame time.</param>
        /// <param name="mPressed">if set to <c>true</c> [mouse pressed].</param>
        /// <param name="prevMPressed">if set to <c>true</c> [previous mouse pressed].</param>
        public void Update(int mx, int my, double frameTime, bool mPressed, bool prevMPressed)
        {
            _mx = mx;
            _my = my;
            _frameTime = frameTime;
            _mpressed = mPressed;
            _prevMpressed = prevMPressed;
            if (CheckIfButtonContainsPoint(_mx, _my))
            {
                _timer = 0.0;
                if (_mpressed)
                {
                    _state = BState.Down;
                    Color = _pressedColor;
                }
                else if (!_mpressed && _prevMpressed)
                {
                    if (_state == BState.Down)
                    {
                        _state = BState.JustReleased;
                    }
                }
                else
                {
                    _state = BState.Hover;
                    Color = _hoverColor;
                }
            }
            else
            {
                _state = BState.Up;
                if (_timer > 0)
                {
                    _timer = _timer - _frameTime;
                }
                else
                {
                    Color = _normalColor;
                }
            }
            if (_state == BState.JustReleased)
            {
                OnClick(_timer);
            }
            if (_textLabel != null && _textLabel.Text.Length > 0)
            {
                _textLabel.Position = new Vector2(Position.X - Texture.Width * Scale.X / 2,
                    Position.Y - Texture.Height * Scale.Y / 2);
            }
        }

        /// <summary>
        /// Draws the specified sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (_textLabel != null && _textLabel.Text.Length > 0)
            {
                _textLabel.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Called when [click].
        /// </summary>
        /// <param name="timer">The timer.</param>
        public void OnClick(double timer)
        {
            _timer = timer;
            //Color = Color.Green;
            if (Click != null)
            {
                Click();
            }
        }

        /// <summary>
        /// Checks if button contains point.
        /// </summary>
        /// <param name="x">The x position of point.</param>
        /// <param name="y">The y position of point.</param>
        /// <returns></returns>
        private bool CheckIfButtonContainsPoint(int x, int y)
        {
            return CheckIfRectangleContainsPoint(0, 0,
                (int)((x - (Position.X - Texture.Width * Scale.X / 2)) / Scale.X),
                (int)((y - (Position.Y - Texture.Height * Scale.Y / 2)) / Scale.Y));
        }

        /// <summary>
        /// Checks if texture contains point.
        /// </summary>
        /// <param name="tx">The tx of texture.</param>
        /// <param name="ty">The ty of texture.</param>
        /// <param name="x">The x of point.</param>
        /// <param name="y">The y of point.</param>
        /// <returns></returns>
        private bool CheckIfTextureContainsPoint(float tx, float ty, int x, int y)
        {
            return (x >= tx &&
                    x <= tx + Texture.Width &&
                    y >= ty &&
                    y <= ty + Texture.Height);
        }

        /// <summary>
        /// Checks if rectangle of button contains point.
        /// </summary>
        /// <param name="tx">The tx of rectangle.</param>
        /// <param name="ty">The ty of rectangle.</param>
        /// <param name="x">The x of point.</param>
        /// <param name="y">The y of point.</param>
        /// <returns></returns>
        private bool CheckIfRectangleContainsPoint(float tx, float ty, int x, int y)
        {
            if (CheckIfTextureContainsPoint(tx, ty, x, y))
            {
                uint[] data = new uint[Texture.Width * Texture.Height];
                Texture.GetData(data);
                if ((x - (int)tx) + (y - (int)ty) *
                    Texture.Width < Texture.Width * Texture.Height)
                {
                    return ((data[(x - (int)tx) + (y - (int)ty) * Texture.Width] & 0xFF000000) >> 24) > 20;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Enum for representing button state.
    /// </summary>
    public enum BState
    {
        Hover,
        Up,
        JustReleased,
        Down
    }
}
