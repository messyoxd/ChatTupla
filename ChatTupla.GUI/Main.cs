using System.Linq;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using System;

namespace ChatTupla.GUI
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Desktop _desktop;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            MyraEnvironment.Game = this;

            
            
            // var telaPrincipal = new TelaPrincipal("Messyo");
            var telaInicial = new TelaInicial(setWindowTitle);

            // Add it to the desktop
            _desktop = new Desktop();
            // _desktop.Root = telaPrincipal.ReturnTelaPrincipal(amigos);
            _desktop.Root = telaInicial.returnTelaPrincipal();
            _desktop.HasExternalTextInput = true;
            Window.Title = "Chat Tupla";
            // Provide that text input
            Window.TextInput += (s, a) =>
            {
                _desktop.OnChar(a.Character);
            };
        }
        public void setWindowTitle(string text){
            Window.Title = text;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            IsMouseVisible = true;
            GraphicsDevice.Clear(Color.Black);
            _desktop.Render();
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args){
            Console.WriteLine("Saindo");
            Exit();
            Environment.Exit(0);
        }

    }
}
