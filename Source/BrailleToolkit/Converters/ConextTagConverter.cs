﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using BrailleToolkit.Data;

namespace BrailleToolkit.Converters
{
    /// <summary>
    /// 情境標籤轉換器。
    /// 
    /// NOTE: 此轉換器必須是第一個呼叫的轉換器。因為它會把情境標籤先處理掉，
    /// 這樣的話，後續的轉換器在碰到半形的小於、大於符號時，就可以當作是一般的
    /// 字元處理（由 EnglishBrailleConverter 處理）。
    /// </summary>
    public sealed class ContextTagConverter : WordConverter
    {
        public ContextTagConverter()
            : base()
        {
        }

        public override string Convert(string text)
        {
            throw new Exception("Not Implemented!");
        }

        public override List<BrailleWord> Convert(Stack<char> charStack, ContextTagManager context)
        {
            if (charStack.Count < 1)
                throw new ArgumentException("傳入空的字元堆疊!");

            char ch = charStack.Peek();

            if (ch != '<')
                return null;

            List<BrailleWord> brWordList = null; 

            char[] charBuf = charStack.ToArray();
            string s = new string(charBuf);
            bool isBeginTag;

			// 剖析字串是否為情境標籤，是則"進入"該情境標籤。
            ContextTag ctag = context.Parse(s, out isBeginTag);	

            if (ctag != null)
            {
                // 控制字（語境字）

                // var brWord = ctag.ToBrailleWord(isBeginTag);
                var brWord = new BrailleWord();

                brWord.Text = isBeginTag ? ctag.TagName : ctag.EndTagName;
                brWord.ContextTag = ctag;
                brWord.IsContextTag = true;
                brWord.ContextNames = context.ContextNames;

                brWordList = new List<BrailleWord>();
                brWordList.Add(brWord);


                // 將此標籤從堆疊中移除。
                for (int i = 0; i < brWord.Text.Length; i++)
                {
                    charStack.Pop();
                }
            }
            return brWordList;
        }

        internal override BrailleTableBase BrailleTable
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }
    }
}
