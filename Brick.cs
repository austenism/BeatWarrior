using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gaming
{
    
    public class Brick
    {
        Texture2D texture;

        public Facing direction;
        public Vector2 Position;
        Vector2 position;
        private ContentManager Content;
        

        private static int BPM = 120;
        private float delay = 60 / (float)BPM;

        public int facingModified;

        public int deltaX;
        public int deltaY;

        public int animationFrame = 0;
        public bool Hurty = true;

        public Brick(Facing dir, Vector2 Pos, GameTime gameTime, ContentManager content)
        {
            direction = dir;
            Position = Pos;

            facingModified = (((((int)direction + 1) / 2) % 2) * 2) - 1;

            deltaX = (facingModified * (int)direction % 2) * -1;
            deltaY = (facingModified * ((int)direction + 1) % 2) * -1;

            position.X = (Position.X * 64) + Constants.BORDERSIZE;
            position.Y = (Position.Y * 64) + Constants.BORDERSIZE;

            Content = content;

            LoadContent();
        }
        
        public void LoadContent()
        {
            texture = Content.Load<Texture2D>("BrickAttackParticle");
        }
        public void Update(GameTime gameTime)
        {
            #region PositionCalculation
            //update positions relative to grid and stuff
                position.X = (Position.X * 64) + Constants.BORDERSIZE;
                position.Y = (Position.Y * 64) + Constants.BORDERSIZE;
            #endregion
            int modifiedfacing = (((((int)direction + 1) / 2) % 2) * 2) - 1;
            delay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (delay <= 0)
            {
                delay = 60 / (float)BPM;

                Position.X += (facingModified * (int)direction % 2) * -1;
                Position.Y += (facingModified * ((int)direction + 1) % 2) * -1;
            }
            animationFrame = (int)(delay /0.125) % 4;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 origin = Vector2.Zero;
            SpriteEffects effects = SpriteEffects.None;
            
            if(direction == Facing.Left)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            spriteBatch.Draw(texture, position, new Rectangle(64 * animationFrame, 0, 64, 64), Color.White, 0f, origin, 1f, effects, 0);
        }
    }
}
