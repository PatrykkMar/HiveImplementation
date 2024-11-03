using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IStateStrategy
{
    List<ButtonHelper> GetAvailableButtonsList();
    string InformationText { get; }
    string Scene { get; }
    public virtual void OnStateEntry()
    {

    }

    public virtual void OnStateExit()
    {

    }

    public virtual void OnInsectButtonClick(InsectType insect)
    {

    }

    public virtual void OnHexClick(VertexDTO hex)
    {

    }

    public virtual void OnHexMove(VertexDTO hex)
    {
        if(!hex.isempty)
            ServiceLocator.Services.EventAggregator.InvokeMinorInformationTextReceived(Enum.GetName(typeof(InsectType), hex.insect), 3f);
    }
}
