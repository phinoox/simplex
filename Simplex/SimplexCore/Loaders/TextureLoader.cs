using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Text;
using ObjectTK;
using ObjectTK.Textures;

namespace Simplex.Core.Loaders
{
    public class TextureLoader
    {
        Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        private static TextureLoader instance = new TextureLoader();

        public static TextureLoader Instance { get => instance; }

        public Texture2D LoadTexture2D(string path)
        {
            if (textures.ContainsKey(path))
                return textures[path];
            if (!File.Exists(path))
                return null;
            Texture2D tex;
            using (var bitmap = new Bitmap(path))
            {
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                BitmapTexture.CreateCompatible(bitmap, out tex);
                tex.LoadBitmap(bitmap);
            }
            textures[path] = tex;
            return tex;
        }


    }
}
