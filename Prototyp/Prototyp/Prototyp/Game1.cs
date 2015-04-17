using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Prototyp
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        VertexPositionColor[] vertices;

        BasicEffect effect;

        Vector3 direction;
        Vector3[] view;
        Matrix projektion;
        SpriteFont font;
        KeyboardState keyboard;

        float jumpvalue, timescaler, timeSinceLastUpdate;
        int score;

        // Set the 3D model to draw.
        Model Cube1;

        // The aspect ratio determines how to scale 3d to 2d projection.
        float aspectRatio;
       

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            keyboard = Keyboard.GetState();
            Random rand = new Random();
            view = new Vector3[3];
            vertices = new VertexPositionColor[4];
            vertices[0] = new VertexPositionColor(new Vector3(-50.0f, 0.0f, -50.0f), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(50.0f, 0.0f, -50.0f), Color.Gold);
            vertices[2] = new VertexPositionColor(new Vector3(-50.0f, 0.0f, 50.0f), Color.Yellow);
            vertices[3] = new VertexPositionColor(new Vector3(50.0f, 0.0f, 50.0f), Color.Green);

            view[0] = new Vector3(0, 1, 0);
            view[1] = new Vector3(0, 1, 1);
            view[2] = new Vector3(0, 1, 0);

            projektion = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.5f, 1000.0f);

            effect = new BasicEffect(GraphicsDevice);

            score = 0;
            timescaler = 100;            
            jumpvalue = 0;

            //font = Content.Load<SpriteFont>("SpriteFont1");

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here


            Cube1 = Content.Load<Model>("cube");
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            timeSinceLastUpdate = gameTime.ElapsedGameTime.Milliseconds;
            keyboard = Keyboard.GetState();
            jumpvalue -= timeSinceLastUpdate / timescaler / 5;
            direction = view[1] - view[0];
            
            if (view[0].X < -50)
            {
                view[0].X = -50;
                view[1] = view[0] +direction;
            }

            if (view[0].X > 50)
            {
                view[0].X = 50;
                view[1] = view[0] + direction;
            }

            if (view[0].Z < -50)
            {
                view[0].Z = -50;
                view[1] = view[0] + direction;
            }

            if (view[0].Z > 50)
            {
                view[0].Z = 50;
                view[1] = view[0] + direction;
            }

            if(jumpvalue < 0 && view[0].Y == 1) jumpvalue = 0;

            //exit
            if(keyboard.IsKeyDown(Keys.Escape)) this.Exit();
            //jump
            if (keyboard.IsKeyDown(Keys.Space) && view[0].Y == 1) jumpvalue += 1;
            //forward
            if (keyboard.IsKeyDown(Keys.W) && !keyboard.IsKeyDown(Keys.S))
            {   


                view[0] = view[0] + direction * (timeSinceLastUpdate / timescaler * 4);
                view[1] = view[0] + direction;

            }
            //backward
            if (keyboard.IsKeyDown(Keys.S) && !keyboard.IsKeyDown(Keys.W))
            {

                view[0] = view[0] - direction * (timeSinceLastUpdate / timescaler * 4);
                view[1] = view[0] + direction;
            }

            if (keyboard.IsKeyDown(Keys.A) && !keyboard.IsKeyDown(Keys.D))
            {
                view[1] = view[0] + (Vector3.Transform((view[1] - view[0]), Matrix.CreateRotationY(timeSinceLastUpdate / 4 / timescaler)));
            }

            if (keyboard.IsKeyDown(Keys.D) && !keyboard.IsKeyDown(Keys.A))
            {
                view[1] = view[0] + (Vector3.Transform((view[1] - view[0]), Matrix.CreateRotationY(-timeSinceLastUpdate / 4 / timescaler)));
            }

            //drop
            if (jumpvalue != 0)
            {
                view[0].Y += jumpvalue;
                view[1].Y += jumpvalue;
            }
            //kollision eith ground
            if (view[0].Y < 1)
            {
                view[0].Y = 1;
                view[1].Y = 1;
            }
             
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            effect.VertexColorEnabled = true;
            effect.View = Matrix.CreateLookAt(view[0], view[1], view[2]);
            effect.Projection = projektion;

            effect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, vertices, 0, 2);


            spriteBatch.Begin();

            //spriteBatch.DrawString(font, score.ToString(), new Vector2(10, 10), Color.Black);

            spriteBatch.End();

            Cube1.Draw

            base.Draw(gameTime);
        }
    }
}
