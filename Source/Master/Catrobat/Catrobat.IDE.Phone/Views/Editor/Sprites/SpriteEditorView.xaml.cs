﻿using System.Windows;
using System.Windows.Navigation;
using Catrobat.IDE.Core.Services;
using Catrobat.IDE.Core.ViewModel;
using Catrobat.IDE.Core.ViewModel.Editor.Sprites;
using Microsoft.Phone.Controls;

namespace Catrobat.IDE.Phone.Views.Editor.Sprites
{
    public partial class SpriteEditorView : PhoneApplicationPage
    {
        private readonly SpriteEditorViewModel _viewModel = ((ViewModelLocator)ServiceLocator.ViewModelLocator).SpriteEditorViewModel;


        public SpriteEditorView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _viewModel.GoBackCommand.Execute(null);
        }

        private void reorderListBoxScriptBricks_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedBrick != null)
            {
                ReorderListBoxScriptBricks.ScrollIntoView(_viewModel.SelectedBrick);
                _viewModel.SelectedBrick = null;
            }
        }
    }
}
