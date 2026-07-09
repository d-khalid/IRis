using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components;

public class ClockTest
{
    [Theory]
    [InlineData(LogicState.High)]
    [InlineData(LogicState.Low)]
    public void SimulateExampleScenarios(LogicState state)
    {
        var clock = new Clock { Output = new Terminal(), State = state };

        clock.Simulate();

        Assert.Equal(state, clock.Output.State);
    }

    [Fact]
    public void SimulateResetScenario()
    {
        var clock = new Clock
        {
            Output = new Terminal { State = LogicState.High },
            State = LogicState.High,
        };

        clock.Reset();

        Assert.Equal(LogicState.Unknown, clock.State);
        Assert.Equal(LogicState.Unknown, clock.Output.State);
    }
}
