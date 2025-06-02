using UnityEngine;

public interface IPopup
{
    public void Show(string header, string message);
    public void Close();
}
