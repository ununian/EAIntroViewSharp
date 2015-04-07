# Xamarin.iOS - 利用Settings插件与EAIntroView制作App的欢迎界面

------

##关于欢迎界面
很多App第一次启动都会有一个欢迎界面，欢迎界面往往决定这用户对App的第一映像，所以欢迎界面的重要性不言而喻。QQ、微博、知乎等App都有制作精良的欢迎界面。 
    
大多数欢迎界面由几个界面组成，通常界面上会有一张背景图和简单的介绍文字，页面直接的切换类似于Android的ViewPager一样，靠左右滑动来切换。通常会提供了一个Skip按钮来让用户跳过欢迎界面。

本文将告诉你如何制作一个欢迎界面。

##需要用到的第三方库
>*  [EAIntroView][1]（Object-C写的欢迎界面第三方库）
  https://github.com/ealeksandrov/EAIntroView
>*  [Settings插件][2] (JamesMontemagno制作的插件之一，用来记录App的设置数据）
  https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/Settings
  
------
##一、绑定EAIntroView
为了使用EAIntroView我们首先需要将源生的Ojbect-C库绑定成Xamarin能用的程序集。
Xamarin绑定请参考Xamarin官网的教程，我只会讲主要的操作和贴一些关键的代码。
>http://developer.xamarin.com/guides/ios/advanced_topics/binding_objective-c/

在有了Objective Sharpie后绑定已经比较方便了，基本上只要稍微修改下自动生成ApiDefinitions文件即可。

###1.获取EAIntroView源代码

用git命令将EAIntroView克隆下来，并稍微浏览下源生的Ojbect-C代码
```
git clone https://github.com/ealeksandrov/EAIntroView.git
```

###2.生成静态库

在XCode中建立新的iOS Cocoa Touch Static Library，名字叫做EAIntroViewStatic。

将EAIntroView的源代码文件（EAIntroView文件夹中，共4个）复制到XCode的工程中。

按下Command+B编译，我们会发现提示缺少了EARestrictedScrollView相关的文件。这是因为EAIntroView依赖于EARestrictedScrollView造成的，EARestrictedScrollView是EAIntroView的作者的另一个第三方库。

和EAIntroView一样，在源生开发中也是利用CocoaPod将源代码文件引用到当前工程中的，所以我们到Example\Pods\EARestrictedScrollView文件夹中将EARestrictedScrollView的源代码复制到我们的工程中来。再修改下头文件的引用就OK了。

再次按下Command+B就提示Build Successed了。

当目标平台为iOS Device时会显示Build Faild，为了在真机中可以使用，我们需要进行签名。
点击工程，就可以进入设置界面，在Build Setting中的Code Signing Idtntity中选择iOS Developer。
![代码签名][3]

###3.制作模拟器与真机都能使用的通用类库

然后我们需要将.a文件制作成通用类库
>参考[这篇文章][4]
>官网的绑定教程中也有提及

为了方便我给出Makefile，按照上述操作进行过代码签名后可以用make命令方便的生成模拟器和真机（32位、64位）都可以使用的.a文件，如果你开始和我的工程名不一样的话请注意修改。

```makefile
XBUILD=/Applications/Xcode.app/Contents/Developer/usr/bin/xcodebuild
PROJECT_ROOT=.
PROJECT=$(PROJECT_ROOT)/EAIntroViewStatic.xcodeproj
TARGET=EAIntroViewStatic

all: libEAIntroView.a

libEAIntroView-i386.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphonesimulator -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphonesimulator/lib$(TARGET).a $@

libEAIntroView-armv7.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphoneos -arch armv7 -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphoneos/lib$(TARGET).a $@

libEAIntroView-arm64.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphoneos -arch arm64 -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphoneos/lib$(TARGET).a $@

libEAIntroView.a: libEAIntroView-armv7.a libEAIntroView-i386.a libEAIntroView-arm64.a
	lipo -create -output $@ $^

clean:
	-rm -f *.a *.dll
```

libEAIntroView.a文件就是最终的生成结果。


##4.利用Objective Sharpie工具进行绑定

首先还是在Xamarin中建立iOS Binding Project。

将刚刚生成的.a文件拖入到工程中，并修改linkWith描述文件

```csharp
using System;
using ObjCRuntime;

[assembly: LinkWith("libEAIntroView.a", LinkTarget.ArmV7 | LinkTarget.Simulator | LinkTarget.Arm64 | LinkTarget.ArmV7s | LinkTarget.Simulator64, SmartLink = true, ForceLoad = true)]
```

然后用Objective Sharpie将Object-C的头文件翻译成ApiDefinitions
具体的教程我也不写了，官网已经非常详细了，下面是要执行的命令

```sh
sharpie bind --output=EAIntroView --namespace=EAIntroView --sdk=iphoneos8.2  [项目的绝对路径]/EAIntroViewStatic/*.h
```

生成ApiDefinitions.cs和StructsAndEnums.cs后覆盖Binding项目的同名文件。

然后还需要进行少量的修改，主要是同名函数的问题（Object-C的函数名由函数名+参数名决定，所以当函数名相同而参数名不同时C#没办法分辨，只要改改函数名就行），还有几个提示是需要需要用强类型替换NObject类型，这个我们可以先不管。

ApiDefinitions.cs文件太长我就不贴了，到时候会放在Github上。

至此我们生成了Xamarin能使用的dll文件。

------
##二、Settings插件的使用

###1.安装Settings插件

有2种方式
>* [通过Nuget包管理器下载][5]
>* [通过源代码自己编译][6] 

这里我们用Nuget省事，在Nuget命令行中输入如下的命令即可。
```powershell
Install-Package Xam.Plugins.Settings
```
另外iOS需要这样设置下，启用Generic Value Type Sharing
 ![iOS额外设置][7]
  
###2.基本教程

参考
>* http://components.xamarin.com/gettingstarted/settingsplugin
>* https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/Settings

主要是CrossSettings.Current对象和它的2个函数GetValueOrDefault、AddOrUpdateValue，这2个函数的功能看名字应该就非常清楚了。

```csharp
// 从设置中获取指定Key的值，并转换成相应的类型。
GetValueOrDefault<T>(string key);

// 向设置中添加制定key的值，如果已存在key则是更新当前值。
AddOrUpdateValue<T>(string key,T value);
```

设置的生命周期与应用程序一样，当应用程序被卸载时清空。

------
##三、实例 

###1.新建工程

>* 在刚刚的Binding Project的解决方案中新建一个iOS的SingleView工程，工程名为EAintroView.Sample。

###2.添加引用
>* 通过Edit References引用绑定工程。
>* 通过Nuget引用Settings插件

###3.修改EAIntroView_SampleViewController文件如下：

```csharp
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
```
效果如下： 


 ![例子][8]

当第一次运行时，第一个界面显示首次运行本程序，当在后台关闭程序后再打开界面会显示是第二次打开本程序。

有关EAIntroView的详细配置请参考Github的原项目，样式还是挺多的。

------
##四、总结
本文主要描述了

>*  Settings插件的使用
>*  绑定了一个叫EAintroView的iOS第三方库
>*  利用以上2点制作了一个简单欢迎界面 
> 相关源代码在 https://github.com/unhappy224/EAIntroViewSharp
> 如有疑问可以写在评论中，或者联系我：unhappy224@163.com  QQ:104228916 
> 欢迎加入QQ群：230865920

  [1]:https://github.com/ealeksandrov/EAIntroView
  [2]:https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/Settings
  [3]: http://images.cnblogs.com/cnblogs_com/visualakari/675805/o_CD41BB4B-F44B-408A-8910-DFEBBF64A982.png
  [4]:http://blog.csdn.net/u012069227/article/details/40743011
  [5]:https://www.nuget.org/packages/Xam.Plugins.Settings/
  [6]:https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/Settings
  [7]: https://camo.githubusercontent.com/cd575cbb9534cf7db017b7c924b122da1b0b4852/687474703a2f2f636f6e74656e742e73637265656e636173742e636f6d2f75736572732f4a616d65734d6f6e74656d61676e6f2f666f6c646572732f4a696e672f6d656469612f37343636626361362d613931362d346664392d393330312d3363333430336433613661642f30303030303039372e706e67
  [8]: http://images.cnblogs.com/cnblogs_com/visualakari/675805/o_a.gif
