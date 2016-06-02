using LSC1DatabaseLibrary.LSC1JobRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace LSC1DatabaseLibrary.LSC1Visualisation
{
    public class LSC1MachinePathPointGenerator
    {
        public List<Point3DCollection> GeneratePathPoints(LSC1StructuredJob<InstructionStepAndMachineState> job)
        {
            var linePoints = new List<Point3DCollection>();

            foreach (var InstructionSubItems in job.JobDataSteps)
            {
                Point3DCollection points = new Point3DCollection();
                foreach (var item in InstructionSubItems.StepInstructions)
                {
                    points.Add(item.MachineStatusAfterInstructions.TCPOrientation.Position);
                }

                linePoints.Add(points);
            }

            return linePoints;
        }
    }
}
