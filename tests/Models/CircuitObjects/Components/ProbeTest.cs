using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components;

public class ProbeTest
{
    [Theory]
    [InlineData(LogicState.High)]
    [InlineData(LogicState.Low)]
    [InlineData(LogicState.Unknown)]
    public void SimulateExampleScenarios(LogicState input)
    {
        var probe = new Probe { Input = new Terminal { State = input } };

        probe.Simulate();

        Assert.Equal(input, probe.State);
    }

    [Fact]
    public void SimulateResetScenario()
    {
        var probe = new Probe
        {
            Input = new Terminal { State = LogicState.High },
            State = LogicState.High,
        };

        probe.Reset();

        Assert.Equal(LogicState.Unknown, probe.State);
    }
}
