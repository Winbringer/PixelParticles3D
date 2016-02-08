using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PicselParticles
{
    class Piramida : DrawableGameComponent
    {
        #region fields
      public  GraphicsDeviceManager graphics;
        BasicEffect effect;
        public Matrix WorldMatrix;
        public Matrix ViewMatrix;
        public Matrix ProjectMatrix;
        public Color Color;
        VertexPositionColor[] vertex;
        public float Size { get; set; }
        #endregion

        public Piramida(Game game, GraphicsDeviceManager gm):base(game) 
        {

            graphics = gm;            
            Color = Color.Orange;
            Size = 0.1f;
            vertex = new VertexPositionColor[12];
            ViewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 50), Vector3.Zero, Vector3.Up);
            ProjectMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                (float)game.Window.ClientBounds.Width /
                (float)game.Window.ClientBounds.Height,
                1.5f, 100);
            Random rndd = new Random(DateTime.Now.Millisecond);
            Random rnd = new Random(DateTime.Now.Millisecond+rndd.Next());
            float x = (float)(rnd.NextDouble() - rnd.NextDouble()) * 10;
            float y = (float)(rnd.NextDouble() - rnd.NextDouble()) * 10;
            float z = (float)(rnd.NextDouble() - rnd.NextDouble()) * 10;
            WorldMatrix =Matrix.CreateTranslation(x,y,z)* Matrix.CreateWorld(new Vector3(0f, 0f, 0f), new Vector3(0, 0, -1), Vector3.Up);

        }

        public override void Initialize()
        {
            float X = 0;
            float Y = 0;
            float Z = 0;

            vertex[0] = new VertexPositionColor(new Vector3(X, Y, Z), Color);
            vertex[1] = new VertexPositionColor(new Vector3(X + Size, Y + Size / 6, Z + Size / 6), Color);
            vertex[2] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size, Z + Size / 6), Color);

            vertex[3] = new VertexPositionColor(new Vector3(X + 0, Y + 0, Z + 0), Color);
            vertex[4] = new VertexPositionColor(new Vector3(X + Size, Y + Size / 6, Z + Size / 6), Color);
            vertex[5] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size / 6, Z + Size), Color);

            vertex[6] = new VertexPositionColor(new Vector3(X + 0, Y + 0, Z + 0), Color);
            vertex[7] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size / 6, Z + Size), Color);
            vertex[8] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size, Z + Size / 6), Color);

            vertex[9] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size / 6, Z + Size), Color);
            vertex[10] = new VertexPositionColor(new Vector3(X + Size, Y + Size / 6, Z + Size / 6), Color);
            vertex[11] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size, Z + Size / 6), Color);

           
            base.Initialize();
           
        }

        protected override void LoadContent()
        {
            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.VertexColorEnabled = true;

            base.LoadContent();
            
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            WorldMatrix *= Matrix.CreateTranslation(0.01f, 0, 0); ;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

           
            effect.World = WorldMatrix;
            effect.View = ViewMatrix;
            effect.Projection = ProjectMatrix;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertex, 0, 4);
            }

            base.Draw(gameTime);
        }

       
    }
}
