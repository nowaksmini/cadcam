using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CADCAM
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CadCamGame : Game
    {
        readonly GraphicsDeviceManager _graphics;
        private SnakeEngine _snakeEngine;
        private BasicEffect _basicEffect;
        private SpriteBatch _spriteBatch;

        public CadCamGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = "Cad/Cam Snake";
            Window.AllowUserResizing = true;
            //_graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferHeight = 740;
            _graphics.PreferredBackBufferWidth = 1080;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.SteelBlue);
            //_basicEffect = new BasicEffect(GraphicsDevice);

            _snakeEngine.CreateVertexBuffer(_graphics);
            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.None;
            _graphics.GraphicsDevice.RasterizerState = rasterizerState1;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            
            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphics.GraphicsDevice.DrawPrimitives(
                    PrimitiveType.TriangleList,
                    0,  // start vertex
                    8 * SnakeEngine.NumberOfFigures * 2 // number of primitives to draw
                );
            }
            _spriteBatch.Begin();
            _snakeEngine.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void Initialize()
        {
            _basicEffect = new BasicEffect(GraphicsDevice);
            base.Initialize();
        }

        
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            float width = GraphicsDevice.Viewport.Width;
            float height = GraphicsDevice.Viewport.Height;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D buttonTexture2D = Content.Load<Texture2D>("button");
            SpriteFont buttonSpriteFont = Content.Load<SpriteFont>("Input");
            _snakeEngine = new SnakeEngine(buttonTexture2D, buttonSpriteFont);
            _snakeEngine.InitializeTransform(width, height);
            Texture2D texture = Content.Load<Texture2D>("logo");
            _snakeEngine.InitializeEffect(_basicEffect, texture);
            //_snakeEngine.InitializeTetrahedron(0);
        }


        protected override void UnloadContent()
        {

        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            _snakeEngine.Update(gameTime, _basicEffect, Window.ClientBounds.Width, Window.ClientBounds.Height);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed)
                Exit();
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            base.Update(gameTime);
        }

    }
}
