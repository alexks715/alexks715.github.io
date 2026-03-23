using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace DictionaryLibrary
{
    public class Variant19Search
    {
        private Dictionary dictionary;

        public Variant19Search(Dictionary dict)
        {
            dictionary = dict;
        }

        public List<string> FindWordsFromLetters(string sourceWord)
        {
            if (string.IsNullOrWhiteSpace(sourceWord))
                return new List<string>();

            sourceWord = sourceWord.ToLower();
            var results = new List<string>();

            Dictionary<char, int> sourceLetterCount = GetLetterCount(sourceWord);

            foreach (string word in dictionary.WordList)
            {
                if (word == sourceWord)
                    continue;

                if (CanFormWord(word, sourceLetterCount))
                {
                    results.Add(word);
                }
            }

            results = results.OrderBy(w => w.Length).ThenBy(w => w).ToList();
            return results;
        }

        private bool CanFormWord(string target, Dictionary<char, int> sourceLetters)
        {
            var availableLetters = new Dictionary<char, int>(sourceLetters);

            foreach (char c in target)
            {
                if (!availableLetters.ContainsKey(c) || availableLetters[c] <= 0)
                {
                    return false;
                }
                availableLetters[c]--;
            }
            return true;
        }

        private Dictionary<char, int> GetLetterCount(string word)
        {
            var result = new Dictionary<char, int>();

            foreach (char c in word)
            {
                if (result.ContainsKey(c))
                    result[c]++;
                else
                    result[c] = 1;
            }
            return result;
        }

        public void SaveResultsToFile(List<string> results, string filename)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                {
                    writer.WriteLine("Результаты поиска слов, которые можно составить из букв заданного слова:");
                    writer.WriteLine($"Найдено слов: {results.Count}");
                    writer.WriteLine(new string('-', 50));

                    foreach (string word in results)
                    {
                        writer.WriteLine(word);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения результатов: {ex.Message}");
            }
        }
    }
}