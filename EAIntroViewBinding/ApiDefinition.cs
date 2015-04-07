using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace EAIntroView
{
    // typedef void (^VoidBlock)();
    delegate void VoidBlock();

    // @interface EAIntroPage : NSObject
    [BaseType(typeof(NSObject))]
    interface EAIntroPage
    {
        // @property (nonatomic, strong) UIImage * bgImage;
        [Export("bgImage", ArgumentSemantic.Strong)]
        UIImage BgImage { get; set; }

        // @property (nonatomic, strong) UIColor * bgColor;
        [Export("bgColor", ArgumentSemantic.Strong)]
        UIColor BgColor { get; set; }

        // @property (assign, nonatomic) BOOL showTitleView;
        [Export("showTitleView")]
        bool ShowTitleView { get; set; }

        // @property (nonatomic, strong) UIView * titleIconView;
        [Export("titleIconView", ArgumentSemantic.Strong)]
        UIView TitleIconView { get; set; }

        // @property (assign, nonatomic) CGFloat titleIconPositionY;
        [Export("titleIconPositionY", ArgumentSemantic.Assign)]
        nfloat TitleIconPositionY { get; set; }

        // @property (nonatomic, strong) NSString * title;
        [Export("title", ArgumentSemantic.Strong)]
        string Title { get; set; }

        // @property (nonatomic, strong) UIFont * titleFont;
        [Export("titleFont", ArgumentSemantic.Strong)]
        UIFont TitleFont { get; set; }

        // @property (nonatomic, strong) UIColor * titleColor;
        [Export("titleColor", ArgumentSemantic.Strong)]
        UIColor TitleColor { get; set; }

        // @property (assign, nonatomic) CGFloat titlePositionY;
        [Export("titlePositionY", ArgumentSemantic.Assign)]
        nfloat TitlePositionY { get; set; }

        // @property (nonatomic, strong) NSString * desc;
        [Export("desc", ArgumentSemantic.Strong)]
        string Desc { get; set; }

        // @property (nonatomic, strong) UIFont * descFont;
        [Export("descFont", ArgumentSemantic.Strong)]
        UIFont DescFont { get; set; }

        // @property (nonatomic, strong) UIColor * descColor;
        [Export("descColor", ArgumentSemantic.Strong)]
        UIColor DescColor { get; set; }

        // @property (assign, nonatomic) CGFloat descWidth;
        [Export("descWidth", ArgumentSemantic.Assign)]
        nfloat DescWidth { get; set; }

        // @property (assign, nonatomic) CGFloat descPositionY;
        [Export("descPositionY", ArgumentSemantic.Assign)]
        nfloat DescPositionY { get; set; }

        // @property (nonatomic, strong) NSArray * subviews;
        [Export("subviews", ArgumentSemantic.Strong)]
        //        [Verify(StronglyTypedNSArray)]
        NSObject[] Subviews { get; set; }

        // @property (assign, nonatomic) CGFloat alpha;
        [Export("alpha", ArgumentSemantic.Assign)]
        nfloat Alpha { get; set; }

        // @property (copy, nonatomic) VoidBlock onPageDidLoad;
        [Export("onPageDidLoad", ArgumentSemantic.Copy)]
        VoidBlock OnPageDidLoad { get; set; }

        // @property (copy, nonatomic) VoidBlock onPageDidAppear;
        [Export("onPageDidAppear", ArgumentSemantic.Copy)]
        VoidBlock OnPageDidAppear { get; set; }

        // @property (copy, nonatomic) VoidBlock onPageDidDisappear;
        [Export("onPageDidDisappear", ArgumentSemantic.Copy)]
        VoidBlock OnPageDidDisappear { get; set; }

        // @property (nonatomic, strong) UIView * customView;
        [Export("customView", ArgumentSemantic.Strong)]
        UIView CustomView { get; set; }

        // @property (readonly, nonatomic, strong) UIView * pageView;
        [Export("pageView", ArgumentSemantic.Strong)]
        UIView PageView { get; }

        // +(instancetype)page;
        [Static]
        [Export("page")]
        EAIntroPage Page();

        // +(instancetype)pageWithCustomView:(UIView *)customV;
        [Static]
        [Export("pageWithCustomView:")]
        EAIntroPage PageWithCustomView(UIView customV);

        // +(instancetype)pageWithCustomViewFromNibNamed:(NSString *)nibName;
        [Static]
        [Export("pageWithCustomViewFromNibNamed:")]
        EAIntroPage PageWithCustomViewFromNibNamed(string nibName);
    }

    // @interface EARestrictedScrollView : UIScrollView
    [BaseType(typeof(UIScrollView))]
    interface EARestrictedScrollView
    {
        // @property (assign, nonatomic) CGRect restrictionArea;
        [Export("restrictionArea", ArgumentSemantic.Assign)]
        CGRect RestrictionArea { get; set; }

        // @property (getter = alignedContentOffset, assign, nonatomic, setter = setAlignedContentOffset:) CGPoint contentOffset;
        [Export("contentOffset", ArgumentSemantic.Assign)]
        CGPoint AlignedContentOffset { [Bind ("alignedContentOffset")] get; [Bind ("setAlignedContentOffset:")] set; }

        // -(CGPoint)contentOffset __attribute__((unavailable("use dot notation to access property")));
        // -(void)setContentOffset:(CGPoint)contentOffset __attribute__((unavailable("use dot notation to access property")));
        [Export("contentOffset")]
        //        [Verify(MethodToProperty)]
        CGPoint ContentOffset { get; set; }
    }

    // @protocol EAIntroDelegate <NSObject>
    [Protocol, Model]
    [BaseType(typeof(NSObject))]
    interface EAIntroDelegate
    {
        // @optional -(void)introDidFinish:(EAIntroView *)introView;
        [Export("introDidFinish:")]
        void IntroDidFinish(EAIntroView introView);

        // @optional -(void)intro:(EAIntroView *)introView pageAppeared:(EAIntroPage *)page withIndex:(NSUInteger)pageIndex;
        [Export("intro:pageAppeared:withIndex:")]
        void IntroAppeared(EAIntroView introView, EAIntroPage page, nuint pageIndex);

        // @optional -(void)intro:(EAIntroView *)introView pageStartScrolling:(EAIntroPage *)page withIndex:(NSUInteger)pageIndex;
        [Export("intro:pageStartScrolling:withIndex:")]
        void IntroStartScrolling(EAIntroView introView, EAIntroPage page, nuint pageIndex);

        // @optional -(void)intro:(EAIntroView *)introView pageEndScrolling:(EAIntroPage *)page withIndex:(NSUInteger)pageIndex;
        [Export("intro:pageEndScrolling:withIndex:")]
        void IntroEndScrolling(EAIntroView introView, EAIntroPage page, nuint pageIndex);
    }

    // @interface EAIntroView : UIView <UIScrollViewDelegate>
    [BaseType(typeof(UIView))]
    interface EAIntroView : IUIScrollViewDelegate
    {
        [Wrap("WeakDelegate")]
        EAIntroDelegate Delegate { get; set; }

        // @property (nonatomic, weak) id<EAIntroDelegate> delegate;
        [NullAllowed, Export("delegate", ArgumentSemantic.Weak)]
        NSObject WeakDelegate { get; set; }

        // @property (assign, nonatomic) BOOL swipeToExit;
        [Export("swipeToExit")]
        bool SwipeToExit { get; set; }

        // @property (assign, nonatomic) BOOL tapToNext;
        [Export("tapToNext")]
        bool TapToNext { get; set; }

        // @property (assign, nonatomic) BOOL hideOffscreenPages;
        [Export("hideOffscreenPages")]
        bool HideOffscreenPages { get; set; }

        // @property (assign, nonatomic) BOOL easeOutCrossDisolves;
        [Export("easeOutCrossDisolves")]
        bool EaseOutCrossDisolves { get; set; }

        // @property (assign, nonatomic) BOOL useMotionEffects;
        [Export("useMotionEffects")]
        bool UseMotionEffects { get; set; }

        // @property (assign, nonatomic) CGFloat motionEffectsRelativeValue;
        [Export("motionEffectsRelativeValue", ArgumentSemantic.Assign)]
        nfloat MotionEffectsRelativeValue { get; set; }

        // @property (nonatomic, strong) UIView * titleView;
        [Export("titleView", ArgumentSemantic.Strong)]
        UIView TitleView { get; set; }

        // @property (assign, nonatomic) CGFloat titleViewY;
        [Export("titleViewY", ArgumentSemantic.Assign)]
        nfloat TitleViewY { get; set; }

        // @property (nonatomic, strong) UIImage * bgImage;
        [Export("bgImage", ArgumentSemantic.Strong)]
        UIImage BgImage { get; set; }

        // @property (assign, nonatomic) UIViewContentMode bgViewContentMode;
        [Export("bgViewContentMode", ArgumentSemantic.Assign)]
        UIViewContentMode BgViewContentMode { get; set; }

        // @property (nonatomic, strong) UIPageControl * pageControl;
        [Export("pageControl", ArgumentSemantic.Strong)]
        UIPageControl PageControl { get; set; }

        // @property (assign, nonatomic) CGFloat pageControlY;
        [Export("pageControlY", ArgumentSemantic.Assign)]
        nfloat PageControlY { get; set; }

        // @property (assign, nonatomic) NSUInteger currentPageIndex;
        [Export("currentPageIndex", ArgumentSemantic.Assign)]
        nuint CurrentPageIndex { get; set; }

        // @property (readonly, assign, nonatomic) NSUInteger visiblePageIndex;
        [Export("visiblePageIndex", ArgumentSemantic.Assign)]
        nuint VisiblePageIndex { get; }

        // @property (nonatomic, strong) UIButton * skipButton;
        [Export("skipButton", ArgumentSemantic.Strong)]
        UIButton SkipButton { get; set; }

        // @property (assign, nonatomic) CGFloat skipButtonY;
        [Export("skipButtonY", ArgumentSemantic.Assign)]
        nfloat SkipButtonY { get; set; }

        // @property (assign, nonatomic) CGFloat skipButtonSideMargin;
        [Export("skipButtonSideMargin", ArgumentSemantic.Assign)]
        nfloat SkipButtonSideMargin { get; set; }

        // @property (assign, nonatomic) EAViewAlignment skipButtonAlignment;
        [Export("skipButtonAlignment", ArgumentSemantic.Assign)]
        EAViewAlignment SkipButtonAlignment { get; set; }

        // @property (assign, nonatomic) BOOL showSkipButtonOnlyOnLastPage;
        [Export("showSkipButtonOnlyOnLastPage")]
        bool ShowSkipButtonOnlyOnLastPage { get; set; }

        // @property (nonatomic, strong) EARestrictedScrollView * scrollView;
        [Export("scrollView", ArgumentSemantic.Strong)]
        EARestrictedScrollView ScrollView { get; set; }

        // @property (assign, nonatomic) BOOL scrollingEnabled;
        [Export("scrollingEnabled")]
        bool ScrollingEnabled { get; set; }

        // @property (nonatomic, strong) NSArray * pages;
        [Export("pages", ArgumentSemantic.Strong)]
        //        [Verify(StronglyTypedNSArray)]
        NSObject[] Pages { get; set; }

        // -(id)initWithFrame:(CGRect)frame andPages:(NSArray *)pagesArray;
        [Export("initWithFrame:andPages:")]
        //        [Verify(StronglyTypedNSArray)]
        IntPtr Constructor(CGRect frame, NSObject[] pagesArray);

        // -(void)showFullscreen;
        [Export("showFullscreen")]
        void ShowFullscreen();

        // -(void)showFullscreenWithAnimateDuration:(CGFloat)duration;
        [Export("showFullscreenWithAnimateDuration:")]
        void ShowFullscreenWithAnimateDuration(nfloat duration);

        // -(void)showFullscreenWithAnimateDuration:(CGFloat)duration andInitialPageIndex:(NSUInteger)initialPageIndex;
        [Export("showFullscreenWithAnimateDuration:andInitialPageIndex:")]
        void ShowFullscreenWithAnimateDuration(nfloat duration, nuint initialPageIndex);

        // -(void)showInView:(UIView *)view;
        [Export("showInView:")]
        void ShowInView(UIView view);

        // -(void)showInView:(UIView *)view animateDuration:(CGFloat)duration;
        [Export("showInView:animateDuration:")]
        void ShowInView(UIView view, nfloat duration);

        // -(void)showInView:(UIView *)view animateDuration:(CGFloat)duration withInitialPageIndex:(NSUInteger)initialPageIndex;
        [Export("showInView:animateDuration:withInitialPageIndex:")]
        void ShowInView(UIView view, nfloat duration, nuint initialPageIndex);

        // -(void)hideWithFadeOutDuration:(CGFloat)duration;
        [Export("hideWithFadeOutDuration:")]
        void HideWithFadeOutDuration(nfloat duration);

        // -(void)setCurrentPageIndex:(NSUInteger)currentPageIndex animated:(BOOL)animated;
        [Export("setCurrentPageIndex:animated:")]
        void SetCurrentPageIndex(nuint currentPageIndex, bool animated);

        // -(void)limitScrollingToPage:(NSUInteger)lastPageIndex;
        [Export("limitScrollingToPage:")]
        void LimitScrollingToPage(nuint lastPageIndex);
    }
}
