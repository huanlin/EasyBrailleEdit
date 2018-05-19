using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using BrailleToolkit.Tags;
using BrailleToolkit.Data;
using Huanlin.Common.Helpers;

namespace BrailleToolkit
{
    public enum BrailleLanguage
    {
        Neutral = 0,
        Chinese,
        English
    };


    /// <summary>
    /// �N��@�Ӥ���r�A���t�`���X�P�I�r�ȡC
    /// </summary>
    [Serializable]
    [DataContract]
    public class BrailleWord
    {
        public static BrailleWord BlankWord { get; } = NewBlank();

        private List<string> m_PhoneticCodes;   // �Ҧ��`���զr�r�ڡ]�H�䴩�}���r�^�C
        private int m_ActivePhoneticIndex;      // �ثe�ϥΪ��`���զr�r�گ��ޡC

        [NonSerialized]
        private string m_PhoneticCode;          // �`���r�ڡ]�t�u�v�w�^�C

        [NonSerialized]
        private bool m_IsPolyphonic;            // �O�_���h���r�C

        [NonSerialized]
        private bool m_IsContextTag;

        [NonSerialized]
        private bool m_NoDigitCell;             // �O�_���[�ƲšC

        [NonSerialized]
        private bool m_IsEngPhonetic;			// �O�_���^�y���С]�ΨӧP�_���n�[�Ť�^.

        //private bool m_QuotationResolved;	// �O�_�w�g�ѧO�X���k�޸��]�^�媺��޸��M���޸����O�P�@�ӲŸ��A���I�r���P�^

        public BrailleWord(string text)
        {
            Text = text;
            OriginalText = text;

            Language = BrailleLanguage.Neutral;
            CellList = new BrailleCellList();

            m_PhoneticCodes = new List<string>();
            m_ActivePhoneticIndex = -1;

            DontBreakLineHere = false;
            ContextNames = String.Empty;

            m_IsContextTag = false;
            m_NoDigitCell = false;
            m_IsEngPhonetic = false;
        }

        public BrailleWord(string text, BrailleCellCode brCode) : this(text)
        {
            CellList.Add(BrailleCell.GetInstance(brCode));
        }

        public BrailleWord(string text, string brCode) : this(text)
        {
            AddCells(brCode);
        }

        public BrailleWord(string text, byte brCode) : this(text)
        {
            CellList.Add(BrailleCell.GetInstance(brCode));
        }

        public BrailleWord(string text, string phCode, string brCode) : this(text, brCode)
        {
            Language = BrailleLanguage.Chinese;
            m_PhoneticCodes.Add(phCode);
            m_ActivePhoneticIndex = 0;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            BrailleWord brWord = (BrailleWord)obj;

            if (CellList.Count != brWord.Cells.Count)
                return false;
            for (int i = 0; i < CellList.Count; i++)
            {				
                if (!CellList[i].Equals(brWord.Cells[i]))
                    return false;
            }

            // �����`���A�]���@�n�`�`�M���Ϊťշd�V�C
            //if (m_PhoneticCode != brWord.PhoneticCode)
            //    return false;

            // ������r�A�]�����ΪťթM�b�Ϊťը���������۵��A��� cells �N���F�C
            //if (m_Text != brWord.Text)
            //
            //{
            //    return false;
            //}

            return true;
        }

        public override string ToString()
        {
            return Text;
        }

        public string ToPositionNumberString(bool useParenthesis)
        {
            if (IsContextTag)
            {
                return String.Empty;
            }

            var sb = new StringBuilder();
            if (useParenthesis)
                sb.Append("(");
            foreach (var cell in Cells)
            {
                sb.Append(cell.ToPositionNumberString());
                sb.Append(" ");
            }
            var result = sb.ToString().TrimEnd();
            if (useParenthesis)
                result += ")";
            return result;
        }

        public string ToHexSting()
        {
            var sb = new StringBuilder();
            foreach (var cell in Cells)
            {
                sb.Append(cell.ToHexString());
            }
            return sb.ToString();
        }

        public void Clear()
        {
            Text = String.Empty;
            CellList.Clear();
            ContextTag = null;
            ContextNames = String.Empty;
        }

        /// <summary>
        /// ��ܤ�r�C�i��O�@�ӭ^�Ʀr�B����r�B���μ��I�Ÿ��B�����r�����I�Ÿ��A�Ҧp�G�}�鸹�C
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// �O�d�̪쪺��r�C
        /// ���ݩʥi�ΨӧP�_��e�� BrailleWord �O���O�q context tag ���_�l�ε��������ഫ�Ӧ��C
        /// �ƦܱN�ӥi��Q�Φ��ݩʱN�w�g�ഫ�n���I�r����٭즨�¤�r�C
        /// </summary>
        [DataMember]
        public string OriginalText { get; private set; }

        public int CellCount
        {
            get { return CellList.Count; }
        }
        
        public List<BrailleCell> Cells
        {
            get
            {
                return CellList.Items;
            }
        }

        /// <summary>
        /// �I�r��C�C
        /// </summary>
        [DataMember]
        public BrailleCellList CellList { get; set; }

        [DataMember]
        public string PhoneticCode
        {
            get
            {
                if (String.IsNullOrEmpty(m_PhoneticCode))   // �o�O���F�V�U�ۮe�A�ª��S�� PhoneticCode �ݩʡC
                {
                    // �Y�S���`���r�ڡA�h�Ǧ^�Ŧr��C
                    if (m_PhoneticCodes == null || m_PhoneticCodes.Count < 1 || m_ActivePhoneticIndex < 0)
                    {
                        return "";
                    }
                    return m_PhoneticCodes[m_ActivePhoneticIndex];
                }
                return m_PhoneticCode;
            }
            set
            {
                if (m_PhoneticCode == value)
                    return;
                m_PhoneticCode = value;
/*
                // �Y�S���`���r�ڡA�h�W�[�@�ӡC
                if (m_PhoneticCodes.Count < 1)
                {
                    m_PhoneticCodes.Add(value);
                    m_ActivePhoneticIndex = 0;
                }
                else
                {   // �_�h�]�w�@�Τ����`���r�گ���
                    int i = m_PhoneticCodes.IndexOf(value);
                    if (i < 0)
                    {
                        m_PhoneticCodes.Add(value);
                        i = m_PhoneticCodes.Count - 1;
                        System.Diagnostics.Trace.WriteLine("���w�� BrailleWord.PhoneticCode ���`���r�ڤ��s�b! �w�۰ʥ[�J�C");
                    }
                    m_ActivePhoneticIndex = i;
                }
*/
            }
        }

        [DataMember]
        public bool IsPolyphonic
        {
            get 
            {
                if (m_PhoneticCodes != null && m_PhoneticCodes.Count > 1)   // for �V�U�ۮe.
                {
                    return true;
                }
                return m_IsPolyphonic; 
            }
            set { m_IsPolyphonic = value; }
        }

        /// <summary>
        /// �]�w/�P�O�b�_��ɬO�_���_�b�o�Ӧr�C
        /// </summary>
        [DataMember]
        public bool DontBreakLineHere { get; set; }

        public List<string> PhoneticCodes
        {
            get
            {
                if (m_PhoneticCodes == null)
                {
                    m_PhoneticCodes = new List<string>();
                }
                return m_PhoneticCodes;
            }
        }

        public int ActivePhoneticIndex
        {
            get
            {
                return m_ActivePhoneticIndex;
            }
            set
            {
                if (value >= m_PhoneticCodes.Count)
                    throw new ArgumentOutOfRangeException();
                m_ActivePhoneticIndex = value;
            }
        }

        /// <summary>
        /// �y����O�C�Ψ��ѧO�O�����٬O�^��C
        /// </summary>
        public BrailleLanguage Language { get; set; }

        /// <summary>
        /// �H�ťհϹj�� context �W�١A���t tag �����A���C�Ҧp "�ƾ� �p�W��"�C
        /// </summary>
        [DataMember]
        public string ContextNames { get; set; } 

        /// <summary>
        /// ContextTag �ݩʷ|�Φb�ഫ�I�r���L�{���ȮɫO�d���y�Ҽ��ҡC
        /// �o�ǻy�Ҽ��Ҧb������I�r�{�ǧ����ɳ��|�Q�����]���ഫ���������I�r�^�C
        /// �O�_�i�ǦC�ơG�_�C
        /// </summary>
        public IContextTag ContextTag { get; set; }

        public void SetPhoneticCodes(string[] phCodes)
        {
            m_PhoneticCodes.Clear();
            m_PhoneticCodes.AddRange(phCodes);
        }

        /// <summary>
        /// �إߤ@�ӷs�� BrailleWord ����A�ñN�ۤv�����e����ƻs��s������C
        /// </summary>
        /// <returns></returns>
        public BrailleWord Copy()
        {
            BrailleWord newBrWord = new BrailleWord(Text);
            newBrWord.Language = Language;
            newBrWord.DontBreakLineHere = DontBreakLineHere;
            newBrWord.NoDigitCell = m_NoDigitCell;
            newBrWord.PhoneticCode = PhoneticCode;
            newBrWord.IsPolyphonic = IsPolyphonic;
            newBrWord.IsContextTag = IsContextTag;
            newBrWord.IsConvertedFromTag = IsConvertedFromTag;
            newBrWord.ContextTag = ContextTag;
            newBrWord.ContextNames = ContextNames;

            foreach (BrailleCell brCell in CellList.Items)
            {
                newBrWord.Cells.Add(brCell);
            }

            return newBrWord;
        }

        /// <summary>
        /// �N���w�� BrailleWord ���e����ƻs���ۤv�C
        /// </summary>
        /// <param name="brWord"></param>
        public void Copy(BrailleWord brWord)
        {
            if (brWord == null)
            {
                throw new ArgumentNullException("�Ѽ� brWord ���i�� null!");
            }

            Text = brWord.Text;
            Language = brWord.Language;

            CellList.Clear();
            foreach (BrailleCell brCell in brWord.CellList.Items)
            {
                CellList.Add(brCell);
            }

            PhoneticCode = brWord.PhoneticCode;
            IsPolyphonic = brWord.IsPolyphonic;
            IsContextTag = brWord.IsContextTag;
            IsConvertedFromTag = brWord.IsConvertedFromTag;
            ContextTag = brWord.ContextTag;
            ContextNames = brWord.ContextNames;
/*
            // �ƻs�Ҧ��`���r�ڻP�I�r��C, for �V�U�ۮe.
            if (brWord.PhoneticCodes != null)
            {
                m_PhoneticCodes.Clear();
                m_PhoneticCodes.AddRange(brWord.PhoneticCodes);
                m_ActivePhoneticIndex = brWord.ActivePhoneticIndex;
            }
 */
        }

        /// <summary>
        /// ����w���I�r�r��]16�i��^�ন BrailleCell ����A�å[�J�I�r��C���C
        /// </summary>
        /// <param name="brCodes">���[�J��C���I�r�X 16 �i��r��C</param>
        public void AddCells(string brCodes)
        {
            if (String.IsNullOrEmpty(brCodes))
            {
                return;
            }

            for (int i = 0; i < brCodes.Length; i += 2)
            {
                string s = brCodes.Substring(i, 2);
                byte aByte = StrHelper.HexStrToByte(s);
                BrailleCell cell = BrailleCell.GetInstance(aByte);
                CellList.Add(cell);
            }
        }

        public void AddCellsFromPositionNumbers(string positionNumberString)
        {
            if (String.IsNullOrEmpty(positionNumberString))
                return;
            var numbers = positionNumberString.Split(' ');
            foreach (string num in numbers)
            {
                var cell = BrailleCell.GetInstanceFromPositionNumberString(num);
                CellList.Add(cell);
            }
        }

        public bool IsWhiteSpace
        {
            get
            {
                if (Text.Length == 1)
                {
                    if (Char.IsWhiteSpace(Text[0]))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsLetter
        {
            get
            {
                if (Text.Length == 1)
                {
                    if (CharHelper.IsAsciiLetter(Text[0]))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsDigit
        {
            get
            {
                if (Text.Length == 1)
                {
                    if (CharHelper.IsAsciiDigit(Text[0]))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsLetterOrDigit
        {
            get
            {
                if (Text.Length == 1)
                {
                    if (CharHelper.IsAsciiLetterOrDigit(Text[0]))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// �إߤ@�ӷs���Ť��I�r����C
        /// </summary>
        /// <returns></returns>
        public static BrailleWord NewBlank()
        {
            return new BrailleWord(" ", BrailleCellCode.Blank);
        }

        /// <summary>
        /// �ˬd���w�� BrailleWord �O�_���Ť�C
        /// </summary>
        /// <param name="brWord"></param>
        /// <returns></returns>
        public static bool IsBlank(BrailleWord brWord)
        {
            if (brWord.Equals(BlankWord))
                return true;
            return false;
        }

        /// <summary>
        /// �ˬd���w�� BrailleWord ����O�_�S���]�t���󦳷N�q����ơ]�Ť�⦳�N�q����ơ^�C
        /// </summary>
        /// <param name="brWord"></param>
        /// <returns></returns>
        public static bool IsEmpty(BrailleWord brWord)
        {
            if (StrHelper.IsEmpty(brWord.Text) && brWord.CellCount < 1) 
            {
                // ��r���Ŧr��A�B�S�������I�r����A�Y�����Ū� BrailleWord ����.
                return true;
            }
            return false;
        }

        /// <summary>
        /// �O�_���y�Ҽ��ҡC���ݩʻP ContextNames �S���������Y�C
        /// �Y�� true�A�N�� BraillWord �b��l��󤤬O�@�ӻy�Ҽ��ҡA�ӥB�ثe�٨S���ഫ���������I�r�C
        /// �Y�� false�A�N�� BraillWord ���O�y�Ҽ��ҡA�Ϊ̤w�g�Q�ഫ���������I�r�C
        /// </summary>
        [DataMember]
        public bool IsContextTag
        {
            get { return m_IsContextTag; }
            set
            {
                m_IsContextTag = value;
                if (m_IsContextTag)    // �p�G�O�y�Ҽ��ҡA�N�n�M���I�r��C
                {
                    CellList.Clear();
                }
                else
                {
                    ContextTag = null; // �Y���O�y�Ҽ��ҡA���M�� ContextTag �Ѧ�
                }
            }
        }

        /// <summary>
        ///  ������O�_�� context tag �ҭl�͡]���O context tag�A���O�]�� context tag ���B�~�W�[����r�^�C
        /// </summary>
        [DataMember]
        public bool IsConvertedFromTag { get; set; }

        public bool NoDigitCell
        {
            get { return m_NoDigitCell; }
            set { m_NoDigitCell = value; }
        }

        public bool IsEngPhonetic
        {
            get { return m_IsEngPhonetic; }
            set { m_IsEngPhonetic = value; }
        }

        /// <summary>
        /// �ˬd���w�� BrailleWord �O�_���Ʀr�s�����_�l�I�r�C��Y�H # �}�Y���Ʀr�C
        /// </summary>
        /// <param name="brWord"></param>
        /// <returns></returns>
        public static bool IsOrderedListItem(BrailleWord brWord)
        {
            if (brWord.Cells.Count < 2)
                return false;
            if (brWord.Cells[0].Value == (byte)BrailleCellCode.Digit) // �H�Ʀr�I�}�Y�H
            {
                // ���ۤ���ĤG��O�_���W���I�A�`�N�o�̪��W���I�ƭȨå��ϥάd��A
                // �ӬO�g���b�{���̡C�Ѧ�: BraillTableEng.xml�C
                // TODO: �令�d��C
                byte value = brWord.Cells[1].Value;
                switch (value) 
                {
                    case 0x01:  case 0x03:
                    case 0x09:  case 0x19:
                    case 0x11:  case 0x0B:
                    case 0x1B:  case 0x13:
                    case 0x4A:  case 0x1A:
                        return true;
                    default:
                        break;
                }
            }
            return false;
        }
    }
}
