using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Dogfight
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Matrix world;
        Matrix view;
        Matrix proj;

        Viewport view2D;

        Camera camera = new Camera();
        Player player = new Player();
        List<Enemy> enemyList;

        KeyboardState lastkeyboardState = new KeyboardState();
        KeyboardState currentKeyboardState = new KeyboardState();

        Model shipModel;
        Model groundModel;

        bool enableCamSpring;
        public int health;
        public float speed;
        public int wave;
        
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
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
        }

        private void UpdateChaseTarget()
        {
            camera.ChasePos = player.pos;
            camera.ChaseDir = player.dir;
            camera.Up = player.up;
        }

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
            lastkeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            player.Update(gameTime, enemyList, view2D, proj, view, world);

            UpdateChaseTarget();
            camera.Update(gameTime);

            if (enemyList.Count == 0) {
                wave++;
                newWave(wave);
            } else {
                foreach (Enemy enemy in enemyList)
                {
                    enemy.Update(gameTime);
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void DrawModel(Model model, Matrix world)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = world;
                    effect.View = camera.View;
                    effect.Projection = camera.projectionView;
                }
                mesh.Draw();
            }
        }
    }
}