using Gaming.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System.Collections.Generic;
using System.Security.Policy;

namespace Gaming.GameLoops
{
    public abstract class LevelLoop
    {
        public TiledMap _tiledMap;
        public TiledMapRenderer _tiledMapRenderer;

        public int[,] Obstacles;

        public GraphicsDevice graphicsDevice;
        public ContentManager Content;

        public PlayerController player;

        
        public SoundEffect backgroundMusic;
        public int SongLength;
        public float SongTimer = 0;

        public List<Soup> soups;

        public virtual void Initialize(ContentManager content, GraphicsDevice gd)
        {
            graphicsDevice = gd;
            Content = content;
            soups = new List<Soup>();
        }
        public abstract void LoadContent();
        public abstract bool Update(GameTime gameTime);
        public abstract bool Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
