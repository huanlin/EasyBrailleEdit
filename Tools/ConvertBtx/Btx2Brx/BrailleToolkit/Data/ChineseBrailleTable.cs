using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

namespace BrailleToolkit.Data
{
	internal sealed class ChineseBrailleTable : XmlBrailleTable
	{
        private static ChineseBrailleTable m_Instance = null;

        private ChineseBrailleTable()
            : base()
        {
        }

        // ���}��o�� method
		private ChineseBrailleTable(string filename) : base(filename)
		{
		}

        /// <summary>
        /// �Ǧ^ singleton ����A�ø��J�귽�C
        /// </summary>
        /// <returns></returns>
        public static ChineseBrailleTable GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new ChineseBrailleTable();
                m_Instance.LoadFromResource();
            }
            return m_Instance;
        }

		/// <summary>
		/// �j�M�Y�Ӫ`���Ÿ��A�öǦ^�������I�r�X�C
		/// </summary>
		/// <param name="text">���j�M���`���Ÿ��C�Ҧp�G"�t"�C</param>
		/// <returns>�Y�����A�h�Ǧ^�������I�r�X�A�_�h�Ǧ^�Ŧr��C</returns>
		public string GetPhoneticCode(string text)
		{
			CheckLoaded();

			string filter = "type='Phonetic' and text='" + text + "'";
			DataRow[] rows = m_Table.Select(filter);
			if (rows.Length > 0)
				return rows[0]["code"].ToString();
			return null;
		}

		/// <summary>
		/// �M�䵲�X���A�öǦ^�������I�r�X�C
		/// </summary>
		/// <param name="text">���X�����`���Ÿ��A���t�n�աC�Ҧp "����"�C</param>
		/// <returns>�Y�O���X���A�h�Ǧ^�������I�r�X�A�_�h�Ǧ^�Ŧr��C</returns>
		public string GetPhoneticJoinedCode(string text)
		{
			CheckLoaded();

			string filter = "type='Phonetic' and joined=true and text='" + text + "'";
			DataRow[] rows = m_Table.Select(filter);
			if (rows.Length > 0)
				return rows[0]["code"].ToString();
			return null;
		}

		/// <summary>
		/// �M��`���Ÿ����C�ӯS��歵�]���B���B���B���B���B���B���^�C
		/// </summary>
		/// <param name="text">�Y�ӳ歵�`���Ÿ��A�Ҧp "��"�C
		/// <returns>�Y�O�S��歵�r�A�h�Ǧ^�������I�r�X�A�_�h�Ǧ^�Ŧr��C</returns>
		public string GetPhoneticMonoCode(string text)
		{
			CheckLoaded();

			string filter = "type='Phonetic' and mono=true and text='" + text + "'";
			DataRow[] rows = m_Table.Select(filter);
			if (rows.Length > 0)
				return rows[0]["code"].ToString();
			return null;
		}

		/// <summary>
		/// �M��`�����n�ղŸ��C
		/// </summary>
		/// <param name="text">���M�䪺�n�ղŸ��A���ΪťեN��@�n�C</param>
		/// <returns>�Y�����A�h�Ǧ^�������I�r�X�A�_�h�Ǧ^�Ŧr��C</returns>
		public string GetPhoneticToneCode(string text)
		{
			CheckLoaded();

			string filter = "type='Tone' and text='" + text + "'";
			DataRow[] rows = m_Table.Select(filter);
			if (rows.Length > 0)
				return rows[0]["code"].ToString();
			return null;
		}

		/// <summary>
		/// �M����I�Ÿ��C
		/// </summary>
		/// <param name="text">���M�䪺���I�Ÿ��C</param>
		/// <returns>�Y�����A�h�Ǧ^�������I�r�X�A�_�h�Ǧ^�Ŧr��C</returns>
		public string GetPunctuationCode(string text)
		{
			CheckLoaded();

            // �ץ���޸��G�b SQL �d�߱��󤤪���޸������s����
            if ("'".Equals(text))
            {
                text = "''";
            }
			string filter = "type='Punctuation' and text='" + text + "'";
			DataRow[] rows = m_Table.Select(filter);
			if (rows.Length > 0)
				return rows[0]["code"].ToString();
			return null;
		}

        public string GetAllPunctuations()
        {
            CheckLoaded();

            string filter = "type='Punctuation'";
            DataRow[] rows = m_Table.Select(filter);
            var sb = new StringBuilder();
            foreach (var row in rows)
            {
                sb.Append(row["text"]).ToString();
            }
            return sb.ToString();
        }
	}
}
