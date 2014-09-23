﻿using Catrobat.IDE.WindowsShared.Common;
using Catrobat.Paint.Phone;
using Catrobat.Paint.Phone.Command;
using Catrobat.Paint.Phone.Tool;
using Catrobat.Paint.Phone.Ui;
using Catrobat.Paint.WindowsPhone.Controls.AppBar;
using Catrobat.Paint.WindowsPhone.Tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkID=390556 dokumentiert.

namespace Catrobat.Paint.WindowsPhone.View
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class PaintingAreaView : Page
    {
        Int32 slider_thickness_textbox_last_value = 1;
        static string current_appbar = "barStandard";
        Point start_point = new Point();
        Point old_point = new Point();
        public PaintingAreaView()
        {
            this.InitializeComponent();

            PocketPaintApplication.GetInstance().RecDrawingRectangle = rectDrawRectangle;

            PocketPaintApplication.GetInstance().PaintingAreaCanvas = PaintingAreaCanvas;
            
            HardwareButtons.BackPressed +=HardwareButtons_BackPressed;

            LayoutRoot.Height = Window.Current.Bounds.Height;
            LayoutRoot.Width = Window.Current.Bounds.Width;
            PocketPaintApplication.GetInstance().PaintingAreaLayoutRoot = LayoutRoot;
            PocketPaintApplication.GetInstance().PaintingAreaCanvasUnderlaying = PaintingAreaCanvasUnderlaying;
            PocketPaintApplication.GetInstance().PaintingAreaCheckeredGrid = PaintingAreaCheckeredGrid;
            PaintingAreaContentPanelGrid.Width = Window.Current.Bounds.Width;

            PaintingAreaContentPanelGrid.Height = Window.Current.Bounds.Height - 144;
            PaintingAreaContentPanelGrid.Width = Window.Current.Bounds.Width;
            PocketPaintApplication.GetInstance().PaintingAreaContentPanelGrid = PaintingAreaContentPanelGrid;
            PocketPaintApplication.GetInstance().PaintingAreaView = this;

            Spinner.SpinnerGrid = SpinnerGrid;
            Spinner.SpinnerStoryboard = new Storyboard();

            PocketPaintApplication.GetInstance().MainGrid = LayoutRoot;
            UndoRedoActionbarManager.GetInstance().ApplicationBarTop = PocketPaintApplication.GetInstance().AppbarTop;

            btnRoundImage.Click += PocketPaintApplication.GetInstance().ApplicationBarListener.RoundButton_OnClick;
            btnTriangleImage.Click += PocketPaintApplication.GetInstance().ApplicationBarListener.TriangleButton_OnClick;
            btnSquareImage.Click += PocketPaintApplication.GetInstance().ApplicationBarListener.SquareButton_OnClick;
            PocketPaintApplication.GetInstance().PaintData.ToolCurrentChanged += ToolChangedHere;
            SliderThickness.ValueChanged += SliderThickness_ValueChanged;
            SliderThickness.Value = PocketPaintApplication.GetInstance().PaintData.ThicknessSelected;
            PocketPaintApplication.GetInstance().AppbarTop.ToolChangedHere(PocketPaintApplication.GetInstance().ToolCurrent);
            PaintingAreaCanvas.ManipulationStarted += PocketPaintApplication.GetInstance().PaintingAreaManipulationListener.ManipulationStarted;
            PaintingAreaCanvas.ManipulationDelta += PocketPaintApplication.GetInstance().PaintingAreaManipulationListener.ManipulationDelta;
            PaintingAreaCanvas.ManipulationCompleted += PocketPaintApplication.GetInstance().PaintingAreaManipulationListener.ManipulationCompleted;
            PaintingAreaCanvas.ManipulationStarting += PocketPaintApplication.GetInstance().PaintingAreaManipulationListener.ManipulationStarting;
            // PaintingAreaCanvas.PointerPressed += PaintingAreaCanvas_PointerEntered;
           /* if(PocketPaintApplication.GetInstance().ToolCurrent.GetToolType() == ToolType.Brush || 
                PocketPaintApplication.GetInstance().ToolCurrent.GetToolType() == ToolType.Eraser)
            {
                LayoutRoot.PointerEntered += LayoutRoot_PointerEntered;
            }
            else
            {
                LayoutRoot.PointerEntered += null;
            }*/
            btnTools.Click += PocketPaintApplication.GetInstance().ApplicationBarListener.BtnTools_OnClick;
            btnColor.Click += PocketPaintApplication.GetInstance().ApplicationBarListener.BtnColor_Click;
            btnBrushThickness.Click += PocketPaintApplication.GetInstance().ApplicationBarListener.BtnBrushThickness_OnClick;
            btnThickness.Click += PocketPaintApplication.GetInstance().ApplicationBarListener.BtnThickness_OnClick;
            
            setPaintingAreaViewLayout();

            checkPenLineCap(PocketPaintApplication.GetInstance().PaintData.CapSelected);
            createAppBarAndSwitchAppBarContent(current_appbar);        
        }

        private void setPaintingAreaViewLayout()
        {
            double width_multiplicator = PocketPaintApplication.GetInstance().size_width_multiplication;
            double height_multiplicator = PocketPaintApplication.GetInstance().size_width_multiplication;

            uctrlOwnKeyboard.Height *= height_multiplicator;
            uctrlOwnKeyboard.Width *= width_multiplicator;

            grdOwnKeyboard.Height *= height_multiplicator;
            grdOwnKeyboard.Width *= width_multiplicator;

            foreach (Object obj in grdOwnKeyboard.Children)
            {
                if (obj.GetType() == typeof(Button))
                {
                    ((Button)obj).Height *= height_multiplicator;
                    ((Button)obj).Width *= width_multiplicator;

                    ((Button)obj).Margin = new Thickness(
                                            ((Button)obj).Margin.Left * width_multiplicator,
                                            ((Button)obj).Margin.Top * height_multiplicator,
                                            ((Button)obj).Margin.Right * width_multiplicator,
                                            ((Button)obj).Margin.Bottom * height_multiplicator);
                    ((Button)obj).FontSize *= height_multiplicator;
                }
            }

            SliderThicknessControl.Width = Window.Current.Bounds.Width;
            SliderThicknessControl.Height = Window.Current.Bounds.Height * 0.23; //Visibility="Collapsed"
            SliderThicknessGrid.Width = Window.Current.Bounds.Width;
            SliderThicknessGrid.Height = Window.Current.Bounds.Height * 0.23;
            SliderThickness.Height = SliderThicknessGrid.Height * 0.3;
            SliderThickness.Width = SliderThicknessGrid.Width * 0.6053;
            SliderThickness.Margin = new Thickness(SliderThicknessGrid.Width * 0.03, SliderThicknessGrid.Height * 0.15, 0, 0);

            btnBrushThickness.Height = SliderThicknessGrid.Height * 0.3;
            btnBrushThickness.Width = SliderThicknessGrid.Width * 0.2;
            btnBrushThickness.Margin = new Thickness(SliderThicknessGrid.Width * 0.03 + SliderThicknessGrid.Width * 0.666, SliderThicknessGrid.Height * 0.13, SliderThicknessGrid.Width * 0.03, 0);

            btnRoundImage.Height = SliderThicknessGrid.Height * 0.3;
            btnRoundImage.Width = SliderThicknessGrid.Width * 0.2;
            btnRoundImage.Margin = new Thickness(SliderThicknessGrid.Width * 0.03, 0, 0, SliderThicknessGrid.Height * 0.15);

            btnSquareImage.Height = SliderThicknessGrid.Height * 0.3;
            btnSquareImage.Width = SliderThicknessGrid.Width * 0.2;
            btnSquareImage.Margin = new Thickness(SliderThicknessGrid.Width * 0.03 + SliderThicknessGrid.Width * 0.333, 0, 0, SliderThicknessGrid.Height * 0.15);

            btnTriangleImage.Height = SliderThicknessGrid.Height * 0.3;
            btnTriangleImage.Width = SliderThicknessGrid.Width * 0.2;
            btnTriangleImage.Margin = new Thickness(SliderThicknessGrid.Width * 0.666 + SliderThicknessGrid.Width * 0.03, 0, 0, SliderThicknessGrid.Height * 0.15);
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {

            if (this.Frame.CurrentSourcePageType == typeof(PaintingAreaView))
            {
                //MessageDialog md = new MessageDialog("confirm exit?");
                //List<IUICommand> commands = new List<IUICommand>(2);
                //UICommand yes = new UICommand("Yes");
                //UICommand no = new UICommand("No");
                //commands.Add(yes);
                //commands.Add(no);
                //md.Commands.Add(yes);
                //md.Commands.Add(no);
                //var test = md.ShowAsync();

            }
            else if (this.Frame.CurrentSourcePageType == typeof(ViewColorPicker))
            {
                e.Handled = true;
                this.Frame.GoBack();
            }
            else if (this.Frame.CurrentSourcePageType == typeof(ViewToolPicker))
            {
                e.Handled = true;
                this.Frame.GoBack();
            }
        }

        /// <summary>
        /// Wird aufgerufen, wenn diese Seite in einem Frame angezeigt werden soll.
        /// </summary>
        /// <param name="e">Ereignisdaten, die beschreiben, wie diese Seite erreicht wurde.
        /// Dieser Parameter wird normalerweise zum Konfigurieren der Seite verwendet.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["PaintingAreaCanvasUnderlaying"] = PaintingAreaCanvasUnderlaying.Children;
            e.PageState["PaintingAreaCanvas"] = PaintingAreaCanvas.Children;
        }

        public void checkPenLineCap(PenLineCap pen_line_cap)
        {
            if (pen_line_cap == PenLineCap.Round)
            {
                btnRoundImage.BorderBrush = new SolidColorBrush(Colors.White);
                btnSquareImage.BorderBrush = new SolidColorBrush(Colors.Gray);
                btnTriangleImage.BorderBrush = new SolidColorBrush(Colors.Gray);
            }
            else if (pen_line_cap == PenLineCap.Square)
            {
                btnRoundImage.BorderBrush = new SolidColorBrush(Colors.Gray);
                btnSquareImage.BorderBrush = new SolidColorBrush(Colors.White);
                btnTriangleImage.BorderBrush = new SolidColorBrush(Colors.Gray);
            }
            else
            {
                btnRoundImage.BorderBrush = new SolidColorBrush(Colors.Gray);
                btnSquareImage.BorderBrush = new SolidColorBrush(Colors.Gray);
                btnTriangleImage.BorderBrush = new SolidColorBrush(Colors.White);
            }
        }

        public void createAppBarAndSwitchAppBarContent(string type)
        {
            CommandBar cmdBar = new CommandBar();
            if("barStandard" == type)
            {
                AppBarButton app_btnBrushThickness = new AppBarButton();
                AppBarButton app_btnColor = new AppBarButton();

                BitmapIcon thickness_icon = new BitmapIcon();
                thickness_icon.UriSource = new Uri("ms-resource:/Files/Assets/ColorPicker/icon_menu_strokes.png", UriKind.Absolute); 
                app_btnBrushThickness.Icon = thickness_icon;

                BitmapIcon color_icon = new BitmapIcon();
                color_icon.UriSource = new Uri("ms-resource:/Files/Assets/ColorPicker/icon_menu_color_palette.png", UriKind.Absolute);
                app_btnColor.Icon = color_icon;

                app_btnBrushThickness.Label = "Pinselstärke";
                app_btnColor.Label = "Farbe";

                app_btnBrushThickness.Click += btnThickness_Click;
                app_btnColor.Click += PocketPaintApplication.GetInstance().ApplicationBarListener.BtnColor_Click; 

                cmdBar.PrimaryCommands.Add(app_btnBrushThickness);
                cmdBar.PrimaryCommands.Add(app_btnColor);
           
            }
            else if("barPipette" == type)
            {
            

            }
            else if("barEllipse" == type)
            {
                AppBarButton app_btnBrushThickness = new AppBarButton();
                AppBarButton app_btnReset = new AppBarButton();

                BitmapIcon thickness_icon = new BitmapIcon();
                thickness_icon.UriSource = new Uri("ms-resource:/Files/Assets/ColorPicker/icon_menu_strokes.png", UriKind.Absolute);
                app_btnBrushThickness.Icon = thickness_icon;

                BitmapIcon reset_icon = new BitmapIcon();
                reset_icon.UriSource = new Uri("ms-resource:/Files/Assets/ToolMenu/icon_menu_cursor.png", UriKind.Absolute);
                app_btnReset.Icon = reset_icon;

                app_btnReset.Label = "Ausgangsposition";

                app_btnBrushThickness.Click += btnThicknessBorder_Click;
                app_btnReset.Click += app_btn_reset_Click;

                cmdBar.PrimaryCommands.Add(app_btnReset);
                cmdBar.PrimaryCommands.Add(app_btnBrushThickness);
            }
            else if("barEraser" == type)
            {
                AppBarButton app_btnBrushThickness = new AppBarButton();

                BitmapIcon thickness_icon = new BitmapIcon();
                thickness_icon.UriSource = new Uri("ms-resource:/Files/Assets/ColorPicker/icon_menu_strokes.png", UriKind.Absolute);
                app_btnBrushThickness.Icon = thickness_icon;
               
                app_btnBrushThickness.Label = "Pinselstärke";

                app_btnBrushThickness.Click += btnThickness_Click;

                cmdBar.PrimaryCommands.Add(app_btnBrushThickness);
            }
            else if ("barMove" == type)
            {
                AppBarButton app_btnZoomIn = new AppBarButton();
                AppBarButton app_btnZoomOut = new AppBarButton();
                AppBarButton app_btnReset = new AppBarButton();

                BitmapIcon zoom_in_icon = new BitmapIcon();
                zoom_in_icon.UriSource = new Uri("ms-resource:/Files/Assets/AppBar/icon_zoom_in.png", UriKind.Absolute);
                app_btnZoomIn.Icon = zoom_in_icon;

                BitmapIcon zoom_out_icon = new BitmapIcon();
                zoom_out_icon.UriSource = new Uri("ms-resource:/Files/Assets/AppBar/icon_zoom_out.png", UriKind.Absolute);
                app_btnZoomOut.Icon = zoom_out_icon;

                BitmapIcon reset_icon = new BitmapIcon();
                reset_icon.UriSource = new Uri("ms-resource:/Files/Assets/ToolMenu/icon_menu_cursor.png", UriKind.Absolute);
                app_btnReset.Icon = reset_icon;

                app_btnZoomIn.Label = "Vergrößern";
                app_btnZoomOut.Label = "Verkleinern";
                app_btnReset.Label = "Ausgangsposition";

                app_btnZoomIn.Click += BtnZoomIn_Click;
                app_btnZoomOut.Click += BtnZoomOut_Click;
                app_btnReset.Click += app_btn_reset_Click;

                cmdBar.PrimaryCommands.Add(app_btnZoomIn);
                cmdBar.PrimaryCommands.Add(app_btnZoomOut);
                cmdBar.PrimaryCommands.Add(app_btnReset);
            }
            else if("barRotate" == type)
            {
                AppBarButton app_btnRotate_left = new AppBarButton();
                AppBarButton app_btnRotate_right = new AppBarButton();
                AppBarButton app_btnReset = new AppBarButton();

                BitmapIcon rotate_left_icon = new BitmapIcon();
                rotate_left_icon.UriSource = new Uri("ms-resource:/Files/Assets/AppBar/icon_menu_rotate_left.png", UriKind.Absolute);
                app_btnRotate_left.Icon = rotate_left_icon;
                
                BitmapIcon rotate_right_icon = new BitmapIcon();
                rotate_right_icon.UriSource = new Uri("ms-resource:/Files/Assets/AppBar/icon_menu_rotate_right.png", UriKind.Absolute);
                app_btnRotate_right.Icon = rotate_right_icon;

                BitmapIcon reset_icon = new BitmapIcon();
                reset_icon.UriSource = new Uri("ms-resource:/Files/Assets/ToolMenu/icon_menu_cursor.png", UriKind.Absolute);
                app_btnReset.Icon = reset_icon;

                app_btnRotate_left.Label = "rechts drehen";
                app_btnRotate_right.Label = "links drehen";
                app_btnReset.Label = "Ausgangsposition";

                app_btnRotate_left.Click += BtnLeft_OnClick;
                app_btnRotate_right.Click += BtnRight_OnClick;
                app_btnReset.Click += app_btn_reset_Click;

                cmdBar.PrimaryCommands.Add(app_btnRotate_left);
                cmdBar.PrimaryCommands.Add(app_btnRotate_right);
                cmdBar.PrimaryCommands.Add(app_btnReset);
            }
            else if ("barRectangle" == type)
            {
                AppBarButton app_btnBrushThickness = new AppBarButton();
                AppBarButton app_btnReset = new AppBarButton();

                BitmapIcon thickness_icon = new BitmapIcon();
                thickness_icon.UriSource = new Uri("ms-resource:/Files/Assets/ColorPicker/icon_menu_strokes.png", UriKind.Absolute);
                app_btnBrushThickness.Icon = thickness_icon;

                BitmapIcon reset_icon = new BitmapIcon();
                reset_icon.UriSource = new Uri("ms-resource:/Files/Assets/ToolMenu/icon_menu_cursor.png", UriKind.Absolute);
                app_btnReset.Icon = reset_icon;

                app_btnReset.Label = "Ausgangsposition";

                app_btnBrushThickness.Click += btnThicknessBorder_Click;
                app_btnReset.Click += app_btn_reset_Click;

                cmdBar.PrimaryCommands.Add(app_btnReset);
                cmdBar.PrimaryCommands.Add(app_btnBrushThickness);
            }
            else if("barFlip" == type)
            {
                AppBarButton app_btnHorizontal = new AppBarButton();
                AppBarButton app_btnVertical = new AppBarButton();
                AppBarButton app_btnReset = new AppBarButton();

                BitmapIcon horizontal_icon = new BitmapIcon();
                horizontal_icon.UriSource = new Uri("ms-resource:/Files/Assets/AppBar/icon_menu_flip_horizontal.png", UriKind.Absolute);
                app_btnHorizontal.Icon = horizontal_icon;

                BitmapIcon vertical_icon = new BitmapIcon();
                vertical_icon.UriSource = new Uri("ms-resource:/Files/Assets/AppBar/icon_menu_flip_vertical.png", UriKind.Absolute);
                app_btnVertical.Icon = vertical_icon;

                BitmapIcon reset_icon = new BitmapIcon();
                reset_icon.UriSource = new Uri("ms-resource:/Files/Assets/ToolMenu/icon_menu_cursor.png", UriKind.Absolute);
                app_btnReset.Icon = reset_icon;

                app_btnHorizontal.Label = "horizontal";
                app_btnVertical.Label = "vertikal";
                app_btnReset.Label = "Ausgangsposition";

                app_btnHorizontal.Click += BtnHorizotal_OnClick;
                app_btnVertical.Click += BtnVertical_OnClick;
                app_btnReset.Click += app_btn_reset_Click;

                cmdBar.PrimaryCommands.Add(app_btnHorizontal);
                cmdBar.PrimaryCommands.Add(app_btnVertical);
                cmdBar.PrimaryCommands.Add(app_btnReset);
            }
            else
            {
                return;
            }
            AppBarButton app_btnTools = new AppBarButton();
            AppBarButton app_btnSave = new AppBarButton();
            AppBarButton app_btnSaveCopy = new AppBarButton();
            AppBarButton app_btnNewPicture = new AppBarButton();
            AppBarButton app_btnLoad = new AppBarButton();
            AppBarButton app_btnFullScreen = new AppBarButton();
            AppBarButton app_btnAbout = new AppBarButton();

            BitmapIcon tools_icon = new BitmapIcon();
            tools_icon.UriSource = new Uri("ms-resource:/Files/Assets/AppBar/menu_tools_.png", UriKind.Absolute);
            app_btnTools.Icon = tools_icon;
            app_btnTools.Label = "Werkzeug";
            app_btnTools.Click += PocketPaintApplication.GetInstance().ApplicationBarListener.BtnTools_OnClick;

            app_btnSave.Label = "Speichern";
            app_btnSaveCopy.Label = "Kopie speichern";
            app_btnNewPicture.Label = "New Picture";
            app_btnLoad.Label = "Laden";
            app_btnFullScreen.Label = "Vollbild";
            app_btnAbout.Label = "Über";

            cmdBar.PrimaryCommands.Add(app_btnTools);

            cmdBar.SecondaryCommands.Add(app_btnSave);
            cmdBar.SecondaryCommands.Add(app_btnSaveCopy);
            cmdBar.SecondaryCommands.Add(app_btnNewPicture);
            cmdBar.SecondaryCommands.Add(app_btnLoad);
            cmdBar.SecondaryCommands.Add(app_btnFullScreen);

            BottomAppBar = cmdBar;
            current_appbar = type;
        }

        void app_btn_reset_Click(object sender, RoutedEventArgs e)
        {
            PocketPaintApplication.GetInstance().PaintingAreaManipulationListener.ResetDrawingSpace();
        }

        private void BtnLeft_OnClick(object sender, RoutedEventArgs e)
        {
            if (PocketPaintApplication.GetInstance().ToolCurrent.GetToolType() == ToolType.Rotate)
            {
                var rotateTool = (RotateTool)PocketPaintApplication.GetInstance().ToolCurrent;
                rotateTool.RotateLeft();
            }
            else
                return;

        }

        private void BtnRight_OnClick(object sender, RoutedEventArgs e)
        {
            if (PocketPaintApplication.GetInstance().ToolCurrent.GetToolType() == ToolType.Rotate)
            {
                var rotateTool = (RotateTool)PocketPaintApplication.GetInstance().ToolCurrent;
                rotateTool.RotateRight();
            }
            else
                return;
        }

        void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            MoveZoomTool tool = new MoveZoomTool();
            ScaleTransform scaletransform = new ScaleTransform();
            scaletransform.ScaleX = 0.9;
            scaletransform.ScaleY = 0.9;
            tool.HandleMove(scaletransform);
        }

        void BtnZoomIn_Click(object sender, RoutedEventArgs e )
        {

            MoveZoomTool tool = new MoveZoomTool();
            ScaleTransform scaletransform = new ScaleTransform();
            scaletransform.ScaleX = 1.1;
            scaletransform.ScaleY = 1.1;
            tool.HandleMove(scaletransform);
        }

        public void NavigatedTo(Type source_type)
        {
            this.Frame.Navigate(source_type);
        }

        private void ToolChangedHere(ToolBase tool)
        {
            switch (tool.GetToolType())
            {
                case ToolType.Brush:
                case ToolType.Cursor:
                case ToolType.Line:
                    createAppBarAndSwitchAppBarContent("barStandard");
                    break;
                case ToolType.Crop:
                    // TODO: ApplicationBar = (IApplicationBar)this.Resources["barCrop"];
                    break;
                case ToolType.Ellipse:
                    createAppBarAndSwitchAppBarContent("barEllipse");
                    break;
                case ToolType.Eraser:
                    createAppBarAndSwitchAppBarContent("barEraser");
                    break;
                case ToolType.Flip:
                    createAppBarAndSwitchAppBarContent("barFlip");
                    break;
                case ToolType.Pipette:
                    createAppBarAndSwitchAppBarContent("barPipette");
                    break;
                case ToolType.Move:
                case ToolType.Zoom:
                    createAppBarAndSwitchAppBarContent("barMove");
                    break;
                case ToolType.Rect:
                    createAppBarAndSwitchAppBarContent("barRectangle");
                    break;
                case ToolType.Rotate:
                    createAppBarAndSwitchAppBarContent("barRotate");
                    break;
            }
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            SliderThicknessControl.Margin = new Thickness(0.0, 0.0, 0.0, 0.0);

            checkIfThicknessWasEntered();
            checkIfValueIsInRange(true);
            uctrlOwnKeyboard.Visibility = Visibility.Collapsed;
        }

        private void checkIfValueIsInRange(bool pressed_accept)
        {
            
            if (btnBrushThickness.Content == null)
            {
                btnValue0.IsEnabled = true;
                btnValue1.IsEnabled = true;
                btnValue2.IsEnabled = true;
                btnValue3.IsEnabled = true;
                btnValue4.IsEnabled = true;
                btnValue5.IsEnabled = true;
                btnValue6.IsEnabled = true;
                btnValue7.IsEnabled = true;
                btnValue8.IsEnabled = true;
                btnValue9.IsEnabled = true;
                btnValue0.IsEnabled = false;
            }
            else
            {
                Int32 input = Convert.ToInt32(btnBrushThickness.Content);
                if (input > 5 && input < 10)
                {
                    btnValue0.IsEnabled = false;
                    btnValue1.IsEnabled = false;
                    btnValue2.IsEnabled = false;
                    btnValue3.IsEnabled = false;
                    btnValue4.IsEnabled = false;
                    btnValue5.IsEnabled = false;
                    btnValue6.IsEnabled = false;
                    btnValue7.IsEnabled = false;
                    btnValue8.IsEnabled = false;
                    btnValue9.IsEnabled = false;
                }
                else if (input == 5)
                {
                    btnValue0.IsEnabled = true;
                    btnValue1.IsEnabled = false;
                    btnValue2.IsEnabled = false;
                    btnValue3.IsEnabled = false;
                    btnValue4.IsEnabled = false;
                    btnValue5.IsEnabled = false;
                    btnValue6.IsEnabled = false;
                    btnValue7.IsEnabled = false;
                    btnValue8.IsEnabled = false;
                    btnValue9.IsEnabled = false;
                }
                else if (input < 5)
                {
                    btnValue0.IsEnabled = true;
                    btnValue1.IsEnabled = true;
                    btnValue2.IsEnabled = true;
                    btnValue3.IsEnabled = true;
                    btnValue4.IsEnabled = true;
                    btnValue5.IsEnabled = true;
                    btnValue6.IsEnabled = true;
                    btnValue7.IsEnabled = true;
                    btnValue8.IsEnabled = true;
                    btnValue9.IsEnabled = true;
                }
                else
                {
                    btnValue0.IsEnabled = false;
                    btnValue1.IsEnabled = true;
                    btnValue2.IsEnabled = true;
                    btnValue3.IsEnabled = true;
                    btnValue4.IsEnabled = true;
                    btnValue5.IsEnabled = true;
                    btnValue6.IsEnabled = true;
                    btnValue7.IsEnabled = true;
                    btnValue8.IsEnabled = true;
                    btnValue9.IsEnabled = true;
                }

                SliderThickness.Value = Convert.ToDouble(input);
            }
        }

        public void checkIfThicknessWasEntered()
        {
            if (uctrlOwnKeyboard.Visibility == Visibility.Visible)
            {
                string slider_thickness_text_box_value = string.Empty;
                if(btnBrushThickness.Content != null)
                {
                    slider_thickness_text_box_value = btnBrushThickness.Content.ToString();
                }
                Int32 slider_thickness_text_box_int_value;

                if (!slider_thickness_text_box_value.Equals(""))
                {
                    slider_thickness_text_box_int_value = Convert.ToInt32(slider_thickness_text_box_value);

                    if (!(slider_thickness_text_box_int_value >= 1 && slider_thickness_text_box_int_value <= 50))
                    {
                        btnBrushThickness.Content = slider_thickness_textbox_last_value.ToString();
                    }
                    else
                    {
                        slider_thickness_textbox_last_value = slider_thickness_text_box_int_value;
                        SliderThickness.Value = slider_thickness_text_box_int_value;
                    }
                }
                else
                {
                    btnBrushThickness.Content = slider_thickness_textbox_last_value.ToString();
                }

                btnBrushThickness.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        private void btnDeleteNumbers_Click(object sender, RoutedEventArgs e)
        {
            if (btnBrushThickness.Content.ToString() != "" && Convert.ToInt32(btnBrushThickness.Content.ToString()) > 0)
            {
                btnBrushThickness.Content = btnBrushThickness.Content.ToString().Remove(btnBrushThickness.Content.ToString().Length - 1);
            }

            checkIfValueIsInRange(false);
        }

        private void ButtonNumbers_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string get_clicked_button_number = button.Name.Substring(8);

                if (btnBrushThickness.Content == null || btnBrushThickness.Content.ToString().Length < 2)
                {
                    btnBrushThickness.Content += get_clicked_button_number;
                }
                else if (btnBrushThickness.Content.ToString().Length == 2)
                {
                    btnBrushThickness.Content = "";
                    btnBrushThickness.Content += get_clicked_button_number;
                }
                checkIfValueIsInRange(false);
            }
        }

        private void SliderThickness_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (SliderThickness != null)
            {
                btnBrushThickness.Content = Convert.ToInt32(SliderThickness.Value).ToString();
                slider_thickness_textbox_last_value = Convert.ToInt32(SliderThickness.Value);
                PocketPaintApplication.GetInstance().PaintData.ThicknessSelected = Convert.ToInt32(SliderThickness.Value);
            }
        }

        public Visibility getVisibilityOFSliderThicknessControl()
        {
            return SliderThicknessControl.Visibility;
        }

        public void setVisibilityOFSliderThicknessControl(Visibility visibility)
        {
            SliderThicknessControl.Visibility = visibility;
            if (Visibility.Collapsed == visibility)
            {
                setVisibilityOFThicknessKeyboard(visibility);
            }
        }
        public void setVisibilityOFThicknessKeyboard(Visibility visibility)
        {
            uctrlOwnKeyboard.Visibility = visibility;
        }
        public void setSliderThicknessControlMargin(Thickness margin)
        {
            SliderThicknessControl.Margin = margin;
        }
        public void setVisibilityOFRectEllUserControl(Visibility visibility)
        {
            GridUserControlRectEll.Visibility = visibility;
        }
        public void setRectEllUserControlMargin(Thickness margin)
        {
            GridUserControlRectEll.Margin = margin;
        }
        public Visibility getVisibilityOFRectEllUserControl()
        {
            return GridUserControlRectEll.Visibility;
        }

        private void btnBrushThickness_Click(object sender, RoutedEventArgs e)
        {
            checkIfThicknessWasEntered();
            if (getVisibilityOFThicknessKeyboard() == Visibility.Collapsed)
            {
                setSliderThicknessControlMargin(new Thickness(0.0, -324.0, 0.0, 287.0));
                setVisibilityOFThicknessKeyboard(Visibility.Visible);
            }
            else
            {
                setVisibilityOFThicknessKeyboard(Visibility.Collapsed);
                setSliderThicknessControlMargin(new Thickness(0.0, 0.0, 0.0, 0.0));
            }
        }
        public Visibility getVisibilityOFThicknessKeyboard()
        {
            return uctrlOwnKeyboard.Visibility;
        }

        private void btnThickness_Click(object sender, RoutedEventArgs e)
        {
            if (getVisibilityOFSliderThicknessControl() == Visibility.Collapsed)
            {
                setVisibilityOFSliderThicknessControl(Visibility.Visible);
                setSliderThicknessControlMargin(new Thickness(0.0, 0.0, 0.0, 0.0));
            }
            else
            {
                setVisibilityOFSliderThicknessControl(Visibility.Collapsed);
            }
        }

        private void btnThicknessBorder_Click(object sender, RoutedEventArgs e)
        {
            if (getVisibilityOFRectEllUserControl() == Visibility.Collapsed)
            {
                setVisibilityOFRectEllUserControl(Visibility.Visible);
                setRectEllUserControlMargin(new Thickness(0.0, 0.0, 0.0, 0.0));
            }
            else
            {
                setVisibilityOFRectEllUserControl(Visibility.Collapsed);
            }
        }

        private void LayoutRoot_ManipulationStarted_1(object sender, ManipulationStartedRoutedEventArgs e)
        {

        }

        private void PaintingAreaCanvas_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            var point = new Point(Convert.ToInt32(e.GetCurrentPoint(PaintingAreaCanvas).Position.X), Convert.ToInt32(e.GetCurrentPoint(PaintingAreaCanvas).Position.Y));

            // TODO some bubbling? issue here, fast multiple applicationbartop undos result in triggering this event
            if (point.X < 0 || point.Y < 0 || Spinner.SpinnerActive || e.Handled)
            {
                return;
            }

            PocketPaintApplication.GetInstance().ToolCurrent.HandleDown(point);
            PocketPaintApplication.GetInstance().ToolCurrent.HandleUp(point);

            e.Handled = true;
        }

        private void BtnHorizotal_OnClick(object sender, RoutedEventArgs e)
        {
            if (PocketPaintApplication.GetInstance().ToolCurrent.GetToolType() == ToolType.Flip)
            {
                var flipTool = (FlipTool)PocketPaintApplication.GetInstance().ToolCurrent;
                flipTool.FlipHorizontal();
            }
            else
                return;
        }

        private void BtnVertical_OnClick(object sender, RoutedEventArgs e)
        {
            if (PocketPaintApplication.GetInstance().ToolCurrent.GetToolType() == ToolType.Flip)
            {
                var flipTool = (FlipTool)PocketPaintApplication.GetInstance().ToolCurrent;
                flipTool.FlipVertical();
            }
            else
                return;
        }

        private void testRectangle_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            start_point.X = e.GetCurrentPoint(PaintingAreaCanvas).Position.X;
            start_point.Y = e.GetCurrentPoint(PaintingAreaCanvas).Position.Y;
        }

        private void testRectangle_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            
            double bottom = rectDrawRectangle.Margin.Bottom;
            double top = rectDrawRectangle.Margin.Top;
            double left = rectDrawRectangle.Margin.Left;
            double right = rectDrawRectangle.Margin.Right;
            Point current_point = new Point(e.GetCurrentPoint(PaintingAreaCanvas).Position.X, e.GetCurrentPoint(PaintingAreaCanvas).Position.Y);
            if (old_point.X != current_point.X && old_point.Y != current_point.Y)
            {
                Point distance = new Point(current_point.X - start_point.X, current_point.Y - start_point.Y);
                rectDrawRectangle.Margin = new Thickness(left + distance.X, top + distance.Y, right, bottom);
                coordinates.Text = "X: " + (left + current_point.X).ToString() + ", " + (top + current_point.Y).ToString().ToString();
                old_point = current_point;
            }
        }

        public Visibility visibilityRecDrawingRectangle
        {
            get
            {
                return rectDrawRectangle.Visibility;
            }
            set
            {
                rectDrawRectangle.Visibility = value;
            }
        }

        public Point coordinatesRecDrawingRectangle
        {
            get
            {
                Point current_coordinate = new Point();
                current_coordinate.X = rectDrawRectangle.Margin.Left;
                current_coordinate.Y = rectDrawRectangle.Margin.Top;

                return current_coordinate;
            }
            set
            {
                Point current_coordinate = (Point)value;
                double right = rectDrawRectangle.Margin.Right;
                double bottom = rectDrawRectangle.Margin.Bottom;
                rectDrawRectangle.Margin = new Thickness(current_coordinate.X, current_coordinate.Y, right, bottom);
            }
        }

        private void rectDrawRectangle_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            Point coordinatesOfRectangle = new Point(rectDrawRectangle.Margin.Left, rectDrawRectangle.Margin.Top);
            PocketPaintApplication.GetInstance().ToolCurrent.Draw(coordinatesOfRectangle);
        }

        private void rectDrawRectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            start_point.X = e.GetPosition(PaintingAreaCanvas).X;
            start_point.Y = e.GetPosition(PaintingAreaCanvas).Y;
        }
    }
}
