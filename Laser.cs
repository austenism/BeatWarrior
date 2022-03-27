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

        Facing direction;
        Vector2[] Position;
        Vector2[] position;
        ContentManager Content;

        private static int BPM = 120;
        private float delay = 60 / (float)BPM;

        int animationFrame = 0;
        public bool Hurty = false;

        public Laser(Facing dir, Vector2 Pos, GameTime gameTime, ContentManager content)
        {
            direction = dir;
            if(direction == Facing.Up)
            {

            }
            if (direction == Facing.Down)
            {

            }
            if (direction == Facing.Left)
            {

            }
            if (direction == Facing.Right)
            {

            }

            position.X = (Position.X * 64) + Constants.BORDERSIZE;
            position.Y = (Position.Y * 64) + Constants.BORDERSIZE;

            Content = content;
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
                if (animationFrame > 2)
                {
                    animationFrame = 0;
                }
                if (animationFrame == 2)
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

        }
    }
}
