using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HelixToolkit.Wpf;
using LSC1DatabaseEditor.LSC1ProgramSimulator.Messages;
using LSC1DatabaseEditor.LSC1ProgramSimulator.ViewModels.DataStructures;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseEditor.Messages.Common;
using PresentationCoreExtensions;
using System;
using System.Windows.Media.Media3D;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;

namespace LSC1DatabaseEditor.LSC1ProgramSimulator.ViewModels
{
    public class LSC1SimulatorViewModel : ViewModelBase
    {
        public LSC1SimulatorMenuViewModel MenuVM { get; set; } = new LSC1SimulatorMenuViewModel();
        public LSC1ProgramTreeViewViewModel TreeViewVM { get; set; } = new LSC1ProgramTreeViewViewModel();
        public LSC1ViewportViewModel ViewPortVM { get; set; } = new LSC1ViewportViewModel();
        public LSC1StepDataGridViewModel DataGridVM { get; set; } = new LSC1StepDataGridViewModel();

        public static readonly Guid MessageToken = Guid.NewGuid();

        private DbJobNameRow selectedJob;
        public DbJobNameRow SelectedJob
        {
            get => selectedJob;
            set
            {
                selectedJob = value;
                CreatePointModelData();  
            }
        }
                                
        public LSC1SimulatorViewModel()
        {                        
            Messenger.Default.Register<SelectedTreeViewItemChanged>(this, MessageToken, ZoomOnSelectedObject);
        }

        private void ZoomOnSelectedObject(SelectedTreeViewItemChanged msg)
        {
            if(msg.SelectedItem.GetType() == typeof(LSC1TreeViewJobStepNode))
            {
                var node = (LSC1TreeViewJobStepNode)msg.SelectedItem;
                if (node.Points.Count == 0)//TODO Fehler wegen nicht impelementerier Befehle wodurch keine Instruction erstellt wird.
                    return;

                Point3D centerPoint = node.Points.CalculateCenter();

                if (double.IsNaN(centerPoint.X))//TODO Fehler wegen nicht impelementerier Befehle wodurch keine Instruction erstellt wird.
                    return;

                ContainerUIElement3D container = new ContainerUIElement3D();
                container.Children.Add(new PointsVisual3D()
                {
                    Points = node.Points
                });

                var bounds = Visual3DHelper.FindBounds(container.Children);

                Messenger.Default.Send(new CameraZoomMessage(bounds, centerPoint), MessageToken);
            }
            if(msg.SelectedItem.GetType() == typeof(LSC1TreeViewPointLeaveItem))
            {
                var leave = (LSC1TreeViewPointLeaveItem)msg.SelectedItem;
                Messenger.Default.Send(new MoveMachineHeadMessage(leave.InstructionStepData.MachineStatusAfterInstructions.TCPOrientation), MessageToken);
            }
        }

        private void CreatePointModelData()
        {
            //var points = new List<PointsVisual3D>();
            //var lines = new List<LinesVisual3D>();

            //foreach (var item in JobTreeView)
            //{
            //    points.Add(item.Points);
            //    lines.Add(item.Line);
            //}

            //Messenger.Default.Send(new UpdateViewport3DMessage(points, lines));
        }
    }
}
