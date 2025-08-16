using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MonoGameLib.Graphics
{
    public class TextureAtlas
    {
        private Dictionary<string, TextureRegion> _regions;
        private Dictionary<string, Animation> _animations;

        public Texture2D Texture { get; set; }

        public TextureAtlas()
        {
            _regions = new Dictionary<string, TextureRegion>();
            _animations = new Dictionary<string, Animation>();
        }

        public TextureAtlas(Texture2D texture)
        {
            Texture = texture;
            _regions = new Dictionary<string, TextureRegion>();
            _animations = new Dictionary<string, Animation>();
        }

        public void AddRegion(string name, int x, int y, int width, int height)
        {
            TextureRegion region = new TextureRegion(Texture, x, y, width, height);
            _regions.Add(name, region);
        }

        public TextureRegion GetRegion(string name)
        {
            return _regions[name];
        }

        public bool RemoveRegion(string name)
        {
            return _regions.Remove(name);
        }

        public void AddAnimation(string name, Animation animation)
        {
            _animations.Add(name, animation);
        }

        public Animation GetAnimation(string name)
        {
            return _animations[name];
        }

        public bool RemoveAnimation(string name)
        {
            return _animations.Remove(name);
        }

        public void ClearAtlas()
        {
            _regions.Clear();
            _animations.Clear();
        }

        /// <summary>
        /// Create a new Sprite using the specified region from the atlas.
        /// </summary>
        /// <param name="regionName">The name of the region in the TextureAtlas.</param>
        /// <returns>A new Sprite.</returns>
        public Sprite CreateSprite(string regionName)
        {
            TextureRegion region = GetRegion(regionName);
            return new Sprite(region);
        }

        /// <summary>
        /// Create a new AnimatedSprite using the specified animation from the atlas.
        /// </summary>
        /// <param name="animationName">The name of the animation in the TextureAtlas.</param>
        /// <returns></returns>
        public AnimatedSprite CreateAnimatedSprite(string animationName)
        {
            Animation animation = GetAnimation(animationName);
            return new AnimatedSprite(animation);
        }

        /// <summary>
        /// Create a new TextureAtlas from an XML config file.
        /// </summary>
        /// <param name="content">The ContentManager that is used to load the file.</param>
        /// <param name="fileName">Name of the XML config file.</param>
        /// <returns>The created TextureAtlas.</returns>
        public static TextureAtlas FromFile(ContentManager content, string fileName)
        {
            TextureAtlas atlas = new TextureAtlas();

            string filePath = Path.Combine(content.RootDirectory, fileName);

            using (Stream stream = TitleContainer.OpenStream(filePath))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    XDocument doc = XDocument.Load(reader);
                    XElement root = doc.Root;

                    string texturePath = root.Element("Texture").Value;
                    atlas.Texture = content.Load<Texture2D>(texturePath);

                    // Example:
                    // <Regions>
                    //      <Region name="spriteOne" x="0" y="0" width="32" height="32" />
                    //      <Region name="spriteTwo" x="32" y="0" width="32" height="32" />
                    // </Regions>
                    var regions = root.Element("Regions")?.Elements("Region");

                    if (regions != null)
                    {
                        foreach (var region in regions)
                        {
                            string name = region.Attribute("name")?.Value;
                            int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                            int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                            int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                            int height = int.Parse(region.Attribute("height")?.Value ?? "0");

                            if (!string.IsNullOrEmpty(name))
                            {
                                atlas.AddRegion(name, x, y, width, height);
                            }
                        }
                    }

                    // Example:
                    // <Animations>
                    //      <Animation name="animation" delay="100">
                    //          <Frame region="spriteOne" />
                    //          <Frame region="spriteTwo" />
                    //      </Animation>
                    // </Animations>
                    var animationElements = root.Element("Animations").Elements("Animation");

                    if (animationElements != null)
                    {
                        foreach (var animationElement in animationElements)
                        {
                            string name = animationElement.Attribute("name")?.Value;
                            float delayInMilliseconds = float.Parse(animationElement.Attribute("delay")?.Value ?? "0");
                            TimeSpan delay = TimeSpan.FromMilliseconds(delayInMilliseconds);

                            List<TextureRegion> frames = new List<TextureRegion>();

                            var frameElements = animationElement.Elements("Frame");

                            if (frameElements != null)
                            {
                                foreach (var frameElement in frameElements)
                                {
                                    string regionName = frameElement.Attribute("region").Value;
                                    TextureRegion region = atlas.GetRegion(regionName);
                                    frames.Add(region);
                                }
                            }

                            Animation animation = new Animation(frames, delay);
                            atlas.AddAnimation(name, animation);
                        }
                    }

                    return atlas;
                }
            }
        }
    }
}
