using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gaming.Enemies
{
    public class Soup
    {
        private ContentManager Content;
        Texture2D texture;
        Texture2D Uptexture;

        bool movingRight = false;
        bool movingDown = false;
        bool movingUp = false;
        bool movingLeft = false;
        static float speed = 500;

        bool facingLeft = false;
        bool facingUp = false;
        //these can be updated by the level to say if theres anything preventing the enemy from moving there
        public bool ObstacleLeft = false;
        public bool ObstacleRight = false;
        public bool ObstacleUp = false;
        public bool ObstacleDown = false;

        private static int BPM = 120;
        private static float inputWindow = 0.5f;
        private float delay = 60 / (float)BPM;
        private float delay2 = inputWindow;
        bool canMove = false;
        bool movedThisBeat = false;

        int animationFrame = 0;
        static float animationDelayDefault = 0.2f;
        float animationTimer = animationDelayDefault;

        Random random;
        public bool Dead = false;

        /// <summary>
        /// vector that says where he is by the pixel
        /// </summary>
        private Vector2 position;
        /// <summary>
        /// his position according to the grid
        /// </summary>
        public Vector2 Position;
        public Soup(ContentManager content, int x, int y)
        {
            Content = content;

            Position.X = x;
            Position.Y = y;

            random = new Random(System.DateTime.Now.Millisecond);
        }
        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("Enemies/Soup");
            Uptexture = content.Load<Texture2D>("Enemies/SoupBehind");
        }
        public void Update(GameTime gameTime)
        {
            #region PositionCalculation
            //update positions relative to grid and stuff
            if (!movingRight && !movingLeft && !movingDown && !movingUp)
            {
                position.X = (Position.X * 64) + Constants.BORDERSIZE;
                position.Y = (Position.Y * 64) + Constants.BORDERSIZE;
            }
            //handles smooth movement between tiles
            else
            {
                if (movingRight)
                {
                    position.X += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (position.X >= (Position.X * 64) + Constants.BORDERSIZE)
                        movingRight = false;
                }
                if (movingLeft)
                {
                    position.X -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (position.X <= (Position.X * 64) + Constants.BORDERSIZE)
                        movingLeft = false;
                }
                if (movingDown)
                {
                    position.Y += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (position.Y >= (Position.Y * 64) + Constants.BORDERSIZE)
                        movingDown = false;
                }
                if (movingUp)
                {
                    position.Y -= speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (position.Y <= (Position.Y * 64) + Constants.BORDERSIZE)
                        movingUp = false;
                }
            }
            #endregion
            #region BPMMath
            //do BPM math
            delay -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (delay <= 0)
            {
                delay = 60 / (float)BPM;
                delay2 = inputWindow;
                canMove = true;
                movedThisBeat = false;
            }
            if (canMove)
            {
                delay2 -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (delay2 <= 0)
                {
                    canMove = false;
                }
            }
            animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > 60 / (float)BPM)
            {
                animationFrame++;
                if (animationFrame > 3) animationFrame = 0;
                animationTimer -= 60 / (float)BPM;
            }
            #endregion
            #region Movement
            //handle keyboard input
            if (canMove && !movedThisBeat)
            {
                random = new Random((int)DateTime.Now.Ticks);
                int direction = random.Next(0, 4);
                if (direction == 0)
                {
                    facingLeft = false;
                    facingUp = false;
                    if (Position.X < 11 && !ObstacleRight)
                    {
                        Position.X = Position.X + 1;
                        movingRight = true;
                        movedThisBeat = true;
                    }
                }
                if (direction == 1)
                {
                    facingUp = false;
                    if (Position.Y < 11 && !ObstacleDown)
                    {
                        Position.Y = Position.Y + 1;
                        movingDown = true;
                        movedThisBeat = true;
                    }
                }
                if (direction == 2)
                {
                    facingLeft = true;
                    facingUp = false;
                    if (Position.X > 0 && !ObstacleLeft)
                    {
                        Position.X = Position.X - 1;
                        movingLeft = true;
                        movedThisBeat = true;
                    }
                }
                if (direction == 3)
                {
                    facingUp = true;
                    if (Position.Y > 0 && !ObstacleUp)
                    {
                        Position.Y = Position.Y - 1;
                        movingUp = true;
                        movedThisBeat = true;
                    }
                }
            }
            #endregion
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle source = new Rectangle(animationFrame * 64, 0, 64, 64);
            if (facingLeft)
            {
                if (!facingUp)
                    spriteBatch.Draw(texture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                else
                    spriteBatch.Draw(Uptexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
            }
            else
            {
                if (!facingUp)
                    spriteBatch.Draw(texture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                else
                    spriteBatch.Draw(Uptexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
        }
    }
}
