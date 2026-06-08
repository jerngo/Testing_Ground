using System;
using System.Collections.Generic;

[Serializable]
public class LocalizationData
{
    public List<LocalizationItem> items;
}

[Serializable]
public class LocalizationItem
{
    public string key;
    public string value;
}