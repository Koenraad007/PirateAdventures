using Microsoft.Xna.Framework;
using PirateAdventures.GameObjects;
using PirateAdventures.Input;
using PirateAdventures.Interfaces;
using PirateAdventures.Managers;
using PirateAdventures.Settings;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PirateAdventures.Factories
{
    public class CompanionFactory : IGameObjectFactory
    {
        public bool CanCreate(string objectType) => objectType.ToLower() == "companion";

        public IGameObject CreateGameObject(string objectType, Vector2 position)
        {
            KeyboardInputReader kir = new KeyboardInputReader(SettingsManager.LoadSettings());
            return new Companion(kir, TextureManager.Instance.GetTexture("companion"))
            {
                Position = new Vector2(position.X, position.Y - Companion.SPRITE_HEIGHT)
            };
        }
    }
}
