using System.Collections.Generic;

[System.Serializable]
public class Phoneme
{
    public string phoneme;
    public string grapheme;
}

[System.Serializable]
public class DictionaryEntry
{
    public int id;
    public string word;
    public string as_in;
    public List<string> flags;
    public List<string> errors;
    public bool hidden;
    public string locale;
    public List<Phoneme> phonics;
    // Add other fields as needed...
}

[System.Serializable]
public class Word
{
    public string ident;
    public string text;
    public string sentences;
    public string definitions;
    public string phonics;
    public string errors;
    public float difficulty_index;
    public string morphemes;
    public DictionaryEntry dictionary;
}

[System.Serializable]
public class ListData
{
    public int id;
    public string ident;
    public string s_code;
    public string title;
    public List<Word> words;
}

[System.Serializable]
public class RootObject
{
    public ListData list;
}
