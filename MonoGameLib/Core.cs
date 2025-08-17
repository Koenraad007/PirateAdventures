using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLib.Input;
using MonoGameLib.Scenes;
using System;
using System.Diagnostics;

namespace MonoGameLib
{
    public class Core : Game
    {
        internal static Core _instance;

        public static Core Instance => _instance;

        private static Scene _activeScene;
        private static Scene _nextScene;

        public static GraphicsDeviceManager Graphics { get; private set; }

        public static new GraphicsDevice GraphicsDevice { get; private set; }

        public static SpriteBatch SpriteBatch { get; private set; }

        public static new ContentManager Content { get; private set; }

        public static InputManager Input { get; private set; }

        public static bool ExitOnEscape { get; set; } = true;

        public Core(string title, int width, int height, bool fullScreen)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException($"Only a single core instance can be created!");
            }

            _instance = this;

            Graphics = new GraphicsDeviceManager(this);

            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;
            Graphics.IsFullScreen = fullScreen;
            Window.AllowUserResizing = true;
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;

            Graphics.ApplyChanges();

            Window.Title = title;

            Content = base.Content;

            Content.RootDirectory = "Content";

            IsMouseVisible = true;

        }
        protected override void Initialize()
        {
            base.Initialize();

            GraphicsDevice = base.GraphicsDevice;

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Input = new InputManager();
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();

            if (_nextScene != null)
            {
                TransitionScene();
            }

            if (_activeScene != null)
            {
                _activeScene.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _activeScene?.Draw(gameTime);

            base.Draw(gameTime);
        }

        public static void ChangeScene(Scene next)
        {
            if (_activeScene != next)
            {
                _nextScene = next;
            }
        }

        private static void TransitionScene()
        {
            if (_activeScene != null)
            {
                _activeScene.UnloadContent();
                _activeScene.Dispose();
            }
            GC.Collect();
            _activeScene = _nextScene;
            _nextScene = null;
            _activeScene?.Initialize();
        }
    }
}
