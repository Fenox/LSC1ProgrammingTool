using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.DatabaseModel;
using LSC1DatabaseLibrary.LSC1JobRepresentation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace LSC1DatabaseLibrary.LSC1Simulation
{
    public class LSC1MachineSimulator
    {
        public LSC1StructuredJob<InstructionStepAndMachineState> SimulateJob(LSC1StructuredJob<InstructionStep> job)
        {
            var machineJob = LSC1StructuredJob<InstructionStep>.TransformToMachineJob(job);
            
            var oldMachineStatus = new LSC1MachineState();
            foreach (var jobDataStep in machineJob.JobDataSteps)
            {
                foreach (var item in jobDataStep.StepInstructions)
                {
                    oldMachineStatus = SimulateOnState(item.Instructions, jobDataStep.Frame, oldMachineStatus);
                    item.MachineStatusAfterInstructions = oldMachineStatus;
                }
            }

            return machineJob;
        }

        public LSC1MachineState SimulateOnState(List<DbRow> instructions, DbFrameRow frame, LSC1MachineState machineStatus)
        {
            var newMachineStatus = new LSC1MachineState(machineStatus);
            foreach (var instruction in instructions.Where(i => i != null))
            {
                ExecuteRow(instruction, newMachineStatus);
            }

            if(frame != null)
                newMachineStatus.TCPOrientation.Transform(frame);

            return newMachineStatus;
        }

        private void ExecuteRow(DbRow instr, LSC1MachineState newMachineStatus)
        {
            if (instr.GetType() == typeof(DbProcRobotRow))
            {
                DbProcRobotRow row = (DbProcRobotRow)instr;
                newMachineStatus.TCPOrientation.Position.X = double.Parse(row.X, CultureInfo.InvariantCulture);
                newMachineStatus.TCPOrientation.Position.Y = double.Parse(row.Y, CultureInfo.InvariantCulture);
                newMachineStatus.TCPOrientation.Position.Z = double.Parse(row.Z, CultureInfo.InvariantCulture);

                newMachineStatus.TCPOrientation.Rotation.X = double.Parse(row.RX, CultureInfo.InvariantCulture);
                newMachineStatus.TCPOrientation.Rotation.Y = double.Parse(row.RY, CultureInfo.InvariantCulture);
                newMachineStatus.TCPOrientation.Rotation.Z = double.Parse(row.RZ, CultureInfo.InvariantCulture);
            }
            if (instr.GetType() == typeof(DbPosRow))
            {
                DbPosRow row = (DbPosRow)instr;
                newMachineStatus.TCPOrientation.Position.X = double.Parse(row.X, CultureInfo.InvariantCulture);
                newMachineStatus.TCPOrientation.Position.Y = double.Parse(row.Y, CultureInfo.InvariantCulture);
                newMachineStatus.TCPOrientation.Position.Z = double.Parse(row.Z, CultureInfo.InvariantCulture);

                newMachineStatus.TCPOrientation.Rotation.X = double.Parse(row.RX, CultureInfo.InvariantCulture);
                newMachineStatus.TCPOrientation.Rotation.Y = double.Parse(row.RY, CultureInfo.InvariantCulture);
                newMachineStatus.TCPOrientation.Rotation.Z = double.Parse(row.RZ, CultureInfo.InvariantCulture);
            }
            if (instr.GetType() == typeof(DbProcLaserDataRow))
            {
                DbProcLaserDataRow row = (DbProcLaserDataRow)instr;
                newMachineStatus.LaserParameters.BeamOn = row.BeamOn == "yes";
                newMachineStatus.LaserParameters.C_Grip = int.Parse(row.C_Grip);
                newMachineStatus.LaserParameters.Power = int.Parse(row.Power);
            }
        }
    }
}
