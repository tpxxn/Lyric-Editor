﻿using LyricsEditor.Auxiliary;
using LyricsEditor.Information;
using LyricsEditor.Model;
using LyricsEditor.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinRTExceptions;

namespace LyricsEditor
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        public static bool isPressCtrl = false;
        public static bool isPressShift = false;

        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;
        }

        private void Window_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.Control)
                App.isPressCtrl = true;
            if (args.VirtualKey == VirtualKey.Shift)
                App.isPressShift = true;
        }

        private void Window_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.Control)
                App.isPressCtrl = false;
            if (args.VirtualKey == VirtualKey.Shift)
                App.isPressShift = false;
        }

        private async void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await ShowErrorReport(e.Exception);
        }


        private async System.Threading.Tasks.Task ShowErrorReport(Exception e)
        {
            var error = new Error();
            error.Code = "0x" + e.HResult.ToString("X");
            error.Source = e.Source;
            error.Content = e.Message;
            error.StackTrace = e.StackTraceEx();
            var buttons = new Dictionary<string, UICommandInvokedHandler>();
            string report = $"请说明你做了什么\nPlease introduce what you do\n\n\n\n{SystemInfo.PrintInfo()}\n{Information.AppInfo.PrintInfo()}\n{error.ToString()}";
            report = report.Replace("\n", "%0A");
            buttons.Add(CharacterLibrary.MessageBox.GetString("EmailErrorReport"),
                async u => await Launcher.LaunchUriAsync(new Uri($"mailto:kljzndx@outlook.com?subject=Simple Lyric Editor Error Report&body={report}"))
            );
            await MessageBox.ShowMessageBoxAsync("Error", error.Content, buttons, "关闭");
        }

        private void EnsureSyncContext()
        {
            ExceptionHandlingSynchronizationContext.Register().UnhandledException += OnSynchronizationContextUnhandledException;
        }

        private async void OnSynchronizationContextUnhandledException(object sender, WinRTExceptions.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await ShowErrorReport(e.Exception);
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;

            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }
            EnsureSyncContext();
            Window.Current.CoreWindow.KeyDown += Window_KeyDown;
            Window.Current.CoreWindow.KeyUp += Window_KeyUp;
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            EnsureSyncContext();
        }
        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            var frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                frame = new Frame();
                frame.NavigationFailed += OnNavigationFailed;
                Window.Current.Content = frame;
                Window.Current.CoreWindow.KeyDown += Window_KeyDown;
                Window.Current.CoreWindow.KeyUp += Window_KeyUp;
            }
            if (frame.Content == null)
            {
                frame.Navigate(typeof(MainPage));
            }
            Window.Current.Activate();
            var file = args.Files[0] as IStorageFile;
            LyricFileTools.ChanageFile(file, await LyricFileTools.ReadLyricFile(file));
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }
    }
}
