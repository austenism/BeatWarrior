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
        public override void LoadSpecific()
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

            //im at soup
            soups.Add(new Soup(Content, 9, 3));
            soups.Add(new Soup(Content, 6, 9));
            soups.Add(new Soup(Content, 3, 10));
            soups.Add(new Soup(Content, 11, 8));

            spikes.Add(new Spike(Content, 5, 3));
            spikes.Add(new Spike(Content, 0, 5));
            spikes.Add(new Spike(Content, 2, 5));
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
