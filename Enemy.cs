using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace group_2_assignment7
{
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }

    public class Enemy
    {
        public Vector2 Position;
        public int HealthPoints { get; private set; }
        public float MoveSpeed { get; private set; }
        
        private Vector2 _velocity;
        private float _gravity = 0.5f;
        private int _width = 125;
        private int _height = 125;
        
        private EnemyState _currentState;
        private float _detectionRange = 300f;
        private float _attackRange = 40f;
        private float _attackCooldown = 1.5f;
        private float _attackTimer = 0f;

        // idle patrol jw. the slimes will still not follow the player over that right edge unfortunately
        private float _patrolTimer = 0f;
        private float _patrolDuration = 2f;
        private int _patrolDirection = 1;

        private float _animationTimer;
        private float _timePerFrame = 0.15f;
        private int _currentFrame;
        private int _totalFrames = 8;

        public Enemy(Vector2 startPosition)
        {
            Position = startPosition;
            HealthPoints = 3;
            MoveSpeed = 2.0f;
            _velocity = Vector2.Zero;
            _currentState = EnemyState.Idle;
        }

        public Rectangle Hitbox
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, _width, _height); }
        }

        public bool Update(GameTime gameTime, Vector2 playerPos, Rectangle playerHitbox, List<Rectangle> terrainHitboxes)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            bool didAttackPlayer = false;

            float distanceToPlayer = Vector2.Distance(Position, playerPos);

            if (distanceToPlayer <= _attackRange)
            {
                _currentState = EnemyState.Attack;
            }
            else if (distanceToPlayer <= _detectionRange)
            {
                _currentState = EnemyState.Chase;
            }
            else
            {
                _currentState = EnemyState.Idle;
            }

            _velocity.X = 0;

            switch (_currentState)
            {
                case EnemyState.Idle:
                    _patrolTimer += deltaTime;
                    if (_patrolTimer >= _patrolDuration)
                    {
                        _patrolDirection *= -1;
                        _patrolTimer = 0f;
                    }
                    _velocity.X = MoveSpeed * 0.5f * _patrolDirection;
                    break;

                case EnemyState.Chase:
                    if (playerPos.X < Position.X)
                        _velocity.X = -MoveSpeed;
                    else if (playerPos.X > Position.X)
                        _velocity.X = MoveSpeed;
                    break;

                case EnemyState.Attack:
                    _attackTimer += deltaTime;
                    if (_attackTimer >= _attackCooldown)
                    {
                        if (Hitbox.Intersects(playerHitbox))
                        {
                            didAttackPlayer = true;
                        }
                        _attackTimer = 0f;
                    }
                    break;
            }

            _velocity.Y += _gravity;

            HandleCollisions(terrainHitboxes);

            Position += _velocity;

            UpdateAnimation(deltaTime);

            return didAttackPlayer;
        }

        private void HandleCollisions(List<Rectangle> terrainHitboxes)
        {
            Rectangle predictedXBox = new Rectangle((int)(Position.X + _velocity.X), (int)Position.Y, _width, _height);
            
            Rectangle predictedYBox = new Rectangle((int)Position.X, (int)(Position.Y + _velocity.Y), _width, _height);

            foreach (Rectangle terrain in terrainHitboxes)
            {
                if (predictedXBox.Intersects(terrain))
                {
                    _velocity.X = 0;
                    
                    if (_currentState == EnemyState.Chase && _velocity.Y >= 0) 
                    {
                        _velocity.Y = -8f;
                    }
                }

                if (predictedYBox.Intersects(terrain))
                {
                    if (_velocity.Y > 0)
                    {
                        Position.Y = terrain.Top - _height;
                    }
                    _velocity.Y = 0;
                }
            }
        }

        private void UpdateAnimation(float deltaTime)
        {
            _animationTimer += deltaTime;
            if (_animationTimer >= _timePerFrame)
            {
                _currentFrame = (_currentFrame + 1) % _totalFrames;
                _animationTimer = 0f;
            }
        }

        public void TakeDamage(int damage)
        {
            HealthPoints -= damage;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D enemySheet)
        {
            int rowOffset = 0; 

            Rectangle sourceRect = new Rectangle(_currentFrame * _width, rowOffset, _width, _height);

            SpriteEffects flipEffect = SpriteEffects.None;
            if (_velocity.X < 0) 
            {
                flipEffect = SpriteEffects.FlipHorizontally;
            }

            spriteBatch.Draw(
                enemySheet, 
                Position, 
                sourceRect, 
                Color.White, 
                0f, 
                Vector2.Zero, 
                1f, 
                flipEffect, 
                0f
            );
        }
    }
}