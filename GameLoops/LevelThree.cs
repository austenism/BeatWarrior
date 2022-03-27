using Gaming.Enemies;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System.Collections.Generic;
using System.Text;

namespace Gaming.GameLoops
{
    public class LevelThree : LevelLoop
    {
        List<Soup> soupsToRemove = new List<Soup>();
        List<Spike> spikes = new List<Spike>();
        public override void LoadContent()
        {
            backgroundMusic = Content.Load<SoundEffect>("LevelThreeContent/moosic");
            SongLength = 8;

            _tiledMap = Content.Load<TiledMap>("LevelThreeContent/levelThree");
            _tiledMapRenderer = new TiledMapRenderer(graphicsDevice, _tiledMap);


            #region fixArray
            Obstacles = new int[12, 12];
            int[,] temp = new int[12, 12] {
                {0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0},
                {0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 0, 0},
                {0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 1, 0},
                {0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0},
                {0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0},
                {1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0},
                {0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
                {0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0}
            };
            for (int r = 0; r < 12; ++r)
            {
                for (int c = 0; c < 12; ++c)
                {
                    Obstacles[r, c] = temp[c, r];
                }
            }
            #endregion

            player = new PlayerController(Content);
            player.LoadContent();
            
            //im at soup
            soups.Add(new Soup(Content, 9, 3));
            soups.Add(new Soup(Content, 6, 9));
            soups.Add(new Soup(Content, 3, 10));
            soups.Add(new Soup(Content, 11, 8));

            spikes.Add(new Spike(Content, 5, 3));
            spikes.Add(new Spike(Content, 0, 5));
            spikes.Add(new Spike(Content, 2, 5));
            foreach (Soup s in soups)
            {
                s.LoadContent(Content);
            }
            foreach (Spike s in spikes)
            {
                s.LoadContent(Content);
            }
        }
        public override bool Update(GameTime gameTime)
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
            #region CheckCollisions
            foreach (Soup s in soups)
            {
                if (player.Attacking && (s.PrevPos == player.AttackedSquare || s.Position == player.AttackedSquare))
                {
                    s.Dead = true;
                    soupsToRemove.Add(s);
                }
                if (s.Position == player.Position && !player.Invincible && !s.Dead)
                {
                    player.TakeDamage();
                }
            }
            foreach (Spike s in spikes)
            {
                if (s.Position == player.Position && !player.Invincible && s.Hurty)
                {
                    player.TakeDamage();
                }
            }
            foreach (Soup s in soupsToRemove)
            {
                soups.Remove(s);
            }
            #endregion

            if (soups.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override bool Draw(GameTime gameTime, SpriteBatch spriteBatch)
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
            foreach (Spike s in spikes)
            {
                s.Draw(gameTime, spriteBatch);
            }
            spriteBatch.End();

            return false;
        }
    }
}
