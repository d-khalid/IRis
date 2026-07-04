using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components;

public class FullAdderTest
{
    [Theory]
    [InlineData(LogicState.Low, LogicState.Low, LogicState.Low, LogicState.Low, LogicState.Low)]
    [InlineData(LogicState.High, LogicState.Low, LogicState.Low, LogicState.High, LogicState.Low)]
    [InlineData(LogicState.High, LogicState.High, LogicState.Low, LogicState.Low, LogicState.High)]
    [InlineData(
        LogicState.High,
        LogicState.High,
        LogicState.High,
        LogicState.High,
        LogicState.High
    )]
    [InlineData(
        LogicState.Unknown,
        LogicState.High,
        LogicState.Low,
        LogicState.Unknown,
        LogicState.Unknown
    )]
    public void SimulateExampleScenarios(
        LogicState a,
        LogicState b,
        LogicState cin,
        LogicState expectedSum,
        LogicState expectedCout
    )
    {
        var adder = new FullAdder
        {
            A = new Terminal { State = a },
            B = new Terminal { State = b },
            Cin = new Terminal { State = cin },
            Sum = new Terminal(),
            Cout = new Terminal(),
        };

        adder.Simulate();

        Assert.Equal(expectedSum, adder.Sum.State);
        Assert.Equal(expectedCout, adder.Cout.State);
    }
}
