using ObjectTK.Textures;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Simplex.Core.Loaders
{
    /// <summary>
    /// class for loading textures via opentk
    /// </summary>
    public class TextureLoader
    {
        #region Private Fields

        private static TextureLoader instance = new TextureLoader();
        private Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// the instance of the textureloader
        /// ToDo: move to ResourceLoader
        /// </summary>
        public static TextureLoader Instance { get => instance; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// loads a texture from a given filepath
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Texture2D LoadTexture2D(string path)
        {
            if (textures.ContainsKey(path))
                return textures[path];
            if (!File.Exists(path))
                return null;
            Texture2D tex;
            using (var bitmap = new Bitmap(path))
            {
                //bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                BitmapTexture.CreateCompatible(bitmap, out tex);
                tex.LoadBitmap(bitmap);
                tex.GenerateMipMaps();
            }
            textures[path] = tex;
            return tex;
        }

        #endregion Public Methods
    }
}