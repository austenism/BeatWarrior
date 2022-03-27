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

        LevelLoop currentLevel;
        bool playingGame = false;

        MenuLoop menu;

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
            currentLevel = new FillerLevel();
            currentLevel.Initialize(Content, GraphicsDevice);

            menu = new MenuLoop(Content, GraphicsDevice);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            currentLevel.LoadContent();

            menu.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (playingGame)
                currentLevel.Update(gameTime);
            else
                playingGame = menu.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);
            if (playingGame)
                currentLevel.Draw(gameTime, _spriteBatch);
            else
                menu.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }
    }
}
