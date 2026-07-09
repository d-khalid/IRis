using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components;

public class TFlipFlopTest
{
    [Fact]
    public void SimulateExampleScenarios()
    {
        var ff = new TFlipFlop
        {
            T = new Terminal { State = LogicState.High },
            Clk = new Terminal { State = LogicState.Low },
            Set = new Terminal(),
            Clr = new Terminal(),
            Q = new Terminal { State = LogicState.Low },
            QBar = new Terminal { State = LogicState.High },
        };

        ff.Simulate();
        ff.Clk.State = LogicState.High;
        ff.Simulate();
        Assert.Equal(LogicState.High, ff.Q.State);
        Assert.Equal(LogicState.Low, ff.QBar.State);

        ff.Clk.State = LogicState.Low;
        ff.Simulate();
        ff.Clk.State = LogicState.High;
        ff.Simulate();
        Assert.Equal(LogicState.Low, ff.Q.State);
        Assert.Equal(LogicState.High, ff.QBar.State);

        ff.T.State = LogicState.Low;
        ff.Clk.State = LogicState.Low;
        ff.Simulate();
        ff.Clk.State = LogicState.High;
        ff.Simulate();
        Assert.Equal(LogicState.Low, ff.Q.State);

        ff.Set.State = LogicState.High;
        ff.Simulate();
        Assert.Equal(LogicState.High, ff.Q.State);
        Assert.Equal(LogicState.Low, ff.QBar.State);
    }

    [Fact]
    public void SimulateResetScenario()
    {
        var ff = new TFlipFlop
        {
            T = new Terminal(),
            Clk = new Terminal(),
            Set = new Terminal(),
            Clr = new Terminal(),
            Q = new Terminal { State = LogicState.High },
            QBar = new Terminal { State = LogicState.Low },
        };

        ff.Reset();

        Assert.Equal(LogicState.Unknown, ff.Q.State);
        Assert.Equal(LogicState.Unknown, ff.QBar.State);
    }
}
