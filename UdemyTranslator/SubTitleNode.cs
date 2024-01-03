namespace UdemyTranslator;

public class SubTitleNode
{
    public SubTitleNode()
    {
        this.Lines = new List<string>();
        this.PlaintextLines = new List<string>();
    }

    public int StartTime { get; set; }
    public int EndTime { get; set; }
    public List<string> Lines { get; set; }
    public List<string> PlaintextLines { get; set; }

    public override string ToString()
    {
        var startTs = new TimeSpan(0, 0, 0, 0, StartTime);
        var endTs = new TimeSpan(0, 0, 0, 0, EndTime);

        var res = string.Format("{0} --> {1}: {2}", startTs.ToString("G"), endTs.ToString("G"),
            string.Join(Environment.NewLine, Lines));
        return res;
    }
}