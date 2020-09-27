﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Simplex.Gui.Control;

namespace Simplex.Gui.Renderer
{
    public static class ImageLoader{

        public delegate Bitmap Loader(string filename);

        public static Dictionary<string, Loader> loaders = new Dictionary<string, Loader>(){
            { "jpeg",StandartLoader},
            { "jpe",StandartLoader},
            { "jfif",StandartLoader},
            { "jpg",StandartLoader},

            { "bmp",StandartLoader},
            { "dib",StandartLoader},
            { "rle",StandartLoader},
            
            { "png",StandartLoader},
            
            { "gif",StandartLoader},

            { "tif",StandartLoader},
            { "exif",StandartLoader},

            { "wmf",StandartLoader},
            { "emf",StandartLoader},
        };

        public static Bitmap StandartLoader(string s){
            return new Bitmap(s);
        }

        public static Bitmap Load(string filename){
            try{
                string s = filename.ToLower().Split('.').Last();
                if (loaders.ContainsKey(s)){
                    return loaders[s].Invoke(filename);
                }else{
                    throw new Exception("не найден загрузчик формата. Add format loader in ImageLoader.loaders");
                }
            }
            catch (Exception e){
                throw new Exception("Image not found: " + filename);
            }
        }
    }
}
