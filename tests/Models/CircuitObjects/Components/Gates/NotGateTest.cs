using IRis.Models.CircuitObjects.Components.Gates;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components.Gates;

public class NotGateTest
{
    [Theory]
    [InlineData(LogicState.High, LogicState.Low)]
    [InlineData(LogicState.Low, LogicState.High)]
    [InlineData(LogicState.Unknown, LogicState.Unknown)]
    public void SimulateExampleScenarios(LogicState input, LogicState expected)
    {
        var gate = new NotGate
        {
            Output = new Terminal(),
            Input = new Terminal { State = input },
        };

        gate.Simulate();

        Assert.Equal(expected, gate.Output.State);
    }

    [Fact]
    public void SimulateResetScenario()
    {
        var gate = new NotGate
        {
            Output = new Terminal { State = LogicState.High },
            Input = new Terminal(),
        };

        gate.Reset();

        Assert.Equal(LogicState.Unknown, gate.Output.State);
    }
}
