
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace PicselParticles
{
    public class Game1 : Game
    {
        #region Переменные
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

        private float cameraDistance;

        #endregion
        List<Piramida> piramida;

        public Game1()
        {

            Content.RootDirectory = "Content";
            cameraDistance = 50;
            graphics = new GraphicsDeviceManager(this);           
            graphics.PreferredBackBufferHeight = 700;
            graphics.PreferredBackBufferWidth = 800;
            Window.Title = "Kezumie";
            IsMouseVisible = true;
            piramida = new List<Piramida>();
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                 graphics.PreferredBackBufferWidth /
                (float)graphics.PreferredBackBufferHeight,
                1.5f, 100);
            worldMatrix = Matrix.CreateWorld(new Vector3(0f, 0f, 0f), new Vector3(0, 0, -1), Vector3.Up);

            Piramida p = new Piramida(this);
            p.graphics = graphics;
            piramida.Add(p);
            Components.Clear();
            Components.Add(p);
          
        }

        protected override void Initialize()
        {
            BallInitalize(10,12000000,0.02f);
            BufferInitalize();
            base.Initialize();
        }            

        protected override void LoadContent()
        {
            effect = new BasicEffect(graphics.GraphicsDevice);           
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);           
        }

        protected override void UnloadContent()
        { }

        protected override void Update(GameTime gameTime)
        {
            HandleInput();
            UpdateMatrix();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            graphics.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            graphics.GraphicsDevice.Indices = indexBuffer;

            effect.VertexColorEnabled = true;
            effect.World = worldMatrix;
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();                                         
                graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, v.Length, 0, PiramidaIndices.Length / 3);
            }
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            base.Draw(gameTime);
        }


        private void BufferInitalize()
        {
           
            graphics.GraphicsDevice.Flush();
            vertexBuffer = new VertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionColor), v.Length, BufferUsage.WriteOnly);
            indexBuffer = new IndexBuffer(graphics.GraphicsDevice, typeof(int), PiramidaIndices.Length, BufferUsage.WriteOnly);

            vertexBuffer.SetData<VertexPositionColor>(v);
            indexBuffer.SetData<int>(PiramidaIndices);           
        }

        private void KubeParticles()
        {
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
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                rotationMatrix *= Matrix.CreateRotationZ(-1 * MathHelper.ToRadians(1));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                rotationMatrix *= Matrix.CreateRotationZ(MathHelper.ToRadians(1));
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
                rotationMatrix = Matrix.Identity;
                cameraDistance = 50;
            }

            viewMatrix = rotationMatrix * Matrix.CreateLookAt(new Vector3(0, 0, cameraDistance),
                                              new Vector3(0, 0, 0), Vector3.Up);
            foreach (var p in piramida)
            {
                p.ViewMatrix = viewMatrix;
            }

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

        void BallInitalize(double radius,int count, float size)
        {
            v = new VertexPositionColor[count];
            PiramidaIndices = new int[v.Length * 3];
            float f = size;

            for (int i = 0; i < v.Length / 4; ++i)
            {
                Color c1 = Color.Orange;
                Color c2 = Color.Orange;
                Color c3 = Color.Orange;
                Color c4 = Color.Orange;
                Random rnd = new Random(i);
                double R = rnd.NextDouble() * radius; //rnd.NextDouble()* 10;
                float sin = (float)(rnd.NextDouble() * 180);
                float cos = (float)(rnd.NextDouble() * 360);
                float x = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Cos(MathHelper.ToRadians(cos)));
                float y = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Sin(MathHelper.ToRadians(cos)));
                float z = (float)(R * Math.Cos(MathHelper.ToRadians(sin)));


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

        }

        //private void UpdateWorld()
        //{
        //    if (Keyboard.GetState().IsKeyDown(Keys.Z))
        //    {
        //        translationMatrix *= Matrix.CreateTranslation(0, 0, .05f);
        //    }
        //    if (Keyboard.GetState().IsKeyDown(Keys.X))
        //    {
        //        translationMatrix *= Matrix.CreateTranslation(0, 0, -.05f);
        //    }
        //    if (Keyboard.GetState().IsKeyDown(Keys.Up))
        //    {
        //        translationMatrix *= Matrix.CreateTranslation(0, .01f, 0);
        //    }
        //    if (Keyboard.GetState().IsKeyDown(Keys.Down))
        //    {
        //        translationMatrix *= Matrix.CreateTranslation(0, -.01f, 0);
        //    }
        //    if (Keyboard.GetState().IsKeyDown(Keys.Left))
        //    {
        //        translationMatrix *= Matrix.CreateTranslation(-.01f, 0, 0);
        //    }
        //    if (Keyboard.GetState().IsKeyDown(Keys.Right))
        //    {
        //        translationMatrix *= Matrix.CreateTranslation(.01f, 0, 0);
        //    }
        //    if (Keyboard.GetState().IsKeyDown(Keys.A))
        //    {
        //        rotationMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(1));
        //    }
        //    if (Keyboard.GetState().IsKeyDown(Keys.D))
        //    {
        //        rotationMatrix *= Matrix.CreateRotationY(-1 * MathHelper.ToRadians(1));
        //    }
        //    if (Keyboard.GetState().IsKeyDown(Keys.W))
        //    {
        //        rotationMatrix *= Matrix.CreateRotationX(-1 * MathHelper.ToRadians(1));
        //    }
        //    if (Keyboard.GetState().IsKeyDown(Keys.S))
        //    {
        //        rotationMatrix *= Matrix.CreateRotationX(MathHelper.ToRadians(1));
        //    }
        //    if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
        //    {
        //        scaleMarix *= Matrix.CreateScale(1.01f);
        //    }
        //    if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
        //    {
        //        scaleMarix *= Matrix.CreateScale(0.99f);
        //    }
        //    worldMatrix = scaleMarix * rotationMatrix * translationMatrix;
        //}

    }
}