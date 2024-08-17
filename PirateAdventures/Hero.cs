using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PirateAdventures.Interfaces;

namespace PirateAdventures
{
    public class Hero : IGameObject
    {
        Texture2D heroTexture;
        private Rectangle _deelRectangle;
        private int _schuifOp_X = 0;

        public Hero(Texture2D texture)
        {
            heroTexture = texture;
            _deelRectangle = new Rectangle(_schuifOp_X, 0, 58, 58);
        }

        public void Update()
        {
            _schuifOp_X += 58;
            if (_schuifOp_X > 26 * 58) _schuifOp_X = 0;
            _deelRectangle.X = _schuifOp_X;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(heroTexture, new Vector2(0, 0), _deelRectangle, Color.White);
        }

    }
}