using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BrailleToolkit.Tags;

namespace BrailleToolkit
{
    /// <summary>
    /// �Ψ��x�s�@�C�I�r�C
    /// </summary>
    [Serializable]
    [DataContract]
    public class BrailleLine : ICloneable
    {
        public BrailleLine()
        {
            Words = new List<BrailleWord>();
        }

        public void Clear()
        {
            Words.Clear();
        }

        public bool IsEmpty()
        {
            return WordCount < 1;
        }

        public bool IsEmptyOrWhiteSpace()
        {
            foreach (var word in Words)
            {
                if (!BrailleWord.IsBlank(word) && !BrailleWord.IsEmpty(word))
                {
                    return false;
                }

            }
            return true;
        }

        public bool IsBeginOfParagraph()
        {
            if (WordCount >= 2)
            {
                if (Words[0].IsWhiteSpace && Words[1].IsWhiteSpace)
                {
                    return true;
                }
            }
            return false;
        }

        [DataMember]
        public List<BrailleWord> Words { get; private set; }

        public int WordCount
        {
            get { return Words.Count; }
        }

        public BrailleWord this[int index]
        {
            get
            {
                return Words[index];
            }
        }

        /// <summary>
        /// �Ǧ^�Ҧ��I�r���`��ơC
        /// </summary>
        public int CellCount
        {
            get
            {
                int cnt = 0;
                foreach (BrailleWord brWord in Words)
                {
                    cnt += brWord.Cells.Count;
                }
                return cnt;
            }
        }

        /// <summary>
        /// ���o����C�����Ҧ��� BraillCell ����C
        /// </summary>
        /// <returns></returns>
        public List<BrailleCell> GetBrailleCells()
        {
            var list = new List<BrailleCell>();
            foreach (var brWord in Words)
            {
                list.AddRange(brWord.Cells);
            }
            return list;
        }

        /// <summary>
        /// �p���_�檺�I�r���ަ�m�C
        /// ���B�ȮھڶǤJ���̤j��ƨӭp��i�_�檺�I�r���ޡA�å��[�J��L�_��W�h���P�_�C
        /// </summary>
        /// <param name="cellsPerLine">�@��i���\�h�֤�ơC</param>
        /// <returns>�i�_�檺�I�r���ޡC�Ҧp�A�Y���޽s���� 29 �Ӧr�]0-based�^�������U�@��A
        /// �Ǧ^�ȴN�O 29�C�Y���ݭn�_��A�h�Ǧ^��檺�r�ơC</returns>
        public int CalcBreakPoint(int cellsPerLine)
        {
            if (cellsPerLine < 4)
            {
                throw new ArgumentException("cellsPerLine �ѼƭȤ��i�p�� 4�C");
            }

            int cellCnt = 0;
            int index = 0;
            while (index < Words.Count)
            {
                cellCnt += Words[index].Cells.Count;
                if (cellCnt > cellsPerLine)
                {
                    break;
                }
                index++;
            }
            return index;
        }

        /// <summary>
        /// �q���w���_�l��m�ƻs���w�Ӽƪ��I�r (BrailleWord) ��s�إߪ��I�r��C�C
        /// </summary>
        /// <param name="index">�_�l��m</param>
        /// <param name="count">�n�ƻs�X���I�r�C</param>
        /// <returns>�s���I�r��C�C</returns>
        public BrailleLine Copy(int index, int count)
        {
            BrailleLine brLine = new BrailleLine();
            BrailleWord newWord = null;
            while (index < Words.Count && count > 0)
            {
                //newWord = Words[index].Copy();
                newWord = Words[index]; 
                brLine.Words.Add(newWord);

                index++;
                count--;

            }
            return brLine;
        }

        public void RemoveAt(int index)
        {
            Words.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            if ((index + count) > Words.Count)    // ����n�����ƶq�W�X��ɡC
            {
                count = Words.Count - index;
            }
            Words.RemoveRange(index, count);
        }

        /// <summary>
        /// �N���w���I�r�C���[�ܦ��I�r�C�C
        /// </summary>
        /// <param name="brLine"></param>
        public void Append(BrailleLine brLine)
        {
            if (brLine == null || brLine.WordCount < 1)
                return;

            Words.AddRange(brLine.Words);
        }

        public void Insert(int index, BrailleWord brWord)
        {
            Words.Insert(index, brWord);
        }

        /// <summary>
        /// �h���}�Y���ťզr���C
        /// </summary>
        public void TrimStart()
        {
            int i = 0;
            while (i < Words.Count)
            {
                if (BrailleWord.IsBlank(Words[i]) || BrailleWord.IsEmpty(Words[i]))
                {
                    Words.RemoveAt(i);
                    continue;
                }
                break;
            }
        }

        /// <summary>
        /// �h���������ťզr���C
        /// </summary>
        public void TrimEnd()
        {
            int i = Words.Count - 1;
            while (i >= 0)
            {
                if (BrailleWord.IsBlank(Words[i]) || BrailleWord.IsEmpty(Words[i]))
                {
                    Words.RemoveAt(i);
                    i--;
                    continue;
                }
                break;
            }
        }

        /// <summary>
        /// ���Y�����ťեh���C
        /// </summary>
        public void Trim()
        {
            TrimStart();
            TrimEnd();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (BrailleWord brWord in Words)
            {
                sb.Append(brWord.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// �N����C�����Ҧ��I�r�ন 16 �i�쪺�r��C
        /// </summary>
        /// <returns></returns>
        public string ToBrailleCellHexString()
        {
            var sb = new StringBuilder();
            foreach (var brWord in Words)
            {
                foreach (var cell in brWord.Cells)
                {
                    sb.Append(cell.ToHexString());
                }                
            }
            return sb.ToString();
        }

        /// <summary>
        /// �N����C�����Ҧ��I�r�ন�H�I��զ����r��C�U�I�r�H�@�Ӫťզr���j�}�C
        /// </summary>
        /// <returns></returns>
        public string ToPositionNumberString()
        {
            var sb = new StringBuilder();
            foreach (var brWord in Words)
            {
                sb.Append(brWord.ToPositionNumberString(useParenthesis: true));
            }
            return sb.ToString();
        }

        /// <summary>
        /// �O�_�]�t���D���Ҽ��ҡC
        /// </summary>
        /// <returns></returns>
        public bool ContainsTitleTag()
        {
            if (Words.Count > 0 && Words[0].Text.Equals(ContextTagNames.Title))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// �����Ҧ����Ҽ��ҡC
        /// </summary>
        public void RemoveContextTags()
        {
            BrailleWord brWord;

            for (int i = WordCount - 1; i >= 0; i--)
            {
                brWord = Words[i];
                if (brWord.IsContextTag)
                {
                    Words.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// �b��C���M����w���r��A�q��C������ startIndex �Ӧr�}�l��_�C
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public int IndexOf(string value, int startIndex, StringComparison comparisonType)
        {
            if (startIndex + value.Length > this.WordCount)
            {
                return -1;
            }

            int i;
            StringBuilder sb = new StringBuilder();
            for (i = startIndex; i < this.WordCount; i++)
            {
                sb.Append(Words[i].Text);
            }

            int idx = sb.ToString().IndexOf(value, comparisonType);
            if (idx < 0)
            {
                return -1;
            }

            // �����A���o�O�r�����ޡA�٥����ץ����I�r���ޡC
            for (i = startIndex; i < this.WordCount; i++)
            {
                idx = idx - Words[i].Text.Length + 1;
            }

            return startIndex + idx;
        }

        #region ICloneable Members

        /// <summary>
        /// �`�h�ƻs�C
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            BrailleLine brLine = new BrailleLine();
            BrailleWord newWord = null;

            foreach (BrailleWord brWord in Words)
            {
                newWord = brWord.Copy();
                brLine.Words.Add(newWord);
            }
            return brLine;
        }

        #endregion
    }
}
