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


    [Header ("UI")]
    public RectTransform leftTerms, rightTerms, synonms;
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
        this.synonms.ClearChildren ();

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

        foreach (var term in this._currentNameSet.synonyms)
        {
            Button button = Instantiate<Button> (this.buttonPrefab);
            button.image.rectTransform.SetParent (this.synonms, false);
            button.GetComponentInChildren<Text> ().text = term;
            //button.onClick.AddListener (delegate () { set.synonyms.Remove (term); Save (); ClearAll (); PopulateAll (set); });

        }
    }


    public void GenerateTitle ()
    {
        if (this._currentNameSet != null)
        {
            this.label.text = this._currentNameSet.GenerateTitle (this.data,this.influencer);
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

    public void GenerateTrueRandom ()
    {
        this.label.text = this.data.GenerateTrueRandomName ().ToTitle ();
    }

    public void GetAnyName ()
    {
        this.label.text = this.data.GetAnyName (Chance.FiftyFifty).ToTitle ();
    }

    public void GetAnySerbianName ()
    {
        this.label.text = this.data.GetNameSet ("town").GeneratePseudoName ().ToTitle ();
    }

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
}
