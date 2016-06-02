using HelixToolkit.Wpf;
using LSC1DatabaseLibrary.LSC1JobRepresentation;
using PresentationCoreExtensions;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LSC1DatabaseEditor.ViewModel.DataStructures
{
    public class LSC1TreeViewJobStepNode
    {
        public string Text
        {
            get
            {
                return JobStepData.JobDataStepRow.Who + " " + JobStepData.JobDataStepRow.What + " " + JobStepData.JobDataStepRow.Name;
            }
        }

        public Color BackgroundColor { get; set; }

        public Point3DCollection Points
        {
            get
            {
                Point3DCollection points = new Point3DCollection();
                foreach (var item in InstructionSubItems)
                    points.Add(item.Position);

                return points;
            }
        }

        public ObservableCollection<LSC1TreeViewPointLeaveItem> InstructionSubItems { get; set; } = new ObservableCollection<LSC1TreeViewPointLeaveItem>();

        public LSC1JobDataStep<InstructionStepAndMachineState> JobStepData { get; set; }
    }

    public class LSC1TreeViewPointLeaveItem
    {
        public string Text
        {
            get
            {
                string text = "";
                foreach (var item in InstructionStepData.Instructions)
                {
                    text += item.TableName + " ";
                }
                return text;
            }
        }

        public Point3D Position
        {
            get
            {
                return InstructionStepData.MachineStatusAfterInstructions.TCPOrientation.Position;
            }
        }

        public InstructionStepAndMachineState InstructionStepData { get; set; }
    }

}
