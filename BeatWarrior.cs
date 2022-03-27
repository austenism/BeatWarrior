using Gaming.GameLoops;
using Gaming.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gaming
{
    public class BeatWarrior : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        
        bool levelOne = false;
        bool levelTwo = false;
        bool levelThree = false;
        bool levelMenu = true;

        MenuLoop menu;
        LevelLoop levelOneLoop;
        LevelLoop levelTwoLoop;
        LevelLoop levelThreeLoop;

        public BeatWarrior()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = Constants.GAMEWIDTH;
            _graphics.PreferredBackBufferHeight = Constants.GAMEHEIGHT;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ReloadLevels();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (levelMenu)
            {
                levelMenu = !menu.Update(gameTime);
                levelOne = !levelMenu;
            }
            if (levelOne)
            {
                levelOne = !levelOneLoop.Update(gameTime);
                levelTwo = !levelOne;
            }
            if (levelTwo)
            {
                levelTwo = !levelTwoLoop.Update(gameTime);
                levelThree = !levelTwo;
            }
            if (levelThree)
            {
                levelThree = !levelThreeLoop.Update(gameTime);
                levelMenu = !levelThree;
            }
           
            

            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);
            if (levelMenu)
            {
                menu.Draw(gameTime, _spriteBatch);
            }
            if (levelOne)
            {
                if(levelOneLoop.Draw(gameTime, _spriteBatch))
                {
                    levelOne = false;
                    levelMenu = true;
                    ReloadLevels();
                }
            }
            if (levelTwo)
            {
                if (levelTwoLoop.Draw(gameTime, _spriteBatch))
                {
                    levelTwo = false;
                    levelMenu = true;
                    ReloadLevels();
                }
            }
            if (levelThree)
            {
                if (levelThreeLoop.Draw(gameTime, _spriteBatch))
                {
                    levelThree = false;
                    levelMenu = true;
                    ReloadLevels();
                }
            }

            base.Draw(gameTime);
        }

        private void ReloadLevels()
        {
            levelOneLoop = new FillerLevel();
            levelTwoLoop = new LevelTwoLoop();
            levelThreeLoop = new LevelThree();

            levelOneLoop.Initialize(Content, GraphicsDevice);
            levelTwoLoop.Initialize(Content, GraphicsDevice);
            levelThreeLoop.Initialize(Content, GraphicsDevice);

            menu = new MenuLoop(Content, GraphicsDevice);

            menu.LoadContent();
            levelOneLoop.LoadContent();
            levelTwoLoop.LoadContent();
            levelThreeLoop.LoadContent();
        }
    }
}
