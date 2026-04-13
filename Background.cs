using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace group_2_assignment7;

public class Background
{
    private Texture2D img;
    private float scrollSpeed;
    
    private float viewStartX = 0f; // where view of the image starts

   
    public Background(Texture2D img, float scrollSpeed){
        this.img = img;
        this.scrollSpeed = scrollSpeed;
    }

    public void Scroll(GameTime gameTime)
    {
        viewStartX += scrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public void Display(SpriteBatch spriteBatch, int screenWidth, int screenHeight)
    {
        spriteBatch.Draw(img, new Rectangle(0, 0, screenWidth, screenHeight), //dest rectang
            new Rectangle((int)viewStartX, 0, img.Width, img.Height), Color.White); //source rectang
    }
}