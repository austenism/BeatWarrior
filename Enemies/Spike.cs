using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gaming.Enemies
{
    public class Spike
    {
        private ContentManager Content;
        Texture2D texture;

        private static int BPM = 120;
        private float delay = 60 / (float)BPM;
        bool movedThisBeat = false;

        int animationFrame = 0;
        public bool Hurty = false;

        /// <summary>
        /// vector that says where he is by the pixel
        /// </summary>
        private Vector2 position;
        /// <summary>
        /// his position according to the grid
        /// </summary>
        public Vector2 Position;

        public Spike(ContentManager content, int x, int y)
        {
            Content = content;

            Position.X = x;
            Position.Y = y;

            position.X = (Position.X * 64) + Constants.BORDERSIZE;
            position.Y = (Position.Y * 64) + Constants.BORDERSIZE;
        }
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Enemies/SpikeBrass");
        }

        public void Update(GameTime gameTime)
        {
            delay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(delay <= 0)
            {
                delay = 60 / (float)BPM;
                animationFrame++;
                if(animationFrame > 2)
                {
                    animationFrame = 0;
                }
                if(animationFrame == 2)
                {
                    Hurty = true;
                }
                else
                {
                    Hurty = false;
                }
            }
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle source = new Rectangle(animationFrame * 64, 0, 64, 64);

            spriteBatch.Draw(texture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }
}
