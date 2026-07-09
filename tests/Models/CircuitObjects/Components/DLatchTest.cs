using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components;

public class DLatchTest
{
    [Fact]
    public void SimulateExampleScenarios()
    {
        var latch = new DLatch
        {
            D = new Terminal { State = LogicState.High },
            En = new Terminal { State = LogicState.High },
            Q = new Terminal(),
            QBar = new Terminal(),
        };

        latch.Simulate();
        Assert.Equal(LogicState.High, latch.Q.State);
        Assert.Equal(LogicState.Low, latch.QBar.State);

        latch.En.State = LogicState.Low;
        latch.D.State = LogicState.Low;
        latch.Simulate();
        Assert.Equal(LogicState.High, latch.Q.State);

        latch.En.State = LogicState.High;
        latch.Simulate();
        Assert.Equal(LogicState.Low, latch.Q.State);
        Assert.Equal(LogicState.High, latch.QBar.State);
    }

    [Fact]
    public void SimulateResetScenario()
    {
        var latch = new DLatch
        {
            D = new Terminal(),
            En = new Terminal(),
            Q = new Terminal { State = LogicState.High },
            QBar = new Terminal { State = LogicState.Low },
        };

        latch.Reset();

        Assert.Equal(LogicState.Unknown, latch.Q.State);
        Assert.Equal(LogicState.Unknown, latch.QBar.State);
    }
}
