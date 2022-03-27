using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gaming
{
    enum Actions
    {
        None,
        Move,
        Attack
    }

    enum Facing
    {
        Down,
        Left,
        Up,
        Right
    }
    public class PlayerController
    {
        private ContentManager Content;
        private Texture2D playerTexture;
        private Texture2D UpplayerTexture;

        private Texture2D barTexture;
        private Texture2D trackerTexture;
        private Texture2D movingTexture;
        private Texture2D attackedTexture;
        private Texture2D reticleTexture;
        private Texture2D swingingTexture;

        int deltaX = 0;
        int deltaY = 0;

        private KeyboardState currentKeyboardState;
        private KeyboardState priorKeyboardState;

        bool movingRight = false;
        bool movingDown = false;
        bool movingUp = false;
        bool movingLeft = false;
        static float speed = 500;

        Facing curFacing = 0;

        //these can be updated by the level to say if theres anything preventing the player from moving there
        public bool ObstacleLeft = false;
        public bool ObstacleRight = false;
        public bool ObstacleUp = false;
        public bool ObstacleDown = false;

        private static int BPM = 120;
        private float delay = 60 / (float)BPM;
        //private float delay2 = inputWindow;
        //bool canMove = false;
        //bool movedThisBeat = false;
        //bool attackedThisBeat = false;
        int beat = 0;
        Actions[] Measure = new Actions[4];
        
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
        public bool swinging = false;
        public float swingingTimer;
        public int swingingFrame;

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
            swingingTexture = Content.Load<Texture2D>("SwordAttack");

            heartTexture = Content.Load<Texture2D>("Heart");
            HurtSound = Content.Load<SoundEffect>("HurtSound");

            barTexture = Content.Load<Texture2D>("BarStuff/EmptySlots");
            trackerTexture = Content.Load<Texture2D>("BarStuff/BeatTracker");
            movingTexture = Content.Load<Texture2D>("BarStuff/MoveSlots");
            attackedTexture = Content.Load<Texture2D>("BarStuff/AttackSlots");
            reticleTexture = Content.Load<Texture2D>("crosshair");
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

            if (delay <= 0.01f)
            {
                //movedThisBeat = false;
                //attackedThisBeat = false;
            }
            if (delay <= 0) //THE BEAT
            {
                Attacking = false;
                switch (Measure[beat])
                {
                    case Actions.None:
                        //take stamina
                        break;
                    case Actions.Move:
                        Position.X += deltaX;
                        Position.Y += deltaY;

                        if (MathHelper.Min(Position.X, Position.Y) < 0 ||
                            MathHelper.Max(Position.X, Position.Y) > 11 ||
                            ObstacleLeft && deltaX == -1 ||
                            ObstacleRight && deltaX == 1 ||
                            ObstacleUp && deltaY == -1 ||
                            ObstacleDown && deltaY == 1)
                        {
                            Position.X -= deltaX;
                            Position.Y -= deltaY;
                        }
                        else
                        {
                            //movedThisBeat = true;
                            //canMove = false;
                            //movingUp = true;
                            movingUp = deltaY == -1;
                            movingDown = deltaY == 1;
                            movingRight = deltaX == 1;
                            movingLeft = deltaX == -1;
                        }
                        break;
                    case Actions.Attack:
                        AttackedSquare = Position;
                        AttackedSquare.X += deltaX;
                        AttackedSquare.Y += deltaY;
                        swinging = true;
                        swingingTimer = 0.5f;
                        Attacking = true;
                        break;
                    default:
                        //also take stamina bc "why"
                        break;
                }
                delay = 60 / (float)BPM;
                //canMove = true;
                //actionTaken = false;

                beat = beat + 1;
                switch (beat)
                {
                    case 1:
                        Measure[2] = Actions.None;
                        break;
                    case 2:
                        Measure[3] = Actions.None;
                        break;
                    case 3:
                        Measure[0] = Actions.None;
                        break;
                    case 4:
                        Measure[1] = Actions.None;
                        beat = 0;
                        break;

                }
            }

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
            currentKeyboardState = Keyboard.GetState();
            if (currentKeyboardState.IsKeyDown(Keys.D) && priorKeyboardState.IsKeyUp(Keys.D))
            {
                Measure[beat] = Actions.Move;
                curFacing = Facing.Right;
            }
            if (currentKeyboardState.IsKeyDown(Keys.S) && priorKeyboardState.IsKeyUp(Keys.S))
            {
                Measure[beat] = Actions.Move;
                curFacing = Facing.Down;
            }
            if (currentKeyboardState.IsKeyDown(Keys.A) && priorKeyboardState.IsKeyUp(Keys.A))
            {
                Measure[beat] = Actions.Move;
                curFacing = Facing.Left;
            }
            if (currentKeyboardState.IsKeyDown(Keys.W) && priorKeyboardState.IsKeyUp(Keys.W))
            {
                Measure[beat] = Actions.Move;
                curFacing = Facing.Up;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Space) && priorKeyboardState.IsKeyUp(Keys.Space))
            {
                Measure[beat] = Actions.Attack;
            }
            if (currentKeyboardState.IsKeyDown(Keys.I) && priorKeyboardState.IsKeyUp(Keys.I))
            {
                curFacing = Facing.Up;
                Measure[beat] = Actions.Attack;
            }
            if (currentKeyboardState.IsKeyDown(Keys.J) && priorKeyboardState.IsKeyUp(Keys.J))
            {
                curFacing = Facing.Left;
                Measure[beat] = Actions.Attack;
            }
            if (currentKeyboardState.IsKeyDown(Keys.K) && priorKeyboardState.IsKeyUp(Keys.K))
            {
                curFacing = Facing.Down;
                Measure[beat] = Actions.Attack;
            }
            if (currentKeyboardState.IsKeyDown(Keys.L) && priorKeyboardState.IsKeyUp(Keys.L))
            {
                curFacing = Facing.Right;
                Measure[beat] = Actions.Attack;
            }
            int facingModified = (((((int)curFacing + 1) / 2) % 2) * 2) - 1;

            deltaX = (facingModified * (int)curFacing % 2) * -1;
            deltaY = (facingModified * ((int)curFacing + 1) % 2) * -1;


            priorKeyboardState = currentKeyboardState;

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
            #region swingingmath
            if (swinging)
            {
                swingingTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (swingingTimer < 0)
                {
                    swinging = false;
                }
                else if (swingingTimer <= 0.125f)
                {
                    swingingFrame = 3;
                }
                else if (swingingTimer <= 0.25f)
                {
                    swingingFrame = 2;
                }
                else if (swingingTimer <= 0.375)
                {
                    swingingFrame = 1;
                }
                else
                {
                    swingingFrame = 0;
                }
            }
            #endregion
            #region DrawPlayer
            Rectangle source = new Rectangle(animationFrame * 64, 0, 64, 64);
            Rectangle swingingSource = new Rectangle(swingingFrame * 64, 0, 64, 64);
            if (!Invincible)
            {
                if (!swinging)
                {
                    if (curFacing == Facing.Left)
                    {
                        if (curFacing != Facing.Up)
                            spriteBatch.Draw(playerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                        else
                            spriteBatch.Draw(UpplayerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                    }
                    else
                    {
                        if (curFacing != Facing.Up)
                            spriteBatch.Draw(playerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(UpplayerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                }
                else if(swinging)
                {
                    if (curFacing == Facing.Left)
                    {
                        if (curFacing != Facing.Up)
                            spriteBatch.Draw(swingingTexture, position, swingingSource, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                        else
                            spriteBatch.Draw(swingingTexture, position, swingingSource, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                    }
                    else
                    {
                        if (curFacing != Facing.Up)
                            spriteBatch.Draw(swingingTexture, position, swingingSource, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(swingingTexture, position, swingingSource, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                }
            }
            else
            {
                if (!red)
                {
                    if (curFacing == Facing.Left)
                    {
                        if (curFacing != Facing.Up)
                            spriteBatch.Draw(playerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                        else
                            spriteBatch.Draw(UpplayerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                    }
                    else
                    {
                        if (curFacing != Facing.Up)
                            spriteBatch.Draw(playerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(UpplayerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                    red = true;
                }
                else
                {
                    if (curFacing == Facing.Left)
                    {
                        if (curFacing != Facing.Up)
                            spriteBatch.Draw(playerTexture, position, source, Color.Red, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                        else
                            spriteBatch.Draw(UpplayerTexture, position, source, Color.Red, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                    }
                    else
                    {
                        if (curFacing != Facing.Up)
                            spriteBatch.Draw(playerTexture, position, source, Color.Red, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(UpplayerTexture, position, source, Color.Red, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                    red = false;
                }
            }
            spriteBatch.Draw(reticleTexture, new Vector2((((deltaX + Position.X) * 64) + Constants.BORDERSIZE), (((deltaY + Position.Y) * 64) + Constants.BORDERSIZE)),
                null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

            
            #endregion
            #region DrawHearts
            Rectangle fullHeart = new Rectangle(0, 0, 64, 64);
            Rectangle emptyHeart = new Rectangle(64, 0, 64, 64);
            for (int i = 0; i < 3; i++)
            {
                if(i + 1 <= Health)
                {
                    spriteBatch.Draw(heartTexture, healthBarPosition + new Vector2(64 * i, 0), fullHeart, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(heartTexture, healthBarPosition + new Vector2(64 * i, 0), emptyHeart, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                }
            }
            
            #endregion
            spriteBatch.Draw(barTexture, new Vector2(64 + 8, 856), Color.White);



            //spriteBatch.Draw(playerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
            for(int ind = 0; ind < 4; ind++)
            {
                Texture2D Placeholder = null;
                switch (Measure[ind])
                {
                    case Actions.Move:
                        Placeholder = movingTexture;
                        break;
                    case Actions.Attack:
                        Placeholder = attackedTexture;
                        break;
                }
                if (Placeholder != null)
                spriteBatch.Draw(Placeholder, new Vector2(64 + 8 + (64 * ind), 856), new Rectangle(64 * ind, 0, 64, 64), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            
            spriteBatch.Draw(trackerTexture, new Vector2(64 + 8 + (64 * beat), 852), Color.White * 0.2f);
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
