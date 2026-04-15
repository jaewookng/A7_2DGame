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
        private bool _isPaused = false;
        
        // DHARMA'S ENEMY VARIABLES
        private List<Enemy> _enemies;
        private Texture2D _enemyTexture;
        private Texture2D _blankTexture;
        // WAVE SYSTEM
        private int _currentWave = 1;
        private int _totalWaves = 10;
        private int _enemiesLeftToSpawn;
        private float _enemySpawnTimer = 0f;
        private float _baseSpawnInterval = 5f;
        private float _waveSpawnInterval;
        private bool _waveInProgress = false;
        private float _waveCooldown = 3f;
        private float _waveCooldownTimer = 0f;
        private bool _waitingForNextWave = false;
        
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

        private ViewPort viewPort;
        private MouseState _previousMouseState;

        // HUD BUTTONS
        private Rectangle _restartButton = new Rectangle(880, 10, 100, 30);
        private Rectangle _exitButton = new Rectangle(880, 45, 100, 30);
        private Rectangle _pauseButton = new Rectangle(880, 80, 100, 30);

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();

            _enemies = new List<Enemy>();

            // XINLIN'S CODE HERE
            // terrain is created in LoadContent
            viewPort = new ViewPort();

            // JAEWOO'S CODE HERE
            _player = new Player(new Vector2(100, 296));

            // WAVE SYSTEM INIT
            _enemiesLeftToSpawn = _currentWave;
            _waveSpawnInterval = _baseSpawnInterval;
            _waveInProgress = true;

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

            // HUD BUTTON CLICKS (always active, even when frozen)
            MouseState mouseState = Mouse.GetState();
            bool clicked = mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
            Point mousePoint = new Point(mouseState.X, mouseState.Y);

            if (clicked)
            {
                if (_exitButton.Contains(mousePoint))
                {
                    Exit();
                }
                if (_restartButton.Contains(mousePoint))
                {
                    ResetGame();
                    _previousMouseState = mouseState;
                    return;
                }
                if (_pauseButton.Contains(mousePoint))
                {
                    _isPaused = !_isPaused;
                }
            }
            _previousMouseState = mouseState;

            viewPort.Update(_player.Position, Window.ClientBounds.Width, Window.ClientBounds.Height);
            
            //freeze the game if the player wins, loses, or pauses
            if (_isGameOver || _isVictory || _isPaused) return;

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
            
            // WAVE SPAWNING
            if (_waveInProgress)
            {
                _enemySpawnTimer += deltaTime;
                if (_enemySpawnTimer >= _waveSpawnInterval && _enemiesLeftToSpawn > 0)
                {
                    _enemies.Add(new Enemy(new Vector2(800, 100)));
                    _enemiesLeftToSpawn--;
                    _enemySpawnTimer = 0f;
                }

                if (_enemiesLeftToSpawn <= 0 && _enemies.Count == 0)
                {
                    _waveInProgress = false;
                    if (_currentWave >= _totalWaves)
                    {
                        _isVictory = true;
                    }
                    else
                    {
                        _waitingForNextWave = true;
                        _waveCooldownTimer = 0f;
                    }
                }
            }

            if (_waitingForNextWave)
            {
                _waveCooldownTimer += deltaTime;
                if (_waveCooldownTimer >= _waveCooldown)
                {
                    _currentWave++;
                    _enemiesLeftToSpawn = _currentWave;
                    _waveSpawnInterval = _baseSpawnInterval - (_currentWave - 1) * 0.5f;
                    if (_waveSpawnInterval < 0.5f)
                    {
                        _waveSpawnInterval = 0.5f;
                    }
                    _enemySpawnTimer = 0f;
                    _waveInProgress = true;
                    _waitingForNextWave = false;
                }
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


            //LOSE CONDITION
            if (_player.IsDead || _player.Position.Y >= 900)
            {
                _isGameOver = true;
            }
            

            base.Update(gameTime);
        }

        private void ResetGame()
        {
            _player = new Player(new Vector2(100, 296));
            _player.LoadContent(Content);
            _enemies.Clear();
            _currentWave = 1;
            _enemiesLeftToSpawn = 1;
            _waveSpawnInterval = _baseSpawnInterval;
            _enemySpawnTimer = 0f;
            _waveInProgress = true;
            _waitingForNextWave = false;
            _waveCooldownTimer = 0f;
            _isGameOver = false;
            _isVictory = false;
            _isPaused = false;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //XINLIN'S CODE HERE
            //------------------------------terrain and background --------------------------------------
            _spriteBatch.Begin(samplerState: SamplerState.LinearWrap);
            
            bgLayer1.Display(_spriteBatch, Window.ClientBounds.Width, Window.ClientBounds.Height);
            bgLayer2.Display(_spriteBatch, Window.ClientBounds.Width, Window.ClientBounds.Height);
            bgLayer3.Display(_spriteBatch, Window.ClientBounds.Width, Window.ClientBounds.Height);
            bgLayer4.Display(_spriteBatch, Window.ClientBounds.Width, Window.ClientBounds.Height);
            
            _spriteBatch.End();
            
            // separate spritebatch for camerawork (eveyrthign besides backgrounds & HUD/UI shoudl be under this)
            _spriteBatch.Begin(transformMatrix: viewPort.Transform);
            
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
            
            _spriteBatch.End();

            
            //GUI / HUD REQUIREMENT
            _spriteBatch.Begin();
            
            _player.DrawHealthBar(_spriteBatch, _blankTexture, _guiFont);

            if (!_isGameOver && !_isVictory)
            {
                _spriteBatch.DrawString(_guiFont, "Wave " + _currentWave + "/" + _totalWaves, new Vector2(20, 45), Color.White);
            }

            // RESTART & EXIT BUTTONS
            MouseState ms = Mouse.GetState();
            Point mp = new Point(ms.X, ms.Y);

            Color restartColor = _restartButton.Contains(mp) ? Color.Gray : Color.DarkGray;
            Color exitColor = _exitButton.Contains(mp) ? Color.DarkRed : Color.Gray;

            _spriteBatch.Draw(_blankTexture, _restartButton, restartColor);
            _spriteBatch.DrawString(_guiFont, "Restart", new Vector2(_restartButton.X + 10, _restartButton.Y + 5), Color.White);

            _spriteBatch.Draw(_blankTexture, _exitButton, exitColor);
            _spriteBatch.DrawString(_guiFont, "Exit", new Vector2(_exitButton.X + 25, _exitButton.Y + 5), Color.White);

            Color pauseColor = _pauseButton.Contains(mp) ? Color.Gray : Color.DarkGray;
            _spriteBatch.Draw(_blankTexture, _pauseButton, pauseColor);
            _spriteBatch.DrawString(_guiFont, _isPaused ? "Resume" : "Pause", new Vector2(_pauseButton.X + 10, _pauseButton.Y + 5), Color.White);

            if (_isPaused)
            {
                _spriteBatch.DrawString(_guiFont, "PAUSED", new Vector2(430, 250), Color.White);
            }
            else if (_isGameOver)
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