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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using HelixToolkit;
using LSC1DatabaseEditor.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Messages;
using HelixToolkit.Wpf;
using PresentationCoreExtensions;
using LSC1DatabaseEditor.Messages.Common;
using ExtensionsAndCodeSnippets;
using LSC1DatabaseEditor.LSC1ProgramSimulator.Messages;
using System.Data;
using LSC1DatabaseEditor.LSC1CommonTool.Messages;
using LSC1DatabaseLibrary;
using LSC1DatabaseEditor.LSC1ProgramSimulator.ViewModels;
using LSC1DatabaseEditor.ViewModel.DataStructures;

namespace LSC1DatabaseEditor.Views
{
    /// <summary>
    /// Interaction logic for LSC1Simulator.xaml
    /// </summary>
    public partial class LSC1SimulatorWindow : Window
    {
        public LSC1SimulatorWindow()
        {
            InitializeComponent();

            DataContext = new SimulatorViewModel();

            Messenger.Default.Register<UpdateViewport3DMessage>(this, SimulatorViewModel.MessageToken, UpdateLines);
            Messenger.Default.Register<CameraZoomMessage>(this, SimulatorViewModel.MessageToken, Zoom);
            Messenger.Default.Register<MoveMachineHeadMessage>(this, SimulatorViewModel.MessageToken, MoveMachineHead);
            Messenger.Default.Register<TableSelectionChangedMessage>(this, SimulatorViewModel.MessageToken, ChangeTableResource);
        }

        private void ChangeTableResource(TableSelectionChangedMessage msg)
        {
            if (msg == null)
            {
                gridContainter1.Content = null;
                gridContainter2.Content = null;
            }
                
            if(msg.SelectedTable.Table == TablesEnum.tpos)
            {
                gridContainter1.Content = FindResource(msg.SelectedTable.DataGridName);
                gridContainter2.Content = null;
            }

            if(msg.SelectedTable.Table == TablesEnum.tjobdata)
            {
                gridContainter1.Content = FindResource(msg.SelectedTable.DataGridName);
                gridContainter2.Content = null;
            }

            if(msg.SelectedTable.Table == TablesEnum.tprocrobot)
                gridContainter1.Content = FindResource(msg.SelectedTable.DataGridName);

            if(msg.SelectedTable.Table == TablesEnum.tproclaserdata)
                gridContainter2.Content = FindResource(msg.SelectedTable.DataGridName);
        }


        private void dataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var selectedCells = ((gridContainter1.Content) as DataGrid).SelectedCells;

            var val = e.EditingElement as TextBox;

            foreach (var cell in selectedCells)
            {
                DataRowView row = cell.Item as DataRowView;
                string columnName = e.Column.Header.ToString();
                string oldValue = (row.Row[columnName]).ToString();
                row.Row[columnName] = val.Text;

                Messenger.Default.Send(new DataGridCellValueChangedMessage(val.Text, oldValue, columnName, row, ((LSC1StepDataGridVM)gridContainter1.DataContext).Items1), SimulatorViewModel.MessageToken);
            }
        }

        private void dataGrid2_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var selectedCells = ((gridContainter1.Content) as DataGrid).SelectedCells;

            var val = e.EditingElement as TextBox;

            foreach (var cell in selectedCells)
            {
                DataRowView row = cell.Item as DataRowView;
                string columnName = e.Column.Header.ToString();
                string oldValue = (row.Row[columnName]).ToString();
                row.Row[columnName] = val.Text;

                Messenger.Default.Send(new DataGridCellValueChangedMessage(val.Text, oldValue, columnName, row, ((LSC1StepDataGridVM)gridContainter1.DataContext).Items2), SimulatorViewModel.MessageToken);
            }
        }

        void UpdateLines(UpdateViewport3DMessage msg)
        {
            if (msg.Op == ViewportUpdateOperation.Add)
            {
                machineTrackVisuals.Children.Add(msg.Visual);                
            }
            else if (msg.Op == ViewportUpdateOperation.Remove)
            {
                machineTrackVisuals.Children.Remove(msg.Visual);
            }
            else if(msg.Op == ViewportUpdateOperation.Clear)
            {
                machineTrackVisuals.Children.Clear();
            }
        }

        void Zoom(CameraZoomMessage msg)
        {
            viewPort3D.Camera.LookAt(msg.Center, 100);
            viewPort3D.ZoomExtents(msg.Bounds, 100);            
        }

        void MoveMachineHead(MoveMachineHeadMessage msg)
        {
            
            Transform3DGroup col = new Transform3DGroup();
            col.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), msg.Orientation.Rotation.X)));
            col.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), msg.Orientation.Rotation.Y)));
            col.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), msg.Orientation.Rotation.Z)));
            
            col.Children.Add(new TranslateTransform3D(msg.Orientation.Position.X, msg.Orientation.Position.Y, msg.Orientation.Position.Z));           

            machineHead.Transform = col;
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e == null || e.NewValue == null)
                return;

            if (e.NewValue.GetType() == typeof(LSC1TreeViewPointLeaveItem))
            {
                var orgS = (LSC1TreeViewPointLeaveItem)e.NewValue;

                Messenger.Default.Send(new SelectedTreeViewItemChanged(sender, orgS), SimulatorViewModel.MessageToken);
            }

            if (e.NewValue.GetType() == typeof(LSC1TreeViewJobStepNode))
            {
                var orgS = (LSC1TreeViewJobStepNode)e.NewValue;

                Messenger.Default.Send(new SelectedTreeViewItemChanged(sender, orgS), SimulatorViewModel.MessageToken);
            }
        }
    }
}
