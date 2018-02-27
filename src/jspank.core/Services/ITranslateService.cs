using System.Collections.Generic;

namespace jspank.core.services
{
    public interface ITranslateService
    {
        string Key { get; }

        ICollection<string> Translate(string value, string languagetarget = "");
    }
}
