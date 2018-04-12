using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HelixToolkit.Wpf;
using LSC1DatabaseEditor.LSC1ProgramSimulator.Messages;
using LSC1DatabaseEditor.LSC1ProgramSimulator.ViewModels;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseEditor.Messages.Common;
using LSC1DatabaseEditor.ViewModel.DataStructures;
using LSC1DatabaseLibrary.DatabaseModel;
using PresentationCoreExtensions;
using System;
using System.Windows.Media.Media3D;

namespace LSC1DatabaseEditor.ViewModel
{
    public class SimulatorViewModel : ViewModelBase
    {
        public LSC1SimulatorMenuVM MenuVM { get; set; } = new LSC1SimulatorMenuVM();
        public LSC1ProgramTreeViewVM TreeViewVM { get; set; } = new LSC1ProgramTreeViewVM();
        public LSC1ViewportVM ViewPortVM { get; set; } = new LSC1ViewportVM();
        public LSC1StepDataGridVM DataGridVM { get; set; } = new LSC1StepDataGridVM();

        public static readonly Guid MessageToken = Guid.NewGuid();

        private DbJobNameRow selectedJob;
        public DbJobNameRow SelectedJob
        {
            get { return selectedJob; }
            set
            {
                selectedJob = value;
                CreatePointModelData();  
            }
        }
                                
        public SimulatorViewModel()
        {                        
            Messenger.Default.Register<SelectedTreeViewItemChanged>(this, MessageToken, (msg) => ZoomOnSelectedObject(msg));
        }

        void ZoomOnSelectedObject(SelectedTreeViewItemChanged msg)
        {
            if(msg.SelectedItem.GetType() == typeof(LSC1TreeViewJobStepNode))
            {
                var node = (LSC1TreeViewJobStepNode)msg.SelectedItem;
                if (node.Points.Count == 0)//TODO Fehler wegen nicht impelementerier Befehle wodurch keine Instruction erstellt wird.
                    return;

                Point3D centerPoint = node.Points.CalculateCenter();

                if (centerPoint.X == double.NaN)//TODO Fehler wegen nicht impelementerier Befehle wodurch keine Instruction erstellt wird.
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

        void CreatePointModelData()
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
