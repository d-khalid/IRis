using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components;

public class ToggleTest
{
    [Theory]
    [InlineData(LogicState.High)]
    [InlineData(LogicState.Low)]
    public void SimulateExampleScenarios(LogicState state)
    {
        var toggle = new Toggle { Output = new Terminal(), State = state };

        toggle.Simulate();

        Assert.Equal(state, toggle.Output.State);
    }
}
