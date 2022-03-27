using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;


namespace Gaming.Menu
{
    public class MenuLoop
    {
        public TiledMap _tiledMap;
        public TiledMapRenderer _tiledMapRenderer;

        public GraphicsDevice graphicsDevice;
        public ContentManager Content;

        private Texture2D BigMan;
        private Texture2D startButton;

        int animationFrame = 0;
        float animationTimer;

        Rectangle ButtonBounds = new Rectangle(306, 750, 312, 128);
        bool MouseOnButton = false;
        MouseState mouseState;

        public MenuLoop(ContentManager content, GraphicsDevice gd)
        {
            graphicsDevice = gd;
            Content = content;
        }
        public void LoadContent()
        {
            BigMan = Content.Load<Texture2D>("SwordAttack");
            startButton = Content.Load<Texture2D>("MenuContent/Start");
            
            _tiledMap = Content.Load<TiledMap>("MenuContent/titleTileMap");
            _tiledMapRenderer = new TiledMapRenderer(graphicsDevice, _tiledMap);
        }
        public bool Update(GameTime gameTime)
        {
            animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(animationTimer > 0.1f)
            {
                animationFrame = animationFrame + 1;
                if (animationFrame > 3) animationFrame = 0;
                animationTimer -= 0.1f;
            }

            mouseState = Mouse.GetState();
            if (ButtonBounds.Contains(mouseState.Position))
            {
                MouseOnButton = true;
            }
            else
            {
                MouseOnButton = false;
            }

            
            _tiledMapRenderer.Update(gameTime);

            if (MouseOnButton && mouseState.LeftButton == ButtonState.Pressed)
            {
                return true;
            }
            else
                return false;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _tiledMapRenderer.Draw();
            spriteBatch.Begin();


            if(!MouseOnButton)
                spriteBatch.Draw(startButton, new Vector2(306, 750), null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            else
                spriteBatch.Draw(startButton, new Vector2(306, 750), null, Color.Green, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            
            spriteBatch.Draw(BigMan, new Vector2(336, 448), new Rectangle(animationFrame * 192, 0, 192, 192), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
