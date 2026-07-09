using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components;

public class DFlipFlopTest
{
    [Fact]
    public void SimulateExampleScenarios()
    {
        var ff = new DFlipFlop
        {
            D = new Terminal { State = LogicState.High },
            Clk = new Terminal { State = LogicState.Low },
            Set = new Terminal(),
            Clr = new Terminal(),
            Q = new Terminal(),
            QBar = new Terminal(),
        };

        ff.Simulate();
        Assert.Equal(LogicState.Unknown, ff.Q.State);

        ff.Clk.State = LogicState.High;
        ff.Simulate();
        Assert.Equal(LogicState.High, ff.Q.State);
        Assert.Equal(LogicState.Low, ff.QBar.State);

        ff.D.State = LogicState.Low;
        ff.Simulate();
        Assert.Equal(LogicState.High, ff.Q.State);

        ff.Clk.State = LogicState.Low;
        ff.Simulate();
        ff.Clk.State = LogicState.High;
        ff.Simulate();
        Assert.Equal(LogicState.Low, ff.Q.State);
        Assert.Equal(LogicState.High, ff.QBar.State);

        ff.Set.State = LogicState.High;
        ff.Simulate();
        Assert.Equal(LogicState.High, ff.Q.State);
        Assert.Equal(LogicState.Low, ff.QBar.State);

        ff.Set.State = LogicState.Low;
        ff.Clr.State = LogicState.High;
        ff.Simulate();
        Assert.Equal(LogicState.Low, ff.Q.State);
        Assert.Equal(LogicState.High, ff.QBar.State);
    }

    [Fact]
    public void SimulateResetScenario()
    {
        var ff = new DFlipFlop
        {
            D = new Terminal(),
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
