namespace Synthwave.Core.Classes.Core.Interfaces;

public interface IFocusable
{
    bool IsFocused { get; set; }
    void OnFocus();
    void OnUnfocus();
    void OnConfirm();
}
