using ExtensionsAndCodeSnippets;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using HelixToolkit.Wpf;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseEditor.Messages.Common;
using LSC1DatabaseEditor.ViewModel;
using LSC1DatabaseEditor.ViewModel.DataStructures;
using LSC1DatabaseLibrary.LSC1JobRepresentation;
using LSC1DatabaseLibrary.LSC1JobRepresentation.JobDataConverter;
using LSC1DatabaseLibrary.LSC1Simulation;
using LSC1DatabaseLibrary.LSC1Visualisation;
using PresentationCoreExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LSC1DatabaseEditor.LSC1ProgramSimulator.ViewModels
{
    public class LSC1ViewportVM
    {
        public LSC1ViewportVM()
        {
            Messenger.Default.Register<LSC1JobChangedMessage>(this, SimulatorViewModel.MessageToken, CreateJobObjects);
            Messenger.Default.Register<SelectedTreeViewItemChanged>(this, SimulatorViewModel.MessageToken, ShowManipulatorOnObject);
                   
        }

        void CreateJobObjects(LSC1JobChangedMessage msg)
        {
            Messenger.Default.Send(new UpdateViewport3DMessage(null, ViewportUpdateOperation.Clear), SimulatorViewModel.MessageToken);


            //TODO Eigenes Visual erstellen welches aus Punkten die benkötigten dinge erstellt
            var jobData = new LSC1JobData(msg.NewJob);
            jobData.LoadJob(LSC1UserSettings.Instance.DBSettings);

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
                PointsVisual3D pointsVisual = new PointsVisual3D();
                pointsVisual.Points = item;
                pointsVisual.Color = colors.RepeatingIndex(colorIndex);
                pointsVisual.Size = LSC1UserSettings.Instance.PointSize;
                Messenger.Default.Send(new UpdateViewport3DMessage(pointsVisual, ViewportUpdateOperation.Add), SimulatorViewModel.MessageToken);

                LinesVisual3D linesVisual = new LinesVisual3D();

                item.TransformToLineCollection();
                linesVisual.Points = item;
                linesVisual.Color = colors.RepeatingIndex(colorIndex);
                Messenger.Default.Send(new UpdateViewport3DMessage(linesVisual, ViewportUpdateOperation.Add), SimulatorViewModel.MessageToken);

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
                    
                    Messenger.Default.Send(new UpdateViewport3DMessage(line, ViewportUpdateOperation.Add), SimulatorViewModel.MessageToken);
                }

                colorIndex++;
            }

        }

        void ShowManipulatorOnObject(SelectedTreeViewItemChanged msg)
        {
            if (msg.SelectedItem.GetType() == typeof(LSC1TreeViewPointLeaveItem))
            {
                var item = (LSC1TreeViewPointLeaveItem)msg.SelectedItem;

                TranslateManipulator mani = new TranslateManipulator();
                Binding binding = new Binding("Transform");
                binding.Source = mani;
            }
        }
    }

    public class PointLineWithManipulator : ModelVisual3D
    {
        LinesVisual3D line = new LinesVisual3D();
        PointsVisual3D points = new PointsVisual3D();
        TranslateManipulator mani = new TranslateManipulator();

        public PointLineWithManipulator()
        {
            Point3DCollection ps = new Point3DCollection();
            ps.Add(new Point3D(0, 0, 0));
            points.Points = ps;

            Binding binding = new Binding("Transform");
            binding.Source = mani;

            BindingOperations.SetBinding(points, ModelVisual3D.TransformProperty, binding);

            Children.Add(points);
            Children.Add(mani);
        }
    }
}
