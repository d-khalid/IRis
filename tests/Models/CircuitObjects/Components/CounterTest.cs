using IRis.Models.CircuitObjects.Components;
using IRis.Models.Core;
using Xunit;

namespace IRis.Tests.Models.CircuitObjects.Components;

public class CounterTest
{
    private static Counter Create(int bits)
    {
        var counter = new Counter
        {
            Clk = new Terminal { State = LogicState.Low },
            Clr = new Terminal(),
            Load = new Terminal(),
            Enable = new Terminal { State = LogicState.High },
            Carry = new Terminal(),
        };

        for (int i = 0; i < bits; i++)
        {
            counter.Inputs.Add(new Terminal { State = LogicState.Low });
            counter.Outputs.Add(new Terminal());
            counter.States.Add(LogicState.Low);
        }

        return counter;
    }

    private static void RisingEdge(Counter counter)
    {
        counter.Clk.State = LogicState.Low;
        counter.Simulate();
        counter.Clk.State = LogicState.High;
        counter.Simulate();
    }

    [Fact]
    public void CountsOnRisingEdgeWhenEnabled()
    {
        var counter = Create(2);
        counter.Simulate();

        RisingEdge(counter);
        Assert.Equal(LogicState.High, counter.Outputs[0].State);
        Assert.Equal(LogicState.Low, counter.Outputs[1].State);
        Assert.Equal(LogicState.Low, counter.Carry.State);

        RisingEdge(counter);
        Assert.Equal(LogicState.Low, counter.Outputs[0].State);
        Assert.Equal(LogicState.High, counter.Outputs[1].State);
    }

    [Fact]
    public void EnableLowHoldsCount()
    {
        var counter = Create(2);
        RisingEdge(counter);

        counter.Enable.State = LogicState.Low;
        RisingEdge(counter);

        Assert.Equal(LogicState.High, counter.Outputs[0].State);
        Assert.Equal(LogicState.Low, counter.Outputs[1].State);
    }

    [Fact]
    public void LoadOverridesCount()
    {
        var counter = Create(2);
        counter.Inputs[0].State = LogicState.High;
        counter.Inputs[1].State = LogicState.High;
        counter.Load.State = LogicState.High;

        RisingEdge(counter);

        Assert.Equal(LogicState.High, counter.Outputs[0].State);
        Assert.Equal(LogicState.High, counter.Outputs[1].State);
        Assert.Equal(LogicState.Low, counter.Carry.State);
    }

    [Fact]
    public void ResetClearsAsync()
    {
        var counter = Create(2);
        RisingEdge(counter);
        RisingEdge(counter);

        counter.Clr.State = LogicState.High;
        counter.Simulate();

        Assert.Equal(LogicState.Low, counter.Outputs[0].State);
        Assert.Equal(LogicState.Low, counter.Outputs[1].State);
        Assert.Equal(LogicState.Low, counter.Carry.State);
    }

    [Fact]
    public void CarryOnOverflow()
    {
        var counter = Create(2);
        RisingEdge(counter);
        RisingEdge(counter);
        RisingEdge(counter);
        RisingEdge(counter);

        Assert.Equal(LogicState.Low, counter.Outputs[0].State);
        Assert.Equal(LogicState.Low, counter.Outputs[1].State);
        Assert.Equal(LogicState.High, counter.Carry.State);
    }

    [Fact]
    public void SimulateResetScenario()
    {
        var counter = Create(1);
        counter.States[0] = LogicState.High;
        counter.Outputs[0].State = LogicState.High;
        counter.Carry.State = LogicState.High;

        counter.Reset();

        Assert.Equal(LogicState.Unknown, counter.Outputs[0].State);
        Assert.Equal(LogicState.Unknown, counter.States[0]);
        Assert.Equal(LogicState.Unknown, counter.Carry.State);
    }
}
