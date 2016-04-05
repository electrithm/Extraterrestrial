using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace KillAliens
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        //stuff required for drawing
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //creates variables to hold textures
        public Texture2D alien;
        public Texture2D player;
        public Texture2D star;
        public Texture2D bullet;

        //creates variable to store font
        public SpriteFont font;

        //stores any numbers
        public int playerX;
        public int playerY;
        public int playerSpeed = 5;
        public int score = 0;
        public int bulletSpeed = 30;

        //stores decimals
        public float playerRot = 0F;

        //holds all the objects and where they are
        List<int[]> aliens = new List<int[]>();
        List<Rectangle> alienBounds = new List<Rectangle>();
        List<int[]> bullets = new List<int[]>();
        List<Rectangle> bulletBounds = new List<Rectangle>();
        List<int[]> stars = new List<int[]>();

        //allows random number generation
        public Random rnd = new Random();

        //creates rectangle the size of the window to see if objects are outside of it
        Rectangle windowBox;
        Rectangle playerBounds;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //sets the window name
            this.Window.Title = "Extraterrestrial";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //used for drawing textures
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //loads the font
            font = Content.Load<SpriteFont>("score");

            //loads the textures
            alien = Content.Load<Texture2D>("alien");
            player = Content.Load<Texture2D>("player");
            star = Content.Load<Texture2D>("star");
            bullet = Content.Load<Texture2D>("bullet");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() { }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //runs all the different controllers
            KeyboardController();
            AlienController();
            BulletController();
            PlayerController();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //makes the window black
            GraphicsDevice.Clear(Color.Black);

            //puts stars in different places at the start of the game
            if (stars.Count == 0)
            {
                int starCount = rnd.Next(1, 300);
                for (int i = 0; i < starCount; i++)
                {
                    int[] starCoord;
                    starCoord = new int[2];
                    starCoord[0] = rnd.Next(0, Window.ClientBounds.Width);
                    starCoord[1] = rnd.Next(0, Window.ClientBounds.Height);
                    stars.Add(starCoord);
                }
                playerX = Window.ClientBounds.Width / 2;
                playerY = Window.ClientBounds.Height / 2;
                windowBox = new Rectangle(0,0,Window.ClientBounds.Width,Window.ClientBounds.Height);
                //spawns aliens at the start
                AddAlien(5);
            }

            //starts drawing
            spriteBatch.Begin();

            //draws each star
            for (int i = 0; i < stars.Count; i++)
            {
                spriteBatch.Draw(star, new Vector2(stars[i][0], stars[i][1]), Color.Yellow);
            }

            //draws player
            spriteBatch.Draw(player, new Vector2(playerX, playerY), null, Color.Beige, playerRot, new Vector2(player.Width / 2, player.Height / 2), 1, SpriteEffects.None, 1);

            //draws aliens
            for (int i = 0; i < aliens.Count-1; i++)
            {
                spriteBatch.Draw(alien, new Vector2(aliens[i][0], aliens[i][1]), Color.Firebrick);
                spriteBatch.Draw(alien, new Vector2(aliens[i][0], aliens[i][1]), null, Color.Firebrick, MathHelper.ToRadians(0), new Vector2(alien.Width / 2, alien.Height / 2), 1, SpriteEffects.None, 1);
            }

            //draws bullets
            for (int i = 0; i < bullets.Count -1; i++)
            {
                spriteBatch.Draw(bullet, new Vector2(bullets[i][0], bullets[i][1]), Color.White);
            }

            //writes text to the screen
            spriteBatch.DrawString(font, score.ToString(), new Vector2(0, 0), Color.LightBlue);

            //stops drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }

        //handles input from the keyboard
        public void KeyboardController()
        {
            //gets current keys being pressed
            KeyboardState state = Keyboard.GetState();

            //if the escape button is pressed on a controller or keyboard the game exits
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            //if the left key is held the player moves left and faces left
            if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
            {
                playerX = playerX - 5;
                playerRot = MathHelper.ToRadians(270);
                playerBounds = new Rectangle(playerX - player.Width / 2, playerY - player.Width / 2, player.Width, player.Height);
            }
            //if the right key is held the player moves right and faces right
            if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
            {
                playerX = playerX + 5;
                playerRot = MathHelper.ToRadians(90);
                playerBounds = new Rectangle(playerX - player.Width / 2, playerY - player.Width / 2, player.Width, player.Height);
            }
            //if the up key is held the player moves up and faces up
            if (state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.W))
            {
                playerY = playerY - 5;
                playerRot = MathHelper.ToRadians(0);
                playerBounds = new Rectangle(playerX - player.Width / 2, playerY - player.Width / 2, player.Width, player.Height);
            }
            //if the down key is held the player moves down and faces down
            if (state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.S))
            {
                playerY = playerY + 5;
                playerRot = MathHelper.ToRadians(180);
                playerBounds = new Rectangle(playerX - player.Width / 2, playerY - player.Width / 2, player.Width, player.Height);
            }

            //if the space key is held and the rotation is up the player shoots up
            if (state.IsKeyDown(Keys.Space) && playerRot==MathHelper.ToRadians(0) && bullets.Count<3)
            {
                AddPlayerBullet(0);
            }
            //if the space key is held and the rotation is up the player shoots left
            if (state.IsKeyDown(Keys.Space) && playerRot == MathHelper.ToRadians(270) && bullets.Count < 3)
            {
                AddPlayerBullet(270);
            }
            //if the space key is held and the rotation is up the player shoots down
            if (state.IsKeyDown(Keys.Space) && playerRot == MathHelper.ToRadians(180) && bullets.Count < 3)
            {
                AddPlayerBullet(180);
            }
            //if the space key is held and the rotation is up the player shoots right
            if (state.IsKeyDown(Keys.Space) && playerRot == MathHelper.ToRadians(90) && bullets.Count < 3)
            {
                AddPlayerBullet(90);
            }
        }

        //controls the aliens
        public void AlienController()
        {
            //for each alien
            for (int i = 0; i<aliens.Count - 1; i++)
            {
                //if the alien is on the left side of the player it moves right
                if (aliens[i][0] < playerX)
                {
                    aliens[i][0] = aliens[i][0] + 5;//aliens[i][2];
                    alienBounds[i] = new Rectangle(aliens[i][0] - alien.Width / 2, aliens[i][1] - alien.Width / 2, alien.Width, alien.Height);
                }
                //if the alien is on the right side of the player it moves left
                if (aliens[i][0] > playerX)
                {
                    aliens[i][0] = aliens[i][0] - 5;//aliens[i][2];
                    alienBounds[i] = new Rectangle(aliens[i][0] - alien.Width / 2, aliens[i][1] - alien.Width / 2, alien.Width, alien.Height);
                }
                //if the alien is on the top side of the player it moves down
                if (aliens[i][1] < playerY)
                {
                    aliens[i][1] = aliens[i][1] + 5;//aliens[i][2];
                    alienBounds[i] = new Rectangle(aliens[i][0] - alien.Width / 2, aliens[i][1] - alien.Width / 2, alien.Width, alien.Height);
                }
                //if the alien is on the bottom side of the player it moves up
                if (aliens[i][1] < playerX)
                {
                    aliens[i][1] = aliens[i][1] - 5;//aliens[i][2];
                    alienBounds[i] = new Rectangle(aliens[i][0] - alien.Width / 2, aliens[i][1] - alien.Width / 2, alien.Width, alien.Height);
                }
                //Console.WriteLine(aliens.Count+"-"+i);
                //Console.WriteLine("There are "+aliens.Count+" aliens at " + aliens[i][0] + "-" + aliens[i][1]);
            }
        }

        //make the bullets move
        public void BulletController()
        {
            //for each bullet
            for (int i = 0; i < bullets.Count - 1; i++)
            {
                //for each alien check if it was shot
                for (int x = 0; x < aliens.Count - 1; x++)
                {
                    if (bulletBounds[i].Intersects(alienBounds[x]))
                    {
                        //debug
                        //Console.WriteLine("Bullet " + i + " shot alien " + aliens[x].ToString());
                        bullets.RemoveAt(i);
                        bulletBounds.RemoveAt(i);
                        aliens.RemoveAt(x);
                        alienBounds.RemoveAt(x);
                        AddAlien(2);
                    }
                }

                if (bullets.Count == 0)
                {
                    Console.WriteLine("Bullets are too low!");
                    break;
                }
                
                //if the bullet is out of bounds remove it
                if (CheckOutOfBounds(bullets[i][0], bullets[i][1]) == true)
                {
                    bullets.RemoveAt(i);
                    bulletBounds.RemoveAt(i);
                }
                //debug
                //Console.WriteLine(bullets.Count + "-" + bullets[i][0] + "-" + bullets[i][1]);

                //if its facing up move up
                if (bullets[i][2] == 0)
                {
                    bullets[i][1] = bullets[i][1] - bulletSpeed;
                }
                //if its facing down move down
                if (bullets[i][2] == 180)
                {
                    bullets[i][1] = bullets[i][1] + bulletSpeed;
                }
                //if its moving right move right
                if (bullets[i][2] == 90)
                {
                    bullets[i][0] = bullets[i][0] + bulletSpeed;
                }
                //if its facing left move left
                if (bullets[i][2] == 270)
                {
                    bullets[i][0] = bullets[i][0] - bulletSpeed;
                }
            }
        }
        

        //controls players
        public void PlayerController()
        {
            //if the player is going off the screen on the left side they will be stopped
            if (playerX<0 + (player.Width / 2))
            {
                playerX = 0 + (player.Width / 2);
            }
            //if the player is going off the screen on the top side they will be stopped
            if (playerY < 0 + (player.Height / 2))
            {
                playerY = 0 + (player.Height / 2);
            }
            //if the player is going off the screen on the right side they will be stopped
            if (playerX > Window.ClientBounds.Width - (player.Width / 2))
            {
                playerX = Window.ClientBounds.Width - (player.Width / 2);
            }
            //if the player is going off the screen on the bottom side they will be stopped
            if (playerY > Window.ClientBounds.Height - (player.Height / 2))
            {
                playerY = Window.ClientBounds.Height - (player.Height/2);
            }
        }

        //function for checking if a coordinate is out of bounds
        public bool CheckOutOfBounds(int x, int y)
        {
            if (x < 0 || x > Window.ClientBounds.Width || y < 0 || y > Window.ClientBounds.Width)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //add new bullets and have them moving towards the facing
        public void AddPlayerBullet(int facing)
        {
            int[] bulletSpawn;
            bulletSpawn = new int[3];
            bulletSpawn[0] = playerX - player.Width / 2;
            bulletSpawn[1] = playerY - player.Height / 2;
            bulletSpawn[2] = facing;
            bullets.Add(bulletSpawn);
            bulletBounds.Add(new Rectangle(bulletSpawn[0], bulletSpawn[1], bullet.Height, bullet.Width));
        }

        //add aliens
        public void AddAlien(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                int[] alienSpawn;
                alienSpawn = new int[3];
                alienSpawn[0] = rnd.Next(0, Window.ClientBounds.Width);
                alienSpawn[1] = rnd.Next(0, Window.ClientBounds.Height);
                alienSpawn[2] = rnd.Next(1, playerSpeed + 3);
                aliens.Add(alienSpawn);
                alienBounds.Add(new Rectangle(alienSpawn[0], alienSpawn[1], alien.Width, alien.Height));
            }
        }
    }
}
