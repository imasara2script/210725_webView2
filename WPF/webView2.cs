using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// CoreWebView2NavigationCompletedEventArgsの参照に使用
using Microsoft.Web.WebView2.Core;

namespace netHTA
{
    class webView2関係
    {
        private CoreWebView2 cwv2;
        private MainWindow ウィンドウ;

        public void 初期化(CoreWebView2 web, MainWindow win)
        {
            cwv2 = web;
            cwv2.NavigationCompleted += Web_NavigationCompleted;
            cwv2.WebMessageReceived += MessageReceived;

            ウィンドウ = win;
        }

        private void Web_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            //読み込み結果を判定
            if (e.IsSuccess)
                Console.WriteLine("complete");
            else
                Console.WriteLine(e.WebErrorStatus);
        }

        //JavaScriptからメッセージを受信したときに実行します。
        private void MessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {
            String text = args.TryGetWebMessageAsString();
            string[] 配列 = text.Split('=');
            string 項目 = 配列[0];

            string[] 値の範囲 = new string[配列.Length - 1];
            Array.ConstrainedCopy(配列, 1, 値の範囲, 0, 配列.Length - 1);

            string 値 = String.Join("=", 値の範囲);

            if (項目=="icon") {
                var getBF = new getBitmapFrame();
                ウィンドウ.ウィンドウのプロパティを変更("Icon", getBF.fromString(値));
                return;
            }

            string 戻り値;
            string path;
            string callBackIndex;
            string 文字列;
            if (項目.IndexOf("get") != 0)
            {
                switch (項目)
                {
                    case "AppActivate":
                        return;
                    case "Close":
                        return;
                    case "TextWrite":
                        path          = 値の範囲[0];
                        callBackIndex = 値の範囲[値の範囲.Length-1];

                        string[] 文字列の範囲 = new string[値の範囲.Length - 2];
                        Array.ConstrainedCopy(値の範囲, 1, 文字列の範囲, 0, 値の範囲.Length-2);
                        文字列 = string.Join("=", 文字列の範囲);

                        TextWrite.上書き(path, 文字列);
                        cwv2.ExecuteScriptAsync("WPF.callBack(" + callBackIndex + ", null)");
                        return;
                    case "TextRead":
                        path          = 値の範囲[0];
                        callBackIndex = 値の範囲[1];
                        戻り値 = TextRead.getAll(path);
                        戻り値 = 戻り値.Replace("#", "#a").Replace("*", "#b");
                        戻り値 = "(()=>{/*" + 戻り値 + " */}).toString().split('/*')[1].split(' */')[0].replace(/#b/g,'*').replace(/#a/g,'#')"; // 終端側は「*/」にしちゃうと、読み込んだ文字列の末尾が「/」だった場合に「/*」とマッチしてsplitした際に末尾の「/」が消えちゃう。
                        cwv2.ExecuteScriptAsync("WPF.callBack(" + callBackIndex + ", " + 戻り値 + ")");
                        return;
                }

                Int32 int値;
                if (int.TryParse(値, out int値))
                {
                    ウィンドウ.ウィンドウのプロパティを変更(項目, int値);
                }
                else {
                    ウィンドウ.ウィンドウのプロパティを変更(項目, 値);
                }
                return;
            }
            else
            {
                項目 = 項目.Substring(3);

                switch (項目)
                {
                    case "Args":
                        戻り値 = "'" + string.Join("','", ウィンドウ.get引数()).Replace("\\", "/") + "'";
                        break;
                    case "Left":
                    case "Top":
                        戻り値 = ウィンドウ.ウィンドウのプロパティ値を取得<Double>(項目).ToString();
                        break;
                    default:
                        戻り値 = ウィンドウ.ウィンドウのプロパティ値を取得<String>(項目);
                        break;
                }
                cwv2.ExecuteScriptAsync("WPF.callBack(" + 値 + ", " + 戻り値 + ")");
                return;
            }
        }
    }
}
