using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using TCLRecruitmentBot.Dialogs;

namespace Poshelper
{
    public class PosHelper
    {
        static HashSet<string> skillHashSet = new HashSet<string>();
        static HashSet<string> locationHashset = new HashSet<string>();
        static Dictionary<int, string> PosDict = new Dictionary<int, string>();
        static SortedDictionary<int, string> resultDict = new SortedDictionary<int, string>();
        static List<string> EmailList = new List<string>();
        static List<string> removbleItemList = new List<string>();
        
        
        public static async Task<string> PosSplit(Istring Messagetext)
        {

            PosDict.Clear();
            EmailList.Clear();
            resultDict.Clear();
            skillHashSet.Clear();
            locationHashset.Clear();
            removbleItemList.Clear();
            bool statusValue;
            EmailList = await FindEmail(Messagetext);

            string[] pronouns = new string[] { "all", "another", "any", "anybody", "anyone", "anything", "both", "each", "each other", "either", "everybody", "everyone", "everything", "few", "he", "her", "hers", "herself", "him", "himself", "his", "I", "it", "its", "itself", "little", "many", "me", "mine", "more", "most", "much", "my", "myself", "neither", "no one", "nobody", "none", "nothing", "one", "one another", "other", "others", "our", "ours", "ourselves", "several", "she", "some", "somebody", "someone", "something", "that", "their", "theirs", "them", "themselves", "these", "they", "this", "those", "us", "we", "what", "whatever", "which", "whichever", "who", "whoever", "whom", "whomever", "whose", "you", "your", "yours", "yourself", "yourselves" };
            string[] adjectives = new string[] { "available", "average", "awesome", "actually", "acceptable", "bad", "bloody", "brave", "busy", "beautiful", "brief", "calm", "closed", "complete", "complex", "clear", "clever", "cool", "certain", "cut", "common", "daily", "dear", "detailed", "different", "deep", "difficult", "eager", "early", "easy", "empty", "equal", "equal", "even", "fabulous", "female", "funny", "free", "few", "full", "future", "foolish", "friendly", "fixed", "general", "good", "great", "happy", "hard", "high", "huge", "ill", "illegal", "intelligent", "internal", "interesting", "kindhearted", "kind", "known", "learned", "left", "level", "like", "last", "little", "living", "low", "loud", "lucky", "male", "mean", "many", "mute", "mixed", "narrow", "natural", "near", "necessary", "needy", "new", "next", "nice", "null", "numerous", "numberless", "normal", "odd", "old", "one", "open", "opposite", "outgoing", "outstanding", "painful", "pale", "parallel", "past", "perfect", "periodic", "plian", "pleasant", "polite", "poor", "possible", "powerful", "present", "previous", "private", "proud", "public", "quick", "quickly", "rapid", "rare", "ready", "regular", "relieved", "rich", "right", "rude", "sad", "safe", "same", "secret", "sharp", "short", "shut", "sick", "silent", "silly", "simple", "skillful", "starnge", "super", "sweet", "synonymous", "steady", "true", "ultra", "unable", "unknown", "used", "useful", "useless", "utter", "vast", "waiting", "weak", "whole", "wide", "willing", "wonderful", "wrong", "worried" };
            string[] skillSet = new string[] { "java", "ui", "others", "qa", "microsoft", "ruby", "mobility", "cpq", "ror", "python", "javascript", "asp.net", "android development", "manual and automation testing", "data sciences", "selenium", "devops", "linux", "design", "spotques", "j2ee", "ux", "mysql", "php", "sql server", "sql", "reactjs", "content writer", "content", "msbi", "wm", "ssrs", "wpf", "c++", "android", "c#" };
            string[] adverbs = new string[] { "quickly", "so", "well", "urgently", "loyally", "after", "aftrewards", "before", "annually", "daily", "never", "now", "soon", "still", "then", "today", "when", "away", "here", "in", "inside", "out", "there", "too", "very" };
            string[] MislenousWords = new string[] { "resources", "profile", "skills", "skilled", "skillset", "skill", "resume", "need", "employee", "download", "phone number", "number", "mail", "seat", "bay", "cv", "show", "hey", "hello", "file", "send", "want", "files", "profiles", "details", "see", "count", "list", "people", "folks", "guys", "get", "phonenumber", "Phonenumber", "phoneNumber" };
            string[] Location = new string[] { "hyderabad", "chennai", "bengaluru", "bangalore", "hyd", "london", "chen" };
            bool isempty = !EmailList.Any();
            if (isempty)
            {
                using (WebClient client = new WebClient())
                {
                    string x = $"http://posrest.azurewebsites.net/postag?query={Messagetext}";
                    var posrest = client.DownloadString(x);
                    Console.WriteLine(posrest);
                    //posrest.ToList();
                    //posrest=posrest.Replace("/","");
                    string[] y = posrest.Split('"');
                    List<string> list = new List<string>(y);
                    int i = 0;
                    foreach (string str in list)
                    {
                        PosDict.Add(i, str);
                        i++;
                    }
                    HashSet<string> pronounSet = new HashSet<string>(pronouns);
                    HashSet<string> adjectivSet = new HashSet<string>(adjectives);
                    HashSet<string> adverbSet = new HashSet<string>(adverbs);
                    HashSet<string> miscelnousSet = new HashSet<string>(MislenousWords);
                    HashSet<string> skillHashSet = new HashSet<string>();
                    HashSet<string> locationHashset = new HashSet<string>();
                    Task.Run(async () =>
                    {
                        if (list.Contains("NNP"))
                        {
                            statusValue = await Findwords(list, "NNP");
                        }
                        if (list.Contains("NNS"))
                        {
                            statusValue = await Findwords(list, "NNS");
                        }
                        if (list.Contains("NN"))
                        {
                            statusValue = await Findwords(list, "NN");
                        }
                        if (list.Contains("JJ"))
                        {
                            statusValue = await Findwords(list, "JJ");
                        }
                        if (list.Contains("FW"))
                        {
                            statusValue = await Findwords(list, "FW");
                        }
                        if (list.Contains("VBP"))
                        {
                            statusValue = await Findwords(list, "VBP");
                        }
                        if (list.Contains("VBD"))
                        {
                            statusValue = await Findwords(list, "VBD");
                        }
                        if (list.Contains("RBS"))
                        {
                            statusValue = await Findwords(list, "RBS");
                        }
                        if (list.Contains("VBZ"))
                        {
                            statusValue = await Findwords(list, "VBZ");
                        }
                        if (list.Contains("CD"))
                        {
                            statusValue = await Findwords(list, "CD");
                        }

                        statusValue = await FindPronoun(resultDict, pronounSet);
                        statusValue = await FindAdjectives(resultDict, adjectivSet);
                        statusValue = await FindAdverbs(resultDict, adverbSet);
                        statusValue = await FindSamplBags(resultDict, miscelnousSet);
                        //statusValue = await FindSkill(resultDict, skillset);
                    }).GetAwaiter().GetResult();
                }
            }
            else
            {
                foreach (string elist in EmailList)
                {
                    Console.WriteLine(elist);
                    resultDict.Add(1, elist);
                }
            }


            statusValue = await FindSkill(resultDict, skillSet);
            statusValue = await FindLocation(resultDict, Location);
            foreach (var remItem in resultDict.Where(p => removbleItemList.Contains(p.Value)).ToList())
            {
                resultDict.Remove(remItem.Key);
            }
            System.Text.StringBuilder a = new System.Text.StringBuilder();

            isempty = skillHashSet.Any();
            if (!isempty)
            {
                foreach (var item in skillHashSet)
                {

                    a.Append(" ");
                    a.Append(item);
                }
            }

            foreach (var item in resultDict)
            {

                a.Append(" ");
                a.Append(item.Value);
            }

            return a.ToString();
        }

        private async static Task<bool> Findwords(List<string> list, string str)
        {
            int index = 1;
            while (index != -1)
            {
                try
                {
                    index = list.IndexOf(str);
                    //list2.Add(list[index - 2]);
                    index = PosDict.FirstOrDefault(x => x.Value == str).Key;
                    resultDict.Add(index - 2, PosDict.FirstOrDefault(x => x.Key == (index - 2)).Value);
                    PosDict.Remove(PosDict.FirstOrDefault(x => x.Key == (index - 2)).Key);
                    PosDict.Remove(PosDict.FirstOrDefault(x => x.Key == (index)).Key);

                    list.RemoveAt(index - 2);
                    list.RemoveAt(index - 1);
                    ///Console.WriteLine(list);
                }
                catch (Exception e)
                {
                    index = -1;
                }
            }
            return true;
        }

        private async static Task<bool> FindPronoun(SortedDictionary<int, string> list, HashSet<string> hashSet)
        {
            foreach (var item in resultDict)
            {
                if (hashSet.Contains(item.Value))
                {
                    removbleItemList.Add(item.Value);
                }
            }
            return true;
        }
        private async static Task<bool> FindAdjectives(SortedDictionary<int, string> resultDict, HashSet<string> hashSet)
        {
            foreach (var item in resultDict)
            {
                if (hashSet.Contains(item.Value))
                {
                    removbleItemList.Add(item.Value);
                }
            }
            return true;
        }
        private async static Task<bool> FindAdverbs(SortedDictionary<int, string> resultDict, HashSet<string> hashSet)
        {
            foreach (var item in resultDict)
            {
                if (hashSet.Contains(item.Value))
                {
                    removbleItemList.Add(item.Value);
                }
            }
            return true;
        }
        private async static Task<bool> FindSamplBags(SortedDictionary<int, string> resultDict, HashSet<string> hashSet)
        {
            foreach (var item in resultDict)
            {
                if (hashSet.Contains(item.Value))
                {
                    removbleItemList.Add(item.Value);
                }
            }
            return true;
        }
        private async static Task<List<string>> FindEmail(string str)
        {
            List<string> EmailList = new List<string>();
            Regex emailregex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
            MatchCollection emailmatches = emailregex.Matches(str);
            foreach (Match emailMatch in emailmatches)
            {
                EmailList.Add(emailMatch.Value);
            }
            return EmailList;
        }
        private async static Task<bool> FindSkill(SortedDictionary<int, string> resultDict, string[] str)
        {
            foreach (var item in resultDict)
            {
                if (str.Contains(item.Value))
                {
                    ProfileFetchDialog.skillStatus = true;
                    PhoneBookDialog.skillvalue = item.Value;

                    removbleItemList.Add(item.Value);
                    skillHashSet.Add(item.Value);
                }
            }
            return true;
        }

        private async static Task<bool> FindLocation(SortedDictionary<int, string> resultDict, string[] str)
        {
            foreach (var item in resultDict)
            {
                if (item.Value == null)
                {
                    continue;
                }
                else if (str.Contains(item.Value.ToLower()))
                {
                    ProfileFetchDialog.locationstatus = true;
                    PhoneBookDialog.location = item.Value;
                    removbleItemList.Add(item.Value);
                    locationHashset.Add(item.Value);
                }
            }
            return true;
        }
    }

}