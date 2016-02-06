
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PicselParticles
{
    public class Game1 : Game
    {
        // Input state.
        KeyboardState currentKeyboardState;
        GamePadState currentGamePadState;
        KeyboardState lastKeyboardState;
        GamePadState lastGamePadState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BasicEffect effect;

        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;
        Matrix scaleMarix = Matrix.Identity;
        Matrix translationMatrix = Matrix.Identity;
        Matrix rotationMatrix = Matrix.Identity;
        Matrix CameraRotationMatrix = Matrix.Identity;

        VertexPositionColor[] v;
        int[] PiramidaIndices;
        IndexBuffer indexBuffer;
        VertexBuffer vertexBuffer;

        private float cameraArc;
        private float cameraRotation;
        private float cameraDistance;

        public Game1() : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            cameraArc = -5;
            cameraRotation = 0;
            cameraDistance = 50;
            // graphics.IsFullScreen = true;
            graphics.PreferredBackBufferHeight = 700;
            graphics.PreferredBackBufferWidth = 800;
            Window.Title = "Kezumie";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                (float)Window.ClientBounds.Width /
                (float)Window.ClientBounds.Height,
                1.5f, 200);
            worldMatrix = Matrix.CreateWorld(new Vector3(0f, 0f, 0f), new Vector3(0, 0, -1), Vector3.Up);

            v = new VertexPositionColor[12000000];
            PiramidaIndices = new int[v.Length * 3];
            Random rnd = new Random();
            for (int i = 0; i < v.Length; ++i)
            {
                Color c1 = Color.Orange;
                Color c2 = Color.Orange;
                Color c3 = Color.Orange;
                Color c4 = Color.Orange;

                //Диапазон координат от -3 до 3
                float x = (float)(rnd.NextDouble() - rnd.NextDouble()) * 9;
                float y = (float)(rnd.NextDouble() - rnd.NextDouble()) * 9;
                float z = (float)(rnd.NextDouble() - rnd.NextDouble()) * 9;
                float f = 0.01f;

                if ((i * 4 + 3) < v.Length)
                {
                    v[i * 4] = new VertexPositionColor(new Vector3(x, y, z), c1);
                    v[i * 4 + 1] = new VertexPositionColor(new Vector3(x + f, y + 0, z + 0), c2);
                    v[i * 4 + 2] = new VertexPositionColor(new Vector3(x + 0, y + f, z + 0), c3);
                    v[i * 4 + 3] = new VertexPositionColor(new Vector3(x + 0, y + 0, z + f), c4);
                }

                if ((i * 12 + 11) < PiramidaIndices.Length)
                {
                    PiramidaIndices[12 * i] = 0 + i * 4;
                    PiramidaIndices[12 * i + 1] = 1 + i * 4;
                    PiramidaIndices[12 * i + 2] = 2 + i * 4;
                    PiramidaIndices[12 * i + 3] = 0 + i * 4;
                    PiramidaIndices[12 * i + 4] = 2 + i * 4;
                    PiramidaIndices[12 * i + 5] = 3 + i * 4;
                    PiramidaIndices[12 * i + 6] = 0 + i * 4;
                    PiramidaIndices[12 * i + 7] = 1 + i * 4;
                    PiramidaIndices[12 * i + 8] = 3 + i * 4;
                    PiramidaIndices[12 * i + 9] = 1 + i * 4;
                    PiramidaIndices[12 * i + 10] = 2 + i * 4;
                    PiramidaIndices[12 * i + 11] = 3 + i * 4;
                }
            }


            effect = new BasicEffect(GraphicsDevice);
            effect.VertexColorEnabled = true;

            // Создаем буфер вершин
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), v.Length, BufferUsage.WriteOnly);            
            indexBuffer = new IndexBuffer(GraphicsDevice, typeof(int), PiramidaIndices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(v);
            indexBuffer.SetData<int>(PiramidaIndices);

            base.Initialize();

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

        }

        protected override void UnloadContent()
        { }

        protected override void Update(GameTime gameTime)
        {
            HandleInput();
            // UpdateCamera(gameTime);
            UpdateMatrix();
            base.Update(gameTime);
        }
        void UpdateMatrix()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                rotationMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(1));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                rotationMatrix *= Matrix.CreateRotationY(-1 * MathHelper.ToRadians(1));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                rotationMatrix *= Matrix.CreateRotationX(-1 * MathHelper.ToRadians(1));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                rotationMatrix *= Matrix.CreateRotationX(MathHelper.ToRadians(1));
            }

            // Check for input to zoom camera in and out.
            if (currentKeyboardState.IsKeyDown(Keys.Z))
                cameraDistance += 0.25f;

            if (currentKeyboardState.IsKeyDown(Keys.X))
                cameraDistance -= 0.25f;

            // Limit the camera distance.
            if (cameraDistance > 100)
                cameraDistance = 100;
            else if (cameraDistance < 1.5)
                cameraDistance = 1.5f;

            if (currentGamePadState.Buttons.RightStick == ButtonState.Pressed ||
                currentKeyboardState.IsKeyDown(Keys.R))
            {
                cameraArc = -5;
                cameraRotation = 0;
                cameraDistance = 100;
            }
            viewMatrix = rotationMatrix * Matrix.CreateLookAt(new Vector3(0, 0, -cameraDistance),
                                              new Vector3(0, 0, 0), Vector3.Up);            

        }
        void HandleInput()
        {
            lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;

            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // Check for exit.
            if (currentKeyboardState.IsKeyDown(Keys.Escape) ||
                currentGamePadState.Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

        }      

        private void UpdateWorld()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                translationMatrix *= Matrix.CreateTranslation(0, 0, .05f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                translationMatrix *= Matrix.CreateTranslation(0, 0, -.05f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                translationMatrix *= Matrix.CreateTranslation(0, .01f, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                translationMatrix *= Matrix.CreateTranslation(0, -.01f, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                translationMatrix *= Matrix.CreateTranslation(-.01f, 0, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                translationMatrix *= Matrix.CreateTranslation(.01f, 0, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                rotationMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(1));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                rotationMatrix *= Matrix.CreateRotationY(-1 * MathHelper.ToRadians(1));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                rotationMatrix *= Matrix.CreateRotationX(-1 * MathHelper.ToRadians(1));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                rotationMatrix *= Matrix.CreateRotationX(MathHelper.ToRadians(1));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                scaleMarix *= Matrix.CreateScale(1.01f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                scaleMarix *= Matrix.CreateScale(0.99f);
            }
            worldMatrix = scaleMarix * rotationMatrix * translationMatrix;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            effect.World = worldMatrix;
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;

            GraphicsDevice.BlendState = BlendState.Additive;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.Indices = indexBuffer;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                // GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, v.Length);
                // GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vert4, 0, vert4.Length / 3);           
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, v.Length, 0, PiramidaIndices.Length / 3);
            }

            base.Draw(gameTime);
        }

    }
}