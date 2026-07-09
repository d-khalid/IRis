using IRis.Models.CircuitObjects.Components.Gates;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components.Gates;

public class NandGateTest
{
    [Theory]
    [InlineData(LogicState.High, LogicState.High, LogicState.Low)]
    [InlineData(LogicState.High, LogicState.Low, LogicState.High)]
    [InlineData(LogicState.Low, LogicState.High, LogicState.High)]
    [InlineData(LogicState.Low, LogicState.Low, LogicState.High)]
    [InlineData(LogicState.Unknown, LogicState.High, LogicState.Unknown)]
    public void SimulateExampleScenarios(LogicState a, LogicState b, LogicState expected)
    {
        var gate = new NandGate { Output = new Terminal() };
        gate.Inputs.Add(new Terminal { State = a });
        gate.Inputs.Add(new Terminal { State = b });

        gate.Simulate();

        Assert.Equal(expected, gate.Output.State);
    }

    [Fact]
    public void SimulateResetScenario()
    {
        var gate = new NandGate { Output = new Terminal { State = LogicState.High } };
        gate.Inputs.Add(new Terminal());
        gate.Inputs.Add(new Terminal());

        gate.Reset();

        Assert.Equal(LogicState.Unknown, gate.Output.State);
    }
}
