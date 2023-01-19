using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace TractarImg
{
    public class MyImage
    {
        byte[] bytesImatge;
        public int width, height;

        public MyImage(BitmapImage imatgeOriginal)
        {
            int bytesPerPixel = imatgeOriginal.Format.BitsPerPixel / 8;
            if (bytesPerPixel != 4) throw new Exception("NO SE TREBALLAR AMB IMATGES != bpp");

            width = imatgeOriginal.PixelWidth;
            height = imatgeOriginal.PixelHeight;
            bytesImatge = new byte[width * height * bytesPerPixel];
            int stride = width * bytesPerPixel;
            imatgeOriginal.CopyPixels(bytesImatge, stride, 0);
        }

        internal void setPixel(int x, int y, byte[] bytes)
        {
            //P(Y,X) --> Y * WIDTH + X --> Per pasar coordenades (X,Y) a index array
            int nPixel = y * this.width + x; //Ja tenim la posició del pixel en l'array
            int nByte = nPixel * 4;
            bytesImatge[nByte] = bytes[0];
            bytesImatge[nByte + 1] = bytes[1];
            bytesImatge[nByte + 2] = bytes[2];
            bytesImatge[nByte + 3] = bytes[3];
        }

        internal byte[] getPixel(int x, int y)
        {
            int nPixel = y * this.width + x;
            int nByte = nPixel * 4;
            //SI EL TEXT ÉS MÉS LLARG QUE L'ALTURA DE LA IMATGE SALTA ERROR
            byte[] llistaPixel = new byte[4] { bytesImatge[nByte], bytesImatge[nByte + 1], bytesImatge[nByte + 2], bytesImatge[nByte + 3] };

            return llistaPixel;
        }

        internal byte[] getPixel(int nPixel)
        {
            int nByte = nPixel * 4;

            byte[] llistaPixel = new byte[4] { bytesImatge[nByte], bytesImatge[nByte + 1], bytesImatge[nByte + 2], bytesImatge[nByte + 3] };

            return llistaPixel;
        }

        public WriteableBitmap GetWriteableBitmap()
        {
            int bytesPerPixel = 4;
            WriteableBitmap wb = new(width, height, 96, 96, PixelFormats.Bgra32, null);
            int stride = width * bytesPerPixel;
            wb.WritePixels(new Int32Rect(0, 0, width, height), this.bytesImatge, stride, 0);
            return wb;
        }

        internal void writeToDisk(string path)
        {
            using (FileStream streamm = new(path, FileMode.Create))
            {
                BmpBitmapEncoder codificador = new();
                codificador.Frames.Add(BitmapFrame.Create(GetWriteableBitmap()));
                codificador.Save(streamm);
            }
        }
    }
}
