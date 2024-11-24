using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IStateStrategy : IHexEventsHandling, IUIHandling, IStateLifecycle, IPlayerInsectViewHandling
{


}

public interface IUIHandling
{
    List<ButtonHelper> GetAvailableButtonsList();
    string InformationText { get; }
    string Scene { get; }
}

public interface IHexEventsHandling
{
    public virtual void OnHexClick(VertexDTO hex)
    {

    }

    public virtual void OnHexMove(VertexDTO hex)
    {
        if (!hex.isempty)
            ServiceLocator.Services.EventAggregator.InvokeMinorInformationTextReceived(Enum.GetName(typeof(InsectType), hex.insect), 3f);
    }
}

public interface IPlayerInsectViewHandling
{
    public virtual void OnInsectButtonClick(InsectType insect)
    {

    }

}

public interface IStateLifecycle
{
    public virtual void OnStateEntry()
    {

    }

    public virtual void OnStateExit()
    {

    }
}