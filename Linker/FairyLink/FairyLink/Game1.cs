using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FairyLink.Device;
using FairyLink.Scene;
using FairyLink.Def;
using FairyLink.Scene.Scenes;

namespace FairyLink
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private GameDevice gameDevice;
        private SceneManager sceneManager;
        private Renderer renderer;


        public Game1()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            graphicsDeviceManager.PreferredBackBufferWidth = Screen.Width;
            graphicsDeviceManager.PreferredBackBufferHeight = Screen.Height;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //IsMouseVisible = true;

            gameDevice = new GameDevice(Content, GraphicsDevice);
            sceneManager = new SceneManager();
            renderer = gameDevice.GetRenderer;
            
            sceneManager.Add(Scene.Scene.TITLE, new Title(gameDevice));
            sceneManager.Add(Scene.Scene.GAMEPLAY, new GamePlay(gameDevice));
            sceneManager.Add(Scene.Scene.ENDING, new Ending(gameDevice));
            sceneManager.Add(Scene.Scene.CLEAR, new Clear(gameDevice));
            sceneManager.Add(Scene.Scene.STAFFROLL, new StaffRoll(gameDevice));
            sceneManager.Add(Scene.Scene.OPERATE, new Operate(gameDevice));
            sceneManager.Add(Scene.Scene.RANKING, new Ranking(gameDevice));
            sceneManager.Add(Scene.Scene.LOADING, new Loading(gameDevice));
            sceneManager.Change(Scene.Scene.LOADING);

            Window.Title = "Linker";
            base.Initialize();
        }

        protected override void LoadContent() {
            gameDevice.Initialize();
        }

        protected override void UnloadContent()
        {
            gameDevice.UnLoad();
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            { this.Exit(); }

            // TODO: Add your update logic here
            gameDevice.Update();
            sceneManager.Update();
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            renderer.Begin();
            sceneManager.Draw(renderer);
            renderer.End();

            base.Draw(gameTime);
        }
    }
}
