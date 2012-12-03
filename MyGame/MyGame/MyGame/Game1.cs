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

namespace MyGame
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //create a list for models
        List<CModel> models = new List<CModel>();

        //create an instance of the camera class
        CCamera camera;
        //craete an instance of the skysphere class
        SkySphere sky;
        //create an instance of the song class
        Song backgroundMusic;

        //create an instance of the spritefont class
        SpriteFont font;

        KeyboardState oldstate;
        //create an instance of the terrain class
        Terrain terrain;

        //enemies
        private CEnemy[] enemyList = new CEnemy[GameConstants.NumEnemies];
        private Matrix[] mDalekTransforms;
        private Random random = new Random();

        //soundeffet
        SoundEffectInstance soundEngineInstance;
        SoundEffect engine;

        MouseState lastMouseState;


        private void ResetEnemies()
        {

            float xStart;
            float zStart;
            for (int i = 0; i < GameConstants.NumEnemies; i++)
            {
                if (random.Next(2) == 0)
                {
                    xStart = (float)-GameConstants.PlayfieldSizeX;
                }
                else
                {
                    xStart = (float)GameConstants.PlayfieldSizeX;
                }
                zStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeZ;
                enemyList[i].position = new Vector3(xStart, 0.0f, zStart);
                double angle = random.NextDouble() * 2 * Math.PI;
                enemyList[i].direction.X = -(float)Math.Sin(angle);
                enemyList[i].direction.Z = (float)Math.Cos(angle);
                enemyList[i].speed = GameConstants.EnemieskMinSpeed +
                   (float)random.NextDouble() * GameConstants.EnemiesMaxSpeed;
                enemyList[i].isActive = true;
            }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 800;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            Window.Title = "Lab 6 - Collision Detection";
            
            ResetEnemies();

            base.Initialize();
        }

        // Called when the game should load its content
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load models
            models.Add(new CModel(Content.Load<Model>("ship"), new Vector3(0, 40000, 0), Vector3.Zero,
                       new Vector3(0.60f), GraphicsDevice));

            models.Add(new CModel(Content.Load<Model>("alien1"), new Vector3(50000, 35000, 50000), Vector3.Zero,
                              new Vector3(25.0f), GraphicsDevice));


            models.Add(new CModel(Content.Load<Model>("T-88-FBX"), new Vector3(-27000, 37000, 27000), Vector3.Zero,
                              new Vector3(2.5f), GraphicsDevice));

            models.Add(new CModel(Content.Load<Model>("dalek"), new Vector3(-20000, 37000, 20000), Vector3.Zero,
                              new Vector3(2.5f), GraphicsDevice));

            models.Add(new CModel(Content.Load<Model>("dalek"), new Vector3(-20000, 37000, 24000), Vector3.Zero,
                              new Vector3(2.5f), GraphicsDevice));

            models.Add(new CModel(Content.Load<Model>("alien1"), new Vector3(20900, 40000, 700), new Vector3(0, 75, 0),
                           new Vector3(25.0f), GraphicsDevice));


            models.Add(new CModel(Content.Load<Model>("T-88-FBX"), new Vector3(18900, 40000, 7000), new Vector3(0, 75, 0),
                           new Vector3(1.5f), GraphicsDevice));

            models.Add(new CModel(Content.Load<Model>("T-88-FBX"), new Vector3(25900, 38000, 27800), new Vector3(0, 75, 0),
                           new Vector3(1.5f), GraphicsDevice));


            models.Add(new CModel(Content.Load<Model>("alien1"), new Vector3(-36000, 28000, 30010), new Vector3(0, 90, 0),
                           new Vector3(25.0f), GraphicsDevice));

            models.Add(new CModel(Content.Load<Model>("alien1"), new Vector3(16500, 25000, 33610), new Vector3(0, 120, 0),
                           new Vector3(25.0f), GraphicsDevice));


            camera = new ChaseCamera(new Vector3(0, 400, 1500), new Vector3(0, 200, 0),
                new Vector3(0, 0, 0), GraphicsDevice);


            //load texture for the skybox
            sky = new SkySphere(Content, GraphicsDevice, Content.Load<TextureCube>("clouds"));
            //load texture for the terrain
            terrain = new Terrain(Content.Load<Texture2D>("terrain"), 300, 48000, Content.Load<Texture2D>("grass"), 6, new Vector3(1, -1, 0), GraphicsDevice, Content);
            //load sound effects and music
            engine = Content.Load<SoundEffect>(".\\Audio\\engine");
            soundEngineInstance = engine.CreateInstance();
            soundEngineInstance.Volume = 0.3f;
            backgroundMusic = Content.Load<Song>(".\\Audio\\War");
            font = Content.Load<SpriteFont>("Font");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.Volume = 1.0f;
            MediaPlayer.IsRepeating = true;

            lastMouseState = Mouse.GetState();


        }

        // Called when the game should update itself
        protected override void Update(GameTime gameTime)
        {
            updateCamera(gameTime);
            updateModel(gameTime);

            float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < GameConstants.NumEnemies; i++)
            {
                enemyList[i].Update(timeDelta);
            }

            base.Update(gameTime);
        }

        void updateCamera(GameTime gameTime)
        {
            // Move the camera to the new model's position and orientation
            ((ChaseCamera)camera).Move(models[0].Position, models[0].Rotation);

            // Update the camera
            camera.Update();
        }

        void updateModel(GameTime gameTime)
        {
            // Get the new Keyboard and mouse State
           // MouseState mouseState = Mouse.GetState();
            KeyboardState keyState = Keyboard.GetState();

            Vector3 rotChange = new Vector3(0, 0, 0);

            ////Determine how much the camera should turn
            //float deltaX = (float)lastMouseState.X - (float)mouseState.X;
            //float deltaY = (float)lastMouseState.Y - (float)mouseState.Y;
            if (keyState.IsKeyDown(Keys.W))
                rotChange += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.S))
                rotChange += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.A))
                rotChange += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.D))
                rotChange += new Vector3(0, -1, 0);


            models[0].Rotation += rotChange * .025f;
            
           

           // if (keyState.IsKeyDown(Keys.Space))
                 soundEngineInstance.Play();

                if (keyState.IsKeyDown(Keys.Z))
                {
                    soundEngineInstance.Stop();
                    MediaPlayer.Pause();
                }

                if (keyState.IsKeyDown(Keys.X))
                {
                    MediaPlayer.Resume();
                }

                // Check for exit.
                if (keyState.IsKeyDown(Keys.Escape))
                {
                    Exit();
                }
                

              //  oldstate = keyState;

            // Determine what direction to move in
            Matrix rotation = Matrix.CreateFromYawPitchRoll(
                models[0].Rotation.Y, models[0].Rotation.X, models[0].Rotation.Z);

            // Move in the direction dictated by our rotation matrix
            models[0].Position += Vector3.Transform(Vector3.Forward, rotation)
                * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 4;
        }


     
        

        // Called when the game should draw itself
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //spriteBatch.Begin();

            //spriteBatch.DrawString(font, " Z = Sound Off , X = Sound On ", new Vector2(50, 50), Color.Black);
            sky.Draw(camera.View, camera.Projection, ((ChaseCamera)camera).Position);
          
            terrain.Draw(camera.View, camera.Projection);
            foreach (CModel Model in models)
                if (camera.BoundingVolumeIsInView(Model.BoundingSphere))
                    Model.Draw(camera.View, camera.Projection, ((ChaseCamera)camera).Position);
           //spriteBatch.End();
            
                base.Draw(gameTime);
            }
        }
    }


