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
        Vector2 mouseWorldPosition;

        Rectangle window, worldRect;
        Texture2D backgroundTexture;

        Texture2D doraTexture;
        Rectangle doraRectangle;
        Vector2 doraSpeed;

        Rectangle waldoRect;

        List<Rectangle> barriers;

        Vector2 cameraPosition;
        Matrix cameraTransform;

        // This will define the distance the player needs to get to the edge of the 
        // screen before we start moving the camera
        float paddingX; 
        float paddingY;

        SpriteFont instructionFont;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            
            // World Size
            worldRect = new Rectangle(0, 0, 1920, 1233);

            //Window Size
            window = new Rectangle(0, 0, 800, 600);
            _graphics.PreferredBackBufferWidth = window.Width;
            _graphics.PreferredBackBufferHeight = window.Height;
            _graphics.ApplyChanges();

            paddingX = 200f; // how far from left/right before camera moves
            paddingY = 150f;

            doraRectangle = new Rectangle(450, 350, 20, 40);
            waldoRect = new Rectangle(1170, 445, 30, 65);
            barriers = new List<Rectangle>();
            barriers.Add(new Rectangle(0, 0, worldRect.Width,320));    // This will keep our player out of the water

            //cameraPosition = doraRectangle.Center.ToVector2();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            backgroundTexture = Content.Load<Texture2D>("Waldo");
            doraTexture = Content.Load<Texture2D>("Dora");
            instructionFont = Content.Load<SpriteFont>("InstructionFont");
            
        
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            // Converts mouse position from window location to world location
            Matrix inverseTransform = Matrix.Invert(cameraTransform);
            mouseWorldPosition = Vector2.Transform(mouseState.Position.ToVector2(), inverseTransform);

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

            // Keep Dora in the world
            if (!worldRect.Contains(doraRectangle))
                doraRectangle.Offset(-doraSpeed);

            // Barrier collision
            foreach (Rectangle barrier in barriers)
                if (barrier.Intersects(doraRectangle))
                    doraRectangle.Offset(-doraSpeed);

            if (waldoRect.Contains(mouseWorldPosition.ToPoint()) && mouseState.LeftButton == ButtonState.Pressed)
                Exit();

            // Updates the Transformation Matrix relative to the players updated position
            SetCameraPadding();
            //SetCamera();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            // Draws the game from the cameras perspective
            _spriteBatch.Begin(transformMatrix: cameraTransform);
            _spriteBatch.Draw(backgroundTexture, worldRect,  Color.White);
            _spriteBatch.Draw(doraTexture, doraRectangle, Color.White);
            _spriteBatch.End();

            // Draws Instructions normally
            _spriteBatch.Begin();
            _spriteBatch.DrawString(instructionFont, "Click on Waldo!", new Vector2(10, 10), Color.Black);
            _spriteBatch.End();


            base.Draw(gameTime);
        }


        // Centers the camera on the player at all times
        public void SetCamera()
        {
            // Calculates the offset between out players position and the center of the game window
            cameraPosition = doraRectangle.Center.ToVector2() - window.Center.ToVector2();
            
            // Clamp the camera position to the world size
            float maxX = worldRect.Width - window.Width;
            float maxY = worldRect.Height - window.Height;
            cameraPosition.X = MathHelper.Clamp(cameraPosition.X, 0, maxX);
            cameraPosition.Y = MathHelper.Clamp(cameraPosition.Y, 0, maxY);

            // Uses this offset to create a translation matrix that can be applied when we draw our world.
            cameraTransform = Matrix.CreateTranslation(new Vector3(-cameraPosition, 0f));
        }

        // Sets the camera on the player, but adds a padding so the camera only moves when the player gets near the edge of the screen
        public void SetCameraPadding()
        {
            // Calculate dead zone boundaries
            float left = cameraPosition.X - window.Width / 2 + paddingX;
            float right = cameraPosition.X + window.Width / 2 - paddingX;
            float top = cameraPosition.Y - window.Height / 2 + paddingY;
            float bottom = cameraPosition.Y + window.Height / 2 - paddingY;

            // Adjust camera only when player leaves the dead zone
            if (doraRectangle.X < left)
                cameraPosition.X = doraRectangle.X + window.Width / 2 - paddingX;
            else if (doraRectangle.X > right)
                cameraPosition.X = doraRectangle.X - window.Width / 2 + paddingX;

            if (doraRectangle.Y < top)
                cameraPosition.Y = doraRectangle.Y + window.Height / 2 - paddingY;
            else if (doraRectangle.Y > bottom)
                cameraPosition.Y = doraRectangle.Y - window.Height / 2 + paddingY;


            // Clamp the camera position to the world size
            float maxX = worldRect.Width - window.Width / 2;
            float maxY = worldRect.Height - window.Height / 2;

            cameraPosition.X = MathHelper.Clamp(cameraPosition.X, window.Width / 2, maxX);
            cameraPosition.Y = MathHelper.Clamp(cameraPosition.Y, window.Height / 2, maxY);

            // First Transformation Follows the player, the second centers screen
            cameraTransform = Matrix.CreateTranslation(new Vector3(-cameraPosition, 0)) *
                Matrix.CreateTranslation(new Vector3(window.Width / 2f, window.Height / 2f, 0));


        }
    }
}
