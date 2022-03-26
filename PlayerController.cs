using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gaming
{
    public class PlayerController
    {
        private ContentManager Content;
        private Texture2D playerTexture;
        private Texture2D UpplayerTexture;

        private Texture2D barTexture;

        private KeyboardState currentKeyboardState;
        private KeyboardState priorKeyboardState;

        bool movingRight = false;
        bool movingDown = false;
        bool movingUp = false;
        bool movingLeft = false;
        static float speed = 500;

        bool facingLeft = false;
        bool facingUp = false;
        int lastInput;

        //these can be updated by the level to say if theres anything preventing the player from moving there
        public bool ObstacleLeft = false;
        public bool ObstacleRight = false;
        public bool ObstacleUp = false;
        public bool ObstacleDown = false;

        private static int BPM = 120;
        private float delay = 60 / (float)BPM;
        //private float delay2 = inputWindow;
        bool canMove = false;
        bool movedThisBeat = false;
        
        int animationFrame = 0;
        static float animationDelayDefault = 0.2f;
        float animationTimer = animationDelayDefault;
        
        public int Health = 3;
        Texture2D heartTexture;
        Vector2 healthBarPosition = new Vector2(0, 0);
        public bool Invincible = false;
        static float invincibleTimerDefault = 3.0f;
        float invincibleTimer = invincibleTimerDefault;
        bool red = false;
        SoundEffect HurtSound;

        public bool Attacking = false;
        public Vector2 AttackedSquare = Vector2.Zero;

        /// <summary>
        /// vector that says where he is by the pixel
        /// </summary>
        private Vector2 position;
        /// <summary>
        /// his position according to the grid
        /// </summary>
        public Vector2 Position;

        public PlayerController(ContentManager content)
        {
            Content = content;

            Position.X = 0;
            Position.Y = 0;
        }

        public void LoadContent()
        {
            playerTexture = Content.Load<Texture2D>("PlayerStanding");
            UpplayerTexture = Content.Load<Texture2D>("PlayerStandingBehind");

            heartTexture = Content.Load<Texture2D>("Heart");
            HurtSound = Content.Load<SoundEffect>("HurtSound");

            barTexture = Content.Load<Texture2D>("BarStuff/EmptySlots");
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
                //delay2 = inputWindow;
                canMove = true;
                movedThisBeat = false;
            }
            //if (canMove)
            //{
            //    delay2 -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            //    if (delay2 <= 0)
            //    {
            //        canMove = false;
            //    }
            //}
            animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (animationTimer > 60 / (float)BPM)
            {
                animationFrame++;
                if (animationFrame > 1) animationFrame = 0;
                animationTimer -= 60 / (float)BPM;
            }
            #endregion
            #region KeyboardInput
            //handle keyboard input
            if (!movedThisBeat)
            {
                Attacking = false;
                currentKeyboardState = Keyboard.GetState();
                if (currentKeyboardState.IsKeyDown(Keys.D) && priorKeyboardState.IsKeyUp(Keys.D))
                {
                    lastInput = 3;
                    facingLeft = false;
                    if (Position.X < 11 && !ObstacleRight)
                    {
                        Position.X = Position.X + 1;
                        canMove = false;
                        movingRight = true;
                        movedThisBeat = true;
                    }
                }
                if (currentKeyboardState.IsKeyDown(Keys.S) && priorKeyboardState.IsKeyUp(Keys.S))
                {
                    facingUp = false;
                    lastInput = 1;
                    if (Position.Y < 11 && !ObstacleDown)
                    {
                        Position.Y = Position.Y + 1;
                        canMove = false;
                        movingDown = true;
                        movedThisBeat = true;
                    }
                }
                if (currentKeyboardState.IsKeyDown(Keys.A) && priorKeyboardState.IsKeyUp(Keys.A))
                {
                    lastInput = 2;
                    facingLeft = true;
                    if (Position.X > 0 && !ObstacleLeft)
                    {
                        Position.X = Position.X - 1;
                        canMove = false;
                        movingLeft = true;
                        movedThisBeat = true;
                    }
                }
                if (currentKeyboardState.IsKeyDown(Keys.W) && priorKeyboardState.IsKeyUp(Keys.W))
                {
                    facingUp = true;
                    lastInput = 0;
                    if (Position.Y > 0 && !ObstacleUp)
                    {
                        Position.Y = Position.Y - 1;
                        canMove = false;
                        movingUp = true;
                        movedThisBeat = true;
                    }
                }
                if (currentKeyboardState.IsKeyDown(Keys.Space) && priorKeyboardState.IsKeyUp(Keys.Space))
                {
                    movedThisBeat = true;
                    Attacking = true;
                    if (lastInput == 0) //facing up
                        AttackedSquare.Y = Position.Y - 1;
                    else if (lastInput == 1) //facing down
                        AttackedSquare.Y = Position.Y + 1;
                    else if (lastInput == 2) //facing left
                        AttackedSquare.X = Position.X - 1;
                    else                    //facing right
                        AttackedSquare.X = Position.X + 1;
                }
                priorKeyboardState = currentKeyboardState;
            }
            #endregion
            if (Invincible)
            {
                invincibleTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (invincibleTimer <= 0)
                {
                    Invincible = false;
                    red = false;
                }
            }
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            #region DrawPlayer
            Rectangle source = new Rectangle(animationFrame * 64, 0, 64, 64);
            if (!Invincible)
            {
                if (facingLeft)
                {
                    if (!facingUp)
                        spriteBatch.Draw(playerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                    else
                        spriteBatch.Draw(UpplayerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                }
                else
                {
                    if (!facingUp)
                        spriteBatch.Draw(playerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    else
                        spriteBatch.Draw(UpplayerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }
            }
            else
            {
                if (!red)
                {
                    if (facingLeft)
                    {
                        if (!facingUp)
                            spriteBatch.Draw(playerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                        else
                            spriteBatch.Draw(UpplayerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                    }
                    else
                    {
                        if (!facingUp)
                            spriteBatch.Draw(playerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(UpplayerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                    red = true;
                }
                else
                {
                    if (facingLeft)
                    {
                        if (!facingUp)
                            spriteBatch.Draw(playerTexture, position, source, Color.Red, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                        else
                            spriteBatch.Draw(UpplayerTexture, position, source, Color.Red, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                    }
                    else
                    {
                        if (!facingUp)
                            spriteBatch.Draw(playerTexture, position, source, Color.Red, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(UpplayerTexture, position, source, Color.Red, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                    red = false;
                }
            }
            #endregion
            #region DrawHearts
            Rectangle fullHeart = new Rectangle(0, 0, 64, 64);
            Rectangle emptyHeart = new Rectangle(64, 0, 64, 64);
            if (Health == 3)
            {
                spriteBatch.Draw(heartTexture, healthBarPosition, fullHeart, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(heartTexture, healthBarPosition + new Vector2(64, 0), fullHeart, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(heartTexture, healthBarPosition + new Vector2(64 * 2, 0), fullHeart, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            if (Health == 2)
            {
                spriteBatch.Draw(heartTexture, healthBarPosition, fullHeart, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(heartTexture, healthBarPosition + new Vector2(64, 0), fullHeart, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(heartTexture, healthBarPosition + new Vector2(64 * 2, 0), emptyHeart, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            if (Health == 1)
            {
                spriteBatch.Draw(heartTexture, healthBarPosition, fullHeart, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(heartTexture, healthBarPosition + new Vector2(64, 0), emptyHeart, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(heartTexture, healthBarPosition + new Vector2(64 * 2, 0), emptyHeart, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            if (Health == 0)
            {
                spriteBatch.Draw(heartTexture, healthBarPosition, emptyHeart, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(heartTexture, healthBarPosition + new Vector2(64, 0), emptyHeart, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                spriteBatch.Draw(heartTexture, healthBarPosition + new Vector2(64 * 2, 0), emptyHeart, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            #endregion
            spriteBatch.Draw(barTexture, new Vector2(64 + 8, 856), Color.White);
        }
        public void TakeDamage()
        {
            invincibleTimer = invincibleTimerDefault;
            Invincible = true;
            Health -= 1;
            HurtSound.Play();
        }
    }
}
