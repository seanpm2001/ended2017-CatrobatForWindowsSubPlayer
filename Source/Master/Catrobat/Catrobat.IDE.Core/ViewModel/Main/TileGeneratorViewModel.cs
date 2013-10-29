﻿using Catrobat.IDE.Core.CatrobatObjects;
using Catrobat.IDE.Core.Services;
using GalaSoft.MvvmLight.Messaging;

namespace Catrobat.IDE.Core.ViewModel.Main
{
    public class TileGeneratorViewModel : ViewModelBase
    {
        #region private Members

        private ProjectDummyHeader _pinProjectHeader;

        #endregion

        #region Properties
        public ProjectDummyHeader PinProjectHeader
        {
            get
            {
                return _pinProjectHeader;
            }
            set
            {
                if (value == _pinProjectHeader) return;

                _pinProjectHeader = value;

                RaisePropertyChanged(() => PinProjectHeader);
            }
        }

        #endregion

        #region Commands


        #endregion

        #region Actions

        protected override void GoBackAction()
        {
            ResetViewModel();
            ServiceLocator.NavigationService.NavigateBack();
        }

        #endregion

        #region MessageActions

        private void PinProjectHeaderChangedAction(GenericMessage<ProjectDummyHeader> message)
        {
            PinProjectHeader = message.Content;
        }

        #endregion

        public TileGeneratorViewModel()
        {
            Messenger.Default.Register<GenericMessage<ProjectDummyHeader>>(this,
                 ViewModelMessagingToken.PinProjectHeaderListener, PinProjectHeaderChangedAction);
        }


        private void ResetViewModel()
        {
            
        }
    }
}