﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SimpleLyricsEditor.Models;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SimpleLyricsEditor.Views.SettingsPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsRootPage : Page
    {
        private readonly List<PageModel> pms;
        public SettingsRootPage()
        {
            this.InitializeComponent();
            pms = new List<PageModel>();
            pms.Add(new PageModel("背景设置", typeof(BackgroundSettingsPage)));
        }

        private void SettingsRootPage_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (PageModel model in pms)
            {
                Frame frame = new Frame();
                frame.Navigate(model.PageType, model.Title);
                Root_StackPanel.Children.Add(frame);
            }
        }
    }
}
