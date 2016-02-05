/* 
  vert4 = new VertexPositionColor[1000];
 VertexPositionColor[] vert4;
 for (int i = 0; i < 1000; i++)
            {
                //Диапазон координат от -3 до 3
                float x = (float)(rnd.NextDouble() - rnd.NextDouble()) * 3;
                float y = (float)(rnd.NextDouble() - rnd.NextDouble()) * 3;
                float z = (float)(rnd.NextDouble() - rnd.NextDouble()) * 3;

                vert4[i] = new VertexPositionColor(new Vector3(x, y, z), Color.White);
            }
*/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace PicselParticles
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;
        Matrix scaleMarix = Matrix.Identity;

        VertexPositionColor[] triangleVertices;
        VertexPositionColor[] vert4;
        VertexPositionColor[] vert1;

        VertexBuffer vertexBuffer;

        BasicEffect effect;
        Matrix translationMatrix = Matrix.Identity;
        Matrix rotationMatrix = Matrix.Identity;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 6), Vector3.Zero, Vector3.Up);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                (float)Window.ClientBounds.Width /
                (float)Window.ClientBounds.Height,
                1, 100);

            worldMatrix = Matrix.CreateWorld(new Vector3(0f, 0f, 0f), new Vector3(0, 0, -1), Vector3.Up);

            // создаем набор вершин
            triangleVertices = new VertexPositionColor[3];
            triangleVertices[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Red);
            triangleVertices[1] = new VertexPositionColor(new Vector3(1, -1, 0), Color.Green);
            triangleVertices[2] = new VertexPositionColor(new Vector3(-1, -1, 0), Color.Blue);

            // Создаем буфер вершин
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor),
                triangleVertices.Length, BufferUsage.None);

            effect = new BasicEffect(GraphicsDevice);
            effect.VertexColorEnabled = true;

            vertexBuffer.SetData(triangleVertices);

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;

            vert4 = new VertexPositionColor[1000];
            Random rnd = new Random();           
            for (int i = 0; i < 1000; i++)
            {
                //Диапазон координат от -3 до 3
                float x = (float)(rnd.NextDouble() - rnd.NextDouble()) * 3;
                float y = (float)(rnd.NextDouble() - rnd.NextDouble()) * 3;
                float z = (float)(rnd.NextDouble() - rnd.NextDouble()) * 3;

                vert4[i] = new VertexPositionColor(new Vector3(x, y, z), Color.White);
            }

            vert1 = new VertexPositionColor[5];
            vert1[0] = new VertexPositionColor(new Vector3(-1.0f, -1.0f, 0.0f), Color.White);
            vert1[1] = new VertexPositionColor(new Vector3(-1.0f, 1.0f, 0.0f), Color.White);
            vert1[2] = new VertexPositionColor(new Vector3(1.0f, 1.0f, 0.0f), Color.White);
            vert1[3] = new VertexPositionColor(new Vector3(1.0f, -1.0f, 0.0f), Color.White);
            vert1[4] = new VertexPositionColor(new Vector3(-1.0f, -1.0f, 0.0f), Color.White);
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                    Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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
                rotationMatrix *= Matrix.CreateRotationY(MathHelper.ToRadians(3));
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                rotationMatrix *= Matrix.CreateRotationY(-1 * MathHelper.ToRadians(3));
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            effect.World = worldMatrix;

            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;

            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>
                    (PrimitiveType.TriangleStrip, triangleVertices, 0, 1);
                //graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vert1, 0, 4);
                graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vert4, 0, 500);
            }

            base.Draw(gameTime);
        }
    }
}