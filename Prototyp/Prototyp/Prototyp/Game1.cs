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
        int score, timelimit, time;

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
            //first initalize of keyboardstate
            keyboard = Keyboard.GetState();
            //randomNumberGenerator
            Random rand = new Random();

            //groundplane
            vertices = new VertexPositionColor[4];
            vertices[0] = new VertexPositionColor(new Vector3(-50.0f, 0.0f, -50.0f), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(50.0f, 0.0f, -50.0f), Color.Gold);
            vertices[2] = new VertexPositionColor(new Vector3(-50.0f, 0.0f, 50.0f), Color.Yellow);
            vertices[3] = new VertexPositionColor(new Vector3(50.0f, 0.0f, 50.0f), Color.Green);

            //camera/player
            view = new Vector3[3];
            view[0] = new Vector3(0, 1, 0); //position
            view[1] = new Vector3(0, 1, 1); //lookAtPoint
            view[2] = new Vector3(0, 1, 0); //upDirection

            //projektion Matrix ??
            projektion = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.5f, 1000.0f);

            //effecthandler ???
            effect = new BasicEffect(GraphicsDevice);

            //initalize constants and variables
            score = 0;
            timescaler = 100;            
            jumpvalue = 0;
            time = 0;
            timelimit = 30000;
            
            //base initialize ...
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
            // font for drawing text
            font = Content.Load<SpriteFont>("SpriteFont1");
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
            //update time and keyboard
            timeSinceLastUpdate = gameTime.ElapsedGameTime.Milliseconds;
            keyboard = Keyboard.GetState();
            time += (int)timeSinceLastUpdate;

            //exit
            if (keyboard.IsKeyDown(Keys.Escape)) this.Exit();

            //Game over?
            if (time < timelimit)
            {
                // gravity
                jumpvalue -= timeSinceLastUpdate / timescaler / 5;

                //facedirektion
                direction = view[1] - view[0];

                //gameborders
                if (view[0].X < -50)
                {
                    view[0].X = -50;
                    view[1] = view[0] + direction;
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

                // groundkolision(gravity)
                if (jumpvalue < 0 && view[0].Y == 1) jumpvalue = 0;

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

                //rotate left
                if (keyboard.IsKeyDown(Keys.A) && !keyboard.IsKeyDown(Keys.D))
                {
                    view[1] = view[0] + (Vector3.Transform((view[1] - view[0]), Matrix.CreateRotationY(timeSinceLastUpdate / 4 / timescaler)));
                }

                //rotate rigth
                if (keyboard.IsKeyDown(Keys.D) && !keyboard.IsKeyDown(Keys.A))
                {
                    view[1] = view[0] + (Vector3.Transform((view[1] - view[0]), Matrix.CreateRotationY(-timeSinceLastUpdate / 4 / timescaler)));
                }

                //dropdown
                if (jumpvalue != 0)
                {
                    view[0].Y += jumpvalue;
                    view[1].Y += jumpvalue;
                }

                //groundcolision(position)
                if (view[0].Y < 1)
                {
                    view[0].Y = 1;
                    view[1].Y = 1;
                }

                // base update ...
                base.Update(gameTime);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //clear Desktop
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //modify effects
            effect.VertexColorEnabled = true;
            effect.View = Matrix.CreateLookAt(view[0], view[1], view[2]);
            effect.Projection = projektion;
            effect.CurrentTechnique.Passes[0].Apply();

            //draw groundplane
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, vertices, 0, 2);
            
            spriteBatch.Begin();

            //game over screen
            if (time >= timelimit)
            {
                spriteBatch.DrawString(font, "Game Over", new Vector2(GraphicsDevice.Viewport.Width / 2 - 60, GraphicsDevice.Viewport.Height / 2 - 50), Color.Black);
                spriteBatch.DrawString(font, "Your Score: " + score.ToString(), new Vector2(GraphicsDevice.Viewport.Width / 2 - 90, GraphicsDevice.Viewport.Height / 2 - 20), Color.Black);
            }
            //HUD
            else
            {
                spriteBatch.DrawString(font, "Time Left: "+((timelimit-time)/1000).ToString(), new Vector2(GraphicsDevice.Viewport.Width / 2 - 70, 10), Color.Black);
                spriteBatch.DrawString(font, "Score: "+score.ToString(), new Vector2(10, 10), Color.Black);
            }
            
            spriteBatch.End();

            // base draw ...
            base.Draw(gameTime);
        }
    }
}
