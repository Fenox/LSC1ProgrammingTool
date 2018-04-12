using ExtensionsAndCodeSnippets;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Messages;
using LSC1DatabaseEditor.ViewModel;
using LSC1DatabaseEditor.ViewModel.DataStructures;
using LSC1DatabaseLibrary.LSC1JobRepresentation;
using LSC1DatabaseLibrary.LSC1JobRepresentation.JobDataConverter;
using LSC1DatabaseLibrary.LSC1Simulation;
using System.Collections.ObjectModel;
using System.Linq;

namespace LSC1DatabaseEditor.LSC1ProgramSimulator.ViewModels
{
    public class LSC1ProgramTreeViewVM
    {
        public ObservableCollection<LSC1TreeViewJobStepNode> JobTreeView { get; set; } = new ObservableCollection<LSC1TreeViewJobStepNode>();
        
        public LSC1ProgramTreeViewVM()
        {
            Messenger.Default.Register<LSC1JobChangedMessage>(this, SimulatorViewModel.MessageToken, LoadNewJob);
        }

        void LoadNewJob(LSC1JobChangedMessage msg)
        {
            var jobData = new LSC1JobData(msg.NewJob);
            jobData.LoadJob(LSC1UserSettings.Instance.DBSettings);

            JobDataToJobSturctureConverter jobSim = new JobDataToJobSturctureConverter(jobData);
            var structuredJobData = jobSim.Convert();

            LSC1MachineSimulator sim = new LSC1MachineSimulator();
            var machineJobData = sim.SimulateJob(structuredJobData);

            int colorIndex = 0;
            JobTreeView.Clear();
            var colors = LSC1UserSettings.Instance.VisualisationColors;
            foreach (var item in machineJobData.JobDataSteps)
            {
                if (item.JobDataStepRow == null)
                    continue;

                var newItem = new LSC1TreeViewJobStepNode()
                {
                    BackgroundColor = colors.RepeatingIndex(colorIndex++),
                    JobStepData = item,
                };

                JobTreeView.Add(newItem);
                int procStep = 1;
                foreach (var subItem in item.StepInstructions.Where(i => i.MachineStatusAfterInstructions != null))
                {
                    var newSubItem = new LSC1TreeViewPointLeaveItem();
                    subItem.Instructions.RemoveAll(i => i == null);
                    newSubItem.InstructionStepData = subItem;
                    newItem.InstructionSubItems.Add(newSubItem);
                    procStep++;
                }
            }
        }
    }
}
