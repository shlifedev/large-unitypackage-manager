using System.Text;

namespace UdemyTranslator.Interfaces;

public interface ISubParser
{
    List<SubtitleItem> ParseStream(Stream streamg);
}