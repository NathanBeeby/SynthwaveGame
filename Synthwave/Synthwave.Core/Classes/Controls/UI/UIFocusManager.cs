using Synthwave.Core.Classes.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Controls.UI;

public class UIFocusManager
{
    private List<IFocusable> _items = [];
    private int _index;

    public void SetItems(List<IFocusable> items)
    {
        _items = items;
        _index = 0;

        for (int i = 0; i < _items.Count; i++)
            _items[i].IsFocused = i == 0;
    }

    public void Update(UIInputState input)
    {
        if (_items.Count == 0) return;

        if (input.LeftStick.Y > 0.5f)
            Move(-1);

        if (input.LeftStick.Y < -0.5f)
            Move(1);

        if (input.ConfirmPressed)
            _items[_index].OnConfirm();
    }

    private void Move(int dir)
    {
        _items[_index].OnUnfocus();
        _items[_index].IsFocused = false;

        _index = Math.Clamp(_index + dir, 0, _items.Count - 1);

        _items[_index].IsFocused = true;
        _items[_index].OnFocus();
    }
}