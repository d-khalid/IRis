using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components;

public class PriorityEncoderTest
{
    [Theory]
    [InlineData(LogicState.High, LogicState.Low, LogicState.Low)]
    [InlineData(LogicState.Low, LogicState.High, LogicState.High)]
    [InlineData(LogicState.High, LogicState.High, LogicState.High)]
    [InlineData(LogicState.Low, LogicState.Low, LogicState.Low)]
    [InlineData(LogicState.Unknown, LogicState.Low, LogicState.Unknown)]
    public void SimulateExampleScenarios(LogicState input0, LogicState input1, LogicState expected)
    {
        var encoder = new PriorityEncoder();
        encoder.Inputs.Add(new Terminal { State = input0 });
        encoder.Inputs.Add(new Terminal { State = input1 });
        encoder.Outputs.Add(new Terminal());

        encoder.Simulate();

        Assert.Equal(expected, encoder.Outputs[0].State);
    }
}
