using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PirateAdventures.Animations;
using PirateAdventures.Interfaces;

namespace PirateAdventures
{
    public class Hero : IGameObject
    {
        Texture2D heroTexture;
        Animation animation;

        public Hero(Texture2D texture)
        {
            heroTexture = texture;
            animation = new Animation();

            for (int i = 0; i < 26; i++)
            {
                animation.AddFrame(new AnimationFrame(new Rectangle(58 * i, 0, 58, 58)));
            }

        }

        public void Update(GameTime gameTime)
        {
            animation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(heroTexture, new Vector2(0, 0), animation.CurrentFrame.SourceRect, Color.White);
        }

    }
}