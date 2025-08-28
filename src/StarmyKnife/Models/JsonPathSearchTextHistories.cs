using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Models
{
    public class JsonPathSearchTextHistories : HistoriesBase
    {
        private const string Delimiter = "\v";
        private const int MaxHistoryCount = 10;

        public JsonPathSearchTextHistories(UserSettings userSettings) : base(userSettings, MaxHistoryCount, Delimiter)
        {
        }

        protected override string GetTextFromUserSettings()
        {
            return UserSettings.JsonPathSearchHistoriesText;
        }

        protected override void SetTextToUserSettings(string histories)
        {
            UserSettings.JsonPathSearchHistoriesText = histories;
        }
    }
}
