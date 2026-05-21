using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Translation_Matrix
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        KeyboardState keyboardState;
        MouseState mouseState;

        Rectangle window, worldRect;
        Texture2D backgroundTexture;

        Texture2D doraTexture;
        Rectangle doraRectangle;
        Vector2 doraSpeed;

        Rectangle waldoRect;

        List<Rectangle> barriers;

        Matrix cameraTransform;

        Vector2 offset, cameraPosition;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            window = new Rectangle(0, 0, 800, 600);
            worldRect = new Rectangle(0, 0, 1920, 1233);
            _graphics.PreferredBackBufferWidth = window.Width;
            _graphics.PreferredBackBufferHeight = window.Height;
            _graphics.ApplyChanges();

            doraRectangle = new Rectangle(200, 350, 20, 40);
            waldoRect = new Rectangle(1170, 445, 30, 65);
            barriers = new List<Rectangle>();
            barriers.Add(new Rectangle(0, 0, 1920,330));    // This will keep our player out of the water

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            backgroundTexture = Content.Load<Texture2D>("Waldo");
            doraTexture = Content.Load<Texture2D>("Dora");
            
        
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            // TODO: Add your update logic here
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            doraSpeed = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.Up))          
                doraSpeed.Y -= 2;           
            if (keyboardState.IsKeyDown(Keys.Down))
                doraSpeed.Y += 2;
            if (keyboardState.IsKeyDown(Keys.Left))
                doraSpeed.X -= 2;
            if (keyboardState.IsKeyDown(Keys.Right))
                doraSpeed.X += 2;

            doraRectangle.Offset(doraSpeed);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(backgroundTexture, worldRect,  Color.White);
            _spriteBatch.Draw(doraTexture, doraRectangle, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
