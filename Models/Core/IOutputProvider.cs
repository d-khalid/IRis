namespace IRis.Models.Core;

public enum LogicState
{
    High,
    Low,
    DontCare,
}

interface IOutputProvider
{
    public void ComputeOutput();
}