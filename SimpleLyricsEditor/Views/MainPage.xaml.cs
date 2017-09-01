﻿using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SimpleLyricsEditor.DAL;
using SimpleLyricsEditor.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimpleLyricsEditor.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainViewModel _viewModel;
        public MainPage()
        {
            this.InitializeComponent();
            _viewModel = this.DataContext as MainViewModel;
            _viewModel.SelectedItems = Lyrics_ListView.SelectedItems;
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Add(Lyrics_ListView.SelectedIndex, Player.Position, false);
        }

        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Remove();
        }

        private void Move_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Move(Player.Position);
        }
        
        private void Modify_Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Modify();
        }
    }
}
