using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dogfight
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        /// <summary>
        /// The global world matrix
        /// </summary>
        Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));

        /// <summary>
        /// ISAIAH HALP
        /// View matrix? Isn't this in the camera?
        /// </summary>
        Matrix view;

        /// <summary>
        /// ISAIAH HALP
        /// Projection matrix? Isn't this in the camera?
        /// </summary>
        Matrix proj;

        /// <summary>
        /// The viewport for projection to the screen
        /// </summary>
        Viewport view2D;

        /// <summary>
        /// The camera for the game
        /// </summary>
        Camera camera = new Camera();

        /// <summary>
        /// The player's data
        /// </summary>
        Player player = new Player();

        /// <summary>
        /// A list of all enemies currently in the game
        /// </summary>
        List<Enemy> enemyList = new List<Enemy>();

        /// <summary>
        /// The keyboard state of the previous iteration
        /// </summary>
        KeyboardState lastkeyboardState = new KeyboardState();

        /// <summary>
        /// The keyboard state of the current iteration
        /// </summary>
        KeyboardState currentKeyboardState = new KeyboardState();

        /// <summary>
        /// The model of the player's ship
        /// </summary>
        Model shipModel;

        /// <summary>
        /// The model of the skybox
        /// </summary>
        Model skybox;

        /// <summary>
        /// The portion of the crosshair without a center
        /// </summary>
        Texture2D crosshair;

        /// <summary>
        /// The portion of the crosshair with a center
        /// </summary>
        Texture2D crosshairCenter;

        /// <summary>
        /// True if the chase camera should be attatched by a spring. False otherwise
        /// </summary>
        bool enableCamSpring;

        /// <summary>
        /// The player's current health
        /// </summary>
        public int health;

        /// <summary>
        /// The enemies's speed
        /// </summary>
        public float speed;

        /// <summary>
        /// The current wave
        /// </summary>
        public int wave;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //Initialize the camera's data
            camera.DesiredPositionOffset = new Vector3(0.0f, 2000.0f, 3500.0f);
            camera.LookAtOffset = new Vector3(0.0f, 150.0f, 0.0f);
            camera.NearPlaneDist = 10.0f;
            camera.FarPlaneDist = 100000.0f;
            camera.AspectRatio = (float)_graphics.GraphicsDevice.Viewport.Width / (float)_graphics.GraphicsDevice.Viewport.Height;
            
            enableCamSpring = true;
            wave = 0;

            view2D = new Viewport(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            UpdateChaseTarget();
            camera.Reset();
            newWave(wave);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            this.shipModel = Content.Load<Model>("playership3");
            this.skybox = Content.Load<Model>("skybox");
            this.crosshair = Content.Load<Texture2D>("CrosshairSmaller");
            this.crosshairCenter = Content.Load<Texture2D>("CrosshairCenterSmaller");
        }

        /// <summary>
        /// Make the chase target of the camera the player's current position
        /// </summary>
        private void UpdateChaseTarget()
        {
            camera.ChasePos = player.pos;
            camera.ChaseDir = player.dir;
            camera.Up = player.up;
        }

        /// <summary>
        /// Start a new wave
        /// </summary>
        /// <param name="waveNumber">The number of the current wave</param>
        private void newWave(int waveNumber) {
            for (int i = 0; i < waveNumber; i++)
            {
                enemyList.Add(new Enemy(new Vector3(0, 0, 0), new Vector3(0, 0, 0), (waveNumber / 5) + 1)); //Todo: Figure out placement of enemies
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            //Update the keyboard states and make lastkeyboardState the previous keyboard state
            lastkeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            player.Update(gameTime, _graphics, enemyList, view2D, proj, view, world);

            UpdateChaseTarget();
            camera.Update(gameTime);

            //Check if we need to start a new wave. If not, update the enemies
            if (enemyList.Count == 0) {
                wave++;
                newWave(wave);
            } else {
                foreach (Enemy enemy in enemyList)
                {
                    enemy.Update(gameTime, player);
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Debug.WriteLine("" + camera.Pos + ", " + player.pos);
            DrawModel(skybox, world * Matrix.CreateScale(10000f) * Matrix.CreateTranslation(player.pos), true);
            DrawModel(shipModel, player.World, false);

            foreach (Enemy enemy in enemyList)
            {
                //Todo: draw model for enemies
            }


            _spriteBatch.Begin();
            _spriteBatch.Draw(crosshair, player.mousePosInCircle - new Vector2(crosshair.Width / 2, crosshair.Height / 2), Color.White);
            _spriteBatch.Draw(crosshairCenter, player.mousePos - new Vector2(crosshairCenter.Width / 2, crosshairCenter.Height / 2), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, bool skybox)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    if (skybox) {
                        effect.LightingEnabled = true;
                        effect.AmbientLightColor = new Vector3(.1f, .1f, .1f);
                    }
                    effect.World = world;
                    effect.View = camera.View;
                    effect.Projection = camera.projectionView;
                }
                mesh.Draw();
            }
        }
    }
}