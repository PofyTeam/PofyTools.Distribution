namespace PofyTools.NameGenerator
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.IO;
    using System.Text;
    using System.Globalization;
    using PofyTools.Distribution;
    using Extensions;
    using PofyTools.Data;

    [System.Serializable]
    public class SemanticData : IInitializable
    {
        public const string TAG = "<color=green><b>SemanticData: </b></color>";

        #region Constructors

        public SemanticData() { }

        public SemanticData(string path, string filename, bool scramble = false, bool encode = false, string extension = "")
        {
            this._path = path;
            this._filename = filename;
            this._scrable = scramble;
            this._encode = encode;
            this._extension = extension;
        }

        #endregion

        #region Serializable Data

        [Header("Database Version")]
        public string dataVersion = "0.0";

        [Header("Name Sets")]
        public List<NameSet> setNames = new List<NameSet>();
        //[Header ("Title Sets")]
        //public List<TitleSet> setTitles = new List<TitleSet> ();
        [Header("Grammar Sets")]
        public List<GrammarSet> setGrammars = new List<GrammarSet>();

        [Header("Syllable Generator")]
        public List<string> vowels = new List<string>();
        public List<string> vowelPairs = new List<string>();

        public List<string> consonantStart = new List<string>();
        public List<string> consonantOpen = new List<string>();
        public List<string> consonantClose = new List<string>();

        public List<string> maleEndSyllablesOpen = new List<string>();
        public List<string> maleEndSyllablesClose = new List<string>();
        public List<string> femaleEndSyllablesOpen = new List<string>();
        public List<string> femaleEndSyllablesClose = new List<string>();

        [Header("Numbers")]
        public List<string> numberOrdinals = new List<string>();
        public List<string> numberCardinals = new List<string>();

        #endregion

        #region Runtimes

        [System.NonSerialized]
        protected string _path;
        protected string _filename;
        protected string _extension;

        protected bool _scrable;
        protected bool _encode;

        public string FullPath
        {
            get
            {
                return this._path + "/" + this._filename + "." + this._extension;
            }
        }

        [System.NonSerialized]
        protected Dictionary<string, NameSet> _setNames = new Dictionary<string, NameSet>();
        [System.NonSerialized]
        protected List<string> _setNameIds = new List<string>();

        [System.NonSerialized]
        protected Dictionary<string, GrammarSet> _setGrammars = new Dictionary<string, GrammarSet>();
        [System.NonSerialized]
        protected List<string> _setGrammarIds = new List<string>();
        public List<string> GetAllGrammarIds() { return this._setGrammarIds; }
        [System.NonSerialized]
        protected List<string> _allAdjectives = new List<string>();
        [System.NonSerialized]
        protected List<string> _allNouns = new List<string>();

        protected void CreateRuntimeCollections()
        {
            this._allNouns.Clear();
            this._allAdjectives.Clear();

            this._setNames.Clear();
            this._setNameIds.Clear();

            //this._setTitles.Clear ();
            //this._setTitleIds.Clear ();

            this._setGrammars.Clear();
            this._setGrammarIds.Clear();

            foreach (var nameSet in this.setNames)
            {
                if (this._setNames.ContainsKey(nameSet.id))
                    Debug.LogWarning(TAG + "Id " + nameSet.id + " already present Name Sets. Owerwritting...");

                this._setNames[nameSet.id] = nameSet;
                this._setNameIds.Add(nameSet.id);
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
                if (this._setGrammars.ContainsKey(grammarset.nounSingular))
                    Debug.LogWarning(TAG + "Id " + grammarset.nounSingular + " already present in Grammer Sets. Owerwritting...");

                this._setGrammars[grammarset.nounSingular] = grammarset;
                this._setGrammarIds.Add(grammarset.nounSingular);
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

        public NameSet GetNameSet(string id)
        {
            NameSet nameset = null;
            this._setNames.TryGetValue(id, out nameset);
            return nameset;
        }

        //public TitleSet GetTitleSet (string id)
        //{
        //    TitleSet titleset = null;
        //    this._setTitles.TryGetValue (id, out titleset);
        //    return titleset;
        //}

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

        public string GetRandomOrdinalNumber(int max = 1000)
        {
            return (Chance.TryWithChance(0.3f)) ? GetOrdinalNumber(Random.Range(4, max + 1)) : this.numberOrdinals.TryGetRandom();
        }

        public string GetAdjective(bool plural = false, bool useNumbers = true, bool usePossessiveApostrophe = true, string nameSet = "")
        {
            string result = string.Empty;

            //Numbers
            if (useNumbers && Chance.TryWithChance(0.3f))
            {
                if (plural)
                    result += this.numberCardinals.TryGetRandom();
                else
                    result += this.numberOrdinals.TryGetRandom();
                return result;
            }

            //Name
            if (Chance.FiftyFifty)
            {
                string name = (nameSet == "") ? GetAnyName(Chance.FiftyFifty) : GetNameSet(nameSet).GetRandomName();
                if (!string.IsNullOrEmpty(name))
                    result += (usePossessiveApostrophe) ? NameToAdjective(name) : name;
                else
                {
                    Debug.LogError(TAG + "Empty name from GetAnyName!");
                    result += this._allAdjectives.TryGetRandom();
                }
            }
            else
            {
                if (Chance.FiftyFifty)
                    result += this._allAdjectives.TryGetRandom();
                else
                    result += this.setGrammars.TryGetRandom().adjectives.TryGetRandom();
            }

            return result;
        }

        public string GetAdjective(string key)
        {
            GrammarSet set = null;
            if (this._setGrammars.TryGetValue(key, out set))
                return set.adjectives.TryGetRandom();

            return key;
        }

        public string GetPrefix(string key = "")
        {

            if (key == "")
                return this.setGrammars.TryGetRandom().nounSingular;

            return key;
        }

        public string GetGenetive(bool forceSingular = false, bool useOrdinals = true, string nameSet = "")
        {
            GrammarSet grammarset = null;
            string result = "of ";
            string genetive = string.Empty;
            bool plural = !forceSingular && Chance.FiftyFifty;

            bool useAdjective = Chance.TryWithChance(0.3f);

            if (!plural)
            {

                grammarset = this.setGrammars.TryGetRandom();
                if (grammarset.useDeterminer || useAdjective)
                {
                    result += "the ";
                    genetive = grammarset.nounSingular;
                }

                grammarset = this.setGrammars.TryGetRandom();
                result += (useOrdinals && Chance.FiftyFifty) ? this.numberOrdinals.TryGetRandom() + " " : "";
                if (useAdjective)
                    result += grammarset.adjectives.TryGetRandom() + " ";
                result += genetive;
            }
            else
            {
                result += (Chance.FiftyFifty) ? this.numberCardinals.TryGetRandom() + " " : "";
                grammarset = this.setGrammars.TryGetRandom();
                while (grammarset.nounPlurals.Count == 0)
                {
                    grammarset = this.setGrammars.TryGetRandom();
                }

                genetive = grammarset.nounPlurals.TryGetRandom();
                if (useAdjective)
                {
                    grammarset = this.setGrammars.TryGetRandom();
                    result += grammarset.adjectives.TryGetRandom() + " ";
                }
                result += genetive;
            }
            return result;
        }

        public string GetGenetive(string key)
        {
            GrammarSet set = null;
            if (this._setGrammars.TryGetValue(key, out set))
            {
                return (Chance.FiftyFifty) ? (set.useDeterminer) ? "the " + set.nounSingular : set.nounSingular : set.nounPlurals.TryGetRandom();
            }
            return key;
        }

        public string GetAnyName(bool isMale = true)
        {
            if (Chance.FiftyFifty)
            {
                Debug.LogError(TAG + "Getting Name from name data...");
                return this.setNames.TryGetRandom().GetRandomName(isMale);
            }
            Debug.LogError(TAG + "Generating true random name...");
            return GenerateTrueRandomName(3, isMale);
        }

        public static string NameToAdjective(string name)
        {
            return name + "\'s";
        }

        #region Syllable Generator

        public string GenerateTrueRandomName(int maxSyllables = 3, bool isMale = true)
        {
            if (maxSyllables == 0)
                return "[zero syllables]";

            int syllableCount = Random.Range(1, maxSyllables + 1);
            Debug.LogError(TAG + syllableCount + " syllables.");
            int[] syllableLengths = GetSyllableLenghts(syllableCount);
            bool[] syllablesTypes = GetSyllableTypes(syllableLengths);
            string[] syllablesStrings = GetSyllableStrings(syllablesTypes, syllableLengths, isMale);

            string name = ConcatanateSyllables(syllablesStrings);
            return name;
        }

        public int[] GetSyllableLenghts(int syllableCount = 1)
        {
            int[] lenghts = new int[syllableCount];

            for (int i = 0; i < lenghts.Length; i++)
            {
                lenghts[i] = Random.Range(2, 4);
                Debug.LogError(lenghts[i].ToString());
            }

            return lenghts;
        }

        public bool[] GetSyllableTypes(int[] syllableLengths)
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
                Debug.LogError(syllableTypes[i].ToString());
            }
            return syllableTypes;
        }

        public string[] GetSyllableStrings(bool[] types, int[] lengths, bool isMale = true)
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
                        if (types.Length > 1 && Chance.TryWithChance(0.3f))
                        {
                            result = this.vowels.TryGetRandom();
                            syllableStrings[i] = result;
                            continue;
                        }
                        result = this.consonantStart.TryGetRandom();
                        result += this.vowels.TryGetRandom();
                        syllableStrings[i] = result;
                        continue;
                    }

                    if (lengths[i] > 2)
                    {
                        result = this.consonantOpen.TryGetRandom();
                        result += this.vowels.TryGetRandom();
                        result += this.consonantClose.TryGetRandom();
                        syllableStrings[i] = result;
                        continue;
                    }

                    result = this.vowels.TryGetRandom();
                    result += this.consonantClose.TryGetRandom();
                    syllableStrings[i] = result;
                    continue;
                }
                //if it's last
                else if (i == (types.Length - 1))
                {

                    if (isMale)
                        result = (types[i - 1]) ? this.maleEndSyllablesOpen.TryGetRandom() : this.maleEndSyllablesClose.TryGetRandom();
                    else
                        result = (types[i - 1]) ? this.femaleEndSyllablesOpen.TryGetRandom() : this.femaleEndSyllablesClose.TryGetRandom();

                    syllableStrings[i] = result;
                    continue;
                }
                //middle syllables
                if (types[i])
                {
                    result = this.consonantOpen.TryGetRandom();
                    result += this.vowels.TryGetRandom();
                    syllableStrings[i] = result;
                    continue;
                }

                if (lengths[i] > 2)
                {
                    result = this.consonantOpen.TryGetRandom();
                    result += this.vowels.TryGetRandom();
                    result += this.consonantClose.TryGetRandom();
                    syllableStrings[i] = result;
                    continue;
                }

                result = this.vowels.TryGetRandom();
                result += this.consonantClose.TryGetRandom();
                syllableStrings[i] = result;
                continue;

            }
            foreach (var value in syllableStrings)
            {
                Debug.LogError(value);
            }
            return syllableStrings;
        }

        protected string ConcatanateSyllables(string[] syllables)
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
                        right.PadRight(1);
                    }
                }

                result += syllables[i];
            }

            return result;
        }

        #endregion

        public static string GetOrdinalNumber(int number)
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

        public bool Initialize()
        {
            if (!this.IsInitialized)
            {
                SemanticData.LoadData(this.FullPath, this, this._scrable, this._encode);
                CreateRuntimeCollections();
                this.IsInitialized = true;
                return true;
            }
            return false;
        }

        public bool IsInitialized
        {
            get;
            protected set;
        }

        #endregion

        #region IO

        public static void LoadData(string path, SemanticData data, bool scramble = false, bool encode = false)
        {
            DataUtility.LoadOverwrite(path, data, scramble, encode);
            data.PostLoad();
        }

        public void PostLoad()
        {
            foreach (var nameset in this.setNames)
            {
                for (int i = 0; i < nameset.prefixes.Count; i++)
                {
                    nameset.prefixes[i] = nameset.prefixes[i].ToLower();
                }

                nameset.prefixes.Sort();


                for (int i = 0; i < nameset.sufixes.Count; i++)
                {
                    nameset.sufixes[i] = nameset.sufixes[i].ToLower();
                }

                nameset.sufixes.Sort();
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

        public static void SaveData(string fullPath, string filename, SemanticData data, bool scramble = false, bool encode = false, string extension = "")
        {
            data.PreSave();
            DataUtility.Save(fullPath, filename, data, scramble, encode, extension);
        }

        public void PreSave()
        {
            DataUtility.OptimizeStringList(this.vowels);
            DataUtility.OptimizeStringList(this.vowelPairs);

            DataUtility.OptimizeStringList(this.consonantStart);
            DataUtility.OptimizeStringList(this.consonantOpen);
            DataUtility.OptimizeStringList(this.consonantClose);

            DataUtility.OptimizeStringList(this.maleEndSyllablesOpen);
            DataUtility.OptimizeStringList(this.maleEndSyllablesClose);
            DataUtility.OptimizeStringList(this.femaleEndSyllablesOpen);
            DataUtility.OptimizeStringList(this.femaleEndSyllablesClose);

            this.setNames.Sort((x, y) => x.id.CompareTo(y.id));
            foreach (var nameset in this.setNames)
            {
                DataUtility.OptimizeStringList(nameset.prefixes);
                DataUtility.OptimizeStringList(nameset.sufixes);
                DataUtility.OptimizeStringList(nameset.adjectiveKeys);
                DataUtility.OptimizeStringList(nameset.presets);
                DataUtility.OptimizeStringList(nameset.synonyms);

                nameset.concatenationRules.Sort((x, y) => x.replace.CompareTo(y.replace));
            }

            this.setGrammars.Sort((x, y) => x.nounSingular.CompareTo(y.nounSingular));
            foreach (var grammarset in this.setGrammars)
            {
                DataUtility.OptimizeString(grammarset.nounSingular);

                DataUtility.OptimizeStringList(grammarset.nounPlurals);
                DataUtility.OptimizeStringList(grammarset.adjectives);
            }
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
        public List<string> synonyms = new List<string>();

        /// <summary>
        /// Prefixes for concatenation with subjective (or synonym) 
        /// </summary>
        public List<string> adjectiveKeys = new List<string>();

        /// <summary>
        /// The prefixes for pseudo names.
        /// </summary>
        public List<string> prefixes = new List<string>();
        /// <summary>
        /// The sufixes for pseudo names.
        /// </summary>
        public List<string> sufixes = new List<string>();

        /// <summary>
        /// The real name database.
        /// </summary>
        public List<string> names = new List<string>();

        public List<string> presets = new List<string>();

        /// <summary>
        /// The concatenation rules for generating pseudo names.
        /// </summary>
        public List<GrammarRule> concatenationRules = new List<GrammarRule>();

        /// <summary>
        /// The gender conversion rules for generating pseudo names.
        /// </summary>
        public List<GrammarRule> genderConversionRules = new List<GrammarRule>();

        /// <summary>
        /// List of instruction sequences used for generating a title.
        /// </summary>
        public List<ConstructionRule> titleConstructionRules = new List<ConstructionRule>();
        #endregion

        #region API
        /// <summary>
        /// Gets eather a random real or pseudo name.
        /// </summary>
        /// <returns>The random real or pseudo name.</returns>
        /// <param name="male">Should random name be male or female name.</param>
        public string GetRandomName(bool male = true)
        {
            if (this.prefixes.Count + this.sufixes.Count == 0)
            {
                Debug.LogError("No prefixes or sufixes in name set " + this.id);
                return GetName();
            }

            return GetName();
        }

        /// <summary>
        /// Gets the real name from name set database.
        /// </summary>
        /// <returns>A real name from the database.</returns>
        /// <param name="male">Should real name be male or female name.</param>
        public string GetName()
        {
            if (this.names.Count != 0)
                return this.names.TryGetRandom();

            return "NULL(" + id + ")";
        }

        public string GenerateTitle(SemanticData data = null, InfluenceSet influenceSet = null)
        {
            return NameSet.Construct(this, this.titleConstructionRules.TryGetRandom(), data, influenceSet);
        }

        #endregion

        #region STATIC API

        public static string Construct(NameSet set, ConstructionRule rule, SemanticData data = null, InfluenceSet influenceSet = null)
        {
            string result = string.Empty;

            bool hasData = data != null;
            bool hasInfluenceSet = influenceSet != null;

            foreach (var instruction in rule.instructions)
            {
                switch (instruction)
                {
                    case ConstructionInstruction.DETERMINER:
                        result += "the ";
                        break;
                    case ConstructionInstruction.SEPARATOR:
                        result += " ";
                        break;
                    case ConstructionInstruction.NAME_FULL:
                        result += (hasInfluenceSet && Chance.FiftyFifty && influenceSet.HasName) ? influenceSet.Name : set.names.TryGetRandom();
                        break;
                    case ConstructionInstruction.NAME_PARTIAL_PREFIX:
                        result += hasInfluenceSet && Chance.FiftyFifty && influenceSet.HasPrefix ? influenceSet.Prefix : set.prefixes.TryGetRandom();
                        break;
                    case ConstructionInstruction.NEME_PARTIAL_SUFIX:
                        result += hasInfluenceSet && Chance.FiftyFifty && influenceSet.HasSufix ? influenceSet.Sufix : set.sufixes.TryGetRandom();
                        break;
                    case ConstructionInstruction.SUBJECTIVE_ORIGINAL_OR_SYNONYM:
                        result += hasInfluenceSet && Chance.FiftyFifty && influenceSet.HasSynonym ? influenceSet.Synonym : set.synonyms.TryGetRandom();
                        break;
                    case ConstructionInstruction.ADJECTIVE:
                        //Consturcts adjective or fallsback to singular noun
                        result += (hasData) ?
                            data.GetAdjective(hasInfluenceSet && Chance.FiftyFifty && influenceSet.HasAdjectiveKey ?
                                influenceSet.AdjectiveKey : set.adjectiveKeys.TryGetRandom())
                            : set.adjectiveKeys.TryGetRandom();
                        break;
                    case ConstructionInstruction.GENETIVE:
                        //Using only singular noun
                        result += (hasInfluenceSet && Chance.FiftyFifty && influenceSet.HasAdjectiveKey) ? influenceSet.AdjectiveKey : set.adjectiveKeys.TryGetRandom();
                        break;
                    case ConstructionInstruction.PRESET:
                        result += hasInfluenceSet && Chance.FiftyFifty && influenceSet.HasPreset ? influenceSet.Preset : set.presets.TryGetRandom();
                        break;
                    case ConstructionInstruction.PREPOSITION_OF:
                        result += "of ";
                        break;
                    case ConstructionInstruction.POSSESSIVE_APOSTROPHE:
                        result += "\'s";
                        break;
                }
            }

            foreach (var concatenationRule in set.concatenationRules)
            {
                result = result.Replace(concatenationRule.replace, concatenationRule.with);
            }

            return result;
        }

        #endregion
    }

    public class InfluenceSet
    {
        public InfluenceSet() { this._nameSets = new List<NameSet>(); }
        public InfluenceSet(params NameSet[] args) { this._nameSets = new List<NameSet>(args); }
        public InfluenceSet(List<NameSet> namesets) { this._nameSets = new List<NameSet>(namesets); }

        protected List<NameSet> _nameSets;

        #region API

        public void AddNameSet(NameSet nameset)
        {
            if (!this._nameSets.Contains(nameset))
            {
                this._nameSets.Add(nameset);
            }
        }

        public bool RemoveNameSet(NameSet nameset)
        {
            return this._nameSets.Remove(nameset);
        }

        public string Synonym
        {
            get
            {
                NameSet set = null;
                do
                {
                    set = this._nameSets.TryGetRandom();
                }
                while (set.synonyms.Count == 0);

                return set.synonyms.TryGetRandom();
            }
        }

        public bool HasSynonym
        {
            get
            {
                foreach (var nameset in this._nameSets)
                {
                    if (nameset.synonyms.Count > 0)
                        return true;
                }
                return false;
            }
        }

        public string AdjectiveKey
        {
            get
            {
                NameSet set = null;
                do
                {
                    set = this._nameSets.TryGetRandom();
                }
                while (set.adjectiveKeys.Count == 0);

                return set.adjectiveKeys.TryGetRandom();
            }
        }

        public bool HasAdjectiveKey
        {
            get
            {
                foreach (var nameset in this._nameSets)
                {
                    if (nameset.adjectiveKeys.Count > 0)
                        return true;
                }
                return false;
            }
        }

        public string Prefix
        {
            get
            {
                NameSet set = null;
                do
                {
                    set = this._nameSets.TryGetRandom();
                }
                while (set.prefixes.Count == 0);

                return set.prefixes.TryGetRandom();
            }
        }

        public bool HasPrefix
        {
            get
            {
                foreach (var nameset in this._nameSets)
                {
                    if (nameset.prefixes.Count > 0)
                        return true;
                }
                return false;
            }
        }

        public string Sufix
        {
            get
            {
                NameSet set = null;
                do
                {
                    set = this._nameSets.TryGetRandom();
                }
                while (set.sufixes.Count == 0);

                return set.sufixes.TryGetRandom();
            }
        }

        public bool HasSufix
        {
            get
            {
                foreach (var nameset in this._nameSets)
                {
                    if (nameset.sufixes.Count > 0)
                        return true;
                }
                return false;
            }
        }

        public string Name
        {
            get
            {
                NameSet set = null;
                do
                {
                    set = this._nameSets.TryGetRandom();
                }
                while (set.names.Count == 0);

                return set.names.TryGetRandom();
            }
        }

        public bool HasName
        {
            get
            {
                foreach (var nameset in this._nameSets)
                {
                    if (nameset.names.Count > 0)
                        return true;
                }
                return false;
            }
        }

        public string Preset
        {
            get
            {
                NameSet set = null;
                do
                {
                    set = this._nameSets.TryGetRandom();
                }
                while (set.presets.Count == 0);

                return set.presets.TryGetRandom();
            }
        }

        public bool HasPreset
        {
            get
            {
                foreach (var nameset in this._nameSets)
                {
                    if (nameset.presets.Count > 0)
                        return true;
                }
                return false;
            }
        }

        #endregion

    }


    //[System.Serializable]
    //public class TitleSet
    //{
    //    public string id;
    //    public string opposingId;

    //    public List<string> adjectives = new List<string> ();

    //    public List<string> objectivePros = new List<string> ();
    //    public List<string> objectivesNeutral = new List<string> ();

    //    public List<string> subjectivesPros = new List<string> ();
    //    public List<string> subjectivesCons = new List<string> ();
    //    public List<string> subjectivesNeutral = new List<string> ();

    //    public List<string> genetives = new List<string> ();
    //}

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
        //public enum Type : int
        //{
        //    RemoveLeft,
        //    RemoveRight,
        //    ReplaceLeft,
        //    ReplaceRight,
        //    Insert,
        //    Append,
        //    MergeInto,
        //}

        //public char left;
        //public char right;
        //public string affix;
        //public Type type;
        public string replace, with;
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

        //ADJECTIVE_PREFIX = 5,
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

        public static ConstructionRule PresetRule
        {
            get
            {
                return new ConstructionRule
                {
                    name = "Preset",
                    instructions = new ConstructionInstruction[] { ConstructionInstruction.PRESET }
                };
            }
        }

        public static ConstructionRule AdjectivePrefixSynonymRule
        {
            get
            {
                return new ConstructionRule
                {
                    name = "AdjectivePrefix-Synonym",
                    instructions = new ConstructionInstruction[] { ConstructionInstruction.ADJECTIVE, ConstructionInstruction.SUBJECTIVE_ORIGINAL_OR_SYNONYM }
                };
            }
        }

        public static ConstructionRule AdjectiveSynonymRule
        {
            get
            {
                return new ConstructionRule
                {
                    name = "Adjective-Synonym",
                    instructions = new ConstructionInstruction[] { ConstructionInstruction.ADJECTIVE, ConstructionInstruction.SEPARATOR, ConstructionInstruction.SUBJECTIVE_ORIGINAL_OR_SYNONYM }
                };
            }
        }

        public static ConstructionRule SynonymGenetiveRule
        {
            get
            {
                return new ConstructionRule
                {
                    name = "Synonym-Genetive",
                    instructions = new ConstructionInstruction[] {
                        ConstructionInstruction.SUBJECTIVE_ORIGINAL_OR_SYNONYM,
                        ConstructionInstruction.SEPARATOR,
                        ConstructionInstruction.PREPOSITION_OF,
                        ConstructionInstruction.DETERMINER,
                        ConstructionInstruction.GENETIVE
                    }
                };
            }
        }

        public static ConstructionRule SynonymAdjectiveRule
        {
            get
            {
                return new ConstructionRule
                {
                    name = "Synonym-Adjective",
                    instructions = new ConstructionInstruction[] {
                        ConstructionInstruction.SUBJECTIVE_ORIGINAL_OR_SYNONYM,
                        ConstructionInstruction.SEPARATOR,
                        ConstructionInstruction.PREPOSITION_OF,
                        ConstructionInstruction.DETERMINER,
                        ConstructionInstruction.ADJECTIVE
                    }
                };
            }
        }

        public static ConstructionRule GenetiveSynonymRule
        {
            get
            {
                return new ConstructionRule
                {
                    name = "Synonym-Adjective",
                    instructions = new ConstructionInstruction[] {
                        ConstructionInstruction.GENETIVE,
                        ConstructionInstruction.POSSESSIVE_APOSTROPHE,
                        ConstructionInstruction.SEPARATOR,
                        ConstructionInstruction.SUBJECTIVE_ORIGINAL_OR_SYNONYM,
                    }
                };
            }
        }

    }

    [System.Serializable]
    public class SemanticConnection
    {
        public string noun;

        public List<string> adjectives;
    }

}