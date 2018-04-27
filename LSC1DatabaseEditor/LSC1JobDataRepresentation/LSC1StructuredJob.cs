using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.LSC1Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;

namespace LSC1DatabaseLibrary.LSC1JobRepresentation
{
    public class LSC1StructuredJob<T> where T : InstructionStep
    {
        public DbJobNameRow JobInfo { get; set; }
        public List<LSC1JobDataStep<T>> JobDataSteps { get; set; } = new List<LSC1JobDataStep<T>>();

        public static LSC1StructuredJob<InstructionStepAndMachineState> TransformToMachineJob(LSC1StructuredJob<InstructionStep> job)
        {
            var newJob = new LSC1StructuredJob<InstructionStepAndMachineState>
            {
                JobInfo = job.JobInfo
            };

            foreach (var jobDataStep in job.JobDataSteps)
            {
                newJob.JobDataSteps.Add(LSC1JobDataStep<InstructionStep>.TransformToMachineStep(jobDataStep));
            }

            return newJob;
        }
    }

    public class LSC1JobDataStep<T> where T : InstructionStep
    {
        public DbJobDataRow JobDataStepRow;

        public DbFrameRow Frame { get; set; }
        public DbMoveParamRow MoveParam { get; set; }
        public DbToolRow Tool { get; set; }

        public List<T> StepInstructions { get; set; } = new List<T>();

        public static LSC1JobDataStep<InstructionStepAndMachineState> TransformToMachineStep(LSC1JobDataStep<InstructionStep> step)
        {
            var machineStep = new LSC1JobDataStep<InstructionStepAndMachineState>
            {
                Frame = step.Frame,
                MoveParam = step.MoveParam,
                Tool = step.Tool,
                JobDataStepRow = step.JobDataStepRow
            };

            foreach (var stepInstruction in step.StepInstructions)
            {
                machineStep.StepInstructions.Add(new InstructionStepAndMachineState(stepInstruction));
            }

            return machineStep;
        }
    }

    public class InstructionStep
    {
        public List<DbRow> Instructions { get; set; } = new List<DbRow>();
    }

    public class InstructionStepAndMachineState : InstructionStep
    {
        public InstructionStepAndMachineState(InstructionStep step)
        {
            Instructions = step.Instructions;
        }

        public LSC1MachineState MachineStatusAfterInstructions { get; set; }
    }
}
