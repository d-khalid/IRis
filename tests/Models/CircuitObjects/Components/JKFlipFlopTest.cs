using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components;

public class JKFlipFlopTest
{
    [Fact]
    public void SimulateExampleScenarios()
    {
        var ff = new JKFlipFlop
        {
            J = new Terminal { State = LogicState.High },
            K = new Terminal { State = LogicState.Low },
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

        ff.J.State = LogicState.Low;
        ff.K.State = LogicState.High;
        ff.Clk.State = LogicState.Low;
        ff.Simulate();
        ff.Clk.State = LogicState.High;
        ff.Simulate();
        Assert.Equal(LogicState.Low, ff.Q.State);
        Assert.Equal(LogicState.High, ff.QBar.State);

        ff.J.State = LogicState.High;
        ff.K.State = LogicState.High;
        ff.Clk.State = LogicState.Low;
        ff.Simulate();
        ff.Clk.State = LogicState.High;
        ff.Simulate();
        Assert.Equal(LogicState.High, ff.Q.State);
        Assert.Equal(LogicState.Low, ff.QBar.State);

        ff.Clr.State = LogicState.High;
        ff.Simulate();
        Assert.Equal(LogicState.Low, ff.Q.State);
        Assert.Equal(LogicState.High, ff.QBar.State);
    }

    [Fact]
    public void SimulateResetScenario()
    {
        var ff = new JKFlipFlop
        {
            J = new Terminal(),
            K = new Terminal(),
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
