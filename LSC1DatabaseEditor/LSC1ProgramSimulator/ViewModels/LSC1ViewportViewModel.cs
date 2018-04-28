using ExtensionsAndCodeSnippets;
using GalaSoft.MvvmLight.Messaging;
using HelixToolkit.Wpf;
using LSC1DatabaseEditor.LSC1ProgramSimulator.ViewModels.DataStructures;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseLibrary.LSC1JobRepresentation;
using LSC1DatabaseLibrary.LSC1JobRepresentation.JobDataConverter;
using LSC1DatabaseLibrary.LSC1Simulation;
using LSC1DatabaseLibrary.LSC1Visualisation;
using PresentationCoreExtensions;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using LSC1DatabaseEditor.Common.Messages;
using LSC1DatabaseEditor.LSC1CommonTool.Messages;

namespace LSC1DatabaseEditor.LSC1ProgramSimulator.ViewModels
{
    public class LSC1ViewportViewModel
    {
        public LSC1ViewportViewModel()
        {
            Messenger.Default.Register<LSC1JobChangedMessage>(this, LSC1SimulatorViewModel.MessageToken, CreateJobObjects);
            Messenger.Default.Register<SelectedTreeViewItemChanged>(this, LSC1SimulatorViewModel.MessageToken, ShowManipulatorOnObject);
                   
        }

        private void CreateJobObjects(LSC1JobChangedMessage msg)
        {
            Messenger.Default.Send(new UpdateViewport3DMessage(null, ViewportUpdateOperation.Clear), LSC1SimulatorViewModel.MessageToken);


            //TODO Eigenes Visual erstellen welches aus Punkten die benkötigten dinge erstellt
            var jobData = new LSC1JobData(msg.NewJob);
            jobData.LoadJob();

            JobDataToJobSturctureConverter jobSim = new JobDataToJobSturctureConverter(jobData);
            var structuredJobData = jobSim.Convert();

            LSC1MachineSimulator sim = new LSC1MachineSimulator();
            var machineJobData = sim.SimulateJob(structuredJobData);

            var pointGenerator = new LSC1MachinePathPointGenerator();
            var points = pointGenerator.GeneratePathPoints(machineJobData);

            var colors = LSC1UserSettings.Instance.VisualisationColors;
            int colorIndex = 0;

            foreach (var item in points)
            {
                PointsVisual3D pointsVisual = new PointsVisual3D
                {
                    Points = item,
                    Color = colors.RepeatingIndex(colorIndex),
                    Size = LSC1UserSettings.Instance.PointSize
                };
                Messenger.Default.Send(new UpdateViewport3DMessage(pointsVisual, ViewportUpdateOperation.Add), LSC1SimulatorViewModel.MessageToken);

                LinesVisual3D linesVisual = new LinesVisual3D();

                item.TransformToLineCollection();
                linesVisual.Points = item;
                linesVisual.Color = colors.RepeatingIndex(colorIndex);
                Messenger.Default.Send(new UpdateViewport3DMessage(linesVisual, ViewportUpdateOperation.Add), LSC1SimulatorViewModel.MessageToken);

                //Hinzufügen der Strecken ziwschen 2 Steps
                if (colorIndex > 0)
                {
                    var lastPoints = points[colorIndex - 1];
                    var currentPoints = item;

                    var point1 = lastPoints[lastPoints.Count - 1];
                    var point2 = item[0];

                    LinesVisual3D line = new LinesVisual3D();
                    line.Points.Add(point1);
                    line.Points.Add(point2);
                    line.Color = Colors.Black;
                    
                    Messenger.Default.Send(new UpdateViewport3DMessage(line, ViewportUpdateOperation.Add), LSC1SimulatorViewModel.MessageToken);
                }

                colorIndex++;
            }

        }

        private void ShowManipulatorOnObject(SelectedTreeViewItemChanged msg)
        {
            if (msg.SelectedItem.GetType() == typeof(LSC1TreeViewPointLeaveItem))
            {
                var item = (LSC1TreeViewPointLeaveItem)msg.SelectedItem;

                TranslateManipulator mani = new TranslateManipulator();
                Binding binding = new Binding("Transform")
                {
                    Source = mani
                };
            }
        }
    }

    public class PointLineWithManipulator : ModelVisual3D
    {
        private LinesVisual3D line = new LinesVisual3D();
        private PointsVisual3D points = new PointsVisual3D();
        private TranslateManipulator mani = new TranslateManipulator();

        public PointLineWithManipulator()
        {
            Point3DCollection ps = new Point3DCollection
            {
                new Point3D(0, 0, 0)
            };
            points.Points = ps;

            Binding binding = new Binding("Transform")
            {
                Source = mani
            };

            BindingOperations.SetBinding(points, ModelVisual3D.TransformProperty, binding);

            Children.Add(points);
            Children.Add(mani);
        }
    }
}
