using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RogerNES
{
    public class GameRender
    {
        RogerNES rogerNes;
        SpriteBatch spriteBatch;
        Texture2D textureScreenBuffer;
        public PresentationParameters pp;

        ///Set up background color with use of Alpha
        Microsoft.Xna.Framework.Graphics.GraphicsDevice device;

        public void IntializeGame()
        {
            ///Create Panel so the same can be rendered on it
            createGamePanel();
        }

        private void createGamePanel()
           
        Set the 
        {
            throw new NotImplementedException();
        }
    }
}

