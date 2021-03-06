﻿using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using SimpleLyricsEditor.Events;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SimpleLyricsEditor.Control
{
    public sealed partial class BlurBackgroundImage : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", typeof(BitmapSource), typeof(BlurBackgroundImage), new PropertyMetadata(null));

        public static readonly DependencyProperty BlurDegreeProperty = DependencyProperty.Register(
            "BlurDegree", typeof(double), typeof(BlurBackgroundImage), new PropertyMetadata(5D));

        public BlurBackgroundImage()
        {
            InitializeComponent();
        }

        public BitmapSource Source
        {
            get => (BitmapSource) GetValue(SourceProperty);
            set
            {
                SetValue(SourceProperty, value);

                FadeOut.Begin();
            } 
        }

        public double BlurDegree
        {
            get => (double) GetValue(BlurDegreeProperty);
            set => SetValue(BlurDegreeProperty, value);
        }
        
        private void FadeOut_Completed(object sender, object e)
        {
            Image.Source = Source;
            FadeIn.Begin();
        }
    }
}