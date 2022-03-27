using Gaming.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;
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
        public List<Soup> soupsToRemove = new List<Soup>();
        public List<Laser> LasersToRemove = new List<Laser>();
        public List<Laser> Lasers = new List<Laser>();
        public List<Brick> Bricks = new List<Brick>();
        public List<Brick> BricksToRemove = new List<Brick>();

        public List<Spike> spikes = new List<Spike>();

        public void lasercreator(int direction, Vector2 start, GameTime gametime)
        {
            int facingModified = ((((direction + 1) / 2) % 2) * 2) - 1;

            int deltaX = (facingModified * direction % 2) * -1;
            int deltaY = (facingModified * (direction + 1) % 2) * -1;
            if (direction % 2 == 0)
            {
                for (int i = (int)start.Y; i >= 0 && i < 12; i += deltaY)
                {

                    Lasers.Add(new Laser((Facing)direction, start, gametime, Content));
                    start.Y += deltaY;
                }
            }
            else
            {
                for (int i = (int)start.X; i >= 0 && i < 12; i += deltaX)
                {

                    Lasers.Add(new Laser((Facing)direction, start, gametime, Content));
                    start.X += deltaX;
                }
            }
        }
        public void brickCreator(int direction, Vector2 start, GameTime gametime)
        {
            int facingModified = ((((direction + 1) / 2) % 2) * 2) - 1;

            int deltaX = (facingModified * direction % 2) * -1;
            int deltaY = (facingModified * (direction + 1) % 2) * -1;

            Bricks.Add(new Brick((Facing)direction, start, gametime, Content));
            
        }

        public virtual void Initialize(ContentManager content, GraphicsDevice gd)
        {
            graphicsDevice = gd;
            Content = content;
            soups = new List<Soup>();
        }
        public virtual void LoadContent()
        {
            LoadSpecific();
            player = new PlayerController(Content);
            player.LoadContent();
            player.createLaser = lasercreator;
            player.createBrick = brickCreator;

            foreach (Soup s in soups)
            {
                s.LoadContent(Content);
            }
            foreach (Spike s in spikes)
            {
                s.LoadContent(Content);
            }
        }

        public abstract void LoadSpecific();
        public virtual bool Update(GameTime gameTime)
        {
            SongTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (SongTimer <= 0)
            {
                SongTimer = SongLength;
                backgroundMusic.Play();
            }

            _tiledMapRenderer.Update(gameTime);

            #region PlayerObstacleMath
            //handles obstacle movement for the player
            if (player.Position.X < 11)
            {
                if (Obstacles[(int)player.Position.X + 1, (int)player.Position.Y] == 1)
                {
                    player.ObstacleRight = true;
                }
                else
                {
                    player.ObstacleRight = false;
                }
            }
            if (player.Position.X > 0)
            {
                if (Obstacles[(int)player.Position.X - 1, (int)player.Position.Y] == 1)
                {
                    player.ObstacleLeft = true;
                }
                else
                {
                    player.ObstacleLeft = false;
                }
            }
            if (player.Position.Y < 11)
            {
                if (Obstacles[(int)player.Position.X, (int)player.Position.Y + 1] == 1)
                {
                    player.ObstacleDown = true;
                }
                else
                {
                    player.ObstacleDown = false;
                }
            }
            if (player.Position.Y > 0)
            {
                if (Obstacles[(int)player.Position.X, (int)player.Position.Y - 1] == 1)
                {
                    player.ObstacleUp = true;
                }
                else
                {
                    player.ObstacleUp = false;
                }
            }
            #endregion

            player.Update(gameTime);
            #region SoupObstacles
            foreach (Soup s in soups)
            {
                if (s.Position.X < 11)
                {
                    if (Obstacles[(int)s.Position.X + 1, (int)s.Position.Y] == 1 || Obstacles[(int)s.Position.X + 1, (int)s.Position.Y] == 2)
                    {
                        s.ObstacleRight = true;
                    }
                    else
                    {
                        s.ObstacleRight = false;
                    }
                }
                if (s.Position.X > 0)
                {
                    if (Obstacles[(int)s.Position.X - 1, (int)s.Position.Y] == 1 || Obstacles[(int)s.Position.X - 1, (int)s.Position.Y] == 2)
                    {
                        s.ObstacleLeft = true;
                    }
                    else
                    {
                        s.ObstacleLeft = false;
                    }
                }
                if (s.Position.Y < 11)
                {
                    if (Obstacles[(int)s.Position.X, (int)s.Position.Y + 1] == 1 || Obstacles[(int)s.Position.X, (int)s.Position.Y + 1] == 2)
                    {
                        s.ObstacleDown = true;
                    }
                    else
                    {
                        s.ObstacleDown = false;
                    }
                }
                if (s.Position.Y > 0)
                {
                    if (Obstacles[(int)s.Position.X, (int)s.Position.Y - 1] == 1 || Obstacles[(int)s.Position.X, (int)s.Position.Y - 1] == 2)
                    {
                        s.ObstacleUp = true;
                    }
                    else
                    {
                        s.ObstacleUp = false;
                    }
                }
                Obstacles[(int)s.Position.X, (int)s.Position.Y] = 0;
                s.Update(gameTime);
                Obstacles[(int)s.Position.X, (int)s.Position.Y] = 2;
            }
            #endregion
            foreach (Spike s in spikes)
            {
                s.Update(gameTime);
            }
            foreach(Brick b in Bricks)
            {
                b.Update(gameTime);
            }
            #region CheckCollisions
            foreach (Soup s in soups)
            {
                if (player.Attacking && (s.PrevPos == player.AttackedSquare || s.Position == player.AttackedSquare))
                {
                    s.Dead = true;
                    soupsToRemove.Add(s);
                }
                foreach (Laser l in Lasers)
                {
                    if ((s.Position == l.Position || s.PrevPos == l.Position) && l.Hurty)
                    {
                        s.Dead = true;
                        if (!soupsToRemove.Contains(s))
                            soupsToRemove.Add(s);
                    }
                }
                foreach (Brick l in Bricks)
                {
                    if ((s.Position == l.Position || s.PrevPos == l.Position) && l.Hurty)
                    {
                        s.Dead = true;
                        if (!soupsToRemove.Contains(s))
                            soupsToRemove.Add(s);
                    }
                }
                if (s.Position == player.Position && !player.Invincible && !s.Dead)
                {
                    player.TakeDamage();
                }
            }
            foreach (Laser l in Lasers)
            {
                if (l.animationFrame > 3)
                {
                    LasersToRemove.Add(l);
                }
            }
            foreach (Brick b in Bricks)
            {
                if(b.Position.X > 12 || b.Position.X < 0 || b.Position.Y > 12 || b.Position.Y < 0)
                {
                    BricksToRemove.Add(b);
                }
            }
            foreach (Soup s in soupsToRemove)
            {
                soups.Remove(s);
            }
            foreach (Laser l in LasersToRemove)
            {
                Lasers.Remove(l);
            }
            foreach(Brick b in BricksToRemove)
            {
                Bricks.Remove(b);
            }
            #endregion

            foreach (Laser l in Lasers)
            {
                l.Update(gameTime);
            }

            if (soups.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (player.Dead)
            {
                return true;
            }
            _tiledMapRenderer.Draw(viewMatrix: Matrix.CreateTranslation((float)Constants.BORDERSIZE, (float)Constants.BORDERSIZE, 0));
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            player.Draw(gameTime, spriteBatch);

            foreach (Soup s in soups)
            {
                s.Draw(gameTime, spriteBatch);
            }

            foreach (Laser l in Lasers)
            {
                l.Draw(gameTime, spriteBatch);
            }

            foreach (Brick b in Bricks)
            {
                b.Draw(gameTime, spriteBatch);
            }

            foreach (Spike s in spikes)
            {
                s.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();

            return false;
        }
    }
}
