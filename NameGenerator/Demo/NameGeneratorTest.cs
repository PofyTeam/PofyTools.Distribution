using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PofyTools.NameGenerator;
using UnityEngine.UI;
using PofyTools;
using PofyTools.Distribution;

public class NameGeneratorTest : MonoBehaviour
{

    public SemanticData data;

    public Text label;

    void Awake ()
    {
        this.data.Initialize ();
    }

    //    public void Generate()
    //    {
    //        this.label.text = this.data.GenerateName("angel", "good");
    //    }

    //public void GenerateGeoName ()
    //{
    //    string geoName = this.data.GenerateGeolocationName (Chance.FiftyFifty, true, Chance.FiftyFifty);
    //    this.label.text = geoName;
    //}

    public void GenerateLakeName ()
    {
        this.label.text = this.data.GetNameSet ("geo_lake").GenerateTitle ();
    }

    //public void GenerateStory ()
    //{
    //    bool useAdjective = true;
    //    //        Debug.LogError("useAdjective " + useAdjective);
    //    bool useSubjective = true;
    //    //        Debug.LogError("useSubjective " + useSubjective);
    //    bool useGenetive = true;
    //    //        Debug.LogError("useGenetive " + useGenetive);
    //    string story = this.data.GenerateStoryName (useAdjective, useSubjective, useGenetive);
    //    //        Debug.LogError(story);
    //    this.label.text = story.ToTitle ();

    //}

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
