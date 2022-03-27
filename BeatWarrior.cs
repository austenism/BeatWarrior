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
            levelOneLoop = new FillerLevel();

            levelOneLoop.Initialize(Content, GraphicsDevice);
            //levelTwoLoop.Initialize(Content, GraphicsDevice);
            //levelThreeLoop.Initialize(Content, GraphicsDevice);

            menu = new MenuLoop(Content, GraphicsDevice);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            menu.LoadContent();
            levelOneLoop.LoadContent();
            //levelTwoLoop.LoadContent();
            //levelThreeLoop.LoadContent();
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
                levelOneLoop.Draw(gameTime, _spriteBatch);
            }

            base.Draw(gameTime);
        }
    }
}
