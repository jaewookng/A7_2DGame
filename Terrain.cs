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
        _tilePalette.Add(new Rectangle(tileSize, 0, tileSize, tileSize));
        _tilePalette.Add(new Rectangle(tileSize*2, 0, tileSize, tileSize));
        
        _layers = new List<int[]>();
        
        MakeBottomLayer();
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
    
    
    public void HitboxInteraction(ref Vector2 position, Rectangle hitbox, ref Vector2 velocity)
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
        return hitboxes;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < mapPathBottom.Length; i++)
        {
            int tileIndex = mapPathBottom[i]; //match tile in palette to number in mappath
            
            if (tileIndex < 0) continue;

            //where tile goes on screen
            //400 as h for now
            Vector2 position = new Vector2(i * (_tileSize*_tileScale), 800); 

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