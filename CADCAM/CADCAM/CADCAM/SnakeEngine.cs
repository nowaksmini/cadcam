using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CADCAM
{
    /// <summary>
    /// Engine repsonsible for buttons handling and showing snake.
    /// </summary>
    public class SnakeEngine
    {
        private readonly float _tolerance = 0.01f;
        public const int NumberOfFigures = 9;
        private Triangle[] _cubeVertices;
        private Matrix _worldMatrix;
        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;
        private const int NumberOfVertices = 24*NumberOfFigures;
        private VertexBuffer _vertexBuffer;
        private Button _zoomInButton;
        private Button _zoomOutButton;
        private Button _rightButton;
        private Button _leftButton;
        private Button _upButton;
        private Button _downButton;
        private Button _rotationButton;
        private Button[] _angleButtons;
        private const float PositionChange = 0.5f;
        private int _angleChange = 10;
        private float _xPerspective;
        private float _yPerspective;
        private float _zoom;
        private bool _mousePressed, _prevMousePressed;
        private readonly Button[] _rightRotationButtons;
        private readonly Button[] _leftRotationButtons;
        private readonly Label _angleLabel;
        private readonly Button _clearButton;
        private bool _rightDirection = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="SnakeEngine"/> class.
        /// </summary>
        public SnakeEngine(Texture2D buttonTexture2D, SpriteFont buttonTextSpriteFont)
        {
            _zoom = 10f;
            _angleLabel = new Label(buttonTextSpriteFont, "Degrees " , Color.Black);
            _rightRotationButtons = new Button[NumberOfFigures - 1];
            _leftRotationButtons = new Button[NumberOfFigures];
            _cubeVertices = new Triangle[NumberOfVertices*2];
            for (int i = 0; i < NumberOfFigures; i++)
            {
                InitializeTetrahedron(i, true);
                InitializeTetrahedron(i, false);
                var i1 = i;
                if (i != NumberOfFigures)
                {
                    _leftRotationButtons[i] = new Button(buttonTexture2D, Color.White,
                        buttonTextSpriteFont, "L" + (i + 1), Color.Black)
                    {
                        Click = delegate { Rotate(FindCenterOfRotation(false, i1), i1, false); }
                    };
                }
                if (i == 0) continue;
                _rightRotationButtons[i - 1] = new Button(buttonTexture2D, Color.White,
                    buttonTextSpriteFont, "R" + i, Color.Black)
                {
                    Click = delegate { Rotate(FindCenterOfRotation(true, i1), i1, true); }
                };
            }
            _clearButton = new Button(buttonTexture2D, Color.White,
                    buttonTextSpriteFont, "CLEAR", Color.Black)
                {
                    Click = delegate
                    {
                        for (int i = 0; i < NumberOfFigures; i++)
                        {
                            InitializeTetrahedron(i, true);
                            InitializeTetrahedron(i, false);
                        }
                        _angleChange = 10;
                    }
                };
            GenerateClickListeners(buttonTexture2D, buttonTextSpriteFont);
        }


        /// <summary>
        /// Generates the click listeners for all position buttons.
        /// </summary>
        /// <param name="buttonTexture2D">The button texture2d.</param>
        /// <param name="buttonTextSpriteFont">The button text sprite font.</param>
        private void GenerateClickListeners(Texture2D buttonTexture2D, SpriteFont buttonTextSpriteFont)
        {
            Vector2 scale = new Vector2(0.2f, 0.1f);
            _angleButtons = new Button[2];

            _angleButtons[0] = new Button(buttonTexture2D, Color.White, buttonTextSpriteFont,
                "+", Color.Black)
            {
                Click = delegate
                {
                    _angleChange += 1;
                },
            };
            _angleButtons[1] = new Button(buttonTexture2D, Color.White, buttonTextSpriteFont,
                "-", Color.Black)
            {
                Click = delegate
                {
                    _angleChange -= 1;
                },
            };
            
            _rotationButton = new Button(buttonTexture2D, Color.White, buttonTextSpriteFont,
                "ROTATE RIGHT", Color.Black)
            {
                Click = delegate
                {
                    _rightDirection = !_rightDirection;
                    _rotationButton.Text = _rightDirection ? "ROTATE RIGHT" : "ROTATE LEFT";
                },
            };
            _zoomInButton = new Button(buttonTexture2D, Color.White, buttonTextSpriteFont,
                "ZOOM IN", Color.Black)
            {
                Click = delegate { _zoom -= PositionChange; },
                Scale = scale
            };
            _zoomOutButton = new Button(buttonTexture2D, Color.White, buttonTextSpriteFont,
                "ZOOM OUT", Color.Black)
            {
                Click = delegate { _zoom += PositionChange; },
            };
            _rightButton = new Button(buttonTexture2D, Color.White, buttonTextSpriteFont,
                "RIGHT", Color.Black)
            {
                Click = delegate { _xPerspective += PositionChange; },
                Scale = scale
            };
            _leftButton = new Button(buttonTexture2D, Color.White, buttonTextSpriteFont,
                "LEFT", Color.Black)
            {
                Click = delegate { _xPerspective -= PositionChange; },
                Scale = scale
            };
            _upButton = new Button(buttonTexture2D, Color.White, buttonTextSpriteFont,
                "UP", Color.Black)
            {
                Click = delegate { _yPerspective += PositionChange; },
                Scale = scale
            };
            _downButton = new Button(buttonTexture2D, Color.White, buttonTextSpriteFont,
                "DOWN", Color.Black)
            {
                Click = delegate { _yPerspective -= PositionChange; },
                Scale = scale
            };
        }

        /// <summary>
        /// Finds the center of rotation and rotation axis.
        /// </summary>
        /// <param name="isRight">if set to <c>true</c> [rotation is right direction].</param>
        /// <param name="figure">The figure number.</param>
        /// <returns>Two points on rotation axis, one on right face other on bottom/top</returns>
        private Vector3[] FindCenterOfRotation(bool isRight, int figure)
        {
            Vector3[] points = new Vector3[2];
            if (isRight)
                figure--;
            Vector3 leftFront;
            Vector3 centerFront;
            Vector3 rightBack;
            if (isRight)
            {
                leftFront = _cubeVertices[8 + NumberOfVertices + figure*24].Position;
                centerFront = _cubeVertices[11 + NumberOfVertices + figure*24].Position;
                rightBack = _cubeVertices[5 + NumberOfVertices + figure*24].Position;
            }
            else
            {
                leftFront = _cubeVertices[8 + figure*24].Position;
                centerFront = _cubeVertices[11 + figure*24].Position;
                rightBack = _cubeVertices[5 + figure*24].Position;
            }

            var x = Math.Min(centerFront.X, rightBack.X) +
                    Math.Abs(centerFront.X - rightBack.X)/2;
            var y = Math.Min(centerFront.Y, rightBack.Y) +
                    Math.Abs(centerFront.Y - rightBack.Y)/2;
            var z = Math.Min(centerFront.Z, rightBack.Z) +
                    Math.Abs(centerFront.Z - rightBack.Z)/2;
            Vector3 rigthFace = new Vector3(x, y, z);
            x = Math.Min(leftFront.X, rightBack.X) +
                Math.Abs(leftFront.X - rightBack.X)/2;
            y = Math.Min(leftFront.Y, rightBack.Y) +
                Math.Abs(leftFront.Y - rightBack.Y)/2;
            z = Math.Min(leftFront.Z, rightBack.Z) +
                Math.Abs(leftFront.Z - rightBack.Z)/2;
            Vector3 upperFace = new Vector3(x, y, z);
            points[0] = rigthFace;
            points[1] = upperFace;
            return points;
        }

        /// <summary>
        /// Rotates figure from specified base tetrahedron around axis 
        /// through middle of one of "side's" figures.
        /// </summary>
        /// <param name="rotationMiddle">The rotation middle's.</param>
        /// <param name="figure">The start figure number.</param>
        /// <param name="isRight">if set to <c>true</c> [rotation is right direction].</param>
        private void Rotate(Vector3[] rotationMiddle, int figure, bool isRight)
        {
            Triangle[] copy = (Triangle[]) _cubeVertices.Clone();
            for (int i = 0 + figure*24; i < NumberOfVertices; i++)
            {
                /* create matrix and rotate point */
                float angle = _rightDirection ? MathHelper.ToRadians(_angleChange) : MathHelper.ToRadians(-_angleChange);
                Vector3 rightFace = rotationMiddle[0];
                Vector3 upperFace = rotationMiddle[1];
                Vector3 directionRotation = upperFace - rightFace;
                RotationMatrix rotationMatrix = new RotationMatrix(rightFace.X, rightFace.Y, rightFace.Z,
                    directionRotation.X, directionRotation.Y, directionRotation.Z, angle);
                float[][] rotationData = rotationMatrix.GetMatrix();
                Matrix m = new Matrix(rotationData[0][0], rotationData[0][1], rotationData[0][2], rotationData[0][3],
                    rotationData[1][0], rotationData[1][1], rotationData[1][2], rotationData[1][3],
                    rotationData[2][0], rotationData[2][1], rotationData[2][2], rotationData[2][3],
                    rotationData[3][0], rotationData[3][1], rotationData[3][2], rotationData[3][3]);
                Vector3 basePosition = _cubeVertices[i].Position;
                Vector3 newPosition = basePosition - rotationMiddle[0];
                Vector3.Transform(ref newPosition, ref m, out newPosition);
                if (isRight || (i >= figure*24 + 24))
                    _cubeVertices[i].Position = newPosition + rotationMiddle[0];
                basePosition = _cubeVertices[i + NumberOfVertices].Position;
                newPosition = basePosition - rotationMiddle[0];
                Vector3.Transform(ref newPosition, ref m, out newPosition);
                _cubeVertices[i + NumberOfVertices].Position = newPosition + rotationMiddle[0];
            }
            // check if any figure is overlapping another
            if (CheckIfAnythingIsOverLapping())
            {
                _cubeVertices = copy;
            }
        }

        /// <summary>
        /// Checks if anything is over lapping.
        /// </summary>
        /// <returns></returns>
        private bool CheckIfAnythingIsOverLapping()
        {
            for (int i = 0; i < NumberOfFigures*2; i++)
            {
                // check overlapping figures
                for (int j = i + 2; j < NumberOfFigures*2; j++)
                {
                    if (j <= i + NumberOfFigures + 1 && j >= i + NumberOfFigures - 1)
                        continue;
                    for (int k = 0; k < 22; k += 3)
                    {
                        var isOverLapping = CheckColision(_cubeVertices[i*24 + k].Position,
                            _cubeVertices[i*24 + k + 1].Position,
                            _cubeVertices[i*24 + k + 2].Position,
                            _cubeVertices[j*24 + k].Position,
                            _cubeVertices[j*24 + k + 1].Position,
                            _cubeVertices[j*24 + k + 2].Position);
                        if (isOverLapping)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks the colision of two faces.
        /// </summary>
        /// <param name="t1A">The first point of triangle 1.</param>
        /// <param name="t1B">The second point of triangle 1.</param>
        /// <param name="t1C">The third point of triangle 1.</param>
        /// <param name="t2A">The first point of triangle 2.</param>
        /// <param name="t2B">The second point of triangle 2.</param>
        /// <param name="t2C">The third point of triangle 2.</param>
        /// <returns></returns>
        private bool CheckColision(Vector3 t1A, Vector3 t1B, Vector3 t1C, Vector3 t2A, 
            Vector3 t2B, Vector3 t2C)
        {
            //rotates each edge of the first triangle to the Z axis and 
            //checks the second triangle against it then repeats with the second one against the first, 
            //and lastly checks to see if all points of the second triangle are on the same side as the first
            if (!CheckIfTriagleIsUnderOtherTriangle(t1A, t1B, t1C, t2A, t2B, t2C))
                return false;
            if (!CheckIfTriagleIsUnderOtherTriangle(t1B, t1C, t1A, t2A, t2B, t2C))
                return false;
            if (!CheckIfTriagleIsUnderOtherTriangle(t1C, t1A, t1B, t2A, t2B, t2C))
                return false;
            if (!CheckIfTriagleIsUnderOtherTriangle(t2A, t2B, t2C, t1A, t1B, t1C))
                return false;
            if (!CheckIfTriagleIsUnderOtherTriangle(t2B, t2C, t2A, t1A, t1B, t1C))
                return false;
            if (!CheckIfTriagleIsUnderOtherTriangle(t2C, t2A, t2B, t1A, t1B, t1C))
                return false;

            return CheckIfTriangleIsOnOneSideOfAnotherTriangle(t1A, t1B, t1C, t2A, t2B, t2C);
        }

        /// <summary>
        /// Checks if triangle is on one side of another triangle.
        /// </summary>
        /// <param name="t1A">The first point of triangle 1.</param>
        /// <param name="t1B">The second point of triangle 1.</param>
        /// <param name="t1C">The third point of triangle 1.</param>
        /// <param name="t2A">The first point of triangle 2.</param>
        /// <param name="t2B">The second point of triangle 2.</param>
        /// <param name="t2C">The third point of triangle 2.</param>
        /// <returns></returns>
        private bool CheckIfTriangleIsOnOneSideOfAnotherTriangle(Vector3 t1A, Vector3 t1B, Vector3 t1C, Vector3 t2A, Vector3 t2B,
            Vector3 t2C)
        {
            //simply performs a transformation to check if all points on one triangle 
            //are on the same side of the other triangle
            Matrix m = Matrix.CreateLookAt(t1A, t1B, t1C - t1A);
            t2A = Vector3.Transform(t2A, m);
            t2B = Vector3.Transform(t2B, m);
            t2C = Vector3.Transform(t2C, m);
            //0
            if (t2A.X < _tolerance && t2B.X < _tolerance && t2C.X < _tolerance)
                return false;
            if (-_tolerance < t2A.X && -_tolerance < t2B.X && -_tolerance < t2C.X)
                return false;
            return true;
        }

        /// <summary>
        /// Checks if triagle is under other triangle.
        /// </summary>
        /// <param name="t1A">The first point of triangle 1.</param>
        /// <param name="t1B">The second point of triangle 1.</param>
        /// <param name="t1C">The third point of triangle 1.</param>
        /// <param name="t2A">The first point of triangle 2.</param>
        /// <param name="t2B">The second point of triangle 2.</param>
        /// <param name="t2C">The third point of triangle 2.</param>
        /// <returns></returns>
        private bool CheckIfTriagleIsUnderOtherTriangle(Vector3 t1A, Vector3 t1B, Vector3 t1C, Vector3 t2A, Vector3 t2B,
            Vector3 t2C)
        {
            //performs a transformation and checks if all points 
            //of the one triangle are under the other triangle after the transformation

            Matrix m = Matrix.CreateLookAt(t1A, t1B, t1C - t1A);
            t1A = Vector3.Transform(t1A, m);
            if (_tolerance < Math.Abs(t1A.X) || _tolerance < Math.Abs(t1A.Y) || _tolerance < Math.Abs(t1A.Z))
                return true;
            if (_tolerance < Math.Abs(t1A.X) || _tolerance < Math.Abs(t1A.Y))
                return true;
            if (_tolerance < Math.Abs(t1A.X))
                return true;
            t2A = Vector3.Transform(t2A, m);
            t2B = Vector3.Transform(t2B, m);
            t2C = Vector3.Transform(t2C, m);
            //0
            if (t2A.Y < _tolerance && t2B.Y < _tolerance && t2C.Y < _tolerance)
                return false;
            return true;
        }

        /// <summary>
        /// Initializes the effect (loading, parameter setting, 
        /// and technique selection) used for the 3D model.
        /// </summary>
        public void InitializeEffect(BasicEffect basicEffect, Texture2D texture2D)
        {
            basicEffect.World = _worldMatrix;
            basicEffect.View = _viewMatrix;
            basicEffect.Projection = _projectionMatrix;
            basicEffect.Texture = texture2D;
            basicEffect.TextureEnabled = true;
        }

        /// <summary>
        /// Initializes the transforms used for the 3D model.
        /// </summary>
        public void InitializeTransform(float width, float height)
        {
            float tilt = (float) Math.PI/8.0f;
            _worldMatrix = Matrix.CreateRotationX(tilt)*Matrix.CreateRotationY(tilt);

            _viewMatrix = Matrix.CreateLookAt(new Vector3(_xPerspective, _yPerspective, _zoom),
                Vector3.Zero, Vector3.Up);
            _projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                (float) Math.PI/4.0f,
                2f,
                1f, 100.0f);
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="basicEffect">The basic effect.</param>
        /// <param name="windowWidth">Width of the window.</param>
        /// <param name="windowHeight">Height of the window.</param>
        public void Update(GameTime gameTime, BasicEffect basicEffect, int windowWidth, int windowHeight)
        {
            double frameTime = gameTime.ElapsedGameTime.Milliseconds/1000.0;
            MouseState mouseState = Mouse.GetState();
            _prevMousePressed = _mousePressed;
            _mousePressed = mouseState.LeftButton == ButtonState.Pressed;
            _zoomOutButton.Update(mouseState.X, mouseState.Y, frameTime, _mousePressed, _prevMousePressed);
            _zoomOutButton.Position = new Vector2(207, 30);
            _zoomOutButton.Scale = new Vector2(0.26f, 0.1f);
            _rotationButton.Position = new Vector2(110, 150);
            _rotationButton.Scale = new Vector2(0.37f, 0.1f);
            _rotationButton.Update(mouseState.X, mouseState.Y, frameTime, _mousePressed, _prevMousePressed);
            _zoomInButton.Position = new Vector2(60, 30);
            _zoomInButton.Update(mouseState.X, mouseState.Y, frameTime, _mousePressed, _prevMousePressed);
            _leftButton.Position = new Vector2(60, 70);
            _leftButton.Update(mouseState.X, mouseState.Y, frameTime, _mousePressed, _prevMousePressed);
            _rightButton.Position = new Vector2(190, 70);
            _rightButton.Update(mouseState.X, mouseState.Y, frameTime, _mousePressed, _prevMousePressed);
            _upButton.Position = new Vector2(60, 110);
            _upButton.Update(mouseState.X, mouseState.Y, frameTime, _mousePressed, _prevMousePressed);
            _downButton.Position = new Vector2(190, 110);
            _downButton.Update(mouseState.X, mouseState.Y, frameTime, _mousePressed, _prevMousePressed);
            int x = windowWidth - 40;
            int y = 30;
            foreach (var button in _rightRotationButtons)
            {
                button.Position = new Vector2(x, y);
                button.Scale = new Vector2(0.1f, 0.1f);
                button.Update(mouseState.X, mouseState.Y, frameTime, _mousePressed, _prevMousePressed);
                y += 40;
            }
            x = windowWidth - 120;
            y = 30;
            foreach (var button in _leftRotationButtons)
            {
                button.Position = new Vector2(x, y);
                button.Scale = new Vector2(0.1f, 0.1f);
                button.Update(mouseState.X, mouseState.Y, frameTime, _mousePressed, _prevMousePressed);
                y += 40;
            }
            y = 180;
            x = 2;
            _angleLabel.Position = new Vector2(x,y);
            _angleLabel.Text = "Degrees " + _angleChange;
            x = 20;
            y = 230;
            foreach (var button in _angleButtons)
            {
                button.Position = new Vector2(x, y);
                button.Scale = new Vector2(0.05f, 0.1f);
                button.Update(mouseState.X, mouseState.Y, frameTime, _mousePressed, _prevMousePressed);
                x += 40;
            }
            _clearButton.Position = new Vector2(60, 270);
            _clearButton.Scale = new Vector2(0.2f, 0.1f);
            _clearButton.Update(mouseState.X, mouseState.Y, frameTime, _mousePressed, _prevMousePressed);
            _viewMatrix = Matrix.CreateLookAt(new Vector3(_xPerspective, _yPerspective, _zoom),
                Vector3.Zero, Vector3.Up);
            basicEffect.View = _viewMatrix;
        }

        /// <summary>
        /// Draws the specified sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch.</param>
        /// <param name="gameTime">The game time.</param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _clearButton.Draw(spriteBatch);
            _angleLabel.Draw(spriteBatch);
            _rotationButton.Draw(spriteBatch);
            _zoomInButton.Draw(spriteBatch);
            _zoomOutButton.Draw(spriteBatch);
            _rightButton.Draw(spriteBatch);
            _leftButton.Draw(spriteBatch);
            _upButton.Draw(spriteBatch);
            _downButton.Draw(spriteBatch);
            foreach (var button in _rightRotationButtons)
            {
                button.Draw(spriteBatch);
            }
            foreach (var button in _leftRotationButtons)
            {
                button.Draw(spriteBatch);
            }
            foreach (var button in _angleButtons)
            {
                button.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Creates the vertex buffer.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        public void CreateVertexBuffer(GraphicsDeviceManager graphics)
        {
            _vertexBuffer = new VertexBuffer(
                graphics.GraphicsDevice,
                typeof (Triangle),
                NumberOfVertices*2,
                BufferUsage.None
                );

            _vertexBuffer.SetData(_cubeVertices);

            graphics.GraphicsDevice.SetVertexBuffer(_vertexBuffer);
        }

        /// <summary>
        /// Initializes the tetrahedron.
        /// </summary>
        /// <param name="figure">The figure number.</param>
        /// <param name="isUp">Define direction of bottom of figure</param>
        public void InitializeTetrahedron(int figure, bool isUp)
        {
            float sqrt = (float) (Math.Sqrt(2));
            float distance = figure*2f*sqrt;

            Vector2 textureLeftTop = new Vector2(0.0f, 0.0f);
            Vector2 textureLeftBottom = new Vector2(0.0f, 1.0f);
            Vector2 textureRightTop = new Vector2(1.0f, 0.0f);
            Vector2 textureRightBottom = new Vector2(1.0f, 1.0f);
            Vector3 leftFront = new Vector3(-1 + distance, -1, -1);
            Vector3 leftBack = new Vector3(-1 + distance, 1, -1);
            Vector3 rightFront = new Vector3(-1 + 2*sqrt + distance, -1, -1);
            Vector3 rightBack = new Vector3(-1 + 2*sqrt + distance, 1, -1);
            Vector3 centerFront = new Vector3(-1 + sqrt + distance, -1, -1 + sqrt);
            Vector3 centerBack = new Vector3(-1 + sqrt + distance, 1, -1 + sqrt);

            if (isUp)
            {
                leftFront = centerFront;
                leftBack = centerBack;
                centerFront = rightFront;
                centerBack = rightBack;
                rightFront = leftFront + new Vector3(2*sqrt, 0, 0);
                rightBack = leftBack + new Vector3(2*sqrt, 0, 0);
            }

            //Front 
            _cubeVertices[0 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(leftFront, textureLeftBottom);
            _cubeVertices[1 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(centerFront, textureRightBottom);
            _cubeVertices[2 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(rightFront, textureLeftTop);

            //Back
            _cubeVertices[3 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(leftBack, textureLeftBottom);
            _cubeVertices[4 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(centerBack, textureRightBottom);
            _cubeVertices[5 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(rightBack, textureLeftTop);

            //Right
            _cubeVertices[6 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(leftBack, textureLeftBottom);
            _cubeVertices[7 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(centerBack, textureRightBottom);
            _cubeVertices[8 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(leftFront, textureLeftTop);
            _cubeVertices[9 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(leftFront, textureLeftTop);
            _cubeVertices[10 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(centerBack, textureRightBottom);
            _cubeVertices[11 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(centerFront, textureRightTop);

            //Left
            _cubeVertices[12 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(rightBack, textureLeftBottom);
            _cubeVertices[13 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(centerBack, textureRightBottom);
            _cubeVertices[14 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(rightFront, textureLeftTop);
            _cubeVertices[15 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(rightFront, textureLeftTop);
            _cubeVertices[16 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(centerBack, textureRightBottom);
            _cubeVertices[17 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(centerFront, textureRightTop);

            //Down
            _cubeVertices[18 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(rightBack, textureLeftBottom);
            _cubeVertices[19 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(leftBack, textureRightBottom);
            _cubeVertices[20 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(rightFront, textureLeftTop);
            _cubeVertices[21 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(rightFront, textureLeftTop);
            _cubeVertices[22 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(leftBack, textureRightBottom);
            _cubeVertices[23 + figure*24 + (isUp ? NumberOfVertices : 0)] = new Triangle(leftFront, textureRightTop);
        }
    }
}
