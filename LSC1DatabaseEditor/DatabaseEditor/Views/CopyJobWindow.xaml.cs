﻿using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LSC1DatabaseEditor.Views
{
    /// <summary>
    /// Interaction logic for CopyJobWindow.xaml
    /// </summary>
    public partial class CopyJobWindow : Window
    {
        CopyJobViewModel viewModel = new CopyJobViewModel();

        public CopyJobWindow()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Messenger.Default.Send(new TextChangedMessage()
            {
                NewText = ((TextBox)e.Source).Text,
            });
        }
    }
}
