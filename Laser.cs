using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gaming
{
    
    public class Laser
    {
        Texture2D texture;

        public Facing direction;
        public Vector2 Position;
        Vector2 position;
        public float opacity;
        private ContentManager Content;
        

        private static int BPM = 120;
        private float delay = 60 / (float)BPM;

        public int animationFrame = 0;
        public bool Hurty = false;

        public Laser(Facing dir, Vector2 Pos, GameTime gameTime, ContentManager content)
        {
            opacity = 1f;
            direction = dir;
            Position = Pos;

            position.X = (Position.X * 64) + Constants.BORDERSIZE;
            position.Y = (Position.Y * 64) + Constants.BORDERSIZE;

            Content = content;

            LoadContent();
        }

        public void LoadContent()
        {
            texture = Content.Load<Texture2D>("LazerAttackParticle");
        }
        public void Update(GameTime gameTime)
        {
            delay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (delay <= 0)
            {
                delay = 60 / (float)BPM;
                animationFrame++;
                if (animationFrame > 3)
                {
                    animationFrame--;
                    if(opacity == 1f)
                    {
                        opacity = 0.5f;
                    }
                    else
                    {
                        animationFrame = 5;
                    }

                }
                else
                {
                    opacity = 1f;
                }
                if (animationFrame == 3)
                {
                    Hurty = true;
                }
            }
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int angle = ((int)direction + 1) * 90;
            Vector2 origin = Vector2.Zero;
            SpriteEffects effects = SpriteEffects.None;
            int offset = 0;
            if(angle == 180 || angle == 360)
            {
                angle = 0;
                effects = SpriteEffects.FlipHorizontally;
            }

            if(angle == 270)
            {
                origin = new Vector2(64, 0);
            }
            if(angle == 90)
            {
                origin = new Vector2(0, 64);
            }
            if (animationFrame <= 0)
            {
                offset = 256;
                opacity = 0f;
            }
            spriteBatch.Draw(texture, position, new Rectangle(64 * animationFrame - 64 + offset, 0, 64, 64), Color.White * opacity, MathHelper.ToRadians(angle), origin, 1f, effects, 0);
        }
    }
}
