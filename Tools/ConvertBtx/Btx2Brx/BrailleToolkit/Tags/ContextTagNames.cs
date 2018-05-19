﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleToolkit.Tags
{
    public static class ContextTagNames
    {
        public const string Title = "<標題>";
        public const string Indent = "<縮排>";
        public const string Math = "<數學>";
        public const string Coordinate = "<座標>";
        public const string Fraction = "<分數>";
        public const string Table = "<表格>";
        public const string TableTopLine1 = "<上表格線1>";
        public const string TableBottomLine1 = "<下表格線1>";
        public const string TableTopLine2 = "<上表格線2>";
        public const string TableBottomLine2 = "<下表格線2>";
        public const string Time = "<時間>";
        public const string Delete = "<刪>";
        public const string Phonetic = "<音標>";
        public const string SpecificName = "<私名號>";
        public const string BookName = "<書名號>";
        public const string BrailleTranslatorNote = "<點譯者註>";
        public const string OrgPageNumber = "<P>";
        public const string Choice = "<選項>"; // 選項裡面的 "ㄅ." 之間不空方，且小數點的點字為 6。
        public const string UpperPosition = "<上位點>"; // 數字一律使用上位點。

        // NOTE: 每當有變動時，必須同步修改 AllTagNames 的內容。

        public static HashSet<string> Collection =
            new HashSet<string>()
            {
                Title,
                Indent,
                Indent,
                Math,
                Coordinate,
                Fraction,
                Table,
                TableTopLine1,
                TableBottomLine1,
                TableTopLine2,
                TableBottomLine2,
                Time,
                Delete,
                Phonetic,
                SpecificName,
                BookName,
                BrailleTranslatorNote,
                OrgPageNumber,          // 原書頁碼
                Choice,
                UpperPosition
            };

      

        /// <summary>
        /// 檢查傳入的字串是否是以語境標籤（含起始及結束標籤）開頭。
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool StartsWithContextTag(string s)
        {
            string beginTag;
            string endTag;
            bool found = false;

            foreach (string tagName in Collection)
            {
                beginTag = tagName;
                endTag = beginTag.Insert(1, "/");
                if (s.StartsWith(beginTag) || s.StartsWith(endTag))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

    }
}
