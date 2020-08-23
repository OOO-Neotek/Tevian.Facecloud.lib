using System;
using System.Collections.Generic;
using System.Drawing;
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
using Tevian;
using System.IO;
using Newtonsoft.Json;
using ImageMagick;
using SkiaSharp;
using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;


namespace TestingSuite
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Tevian.Tevian tevian;


        public MainWindow()
        {
            InitializeComponent();
        }

        public static SKImageInfo ResizeUntilMPixels(SKImageInfo info, int mpixels)
        {
            float width = info.Width;
            float height = info.Height;
            while (width * height >= mpixels - 1)
            {
                width *= 80.0f / 100.0f;
                height *= 80.0f / 100.0f;
            }

            return new SKImageInfo((int) width, (int) height);
        }

        public byte[] ResizePhoto(Stream photoStream)
        {
            var bitmap = SKBitmap.Decode(photoStream);
            SKImage image;
            SKBitmap newBitmap = null;
            int maxMpix = 12 * 1000 * 1000;
            RtbLog.AppendText($"Image size: {bitmap.Width}x{bitmap.Height}\n");
            if ((bitmap.Width * bitmap.Height) >= maxMpix - 1) // more than 12 MP
            {
                newBitmap = bitmap.Resize(ResizeUntilMPixels(bitmap.Info, maxMpix), SKFilterQuality.High);
                image = SKImage.FromBitmap(newBitmap);
                RtbLog.AppendText($"Image resized: {newBitmap.Width}x{newBitmap.Height}\n");
            }
            else
            {
                image = SKImage.FromBitmap(bitmap);
            }

            using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 90))
            {
                var array = data.ToArray();
                bitmap.Dispose();
                newBitmap?.Dispose();
                image.Dispose();
                return array;
            }
        }


        private async void ButtonGo_OnClick(object sender, RoutedEventArgs e)
        {
            if (TabCtlAction.SelectedItem == TabActDetect)
            {
                await ActDetect();
            }
            else if (TabCtlAction.SelectedItem == TabActMatch)
            {
                await ActMatch();
            }
        }

        private byte[] GetPhoto(string fpath)
        {
            if (fpath == "") return null;

            using (var fstream = File.OpenRead(fpath))
            {
                return ResizePhoto(fstream);
            }
        }

        private async Task ActMatch()
        {
            var photo1 = GetPhoto(Match1FPath.TextBox);
            var photo2 = GetPhoto(Match2FPath.TextBox);

            if (photo1 == null || photo2 == null)
                return;

            AddViewTabWithPB(photo1, ref ViewImage);
            AddViewTabWithPB(photo2, ref ViewImage2);
            ViewImage2.Visibility = Visibility.Visible;

            RtbLog.AppendText("Called Match method...\n");
            InGo(false);

            await Task.Run(async () =>
            {
                var r = await tevian.Match(photo1, photo2,
                    fdMinSize2.Float,
                    fdMinSize2.Float,
                    fdMinSize2.Float);

                await Dispatcher.BeginInvoke((Action) (() =>
                {
                    RtbLog.AppendText("Response: " + JsonConvert.SerializeObject(r) + "\n");

                    if (r == null)
                        return;

                    //var bboxes = r.face1_bbox.Select(fwi => fwi.Bbox).ToArray();

                    DrawBboxes(photo1, new[] {r.Face1.Bbox}, ref ViewImage);

                    DrawBboxes(photo2, new[] {r.Face2.Bbox}, ref ViewImage2);
                    InGo(true);
                }));
            });
        }

        private async Task ActDetect()
        {
            var photo = GetPhoto(DetectFpath.TextBox);

            if (photo == null)
                return;

            AddViewTabWithPB(photo, ref ViewImage);
            ViewImage2.Visibility = Visibility.Hidden;


            RtbLog.AppendText("Called Detect method...\n");
            InGo(false);

            bool? demographics = CbDemographics.IsChecked;
            bool? attributes = CbAttributes.IsChecked;
            bool? landmarks = CbLandmarks.IsChecked;
            bool? liveness = CbLiveness.IsChecked;

            await Task.Run(async () =>
            {
                var r = (await tevian.Detect(photo,
                    fdMinSize.Float,
                    fdMinSize.Float,
                    fdThreshold.Float,
                    false,
                    null,
                    demographics,
                    attributes,
                    landmarks,
                    liveness));

                await Dispatcher.BeginInvoke((Action) (() =>
                {
                    RtbLog.AppendText("Response: " + JsonConvert.SerializeObject(r) + "\n");

                    if (r.Item1.Length == 0)
                        return;

                    var bboxes = r.Item1.Select(fwi => fwi.Bbox).ToArray();

                    DrawBboxes(photo, bboxes, ref ViewImage);

                    InGo(true);
                }));
            });
        }

        private async void ButtonRegister_OnClick(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                try
                {
                    await Tevian.Tevian.CreateAccount(LtbEmail.TextBox, LtbPassw.TextBox);
                    ButtonLogin_OnClick(sender, e);
                }
                catch (TevianException ex)
                {
                    await Dispatcher.BeginInvoke((Action) (() => { RtbLog.AppendText(ex.ToString() + "\n"); }));
                    MessageBox.Show(ex.ToString(), "Exception", MessageBoxButton.OK);
                }
            });
        }

        private async void ButtonLogin_OnClick(object sender, RoutedEventArgs e)
        {
            await Task.Run(async () =>
            {
                try
                {
                    tevian = new Tevian.Tevian(LtbEmail.TextBox, LtbPassw.TextBox);
                    await Dispatcher.BeginInvoke((Action) (() =>
                    {
                        BtnGo.IsEnabled = true;
                        AuthPanel.IsEnabled = false;
                        RtbLog.AppendText("Logged in. JWT: " + tevian.jwt + "\n");
                    }));
                }
                catch (TevianException ex)
                {
                    MessageBox.Show(ex.ToString(), "Exception", MessageBoxButton.OK);
                }
            });
        }

        private void AddViewTabWithPB(byte[] photo, ref Image image)
        {
            using (var stream = new MemoryStream(photo))
            {
                image.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }


        private void DrawBboxes(byte[] photo, Bbox[] bboxes, ref Image img)
        {
            using (var image = new MagickImage(photo))
            {
                var drawables = new Drawables();
                drawables.StrokeWidth(ViewImage.Source.Height >= 1000 ? 8 : 2);
                drawables.FillColor(MagickColor.FromRgba(255, 255, 255, 0));
                drawables.StrokeColor(MagickColor.FromRgb(255, 0, 0));

                foreach (var bbox in bboxes)
                {
                    drawables.Rectangle(bbox.X,
                        bbox.Y,
                        bbox.X + bbox.Width,
                        bbox.Y + bbox.Height
                    );
                }

                image.Draw(drawables);

                using (var ms = new MemoryStream(image.ToByteArray()))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    img.Source = bitmap;
                }
            }
        }

        private void InGo(bool inGo)
        {
            TabCtlAction.IsEnabled = inGo;
            BtnGo.IsEnabled = inGo;
        }

        private void RtbLog_TextChanged(object sender, EventArgs e)
        {
            //if (Math.Abs(RtbLog.VerticalOffset + RtbLog.ViewportHeight - RtbLog.ExtentHeight) < 0.05)
            RtbLog.ScrollToEnd();
        }
    }
}
