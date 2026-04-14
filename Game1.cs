using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace group_2_assignment7
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // GLOBAL GAME STATE & UI
        private SpriteFont _guiFont;
        private bool _isGameOver = false;
        private bool _isVictory = false;
        
        // DHARMA'S ENEMY VARIABLES
        private List<Enemy> _enemies;
        private Texture2D _enemyTexture;
        private Texture2D _blankTexture;
        private float _enemySpawnTimer = 0f; // the "additional feature timer" requirement
        
        //JAEWOO'S PLAYER VARIABLES
        private Player _player;
        
        // XINLIN'S ENVIRONMENT PLACEHOLDERS
        // public Environment _level;
        private Texture2D _tileMap;
        private Terrain terrain;
        private List<Rectangle> terrainCollison;
    
        private Texture2D sky;
        private Texture2D cloudsMiddle;
        private Texture2D cloudsFront;
        private Texture2D trees;

        private Background bgLayer1;
        private Background bgLayer2;
        private Background bgLayer3;
        private Background bgLayer4;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();

            _enemies = new List<Enemy>();

            // XINLIN'S CODE HERE
            // terrain is created in LoadContent

            // JAEWOO'S CODE HERE
            _player = new Player(new Vector2(100, 296));

            //spawn an initial enemy to test
            _enemies.Add(new Enemy(new Vector2(600, 100)));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            //load Font for GUI
            _guiFont = Content.Load<SpriteFont>("DefaultFont");

            // DHARMA'S CODE
            _enemyTexture = Content.Load<Texture2D>("50054d2b-242e-47f8-9ff4-911adb4c5e13 (1)");
            _blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            _blankTexture.SetData(new[] { Color.White });
            
            // JAEWOO'S CODE HERE
            _player.LoadContent(Content);
            
            // XINLIN'S CODE HERE
            // create tilemap/terrain, background
            _tileMap =  Content.Load<Texture2D>("Tiles");
            terrain = new Terrain(_tileMap, 32, 2);
            
            sky = Content.Load<Texture2D>("bg_layer1");
            cloudsMiddle = Content.Load<Texture2D>("bg_layer2");
            cloudsFront = Content.Load<Texture2D>("bg_layer3");
            trees = Content.Load<Texture2D>("fg_layer1");
            
            bgLayer1 = new Background(sky, 2f);
            bgLayer2 = new Background(cloudsMiddle, 4f);
            bgLayer3 = new Background(cloudsFront, 15f);
            bgLayer4 = new Background(trees, _player.Velocity.X);//------ replace 0f with player velocity and it should scroll with player------
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //freeze the game if the player wins or loses
            if (_isGameOver || _isVictory) return;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // XINLIN'S CODE HERE
            // access terrain data
            // incorporate with enemy collision
            List<Rectangle> terrainCollison = terrain.GetTerrain();
        
            // -------------------------player interaction w terrain----------------------
            //////terrain.HitboxInteraction(player.position, player.hitbox, player.velocity);
            // -------------------------------------------------------------------------
        
            //parallax bckground
            bgLayer1.Scroll(gameTime);
            bgLayer2.Scroll(gameTime);
            bgLayer3.Scroll(gameTime);
            bgLayer4.Scroll(gameTime);
            
            // JAEWOO'S CODE HERE
            bool playerAttacking = _player.Update(gameTime, terrainCollison);

            // DHARMA'S CODE: ENEMY UPDATES
            
            // spawner timer Logic
            _enemySpawnTimer += deltaTime;
            if (_enemySpawnTimer >= 10f && _enemies.Count < 5) //spawn a new enemy every 10 seconds, max 5
            {
                _enemies.Add(new Enemy(new Vector2(800, 100))); //spawn off right side of screen
                _enemySpawnTimer = 0f;
            }

            //update all existing enemies
            for (int i = _enemies.Count - 1; i >= 0; i--)
            {
                //we pass in the player's real data now
                bool didHitPlayer = _enemies[i].Update(gameTime, _player.Position, _player.Hitbox, terrainCollison);

                //combat logic
                if (didHitPlayer)
                {
                    _player.TakeDamage(1);
                }

                //if Jaewoo's player attacks and hits the enemy hitbox, deal damage
                if (playerAttacking && _player.AttackHitbox.Intersects(_enemies[i].Hitbox))
                {
                    _enemies[i].TakeDamage(1);
                }
                
                //remove dead enemies
                if (_enemies[i].HealthPoints <= 0)
                {
                    _enemies.RemoveAt(i);
                }
            }


            //WIN / LOSE CONDITIONS (GUI REQUIREMENT)
            if (_player.IsDead)
            {
                _isGameOver = true;
            }
            //win condition: Survived the waves and killed all enemies
            else if (_enemies.Count == 0 && _enemySpawnTimer < 0f) //modify logic based on final level design
            {
                _isVictory = true;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.LinearWrap);

            //XINLIN'S CODE HERE
            //------------------------------terrain and background --------------------------------------
            
            bgLayer1.Display(_spriteBatch, Window.ClientBounds.Width, Window.ClientBounds.Height);
            bgLayer2.Display(_spriteBatch, Window.ClientBounds.Width, Window.ClientBounds.Height);
            bgLayer3.Display(_spriteBatch, Window.ClientBounds.Width, Window.ClientBounds.Height);
            bgLayer4.Display(_spriteBatch, Window.ClientBounds.Width, Window.ClientBounds.Height);
            
            terrain.Draw(_spriteBatch);
            // //---------------------------------------------------------------------------------

            //JAEWOO'S CODE HERE
            _player.Draw(_spriteBatch);

            //DHARMA'S CODE: ENEMY DRAW
            foreach (var enemy in _enemies)
            {
                //if no texture yet, this will crash, comment it out if testing just logic.
                enemy.Draw(_spriteBatch, _enemyTexture);
            }

            //GUI / HUD REQUIREMENT
            _player.DrawHealthBar(_spriteBatch, _blankTexture, _guiFont);

            if (_isGameOver)
            {
                _spriteBatch.DrawString(_guiFont, "GAME OVER! You Died.", new Vector2(300, 250), Color.Red);
            }
            else if (_isVictory)
            {
                _spriteBatch.DrawString(_guiFont, "VICTORY! All Enemies Defeated.", new Vector2(280, 250), Color.Gold);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}