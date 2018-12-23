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
using System.Reflection;
using System.Runtime.InteropServices;

namespace RealTimeTranslate3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WebBrowser webBrowser;
        TextBox textBoxInput;
        RadioButton radioButtonGoogle, radioButtonYahoo;
        RadioButton radioButtonGoogleAuto, radioButtonGoogleEC, radioButtonGoogleCE;
        Button buttonExpand,buttonRefresh,buttonBackword,buttonForward;
        Grid gridControl;
        Brush buttonForeground;
        Label labelBrowserStatus;
        TextBox textBoxUrl;
        CheckBox checkBoxMonitorClipboard, checkBoxAutoPopUp;
        private async void ReportError(Exception error,ContentControl e=null)
        {
            if(e!=null)
            {
                e.Foreground = new SolidColorBrush(Colors.MediumVioletRed);
                await Task.Delay(1000);
                if (e is Button) e.Foreground = buttonForeground;
                else MessageBox.Show($"type: {e.GetType()}\r\ninstance: {e}");
            }
        }
        private void InitializeViews()
        {
            webBrowser = new WebBrowser();
            webBrowser.Focusable = false;
            webBrowser.Navigating += (o, e) =>
            {
                labelBrowserStatus.Content = "⏳";
                textBoxUrl.Text = e.Uri.AbsoluteUri;
            };
            webBrowser.Navigated += (o, e) =>
            {
                SetSilent(webBrowser, true);
                labelBrowserStatus.Content = "🗸";
                textBoxUrl.Text = e.Uri.AbsoluteUri;
            };
            webBrowser.LoadCompleted += (o, e) =>
            {
                labelBrowserStatus.Content = "✔";
                textBoxUrl.Text = e.Uri.AbsoluteUri;
            };
            //PerformNavigate("https://codingsimplifylife.blogspot.com/");
            textBoxInput = new TextBox { FontSize = 18,MaxHeight=50 };
            textBoxInput.TextInput += TextBoxInput_TextInput;
            radioButtonGoogle = new RadioButton { Content = "Google", FontSize = 15 };
            radioButtonYahoo = new RadioButton { Content = "Yahoo", FontSize = 15, IsChecked = true };
            radioButtonGoogleAuto = new RadioButton { Content = "Auto", FontSize = 12, IsChecked = true };
            radioButtonGoogleCE = new RadioButton { Content = "CE", FontSize = 12 };
            radioButtonGoogleEC = new RadioButton { Content = "EC", FontSize = 12 };
            radioButtonGoogle.Checked += delegate { Refresh(); };
            radioButtonYahoo.Checked += delegate { Refresh(); };
            radioButtonGoogleAuto.Checked += delegate { Refresh(); };
            radioButtonGoogleCE.Checked += delegate { Refresh(); };
            radioButtonGoogleEC.Checked += delegate { Refresh(); };

            buttonExpand = new Button { Content = "︾展開", FontSize = 15 };//︽︾
            {
                bool folded = true;
                buttonExpand.Click += delegate
                {
                    folded ^= true;
                    gridControl.Visibility = folded ? Visibility.Hidden : Visibility.Visible;
                    gridControl.MaxHeight = folded ? 0 : double.MaxValue;
                    buttonExpand.Content = folded ? "︾展開" : "︽摺疊";
                };
            }
            buttonRefresh = new Button { Content = "⭮重整", FontSize = 15 };
            buttonRefresh.Click += delegate { try { webBrowser.Refresh(); } catch (Exception error) { ReportError(error,buttonRefresh); } };
            buttonBackword = new Button { Content = "←上一頁", FontSize = 15 };
            buttonBackword.Click += delegate { try { webBrowser.GoBack(); } catch (Exception error) { ReportError(error,buttonBackword); } };
            buttonForward = new Button { Content = "下一頁→", FontSize = 15 };
            buttonForward.Click += delegate { try { webBrowser.GoForward(); } catch (Exception error) { ReportError(error, buttonForward); } };
            checkBoxMonitorClipboard = new CheckBox { Content = "監視剪貼簿", IsChecked = true, FontSize = 12 };
            checkBoxMonitorClipboard.Checked += delegate { LaunchClipboardMonitor(); };
            checkBoxMonitorClipboard.Unchecked += delegate { clipboardMonitorCounter++; };
            checkBoxAutoPopUp = new CheckBox { Content = "自浮", IsChecked = true, FontSize = 12 };
            labelBrowserStatus = new Label { Content = "💤" };
            labelBrowserStatus.MouseDown += delegate { PerformNavigate("https://codingsimplifylife.blogspot.com/"); };
            textBoxUrl = new TextBox();
            textBoxUrl.TextInput += delegate { try { webBrowser.Navigate(textBoxUrl.Text); } catch (Exception error) { MessageBox.Show(error.ToString()); } };
        }
        public static void SetSilent(WebBrowser browser, bool silent)
        {
            if (browser == null)
                throw new ArgumentNullException("browser");

            // get an IWebBrowser2 from the document
            IOleServiceProvider sp = browser.Document as IOleServiceProvider;
            if (sp != null)
            {
                Guid IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
                Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

                object webBrowser;
                sp.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out webBrowser);
                if (webBrowser != null)
                {
                    webBrowser.GetType().InvokeMember("Silent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty, null, webBrowser, new object[] { silent });
                }
            }
        }
        [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IOleServiceProvider
        {
            [PreserveSig]
            int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
        }

        //DateTime datetime = DateTime.Now;
        //private async void LostBrowserFocus()
        //{
        //    this.Focus();
        //    //webBrowser.Visibility = Visibility.Hidden;
        //    //webBrowser.IsEnabled = false;
        //    //this.Focus();
        //    //webBrowser.Visibility = Visibility.Visible;
        //    //webBrowser.IsEnabled = true;
        //}
        private void DrawViews()
        {
            //var w = new WebBrowser();
            //w.Navigate("https://translate.google.com.tw/#en/zh-TW/https%3A%2F%2Fcodingsimplifylife.blogspot.com%2F");
            this.Content = new Grid
            {
                ClipToBounds = true,
                RowDefinitions =
                {
                    new RowDefinition{Height=new GridLength(1,GridUnitType.Auto) },
                    new RowDefinition{Height=new GridLength(1,GridUnitType.Auto) },
                    new RowDefinition{Height=new GridLength(1,GridUnitType.Star) }
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition{Width=new GridLength(1,GridUnitType.Star)},
                    new ColumnDefinition{Width=new GridLength(1,GridUnitType.Star)}
                },
                Children =
                {
                    textBoxInput.Set(0,0,2),
                    new Grid
                    {
                        RowDefinitions={new RowDefinition { Height=new GridLength(1,GridUnitType.Auto)} },
                        ColumnDefinitions =
                        {
                            new ColumnDefinition{Width=new GridLength(1,GridUnitType.Star)},
                            new ColumnDefinition{Width=new GridLength(1,GridUnitType.Star)},
                            new ColumnDefinition{Width=new GridLength(1,GridUnitType.Star)},
                            new ColumnDefinition{Width=new GridLength(1,GridUnitType.Star)}
                        },
                        Children=
                        {
                            radioButtonGoogle.Set(0,0,2),
                            new Grid
                            {
                                RowDefinitions=
                                {
                                    new RowDefinition {Height=new GridLength(1,GridUnitType.Auto) },
                                    new RowDefinition {Height=new GridLength(1,GridUnitType.Auto) }
                                },
                                ColumnDefinitions=
                                {
                                    new ColumnDefinition { Width=new GridLength(1,GridUnitType.Auto)},
                                    new ColumnDefinition { Width=new GridLength(1,GridUnitType.Auto)}
                                },
                                Children=
                                {
                                    radioButtonGoogleAuto.Set(1,0),
                                    radioButtonGoogleCE.Set(0,0),
                                    radioButtonGoogleEC.Set(0,1)
                                }
                            }.Set(0,1),
                            radioButtonYahoo.Set(0,2,2),
                            buttonExpand.Set(0,3,2)
                        }
                    }.Set(0,1),
                    (gridControl=new Grid
                    {
                        MaxHeight=0,
                        RowDefinitions={new RowDefinition { Height=new GridLength(1,GridUnitType.Auto)} },
                        ColumnDefinitions=
                        {
                            new ColumnDefinition{Width=new GridLength(1,GridUnitType.Auto)},
                            new ColumnDefinition{Width=new GridLength(1,GridUnitType.Auto)},
                            new ColumnDefinition{Width=new GridLength(1,GridUnitType.Auto)},
                            new ColumnDefinition{Width=new GridLength(1,GridUnitType.Auto)},
                            new ColumnDefinition{Width=new GridLength(1,GridUnitType.Auto)},
                            new ColumnDefinition{Width=new GridLength(1,GridUnitType.Auto)},
                            new ColumnDefinition{Width=new GridLength(1,GridUnitType.Star)}
                        },
                        Children=
                        {
                            buttonRefresh.Set(0,0),
                            buttonBackword.Set(0,1),
                            buttonForward.Set(0,2),
                            checkBoxMonitorClipboard.Set(0,3),
                            checkBoxAutoPopUp.Set(0,4),
                            labelBrowserStatus.Set(0,5),
                            textBoxUrl.Set(0,6)
                        }
                    }).Set(1,0).SetSpan(1,2),
                    webBrowser.Set(2,0,2).SetSpan(1,2)
                }
            };
        }
        void PerformNavigate(string url)
        {
            //if (webBrowser.Children.Count == 0)
            //{
            //    WebBrowser w = new WebBrowser();
            //    webBrowser.Children.Clear();
            //    webBrowser.Children.Add(w);
            //}
            //(webBrowser.Children[0] as WebBrowser).Navigate(url);
            try
            {
                //System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                //var webcontent = await client.GetStreamAsync(url);
                ////webBrowser.Visibility = Visibility.Hidden;
                //webBrowser.NavigateToStream(webcontent);
                //webBrowser.Source = new Uri(url);
                //webBrowser.Refresh();
                //await Task.Delay(100);
                //webBrowser.Visibility = Visibility.Visible;
                webBrowser.Navigate(url);
            }
            catch(Exception error) { ReportError(error); }
        }
        private void Refresh()
        {
            var word = this.Title = textBoxInput.Text;
            if ((bool)radioButtonGoogle.IsChecked)
            {
                if ((bool)radioButtonGoogleAuto.IsChecked)
                {
                    PerformNavigate(GoogleTranslate.TranslateUrlAuto(word));
                }
                else if ((bool)radioButtonGoogleCE.IsChecked) PerformNavigate(GoogleTranslate.TranslateUrlCE(word));
                else if ((bool)radioButtonGoogleEC.IsChecked) PerformNavigate(GoogleTranslate.TranslateUrlEC(word));
                else MessageBox.Show("Google Auto/CE/EC?");
            }
            else if ((bool)radioButtonYahoo.IsChecked)
            {
                PerformNavigate(YahooDictionary.TranslateUrl(word));
            }
            else MessageBox.Show("Google/Yahoo?");
        }

        private void TextBoxInput_TextInput(object sender, TextCompositionEventArgs e)
        {
            Refresh();
        }
        public MainWindow()
        {
            InitializeComponent();
            this.Title = "Initializing...";
            InitializeViews();
            this.Title = "Drawing Views...";
            DrawViews();
            PostInitialize();
            this.Title = "OK";
        }
        void PostInitialize()
        {
            buttonForeground = buttonExpand.Foreground;
            LaunchClipboardMonitor();
        }
        string clipBoard = null;
        int clipboardMonitorCounter = 0;
        async void LaunchClipboardMonitor()
        {
            int myCounter = System.Threading.Interlocked.Increment(ref clipboardMonitorCounter);
            clipBoard = null;
            while (myCounter == clipboardMonitorCounter)
            {
                try
                {
                    var s = Clipboard.GetText();
                    if (s != clipBoard)
                    {
                        textBoxInput.Text = clipBoard = s;
                        Refresh();
                        if (checkBoxAutoPopUp.IsChecked == true)
                        {
                            if(!this.Activate())
                            {
                                //this.Title += "#";
                                this.Topmost = true;
                                this.Activate();
                                this.Topmost = false;
                            }
                        }
                    }
                }
                catch (Exception erorr) { this.Title = erorr.ToString(); }
                await Task.Delay(500);
            }
        }
    }
}
