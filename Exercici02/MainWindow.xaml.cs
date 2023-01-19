using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using TractarImg;

namespace Exercici02
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MyImage img;

        public MainWindow()
        {
            InitializeComponent();

            //Test();

            btnCarregarImg.IsEnabled = false;
            img = new MyImage(new BitmapImage(new Uri(imgObrir.Source.ToString())));
        }

        private void Test()
        {
            String text = imgObrir.Source.ToString();
            MyImage myImgTest = new MyImage(new BitmapImage(new Uri("pack://application:,,,/Img/imatge.bmp")));
            EsteganografiarImatgeTest(myImgTest, "az");
            EsteganografiarImatgeTest(myImgTest, "azx");
        }

        // Carrega la img que vulguis dels teus documents
        private void btnCarregarImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Fitxers imatge|*.jpg;*.png;*.bmp";
            ofd.ShowDialog();

            try
            {
                imgObrir.Source = new BitmapImage(new Uri(ofd.FileName));
                img = new MyImage(new BitmapImage(new Uri(imgObrir.Source.ToString())));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //Estenografia la img amb el text de la caixa
        private void btnInserirImg_Click(object sender, RoutedEventArgs e)
        {
            EsteganografiarImatge(img, tbMissatje.Text);
            tbMissatje.Text = "";
            imgManipulada.Source = img.GetWriteableBitmap();
            tbMissatje.MaxLength = img.height;
        }

        //Estenografia la img
        //El primer pixel contindrà la longitud del text així serà més cómode a l'hora de
        //voles desxifrar el text.
        //Cada caràcter està xifrat entre 2 bites, seguint les indiciacions de l'enunciat.
        private void EsteganografiarImatge(MyImage img, string text)
        {
            //El primer pixel conté la longitud del text
            byte[] bytes00 = img.getPixel(0, 0);
            char[] b0 = Convert.ToString(bytes00[0], 2).PadLeft(8, '0').ToCharArray();
            char[] g0 = Convert.ToString(bytes00[1], 2).PadLeft(8, '0').ToCharArray();
            char[] r0 = Convert.ToString(bytes00[2], 2).PadLeft(8, '0').ToCharArray();

            byte[] bytes01 = img.getPixel(0, 1);
            char[] b1 = Convert.ToString(bytes01[0], 2).PadLeft(8, '0').ToCharArray();
            char[] g1 = Convert.ToString(bytes01[1], 2).PadLeft(8, '0').ToCharArray();
            char[] r1 = Convert.ToString(bytes01[2], 2).PadLeft(8, '0').ToCharArray();

            string logitudBinari = Convert.ToString(text.Length, 2).PadLeft(8, '0');

            r0[0] = logitudBinari[0];
            r0[1] = logitudBinari[1];
            b0[0] = logitudBinari[2];
            g0[0] = logitudBinari[3];

            r1[0] = logitudBinari[4];
            r1[1] = logitudBinari[5];
            g1[0] = logitudBinari[6];
            b1[0] = logitudBinari[7];

            int bEnter0 = convertirAEnter(b0);
            int gEnter0 = convertirAEnter(g0);
            int rEnter0 = convertirAEnter(r0);

            int bEnter1 = convertirAEnter(b1);
            int gEnter1 = convertirAEnter(g1);
            int rEnter1 = convertirAEnter(r1);

            img.setPixel(0, 0, new byte[] { (byte)bEnter0, (byte)gEnter0, (byte)rEnter0, bytes00[3] });
            img.setPixel(0, 0, new byte[] { (byte)bEnter1, (byte)gEnter1, (byte)rEnter1, bytes01[3] });

            //AFEGIR TEXT A LA IMG
            int eixX = 0, eixY = 0;
            for (int i = 0; i < text.Length; i++)
            {
                b0 = Convert.ToString(bytes00[0], 2).PadLeft(8, '0').ToCharArray();
                g0 = Convert.ToString(bytes00[1], 2).PadLeft(8, '0').ToCharArray();
                r0 = Convert.ToString(bytes00[2], 2).PadLeft(8, '0').ToCharArray();

                b1 = Convert.ToString(bytes01[0], 2).PadLeft(8, '0').ToCharArray();
                g1 = Convert.ToString(bytes01[1], 2).PadLeft(8, '0').ToCharArray();
                r1 = Convert.ToString(bytes01[2], 2).PadLeft(8, '0').ToCharArray();

                char lletra = text[i];
                string lletraBinari = Convert.ToString((int)lletra, 2).PadLeft(8, '0');

                r0[0] = lletraBinari[0];
                r0[1] = lletraBinari[1];
                b0[0] = lletraBinari[2];
                g0[0] = lletraBinari[3];

                r1[0] = lletraBinari[4];
                r1[1] = lletraBinari[5];
                g1[0] = lletraBinari[6];
                b1[0] = lletraBinari[7];

                bEnter0 = convertirAEnter(b0);
                gEnter0 = convertirAEnter(g0);
                rEnter0 = convertirAEnter(r0);

                bEnter1 = convertirAEnter(b1);
                gEnter1 = convertirAEnter(g1);
                rEnter1 = convertirAEnter(r1);

                eixY++;
                img.setPixel(eixX, eixY, new byte[] { (byte)bEnter0, (byte)gEnter0, (byte)rEnter0, bytes00[3] });
                eixY++;
                img.setPixel(eixX, eixY, new byte[] { (byte)bEnter1, (byte)gEnter1, (byte)rEnter1, bytes00[3] });
            }
        }


        private void EsteganografiarImatgeTest(MyImage img, string text)
        {
            byte[] bytes00 = img.getPixel(0, 0);
            char[] b0;
            char[] g0;
            char[] r0;

            byte[] bytes01 = img.getPixel(0, 1);
            char[] b1;
            char[] g1;
            char[] r1;


            int bEnter0;
            int gEnter0;
            int rEnter0;

            int bEnter1;
            int gEnter1;
            int rEnter1;

            //AFEGIR TEXT A LA IMG
            int eixX = 0, eixY = -1;
            for (int i = 0; i < text.Length; i++)
            {
                b0 = Convert.ToString(bytes00[0], 2).PadLeft(8, '0').ToCharArray();
                g0 = Convert.ToString(bytes00[1], 2).PadLeft(8, '0').ToCharArray();
                r0 = Convert.ToString(bytes00[2], 2).PadLeft(8, '0').ToCharArray();

                b1 = Convert.ToString(bytes01[0], 2).PadLeft(8, '0').ToCharArray();
                g1 = Convert.ToString(bytes01[1], 2).PadLeft(8, '0').ToCharArray();
                r1 = Convert.ToString(bytes01[2], 2).PadLeft(8, '0').ToCharArray();

                char lletra = text[i];
                string lletraBinari = Convert.ToString((int)lletra, 2).PadLeft(8, '0');

                r0[0] = lletraBinari[0];
                r0[1] = lletraBinari[1];
                b0[0] = lletraBinari[2];
                g0[0] = lletraBinari[3];

                r1[0] = lletraBinari[4];
                r1[1] = lletraBinari[5];
                g1[0] = lletraBinari[6];
                b1[0] = lletraBinari[7];

                bEnter0 = convertirAEnter(b0);
                gEnter0 = convertirAEnter(g0);
                rEnter0 = convertirAEnter(r0);

                bEnter1 = convertirAEnter(b1);
                gEnter1 = convertirAEnter(g1);
                rEnter1 = convertirAEnter(r1);

                eixY++;
                img.setPixel(eixX, eixY, new byte[] { (byte)bEnter0, (byte)gEnter0, (byte)rEnter0, bytes00[3] });
                eixY++;
                img.setPixel(eixX, eixY, new byte[] { (byte)bEnter1, (byte)gEnter1, (byte)rEnter1, bytes00[3] });
            }
        }

        private int convertirAEnter(char[] bitsLletra)
        {
            int resultat = 0;
            int potencia = 1;
            for (int i = bitsLletra.Length - 1; i >= 0; i--)
            {
                resultat += (bitsLletra[i] - '0') * potencia;
                potencia = potencia * 2;
            }

            return resultat;
        }

        private void tbMissatje_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbMissatje.Text.Length > 0) btnInserirImg.IsEnabled = true;
            else btnInserirImg.IsEnabled = false;
            tbMissatje.MaxLength = img.height - 1;
        }
    }
}
