using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace group_2_assignment7;

public class Terrain
{
    private Texture2D _tileMap;
    private int _tileSize;
    private int _tileScale;
    private List<Rectangle> _tilePalette;
    public int[] mapPathBottom;
    
    private List<int[]> _layers;

    public Terrain(Texture2D tilemap, int tileSize, int tileScale)
    {
        _tileMap = tilemap;
        _tileSize = tileSize;
        _tileScale = tileScale;
        _tilePalette = new List<Rectangle>();
        
        //picking speicifically the tiles needed from og tilemap
        _tilePalette.Add(new Rectangle(15, 5, tileSize, tileSize*2)); //grass
        //_tilePalette.Add(new Rectangle(tileSize, 0, tileSize, tileSize));
        //_tilePalette.Add(new Rectangle(tileSize*2, 0, tileSize, tileSize));
        
        _layers = new List<int[]>();
        
        //bottommost layer
        AddLayer(0, 0, 0, 0); //all ground no gaps
        // first layer
        AddLayer(-1, 0, -1, 0); //2 platforms
        //second layer
        AddLayer(-1, 0, 0, -1); //1long plotform
        
        //MakeBottomLayer();
    }

    private void MakeBottomLayer()
    {
        mapPathBottom = new int[30];
        for (int i = 0; i < 20; i++)
        {
            mapPathBottom[i]=0;
        }
        
        _layers.Add(mapPathBottom);
    }

    // segment# = 0 or -1 for is theres tile or no tile
    private void AddLayer(int segment1, int segment2, int segment3, int segment4)
    {
        int[] makingLayer = new int[35];
        
        //bottommost layer
        //for (int i = 0; i < 30; i++)
        //{
          //  makingLayer[i] = 0;
        //}
        
        for (int i = 0; i < 7 ; i++)
        {
            makingLayer[i] = segment1;
        }
        for (int i = 7; i <14; i++)
        {
            makingLayer[i] = segment2;
        }
        for (int i = 14; i <21; i++)
        {
            makingLayer[i] = segment3;
        }
        for (int i = 21; i<makingLayer.Length; i++)
        {
            makingLayer[i] = segment4;
        }
        
        _layers.Add(makingLayer);
    }

    
    //apply hitbox to evrey tile
    public List<Rectangle> Collision()
    {
        List<Rectangle> hitboxes = new List<Rectangle>();

        foreach (var layer in _layers)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                if (layer[i] != -1) //dont make hitbox if empty
                {
                    //apply to space the tile occupies
                    int x = i * (_tileSize * _tileScale);
                    int y = 800;
                    int size = _tileSize * _tileScale;

                    hitboxes.Add(new Rectangle(x, y, size, size));
                }
            }
        }

        return hitboxes;
    }
    
    
    public void HitboxInteraction(Vector2 position, Rectangle hitbox, Vector2 velocity)
    {
        foreach (var externalObj in Collision())
        {
            if (hitbox.Intersects(externalObj))
            {
                Rectangle overlap = Rectangle.Intersect(hitbox, externalObj);
                //land on top of tile
                if (velocity.Y > 0 && hitbox.Bottom > externalObj.Top)
                {
                    position.Y = externalObj.Top - hitbox.Height;
                    velocity.Y = 0;
                }
                
                //walk into tile while moving right
                if (velocity.X > 0 && hitbox.Right > externalObj.Left)
                {
                    position.X = externalObj.Left - hitbox.Width;
                    velocity.X = 0;
                }
                // while moving left
                if (velocity.X < 0 && hitbox.Left > externalObj.Right)
                {
                    position.X = externalObj.Right - hitbox.Width;
                    velocity.X = 0;
                }
            }
        }
    }

    public List<Rectangle> GetTerrain()
    {
        List<Rectangle> hitboxes = new List<Rectangle>();
        int scaledSize = _tileSize * _tileScale;

        for (int layerIndex = 0; layerIndex < _layers.Count; layerIndex++)
        {
            //layers are 250 apart
            int layerY = 800 - (layerIndex * 250); 
            int[] currentLayer = _layers[layerIndex];

            for (int i = 0; i < currentLayer.Length; i++)
            {
                if (currentLayer[i] != -1)
                {
                    int x = i * scaledSize;
                    hitboxes.Add(new Rectangle(x, layerY, scaledSize, scaledSize));
                }
            }
        }
        return hitboxes;
        
        /*List<Rectangle> hitboxes = new List<Rectangle>();
        int scaledSize = _tileSize * _tileScale;

        foreach (var layer in _layers)
        {
            for (int i = 0; i < layer.Length; i++)
            {
                if (layer[i] != -1)
                {
                    //multilayer logic????????/////////////////
                    int x = i * scaledSize;
                    int y = 800;

                    hitboxes.Add(new Rectangle(x, y, scaledSize, scaledSize));
                }
            }
        }
        return hitboxes;*/
    }
    
    //updated draw cuz layer heights need to be different
    public void Draw(SpriteBatch spriteBatch)
    {
        int scaledSize = _tileSize * _tileScale;

        for (int layerIndex = 0; layerIndex < _layers.Count; layerIndex++)
        {
            int layerY = 800 - (layerIndex * 250);
            int[] currentLayer = _layers[layerIndex];

            for (int i = 0; i < currentLayer.Length; i++)
            {
                int tileIndex = currentLayer[i];
                if (tileIndex == -1) continue;

                Vector2 position = new Vector2(i * scaledSize, layerY);
                spriteBatch.Draw(_tileMap, position, _tilePalette[tileIndex], Color.White, 0f, Vector2.Zero, _tileScale, SpriteEffects.None, 0f);
            }
        }
    }
    
/*
    public void Draw(SpriteBatch spriteBatch)
    {
        for (int j = 0; j < _layers.Count; j++)
        {
            for (int i = 0; i < mapPathBottom.Length; i++)
            {
                int tileIndex = mapPathBottom[i]; //match tile in palette to number in mappath

                if (tileIndex < 0) continue;

                //where tile goes on screen
                //400 as h for now
                Vector2 position = new Vector2(i * (_tileSize * _tileScale), 800);

                spriteBatch.Draw(
                    _tileMap,
                    position,
                    _tilePalette[tileIndex], //now draw tile
                    Color.White,
                    0f,
                    Vector2.Zero,
                    _tileScale,
                    SpriteEffects.None,
                    1f
                );
            }
        }
    }
*/
}