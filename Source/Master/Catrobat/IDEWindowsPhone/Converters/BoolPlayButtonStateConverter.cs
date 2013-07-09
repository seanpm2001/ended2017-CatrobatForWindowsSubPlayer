﻿using System;
using System.Globalization;
using System.Windows.Data;
using Catrobat.IDEWindowsPhone.Controls.Buttons;

namespace Catrobat.IDEWindowsPhone.Converters
{
    public class BoolPlayButtonStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return PlayPauseButtonState.Pause;

            var isPlaying = (bool)value;

            return isPlaying ? PlayPauseButtonState.Play : PlayPauseButtonState.Pause;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is PlayPauseButtonState))
                return false;

            var playState = (PlayPauseButtonState)value;

            return playState == PlayPauseButtonState.Play;
        }
    }
}