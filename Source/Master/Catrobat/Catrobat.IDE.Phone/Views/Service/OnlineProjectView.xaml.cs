﻿using System.Windows;
using System.Windows.Navigation;
using Catrobat.IDE.Core.CatrobatObjects;
using Catrobat.IDE.Core.Services;
using Catrobat.IDE.Core.ViewModel;
using Catrobat.IDE.Core.ViewModel.Service;
using Microsoft.Phone.Controls;

namespace Catrobat.IDE.Phone.Views.Service
{
    public partial class OnlineProjectView : PhoneApplicationPage
    {
        private readonly OnlineProjectViewModel _viewModel = 
            ((ViewModelLocator)ServiceLocator.ViewModelLocator).OnlineProjectViewModel;

        public OnlineProjectView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _viewModel.GoBackCommand.Execute(null);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.OnLoadCommand.Execute((OnlineProjectHeader) DataContext);
        }
    }
}