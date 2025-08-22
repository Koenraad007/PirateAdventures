using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLib.Graphics;
using PirateAdventures.Interfaces;
using PirateAdventures.Managers;
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
        private readonly TextureAtlas _textureAtlas;
        private Vector2 _position;
        private AnimatedSprite _currentAnimation;
        public bool IsCollected { get; set; } = false;
        private bool _collected = false;
        private readonly CollectableType type;
        public Rectangle BoundingBox { get; set; }

        public event Action<Collectable, CollectableType> OnPickup;

        public Collectable(TextureAtlas ta, Vector2 pos, CollectableType type)
        {
            _textureAtlas = ta;
            _position = new Vector2(pos.X + SPRITE_WIDTH / 4f, pos.Y + SPRITE_HEIGHT / 4f);
            BoundingBox = new Rectangle((int)_position.X + SPRITE_WIDTH / 2 - 2, (int)_position.Y + SPRITE_HEIGHT / 2 - 2, 4, 4);
            this.type = type;

            var typeString = type.ToString();
            _currentAnimation = _textureAtlas.CreateAnimatedSprite(char.ToLower(typeString[0]) + typeString.Substring(1));

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _currentAnimation.Draw(spriteBatch, _position);
        }

        public void Update(List<IGameObject> collisionObjects, GameTime gameTime)
        {
            var collectableHeroes = collisionObjects.Where(obj => obj is Hero || obj is Companion).ToList();
            bool coinIntersected = false;
            foreach (var obj in collectableHeroes)
            {
                var collidable = obj as ICollidable;
                if (collidable != null && collidable.BoundingBox.Intersects(BoundingBox))
                {
                    coinIntersected = true;
                    break;
                }
            }

            if (coinIntersected && !_collected)
            {
                _collected = true;
                SoundManager.Instance.PlaySound("coin", .3f, false);
                OnPickup?.Invoke(this, type);
                switch (type)
                {
                    case CollectableType.SilverCoin:
                    case CollectableType.GoldCoin:
                        _currentAnimation = _textureAtlas.CreateAnimatedSprite("coinPickup");
                        break;
                    case CollectableType.RedGem:
                    case CollectableType.BlueGem:
                    case CollectableType.GreenGem:
                        _currentAnimation = _textureAtlas.CreateAnimatedSprite("gemPickup");
                        break;
                    case CollectableType.ManaPotion:
                    case CollectableType.HealthPotion:
                        _currentAnimation = _textureAtlas.CreateAnimatedSprite("potionPickup");
                        break;
                    case CollectableType.Skull:
                        _currentAnimation = _textureAtlas.CreateAnimatedSprite("skullPickup");
                        break;

                    default:
                        break;
                }
                _currentAnimation.PlayOnce = true;
            }

            if (_collected && _currentAnimation.CurrentFrame == _currentAnimation.Animation.Frames.Count - 1)
            {
                IsCollected = true;
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
