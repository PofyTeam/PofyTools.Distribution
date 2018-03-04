using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PofyTools.NameGenerator;
using UnityEngine.UI;
using PofyTools;
using PofyTools.Distribution;

public class NameGeneratorTest : MonoBehaviour
{
    [Header ("Resources")]
    public Button buttonPrefab;

    [Header ("Dialog")]

    public Dialog dialog;

    [Header ("UI")]
    public RectTransform availableSets, selectedSets;
    public Dropdown dropdownNamesets;
    public Text label;
    public Text input;

    [Header ("Data")]
    public SemanticData data;

    [Header ("Influncer")]
    public NameSet influencerOne;
    public NameSet influencerTwo;

    private InfluenceSet _influenceSet;
    void Awake ()
    {
        Load ();
        this.data.Initialize ();
        this._influenceSet = new InfluenceSet (this.influencerOne, influencerTwo);
        this.dropdownNamesets.onValueChanged.AddListener (this.OnDropdownValueChanged);
        OnDropdownValueChanged (0);
        //RefeshAll ();
    }

    public void AddNamset ()
    {
        if (!string.IsNullOrEmpty (this.input.text) && this.data.GetNameSet (this.input.text) == null)
        {
            var ns = new NameSet ();
            ns.id = this.input.text;
            this.data.setNames.Add (ns);

            ns.titleConstructionRules.Add (ConstructionRule.PresetRule);
            ns.titleConstructionRules.Add (ConstructionRule.AdjectivePrefixSynonymRule);
            ns.titleConstructionRules.Add (ConstructionRule.AdjectiveSynonymRule);
            ns.titleConstructionRules.Add (ConstructionRule.SynonymAdjectiveRule);
            ns.titleConstructionRules.Add (ConstructionRule.SynonymGenetiveRule);
            //TODO: Add name instrucitons


            this._currentNameSet = ns;

            Save ();
            RefeshAll ();
        }
    }

    #region Synonyms
    [Header ("Synonym Editor")]
    public RectTransform synonyms;
    public Text synonymInput;

    public void AddSynonym ()
    {
        if (!string.IsNullOrEmpty (this.synonymInput.text) && !this._currentNameSet.synonyms.Contains (this.synonymInput.text))
        {
            this._currentNameSet.synonyms.Add (this.synonymInput.text);
            Save ();
            RefeshAll ();
        }
    }
    #endregion

    #region Presets
    [Header ("Presets Editor")]
    public RectTransform presets;
    public Text presetsInput;

    public void AddPreset ()
    {
        if (!string.IsNullOrEmpty (this.presetsInput.text) && !this._currentNameSet.presets.Contains (this.presetsInput.text))
        {
            this._currentNameSet.presets.Add (this.presetsInput.text);
            Save ();
            RefeshAll ();
        }
    }

    #endregion

    #region Names

    public RectTransform namePrefix, nameSufix, nameFull;

    #endregion

    void OnDropdownValueChanged (int index)
    {
        this._currentNameSet = this.data.setNames[index];

        RefeshAll ();
    }

    private NameSet _currentNameSet;

    #region API
    void RefeshAll ()
    {
        ClearAll ();
        PopulateAll ();
    }

    void ClearAll ()
    {
        this.availableSets.ClearChildren ();
        this.selectedSets.ClearChildren ();
        this.synonyms.ClearChildren ();
        this.presets.ClearChildren ();

        this.nameFull.ClearChildren ();
        this.namePrefix.ClearChildren ();
        this.nameSufix.ClearChildren ();

        this.dropdownNamesets.ClearOptions ();
    }

    void PopulateAll ()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData> ();
        foreach (var nameset in this.data.setNames)
        {
            options.Add (new Dropdown.OptionData (nameset.id));
        }

        this.dropdownNamesets.AddOptions (options);

        this.dropdownNamesets.value = (this._currentNameSet != null) ? this.data.setNames.IndexOf (this._currentNameSet) : 0;
        this.dropdownNamesets.RefreshShownValue ();

        List<string> notUsing = new List<string> (this.data.GetAllGrammarIds ());

        foreach (var term in this._currentNameSet.adjectiveKeys)
        {
            notUsing.Remove (term);
        }

        foreach (var term in notUsing)
        {
            Button button = Instantiate<Button> (this.buttonPrefab);
            button.image.rectTransform.SetParent (this.availableSets, false);
            button.GetComponentInChildren<Text> ().text = term;
            button.onClick.AddListener (delegate () { this._currentNameSet.adjectiveKeys.Add (term); Save (); RefeshAll (); });
        }

        //Adjective Keys (Grammer Set)
        foreach (var term in this._currentNameSet.adjectiveKeys)
        {
            Button button = Instantiate<Button> (this.buttonPrefab);
            button.image.rectTransform.SetParent (this.selectedSets, false);
            button.GetComponentInChildren<Text> ().text = term;
            button.onClick.AddListener (delegate () { this._currentNameSet.adjectiveKeys.Remove (term); Save (); RefeshAll (); });

        }

        //Synonyms
        foreach (var synonym in this._currentNameSet.synonyms)
        {
            Button button = Instantiate<Button> (this.buttonPrefab);
            button.image.rectTransform.SetParent (this.synonyms, false);
            button.GetComponentInChildren<Text> ().text = synonym;
            button.onClick.AddListener (delegate () { AskRemove (synonym, this._currentNameSet.synonyms); });

        }

        //Title Presets
        foreach (var preset in this._currentNameSet.presets)
        {
            Button button = Instantiate<Button> (this.buttonPrefab);
            button.image.rectTransform.SetParent (this.presets, false);
            button.GetComponentInChildren<Text> ().text = preset;
            button.onClick.AddListener (delegate () { AskRemove (preset, this._currentNameSet.presets); });

        }

        //Name Prefix
        foreach (var namePrefix in this._currentNameSet.prefixes)
        {
            Button button = Instantiate<Button> (this.buttonPrefab);
            button.image.rectTransform.SetParent (this.namePrefix, false);
            button.GetComponentInChildren<Text> ().text = namePrefix;
            button.onClick.AddListener (delegate () { AskRemove (namePrefix, this._currentNameSet.prefixes); });
        }

        //Name Sufix
        foreach (var nameSufix in this._currentNameSet.sufixes)
        {
            Button button = Instantiate<Button> (this.buttonPrefab);
            button.image.rectTransform.SetParent (this.nameSufix, false);
            button.GetComponentInChildren<Text> ().text = nameSufix;
            button.onClick.AddListener (delegate () { AskRemove (nameSufix, this._currentNameSet.sufixes); });
        }

        //Full Names
        foreach (var nameFull in this._currentNameSet.names)
        {
            Button button = Instantiate<Button> (this.buttonPrefab);
            button.image.rectTransform.SetParent (this.nameFull, false);
            button.GetComponentInChildren<Text> ().text = nameFull;
            button.onClick.AddListener (delegate () { AskRemove (nameFull, this._currentNameSet.names); });
        }

    }

    public void AskRemove (string key, List<string> list)
    {
        this._cacheString = key;
        this._cacheList = list;
        this.dialog.ShowDialog ("Remove", "Are you sure you want to delete \"" + key + "\"?", this.RemoveChached);
    }

    private string _cacheString;
    private List<string> _cacheList;

    public void RemoveChached ()
    {
        RemoveFrom (this._cacheString, this._cacheList);
    }

    public void RemoveFrom (string toRemove, List<string> from)
    {
        from.Remove (toRemove);
        Save ();
        RefeshAll ();
    }

    #endregion

    public void GenerateTitle ()
    {
        if (this._currentNameSet != null)
        {
            this.label.text = this._currentNameSet.GenerateTitle (this.data, this._influenceSet);
        }
    }

    public void SavePreset ()
    {

        if (this._currentNameSet != null)
        {
            this._currentNameSet.presets.Add (this.label.text);
            Save ();
            RefeshAll ();
        }
    }

    #region Name
    public void GenerateTrueRandom ()
    {
        this.label.text = this.data.GenerateTrueRandomName ().ToTitle ();
    }

    public void GetAnyName ()
    {
        this.label.text = this.data.GetAnyName (Chance.FiftyFifty).ToTitle ();
    }
    #endregion

    #region IO
    [ContextMenu ("Save")]
    public void Save ()
    {
        this.data.Save ("/definitions/semantic_data.json");
    }

    [ContextMenu ("Load")]
    public void Load ()
    {
        this.data.Load ("/definitions/semantic_data.json");
    }
    #endregion
}
