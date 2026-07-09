using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components;

public class DemultiplexerTest
{
    [Theory]
    [InlineData(LogicState.Low, LogicState.High, LogicState.Low)]
    [InlineData(LogicState.High, LogicState.Low, LogicState.High)]
    [InlineData(LogicState.Unknown, LogicState.Unknown, LogicState.Unknown)]
    public void SimulateExampleScenarios(
        LogicState select,
        LogicState expectedOutput0,
        LogicState expectedOutput1
    )
    {
        var demux = new Demultiplexer { Input = new Terminal { State = LogicState.High } };
        demux.Selects.Add(new Terminal { State = select });
        demux.Outputs.Add(new Terminal());
        demux.Outputs.Add(new Terminal());

        demux.Simulate();

        Assert.Equal(expectedOutput0, demux.Outputs[0].State);
        Assert.Equal(expectedOutput1, demux.Outputs[1].State);
    }

    [Fact]
    public void SimulateResetScenario()
    {
        var demux = new Demultiplexer { Input = new Terminal() };
        demux.Selects.Add(new Terminal());
        demux.Outputs.Add(new Terminal { State = LogicState.High });
        demux.Outputs.Add(new Terminal { State = LogicState.High });

        demux.Reset();

        Assert.Equal(LogicState.Unknown, demux.Outputs[0].State);
        Assert.Equal(LogicState.Unknown, demux.Outputs[1].State);
    }
}
