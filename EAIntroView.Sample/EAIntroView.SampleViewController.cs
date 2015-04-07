using System;
using UIKit;
using Refractored.Xam.Settings;
using Refractored.Xam.Settings.Abstractions;
using CoreGraphics;

namespace EAIntroView.Sample
{
    public partial class EAIntroView_SampleViewController : UIViewController
    {
        public EAIntroView_SampleViewController(IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// App设置
        /// </summary>
        /// <value>The app settings.</value>
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            // Perform any additional setup after loading the view, typically from a nib.
    
            //在主界面中添加一个UILabel用于区分
            var label = new UILabel(new CGRect(0, 200, 320, 60));
            label.Text = "这里是主界面哦~";
            label.TextAlignment = UITextAlignment.Center;
            label.Font = UIFont.SystemFontOfSize(40);
            this.View.AddSubview(label);

            //通过Setting获取启动次数，当第一次启动的时候获取到的值为0
            var BootTimes = AppSettings.GetValueOrDefault<int>("BootTimes");

            //第一个欢迎页面，我们在上面显示本次是第几次启动App
            EAIntroPage page1 = new EAIntroPage();
            page1.Title = "Page1";
            page1.Desc = "Hello World   Page1 no Description";
            page1.BgColor = UIColor.Orange;

            //在正常情况下我们可以通过判断BootTimes的值来决定是否显示欢迎界面
            if (BootTimes <= 0)
            {
                page1.Desc = "你是第一次启动哦~~~";
                BootTimes = 1;
            }
            else
            {
                page1.Desc = string.Format("本次是你第{0}次启动本程序", BootTimes);
            } 
            EAIntroPage page2 = new EAIntroPage();
            page2.Title = "Page2";
            page2.Desc = "Hello World   Page2 no Description";
            page2.BgColor = UIColor.Red;

            EAIntroPage page3 = new EAIntroPage();
            page3.Title = "Page3";
            page3.Desc = "Hello World   Page3 no Description";
            page3.BgImage = UIImage.FromBundle("Visual-Studio.jpg");

            //欢迎界面
            EAIntroView introView = new EAIntroView(this.View.Frame, new []{ page1, page2, page3 });
            //显示欢迎界面
            introView.ShowInView(this.View);

            //将启动次数增加1，并保存在配置文件中
            AppSettings.AddOrUpdateValue("BootTimes", ++BootTimes); 
        }
    }
}

