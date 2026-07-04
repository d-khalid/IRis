using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components;

public class MultiplexerTest
{
    [Theory]
    [InlineData(LogicState.Low, LogicState.Low)]
    [InlineData(LogicState.High, LogicState.High)]
    [InlineData(LogicState.Unknown, LogicState.Unknown)]
    public void SimulateExampleScenarios(LogicState select, LogicState expected)
    {
        var mux = new Multiplexer { Output = new Terminal() };
        mux.Selects.Add(new Terminal { State = select });
        mux.Inputs.Add(new Terminal { State = LogicState.Low });
        mux.Inputs.Add(new Terminal { State = LogicState.High });

        mux.Simulate();

        Assert.Equal(expected, mux.Output.State);
    }
}
