using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace group_2_assignment7
{
    public struct Weapon
    {
        public int AttackPower;
        public float Range;

        public Weapon(int attackPower, float range)
        {
            AttackPower = attackPower;
            Range = range;
        }
    }

    public class Player
    {
        public Vector2 Position;
        public int HealthPoints { get; private set; }
        public float MoveSpeed { get; private set; }
        public bool IsAttacking { get; private set; }

        public Vector2 Velocity;
        private float _gravity = 0.5f;
        private int _frameWidth = 200;
        private int _frameHeight = 200;
        private float _drawScale = 1.5f;
        private int _hitboxWidth = 50;
        private int _hitboxHeight = 55;
        private Vector2 _hitboxOffset = new Vector2(75, 50);
        private bool _facingRight = true;
        private bool _isOnGround = false;
        private Weapon _weapon;

        // animation textures. the download pack had one png strip per anim state
        private Texture2D _idleTexture;
        private Texture2D _runTexture;
        private Texture2D _jumpTexture;
        private Texture2D _fallTexture;
        private Texture2D _attack1Texture;
        private Texture2D _takeHitTexture;
        private Texture2D _deathTexture;

        // frame counts per anim
        private int _idleFrames = 4;
        private int _runFrames = 8;
        private int _jumpFrames = 2;
        private int _fallFrames = 2;
        private int _attack1Frames = 4;
        private int _takeHitFrames = 3;
        private int _deathFrames = 7;

        // anim state
        private float _animationTimer;
        private float _timePerFrame = 0.3f;
        private int _currentFrame;
        private int _activeFrameCount;
        private Texture2D _activeTexture;

        // take hit
        private bool _isTakingHit = false;
        private float _takeHitTimer = 0f;
        private float _takeHitDuration;

        // death
        private bool _isDead = false;
        private bool _deathAnimationDone = false;

        // previous mouse state for click detection
        private MouseState _previousMouseState;

        public Player(Vector2 startPosition)
        {
            Position = startPosition;
            HealthPoints = 5;
            MoveSpeed = 4.0f;
            Velocity = Vector2.Zero;
            IsAttacking = false;
            _weapon = new Weapon(1, 50f);
            _takeHitDuration = _takeHitFrames * _timePerFrame;
            _previousMouseState = Mouse.GetState();
        }

        public Rectangle Hitbox
        {
            get
            {
                return new Rectangle(
                    (int)(Position.X + _hitboxOffset.X * _drawScale),
                    (int)(Position.Y + _hitboxOffset.Y * _drawScale),
                    (int)(_hitboxWidth * _drawScale),
                    (int)(_hitboxHeight * _drawScale));
            }
        }

        public Rectangle AttackHitbox
        {
            get
            {
                Rectangle hb = Hitbox;
                if (_facingRight)
                {
                    return new Rectangle(hb.Right, hb.Y, (int)(_weapon.Range * _drawScale), hb.Height);
                }
                else
                {
                    return new Rectangle((int)(hb.Left - _weapon.Range * _drawScale), hb.Y, (int)(_weapon.Range * _drawScale), hb.Height);
                }
            }
        }

        public bool IsDead
        {
            get { return _isDead; }
        }

        public bool DeathAnimationDone
        {
            get { return _deathAnimationDone; }
        }

        public void LoadContent(ContentManager content)
        {
            _idleTexture = content.Load<Texture2D>("Idle");
            _runTexture = content.Load<Texture2D>("Run");
            _jumpTexture = content.Load<Texture2D>("Jump");
            _fallTexture = content.Load<Texture2D>("Fall");
            _attack1Texture = content.Load<Texture2D>("Attack1");
            _takeHitTexture = content.Load<Texture2D>("Take Hit");
            _deathTexture = content.Load<Texture2D>("Death");

            _activeTexture = _idleTexture;
            _activeFrameCount = _idleFrames;
        }

        public bool Update(GameTime gameTime, List<Rectangle> terrainHitboxes)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            MouseState mouseState = Mouse.GetState();
            KeyboardState keyState = Keyboard.GetState();

            // if death animation is playing, just animate and return
            if (_isDead)
            {
                _activeTexture = _deathTexture;
                _activeFrameCount = _deathFrames;
                UpdateAnimation(deltaTime);
                if (_currentFrame >= _deathFrames - 1)
                {
                    _deathAnimationDone = true;
                }
                return false;
            }

            // take hit timer
            if (_isTakingHit)
            {
                _takeHitTimer += deltaTime;
                if (_takeHitTimer >= _takeHitDuration)
                {
                    _isTakingHit = false;
                    _takeHitTimer = 0f;
                }
            }

            // attack state from mouse
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                IsAttacking = true;
            }
            else
            {
                IsAttacking = false;
            }

            // movement
            Velocity.X = 0;

            if (!IsAttacking)
            {
                if (keyState.IsKeyDown(Keys.A))
                {
                    Velocity.X = -MoveSpeed;
                    _facingRight = false;
                }
                else if (keyState.IsKeyDown(Keys.D))
                {
                    Velocity.X = MoveSpeed;
                    _facingRight = true;
                }
            }

            // jump
            if ((keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Space)) && _isOnGround)
            {
                Velocity.Y = -15f;
                _isOnGround = false;
            }
            Velocity.Y += _gravity;

            // terrain collision
            HandleCollisions(terrainHitboxes);

            // apply velocity
            Position += Velocity;

            // pick the right animation
            if (_isTakingHit)
            {
                _activeTexture = _takeHitTexture;
                _activeFrameCount = _takeHitFrames;
            }
            else if (IsAttacking)
            {
                _activeTexture = _attack1Texture;
                _activeFrameCount = _attack1Frames;
            }
            else if (!_isOnGround && Velocity.Y < 0)
            {
                _activeTexture = _jumpTexture;
                _activeFrameCount = _jumpFrames;
            }
            else if (!_isOnGround && Velocity.Y > 0)
            {
                _activeTexture = _fallTexture;
                _activeFrameCount = _fallFrames;
            }
            else if (Velocity.X != 0)
            {
                _activeTexture = _runTexture;
                _activeFrameCount = _runFrames;
            }
            else
            {
                _activeTexture = _idleTexture;
                _activeFrameCount = _idleFrames;
            }

            UpdateAnimation(deltaTime);

            _previousMouseState = mouseState;
            return IsAttacking; // the bool update was for this purpose
        }

        private void HandleCollisions(List<Rectangle> terrainHitboxes)
        {
            Rectangle hb = Hitbox;
            int hbW = hb.Width;
            int hbH = hb.Height;
            float offsetX = _hitboxOffset.X * _drawScale;
            float offsetY = _hitboxOffset.Y * _drawScale;

            Rectangle predictedXBox = new Rectangle((int)(Position.X + Velocity.X + offsetX), (int)(Position.Y + offsetY), hbW, hbH);
            Rectangle predictedYBox = new Rectangle((int)(Position.X + offsetX), (int)(Position.Y + Velocity.Y + offsetY), hbW, hbH);

            _isOnGround = false;

            foreach (Rectangle terrain in terrainHitboxes)
            {
                if (predictedXBox.Intersects(terrain))
                {
                    Velocity.X = 0;
                }

                if (predictedYBox.Intersects(terrain))
                {
                    if (Velocity.Y > 0)
                    {
                        Position.Y = terrain.Top - hbH - offsetY;
                        _isOnGround = true;
                    }
                    Velocity.Y = 0;
                }
            }
        }

        private void UpdateAnimation(float deltaTime)
        {
            _animationTimer += deltaTime;
            if (_animationTimer >= _timePerFrame)
            {
                _currentFrame = (_currentFrame + 1) % _activeFrameCount;
                _animationTimer = 0f;
            }
        }

        public void TakeDamage(int damage)
        {
            if (_isDead)
            {
                return;
            }

            HealthPoints -= damage;
            _isTakingHit = true;
            _takeHitTimer = 0f;
            _currentFrame = 0;

            if (HealthPoints <= 0)
            {
                HealthPoints = 0;
                _isDead = true;
                _currentFrame = 0;
                _animationTimer = 0f;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRect = new Rectangle(_currentFrame * _frameWidth, 0, _frameWidth, _frameHeight);

            SpriteEffects flipEffect = SpriteEffects.None;
            if (!_facingRight)
            {
                flipEffect = SpriteEffects.FlipHorizontally;
            }

            spriteBatch.Draw(
                _activeTexture,
                Position,
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                _drawScale,
                flipEffect,
                0f
            );
        }

        public void DrawHealthBar(SpriteBatch spriteBatch, Texture2D blankTexture, SpriteFont font)
        {
            int barX = 20;
            int barY = 20;
            int segmentWidth = 30;
            int segmentHeight = 15;
            int segmentSpacing = 4;

            spriteBatch.DrawString(font, "HP", new Vector2(barX, barY - 2), Color.White);

            int offsetX = barX + 30;

            for (int i = 0; i < 5; i++)
            {
                Rectangle segmentRect = new Rectangle(offsetX + i * (segmentWidth + segmentSpacing), barY, segmentWidth, segmentHeight);

                if (i < HealthPoints)
                {
                    spriteBatch.Draw(blankTexture, segmentRect, Color.Red);
                }
                else
                {
                    spriteBatch.Draw(blankTexture, segmentRect, Color.DarkGray);
                }
            }
        }
    }
}