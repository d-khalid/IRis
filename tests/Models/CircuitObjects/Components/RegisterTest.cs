using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components;

public class RegisterTest
{
    [Fact]
    public void SimulateExampleScenarios()
    {
        var reg = new Register
        {
            Clk = new Terminal { State = LogicState.Low },
            Set = new Terminal(),
            Clr = new Terminal(),
        };

        reg.Inputs.Add(new Terminal { State = LogicState.High });
        reg.Inputs.Add(new Terminal { State = LogicState.Low });
        reg.Outputs.Add(new Terminal());
        reg.Outputs.Add(new Terminal());
        reg.States.Add(LogicState.Unknown);
        reg.States.Add(LogicState.Unknown);

        reg.Simulate();
        Assert.Equal(LogicState.Unknown, reg.Outputs[0].State);
        Assert.Equal(LogicState.Unknown, reg.Outputs[1].State);

        reg.Clk.State = LogicState.High;
        reg.Simulate();
        Assert.Equal(LogicState.High, reg.Outputs[0].State);
        Assert.Equal(LogicState.Low, reg.Outputs[1].State);

        reg.Inputs[0].State = LogicState.Low;
        reg.Inputs[1].State = LogicState.High;
        reg.Simulate();
        Assert.Equal(LogicState.High, reg.Outputs[0].State);
        Assert.Equal(LogicState.Low, reg.Outputs[1].State);

        reg.Clk.State = LogicState.Low;
        reg.Simulate();
        reg.Clk.State = LogicState.High;
        reg.Simulate();
        Assert.Equal(LogicState.Low, reg.Outputs[0].State);
        Assert.Equal(LogicState.High, reg.Outputs[1].State);

        reg.Set.State = LogicState.High;
        reg.Simulate();
        Assert.Equal(LogicState.High, reg.Outputs[0].State);
        Assert.Equal(LogicState.High, reg.Outputs[1].State);

        reg.Set.State = LogicState.Low;
        reg.Clr.State = LogicState.High;
        reg.Simulate();
        Assert.Equal(LogicState.Low, reg.Outputs[0].State);
        Assert.Equal(LogicState.Low, reg.Outputs[1].State);
    }

    [Fact]
    public void SimulateResetScenario()
    {
        var reg = new Register
        {
            Clk = new Terminal(),
            Set = new Terminal(),
            Clr = new Terminal(),
        };

        reg.Inputs.Add(new Terminal());
        reg.Outputs.Add(new Terminal { State = LogicState.High });
        reg.States.Add(LogicState.High);

        reg.Reset();

        Assert.Equal(LogicState.Unknown, reg.Outputs[0].State);
        Assert.Equal(LogicState.Unknown, reg.States[0]);
    }
}
