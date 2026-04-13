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
        bgLayer4 = new Background(trees, 0f);//------ replace 0f with player velocity and it should scroll with player------
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
        
        // -------------------------player interaction w terrain----------------------
        //////terrain.HitboxInteraction(player.position, player.hitbox, player.velocity);
        // -------------------------------------------------------------------------
        
        //parallax bckground
        bgLayer1.Scroll(gameTime);
        bgLayer2.Scroll(gameTime);
        bgLayer3.Scroll(gameTime);
        bgLayer4.Scroll(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        // TODO: Add your drawing code here
        
        //------------------------------terrain and background --------------------------------------
        _spriteBatch.Begin(samplerState: SamplerState.LinearWrap);
        
        bgLayer1.Display(_spriteBatch, Window.ClientBounds.Width, Window.ClientBounds.Height);
        bgLayer2.Display(_spriteBatch, Window.ClientBounds.Width, Window.ClientBounds.Height);
        bgLayer3.Display(_spriteBatch, Window.ClientBounds.Width, Window.ClientBounds.Height);
        bgLayer4.Display(_spriteBatch, Window.ClientBounds.Width, Window.ClientBounds.Height);
        
        terrain.Draw(_spriteBatch);
        //---------------------------------------------------------------------------------
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}