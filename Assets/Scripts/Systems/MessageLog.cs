
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

public class MessageLog
{
    private StringBuilder str;
    private static Text text;
    int count;

    private static readonly int _maxLines = 6;
    private readonly Queue<string> _lines;

    public MessageLog()
    {
        _lines = new Queue<string>();
        str = new StringBuilder();
        text = Game.text;
    }

    public void Add(string messate)
    {
        _lines.Enqueue(messate);
        if (_lines.Count > _maxLines)
            _lines.Dequeue();
    }

    // Draw each line of the MessageLog queue to the console
    public void Draw()
    {
        str.Length = 0;
        string[] lines = _lines.ToArray();
        for (int i = 0; i < lines.Length; i++)
        {
            str.Append(lines[i] + "\n");
        }
        text.text = str.ToString();

    }
}

