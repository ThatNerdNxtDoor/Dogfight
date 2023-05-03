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

        List<Projectile> projectileList = new List<Projectile>();

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
        /// The model of the enemy's ship
        /// </summary>
        Model enemyModel;

        /// <summary>
        /// The model of the projectile
        /// </summary>
        Model projectileModel;

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

        SpriteFont gameFont;

        /// <summary>
        /// True if the chase camera should be attatched by a spring. False otherwise
        /// </summary>
        bool enableCamSpring;

        /// <summary>
        /// The player's current health
        /// </summary>
        int health;

        /// <summary>
        /// The enemies's speed
        /// </summary>
        public float speed;

        /// <summary>
        /// The current wave
        /// </summary>
        public int wave;

        bool lose;

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
            wave = 1;
            health = 3;

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
            this.skybox = Content.Load<Model>("skybox");
            this.shipModel = Content.Load<Model>("playership3");
            this.enemyModel = Content.Load<Model>("enemyship2");
            this.projectileModel = Content.Load<Model>("projectile");
            this.crosshair = Content.Load<Texture2D>("CrosshairSmaller");
            this.crosshairCenter = Content.Load<Texture2D>("CrosshairCenterSmaller");
            this.gameFont = Content.Load<SpriteFont>("galleryFont");
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
                Random r = new Random();
                int random = r.Next(0, 6);
                float offset = (float)r.NextDouble() * 500.0f;
                Vector3 spawnPos = new Vector3(0, 0, 0);
                Vector3 spawnDir = new Vector3(0, 0, 0);
                const float enemyStartingDistance = 90000f;
                switch (random) {
                    case 0: // Front
                        spawnPos = new Vector3(enemyStartingDistance, offset - 250, offset - 250);
                        spawnDir = new Vector3(-1, 0, 0);
                        break;
                    case 1: // Back
                        spawnPos = new Vector3(-enemyStartingDistance, offset - 250, offset - 250);
                        spawnDir = new Vector3(1, 0, 0);
                        break;
                    case 2: // Top
                        spawnPos = new Vector3(offset - 250, enemyStartingDistance, offset - 250);
                        spawnDir = new Vector3(0, -1, 0);
                        break;
                    case 3: // Bottom
                        spawnPos = new Vector3(offset - 250, -enemyStartingDistance, offset - 250);
                        spawnDir = new Vector3(0, 1, 0);
                        break;
                    case 4: // Right
                        spawnPos = new Vector3(offset - 250, offset - 250, enemyStartingDistance);
                        spawnDir = new Vector3(0, 0, -1);
                        break;
                    case 5: // Left
                        spawnPos = new Vector3(offset - 250, offset - 250, -enemyStartingDistance);
                        spawnDir = new Vector3(0, 0, 1);
                        break;
                }
                spawnDir = player.pos - spawnPos;
                spawnDir.Normalize();
                enemyList.Add(new Enemy(spawnPos, spawnDir, (waveNumber / 5) + 1));
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

            if (!lose)
            {
                player.Update(gameTime, _graphics, enemyList, view2D, camera.projectionView, camera.View, world);

                UpdateChaseTarget();
                camera.Update(gameTime);

                //Check if we need to start a new wave. If not, update the enemies
                if (enemyList.Count == 0)
                {
                    wave++;
                    newWave(wave);
                }
                else
                {
                    foreach (Enemy enemy in enemyList)
                    {
                        enemy.Update(gameTime, player, projectileList);
                    }
                }

                for (int p = 0; p < projectileList.Count; p++)
                {
                    Projectile pro = projectileList[p];
                    pro.Update(gameTime);
                    //Check if projectile has hit boundary
                    if (pro.pos.X >= 1000000.0f || pro.pos.X <= -1000000.0f)
                    {
                        projectileList.RemoveAt(p);
                        p--;
                        continue;
                    }
                    else if (pro.pos.Y >= 1000000.0f || pro.pos.Y <= -1000000.0f)
                    {
                        projectileList.RemoveAt(p);
                        p--;
                        continue;
                    }
                    else if (pro.pos.Z >= 1000000.0f || pro.pos.Z <= -1000000.0f)
                    {
                        projectileList.RemoveAt(p);
                        p--;
                        continue;
                    }
                    //Check if projectile hits player
                    if (Vector3.Distance(pro.pos, player.pos) <= 500f)
                    {
                        health--;
                        if (health <= 0)
                        {
                            lose = true;
                        }
                        projectileList.RemoveAt(p);
                        p--;
                        continue;
                    }
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            //Debug.WriteLine("P: " + camera.Pos + ", " + player.pos);
            DrawModel(skybox, world * Matrix.CreateScale(60000f) * Matrix.CreateTranslation(player.pos), true, 1f);
            if (!lose)
            {
                DrawModel(shipModel, player.World, false, 0f);
            }
            foreach (Enemy enemy in enemyList)
            {
                DrawModel(enemyModel, enemy.World, true, 255f);
            }
            foreach(Projectile p in projectileList)
            {
                DrawModel(projectileModel, p.World, true, 255f);
            }

            _spriteBatch.Begin();
            _spriteBatch.Draw(crosshair, player.mousePosInCircle - new Vector2(crosshair.Width / 2, crosshair.Height / 2), Color.White);
            _spriteBatch.Draw(crosshairCenter, player.mousePos - new Vector2(crosshairCenter.Width / 2, crosshairCenter.Height / 2), Color.White);
            if (lose)
            {
                _spriteBatch.DrawString(gameFont, "-GAME OVER-", new Vector2(300, 100), Color.DimGray);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world, bool light, float lightValue)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    if (light) {
                        effect.LightingEnabled = true;
                        effect.AmbientLightColor = new Vector3(lightValue, lightValue, lightValue);
                    }
                    effect.World = world;
                    if (model == enemyModel)
                    {
                        Debug.WriteLine(effect.World.ToString());
                    }
                    effect.View = camera.View;
                    effect.Projection = camera.projectionView;
                    effect.Alpha = 1;
                }
                mesh.Draw();
            }
        }
    }
}