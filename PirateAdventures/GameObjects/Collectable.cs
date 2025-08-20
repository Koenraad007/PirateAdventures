using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Graphics;
using PirateAdventures.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateAdventures.GameObjects
{
    public class Collectable : IGameObject
    {
        public const int SPRITE_WIDTH = 64;
        public const int SPRITE_HEIGHT = 64;
        private TextureAtlas _textureAtlas;
        private Vector2 _position;
        private AnimatedSprite _currentAnimation;
        private bool pickup = false;
        private CollectableType type;
        public Rectangle BoundingBox;

        public event Action<Collectable, CollectableType> OnPickup;

        public Collectable(TextureAtlas ta, Vector2 pos, CollectableType type) 
        { 
            _textureAtlas = ta;
            _position = new Vector2(pos.X+SPRITE_WIDTH/4f, pos.Y+SPRITE_HEIGHT/4f);
            BoundingBox = new Rectangle((int)_position.X+SPRITE_WIDTH/2-2, (int)_position.Y+SPRITE_HEIGHT/2-2, 4, 4);
            this.type = type;

            var typeString = type.ToString();
            _currentAnimation = _textureAtlas.CreateAnimatedSprite(char.ToLower(typeString[0])+typeString.Substring(1));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _currentAnimation.Draw(spriteBatch, _position);
        }

        public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
        {
            var hero = collisionObjects.FirstOrDefault(obj => obj is Hero) as Hero;
            if (hero != null && hero.BoundingBox.Intersects(BoundingBox) && !pickup)
            {
                pickup = true;
                OnPickup?.Invoke(this, type);
                switch (type)
                {
                    case CollectableType.SilverCoin:
                    case CollectableType.GoldCoin:
                        _currentAnimation = _textureAtlas.CreateAnimatedSprite("coinPickup");
                        break;

                    default:
                        break;
                }
                _currentAnimation.PlayOnce = true;
            }
            
            if (pickup && _currentAnimation.CurrentFrame == _currentAnimation.Animation.Frames.Count-1)
            {
                //collisionObjects.Remove(this);
                return;
            }

            _currentAnimation.Update(gameTime);
        }
    }

    public enum CollectableType
    {
        SilverCoin,
        GoldCoin,
        RedGem,
        BlueGem,
        GreenGem,
        ManaPotion,
        HealthPotion,
        Skull
    }
}
