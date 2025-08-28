using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Models
{
    public class XPathSearchTextHistories : HistoriesBase
    {
        private const string Delimiter = "\v";
        private const int MaxHistoryCount = 10;

        public XPathSearchTextHistories(UserSettings userSettings) : base(userSettings, MaxHistoryCount, Delimiter)
        {
        }

        protected override string GetTextFromUserSettings()
        {
            return UserSettings.XPathSearchHistoriesText;
        }

        protected override void SetTextToUserSettings(string histories)
        {
            UserSettings.XPathSearchHistoriesText = histories;
        }
    }
}
