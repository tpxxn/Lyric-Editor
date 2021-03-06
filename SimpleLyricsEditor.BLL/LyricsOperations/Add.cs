﻿using System;
using System.Collections.Generic;
using SimpleLyricsEditor.DAL;

namespace SimpleLyricsEditor.BLL.LyricsOperations
{
    public class Add : LyricsOperationBase
    {
        /// <param name="time"></param>
        /// <param name="content"></param>
        /// <param name="insertIndex">为 -1 则添加到最后</param>
        /// <param name="isReverseAdd"></param>
        /// <param name="targetList"></param>
        public Add(TimeSpan time, string content, int insertIndex, bool isReverseAdd, IList<Lyric> targetList) : base(targetList)
        {
            Items = new List<Lyric>();
            InsertIndex = insertIndex;
            IsReverseAdd = isReverseAdd;

            foreach (var str in content.Split('\r'))
                Items.Add(new Lyric(time, str.Trim()));
        }

        public IList<Lyric> Items { get; set; }
        public int InsertIndex { get; set; }
        public bool IsReverseAdd { get; set; }

        public override void Do()
        {
            if (InsertIndex.Equals(-1))
            {
                foreach (var lyric in Items)
                {
                    if (!IsReverseAdd)
                        TargetList.Add(lyric);
                    else
                        TargetList.Insert(0, lyric);
                }
                return;
            }

            foreach (var lyric in Items)
                if (!IsReverseAdd)
                    TargetList.Insert(InsertIndex + 1, lyric);
                else
                    TargetList.Insert(InsertIndex, lyric);
        }

        public override void Undo()
        {
            foreach (var lyric in Items)
                TargetList.Remove(lyric);
        }
    }
}