namespace PofyTools.NameGenerator
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.IO;
    using System.Text;
    using System.Globalization;
    using PofyTools.Distribution;

    [System.Serializable]
    public class SemanticData : IInitializable
    {
        public const string TAG = "<color=green><b>SemanticData: </b></color>";

        #region Constructors

        public SemanticData () { }

        public SemanticData (string fullPath)
        {
            this._fullPath = fullPath;
        }

        #endregion

        #region Serializable Data

        [Header ("Database Version")]
        public string dataVersion = "0.0";

        [Header ("Name Sets")]
        public List<NameSet> setNames = new List<NameSet> ();
        //[Header ("Title Sets")]
        //public List<TitleSet> setTitles = new List<TitleSet> ();
        [Header ("Grammar Sets")]
        public List<GrammarSet> setGrammars = new List<GrammarSet> ();

        [Header ("Syllable Generator")]
        public List<string> vowels = new List<string> ();
        public List<string> vowelPairs = new List<string> ();

        public List<string> consonantStart = new List<string> ();
        public List<string> consonantOpen = new List<string> ();
        public List<string> consonantClose = new List<string> ();

        public List<string> maleEndSyllablesOpen = new List<string> ();
        public List<string> maleEndSyllablesClose = new List<string> ();
        public List<string> femaleEndSyllablesOpen = new List<string> ();
        public List<string> femaleEndSyllablesClose = new List<string> ();

        [Header ("Numbers")]
        public List<string> numberOrdinals = new List<string> ();
        public List<string> numberCardinals = new List<string> ();

        #endregion

        #region Runtimes

        [System.NonSerialized]
        protected string _fullPath;

        [System.NonSerialized]
        protected Dictionary<string, NameSet> _setNames = new Dictionary<string, NameSet> ();
        [System.NonSerialized]
        protected List<string> _setNameIds = new List<string> ();

        [System.NonSerialized]
        protected Dictionary<string, TitleSet> _setTitles = new Dictionary<string, TitleSet> ();
        [System.NonSerialized]
        protected List<string> _setTitleIds = new List<string> ();

        [System.NonSerialized]
        protected Dictionary<string, GrammarSet> _setGrammars = new Dictionary<string, GrammarSet> ();
        [System.NonSerialized]
        protected List<string> _setGrammarIds = new List<string> ();

        [System.NonSerialized]
        protected List<string> _allAdjectives = new List<string> ();
        [System.NonSerialized]
        protected List<string> _allNouns = new List<string> ();

        protected void CreateRuntimeCollections ()
        {
            this._allNouns.Clear ();
            this._allAdjectives.Clear ();

            this._setNames.Clear ();
            this._setNameIds.Clear ();

            this._setTitles.Clear ();
            this._setTitleIds.Clear ();

            this._setGrammars.Clear ();
            this._setGrammarIds.Clear ();

            foreach (var nameSet in this.setNames)
            {
                if (this._setNames.ContainsKey (nameSet.id))
                    Debug.LogWarning (TAG + "Id " + nameSet.id + " already present Name Sets. Owerwritting...");

                this._setNames[nameSet.id] = nameSet;
                this._setNameIds.Add (nameSet.id);
            }

            //foreach (var titleSet in this.setTitles)
            //{
            //    if (this._setTitles.ContainsKey (titleSet.id))
            //        Debug.LogWarning (TAG + "Id " + titleSet.id + " already present in Title Sets. Owerwritting...");

            //    this._setTitles[titleSet.id] = titleSet;
            //    this._setTitleIds.Add (titleSet.id);

            //    foreach (var adjective in titleSet.adjectives)
            //    {
            //        this._allAdjectives.Add (adjective);
            //    }

            //    foreach (var subjective in titleSet.subjectivesCons)
            //    {
            //        this._allNouns.Add (subjective);
            //    }

            //    foreach (var subjective in titleSet.subjectivesPros)
            //    {
            //        this._allNouns.Add (subjective);
            //    }

            //    foreach (var subjective in titleSet.subjectivesNeutral)
            //    {
            //        this._allNouns.Add (subjective);
            //    }

            //    foreach (var genetive in titleSet.genetives)
            //    {
            //        this._allNouns.Add (genetive);
            //    }

            //}

            foreach (var grammarset in this.setGrammars)
            {
                if (this._setGrammars.ContainsKey (grammarset.nounSingular))
                    Debug.LogWarning (TAG + "Id " + grammarset.nounSingular + " already present in Grammer Sets. Owerwritting...");

                this._setGrammars[grammarset.nounSingular] = grammarset;
                this._setGrammarIds.Add (grammarset.nounSingular);
            }
        }

        #endregion

        #region API

        //FIXME
        //        public string GenerateName(string nameSetId = "", string titleSetId = "", bool useAdjective = true, bool useSubjective = true, bool useGenetive = true, bool male = true)
        //        {
        //
        //            NameSet nameSet = null;
        //            TitleSet titleSet = null;
        //            string final = string.Empty;
        //            CultureInfo cultureInfo = new CultureInfo("en-US", false);
        //            TextInfo textInfo = cultureInfo.TextInfo;
        //
        //            if (string.IsNullOrEmpty(nameSetId))
        //            {
        //                nameSetId = this._setNameIds[Random.Range(0, this._setNameIds.Count - 1)];
        //            }
        //
        //            if (this._setNames.TryGetValue(nameSetId, out nameSet))
        //            {
        //                final = textInfo.ToTitleCase(nameSet.prefixes[Random.Range(0, nameSet.prefixes.Count - 1)].ToLower(cultureInfo));
        //                final += nameSet.sufixes[Random.Range(0, nameSet.sufixes.Count - 1)];
        //            }
        //
        //            if (this._setTitles.TryGetValue(titleSetId, out titleSet))
        //            {
        //                if (useAdjective || useSubjective)
        //                {
        //                    final += " the ";
        //
        //                    if (useAdjective)
        //                    {
        //                        final += textInfo.ToTitleCase(titleSet.adjectives[Random.Range(0, titleSet.adjectives.Count - 1)].ToLower(cultureInfo));
        //                        final += " ";
        //                    }
        //
        //                    if (useSubjective)
        //                    {
        //                        TitleSet opposingSet = null;
        //
        //                        bool opposing = Random.Range(0f, 1f) > 0.5f && this._setTitles.TryGetValue(titleSet.opposingId, out opposingSet);
        //                        if (opposing)
        //                        {
        //                            final += textInfo.ToTitleCase(opposingSet.objectivesNeutral[Random.Range(0, opposingSet.objectivesNeutral.Count - 1)].ToLower(cultureInfo));
        //                            final += textInfo.ToTitleCase(this.subjectiveCons[Random.Range(0, this.subjectiveCons.Count - 1)].ToLower(cultureInfo));
        //                        }
        //                        else
        //                        {
        //                            final += textInfo.ToTitleCase(titleSet.objectivesNeutral[Random.Range(0, titleSet.objectivesNeutral.Count - 1)].ToLower(cultureInfo));
        //                            final += textInfo.ToTitleCase(this.subjectivePros[Random.Range(0, this.subjectivePros.Count - 1)].ToLower(cultureInfo));
        //                        }
        //                    }
        //                }
        //
        //                if (useGenetive)
        //                    final += " of " + textInfo.ToTitleCase(titleSet.genetives[Random.Range(0, titleSet.genetives.Count - 1)].ToLower(cultureInfo));
        //            }
        //
        //
        //            //final = textInfo.ToTitleCase(final);
        //
        //            return final;
        //        }
        //TODO

        //


        public NameSet GetNameSet (string id)
        {
            NameSet nameset = null;
            this._setNames.TryGetValue (id, out nameset);
            return nameset;
        }

        public TitleSet GetTitleSet (string id)
        {
            TitleSet titleset = null;
            this._setTitles.TryGetValue (id, out titleset);
            return titleset;
        }

        //public string GenerateStoryName (bool useAdjective = true, bool useSubjective = true, bool useGenetive = true)
        //{
        //    string result = string.Empty;

        //    if (useSubjective)
        //    {
        //        result = "the ";
        //        if (useAdjective)
        //        {
        //            result += GetAdjective () + " ";
        //        }

        //        result += this.subjectiveStory.GetRandom ();
        //    }
        //    if (useGenetive)
        //    {
        //        //TODO: pick genetive from other titlesets
        //        //                result += " of the ";
        //        //                result += this._allNouns.GetRandom();
        //        result += " " + GetGenetive ();
        //    }
        //    result = result.Trim ();
        //    if (string.IsNullOrEmpty (result))
        //        result = "NULL(story)";
        //    return result;
        //}

        //public string GenerateGeolocationName (bool usePrefix = true, bool useSubjective = true, bool useGenetive = true)
        //{
        //    string result = string.Empty;

        //    if (Chance.FiftyFifty)
        //    {

        //        return this.GetNameSet ("town").GetRandomName ();
        //    }

        //    if (useSubjective)
        //    {
        //        //Prefix
        //        if (usePrefix)
        //            result += GetPrefix ();

        //        //Subjective
        //        result += this.subjectiveGeolocation.GetRandom ();
        //    }

        //    //Genetive
        //    if (useGenetive && !usePrefix)
        //    {
        //        //TODO: pick genetive from other titlesets
        //        //                result += " of the ";
        //        //                result += this._allNouns.GetRandom();
        //        result += " " + GetGenetive (forceSingular: true, useOrdinals: false, nameSet: "");
        //    }
        //    result = result.Trim ();
        //    if (string.IsNullOrEmpty (result))
        //        result = "NULL(story)";
        //    return result;
        //}

        public string GetGenetive (bool forceSingular = false, bool useOrdinals = true, string nameSet = "")
        {
            GrammarSet grammarset = null;
            string result = "of ";
            string genetive = string.Empty;
            bool plural = !forceSingular && Chance.FiftyFifty;

            bool useAdjective = Chance.TryWithChance (0.3f);

            if (!plural)
            {

                grammarset = this.setGrammars.GetRandom ();
                if (grammarset.useDeterminer || useAdjective)
                {
                    result += "the ";
                    genetive = grammarset.nounSingular;
                }

                grammarset = this.setGrammars.GetRandom ();
                result += (useOrdinals && Chance.FiftyFifty) ? this.numberOrdinals.GetRandom () + " " : "";
                if (useAdjective)
                    result += grammarset.adjectives.GetRandom () + " ";
                result += genetive;
            }
            else
            {
                result += (Chance.FiftyFifty) ? this.numberCardinals.GetRandom () + " " : "";
                grammarset = this.setGrammars.GetRandom ();
                while (grammarset.nounPlurals.Count == 0)
                {
                    grammarset = this.setGrammars.GetRandom ();
                }

                genetive = grammarset.nounPlurals.GetRandom ();
                if (useAdjective)
                {
                    grammarset = this.setGrammars.GetRandom ();
                    result += grammarset.adjectives.GetRandom () + " ";
                }
                result += genetive;
            }
            return result;
        }

        public string GetRandomOrdinalNumber (int max = 1000)
        {
            return (Chance.TryWithChance (0.3f)) ? GetOrdinalNumber (Random.Range (4, max + 1)) : this.numberOrdinals.GetRandom ();
        }

        public string GetAdjective (bool plural = false, bool useNumbers = true, bool usePossessiveApostrophe = true, string nameSet = "")
        {
            string result = string.Empty;

            //Numbers
            if (useNumbers && Chance.TryWithChance (0.3f))
            {
                if (plural)
                    result += this.numberCardinals.GetRandom ();
                else
                    result += this.numberOrdinals.GetRandom ();
                return result;
            }

            //Name
            if (Chance.FiftyFifty)
            {
                string name = (nameSet == "") ? GetAnyName (Chance.FiftyFifty) : GetNameSet (nameSet).GetRandomName ();
                if (!string.IsNullOrEmpty (name))
                    result += (usePossessiveApostrophe) ? NameToAdjective (name) : name;
                else
                {
                    Debug.LogError (TAG + "Empty name from GetAnyName!");
                    result += this._allAdjectives.GetRandom ();
                }
            }
            else
            {
                if (Chance.FiftyFifty)
                    result += this._allAdjectives.GetRandom ();
                else
                    result += this.setGrammars.GetRandom ().adjectives.GetRandom ();
            }

            return result;
        }

        public string GetPrefix ()
        {
            string result = string.Empty;

            result = this.setGrammars.GetRandom ().nounSingular;

            return result;
        }

        public string GetAnyName (bool isMale = true)
        {
            if (Chance.FiftyFifty)
            {
                Debug.LogError (TAG + "Getting Name from name data...");
                return this.setNames.GetRandom ().GetRandomName (isMale);
            }
            Debug.LogError (TAG + "Generating true random name...");
            return GenerateTrueRandomName (3, isMale);
        }

        public static string NameToAdjective (string name)
        {
            return name + "\'s";
        }

        #region Syllable Generator

        public string GenerateTrueRandomName (int maxSyllables = 3, bool isMale = true)
        {
            if (maxSyllables == 0)
                return "[zero syllables]";

            int syllableCount = Random.Range (1, maxSyllables + 1);
            Debug.LogError (TAG + syllableCount + " syllables.");
            int[] syllableLengths = GetSyllableLenghts (syllableCount);
            bool[] syllablesTypes = GetSyllableTypes (syllableLengths);
            string[] syllablesStrings = GetSyllableStrings (syllablesTypes, syllableLengths, isMale);

            string name = ConcatanateSyllables (syllablesStrings);
            return name;
        }

        public int[] GetSyllableLenghts (int syllableCount = 1)
        {
            int[] lenghts = new int[syllableCount];

            for (int i = 0; i < lenghts.Length; i++)
            {
                lenghts[i] = Random.Range (2, 4);
                Debug.LogError (lenghts[i].ToString ());
            }

            return lenghts;
        }

        public bool[] GetSyllableTypes (int[] syllableLengths)
        {
            bool[] syllableTypes = new bool[syllableLengths.Length];

            for (var i = 0; i < syllableLengths.Length; i++)
            {
                if (syllableLengths[i] < 3 || Chance.FiftyFifty)
                {
                    syllableTypes[i] = true;
                }
                else
                {
                    syllableTypes[i] = false;
                }
                Debug.LogError (syllableTypes[i].ToString ());
            }
            return syllableTypes;
        }

        public string[] GetSyllableStrings (bool[] types, int[] lengths, bool isMale = true)
        {
            string[] syllableStrings = new string[types.Length];

            for (var i = 0; i < types.Length; i++)
            {
                string result = string.Empty;

                //if it's a first syllable
                if (i == 0)
                {
                    //Try for vowel on start
                    if (types[i])
                    {
                        if (types.Length > 1 && Chance.TryWithChance (0.3f))
                        {
                            result = this.vowels.GetRandom ();
                            syllableStrings[i] = result;
                            continue;
                        }
                        result = this.consonantStart.GetRandom ();
                        result += this.vowels.GetRandom ();
                        syllableStrings[i] = result;
                        continue;
                    }

                    if (lengths[i] > 2)
                    {
                        result = this.consonantOpen.GetRandom ();
                        result += this.vowels.GetRandom ();
                        result += this.consonantClose.GetRandom ();
                        syllableStrings[i] = result;
                        continue;
                    }

                    result = this.vowels.GetRandom ();
                    result += this.consonantClose.GetRandom ();
                    syllableStrings[i] = result;
                    continue;
                }
                //if it's last
                else if (i == (types.Length - 1))
                {

                    if (isMale)
                        result = (types[i - 1]) ? this.maleEndSyllablesOpen.GetRandom () : this.maleEndSyllablesClose.GetRandom ();
                    else
                        result = (types[i - 1]) ? this.femaleEndSyllablesOpen.GetRandom () : this.femaleEndSyllablesClose.GetRandom ();

                    syllableStrings[i] = result;
                    continue;
                }
                //middle syllables
                if (types[i])
                {
                    result = this.consonantOpen.GetRandom ();
                    result += this.vowels.GetRandom ();
                    syllableStrings[i] = result;
                    continue;
                }

                if (lengths[i] > 2)
                {
                    result = this.consonantOpen.GetRandom ();
                    result += this.vowels.GetRandom ();
                    result += this.consonantClose.GetRandom ();
                    syllableStrings[i] = result;
                    continue;
                }

                result = this.vowels.GetRandom ();
                result += this.consonantClose.GetRandom ();
                syllableStrings[i] = result;
                continue;

            }
            foreach (var value in syllableStrings)
            {
                Debug.LogError (value);
            }
            return syllableStrings;
        }

        protected string ConcatanateSyllables (string[] syllables)
        {
            string result = string.Empty;
            string left, right;
            for (int i = 0; i < syllables.Length; ++i)
            {

                if (i > 0)
                {
                    left = syllables[i - 1];
                    right = syllables[i];
                    if (left[left.Length - 1] == right[0])
                    {
                        right.PadRight (1);
                    }
                }

                result += syllables[i];
            }

            return result;
        }

        #endregion

        public static string GetOrdinalNumber (int number)
        {
            int remainder = number % 10;
            if (number < 10 || number > 20)
            {
                if (remainder == 1)
                {
                    return number + "st";
                }
                else if (remainder == 2)
                {
                    return number + "nd";
                }
                else if (remainder == 3)
                {
                    return number + "rd";
                }
                else
                {
                    return number + "th";
                }
            }
            return number + "th";
        }

        #endregion

        #region IInitializable

        public bool Initialize ()
        {
            if (!this.isInitialized)
            {
                SemanticData.LoadData (this);
                CreateRuntimeCollections ();
                this.isInitialized = true;
                return true;
            }
            return false;
        }

        public bool isInitialized
        {
            get;
            protected set;
        }

        #endregion

        #region File IO
        public void Load ()
        {
            SemanticData.LoadData (this);
        }

        public void Load (string path)
        {
            this._fullPath = path;
            Load ();
        }

        public void Save ()
        {
            SemanticData.SaveData (this);
        }

        public void Save (string path)
        {
            this._fullPath = path;
            Save ();
        }

        public static void LoadData (SemanticData data)
        {
            DataUtility.LoadOverwrite (Application.dataPath + data._fullPath, data);
            data.PostLoad ();

        }

        public void PostLoad ()
        {
            foreach (var nameset in this.setNames)
            {
                for (int i = 0; i < nameset.prefixes.Count; i++)
                {
                    nameset.prefixes[i] = nameset.prefixes[i].ToLower ();
                }

                nameset.prefixes.Sort ();


                for (int i = 0; i < nameset.sufixes.Count; i++)
                {
                    nameset.sufixes[i] = nameset.sufixes[i].ToLower ();
                }

                nameset.sufixes.Sort ();
            }

            //foreach (var titleset in this.setTitles)
            //{
            //    for (int i = 0; i < titleset.adjectives.Count; i++)
            //    {
            //        titleset.adjectives[i] = titleset.adjectives[i].ToLower ();
            //    }

            //    titleset.adjectives.Sort ();

            //    for (int i = 0; i < titleset.genetives.Count; i++)
            //    {
            //        titleset.genetives[i] = titleset.genetives[i].ToLower ();
            //    }

            //    titleset.genetives.Sort ();

            //    for (int i = 0; i < titleset.objectivesNeutral.Count; i++)
            //    {
            //        titleset.objectivesNeutral[i] = titleset.objectivesNeutral[i].ToLower ();
            //    }

            //    titleset.objectivesNeutral.Sort ();

            //    for (int i = 0; i < titleset.objectivesNeutral.Count; i++)
            //    {
            //        titleset.objectivesNeutral[i] = titleset.objectivesNeutral[i].ToLower ();
            //    }

            //    titleset.objectivesNeutral.Sort ();
            //}

            //            this.subjectiveCons.Sort();
            //            this.subjectivePros.Sort();
            //this.subjectiveStory.Sort ();
            //this.subjectiveGeolocation.Sort ();

        }

        public static void SaveData (SemanticData data)
        {
            data.PreSave ();
            DataUtility.Save (Application.dataPath + data._fullPath, data);
        }

        public void PreSave ()
        {
            //Optimize (this.subjectiveStory);
            //Optimize (this.subjectiveGeolocation);

            Optimize (this.vowels);
            Optimize (this.vowelPairs);

            Optimize (this.consonantStart);
            Optimize (this.consonantOpen);
            Optimize (this.consonantClose);

            Optimize (this.maleEndSyllablesOpen);
            Optimize (this.maleEndSyllablesClose);
            Optimize (this.femaleEndSyllablesOpen);
            Optimize (this.femaleEndSyllablesClose);

            this.setNames.Sort ((x, y) => x.id.CompareTo (y.id));
            foreach (var nameset in this.setNames)
            {
                Optimize (nameset.prefixes);
                Optimize (nameset.sufixes);
                Optimize (nameset.names);
                Optimize (nameset.adjectives);
                Optimize (nameset.genetives);
                Optimize (nameset.presets);
                Optimize (nameset.synonyms);

                nameset.concatenationRules.Sort ((x, y) => x.left.CompareTo (y.left));
            }

            //this.setTitles.Sort ((x, y) => x.id.CompareTo (y.id));
            //foreach (var titleset in this.setTitles)
            //{
            //    Optimize (titleset.adjectives);
            //    Optimize (titleset.genetives);
            //    Optimize (titleset.objectivePros);
            //    Optimize (titleset.objectivesNeutral);
            //    Optimize (titleset.subjectivesCons);
            //    Optimize (titleset.subjectivesNeutral);
            //    Optimize (titleset.subjectivesPros);

            //}

            this.setGrammars.Sort ((x, y) => x.nounSingular.CompareTo (y.nounSingular));
            foreach (var grammarset in this.setGrammars)
            {
                OptimizeString (grammarset.nounSingular);

                Optimize (grammarset.nounPlurals);
                Optimize (grammarset.adjectives);
            }
        }

        public static List<string> Optimize (List<string> toOptimize)
        {
            toOptimize.Sort ();
            for (int i = toOptimize.Count - 1; i >= 0; --i)
            {
                toOptimize[i] = toOptimize[i].Trim ().ToLower ();
                if (i < toOptimize.Count - 1)
                {
                    var left = toOptimize[i];
                    var right = toOptimize[i + 1];
                    if (left == right)
                    {
                        toOptimize.RemoveAt (i);
                    }
                }
            }
            return toOptimize;
        }

        public static string OptimizeString (string toOptimize)
        {
            return toOptimize.Trim ().ToLower ();
        }

        #endregion
    }

    [System.Serializable]
    public class NameSet
    {
        #region DATA
        //subjective
        public string id;

        /// <summary>
        /// Synonyms for the subjective.
        /// </summary>
        public List<string> synonyms = new List<string> ();

        /// <summary>
        /// Prefixes for concatenation with subjective (or synonym) 
        /// </summary>
        public List<string> subjectivePrefixes = new List<string> ();

        public List<string> adjectives = new List<string> ();

        public List<string> genetives = new List<string> ();

        /// <summary>
        /// The prefixes for pseudo names.
        /// </summary>
        public List<string> prefixes = new List<string> ();
        /// <summary>
        /// The sufixes for pseudo names.
        /// </summary>
        public List<string> sufixes = new List<string> ();

        /// <summary>
        /// The real name database.
        /// </summary>
        public List<string> names = new List<string> ();

        public List<string> presets = new List<string> ();

        /// <summary>
        /// The concatenation rules for generating pseudo names.
        /// </summary>
        public List<GrammarRule> concatenationRules = new List<GrammarRule> ();

        /// <summary>
        /// The gender conversion rules for generating pseudo names.
        /// </summary>
        public List<GrammarRule> genderConversionRules = new List<GrammarRule> ();

        /// <summary>
        /// List of instruction sequences used for generating a title.
        /// </summary>
        public List<ConstructionRule> titleConstructionRules = new List<ConstructionRule> ();
        #endregion

        #region API
        /// <summary>
        /// Gets eather a random real or pseudo name.
        /// </summary>
        /// <returns>The random real or pseudo name.</returns>
        /// <param name="male">Should random name be male or female name.</param>
        public string GetRandomName (bool male = true)
        {
            if (this.prefixes.Count + this.sufixes.Count == 0)
            {
                Debug.LogError ("No prefixes or sufixes in name set " + this.id);
                return GetName ();
            }

            if (Chance.FiftyFifty)
                return GeneratePseudoName (true);
            return GetName ();
        }

        /// <summary>
        /// Gets the real name from name set database.
        /// </summary>
        /// <returns>A real name from the database.</returns>
        /// <param name="male">Should real name be male or female name.</param>
        public string GetName ()
        {
            if (this.names.Count != 0)
                return this.names.GetRandom ();

            return "NULL(" + id + ")";
        }

        /// <summary>
        /// Generates a random pseudo name by concatenating prefix and sufix and by applying grammer rules.
        /// </summary>
        /// <returns>A pseudo name.</returns>
        /// <param name="male">Should pseudo name be male or female name.</param>
        public string GeneratePseudoName (bool male = true)
        {
            //string result = string.Empty;

            string prefix = this.prefixes.GetRandom ();
            string sufix = this.sufixes.GetRandom ();

            char prefixEnd = default (char);
            char sufixStart = default (char);

            bool dirty = this.concatenationRules.Count > 0;
            while (dirty)
            {
                dirty = false;
                foreach (var rule in this.concatenationRules)
                {
                    if (prefix.Length == 0 || sufix.Length == 0)
                        break;
                    prefixEnd = prefix[prefix.Length - 1];
                    sufixStart = sufix[0];

                    if (rule.left == prefixEnd && rule.right == sufixStart)
                    {
                        switch (rule.type)
                        {
                            case GrammarRule.Type.RemoveLeft:
                                prefix = prefix.Remove (prefix.Length - 1, 1);
                                break;
                            case GrammarRule.Type.RemoveRight:
                                sufix = sufix.Remove (0, 1);
                                break;
                            case GrammarRule.Type.ReplaceLeft:
                                prefix = prefix.Remove (prefix.Length - 1);
                                prefix = prefix.Insert (prefix.Length - 1, rule.affix);
                                break;
                            case GrammarRule.Type.ReplaceRight:
                                sufix = sufix.Remove (0);
                                sufix = sufix.Insert (0, rule.affix);
                                break;
                            case GrammarRule.Type.Insert:
                                prefix += rule.affix;
                                break;
                            case GrammarRule.Type.Append:
                                sufix += rule.affix;
                                break;
                            case GrammarRule.Type.MergeInto:
                                prefix = prefix.Remove (prefix.Length - 1);
                                sufix = sufix.Remove (0);
                                prefix += rule.affix;
                                break;
                            default:
                                break;
                        }
                        dirty = true;
                        break;
                    }
                }
            }

            dirty = !male && this.genderConversionRules.Count > 0;
            while (dirty)
            {
                dirty = false;
                foreach (var rule in this.genderConversionRules)
                {
                    if (prefix.Length == 0 || sufix.Length == 0)
                        break;
                    prefixEnd = prefix[prefix.Length - 1];
                    sufixStart = sufix[0];

                    if (rule.left == prefixEnd && rule.right == sufixStart)
                    {
                        switch (rule.type)
                        {
                            case GrammarRule.Type.RemoveLeft:
                                prefix = prefix.Remove (prefix.Length - 1, 1);
                                break;
                            case GrammarRule.Type.RemoveRight:
                                sufix = sufix.Remove (0, 1);
                                break;
                            case GrammarRule.Type.ReplaceLeft:
                                prefix = prefix.Remove (prefix.Length - 1);
                                prefix = prefix.Insert (prefix.Length - 1, rule.affix);
                                break;
                            case GrammarRule.Type.ReplaceRight:
                                sufix = sufix.Remove (0);
                                sufix = sufix.Insert (0, rule.affix);
                                break;
                            case GrammarRule.Type.Insert:
                                prefix += rule.affix;
                                break;
                            case GrammarRule.Type.Append:
                                sufix += rule.affix;
                                break;
                            case GrammarRule.Type.MergeInto:
                                prefix = prefix.Remove (prefix.Length - 1);
                                sufix = sufix.Remove (0);
                                prefix += rule.affix;
                                break;
                            default:
                                break;
                        }
                        dirty = true;
                        break;
                    }
                }
            }

            return prefix + sufix;
        }

        public string GenerateTitle ()
        {
            return NameSet.Construct (this, this.titleConstructionRules.GetRandom ());
        }

        #endregion

        #region STATIC API

        public static string Construct (NameSet set, ConstructionRule rule)
        {
            string result = string.Empty;

            foreach (var instruction in rule.instructions)
            {
                switch (instruction)
                {
                    case ConstructionInstruction.DETERMINER:
                        result += " the";
                        break;
                    case ConstructionInstruction.SEPARATOR:
                        result += " ";
                        break;
                    case ConstructionInstruction.NAME_FULL:
                        result += set.names.GetRandom ();
                        break;
                    case ConstructionInstruction.NAME_PARTIAL_PREFIX:
                        result += set.prefixes.GetRandom ();
                        break;
                    case ConstructionInstruction.NEME_PARTIAL_SUFIX:
                        result += set.sufixes.GetRandom ();
                        break;
                    case ConstructionInstruction.SUBJECTIVE_PREFIX:
                        result += set.subjectivePrefixes.GetRandom ();
                        break;
                    //case ConstructionInstruction.SUBJECTIVE_ORIGINAL:
                    //    result += set.id;
                    //break;
                    case ConstructionInstruction.SUBJECTIVE_ORIGINAL_OR_SYNONYM:
                        result += set.synonyms.GetRandom ();
                        break;
                    case ConstructionInstruction.ADJECTIVE:
                        result += set.adjectives.GetRandom ();
                        break;
                    case ConstructionInstruction.GENETIVE:
                        result += set.genetives.GetRandom ();
                        break;
                    case ConstructionInstruction.PRESET:
                        result += set.presets.GetRandom ();
                        break;
                    case ConstructionInstruction.PREPOSITION_OF:
                        result += "of ";
                        break;
                    case ConstructionInstruction.POSSESSIVE_APOSTROPHE:
                        result += "\'s";
                        break;
                }
            }

            return result;
        }

        #endregion
    }

    [System.Serializable]
    public class TitleSet
    {
        public string id;
        public string opposingId;

        public List<string> adjectives = new List<string> ();

        public List<string> objectivePros = new List<string> ();
        public List<string> objectivesNeutral = new List<string> ();

        public List<string> subjectivesPros = new List<string> ();
        public List<string> subjectivesCons = new List<string> ();
        public List<string> subjectivesNeutral = new List<string> ();

        public List<string> genetives = new List<string> ();
    }

    [System.Serializable]
    public class GrammarSet
    {
        //also used as id
        public string nounSingular;
        public bool useDeterminer = true;
        public List<string> nounPlurals;
        public List<string> adjectives;
    }

    [System.Serializable]
    public class GrammarRule
    {
        public enum Type : int
        {
            RemoveLeft,
            RemoveRight,
            ReplaceLeft,
            ReplaceRight,
            Insert,
            Append,
            MergeInto,
        }

        public char left;
        public char right;
        public string affix;
        public Type type;
    }

    public enum ConstructionInstruction
    {
        POSSESSIVE_APOSTROPHE = -3,
        PREPOSITION_OF = -2,
        DETERMINER = -1,
        SEPARATOR = 0,

        NAME_FULL = 1,
        //NAME_FULL_FEMALE,

        NAME_PARTIAL_PREFIX = 3,
        NEME_PARTIAL_SUFIX = 4,

        SUBJECTIVE_PREFIX = 5,
        //SUBJECTIVE_ORIGINAL = 6,
        SUBJECTIVE_ORIGINAL_OR_SYNONYM = 7,

        ADJECTIVE = 8,
        GENETIVE = 9,

        PRESET = 10,
    }

    [System.Serializable]
    public class ConstructionRule
    {
        public string name;
        public ConstructionInstruction[] instructions;
    }

    [System.Serializable]
    public class SemanticConnection
    {
        public string noun;

        public List<string> adjectives;
    }

    public class Adjective
    {

    }

    public class Noun
    {

    }

    public class Verb
    {

    }

    public class Adverb
    {

    }

}