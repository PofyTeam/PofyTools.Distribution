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
    public RectTransform leftTerms, rightTerms;
    public Dropdown dropdownNamesets;
    public Text label;
    public Text input;

    [Header ("Data")]
    public SemanticData data;

    [Header ("Influncer")]
    public NameSet influencer;

    void Awake ()
    {
        Load ();
        this.data.Initialize ();

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

    public void AskRemoveSynonym (string synonym)
    {
        this._cacheSynonym = synonym;

        this.dialog.ShowDialog ("Remove Synonym", "Are you sure you want to delete \"" + synonym + "\"?", this.RemoveChachedSynonym);
    }
    private string _cacheSynonym;

    public void RemoveChachedSynonym ()
    {
        RemoveSynonym (this._cacheSynonym);
    }

    public void RemoveSynonym (string synonym)
    {
        this._currentNameSet.synonyms.Remove (synonym);
        Save ();
        RefeshAll ();
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
    public void AskRemovePreset (string preset)
    {
        this._cachePreset = preset;

        this.dialog.ShowDialog ("Remove Synonym", "Are you sure you want to delete \"" + preset + "\"?", this.RemoveChachedPreset);
    }
    private string _cachePreset;

    public void RemoveChachedPreset ()
    {
        RemovePreset (this._cachePreset);
    }

    public void RemovePreset (string preset)
    {
        this._currentNameSet.presets.Remove (preset);
        Save ();
        RefeshAll ();
    }
    #endregion

    void OnDropdownValueChanged (int index)
    {
        this._currentNameSet = this.data.setNames[index];

        RefeshAll ();
    }

    private NameSet _currentNameSet;

    void RefeshAll ()
    {
        ClearAll ();
        PopulateAll ();
    }

    void ClearAll ()
    {
        this.leftTerms.ClearChildren ();
        this.rightTerms.ClearChildren ();
        this.synonyms.ClearChildren ();
        this.presets.ClearChildren ();

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
            button.image.rectTransform.SetParent (this.leftTerms, false);
            button.GetComponentInChildren<Text> ().text = term;
            button.onClick.AddListener (delegate () { this._currentNameSet.adjectiveKeys.Add (term); Save (); RefeshAll (); });
        }

        foreach (var term in this._currentNameSet.adjectiveKeys)
        {
            Button button = Instantiate<Button> (this.buttonPrefab);
            button.image.rectTransform.SetParent (this.rightTerms, false);
            button.GetComponentInChildren<Text> ().text = term;
            button.onClick.AddListener (delegate () { this._currentNameSet.adjectiveKeys.Remove (term); Save (); RefeshAll (); });

        }

        foreach (var synonym in this._currentNameSet.synonyms)
        {
            Button button = Instantiate<Button> (this.buttonPrefab);
            button.image.rectTransform.SetParent (this.synonyms, false);
            button.GetComponentInChildren<Text> ().text = synonym;
            button.onClick.AddListener (delegate () { AskRemoveSynonym (synonym); });

        }

        foreach (var preset in this._currentNameSet.presets)
        {
            Button button = Instantiate<Button> (this.buttonPrefab);
            button.image.rectTransform.SetParent (this.presets, false);
            button.GetComponentInChildren<Text> ().text = preset;
            button.onClick.AddListener (delegate () { AskRemovePreset (preset); });

        }
    }

    public void GenerateTitle ()
    {
        if (this._currentNameSet != null)
        {
            this.label.text = this._currentNameSet.GenerateTitle (this.data);
        }
    }

    public void SavePreset ()
    {

        if (this._currentNameSet != null)
        {
            this._currentNameSet.presets.Add (this.label.text);
            Save ();
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
