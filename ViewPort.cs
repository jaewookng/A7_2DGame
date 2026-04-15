using Microsoft.Xna.Framework;

namespace group_2_assignment7;

public class ViewPort
{
    public Matrix Transform { get; set; }

    //camera constantly centers player
    public void Update(Vector2 playerPosition, int screenWidth, int screenHeight)
    {
        Transform = Matrix.CreateTranslation(-playerPosition.X, -playerPosition.Y, 0) 
                    * Matrix.CreateTranslation(screenWidth/2, screenHeight/2, 0); 
    }
    
}