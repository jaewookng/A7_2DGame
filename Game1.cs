// Group 2

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace group_2_assignment7;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    
    private Texture2D _tileMap;
    private Terrain terrain;
    private List<Rectangle> terrainCollison;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
        
        _graphics.PreferredBackBufferHeight = 1000;
        _graphics.PreferredBackBufferWidth = 1600;
        _graphics.ApplyChanges();
        
        _tileMap =  Content.Load<Texture2D>("Tiles");
        terrain = new Terrain(_tileMap, 32, 2);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
        
        // access terrain data
        // incorporate with enemy collision
        List<Rectangle> terrainCollison = terrain.GetTerrain();
        
        //player interaction w terrain
        //////terrain.HitboxInteraction(player.position, player.hitbox, player.velocity);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        // TODO: Add your drawing code here
        _spriteBatch.Begin();
        terrain.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}