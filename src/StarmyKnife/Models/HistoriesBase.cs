using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarmyKnife.Models
{
    public abstract class HistoriesBase
    {
        private readonly UserSettings _userSettings;
        private readonly string _delimiter;
        private readonly int _maxHistoryCount;

        private readonly LinkedList<string> _histories;

        public HistoriesBase(UserSettings userSettings, int maxHistoryCount, string delimiter)
        {
            _userSettings = userSettings;
            _delimiter = delimiter;
            _maxHistoryCount = maxHistoryCount;
            _histories = new LinkedList<string>();

            LoadHistories();
        }

        private protected UserSettings UserSettings => _userSettings;

        public void Push(string newHistory)
        {
            if (_histories.Contains(newHistory))
            {
                _histories.Remove(newHistory);
            }
            _histories.AddFirst(newHistory);

            while (_histories.Count > _maxHistoryCount)
            {
                _histories.RemoveLast();
            }

            SaveHistories();
        }

        public IReadOnlyList<string> GetAll()
        {
            return _histories.ToList().AsReadOnly();
        }

        protected abstract string GetTextFromUserSettings();
        protected abstract void SetTextToUserSettings(string histories);

        private void LoadHistories()
        {
            var historiesText = GetTextFromUserSettings();
            _histories.Clear();

            if (string.IsNullOrWhiteSpace(historiesText))
            {
                return;
            }

            var items = historiesText.Split(new[] { _delimiter }, StringSplitOptions.None);
            foreach (var item in items)
            {
                _histories.AddLast(item);

                if (_histories.Count >= _maxHistoryCount)
                {
                    break;
                }
            }
        }

        private void SaveHistories()
        {
            var historiesText = string.Join(_delimiter, _histories);
            SetTextToUserSettings(historiesText);
        }
    }
}
