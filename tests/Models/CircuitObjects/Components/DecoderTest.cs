using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components;

public class DecoderTest
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
        var decoder = new Decoder();
        decoder.Selects.Add(new Terminal { State = select });
        decoder.Outputs.Add(new Terminal());
        decoder.Outputs.Add(new Terminal());

        decoder.Simulate();

        Assert.Equal(expectedOutput0, decoder.Outputs[0].State);
        Assert.Equal(expectedOutput1, decoder.Outputs[1].State);
    }

    [Fact]
    public void SimulateResetScenario()
    {
        var decoder = new Decoder();
        decoder.Selects.Add(new Terminal());
        decoder.Outputs.Add(new Terminal { State = LogicState.High });
        decoder.Outputs.Add(new Terminal { State = LogicState.High });

        decoder.Reset();

        Assert.Equal(LogicState.Unknown, decoder.Outputs[0].State);
        Assert.Equal(LogicState.Unknown, decoder.Outputs[1].State);
    }
}
