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
    public class LevelTwoLoop : LevelLoop
    {
        public override void LoadSpecific()
        {
            backgroundMusic = Content.Load<SoundEffect>("LevelTwoContent/ootsidemoosic");
            SongLength = 8;

            _tiledMap = Content.Load<TiledMap>("LevelTwoContent/levelTwo");
            _tiledMapRenderer = new TiledMapRenderer(graphicsDevice, _tiledMap);


            #region fixArray
            Obstacles = new int[12, 12];
            int[,] temp = new int[12, 12] {
                {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1},
                {0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},
                {0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0},
                {0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0},
                {0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1}
            };
            for (int r = 0; r < 12; ++r)
            {
                for (int c = 0; c < 12; ++c)
                {
                    Obstacles[r, c] = temp[c, r];
                }
            }
            #endregion

            soups.Add(new Enemies.Soup(Content, 2, 6));
            soups.Add(new Enemies.Soup(Content, 8, 10));
            soups.Add(new Soup(Content, 8, 4));
            soups.Add(new Soup(Content, 10, 2));
            soups.Add(new Soup(Content, 8, 0));
            soups.Add(new Soup(Content, 7, 10));
            soups.Add(new Soup(Content, 3, 11));
            soups.Add(new Soup(Content, 1, 10));
        }
    }
}
