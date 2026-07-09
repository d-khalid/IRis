using IRis.Models.CircuitObjects;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects;

public class WireTest
{
    [Theory]
    [InlineData(LogicState.High)]
    [InlineData(LogicState.Low)]
    [InlineData(LogicState.Unknown)]
    public void SimulateExampleScenarios(LogicState state)
    {
        var wire = new Wire
        {
            MainInput = new Terminal { State = state },
            MainOutput = new Terminal(),
        };

        wire.Simulate();

        Assert.Equal(state, wire.MainOutput.State);
    }

    [Fact]
    public void SimulateResetScenario()
    {
        var wire = new Wire
        {
            MainInput = new Terminal { State = LogicState.High },
            MainOutput = new Terminal { State = LogicState.High },
        };

        wire.Reset();

        Assert.Equal(LogicState.Unknown, wire.MainOutput.State);
    }
}
