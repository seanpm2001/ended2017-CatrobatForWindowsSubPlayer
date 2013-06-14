﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using Catrobat.Core;
using Catrobat.Core.Objects;
using Catrobat.Core.Objects.Sounds;
using Catrobat.Core.Storage;
using Catrobat.IDEWindowsPhone.Annotations;
using Catrobat.IDEWindowsPhone.Views.Editor.Sounds;
using GalaSoft.MvvmLight;
using System.ComponentModel;
using GalaSoft.MvvmLight.Command;
using IDEWindowsPhone;
using Microsoft.Phone.Controls;
using Microsoft.Practices.ServiceLocation;
using Catrobat.IDECommon.Resources.Editor;
using Microsoft.Phone.Tasks;
using Catrobat.IDEWindowsPhone.Misc;
using GalaSoft.MvvmLight.Messaging;
using Catrobat.IDEWindowsPhone.Views.Editor.Costumes;
using Catrobat.Core.Objects.Costumes;

namespace Catrobat.IDEWindowsPhone.ViewModel
{
    public class AddNewCostumeViewModel : ViewModelBase
    {
        private readonly EditorViewModel _editorViewModel = ServiceLocator.Current.GetInstance<EditorViewModel>();

        #region Private Members

        private string _costumeName;
        private CostumeBuilder _builder;
        private Sprite _selectedSprite;

        #endregion

        #region Commands

        public RelayCommand OpenGalleryCommand
        {
            get;
            private set;
        }

        public RelayCommand OpenCameraCommand
        {
            get;
            private set;
        }

        #endregion

        #region Actions

        private void OpenGalleryAction()
        {
            lock (this)
            {
                var photoChooserTask = new PhotoChooserTask();
                photoChooserTask.Completed -= Task_Completed;
                photoChooserTask.Completed += Task_Completed;
                photoChooserTask.Show();
            }
        }

        private void OpenCameraAction()
        {
            lock (this)
            {
                var cameraCaptureTask = new CameraCaptureTask();
                cameraCaptureTask.Completed -= Task_Completed;
                cameraCaptureTask.Completed += Task_Completed;
                cameraCaptureTask.Show();
            }
        }

        private void SaveAction()
        {
            var costume = _builder.Save(CostumeName);
            _selectedSprite.Costumes.Costumes.Add(costume);

            Cleanup();
            Navigation.RemoveBackEntry();
            Navigation.NavigateBack();
        }

        private void CancelAction()
        {
            Cleanup();
            Navigation.NavigateBack();
        }

        private void ReceiveSelectedSpriteMessageAction(GenericMessage<Sprite> message)
        {
            _selectedSprite = message.Content;
        }


        #endregion

        #region Events

        public void SaveEvent()
        {
            SaveAction();
        }

        public void CancelEvent()
        {
            CancelAction();
        }


        #endregion

        #region Properties

        public string CostumeName
        {
            get { return _costumeName; }
            set
            {
                if (value == _costumeName) return;
                _costumeName = value;
                RaisePropertyChanged("CostumeName");
                RaisePropertyChanged("IsCostumeNameValid");
            }
        }

        public bool IsCostumeNameValid
        {
            get
            {
                return CostumeName != null && CostumeName.Length >= 2;
            }
        }

        #endregion

        public AddNewCostumeViewModel()
        {
            OpenGalleryCommand = new RelayCommand(OpenGalleryAction);
            OpenCameraCommand = new RelayCommand(OpenCameraAction);

            Messenger.Default.Register<GenericMessage<Sprite>>(this, ViewModelMessagingToken.AddNewCostumeViewModel, ReceiveSelectedSpriteMessageAction);
        }

        public override void Cleanup()
        {
            _costumeName = null;
            _selectedSprite = null;

            if (_builder != null)
            {
                _builder.LoadCostumeSuccess -= LoadCostumeSuccess;
                _builder.LoadCostumeFailed -= LoadCostumeFailed;
                _builder = null;
            }

            base.Cleanup();
        }

        private void Task_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                CostumeName = EditorResources.Image;

                _builder = new CostumeBuilder();
                _builder.LoadCostumeSuccess += LoadCostumeSuccess;
                _builder.LoadCostumeFailed += LoadCostumeFailed;

                _builder.StartCreateCostumeAsync(_selectedSprite, e.ChosenPhoto);
            }
        }

        private void LoadCostumeSuccess()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                Navigation.NavigateTo(typeof(CostumeNameChooserView));
            });
        }

        private void LoadCostumeFailed()
        {
            var message = new DialogMessage(EditorResources.MessageBoxWrongImageFormatText, WrongImageFormatResult)
            {
                Button = MessageBoxButton.OK,
                Caption = EditorResources.MessageBoxWrongImageFormatHeader
            };
            Messenger.Default.Send(message);
        }

        private void WrongImageFormatResult(MessageBoxResult result)
        {
            Navigation.NavigateBack();
        }
    }
}