using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Progress
{
    public class InvalidStatusException : ApplicationException { public InvalidStatusException(string message) : base(message) { } }

    private List<Progress> SubProcesses;

    Action updateCallback = null;
    private double globalStatus = 0;
    public double Status
    {
        get
        {
            if (SubProcesses.Count > 0)
            {
                globalStatus = 0;

                foreach (Progress process in SubProcesses)
                    if (process != null)
                        globalStatus += process.Status / SubProcesses.Count;
            }

            return globalStatus;
        }

        set
        {
            if (SubProcesses.Count > 0)
                throw new InvalidStatusException("Cannot set status of progress that contains sub processes.");

            globalStatus = value;
            updateCallback();
        }
    }

    private Progress(int subProcessCount = 0)
    {
        SubProcesses = new List<Progress>();

        for (int i = 0; i < subProcessCount; i++)
            SubProcesses.Add(new Progress());
    }

    public Progress(Action updateCallback, int subProcessCount = 0) : this(subProcessCount)
    {
        this.updateCallback = updateCallback;
    }

    public Progress(Progress parent, int subProcessCount = 0) : this(subProcessCount)
    {
        parent.SubProcesses.Add(this);
        this.updateCallback = parent.updateCallback;
    }
}