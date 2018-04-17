using LSC1DatabaseLibrary.CommonMySql;
using LSC1DatabaseLibrary.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.LSC1JobRepresentation.JobDataConverter
{
    public class JobDataToJobSturctureConverter
    {
        public LSC1JobData Job { get; set; }

        int JobDataStep { get; set; }
        int ProcStep { get; set; } = 1;

        public DbJobDataRow CurrentJobData
        {
            get
            {
                if (Job.JobData.Count <= JobDataStep)
                    return null;

                return Job.JobData[JobDataStep];
            }
        }

        public DbProcLaserDataRow CurrentLaserData
        {
            get
            {
                return Job.FilterLaserDataBy(CurrentJobData.Name, ProcStep);
            }
        }

        public DbProcPlcRow CurrentPLCData
        {
            get
            {
                return Job.FilterPlcDataBy(CurrentJobData.Name, ProcStep);
            }
        }

        public DbProcPulseRow CurrentPulseData
        {
            get
            {
                return Job.FilterPulseDataBy(CurrentJobData.Name, ProcStep);
            }
        }

        public DbProcRobotRow CurrentRobotData
        {
            get
            {
                if (!(CurrentJobData.Who == "robot" && CurrentJobData.What == "proc"))
                    return null;

                return Job.FilterRobotDataBy(CurrentJobData.Name, ProcStep);
            }
        }

        public DbProcTurnRow CurrentTurnData
        {
            get
            {
                return Job.FilterTurnDataBy(CurrentJobData.Name, ProcStep);
            }
        }

        public DbPosRow CurrentPosData
        {
            get
            {
                if (!(CurrentJobData.Who == "robot" && CurrentJobData.What == "pos"))
                    return null;

                return Job.Positions.FirstOrDefault((item) => item.Name == CurrentJobData.Name);
            }
        }

        public DbFrameRow CurrentFrame
        {
            get
            {
                if(Job.Frames.Any(f => f.Name == CurrentJobData.Frame))
                    return Job.Frames.First(f => f.Name == CurrentJobData.Frame);

                return null;   
            }
        }

        public DbMoveParamRow CurrentMoveParam
        {
            get
            {
                if (Job.MoveParams.Any(f => f.Name == CurrentJobData.MoveParam))
                    return Job.MoveParams.First(f => f.Name == CurrentJobData.MoveParam);

                return null;
            }
        }

        public DbToolRow CurrentTool
        {
            get
            {
                if (Job.Tools.Any(f => f.Name == CurrentJobData.Tool))
                    return Job.Tools.First(f => f.Name == CurrentJobData.Tool);

                return null;
            }
        }


        public bool IsPosData
        {
            get { return CurrentPosData != null; }
        }
        public bool IsProcData
        {
            get { return CurrentRobotData != null; }
        }

        public int MaxProcRobotStep
        {
            get
            {
                List<int> maxNums = new List<int>();
                if(Job.LaserData.Where(d => d.Name == CurrentJobData.Name).ToList().Count > 0)
                    maxNums.Add(Job.LaserData.Where(d => d.Name == CurrentJobData.Name).ToList().Max((i) => int.Parse(i.Step)));
                if (Job.PulseData.Where(d => d.Name == CurrentJobData.Name).ToList().Count > 0)
                    maxNums.Add(Job.PulseData.Where(d => d.Name == CurrentJobData.Name).ToList().Max((i) => int.Parse(i.Step)));
                if (Job.RobotData.Where(d => d.Name == CurrentJobData.Name).ToList().Count > 0)
                    maxNums.Add(Job.RobotData.Where(d => d.Name == CurrentJobData.Name).ToList().Max((i) => int.Parse(i.Step)));
                if (Job.TurnData.Where(d => d.Name == CurrentJobData.Name).ToList().Count > 0)
                    maxNums.Add(Job.TurnData.Where(d => d.Name == CurrentJobData.Name).ToList().Max((i) => int.Parse(i.Step)));
                return maxNums.Count > 0 ? maxNums.Max() : 0;
            }
        }

        public int MaxProcPLCStep
        {
            get
            {
                return Job.PLCData.Max((i) => int.Parse(i.Step));
            }
        }

        public bool SimulationOver
        {
            get
            {
                return JobDataStep >= Job.JobData.Count;
            }
        }


        public JobDataToJobSturctureConverter(LSC1JobData job)
        {
            Job = job;
        }

        public void NextJobDataStep()
        {
            JobDataStep++;
            JobDataChanged = true;
        }

        public void NextProcStep()
        {
            ProcStep++;
        }
        
        public bool JobDataChanged
        { get; private set; }

        public LSC1StructuredJob<InstructionStep> Convert()
        {
            LSC1StructuredJob<InstructionStep> data = new LSC1StructuredJob<InstructionStep>
            {
                JobInfo = Job.JobName
            };
            var firstStep = new LSC1JobDataStep<InstructionStep>
            {
                JobDataStepRow = CurrentJobData
            };
            data.JobDataSteps.Add(firstStep);

            var currentJobDataStep = firstStep;
            while (!SimulationOver)
            {
                if(JobDataChanged)
                {
                    currentJobDataStep = new LSC1JobDataStep<InstructionStep>
                    {
                        Frame = CurrentFrame,
                        MoveParam = CurrentMoveParam,
                        Tool = CurrentTool,
                        JobDataStepRow = CurrentJobData
                    };
                    data.JobDataSteps.Add(currentJobDataStep);
                }


                //Proc Robot und Proc LaserData werden ausgeführt
                if (CurrentJobData.What == "proc" && CurrentJobData.Who == "robot")
                {
                    currentJobDataStep.StepInstructions.Add(new InstructionStep()
                    { 
                         Instructions = new List<DbRow>() { CurrentRobotData, CurrentLaserData },
                    });
                }

                //Pos wird ausgeführt
                else if(CurrentJobData.What == "pos" && CurrentJobData.Who == "robot")
                {
                    currentJobDataStep.StepInstructions.Add(new InstructionStep()
                    {
                        Instructions = new List<DbRow>() { CurrentPosData }
                    });
                }

                else if(CurrentJobData.What == "sequence")
                {
                    currentJobDataStep.StepInstructions.Add(new InstructionStep()
                    {
                        Instructions = new List<DbRow>() { CurrentPLCData }
                    });
                }

                else if(CurrentJobData.Who == "plc" && (CurrentJobData.What == "laser" ||CurrentJobData.What == "?"))
                {
                    currentJobDataStep.StepInstructions.Add(new InstructionStep() { });
                }
                else
                    currentJobDataStep.StepInstructions.Add(new InstructionStep());

                SimulateNextStep();
            }

            return data;
        }

        //Setzt die Indices für den Nächsten Schritt.
        public void SimulateNextStep()
        {
            JobDataChanged = false;

            if (CurrentJobData.What == "proc")
            {
                NextProcStep();
                //Checken ob Proc beendet ist
                if (ProcStep > MaxProcRobotStep)
                {
                    ProcStep = 1;
                    NextJobDataStep();
                }
            }
            else if (CurrentJobData.What == "sequence")
            {
                NextProcStep();

                if (ProcStep > MaxProcPLCStep)
                {
                    ProcStep = 1;
                    NextJobDataStep();
                }
            }
            else if (CurrentJobData.What == "pos" || CurrentJobData.What == "?")
                NextJobDataStep();
            else
                NextJobDataStep();
            
        }
    }
}
