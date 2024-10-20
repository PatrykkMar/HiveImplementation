using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IStateStrategy
{
    List<ButtonHelper> GetAvailableButtonsList();
    string InformationText { get; }
    string Scene { get; }
    public virtual void OnEntry()
    {

    }

    public virtual void OnExit()
    {

    }

    public virtual void OnInsectButtonClick(InsectType insect)
    {

    }

    public virtual void OnHexClick(VertexDTO hex)
    {

    }
}
