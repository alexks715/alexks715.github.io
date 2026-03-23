using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace DictionaryLibrary
{
    public class Dictionary
    {
        private List<string> list = new List<string>();
        private string filename;
        private int count;

        public Dictionary(string filename)
        {
            this.filename = filename;
            OpenFile();
            count = list.Count;
        }

        public int Count
        {
            get { return count; }
        }

        public List<string> WordList
        {
            get { return list; }
        }

        private void OpenFile()
        {
            try
            {
                list.Clear();
                if (!File.Exists(filename))
                {
                    throw new Exception($"Файл {filename} не найден!");
                }

                using (StreamReader f = new StreamReader(filename, Encoding.UTF8))
                {
                    while (!f.EndOfStream)
                    {
                        string word = f.ReadLine()?.Trim();
                        if (!string.IsNullOrEmpty(word))
                        {
                            list.Add(word);
                        }
                    }
                }
                list.Sort();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка доступа к файлу: {ex.Message}");
            }
        }

        public bool AddWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return false;

            word = word.Trim().ToLower();

            if (list.Contains(word))
                return false;

            list.Add(word);
            list.Sort();
            count = list.Count;
            return true;
        }

        public bool RemoveWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return false;

            word = word.Trim().ToLower();

            if (!list.Contains(word))
                return false;

            list.Remove(word);
            count = list.Count;
            return true;
        }

        public void SaveToFile(string outputFilename = null)
        {
            string savePath = outputFilename ?? filename;

            try
            {
                using (StreamWriter writer = new StreamWriter(savePath, false, Encoding.UTF8))
                {
                    foreach (string word in list)
                    {
                        writer.WriteLine(word);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения файла: {ex.Message}");
            }
        }

        public List<string> GetWordsStartingWith(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
                return new List<string>(list);

            prefix = prefix.ToLower();
            return list.Where(w => w.StartsWith(prefix)).ToList();
        }

        public List<string> FuzzySearch(string pattern, int maxDistance = 3)
        {
            if (string.IsNullOrEmpty(pattern))
                return new List<string>();

            pattern = pattern.ToLower();
            var results = new List<string>();

            foreach (string word in list)
            {
                int distance = LevenshteinDistance(pattern, word);
                if (distance <= maxDistance)
                {
                    results.Add(word);
                }
            }

            return results;
        }

        private int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 0; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
    }
}