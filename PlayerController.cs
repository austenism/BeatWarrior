using Gaming.GameLoops;
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
        Attack,
        Laser,
        Brick
    }

    public enum Facing
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
        private Texture2D Key;

        private Texture2D barTexture;
        private Texture2D movingTexture;
        private Texture2D attackedTexture;
        private Texture2D reticleTexture;
        private Texture2D swingingTexture;
        private Texture2D brickTexture;
        private Texture2D laserTexture;
        private Texture2D timeMarkerTexture;

        int deltaX = 0;
        int deltaY = 0;

        private KeyboardState currentKeyboardState;
        private KeyboardState priorKeyboardState;

        //public delegate void createLaser(int facing, Vector2 pos);

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
        int beat = 0;
        Actions[] Measure = new Actions[8];
        
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
        public bool throwing = false;
        public bool casting = false;
        public int laserstartind = -1;
        public int brickstartind = -1;
        public float swingingTimer;
        public int swingingFrame;

        public bool Dead = false;

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
            Key = Content.Load<Texture2D>("Keys");

            heartTexture = Content.Load<Texture2D>("Heart");
            HurtSound = Content.Load<SoundEffect>("HurtSound");

            barTexture = Content.Load<Texture2D>("BarStuff/EmptySlots");
            movingTexture = Content.Load<Texture2D>("BarStuff/MoveSlots");
            attackedTexture = Content.Load<Texture2D>("BarStuff/AttackSlots");
            brickTexture = Content.Load<Texture2D>("BarStuff/BrickSlots");
            laserTexture = Content.Load<Texture2D>("BarStuff/LazerSlot");
            timeMarkerTexture = Content.Load<Texture2D>("BarStuff/TimeMarker");
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
                    case Actions.Brick:
                        int brickdir = (int)curFacing;
                        Vector2 brickspawn = new Vector2(Position.X + deltaX, Position.Y + deltaY);
                        throwing = true;
                        break;
                    case Actions.Laser:
                        int laserdir = (int)curFacing;
                        Vector2 laserspawn = new Vector2(Position.X + deltaX, Position.Y + deltaY);
                        casting = true;
                        break;
                    default:
                        //also take stamina bc "why" or more accurately, HOW
                        break;
                }
                delay = 60 / (float)BPM;

                beat = (beat + 1) % 8;
                Measure[(beat + 5) % 8] = Actions.None;

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
            int effectivebeat = beat;
            if (laserstartind != -1 && laserstartind != beat || brickstartind != -1 && brickstartind != beat)
            {
                if (Measure[beat] == Actions.Laser)
                {
                    effectivebeat = (laserstartind + 4) % 8;
                }
                if (Measure[beat] == Actions.Brick)
                {
                    effectivebeat = (brickstartind + 2) % 8;
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.D) && priorKeyboardState.IsKeyUp(Keys.D))
            {
                Measure[effectivebeat] = Actions.Move;
                curFacing = Facing.Right;
            }
            if (currentKeyboardState.IsKeyDown(Keys.S) && priorKeyboardState.IsKeyUp(Keys.S))
            {
                Measure[effectivebeat] = Actions.Move;
                curFacing = Facing.Down;
            }
            if (currentKeyboardState.IsKeyDown(Keys.A) && priorKeyboardState.IsKeyUp(Keys.A))
            {
                Measure[effectivebeat] = Actions.Move;
                curFacing = Facing.Left;
            }
            if (currentKeyboardState.IsKeyDown(Keys.W) && priorKeyboardState.IsKeyUp(Keys.W))
            {
                Measure[effectivebeat] = Actions.Move;
                curFacing = Facing.Up;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Space) && priorKeyboardState.IsKeyUp(Keys.Space))
            {
                Measure[effectivebeat] = Actions.Attack;
            }
            if (currentKeyboardState.IsKeyDown(Keys.I) && priorKeyboardState.IsKeyUp(Keys.I))
            {
                curFacing = Facing.Up;
                Measure[effectivebeat] = Actions.Attack;
            }
            if (currentKeyboardState.IsKeyDown(Keys.J) && priorKeyboardState.IsKeyUp(Keys.J))
            {
                curFacing = Facing.Left;
                Measure[effectivebeat] = Actions.Attack;
            }
            if (currentKeyboardState.IsKeyDown(Keys.K) && priorKeyboardState.IsKeyUp(Keys.K))
            {
                curFacing = Facing.Down;
                Measure[effectivebeat] = Actions.Attack;
            }
            if (currentKeyboardState.IsKeyDown(Keys.L) && priorKeyboardState.IsKeyUp(Keys.L))
            {
                curFacing = Facing.Right;
                Measure[effectivebeat] = Actions.Attack;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Q) && priorKeyboardState.IsKeyUp(Keys.Q))
            {
                Measure[effectivebeat] = Actions.Brick;
                if (effectivebeat != brickstartind + 2)
                    brickstartind = effectivebeat;
            }
            if (currentKeyboardState.IsKeyDown(Keys.E) && priorKeyboardState.IsKeyUp(Keys.E))
            {
                Measure[effectivebeat] = Actions.Laser;
                if (effectivebeat != laserstartind + 4)
                    laserstartind = effectivebeat;
            }
            if (currentKeyboardState.IsKeyDown(Keys.U) && priorKeyboardState.IsKeyUp(Keys.U))
            {
                Measure[effectivebeat] = Actions.Brick;
                if (effectivebeat == brickstartind + 2)
                    brickstartind = effectivebeat;
            }
            if (currentKeyboardState.IsKeyDown(Keys.O) && priorKeyboardState.IsKeyUp(Keys.O))
            {
                Measure[effectivebeat] = Actions.Laser;
                if (effectivebeat == laserstartind + 4)
                    laserstartind = effectivebeat;
            }

            if (Measure[effectivebeat] == Actions.Laser)
            {
                for (int i = 1; i < 4; i++)
                {
                    Measure[(effectivebeat + i) % 8] = Actions.Laser;
                }
            }
            else if (Measure[effectivebeat] == Actions.Brick)
            {
                Measure[(effectivebeat + 1) % 8] = Actions.Brick;
                Measure[(effectivebeat + 2) % 8] = Actions.None;
                Measure[(effectivebeat + 3) % 8] = Actions.None;
            }
            else
            {
                for (int i = 1; i < 4; i++)
                {
                    Measure[(effectivebeat + i) % 8] = Actions.None;
                }
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
            
            Color color = Color.White;
            Rectangle animsource = source;
            SpriteEffects effects = SpriteEffects.None;
            Texture2D texture = playerTexture;

            if (swinging)
            {
                texture = swingingTexture;
                animsource = swingingSource;
            }
            else if (throwing)
            {
                //texture = null;//replace when have
            }
            else if(curFacing == Facing.Up)
            {
                texture = UpplayerTexture;
                if (casting)
                {
                    //texture = null;//insert upwards laser
                }
            }
            else if (casting)
            {
                //texture = null;//insert proper texture later
            }

            if(curFacing == Facing.Left)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            if (Invincible)
            {
                if (red)
                {
                    color = Color.Red;
                    red = false;
                }
                else
                {
                    red = true;
                }
            }

            spriteBatch.Draw(texture, position, animsource, color, 0, Vector2.Zero, 1f, effects, 0);
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
            spriteBatch.Draw(barTexture, new Vector2(64 + 8 + 256, 856), Color.White);

            spriteBatch.Draw(Key, new Vector2(670, 0), Color.White);

            //spriteBatch.Draw(playerTexture, position, source, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
            for (int ind = 0; ind < 8; ind++)
            {
                int offset = 0;
                Texture2D Placeholder = null;
                Color transparencyFilter = Color.White;
                switch (Measure[ind])
                {
                    case Actions.Move:
                        Placeholder = movingTexture;
                        break;
                    case Actions.Attack:
                        Placeholder = attackedTexture;
                        break;
                    case Actions.Brick:
                        Placeholder = brickTexture;
                        if ((brickstartind % 4) % 2 != 0)
                        {
                            offset = 1;
                        }
                        break;
                    case Actions.Laser:
                        Placeholder = laserTexture;
                        offset = laserstartind % 4;
                        break;
                }
                if (ind == (beat + 6) % 8)
                    transparencyFilter = Color.White * 0.5f;

                if (Placeholder != null)
                    spriteBatch.Draw(Placeholder, new Vector2(64 + 8 + (64 * ind), 856), new Rectangle(offset * 256 + 64 * (ind % 4), 0, 64, 64), transparencyFilter, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            
            spriteBatch.Draw(timeMarkerTexture, new Vector2(32 + 8 + (64 * beat), 856), Color.White);
            if(beat == 0)
            {
                spriteBatch.Draw(timeMarkerTexture, new Vector2(32 + 6 + (8) + (64 * 8), 856), Color.White);
            }
        }
        public void TakeDamage()
        {
            invincibleTimer = invincibleTimerDefault;
            Invincible = true;
            Health -= 1;
            HurtSound.Play();
            if (Health < 1)
                Dead = true;
        }
    }
}
