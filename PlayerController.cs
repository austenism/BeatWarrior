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
        bool canMove = false;
        bool movedThisBeat = false;
        bool attackedThisBeat = false;
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

            Position.X = 6;
            Position.Y = 6;
        }

        public void LoadContent()
        {
            playerTexture = Content.Load<Texture2D>("PlayerStanding");
            UpplayerTexture = Content.Load<Texture2D>("PlayerStandingBehind");

            heartTexture = Content.Load<Texture2D>("Heart");
            HurtSound = Content.Load<SoundEffect>("HurtSound");

            barTexture = Content.Load<Texture2D>("BarStuff/EmptySlots");
            trackerTexture = Content.Load<Texture2D>("BarStuff/BeatTracker");
            movingTexture = Content.Load<Texture2D>("BarStuff/MoveSlots");
            attackedTexture = Content.Load<Texture2D>("BarStuff/AttackSlots");
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
                movedThisBeat = false;
                attackedThisBeat = false;
            }
            if (delay <= 0) //THE BEAT
            {
                delay = 60 / (float)BPM;
                canMove = true;
                
                beat = beat + 1;
                if(beat > 3) //change to fade out the previous beat by 50%, then clear it?
                {
                    beat = 0;
                    Measure[0] = Actions.None;
                    Measure[1] = Actions.None;
                    Measure[2] = Actions.None;
                    Measure[3] = Actions.None;
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
            if (!movedThisBeat && !attackedThisBeat)
            {
                Attacking = false;
                currentKeyboardState = Keyboard.GetState();
                if (currentKeyboardState.IsKeyDown(Keys.D) && priorKeyboardState.IsKeyUp(Keys.D) && !movedThisBeat)
                {
                    Measure[beat] = Actions.Move;
                    curFacing = Facing.Right;
                    if (Position.X < 11 && !ObstacleRight)
                    {
                        movedThisBeat = true;
                        canMove = false;
                        movingRight = true;
                    }
                }
                if (currentKeyboardState.IsKeyDown(Keys.S) && priorKeyboardState.IsKeyUp(Keys.S) && !movedThisBeat)
                {
                    Measure[beat] = Actions.Move;
                    curFacing = Facing.Down;
                    if (Position.Y < 11 && !ObstacleDown)
                    {
                        movedThisBeat = true;
                        canMove = false;
                        movingDown = true;
                    }
                }
                if (currentKeyboardState.IsKeyDown(Keys.A) && priorKeyboardState.IsKeyUp(Keys.A) && !movedThisBeat)
                {
                    Measure[beat] = Actions.Move;
                    curFacing = Facing.Left;
                    if (Position.X > 0 && !ObstacleLeft)
                    {
                        movedThisBeat = true;
                        canMove = false;
                        movingLeft = true;
                    }
                }
                if (currentKeyboardState.IsKeyDown(Keys.W) && priorKeyboardState.IsKeyUp(Keys.W) && !movedThisBeat)
                {
                    Measure[beat] = Actions.Move;
                    curFacing = Facing.Up;
                    if (Position.Y > 0 && !ObstacleUp)
                    {
                        movedThisBeat = true;
                        canMove = false;
                        movingUp = true;
                    }
                }
                if (currentKeyboardState.IsKeyDown(Keys.Space) && priorKeyboardState.IsKeyUp(Keys.Space) && !attackedThisBeat)
                {
                    attackedThisBeat = true;
                    movedThisBeat = false;
                    Measure[beat] = Actions.Attack;
                    Attacking = true;
                }
                if (currentKeyboardState.IsKeyDown(Keys.I) && priorKeyboardState.IsKeyUp(Keys.I) && !attackedThisBeat)
                {
                    curFacing = Facing.Up;
                    attackedThisBeat = true;
                    movedThisBeat = false;
                    Measure[beat] = Actions.Attack;
                    Attacking = true;
                }
                if (currentKeyboardState.IsKeyDown(Keys.J) && priorKeyboardState.IsKeyUp(Keys.J) && !attackedThisBeat)
                {
                    curFacing = Facing.Left;
                    attackedThisBeat = true;
                    movedThisBeat = false;
                    Measure[beat] = Actions.Attack;
                    Attacking = true;
                }
                if (currentKeyboardState.IsKeyDown(Keys.K) && priorKeyboardState.IsKeyUp(Keys.K) && !attackedThisBeat)
                {
                    curFacing = Facing.Down;
                    attackedThisBeat = true;
                    movedThisBeat = false;
                    Measure[beat] = Actions.Attack;
                    Attacking = true;
                }
                if (currentKeyboardState.IsKeyDown(Keys.L) && priorKeyboardState.IsKeyUp(Keys.L) && !attackedThisBeat)
                {
                    curFacing = Facing.Right;
                    attackedThisBeat = true;
                    movedThisBeat = false;
                    Measure[beat] = Actions.Attack;
                    Attacking = true;
                }

                int facingModified = (((((int)curFacing + 1) / 2) % 2) * 2) - 1;

                int deltaX = (facingModified * (int)curFacing % 2) * -1;
                int deltaY = (facingModified * ((int)curFacing + 1) % 2) * -1;
                if (movedThisBeat || attackedThisBeat)
                {
                    switch (Measure[beat])
                    {
                        case Actions.None:
                            //take stamia
                            break;
                        case Actions.Move:
                            Position.X += deltaX;
                            Position.Y += deltaY;
                            break;
                        case Actions.Attack:
                            AttackedSquare = Position;
                            AttackedSquare.X += deltaX;
                            AttackedSquare.Y += deltaY;
                            break;
                        default:
                            //also take stamina bc "why"
                            break;
                    }
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
