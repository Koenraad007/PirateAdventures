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
        private Texture2D heroTexture;
        private Animation idle, running;
        private Vector2 position, speed, acceleration;
        private SpriteEffects spriteFx = SpriteEffects.None;
        private IInputReader input;
        private HeroState state;


        public Hero(Texture2D texture, IInputReader inputReader)
        {
            heroTexture = texture;
            input = inputReader;

            idle = new Animation();
            running = new Animation();

            for (int i = 0; i < 26; i++)
            {
                idle.AddFrame(new AnimationFrame(new Rectangle(58 * i, 0, 58, 58)));
            }
            for (int i = 0; i < 14; i++)
            {
                running.AddFrame(new AnimationFrame(new Rectangle(58 * i + (26 * 58), 0, 58, 58)));
            }

            position = new Vector2(10, 10);
            speed = new Vector2(1, 0);
            acceleration = new Vector2(0.1f, 0f);

        }

        public void Update(GameTime gameTime)
        {
            var direction = input.ReadInput();

            // if the hero is not moving, the state is IDLE (0), else it's RUNNING (1)
            state = (HeroState)Math.Abs(direction.X);

            Move(direction);

            switch (state)
            {
                case HeroState.IDLE:
                    running.ResetAnimation();
                    idle.Update(gameTime);
                    break;

                case HeroState.RUNNING:
                    running.Update(gameTime);
                    idle.ResetAnimation();
                    break;

                default: break;
            }

        }

        private void Move(Vector2 direction)
        {
            direction *= speed;

            position += direction;

            //if (Math.Abs(speed.X) < 5) speed += acceleration;



            // if (position.X > 600 || position.X < 0)
            // {
            //     speed *= -1;
            //     acceleration *= -1;
            // }

            if (direction.X < 0) spriteFx = SpriteEffects.FlipHorizontally;
            if (direction.X > 0) spriteFx = SpriteEffects.None;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (state)
            {
                case HeroState.IDLE:
                    spriteBatch.Draw(heroTexture, position, idle.CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), 1f, spriteFx, 0);
                    break;

                case HeroState.RUNNING:
                    spriteBatch.Draw(heroTexture, position, running.CurrentFrame.SourceRect, Color.White, 0, new Vector2(0, 0), 1f, spriteFx, 0);
                    break;

                default: break;
            }
        }

    }

    enum HeroState
    {
        IDLE,
        RUNNING
    }
}