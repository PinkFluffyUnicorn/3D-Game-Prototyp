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

        VertexPositionColor[] groundplane;

        BasicEffect effect;

        Boolean colision, lastColision;
        Vector3 direction, lastPosition;
        Vector3[] view;
        Matrix projektion, camera;
        SpriteFont font;
        KeyboardState keyboard;

        float jumpvalue, timescaler, timeSinceLastUpdate;
        int score, timelimit, time;

        // Set the 3D model to draw.
        List<Cube> envoirment, coins;

        // The aspect ratio determines how to scale 3d to 2d projection.
        float aspectRatio;

        GraphicsDevice graphicsDevice;

       

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
            colision = false;
            lastColision = false;

            envoirment = new List<Cube>();
            coins = new List<Cube>();

            groundplane = new VertexPositionColor[4];
            groundplane[0] = new VertexPositionColor(new Vector3(-50.0f, 0.0f, -50.0f), Color.Red);
            groundplane[1] = new VertexPositionColor(new Vector3(50.0f, 0.0f, -50.0f), Color.Gold);
            groundplane[2] = new VertexPositionColor(new Vector3(-50.0f, 0.0f, 50.0f), Color.Yellow);
            groundplane[3] = new VertexPositionColor(new Vector3(50.0f, 0.0f, 50.0f), Color.Green);

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
            timelimit = 3000000;
            
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


            envoirment.Add(new Cube(Content.Load<Model>("cube"), 2.0f, new Vector3(25.0f, 0.0f, 3.0f),0,0,0));
            envoirment.Add(new Cube(Content.Load<Model>("cube"), 2.0f, new Vector3(45.0f, 0.0f, 3.0f), 0, 0, 0));
            envoirment.Add(new Cube(Content.Load<Model>("cube"), 2.0f, new Vector3(16.0f, 0.0f, 3.0f), 0, 0, 0));
            envoirment.Add(new Cube(Content.Load<Model>("cube"), 2.0f, new Vector3(18.0f, 0.0f, 3.0f), 0, 0, 0));
            envoirment.Add(new Cube(Content.Load<Model>("cube"), 2.0f, new Vector3(10.0f, 0.0f, 3.0f), 0, 0, 0));
            envoirment.Add(new Cube(Content.Load<Model>("cube"), 2.0f, new Vector3(10.0f, 0.0f, 10.0f), 0, 0, 0));
            envoirment.Add(new Cube(Content.Load<Model>("cube"), 2.0f, new Vector3(10.0f, 0.0f, 15.0f), 0, 0, 0));
            envoirment.Add(new Cube(Content.Load<Model>("cube"), 2.0f, new Vector3(10.0f, 0.0f, 20.0f), 0, 0, 0));
            envoirment.Add(new Cube(Content.Load<Model>("cube"), 2.0f, new Vector3(10.0f, 9.0f, 35.0f), 0, 0, 0));
            envoirment.Add(new Cube(Content.Load<Model>("cube"), 5.0f, new Vector3(10.0f, 0.0f, 35.0f), 0, 0, 0));

            coins.Add(new Cube(Content.Load<Model>("cube"), 0.3f, new Vector3(30.0f, 1.0f, 0.0f),0.0f, 0.0f, 0.0f));


            
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
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

                //save curent positon
                lastPosition = view[0];

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

                //jumping from blocks
                if (jumpvalue < 0 && lastColision) jumpvalue = 0;
                lastColision = false;

                //jump
                if (keyboard.IsKeyDown(Keys.Space) && jumpvalue == 0) jumpvalue += 1;

                //forward
                if (keyboard.IsKeyDown(Keys.W) && !keyboard.IsKeyDown(Keys.S))
                {
                    view[0] = view[0] + direction * (timeSinceLastUpdate / timescaler * 2);
                    view[1] = view[0] + direction;
                }

                //backward
                if (keyboard.IsKeyDown(Keys.S) && !keyboard.IsKeyDown(Keys.W))
                {
                    view[0] = view[0] - direction * (timeSinceLastUpdate / timescaler * 2);
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
                
                //colision with envoirment
                do
                {
                    colision = false;
                    foreach(Cube cube in envoirment)
                    {
                        float X = cube.getPosition().X - view[0].X;
                        if( X < 0) X = -X;
                        float Y = cube.getPosition().Y - view[0].Y;
                        if (Y < 0) Y = -Y;
                        float Z = cube.getPosition().Z - view[0].Z;
                        if (Z < 0) Z = -Z;
                        float skale = 2 * cube.getScaling();
                        if (X < skale && Y < skale && Z < skale)
                        {
                            view[1] += (lastPosition - view[0]);
                            view[0] += (lastPosition - view[0]);
                            colision = true;
                            lastColision = true;
                        }
                    } 
                }
                while (colision);

                //looting coins
                foreach (Cube coin in coins)
                {
                    float X = coin.getPosition().X - view[0].X;
                    if (X < 0) X = -X;
                    float Y = coin.getPosition().Y - view[0].Y;
                    if (Y < 0) Y = -Y;
                    float Z = coin.getPosition().Z - view[0].Z;
                    if (Z < 0) Z = -Z;
                    float skale = 2 * coin.getScaling();
                    if (X < skale && Y < skale && Z < skale)
                    {
                        coins.Remove(coin);
                        score += 100;
                    }
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
            camera = Matrix.CreateLookAt(view[0], view[1], view[2]);
            effect.VertexColorEnabled = true;
            effect.View = camera;
            effect.Projection = projektion;
            effect.CurrentTechnique.Passes[0].Apply();

            



            //draw groundplane
            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, groundplane, 0, 2);
            
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


            foreach (Cube cube in envoirment) cube.Draw(gameTime, projektion, camera);
            foreach (Cube cube in coins) cube.Draw(gameTime, projektion, camera);
            
                // base draw ...
                base.Draw(gameTime);



        }

    }
}
